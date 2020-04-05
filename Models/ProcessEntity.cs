using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Principal;
using TaskManager.Tools;

namespace TaskManager.Models
{
    public class ProcessEntity : INotifyPropertyChanged
    {
        #region Fields

        private readonly Process _process;

        private bool _isActive;
        private double _cpuPercents;
        private float _ramAmount;
        private int _threads;

        private PerformanceCounter? _perfCounter;
        private PerformanceCounter? _ramCounter;

        #endregion

        #region Properties

        public Process ProcessItself => _process;

        public int ID { get; }

        public string Name { get; }

        public bool IsActive
        {
            get => _isActive;
            set
            {
                _isActive = value;
                OnPropertyChanged();
            }
        }

        public double CPUPercents
        {
            get { return _cpuPercents; }
            set
            {
                _cpuPercents = value;
                OnPropertyChanged();
            }
        }

        public float RAMAmount
        {
            get { return _ramAmount; }
            set
            {
                _ramAmount = value;
                OnPropertyChanged();
            }
        }

        public int Threads
        {
            get => _threads;
            set
            {
                _threads = value;
                OnPropertyChanged();
            }
        }

        public string User
        {
            get
            {
                IntPtr processHandle = IntPtr.Zero;
                try
                {
                    OpenProcessToken(_process.Handle, 8, out processHandle);
                    WindowsIdentity wi = new WindowsIdentity(processHandle);
                    string user = wi.Name;
                    return user.Contains(@"\") ? user.Substring(user.IndexOf(@"\") + 1) : user;
                }
                catch
                {
                    return null;
                }
                finally
                {
                    if (processHandle != IntPtr.Zero)
                    {
                        CloseHandle(processHandle);
                    }
                }
            }
        }

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool OpenProcessToken(IntPtr ProcessHandle, uint DesiredAccess, out IntPtr TokenHandle);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CloseHandle(IntPtr hObject);

        public ProcessModuleCollection Modules
        {
            get { return _process.Modules; }
        }

        public ProcessThreadCollection ThreadsCollection
        {
            get { return _process.Threads; }
        }

        public string Filepath
        {
            get
            {
                try
                {
                    return _process.MainModule.FileName;
                }
                catch (Exception e)
                {
                    return "Access denied";
                }
            }
        }

        public bool checkAvailability()
        {
            if (StartingTime == "Access denied")
            {
                return false;
            }

            return true;
        }

        public string StartingTime
        {
            get
            {
                try
                {
                    return _process.StartTime.ToString("HH:mm:ss dd/MM/yyyy");
                }
                catch (Exception e)
                {
                    return "Access denied";
                }
            }
        }

        #endregion

        internal ProcessEntity(Process process)
        {
            _process = process;
            ID = _process.Id;
            Name = process.ProcessName;
            _threads = process.Threads.Count;
            try
            {
                _perfCounter = new PerformanceCounter(
                    "Process", "% Processor Time", process.ProcessName, true);
                CPUPercents = _perfCounter.NextValue();
                _ramCounter = new PerformanceCounter("Process",
                    "Working Set - Private", process.ProcessName);
            }
            catch (InvalidOperationException e)
            {
                _perfCounter = null;
                _ramCounter = null;
            }
        }

        internal void UpdateMetaData(ProcessEntity selectedProcessEntity)
        {
            _process.Refresh();
            IsActive = _process.Responding;
            Threads = _process.Threads.Count;

            if (_perfCounter != null)
                try
                {
                    CPUPercents = _perfCounter.NextValue();
                }
                catch (Exception e)
                {
                    _perfCounter = null;
                }

            ;
            if (_ramCounter != null)
                try
                {
                    RAMAmount = (float) Math.Round((double) _ramCounter.RawValue / 1024 / 1024, 2);
                }
                catch (Exception e)
                {
                    _ramCounter = null;
                }

            if (this != selectedProcessEntity) return;
        }

        [field: NonSerialized] public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}