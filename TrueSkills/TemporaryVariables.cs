using CefSharp.Wpf;
using Notifications.Wpf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using TrueSkills.APIs;
using TrueSkills.Enums;
using TrueSkills.Exceptions;
using TrueSkills.Models;
using TrueSkills.Views;

namespace TrueSkills
{
    public class TemporaryVariables
    {
        public static Bitmap videoFrame;
        public static ParticipentModel currentParticipent;
        private static NotificationManager s_manager;
        public static Frame frame;
        public static ChromiumWebBrowser webView;
        public static TimeSpan? time;
        public static RoomAPI.Rootobject s_rooms;
        public static StreamAPI s_stream;
        private static int s_countExpert;
        private static int s_countSupport;
        public static bool isAuthDevice = false;
        public static List<string> sources = new List<string>();
        public static NotificationManager GetManager()
        {
            if (s_manager == null)
            {
                s_manager = new NotificationManager();
            }
            return s_manager;
        }

        public static void NotNetwork()
        {
            new MessageBoxWindow(GetProperty("a_Network"), GetProperty("a_Error"), MessageBoxWindow.MessageBoxButton.Ok);
        }

        public static async Task<StreamAPI> GetStream()
        {
            if (s_stream == null)
            {
                s_stream = await SupportingMethods.GetWebRequest<StreamAPI>(Url.s_streamUrl, true);
            }
            return s_stream;
        }

        private static bool s_isCheck = false;
        public static async Task SubscribeGetCountMessagesAsync()
        {
            if (App.IsNetwork)
            {
                var url = await GetUrlAsync(Room.Expert, Operation.Get);
                var secondUrl = await GetUrlAsync(Room.Support, Operation.Get);
                var response = await SupportingMethods.PostWebRequest<MessageAPI.Rootobject>(url, true);
                var secondResponse = await SupportingMethods.PostWebRequest<MessageAPI.Rootobject>(secondUrl, true);
                var countExpert = response.Messages.Where(x => x.FullNameUser != currentParticipent.FullName).Count();
                var countSupport = secondResponse.Messages.Where(x => x.FullNameUser != currentParticipent.FullName).Count();
                if (s_isCheck)
                {
                    if (countExpert > s_countExpert)
                    {
                        GetManager().Show(
                               new NotificationContent { Title = GetProperty("a_NotificationTitle2"), Message = GetProperty("a_NotificationChat1"), Type = NotificationType.Information },
                                areaName: "WindowArea", expirationTime: TimeSpan.FromSeconds(10));
                    }
                    if (countSupport > s_countSupport)
                    {
                        GetManager().Show(
                               new NotificationContent { Title = GetProperty("a_NotificationTitle2"), Message = GetProperty("a_NotificationChat2"), Type = NotificationType.Information },
                               areaName: "WindowArea", expirationTime: TimeSpan.FromSeconds(10));
                    }
                }
                s_isCheck = true;
                s_countExpert = countExpert;
                s_countSupport = countSupport;
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                await Task.Delay(5000);
                await SubscribeGetCountMessagesAsync();
            }
        }

        public static async Task<string> GetUrlAsync(Room room, Operation operation)
        {
            string url = string.Empty;
            var rooms = await GetRoomsAsync();
            if (rooms != null)
            {
                if (room == Room.Expert)
                {
                    url = Url.s_chatUrl + $"/{rooms.Rooms.Expert}";
                }
                else if (room == Room.Support)
                {
                    url = Url.s_chatUrl + $"/{rooms.Rooms.Support}";
                }
                if (operation == Operation.Get)
                {
                    url += "/get";
                }
                else if (operation == Operation.Send)
                {
                    url += "/send";
                }
            }

            return url;
        }

        [DllImport("user32.dll")]
        private static extern IntPtr GetDC(IntPtr hWnd);
        [DllImport("user32.dll")]
        private static extern bool ReleaseDC(IntPtr hWnd, IntPtr hdc);
        [DllImport("gdi32.dll")]
        private static extern int GetDeviceCaps(IntPtr hdc, int index);

        public static int RefreshFrequency()
        {
            var hDCScreen = GetDC(IntPtr.Zero);
            return GetDeviceCaps(hDCScreen, 116);
        }

        public static async Task<StepAPI> GetStep()
        {
            return await SupportingMethods.GetWebRequest<StepAPI>(Url.s_stepUrl, true);
        }


        public static bool lostRtmpCamera;
        public static bool lostRtmpScreen;

        private static bool IsSome(string property)
        {
            return frame.Content.ToString().Contains(property);
        }
        private static Rtmp rtmp = new Rtmp();
        public static StepAPI s_step;
        public static async Task SubscribeLoadStepAsync()
        {
            if (App.IsNetwork)
            {
                if (currentParticipent != null)
                {
                    try
                    {
                        var response = await GetStep();
                        s_step = response;
                        var date = Convert.ToDateTime(response.End);
                        var nowResponse = await SupportingMethods.GetWebRequest<NowAPI>(Url.s_nowUrl, true);
                        var now = Convert.ToDateTime(nowResponse.Time);
                        if (now > date || date.TimeOfDay.TotalSeconds <= 0)
                        {
                            time = null;
                        }
                        else
                        {
                            var substract = (date - now).Duration();
                            time = substract;
                        }

                        if (isAuthDevice && lostRtmpScreen)
                        {
                            try
                            {
                                rtmp.RtmpScreen(GetStream().Result.Screen);

                            }
                            catch (Exception ex)
                            {
                                new MessageBoxWindow(ex.Message, GetProperty("a_Error"), MessageBoxWindow.MessageBoxButton.Ok);
                            }
                        }

                        if (isAuthDevice && lostRtmpCamera)
                        {
                            try
                            {
                                rtmp.RtmpCamera(GetStream().Result.Camera);

                            }
                            catch (Exception ex)
                            {
                                new MessageBoxWindow(ex.Message, GetProperty("a_Error"), MessageBoxWindow.MessageBoxButton.Ok);
                            }
                        }
                        SearchBefore(response);
                        if (response.Step == Step.ExamHasStartedDocumentDisplayed)
                        {
                            if (isAuthDevice && !IsSome("DocumentsPage"))
                            {
                                frame.Navigate(new DocumentsPage());
                            }
                        }
                        else if (response.Step == Step.ExamStartTaskDisplay)
                        {
                            if (isAuthDevice && !IsSome("TaskPage"))
                            {
                                frame.Navigate(new TaskPage());
                            }
                        }
                        else if (response.Step == Step.ExamStartModuleUnderway)
                        {
                            if (isAuthDevice && !IsSome("VMPage"))
                            {
                                frame.Navigate(new VMPage());
                            }
                        }
                        else if (response.Step == Step.ExamHasStartedModuleNotStarted)
                        {
                            BeforeExamWindow beforeExamWindow = new BeforeExamWindow(response);
                            beforeExamWindow.ShowDialog();
                        }
                        else if (response.Step == Step.ExamOver)
                        {
                            new ExamEndWindow().Show();
                            CloseAllWindows();
                        }
                    }
                    catch (CodeException ex)
                    {
                        ShowException(ex);
                    }
                }
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                await Task.Delay(30000);
                await SubscribeLoadStepAsync();
            }
        }

        public static void CloseAllWindows()
        {
            foreach (var window in App.Current.Windows)
            {
                if (window.GetType() != typeof(ExamEndWindow))
                {
                    (window as Window).Close();
                }
            }
        }

        private static void SearchBefore(StepAPI step)
        {
            if (step.Step != Step.ExamHasStartedModuleNotStarted)
            {
                foreach (var window in App.Current.Windows)
                {
                    if (window is BeforeExamWindow beforeWindow)
                    {
                        beforeWindow.Close();
                    }
                }
            }
        }
        public static async Task<RoomAPI.Rootobject> GetRoomsAsync()
        {
            if (currentParticipent == null)
                return null;
            if (s_rooms == null)
                s_rooms = await SupportingMethods.GetWebRequest<RoomAPI.Rootobject>(Url.s_chatUrl, true);
            return s_rooms;
        }

        public static void ClearTemp()
        {
            var directory = Path.GetTempPath() + "TrueSkills";
            if (Directory.Exists(directory))
            {
                var files = Directory.GetFiles(directory);
                foreach (var item in files)
                {
                    File.Delete(item);
                }
            }
        }

        public static void Exit()
        {
            Locker.Unlock();
            Process.GetCurrentProcess().Kill();
        }
        public static string englishName;
        public static ResourceDictionary CurrentResource;

        public static string GetProperty(string property)
        {
            return CurrentResource[property].ToString();
        }

        public static List<int> GetCodesError()
        {
            List<int> codes = new List<int>();
            foreach (var item in CurrentResource.Keys)
            {
                if (item.ToString().Contains("a_Code"))
                {
                    codes.Add(int.Parse(item.ToString().Replace("a_Code", "")));
                }
            }
            return codes;
        }

        public static void Restart()
        {
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo()
                {
                    WorkingDirectory = Environment.CurrentDirectory,
                    FileName = "dotnet",
                    Arguments = $"TrueSkills.dll {TemporaryVariables.Language.Name}",
                    Verb = "runas",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                };
                Process.Start(startInfo);
            }
            catch
            {
                return;
            }
            Application.Current.Shutdown();
        }

        public static void ShowException(CodeException ex)
        {
            if (ex.Error != null)
            {
                var code = GetCodesError().FirstOrDefault(x => x == ex.Error.Code);
                if (code == 2)
                {
                    bool isLoseToken = false;
                    foreach (var window in App.Current.Windows)
                    {
                        if (window.GetType() == typeof(MainWindow))
                        {
                            isLoseToken = true;
                            break;
                        }
                    }
                    if (!isLoseToken)
                    {
                        Restart();
                    }
                    return;
                }
                if (code != 0)
                {
                    new MessageBoxWindow(GetProperty($"a_Code{code}"), GetProperty("a_Error"), MessageBoxWindow.MessageBoxButton.Ok);
                }
                else
                {
                    new MessageBoxWindow(ex.Message, GetProperty("a_Error"), MessageBoxWindow.MessageBoxButton.Ok);
                }
            }
            else
            {
                new MessageBoxWindow(ex.Message, GetProperty("a_Error"), MessageBoxWindow.MessageBoxButton.Ok);
            }
        }

        public static CultureInfo Language
        {

            get
            {
                return Thread.CurrentThread.CurrentUICulture;
            }
            set
            {
                if (value == null) throw new ArgumentNullException("value");
                if (value == Thread.CurrentThread.CurrentUICulture) return;

                Thread.CurrentThread.CurrentUICulture = value;

                ResourceDictionary dict = new ResourceDictionary();
                dict.Source = new Uri(string.Format("/Languages/Language.{0}.xaml", value.Name), UriKind.RelativeOrAbsolute);
                CurrentResource = dict;
                ResourceDictionary oldDict = (from d in Application.Current.Resources.MergedDictionaries
                                              where d.Source != null && d.Source.OriginalString == "/Languages/Language.ru-RU.xaml"
                                              select d).FirstOrDefault();
                if (oldDict != null)
                {
                    int ind = Application.Current.Resources.MergedDictionaries.IndexOf(oldDict);
                    Application.Current.Resources.MergedDictionaries.Remove(oldDict);
                    Application.Current.Resources.MergedDictionaries.Insert(ind, dict);
                }
                else
                {
                    Application.Current.Resources.MergedDictionaries.Add(dict);
                }
            }
        }
    }
}
