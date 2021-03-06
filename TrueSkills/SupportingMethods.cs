using DotNetPusher;
using DotNetPusher.Pushers;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.ServiceProcess;
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
                request.Headers.Add(HttpRequestHeader.Authorization, $"Bearer {TemporaryVariables.currentParticipent.Token}");
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
                request.Headers.Add(HttpRequestHeader.Authorization, $"Bearer {TemporaryVariables.currentParticipent.Token}");
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
            var obj = response.GetValueFromJson<V>();
            return obj;
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
                request.Headers.Add(HttpRequestHeader.Authorization, $"Bearer {TemporaryVariables.currentParticipent.Token}");
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
                request.Headers.Add(HttpRequestHeader.Authorization, $"Bearer {TemporaryVariables.currentParticipent.Token}");
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
            var obj = response.GetValueFromJson<T>();
            return obj;
        }


        public static void GetFileWebRequest(string id, string url, bool isToken = false, bool isTask = true)
        {
            using (WebClient client = new WebClient())
            {
                if (isToken)
                {
                    client.Headers.Add(HttpRequestHeader.Authorization, $"Bearer {TemporaryVariables.currentParticipent.Token}");
                }
                client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                Uri uri = new Uri(url + $"\\{id}");

                var directory = $"{Path.GetTempPath()}TrueSkills";
                if (isTask)
                {
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                        Directory.CreateDirectory(directory + "\\Tasks");
                    }
                    else
                    {
                        if (!Directory.Exists(directory + "\\Tasks"))
                        {
                            Directory.CreateDirectory(directory + "\\Tasks");
                        }
                    }
                }
                else
                {
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }
                }
                if (isTask)
                {
                    client.DownloadFileAsync(uri, $"{directory}\\Tasks\\{id}.pdf");
                }
                else
                {
                    client.DownloadFileAsync(uri, $"{directory}\\{id}.pdf");

                }
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
    }
}
