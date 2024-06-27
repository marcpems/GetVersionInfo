using Microsoft.UI.Xaml;
using System.Runtime.InteropServices;
using System;


namespace GetVersionInfo
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {

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

        public MainWindow()
        {
            this.InitializeComponent();
        }

        private void myButton_Click(object sender, RoutedEventArgs e)
        {
            
            myButton.Content = GetWaRunningArchitecture();
//            myButton.Content = GetRunningArchitecture();
//            myButton.Content = System.Runtime.InteropServices.RuntimeInformation.ProcessArchitecture.ToString();
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


            System.Runtime.InteropServices.RuntimeInformation.ProcessArchitecture.ToString();
            var machineArch = System.Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE");

            switch (System.Runtime.InteropServices.RuntimeInformation.ProcessArchitecture)
            {
                case Architecture.Arm64:
                    return "NativeArm64";
                case Architecture.X64:
                    if (machine == 0x8664)
                        return "EmulatedX64";
                    return "NativeX64";
                default: return "Unknown";
            }

            //return nativeMachine switch
            //{
            //    // IMAGE_FILE_MACHINE_ARM64
            //    0xAA64 when machine == 0x8664 => "EmulatedX64",
            //    0xAA64 when machine == 0xAA64 => "NativeArm64",
            //    0xAA64 when machine == 0x014C => "EmulatedX86",
            //    // IMAGE_FILE_MACHINE_AMD64
            //    0x8664 when machine == 0x8664 => "NativeX64",
            //    _ => "Unknown",
            //};
        }

        public static string GetRunningArchitecture()
        {

            //ProcessorArchitecture.GetArchitecture(out var arch); 
            //arch.ToString();
#if ARM64
            return "NativeArm64";
#elif X64
            return "NativeX64";
#else
            return "Unknown";
#endif
        }
    }
}
