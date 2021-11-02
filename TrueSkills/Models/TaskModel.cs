using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using TrueSkills.APIs;
using TrueSkills.Exceptions;
using TrueSkills.Interfaces;

namespace TrueSkills.Models
{
    public class TaskModel : ReactiveObject
    {
        private ObservableCollection<Pdf> _pdfs;
        private Visibility _visibilityListBox;
        private Pdf _currentPdf;
        private TaskAPI.Rootobject _tasks;
        private ObservableCollection<int> _navNumbers;
        private int _selectedIndex = 0;
        public Visibility VisibilityListBox
        {
            get => _visibilityListBox;
            set => this.RaiseAndSetIfChanged(ref _visibilityListBox, value);
        }
        public int SelectedIndex
        {
            get => _selectedIndex;
            set => this.RaiseAndSetIfChanged(ref _selectedIndex, value);
        }
        public ObservableCollection<int> NavNumbers
        {
            get => _navNumbers;
            set => this.RaiseAndSetIfChanged(ref _navNumbers, value);
        }
        public TaskAPI.Rootobject Tasks
        {
            get => _tasks;
            set => this.RaiseAndSetIfChanged(ref _tasks, value);
        }
        public ObservableCollection<Pdf> Pdfs
        {
            get => _pdfs;
            set => this.RaiseAndSetIfChanged(ref _pdfs, value);
        }
        public Pdf CurrentPdf
        {
            get => _currentPdf;
            set => this.RaiseAndSetIfChanged(ref _currentPdf, value);
        }
        public TaskModel()
        {
            NavNumbers = new ObservableCollection<int>();
            VisibilityListBox = Visibility.Collapsed;
            Pdfs = new ObservableCollection<Pdf>();
            Initialization = InitializationAsync();
        }

        public Task Initialization { get; set; }

        private async Task InitializationAsync()
        {
            try
            {
                Tasks = await SupportingMethods.GetWebRequest<TaskAPI.Rootobject>(Url.s_taskUrl, true);
                await GetTasksAsync();
                if (Tasks.Files.Count() > 1)
                {
                    SetNumbers();
                }  
            }
            catch (CodeException ex) { TemporaryVariables.ShowException(ex); }
        }

        private void SetNumbers()
        {
            for (int i = 1; i <= Tasks.Files.Count(); i++)
            {
                NavNumbers.Add(i);
            }
            VisibilityListBox = Visibility.Visible;
        }

        public void NextDocument(int? selectedIndex = null, bool isUp = true)
        {
            if (selectedIndex.HasValue)
            {
                SelectedIndex = selectedIndex.Value;
            }
            else
            {
                if (!isUp && Pdfs.FirstOrDefault().Address == CurrentPdf.Address)
                {
                    return;
                }
                if (isUp && Pdfs.LastOrDefault().Address == CurrentPdf.Address)
                {
                    return;
                }
                _ = isUp ? SelectedIndex += 1 : SelectedIndex -= 1;
            }
            CurrentPdf = Pdfs[SelectedIndex];
        }

        private async Task GetTasksAsync()
        {
            TemporaryVariables.ClearTemp();
            foreach (var item in Tasks.Files)
            {
                try
                {
                    SupportingMethods.GetFileWebRequest(item.Url, Url.s_documentUrl, true);
                    Pdfs.Add(new Pdf() { Address = Path.GetTempPath() + $"TrueSkills\\{item.Url}.pdf", Id = item.Url });
                }
                catch (CodeException ex) { TemporaryVariables.ShowException(ex); }

            }
            await Task.Delay(1000);
            if (Pdfs.Any())
            {
                CurrentPdf = Pdfs[SelectedIndex];
            }
        }
    }
}
