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
            sendToRecycleBinCheckBox.IsChecked = Properties.Settings.Default.SendToRecycleBin;
            rememberLastPositionCheckBox.IsChecked = Properties.Settings.Default.RememberLastPosition;

            sendToRecycleBinCheckBox_CheckedChanged(sendToRecycleBinCheckBox);
        }

        private void Default_Clicked(object sender, RoutedEventArgs e) {
            deleteDestinationTextBox.Text = Properties.Settings.Default.DefaultDeleteDestination;
            sendToRecycleBinCheckBox.IsChecked = Properties.Settings.Default.DefaultSendToRecycleBin;
            rememberLastPositionCheckBox.IsChecked = Properties.Settings.Default.DefaultRememberLastPosition;

            sendToRecycleBinCheckBox_CheckedChanged(sendToRecycleBinCheckBox);
        }

        private void OK_Clicked(object sender, RoutedEventArgs e) {
            Properties.Settings.Default.DeleteDestination = deleteDestinationTextBox.Text;
            Properties.Settings.Default.SendToRecycleBin = (sendToRecycleBinCheckBox.IsChecked == true);
            Properties.Settings.Default.RememberLastPosition = (rememberLastPositionCheckBox.IsChecked == true);

            Properties.Settings.Default.Save();
            Close();
        }

        private void Cancel_Clicked(object sender, RoutedEventArgs e) {
            Close();
        }

        private void sendToRecycleBinCheckBox_Clicked(object sender, RoutedEventArgs e) {
            sendToRecycleBinCheckBox_CheckedChanged(sender);
        }

        private void sendToRecycleBinCheckBox_CheckedChanged(object sender) {
            CheckBox checkBox = sender as CheckBox;

            if (checkBox.IsChecked == true) {
                deleteDestinationTextBox.Text = null;
                deleteDestinationTextBox.IsEnabled = false;
            } else {
                if (Properties.Settings.Default.DeleteDestination.Length > 0) {
                    deleteDestinationTextBox.Text = Properties.Settings.Default.DeleteDestination;
                } else {
                    deleteDestinationTextBox.Text = Properties.Settings.Default.DefaultDeleteDestination;
                }

                deleteDestinationTextBox.IsEnabled = true;
            }
        }
    }
}
