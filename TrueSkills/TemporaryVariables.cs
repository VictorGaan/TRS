using Notifications.Wpf.Core;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        public static ParticipentModel s_currentParticipent;
        public static Frame s_frame;
        public static Step s_step;
        public static TimeSpan? s_time;
        private static RoomAPI.Rootobject s_rooms;
        private static NotificationManager s_manager;
        private static int s_countExpert;
        private static int s_countSupport;
        public static NotificationManager GetManager()
        {
            if (s_manager == null)
                s_manager = new NotificationManager();
            return s_manager;
        }

        private static bool s_isCheck = false;
        public static async Task SubscribeGetCountMessagesAsync()
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
                url = url + "/get";
            }
            else if (operation == Operation.Send)
            {
                url = url + "/send";
            }
            return url;
        }

        public static async Task LoadDate()
        {
            if (s_currentParticipent != null)
            {
                if (s_time == null)
                {
                    try
                    {
                        var response = await SupportingMethods.GetWebRequest<StepAPI>(Url.s_stepUrl, true);
                        s_step = response.Step;
                        if (s_step == Step.ExamOver)
                        {
                            new ExamEndWindow().Show();
                        }
                        else
                        {
                            var date = Convert.ToDateTime(response.End);
                            var nowResponse = await SupportingMethods.GetWebRequest<NowAPI>(Url.s_nowUrl, true);
                            var now = Convert.ToDateTime(nowResponse.Time);
                            if (now > date)
                            {
                                s_time = new TimeSpan();
                                return;
                            }
                            var substract = (date - now).Duration();
                            s_time = substract;
                        }

                    }
                    catch (CodeException ex)
                    {
                        ShowException(ex);
                        Application.Current.Shutdown(0);
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
                VKey.Multiply,
                VKey.Add,
                VKey.Separator,
                VKey.Subtract,
                VKey.Decimal,
                VKey.Divide,
                VKey.Up,
                VKey.Left,
                VKey.Right,
                VKey.Down,
                VKey.Delete,
                VKey.Back,
                VKey.NumberPad0,
                VKey.NumberPad1,
                VKey.NumberPad2,
                VKey.NumberPad3,
                VKey.NumberPad4,
                VKey.NumberPad5,
                VKey.NumberPad6,
                VKey.NumberPad7,
                VKey.NumberPad8,
                VKey.NumberPad9,
                VKey.NumberKeyLock,
                VKey.Menu,
                VKey.Tab
            };

        private static VKey[] EscapeKeys = new VKey[] {
                VKey.E,
                VKey.S,
                VKey.C,
                VKey.F,
                VKey.R,
                VKey.O,
                VKey.M,
                VKey.K,
                VKey.I,
                VKey.O,
                VKey.S,
                VKey.K,
                VKey.F1
            };
    }
}
