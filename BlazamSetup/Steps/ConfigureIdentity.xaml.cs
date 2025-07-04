﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Tulpep.ActiveDirectoryObjectPicker;

namespace BlazamSetup.Steps
{
    /// <summary>
    /// Interaction logic for ConfigureIdentity.xaml
    /// </summary>
    public partial class ConfigureIdentity : UserControl, IInstallationStep
    {
        public ConfigureIdentity()
        {
            InitializeComponent();
        }

        public int Order => throw new NotImplementedException();

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DirectoryObjectPickerDialog picker = new DirectoryObjectPickerDialog()
            {
                AllowedObjectTypes = ObjectTypes.All,
                DefaultObjectTypes = ObjectTypes.Users|ObjectTypes.ServiceAccounts,
                AllowedLocations = Locations.All,
                DefaultLocations = Locations.LocalComputer,
                AttributesToFetch = new Collection<string>() { "distinguishedName"},
                MultiSelect = false,
                ShowAdvancedView = true
            };
            using (picker)
            {
                if (picker.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    InstallationConfiguraion.ApplicationIdentity = picker.SelectedObject;
                    accountTextBox.Text=picker.SelectedObject.Name;
                    foreach (var sel in picker.SelectedObjects)
                    {
                        Console.WriteLine(sel.Name);
                    }
                }
            }
        }

        IInstallationStep IInstallationStep.NextStep()
        {
            return new DatabaseType();
        }
    }
}
