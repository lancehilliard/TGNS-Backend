using System;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace TGNS
{
    public class ObsRecordingStatusChanger
    {
        const int WM_COMMAND = 0x0111;
        const int BN_CLICKED = 0;
        const int ObsStartStopRecordingButtonId = 0x138A;
        const int ObsExitButtonId = 0x138C;
        [DllImport("user32.dll")]
        static extern IntPtr GetDlgItem(IntPtr hWnd, int nIDDlgItem);
        [DllImport("user32.dll")]
        static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, IntPtr lParam);

        private string GetMainModuleFilepath(int processId)
        {
            string wmiQueryString = "SELECT ProcessId, ExecutablePath FROM Win32_Process WHERE ProcessId = " + processId;
            using (var searcher = new ManagementObjectSearcher(wmiQueryString))
            {
                using (var results = searcher.Get())
                {
                    ManagementObject mo = results.Cast<ManagementObject>().FirstOrDefault();
                    if (mo != null)
                    {
                        return (string)mo["ExecutablePath"];
                    }
                }
            }
            return null;
        }
        IntPtr GetMainWindowHandle()
        {
            IntPtr handle = IntPtr.Zero;
            Process[] localAll = Process.GetProcesses();
            foreach (Process p in localAll)
            {
                if (string.Equals(p.ProcessName, "OBS"))
                {
                    handle = p.MainWindowHandle;
                }
            }
            //if (handle == IntPtr.Zero)
            //{
            //    Console.WriteLine("Not found");
            //    return;
            //}
            return handle;
        }

        void ClickButton(IntPtr mainWindowHandle, IntPtr buttonHandle, int buttonId)
        {
            int wParam = (BN_CLICKED << 16) | (buttonId & 0xffff);
            SendMessage(mainWindowHandle, WM_COMMAND, wParam, buttonHandle); // http://www.c-sharpcorner.com/UploadFile/SamTomato/clicking-a-button-in-another-application/
        }

        public void StartRecording()
        {
            var mainWindowHandle = GetMainWindowHandle();
            var buttonHandle = GetDlgItem(mainWindowHandle, ObsStartStopRecordingButtonId);
            var controlTextGetter = new ControlTextGetter();
            var controlText = controlTextGetter.Get(buttonHandle);
            if (controlText.Contains("Start"))
            {
                ClickButton(mainWindowHandle, buttonHandle, ObsStartStopRecordingButtonId);
            }
        }

        public void StopRecording()
        {
            var mainWindowHandle = GetMainWindowHandle();
            var buttonHandle = GetDlgItem(mainWindowHandle, ObsStartStopRecordingButtonId);
            var controlTextGetter = new ControlTextGetter();
            var controlText = controlTextGetter.Get(buttonHandle);
            if (controlText.Contains("Stop"))
            {
                ClickButton(mainWindowHandle, buttonHandle, ObsStartStopRecordingButtonId);
            }
        }

        public void ClickExitButton()
        {
            var mainWindowHandle = GetMainWindowHandle();
            var buttonHandle = GetDlgItem(mainWindowHandle, ObsExitButtonId);
            var controlTextGetter = new ControlTextGetter();
            var controlText = controlTextGetter.Get(buttonHandle);
            if (controlText.Contains("Exit"))
            {
                ClickButton(mainWindowHandle, buttonHandle, ObsExitButtonId);
            }
        }

        public bool IsReadyForButtonClicks()
        {
            IntPtr mainWindowHandle = IntPtr.Zero;
            Process[] localAll = Process.GetProcesses();
            foreach (Process p in localAll)
            {
                if (string.Equals(p.ProcessName, "OBS"))
                {
                    mainWindowHandle = p.MainWindowHandle;
                }
            }
            IntPtr buttonHandle = GetDlgItem(mainWindowHandle, ObsStartStopRecordingButtonId);
            var result = mainWindowHandle != IntPtr.Zero && buttonHandle != IntPtr.Zero;
            return result;
        }

        public bool IsRecording()
        {
            IntPtr mainWindowHandle = IntPtr.Zero;
            Process[] localAll = Process.GetProcesses();
            foreach (Process p in localAll)
            {
                if (string.Equals(p.ProcessName, "OBS"))
                {
                    mainWindowHandle = p.MainWindowHandle;
                }
            }
            if (mainWindowHandle == IntPtr.Zero)
            {
                throw new Exception("OBS window not found.");
            }

            IntPtr buttonHandle = GetDlgItem(mainWindowHandle, ObsStartStopRecordingButtonId);
            if (buttonHandle == IntPtr.Zero)
            {
                throw new Exception("OBS start/stop button not found.");
            }


            var controlTextGetter = new ControlTextGetter();
            var controlText = controlTextGetter.Get(buttonHandle);
            var result = controlText.Contains("Stop");
            return result;
        }

        public string GetRecordingButtonText()
        {
            IntPtr mainWindowHandle = IntPtr.Zero;
            Process[] localAll = Process.GetProcesses();
            foreach (Process p in localAll)
            {
                if (string.Equals(p.ProcessName, "OBS"))
                {
                    mainWindowHandle = p.MainWindowHandle;
                }
            }
            if (mainWindowHandle == IntPtr.Zero)
            {
                throw new Exception("OBS window not found.");
            }

            IntPtr buttonHandle = GetDlgItem(mainWindowHandle, ObsStartStopRecordingButtonId);
            if (buttonHandle == IntPtr.Zero)
            {
                throw new Exception("OBS start/stop button not found.");
            }


            var controlTextGetter = new ControlTextGetter();
            var result = controlTextGetter.Get(buttonHandle);
            return result;
        }

        private static ProcessModule GetModule(Process p)
        {
            ProcessModule pm = null;
            try { pm = p.MainModule; }
            catch
            {
                return null;
            }
            return pm;
        }
    }
}