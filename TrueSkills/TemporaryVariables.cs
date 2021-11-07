using Microsoft.Web.WebView2.Wpf;
using Notifications.Wpf.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management;
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
        public static Bitmap VideoFrame;
        public static ParticipentModel s_currentParticipent;
        public static Frame s_frame;
        public static WebView2 s_webView;
        public static TimeSpan? Time;
        private static RoomAPI.Rootobject s_rooms;
        private static NotificationManager s_manager;
        private static StreamAPI s_stream;
        private static int s_countExpert;
        private static int s_countSupport;
        public static bool IsAuthDevice = false;
        public static List<string> Sources = new List<string>();
        public static NotificationManager GetManager()
        {
            if (s_manager == null)
                s_manager = new NotificationManager();
            return s_manager;
        }

        public static void NotNetwork()
        {
            MessageBox.Show(GetProperty("a_Network"), GetProperty("a_Error"), MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public static async Task<StreamAPI> GetStream()
        {
            if (s_stream == null)
            {
                try
                {
                    s_stream = await SupportingMethods.GetWebRequest<StreamAPI>(Url.s_streamUrl, true);
                }
                catch (CodeException ex)
                {
                    ShowException(ex);
                }
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
                var countExpert = response.Messages.Where(x => x.FullNameUser != s_currentParticipent.FullName).Count();
                var countSupport = secondResponse.Messages.Where(x => x.FullNameUser != s_currentParticipent.FullName).Count();
                if (s_isCheck)
                {
                    if (countExpert > s_countExpert)
                    {
                        await GetManager().ShowAsync(
                               new NotificationContent { Title = "Сообщения", Message = "Вам пришло новое сообщение в комнате c экспертом, проверьте чат!", Type = NotificationType.Information },
                                areaName: "WindowArea", expirationTime: TimeSpan.FromSeconds(10));
                    }
                    if (countSupport > s_countSupport)
                    {
                        await GetManager().ShowAsync(
                               new NotificationContent { Title = "Сообщения", Message = "Вам пришло новое сообщение в комнате c тех. помощью, проверьте чат!", Type = NotificationType.Information },
                               areaName: "WindowArea", expirationTime: TimeSpan.FromSeconds(10));
                    }
                }
                s_isCheck = true;
                s_countExpert = countExpert;
                s_countSupport = countSupport;
                await Task.Delay(5000);
                await SubscribeGetCountMessagesAsync();
            }
        }

        public static async Task<string> GetUrlAsync(Room room, Operation operation)
        {
            string url = string.Empty;
            var rooms = await GetRoomsAsync();
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
            return url;
        }


        public static async Task<StepAPI> GetStep()
        {
            return await SupportingMethods.GetWebRequest<StepAPI>(Url.s_stepUrl, true);
        }


        public static bool LostRtmpCamera;
        public static bool LostRtmpScreen;

        private static bool IsSome(string property)
        {
            return s_frame.Content.ToString().Contains(property);
        }

        public static async Task SubscribeLoadStepAsync()
        {
            if (App.IsNetwork)
            {
                if (s_currentParticipent != null)
                {
                    try
                    {
                        var response = await GetStep();
                        var date = Convert.ToDateTime(response.End);
                        var nowResponse = await SupportingMethods.GetWebRequest<NowAPI>(Url.s_nowUrl, true);
                        var now = Convert.ToDateTime(nowResponse.Time);
                        if (now > date || date.TimeOfDay.TotalSeconds <= 0)
                        {
                            Time = null;
                        }
                        else
                        {
                            var substract = (date - now).Duration();
                            Time = substract;
                        }

                        if (IsAuthDevice && LostRtmpScreen)
                        {
                            SupportingMethods.RtmpScreen(GetStream().Result.Screen);
                        }

                        if (IsAuthDevice && LostRtmpCamera)
                        {
                            SupportingMethods.RtmpCamera(GetStream().Result.Camera);
                        }
                        SearchBefore(response);
                        if (response.Step == Step.ExamHasStartedDocumentDisplayed)
                        {
                            if (IsAuthDevice && !IsSome("DocumentsPage"))
                            {
                                s_frame.Navigate(new DocumentsPage());
                            }
                        }
                        else if (response.Step == Step.ExamStartTaskDisplay)
                        {
                            if (IsAuthDevice && !IsSome("TaskPage"))
                            {
                                s_frame.Navigate(new TaskPage());
                            }
                        }
                        else if (response.Step == Step.ExamStartModuleUnderway)
                        {
                            if (IsAuthDevice && !IsSome("VMPage"))
                            {
                                s_frame.Navigate(new VMPage());
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
                await Task.Delay(30000);
                await SubscribeLoadStepAsync();
            }
        }

        private static void CloseAllWindows()
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
            if (s_currentParticipent == null)
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
            KioskModeAPI.Unlock();
            Process.GetCurrentProcess().Kill();
        }
        public static string PathXaml;
        public static string EnglishName;
        public static ResourceDictionary CurrentResource;

        public static string GetProperty(string property)
        {
            return CurrentResource[property].ToString();
        }

        public static List<int> GetCodesError()
        {
            List<int> codes = new List<int>(7);
            foreach (var item in CurrentResource.Keys)
            {
                if (item.ToString().Contains("a_Code"))
                {
                    codes.Add(int.Parse(item.ToString().Replace("a_Code", "")));
                }
            }
            return codes;
        }


        public static void ShowException(CodeException ex)
        {
            if (ex.Error != null)
            {
                var code = GetCodesError().FirstOrDefault(x => x == ex.Error.Code);
                if (code != 0)
                {
                    MessageBox.Show($"{GetProperty($"a_Code{code}")}", $"{GetProperty("a_Error")}", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    MessageBox.Show(ex.Message, $"{GetProperty("a_Error")}", MessageBoxButton.OK, MessageBoxImage.Error);
                }
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

                ResourceDictionary dict = new ResourceDictionary
                {
                    Source = new Uri(string.Format($"{PathXaml}\\Languages\\Language.{value.Name}.xaml"), UriKind.RelativeOrAbsolute)
                };
                CurrentResource = dict;
                ResourceDictionary oldDict = (from d in Application.Current.Resources.MergedDictionaries
                                              where d.Source != null && d.Source.OriginalString.Contains("Language")
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

        public static bool IsMoreMice()
        {
            ManagementObjectSearcher search = new ManagementObjectSearcher("SELECT * FROM Win32_PointingDevice");
            int inputDeviceCount = search.Get().Count;
            if (inputDeviceCount > 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static void StartKiosk(IntPtr Handle)
        {
            KioskModeAPI.StartKioskMode(AllowedKeys, EscapeKeys, Handle);
        }


        private static VKey[] AllowedKeys = new VKey[] {
            VKey.Q,
            VKey.W,
            VKey.E,
            VKey.R,
            VKey.T,
            VKey.Y,
            VKey.U,
            VKey.I,
            VKey.O,
            VKey.P,
            VKey.A,
            VKey.S,
            VKey.D,
            VKey.F,
            VKey.G,
            VKey.H,
            VKey.J,
            VKey.K,
            VKey.L,
            VKey.Z,
            VKey.X,
            VKey.C,
            VKey.V,
            VKey.B,
            VKey.N,
            VKey.M,
            VKey.Number0,
            VKey.Number1,
            VKey.Number2,
            VKey.Number3,
            VKey.Number4,
            VKey.Number5,
            VKey.Number6,
            VKey.Number7,
            VKey.Number8,
            VKey.Number9,
            VKey.Space,
            VKey.LeftShift,
            VKey.RightShift,
            VKey.Back,
            };

        private static VKey[] EscapeKeys = new VKey[] {
                VKey.Delete,
                VKey.LeftControl,
                VKey.RightControl,
                VKey.RightMenu,
                VKey.LeftMenu,
                VKey.LeftWindows,
                VKey.RightWindows,
                VKey.Escape,
                VKey.Menu,
                VKey.Tab
            };
    }
}
