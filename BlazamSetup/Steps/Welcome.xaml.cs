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
using System.Windows.Threading;

namespace BlazamSetup.Steps
{
    /// <summary>
    /// Interaction logic for Welcome.xaml
    /// </summary>
    public partial class Welcome : UserControl,IInstallationStep

    {

        private LicenseAgreement _licenseAgreement;

        public Welcome()
        {
            InitializeComponent();


        }

        public int Order => 0;

        IInstallationStep IInstallationStep.NextStep()
        {
            if (_licenseAgreement == null)
                _licenseAgreement = new LicenseAgreement();

            return _licenseAgreement;
           // return new LicenseAgreement();
        }

      
    }
}
