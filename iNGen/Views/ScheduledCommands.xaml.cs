﻿using iNGen.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace iNGen.Views
{
    public partial class ScheduledCommands : UserControl
    {
        public ScheduledCommandsViewModel ViewModel { get; set; }
        
        public ScheduledCommands()
        {
            InitializeComponent();
            ViewModel = new ScheduledCommandsViewModel();
        }

        private void DeleteTaskButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void AddTaskBUtton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}