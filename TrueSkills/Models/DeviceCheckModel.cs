using AForge.Video;
using AForge.Video.DirectShow;
using AudioSwitcher.AudioApi.CoreAudio;
using DotNetPusher.Encoders;
using DotNetPusher.Pushers;
using NAudio.Wave;
using Notifications.Wpf.Core;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using TrueSkills.APIs;
using TrueSkills.Exceptions;
using TrueSkills.Interfaces;
using static TrueSkills.APIs.DocumentAPI;

namespace TrueSkills.Models
{
    public class DeviceCheckModel : ReactiveObject, IAsyncInitialization
    {
        [DllImport("user32.dll")]
        public static extern int GetSystemMetrics(int index);

        [DllImport("user32.dll")]
        private static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("gdi32.dll")]
        private static extern bool BitBlt([In] IntPtr hdc, int nXDest, int nYDest, int nWidth, int nHeight, [In] IntPtr hdcSrc, int nXSrc, int nYSrc, uint dwRop);

        [DllImport("user32.dll")]
        private static extern bool ReleaseDC(IntPtr hWnd, IntPtr hdc);


        private FilterInfoCollection _videoDevices;
        private bool _startSound;
        private VideoCaptureDevice _videoSource;
        private BitmapImage _videoSourceImage;
        private bool _isNotFitDocument;
        private Rootobject _documents;
        private ObservableCollection<WaveInCapabilities> _soundSource;
        private WaveInCapabilities _selectedMicrophone;
        private FilterInfo _selectedWebcam;
        private ObservableCollection<CoreAudioDevice> _playbackDevices;
        private CoreAudioDevice _selectedAudioDevice;
        private StreamAPI _stream;
        private bool _isEnabled;
        public bool IsEnabled
        {
            get => _isEnabled;
            set => this.RaiseAndSetIfChanged(ref _isEnabled, value);
        }
        public StreamAPI Stream
        {
            get => _stream;
            set => this.RaiseAndSetIfChanged(ref _stream, value);
        }
        public CoreAudioDevice SelectedAudioDevice
        {
            get => _selectedAudioDevice;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedAudioDevice, value);     
                IsEnabled = IsSuccessfulCheck();
            }
        }
        public ObservableCollection<CoreAudioDevice> PlaybackDevices
        {
            get => _playbackDevices;
            set => this.RaiseAndSetIfChanged(ref _playbackDevices, value);
        }

        public FilterInfo SelectedWebcam
        {
            get => _selectedWebcam;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedWebcam, value); 
                IsEnabled = IsSuccessfulCheck();
            }
        }
        public WaveInCapabilities SelectedMicrophone
        {
            get => _selectedMicrophone;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedMicrophone, value);
                IsEnabled = IsSuccessfulCheck();
            }
        }
        public bool StartSound
        {
            get => _startSound;
            set => this.RaiseAndSetIfChanged(ref _startSound, value);
        }
        public ObservableCollection<WaveInCapabilities> SoundSources
        {
            get => _soundSource;
            set => this.RaiseAndSetIfChanged(ref _soundSource, value);
        }
        public BitmapImage VideoSource
        {
            get => _videoSourceImage;
            set => this.RaiseAndSetIfChanged(ref _videoSourceImage, value);
        }
        public FilterInfoCollection FilterInfoCollection
        {
            get => _videoDevices;
            set => this.RaiseAndSetIfChanged(ref _videoDevices, value);
        }
        public VideoCaptureDevice VideoCaptureDevice
        {
            get => _videoSource;
            set => this.RaiseAndSetIfChanged(ref _videoSource, value);
        }
        public bool IsNotFitDocument
        {
            get => _isNotFitDocument;
            set => this.RaiseAndSetIfChanged(ref _isNotFitDocument, value);
        }
        public Task Initialization { get; set; }
        public Rootobject Documents
        {
            get => _documents;
            set => this.RaiseAndSetIfChanged(ref _documents, value);
        }

        public DeviceCheckModel()
        {
            StartSound = true;
            SoundSources = new ObservableCollection<WaveInCapabilities>();
            PlaybackDevices = new ObservableCollection<CoreAudioDevice>();

            Initialization = InitializationAsync();
        }

        private async Task InitializationAsync()
        {
            try
            {
                Stream = await SupportingMethods.GetWebRequest<StreamAPI>(Url.s_streamUrl, true);
                Documents = await SupportingMethods.GetWebRequest<Rootobject>(Url.s_documentUrl, true);
            }
            catch (CodeException ex)
            {
                TemporaryVariables.ShowException(ex);
            }

            FilterInfoCollection = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            for (int i = 0; i < WaveIn.DeviceCount; i++)
            {
                SoundSources.Add(WaveIn.GetCapabilities(i));
            }
            List<CoreAudioDevice> devices = new CoreAudioController().GetDevices().Where(x => x.IsPlaybackDevice).DistinctBy(x => x.FullName).ToList();
            foreach (var item in devices)
            {
                PlaybackDevices.Add(item);
            }
        }

        private WaveIn _waveIn;
        private WaveInProvider _provideWaveIn;
        private DirectSoundOut _outputWaveIn;
        public void StopWebcam()
        {
            VideoCaptureDevice.Stop();
        }

        public void StartVideo()
        {
            VideoCaptureDevice = new VideoCaptureDevice(SelectedWebcam.MonikerString);
            VideoCaptureDevice.NewFrame += new NewFrameEventHandler(VideoSource_NewFrame);
            VideoCaptureDevice.Start();
        }

        public void ChangeOutput()
        {
            if (SelectedAudioDevice == null)
                return;

            foreach (CoreAudioDevice d in PlaybackDevices)
            {
                if (d.FullName == SelectedAudioDevice.FullName)
                    d.SetAsDefault();
            }
        }

        public void AudioChangeOutput()
        {
            if (StartSound)
            {
                StartAudio();
                TemporaryVariables.GetManager().ShowAsync(
                           new NotificationContent { Title = "Уведомление", Message = "Звук сказанный в микрофон воспроизводится из динамика.", Type = NotificationType.Information },
                           areaName: "WindowArea", expirationTime: TimeSpan.FromSeconds(10));
            }
            else
            {
                StopAudio();
            }
        }

        private void StartAudio()
        {

            _waveIn = new WaveIn();
            var deviceId = SoundSources.IndexOf(SelectedMicrophone);
            _waveIn.DeviceNumber = deviceId;
            _waveIn.WaveFormat = new WaveFormat(44100, WaveIn.GetCapabilities(deviceId).Channels);
            _provideWaveIn = new WaveInProvider(_waveIn);
            _outputWaveIn = new DirectSoundOut();
            _outputWaveIn.Init(_provideWaveIn);
            _waveIn.StartRecording();
            _outputWaveIn.Play();
            StartSound = false;
        }

        private void StopAudio()
        {
            if (_outputWaveIn != null)
            {
                _waveIn.StopRecording();
                _outputWaveIn.Dispose();
                _outputWaveIn = null;
                _provideWaveIn = null;
            }
            StartSound = true;
        }

        public bool IsSuccessfulCheck()
        {
           return SelectedAudioDevice != null && SelectedMicrophone.ProductName != null && SelectedWebcam != null;
        }



        private bool _isSendRtmp = true;
        private void VideoSource_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            Bitmap bitmap = (Bitmap)eventArgs.Frame.Clone();
            _videoFrame = bitmap;
            VideoSource = Convert(bitmap);
            if (_isSendRtmp)
            {
                RtmpScreen();
                RtmpCamera();
            }
            _isSendRtmp = false;
        }

        private Bitmap _videoFrame;
        private BitmapImage Convert(Bitmap src)
        {
            MemoryStream ms = new MemoryStream();
            src.Save(ms, ImageFormat.Bmp);
            ms.Position = 0;
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            ms.Seek(0, SeekOrigin.Begin);
            image.StreamSource = ms;
            image.EndInit();
            image.Freeze();
            return image;
        }

        private void RtmpScreen()
        {
            var pushUrl = Stream.Screen;
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
                    var packet = e.Packet;
                    pusher.PushPacket(packet);
                };
                var screenDc = GetDC(IntPtr.Zero);
                var bitmap = new Bitmap(screenWidth, screenHeight);
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
                finally
                {
                    encoder.Dispose();
                    bitmap.Dispose();
                    ReleaseDC(IntPtr.Zero, screenDc);
                }
            });
            thread.Start();

        }
        private void RtmpCamera()
        {
            var pushUrl = Stream.Camera;
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
                    var packet = e.Packet;
                    pusher.PushPacket(packet);
                };
                var bitmap = _videoFrame;
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
                finally
                {
                    encoder.Dispose();
                    bitmap.Dispose();
                    ReleaseDC(IntPtr.Zero, IntPtr.Zero);
                }
            });
            thread.Start();
        }
    }
}
