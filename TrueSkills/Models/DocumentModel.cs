using ReactiveUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using TrueSkills.APIs;
using TrueSkills.Exceptions;
using TrueSkills.Interfaces;
using static TrueSkills.APIs.DocumentAPI;

namespace TrueSkills.Models
{
    public class DocumentModel : ReactiveObject
    {
        private bool _isChecked;
        private bool _isEnabled;
        private List<Pdf> _pdfs;
        private Pdf _currentPdf;
        private Rootobject _rootobject;
        public bool IsChecked
        {
            get => _isChecked;
            set => this.RaiseAndSetIfChanged(ref _isChecked, value);
        }

        public bool IsEnabled
        {
            get => _isEnabled;
            set => this.RaiseAndSetIfChanged(ref _isEnabled, value);
        }

        public List<Pdf> Pdfs
        {
            get => _pdfs;
            set => this.RaiseAndSetIfChanged(ref _pdfs, value);
        }

        public Pdf CurrentPdf
        {
            get => _currentPdf;
            set => this.RaiseAndSetIfChanged(ref _currentPdf, value);
        }
        public Rootobject Rootobject
        {
            get => _rootobject;
            set => this.RaiseAndSetIfChanged(ref _rootobject, value);
        }
        public Task Initialization { get; set; }

        public DocumentModel()
        {
            IsEnabled = false;
            Pdfs = new List<Pdf>();
            Initialization = GetDocuments();
        }

        public async Task FitDocumentsAsync()
        {
            if (App.IsNetwork)
            {
                if (IsChecked)
                {
                    try
                    {
                        var serializeObject = new FitDocumentAPI() { IsStatus = true, Id = Convert.ToInt32(CurrentPdf.Id) };
                        await SupportingMethods.PostWebRequest<FitDocumentAPI, StatusDocumentAPI>(Url.s_documentUrl + $"/{CurrentPdf.Id}", serializeObject, true);
                        if (Pdfs.Count > 0)
                        {
                            Pdfs.Remove(CurrentPdf);
                            if (Pdfs.Count != 0)
                            {
                                CurrentPdf = null;
                                CurrentPdf = Pdfs[0];
                            }
                        }
                        IsChecked = false;
                    }
                    catch (CodeException ex) { TemporaryVariables.ShowException(ex); }

                }
            }
        }
        public async Task GetDocuments()
        {
            if (App.IsNetwork)
            {
                Rootobject = await SupportingMethods.GetWebRequest<Rootobject>(Url.s_documentUrl, true);
                if (Rootobject.Files.Any())
                {
                    await GetFileAsync();
                }
            }
        }
        private async Task GetFileAsync()
        {
            if (App.IsNetwork)
            {
                TemporaryVariables.ClearTemp();
                foreach (var item in Rootobject.Files)
                {
                    try
                    {
                        SupportingMethods.GetFileWebRequest(item.Url, Url.s_documentUrl, true);
                        Pdfs.Add(new Pdf() { Address = Path.GetTempPath() + $"TrueSkills\\{item.Url}.pdf", Id = item.Id });
                    }
                    catch (CodeException ex) { TemporaryVariables.ShowException(ex); }
                }
                await Task.Delay(1000);
                if (Pdfs.Any())
                {
                    CurrentPdf = Pdfs[0];
                }
                IsEnabled = true;
            }
        }
    }
}
