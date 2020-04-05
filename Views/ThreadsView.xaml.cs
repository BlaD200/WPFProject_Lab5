using System;
using System.Windows;
using TaskManager.Models;
using TaskManager.ViewModels;

namespace TaskManager.Views
{
    public partial class ThreadsView : Window
    {
        public ThreadsView(ref ProcessEntity proc)
        {
            InitializeComponent();
            ThreadsViewModel vm = new ThreadsViewModel(ref proc);
            DataContext = vm;
            if (vm.CloseAction == null)
                vm.CloseAction = new Action(this.Close);
        }
    }
}
