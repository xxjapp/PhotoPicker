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

        static AppCommands() {
            CommandManager.RegisterClassCommandBinding(typeof(AppCommands), new CommandBinding(_openFileCommand));
            CommandManager.RegisterClassCommandBinding(typeof(AppCommands), new CommandBinding(_previousCommand));
            CommandManager.RegisterClassCommandBinding(typeof(AppCommands), new CommandBinding(_nextCommand));
            CommandManager.RegisterClassCommandBinding(typeof(AppCommands), new CommandBinding(_homeCommand));
            CommandManager.RegisterClassCommandBinding(typeof(AppCommands), new CommandBinding(_endCommand));
        }
    }
}
