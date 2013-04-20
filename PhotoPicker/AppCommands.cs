using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PhotoPicker.Commands {
    public static class AppCommands {
        private static RoutedUICommand _openFileCommand = new RoutedUICommand("OpenFile", "OpenFile", typeof(AppCommands));
        private static RoutedUICommand _previousCommand = new RoutedUICommand("Previous", "Previous", typeof(AppCommands));
        private static RoutedUICommand _nextCommand = new RoutedUICommand("Next", "Next", typeof(AppCommands));
        private static RoutedUICommand _homeCommand = new RoutedUICommand("Home", "Home", typeof(AppCommands));
        private static RoutedUICommand _endCommand = new RoutedUICommand("End", "End", typeof(AppCommands));
        private static RoutedUICommand _deleteLeftCommand = new RoutedUICommand("DeleteLeft", "DeleteLeft", typeof(AppCommands));
        private static RoutedUICommand _deleteRightCommand = new RoutedUICommand("DeleteRight", "DeleteRight", typeof(AppCommands));
        private static RoutedUICommand _fitPageCommand = new RoutedUICommand("FitPage", "FitPage", typeof(AppCommands));
        private static RoutedUICommand _actualSizeCommand = new RoutedUICommand("ActualSize", "ActualSize", typeof(AppCommands));
        private static RoutedUICommand _settingsCommand = new RoutedUICommand("Settings", "Settings", typeof(AppCommands));

        public static RoutedCommand OpenFileCommand {
            get { return _openFileCommand; }
        }

        public static RoutedCommand PreviousCommand {
            get { return _previousCommand; }
        }

        public static RoutedCommand NextCommand {
            get { return _nextCommand; }
        }

        public static RoutedCommand HomeCommand {
            get { return _homeCommand; }
        }

        public static RoutedCommand EndCommand {
            get { return _endCommand; }
        }

        public static RoutedCommand DeleteLeftCommand {
            get { return _deleteLeftCommand; }
        }

        public static RoutedCommand DeleteRightCommand {
            get { return _deleteRightCommand; }
        }

        public static RoutedCommand FitPageCommand {
            get { return _fitPageCommand; }
        }

        public static RoutedCommand ActualSizeCommand {
            get { return _actualSizeCommand; }
        }

        public static RoutedCommand SettingsCommand {
            get { return _settingsCommand; }
        }

        static AppCommands() {
            CommandManager.RegisterClassCommandBinding(typeof(AppCommands), new CommandBinding(_openFileCommand));
            CommandManager.RegisterClassCommandBinding(typeof(AppCommands), new CommandBinding(_previousCommand));
            CommandManager.RegisterClassCommandBinding(typeof(AppCommands), new CommandBinding(_nextCommand));
            CommandManager.RegisterClassCommandBinding(typeof(AppCommands), new CommandBinding(_homeCommand));
            CommandManager.RegisterClassCommandBinding(typeof(AppCommands), new CommandBinding(_endCommand));
            CommandManager.RegisterClassCommandBinding(typeof(AppCommands), new CommandBinding(_deleteLeftCommand));
            CommandManager.RegisterClassCommandBinding(typeof(AppCommands), new CommandBinding(_deleteRightCommand));
            CommandManager.RegisterClassCommandBinding(typeof(AppCommands), new CommandBinding(_fitPageCommand));
            CommandManager.RegisterClassCommandBinding(typeof(AppCommands), new CommandBinding(_actualSizeCommand));
            CommandManager.RegisterClassCommandBinding(typeof(AppCommands), new CommandBinding(_settingsCommand));
        }
    }
}
