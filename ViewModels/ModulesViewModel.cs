using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using TaskManager.Models;
using TaskManager.Tools;

namespace TaskManager.ViewModels
{
    internal class ModulesViewModel: BaseViewModel
    {
        private ObservableCollection<Module> _modules;

        public string ProcessName
        {
            get;
        }

        public ObservableCollection<Module> Modules
        {
            get
            {
                
                return _modules;
                
            }
            private set
            {
                _modules = value;
                OnPropertyChanged();
            }
        }

        public Action CloseAction { get; set; }

        internal ModulesViewModel(ref ProcessEntity processEntity)
        {
            Modules = new ObservableCollection<Module>();
            ObservableCollection<Module> tmp = new ObservableCollection<Module>();
            ProcessName = processEntity.Name;
            int id = processEntity.ID;
            foreach (ProcessModule module in processEntity.Modules)
            {
                tmp.Add(new Module(module));
            }
            Modules = tmp;
        }
    }
}
