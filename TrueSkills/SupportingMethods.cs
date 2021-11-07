using DotNetPusher;
using DotNetPusher.Pushers;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using TrueSkills.APIs;
using TrueSkills.Exceptions;
using TrueSkills.Models;

namespace TrueSkills
{
    public static class SupportingMethods
    {
        /// <summary>
        /// POST Web Request
        /// </summary>
        /// <param name="url">Ссылка на запрос</param>
        /// <param name="serializeObject">Объект для сериализации</param>
        /// <param name="isToken">Нужен ли токен для запроса</param>
        /// <returns>Ответ</returns>
        public static async Task PostWebRequest<T>(string url, T serializeObject, bool isToken = false)
        {
            WebRequest request = WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/json";
            if (isToken)
            {
                request.Headers.Add(HttpRequestHeader.Authorization, $"Bearer {TemporaryVariables.s_currentParticipent.Token}");
            }
            var json = JsonConvert.SerializeObject(serializeObject);
            var byteArray = Encoding.UTF8.GetBytes(json);
            request.ContentLength = byteArray.Length;
            using (Stream stream = await request.GetRequestStreamAsync())
            {
                await stream.WriteAsync(byteArray.AsMemory(0, byteArray.Length));
            }
            WebResponse webResponse = await request.GetResponseAsync();
            var response = string.Empty;
            if (!IsBadHttpStatus(webResponse))
            {
                using (Stream stream = webResponse.GetResponseStream())
                {
                    using StreamReader reader = new StreamReader(stream);
                    response = await reader.ReadToEndAsync();
                }
            }
            webResponse.Close();
            var tuple = response.IsValidJson();
            if (tuple.isValid)
            {
                throw new CodeException(tuple.response);
            }
        }
        /// <summary>
        /// POST Web Request
        /// </summary>
        /// <param name="url">Ссылка на запрос</param>
        /// <param name="serializeObject">Объект для сериализации</param>
        /// <param name="isToken">Нужен ли токен для запроса</param>
        /// <returns>Ответ</returns>
        public static async Task<V> PostWebRequest<T, V>(string url, T serializeObject, bool isToken = false)
        {
            WebRequest request = WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/json";
            if (isToken)
            {
                request.Headers.Add(HttpRequestHeader.Authorization, $"Bearer {TemporaryVariables.s_currentParticipent.Token}");
            }
            var json = JsonConvert.SerializeObject(serializeObject);
            var byteArray = Encoding.UTF8.GetBytes(json);
            request.ContentLength = byteArray.Length;
            using (Stream stream = await request.GetRequestStreamAsync())
            {
                await stream.WriteAsync(byteArray.AsMemory(0, byteArray.Length));
            }
            WebResponse webResponse = await request.GetResponseAsync();
            var response = string.Empty;
            if (!IsBadHttpStatus(webResponse))
            {
                using (Stream stream = webResponse.GetResponseStream())
                {
                    using StreamReader reader = new StreamReader(stream);
                    response = await reader.ReadToEndAsync();
                }
            }
            webResponse.Close();
            var tuple = response.IsValidJson();
            if (tuple.isValid)
            {
                throw new CodeException(tuple.response);
            }
            return response.GetValueFromJson<V>();
        }
        /// <summary>
        /// POST Web Request
        /// </summary>
        /// <param name="url">Ссылка на запрос</param>
        /// <param name="serializeObject">Объект для сериализации</param>
        /// <param name="isToken">Нужен ли токен для запроса</param>
        /// <returns>Ответ</returns>
        public static async Task<V> PostWebRequest<V>(string url, bool isToken = false)
        {
            WebRequest request = WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/json";
            if (isToken)
            {
                request.Headers.Add(HttpRequestHeader.Authorization, $"Bearer {TemporaryVariables.s_currentParticipent.Token}");
            }
            WebResponse webResponse = await request.GetResponseAsync();
            var response = string.Empty;
            if (!IsBadHttpStatus(webResponse))
            {
                using (Stream stream = webResponse.GetResponseStream())
                {
                    using StreamReader reader = new StreamReader(stream);
                    response = await reader.ReadToEndAsync();
                }
            }
            webResponse.Close();
            var tuple = response.IsValidJson();
            if (tuple.isValid)
            {
                throw new CodeException(tuple.response);
            }
            return response.GetValueFromJson<V>();
        }

        /// <summary>
        /// GET Web Request
        /// </summary>
        /// <param name="url">Ссылка на запрос</param>
        /// <param name="isToken">Нужен ли токен для запроса</param>
        /// <returns>Ответ</returns>
        public static async Task<T> GetWebRequest<T>(string url, bool isToken = false)
        {
            WebRequest request = WebRequest.Create(url);
            request.Method = "GET";
            request.ContentType = "application/json";
            if (isToken)
            {
                request.Headers.Add(HttpRequestHeader.Authorization, $"Bearer {TemporaryVariables.s_currentParticipent.Token}");
            }
            using WebResponse webResponse = await request.GetResponseAsync();
            var response = string.Empty;

            if (!IsBadHttpStatus(webResponse))
            {
                using Stream webStream = webResponse.GetResponseStream();
                using StreamReader reader = new StreamReader(webStream);
                response = await reader.ReadToEndAsync();
            }
            webResponse.Close();
            var tuple = response.IsValidJson();
            if (tuple.isValid)
            {
                throw new CodeException(tuple.response);
            }
            return response.GetValueFromJson<T>();
        }


        public static void GetFileWebRequest(string id, string url, bool isToken = false)
        {
            using (WebClient client = new WebClient())
            {
                if (isToken)
                {
                    client.Headers.Add(HttpRequestHeader.Authorization, $"Bearer {TemporaryVariables.s_currentParticipent.Token}");
                }
                client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                Uri uri = new Uri(url + $"\\{id}");
                var directory = $"{Path.GetTempPath()}TrueSkills";
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                client.DownloadFileAsync(uri, $"{directory}\\{id}.pdf");
            }
        }

        //Validation Methods
        private static bool IsBadHttpStatus(WebResponse webResponse)
        {
            var value = (int)((HttpWebResponse)webResponse).StatusCode;
            if ((value >= 500 && value <= 599) || value >= 400 && value <= 499)
            {
                return true;
            }
            return false;
        }

        [DllImport("user32.dll")]
        public static extern int GetSystemMetrics(int index);

        [DllImport("user32.dll")]
        private static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("gdi32.dll")]
        private static extern bool BitBlt([In] IntPtr hdc, int nXDest, int nYDest, int nWidth, int nHeight, [In] IntPtr hdcSrc, int nXSrc, int nYSrc, uint dwRop);

        [DllImport("user32.dll")]
        private static extern bool ReleaseDC(IntPtr hWnd, IntPtr hdc);

        public static void RtmpScreen(string url)
        {
            if (App.IsNetwork)
            {
                var pushUrl = url;
                if (pushUrl == null || pushUrl == string.Empty) return;

                var frameRate = 15;
                var waitInterval = 1000 / frameRate;


                var screenWidth = GetSystemMetrics(0);
                var screenHeight = GetSystemMetrics(1);

                var width = screenWidth;
                var height = screenHeight;

                var pusher = new Pusher();
                pusher.StartPush(pushUrl, width, height, frameRate);

                var stopEvent = new ManualResetEvent(false);
                var thread = new Thread(() =>
                {
                    var encoder = new DotNetPusher.Encoders.Encoder(width, height, frameRate, 1024 * 800);
                    encoder.FrameEncoded += (sender, e) =>
                    {
                        if (App.IsNetwork)
                        {
                            var packet = e.Packet;
                            pusher.PushPacket(packet);
                            Debug.WriteLine(packet.Size);
                        }

                    };
                    var screenDc = GetDC(IntPtr.Zero);
                    var bitmap = new Bitmap(screenWidth, screenHeight);
                    if (App.IsNetwork)
                    {
                        try
                        {
                            while (!stopEvent.WaitOne(1))
                            {
                                var start = Environment.TickCount;
                                using (var graphic = Graphics.FromImage(bitmap))
                                {
                                    var imageDc = graphic.GetHdc();
                                    BitBlt(imageDc, 0, 0, width, height, screenDc, 0, 0, 0x00CC0020);
                                }
                                encoder.AddImage(bitmap);
                                var timeUsed = Environment.TickCount - start;
                                var timeToWait = waitInterval - timeUsed;
                                Thread.Sleep(timeToWait < 0 ? 0 : timeToWait);
                            }
                            encoder.Flush();
                        }
                        catch(PusherException)
                        {
                            encoder.Dispose();
                            bitmap.Dispose();
                            ReleaseDC(IntPtr.Zero, screenDc);
                            TemporaryVariables.LostRtmpScreen = true;
                            return;
                        }
                    }
                });
                thread.Start();
            }
        }

        public static void RtmpCamera(string url)
        {
            if (App.IsNetwork)
            {
                var pushUrl = url;
                if (pushUrl == null || pushUrl == string.Empty) return;

                var frameRate = 15;
                var waitInterval = 1000 / frameRate;


                var screenWidth = GetSystemMetrics(0);
                var screenHeight = GetSystemMetrics(1);

                var width = screenWidth;
                var height = screenHeight;

                var pusher = new Pusher();
                pusher.StartPush(pushUrl, width, height, frameRate);

                var stopEvent = new ManualResetEvent(false);
                var thread = new Thread(() =>
                {
                    var encoder = new DotNetPusher.Encoders.Encoder(width, height, frameRate, 1024 * 800);
                    encoder.FrameEncoded += (sender, e) =>
                    {
                        if (App.IsNetwork)
                        {
                            var packet = e.Packet;
                            pusher.PushPacket(packet);
                            Debug.WriteLine(packet.Size);
                        }
                    };
                    var bitmap = TemporaryVariables.VideoFrame;
                    if (App.IsNetwork)
                    {
                        try
                        {
                            while (!stopEvent.WaitOne(1))
                            {
                                var start = Environment.TickCount;
                                encoder.AddImage(bitmap);
                                var timeUsed = Environment.TickCount - start;
                                var timeToWait = waitInterval - timeUsed;
                                Thread.Sleep(timeToWait < 0 ? 0 : timeToWait);
                            }
                            encoder.Flush();
                        }
                        catch (PusherException)
                        {
                            encoder.Dispose();
                            bitmap.Dispose();
                            ReleaseDC(IntPtr.Zero, IntPtr.Zero);
                            TemporaryVariables.LostRtmpCamera = true;
                            return;
                        }
                    }
                });
                thread.Start();
            }
        }
    }
}
