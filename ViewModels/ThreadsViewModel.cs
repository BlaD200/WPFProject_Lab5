using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using TaskManager.Models;
using TaskManager.Tools;

namespace TaskManager.ViewModels
{
    internal class ThreadsViewModel : BaseViewModel
    {
        private ObservableCollection<Thread> _threads;

        public string ProcessName
        {
            get;
        }

        public ObservableCollection<Thread> Threads
        {
            get
            {

                return _threads;

            }
            private set
            {
                _threads= value;
                OnPropertyChanged();
            }
        }

        public Action CloseAction { get; set; }

        internal ThreadsViewModel(ref ProcessEntity processEntity)
        {
            Threads = new ObservableCollection<Thread>();
            ObservableCollection<Thread> tmp = new ObservableCollection<Thread>();
            ProcessName = processEntity.Name;
            int id = processEntity.ID;
            foreach (ProcessThread thread in processEntity.ThreadsCollection)
            {
                tmp.Add(new Thread(thread));
            }
            Threads = tmp;
        }
    }
}
