﻿using BlazamSetup.Services;
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

namespace BlazamSetup.Steps
{
    /// <summary>
    /// Interaction logic for PostInstallation.xaml
    /// </summary>
    public partial class PostInstallation : UserControl,IInstallationStep
    {
        public PostInstallation()
        {
            InitializeComponent();
        }
        protected override void OnVisualParentChanged(DependencyObject oldParent)
        {
            base.OnVisualParentChanged(oldParent);
            if (oldParent == null)
                MainWindow.SetNextText("Finish");


        }

        IInstallationStep IInstallationStep.NextStep()
        {
            if (startCheckBox.IsChecked==true)
            {
                if (InstallationConfiguraion.InstallationType == InstallType.IIS)
                {
                    IISManager.Start();
                }
                else
                {
                    ServiceManager.Start();
                }
            }
           return new ExitStep();
        }


    }
}
