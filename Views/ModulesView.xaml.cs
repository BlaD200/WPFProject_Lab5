using System;
using System.Windows;
using TaskManager.Models;
using TaskManager.ViewModels;

namespace TaskManager.Views
{
    public partial class ModulesView : Window
    {
        public ModulesView(ref ProcessEntity proc)
        {
            InitializeComponent();
            ModulesViewModel vm = new ModulesViewModel(ref proc);
            DataContext = vm;
            if (vm.CloseAction == null)
                vm.CloseAction = new Action(this.Close);
        }
    }
}
