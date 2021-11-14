using DotNetPusher.Pushers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TrueSkills
{
    public class Rtmp
    {
        private bool IsNetwork { get; set; }
        public Rtmp()
        {
            IsNetwork = true;
            NetworkChange.NetworkAvailabilityChanged += new NetworkAvailabilityChangedEventHandler(NetworkChange_NetworkAvailabilityChanged);
        }

        private void NetworkChange_NetworkAvailabilityChanged(object sender, NetworkAvailabilityEventArgs e)
        {
            IsNetwork = e.IsAvailable;
        }

        [DllImport("user32.dll")]
        public static extern int GetSystemMetrics(int index);

        [DllImport("user32.dll")]
        private static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("gdi32.dll")]
        private static extern bool BitBlt([In] IntPtr hdc, int nXDest, int nYDest, int nWidth, int nHeight, [In] IntPtr hdcSrc, int nXSrc, int nYSrc, uint dwRop);

        [DllImport("user32.dll")]
        private static extern bool ReleaseDC(IntPtr hWnd, IntPtr hdc);

        public void RtmpScreen(string url)
        {
            if (IsNetwork)
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
                screenThread = new Thread(() =>
               {
                   var encoder = new DotNetPusher.Encoders.Encoder(width, height, frameRate, 1024 * 800);
                   encoder.FrameEncoded += (sender, e) =>
                   {
                       if (IsNetwork)
                       {
                           var packet = e.Packet;
                           pusher.PushPacket(packet);
                       }

                   };
                   var screenDc = GetDC(IntPtr.Zero);
                   var bitmap = new Bitmap(screenWidth, screenHeight);
                   if (IsNetwork)
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
                           GC.Collect();
                           GC.WaitForPendingFinalizers();
                           GC.Collect();
                       }
                       finally
                       {
                           encoder.Dispose();
                           bitmap.Dispose();
                           ReleaseDC(IntPtr.Zero, screenDc);
                           TemporaryVariables.lostRtmpScreen = true;
                           stopEvent.Set();

                           screenThread.Join();
                           pusher.StopPush();
                           pusher.Dispose();
                           GC.Collect();
                           GC.WaitForPendingFinalizers();
                           GC.Collect();
                       }
                   }
               });
                screenThread.Start();
            }
        }

        static Thread cameraThread = new Thread(() => { });
        static Thread screenThread = new Thread(() => { });

        public void RtmpCamera(string url)
        {
            if (IsNetwork)
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
                cameraThread = new Thread(() =>
                {
                    var encoder = new DotNetPusher.Encoders.Encoder(width, height, frameRate, 1024 * 800);
                    encoder.FrameEncoded += (sender, e) =>
                    {
                        if (App.IsNetwork)
                        {
                            var packet = e.Packet;
                            pusher.PushPacket(packet);
                        }
                    };
                    var bitmap = TemporaryVariables.videoFrame;
                    if (IsNetwork)
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
                            GC.Collect();
                            GC.WaitForPendingFinalizers();
                            GC.Collect();
                        }
                        finally
                        {
                            encoder.Dispose();
                            bitmap.Dispose();
                            ReleaseDC(IntPtr.Zero, IntPtr.Zero);
                            TemporaryVariables.lostRtmpCamera = true;

                            stopEvent.Set();
                            cameraThread.Join();
                            pusher.StopPush();
                            pusher.Dispose();
                            GC.Collect();
                            GC.WaitForPendingFinalizers();
                            GC.Collect();
                        }
                    }
                });
                cameraThread.Start();
            }
        }
    }
}
