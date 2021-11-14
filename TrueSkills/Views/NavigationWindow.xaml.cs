using Notifications.Wpf;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Interop;
using System.Windows.Media;
using TrueSkills.ViewModels;

namespace TrueSkills.Views
{
    /// <summary>
    /// Логика взаимодействия для NavigationWindow.xaml
    /// </summary>
    public partial class NavigationWindow : Window
    {

        public NavigationWindow()
        {
            InitializeComponent();
            TemporaryVariables.frame = MainFrame;
            DataContext = new NavigationVM();
            //CheckMonitors();
            //ProcessModule objCurrentModule = Process.GetCurrentProcess().MainModule;
            //objKeyboardProcess = new LowLevelKeyboardProc(captureKey);
            //ptrHook = SetWindowsHookEx(13, objKeyboardProcess, GetModuleHandle(objCurrentModule.ModuleName), 0);
            //Locker.Lock();
            this.Closed += (o, e) => { UnhookWindowsHookEx(ptrHook); Locker.Unlock();};
        }
        private void CheckMonitors()
        {
            var screen = Screen.FromHandle(new WindowInteropHelper(this).Handle);
            foreach (var item in Screen.AllScreens)
            {
                if (screen.DeviceName != item.DeviceName)
                {
                    BlackWindow window = new BlackWindow();
                    System.Drawing.Rectangle workingArea = item.WorkingArea;
                    window.Left = workingArea.Left;
                    window.Top = workingArea.Top;
                    window.Width = workingArea.Width;
                    window.Height = workingArea.Height;
                    window.Topmost = true;
                    window.Show();
                }

            }
        }
        #region KeyboardHook
        [StructLayout(LayoutKind.Sequential)]
        private struct KBDLLHOOKSTRUCT
        {
            public Keys key;
            public int scanCode;
            public int flags;
            public int time;
            public IntPtr extra;
        }
        //System level functions to be used for hook and unhook keyboard input  
        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int id, LowLevelKeyboardProc callback, IntPtr hMod, uint dwThreadId);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool UnhookWindowsHookEx(IntPtr hook);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hook, int nCode, IntPtr wp, IntPtr lp);
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string name);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern short GetAsyncKeyState(Keys key);
        //Declaring Global objects     
        private IntPtr ptrHook;
        private LowLevelKeyboardProc objKeyboardProcess;

        private IntPtr captureKey(int nCode, IntPtr wp, IntPtr lp)
        {
            if (nCode >= 0)
            {
                KBDLLHOOKSTRUCT objKeyInfo = (KBDLLHOOKSTRUCT)Marshal.PtrToStructure(lp, typeof(KBDLLHOOKSTRUCT));

                // Disabling Windows keys 

                if (objKeyInfo.key == Keys.RWin
                    || objKeyInfo.key == Keys.LWin
                    || objKeyInfo.key == Keys.Tab
                    || objKeyInfo.key == Keys.Escape
                    ||objKeyInfo.key==Keys.PrintScreen)
                {
                    return (IntPtr)1; // if 0 is returned then All the above keys will be enabled
                }

                if ((objKeyInfo.flags == 32) && (objKeyInfo.key == (Keys)0x52)
                    || (objKeyInfo.flags == 0xA2) && (objKeyInfo.key == (Keys)0x4E)
                    || (objKeyInfo.flags == 0xA3) && (objKeyInfo.key == (Keys)0x4E)
                    || (objKeyInfo.flags == 0xA2) && (objKeyInfo.key == (Keys)0x57)
                    || (objKeyInfo.flags == 0xA3) && (objKeyInfo.key == (Keys)0x57)
                    || (objKeyInfo.flags == 0x82) && (objKeyInfo.key == (Keys)0x79)
                    || (objKeyInfo.flags == 0xA1) && (objKeyInfo.key == (Keys)0x79)
                    || (objKeyInfo.flags == 32) && (objKeyInfo.key == (Keys)0x5A)
                    )
                {
                    return (IntPtr)1;
                }
            }
            return CallNextHookEx(ptrHook, nCode, wp, lp);
        }

        bool HasAltModifier(int flags)
        {
            return (flags & 0x20) == 0x20;
        }
        #endregion
    }
}
