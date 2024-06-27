using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace GetVersionInfoUWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool IsWow64Process2(
                                IntPtr process,
                                out ushort processMachine,
                                out ushort nativeMachine
                                );
        [DllImport("kernel32.dll")]
        private static extern IntPtr GetCurrentProcess();

        public struct PROCESS_MACHINE_INFORMATION
        {
            public ushort ProcessMachine;
            public ushort Res0;
            public uint MachineAttributes;
        };


        [DllImport("kernel32.dll")]
        private static extern bool GetProcessInformation(
                                        IntPtr hProcess,
                                        int ProcessInformationClass,
                                        out PROCESS_MACHINE_INFORMATION ProcessInformation,
                                        int ProcessInformationSize);


        private void myButton_Click(object sender, RoutedEventArgs e)
        {
            myButton.Content = GetWaRunningArchitecture();
        }

        public static string GetWaRunningArchitecture()
        {
            var process = GetCurrentProcess();
            if (!IsWow64Process2(
                    process,
                    out var machine,
                    out var nativeMachine))
            {
                return "Unknown";
            }

            /* However, x86_64 on ARM64 claims not to be WOW64, so we have to
             * dig harder... */
            //#define ProcessMachineTypeInfo 

            if (GetProcessInformation(
                    process,
                    9,
                    out var processInfo,
                    8))
            {
                machine = processInfo.ProcessMachine;
            }

            switch (nativeMachine)
            {
                case 0xAA64 when machine == 0x8664: return "EmulatedX64";
                case 0xAA64 when machine == 0xAA64: return "NativeArm64";
                case 0xAA64 when machine == 0x014C: return "EmulatedX86";
                case 0x8664 when machine == 0x8664: return "NativeX64";
                default: return "Unknown";
            }
        }

        public static string GetRunningArchitecture()
        {

            switch (System.Runtime.InteropServices.RuntimeInformation.ProcessArchitecture)
            {
                case Architecture.Arm64:
                    return "NativeArm64";
                case Architecture.X64:
                    if (System.Runtime.InteropServices.RuntimeInformation.OSArchitecture != Architecture.X64)
                        return "EmulatedX64";
                    return "NativeX64";

                default: return $"Unknown ({System.Runtime.InteropServices.RuntimeInformation.OSArchitecture}, {System.Runtime.InteropServices.RuntimeInformation.ProcessArchitecture}";
            }

        }

        private void myButton2_Click(object sender, RoutedEventArgs e)
        {
            myButton2.Content = GetRunningArchitecture();
        }



    }
}
