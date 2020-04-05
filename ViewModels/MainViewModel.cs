using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using TaskManager.Models;
using TaskManager.Tools;
using TaskManager.Views;

namespace TaskManager.ViewModels
{
    class MainViewModel: BaseViewModel
    {
        #region Fields

        private bool _isControlEnabled = true;

        private ObservableCollection<ProcessEntity> _processes = new ObservableCollection<ProcessEntity>();
        private ProcessEntity _selectedProcess;

        #region Commands

        #region Sort

        private RelayCommand<object> _sortById;
        private RelayCommand<object> _sortByName;
        private RelayCommand<object> _sortByIsActive;
        private RelayCommand<object> _sortByCPUPercents;
        private RelayCommand<object> _sortByRAMAmount;
        private RelayCommand<object> _sortByThreadsNumber;
        private RelayCommand<object> _sortByUser;
        private RelayCommand<object> _sortByFilepath;
        private RelayCommand<object> _sortByStartingTime;

        #endregion

        private RelayCommand<object> _endTask;
        private RelayCommand<object> _openFolder;
        private RelayCommand<object> _showThreads;
        private RelayCommand<object> _showModules;

        #endregion

        #endregion

        #region Properties

        private static int SortingParameter { get; set; }

        public ProcessEntity SelectedProcess
        {
            get { return _selectedProcess; }
            set
            {
                _selectedProcess = value;
                OnPropertyChanged();
            }
        }

        public bool IsControlEnabled
        {
            get { return _isControlEnabled; }
            set
            {
                _isControlEnabled = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<ProcessEntity> Processes
        {
            get { return _processes; }
            private set
            {
                _processes = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand<object> EndTask
        {
            get
            {
                return _endTask ?? (_endTask = new RelayCommand<object>(
                           EndTaskImplementation, o => CanExecuteCommand()));
            }
        }

        public RelayCommand<object> OpenFolder
        {
            get
            {
                return _openFolder ?? (_openFolder = new RelayCommand<object>(
                           OpenFolderImplementation, o => CanExecuteCommand()));
            }
        }

        public RelayCommand<object> ShowThreads
        {
            get
            {
                return _showThreads ?? (_showThreads = new RelayCommand<object>(
                           ShowThreadsImplementation, o => CanExecuteCommand()));
            }
        }

        public RelayCommand<object> ShowModules
        {
            get
            {
                return _showModules ?? (_showModules = new RelayCommand<object>(
                           ShowModulesImplementation, o => CanExecuteCommand()));
            }
        }

        public RelayCommand<object> SortById
        {
            get
            {
                return _sortById ?? (_sortById = new RelayCommand<object>(o =>
                           SortImplementation(o, 0)));
            }
        }

        public RelayCommand<object> SortByName
        {
            get
            {
                return _sortByName ?? (_sortByName = new RelayCommand<object>(o =>
                           SortImplementation(o, 1)));
            }
        }

        public RelayCommand<object> SortByIsActive
        {
            get
            {
                return _sortByIsActive ?? (_sortByIsActive = new RelayCommand<object>(o =>
                           SortImplementation(o, 2)));
            }
        }

        public RelayCommand<object> SortByCPUPercents
        {
            get
            {
                return _sortByCPUPercents ?? (_sortByCPUPercents = new RelayCommand<object>(o =>
                           SortImplementation(o, 3)));
            }
        }

        public RelayCommand<object> SortByRAMAmount
        {
            get
            {
                return _sortByRAMAmount ?? (_sortByRAMAmount = new RelayCommand<object>(o =>
                           SortImplementation(o, 4)));
            }
        }

        public RelayCommand<object> SortByThreadsNumber
        {
            get
            {
                return _sortByThreadsNumber ?? (_sortByThreadsNumber = new RelayCommand<object>(o =>
                           SortImplementation(o, 5)));
            }
        }

        public RelayCommand<object> SortByUser
        {
            get
            {
                return _sortByUser ?? (_sortByUser = new RelayCommand<object>(o =>
                           SortImplementation(o, 6)));
            }
        }

        public RelayCommand<object> SortByFilepath
        {
            get
            {
                return _sortByFilepath ?? (_sortByFilepath = new RelayCommand<object>(o =>
                           SortImplementation(o, 7)));
            }
        }

        public RelayCommand<object> SortByStartingTime
        {
            get
            {
                return _sortByStartingTime ?? (_sortByStartingTime = new RelayCommand<object>(o =>
                           SortImplementation(o, 8)));
            }
        }

        #endregion

        #region CommandImpl

        private async void EndTaskImplementation(object obj)
        {
            await Task.Run(() =>
            {
                if (_selectedProcess.checkAvailability())
                {
                    Process process = Process.GetProcessById(SelectedProcess.ID);
                    process.Kill();
                    Application.Current.Dispatcher.Invoke((Action) delegate
                    {
                        Processes.Add(SelectedProcess); // new process
                    });
                    SelectedProcess = null;
                }
                else
                {
                    MessageBox.Show("Have no access");
                }
            });
        }

        private void OpenFolderImplementation(object obj)
        {
            try
            {
                Process.Start("explorer.exe",
                    _selectedProcess.Filepath.Substring(0,
                        _selectedProcess.Filepath.LastIndexOf("\\", StringComparison.Ordinal)));
            }
            catch (Exception e)
            {
                MessageBox.Show("Error while accessing processing data");
            }
        }

        private void ShowModulesImplementation(object obj)
        {
            IsControlEnabled = false;
            try
            {
                ModulesView smw = new ModulesView(ref _selectedProcess);
                smw.ShowDialog();
            }
            catch (Exception e)
            {
                MessageBox.Show("Error occurred while processing modules info");
            }

            IsControlEnabled = true;
        }

        private void ShowThreadsImplementation(object obj)
        {
            IsControlEnabled = false;
            try
            {
                ThreadsView smw = new ThreadsView(ref _selectedProcess);
                smw.ShowDialog();
            }
            catch (Exception e)
            {
                MessageBox.Show("Error occurred while processing threads info");
            }

            IsControlEnabled = true;
        }

        private async void SortImplementation(object obj, int param)
        {
            await Task.Run(() =>
            {
                try
                {
                    SortingParameter = param;
                    Processes = new ObservableCollection<ProcessEntity>(Processes);
                }
                catch (Exception e)
                {
                    MessageBox.Show("Error occurred while accessing process data");
                }
            });
        }

        #endregion

        internal MainViewModel()
        {
            SortingParameter = 1;

            // Timer updates data asynchronously
            var updateMetadata = new Timer(2000);
            updateMetadata.Elapsed += UpdateMetadataCallback;

            UpdateProcessesCallback(new object(), new EventArgs());

            var updateProcesses = new Timer(5000);
            updateProcesses.Elapsed += UpdateProcessesCallback;

            updateProcesses.Start();
            updateMetadata.Start();
        }

        #region Update

        private void UpdateProcessesCallback(object sender, EventArgs e)
        {
            var currentIds = Processes.Select(p => p.ID).ToList();

            var runningProcesses = Process.GetProcesses().ToList();
            foreach (Process p in runningProcesses)
                if (!currentIds.Contains(p.Id))
                    Application.Current.Dispatcher.Invoke((Action) delegate
                    {
                        Processes.Add(new ProcessEntity(p)); // new process
                    });

            // remove processes that do not exist anymore
            var runningIds = runningProcesses.Select(p => p.Id).ToList();
            foreach (var processEntity in Processes.ToList())
            {
                if (!runningIds.Contains(processEntity.ID))
                    Application.Current.Dispatcher.Invoke((Action)delegate
                    {
                        Processes.Remove(processEntity);
                    });
            }

            SortProcesses();
        }

        private void SortProcesses()
        {
            switch (SortingParameter)
            {
                case 0:
                    Processes = new ObservableCollection<ProcessEntity>(Processes.OrderBy(i => i.ID));
                    break;
                case 1:
                    Processes = new ObservableCollection<ProcessEntity>(Processes.OrderBy(i => i.Name));
                    break;
                case 2:
                    Processes = new ObservableCollection<ProcessEntity>(Processes.OrderBy(i => i.IsActive));
                    break;
                case 3:
                    Processes = new ObservableCollection<ProcessEntity>(Processes.OrderByDescending(i => i.CPUPercents));
                    break;
                case 4:
                    Processes = new ObservableCollection<ProcessEntity>(Processes.OrderByDescending(i => i.RAMAmount));
                    break;
                case 5:
                    Processes = new ObservableCollection<ProcessEntity>(Processes.OrderByDescending(i => i.Threads));
                    break;
                case 6:
                    Processes = new ObservableCollection<ProcessEntity>(Processes.OrderByDescending(i => i.User));
                    break;
                case 7:
                    Processes = new ObservableCollection<ProcessEntity>(Processes.OrderByDescending(i => i.StartingTime));
                    break;
                default:
                    throw new ArgumentException("Sort By Unknown Property");
            }
        }

        private void UpdateMetadataCallback(object o, EventArgs eventArgs)
        {
            for (int i = 0; i < Processes.Count; i++)
            {
                ProcessEntity process = Processes[i];
                process.UpdateMetaData(SelectedProcess);
            }
        }

        #endregion

        private bool CanExecuteCommand()
        {
            return SelectedProcess != null;
        }
    }
}
