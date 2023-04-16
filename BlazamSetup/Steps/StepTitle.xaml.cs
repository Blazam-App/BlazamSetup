using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BlazamSetup.Steps
{
    /// <summary>
    /// Interaction logic for StepTitle.xaml
    /// </summary>
    public partial class StepTitle : UserControl, INotifyPropertyChanged
    {
        private string title;

        public event PropertyChangedEventHandler PropertyChanged;
        public StepTitle()
        {
            InitializeComponent();
            this.DataContext = this;
           
        }
        public string Title {
            get => title;
            set
            {
                title = value;
                TitleLabel.Content = Title;
                OnPropertyChanged("Title");
            }

        }
        // Create the OnPropertyChanged method to raise the event
        // The calling member's name will be used as the parameter.
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
