using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace TrueSkills
{
    public class Locker
    {
        public static void Lock()
        {
            LockControlPanel();
            LockTaskBar();
        }

        public static void Unlock()
        {
            if (IsLocked)
            {
                UnlockControlPanel();
                UnlockTaskBar();
            }
        }

        private static bool IsLocked;


        private static void LockControlPanel()
        {
            RegistryKey reg;
            string key = "1";
            string sub = "Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System";

            reg = Registry.CurrentUser.CreateSubKey(sub);
            reg.SetValue("DisableTaskMgr", key);
            reg.Close();
            IsLocked = true;
        }

        private static void UnlockControlPanel()
        {
            RegistryKey reg;
            string sub = "Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System";

            reg = Registry.CurrentUser.OpenSubKey(sub, true);
            reg.DeleteValue("DisableTaskMgr");
            reg.Close();
            IsLocked = false;
        }

        [DllImport("user32.dll")]
        private static extern int FindWindow(string className, string windowText);

        [DllImport("user32.dll")]
        private static extern int ShowWindow(int hwnd, int command);

        private const int SW_HIDE = 0;
        private const int SW_SHOW = 1;

        private static int Handle => FindWindow("Shell_TrayWnd", "");
        private static int StartHandle => FindWindow("Button", "Пуск");
        private static void HideTaskBar() => ShowWindow(Handle, SW_HIDE);
        private static void HideStartButton() => ShowWindow(StartHandle, SW_HIDE);
        private static void ShowTaskBar() => ShowWindow(Handle, SW_SHOW);
        private static void ShowStartButton() => ShowWindow(StartHandle, SW_SHOW);

        private static void LockTaskBar()
        {
            HideTaskBar();
            HideStartButton();
            IsLocked = true;
        }
        private static void UnlockTaskBar()
        {
            ShowTaskBar();
            ShowStartButton();
            IsLocked = false;
        }
    }
}
