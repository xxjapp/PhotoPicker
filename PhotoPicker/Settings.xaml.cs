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
using System.Windows.Shapes;

namespace PhotoPicker {
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window {
        public SettingsWindow() {
            InitializeComponent();
        }

        private void onInitialized(object sender, EventArgs e) {
            deleteDestinationTextBox.Text = Properties.Settings.Default.DeleteDestination;
        }

        private void Default_Clicked(object sender, RoutedEventArgs e) {
            deleteDestinationTextBox.Text = Properties.Settings.Default.DefaultDeleteDestination;
        }

        private void OK_Clicked(object sender, RoutedEventArgs e) {
            Properties.Settings.Default.DeleteDestination = deleteDestinationTextBox.Text;
            Properties.Settings.Default.Save();
            Close();
        }

        private void Cancel_Clicked(object sender, RoutedEventArgs e) {
            Close();
        }
    }
}
