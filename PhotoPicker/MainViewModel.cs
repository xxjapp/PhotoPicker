using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PhotoPicker {
    class MainViewModel : INotifyPropertyChanged {
        #region Construction

        public MainViewModel() {
            _backgroundWorker.WorkerReportsProgress = true;
            _backgroundWorker.WorkerSupportsCancellation = true;

            _backgroundWorker.DoWork += backgroundWorker1_DoWork;
            _backgroundWorker.ProgressChanged += backgroundWorker1_ProgressChanged;
            _backgroundWorker.RunWorkerCompleted += backgroundWorker1_RunWorkerCompleted;
        }

        #endregion

        #region Members
        private string[] _files = new string[0];
        private string _currentDirectory = null;
        private int _index = -1;
        private int _progress = 0;
        private string[] _imageInfos = new string[2];
        private ImageSource[] _imageSources = new ImageSource[2];
        private ImageSource _previousImageSource = null;
        private ImageSource _nextImageSource = null;
        private Dictionary<int, ImageSource> _imageCache = new Dictionary<int, ImageSource>();
        private readonly BackgroundWorker _backgroundWorker = new BackgroundWorker();
        #endregion

        #region Properties

        public string[] Files {
            get { return _files; }
            set {
                if (_files != value) {
                    _files = value;
                    RaisePropertyChanged("Files");
                }
            }
        }

        public string CurrentDirectory {
            get { return _currentDirectory; }
            set {
                if (_currentDirectory != value) {
                    _currentDirectory = value;
                    RaisePropertyChanged("CurrentDirectory");

                    Properties.Settings.Default.LastOpenedDirectory = CurrentDirectory;
                    Properties.Settings.Default.Save();
                }
            }
        }

        public int Index {
            get { return _index; }
            set {
                if (_files == null) {
                    return;
                }

                int newValue = -1;

                if (value < 0) {
                    newValue = 0;
                } else if (value > _files.Length - 2) {
                    newValue = _files.Length - 2;
                } else {
                    newValue = value;
                }

                if (newValue == -1) {
                    newValue = 0;
                }

                if (_index != newValue) {
                    _index = newValue;
                    RaisePropertyChanged("Index");

                    Properties.Settings.Default.LastOpenedDirectoryIndex = Index;
                    Properties.Settings.Default.Save();
                }
            }
        }

        public int Progress {
            get { return _progress; }
            set {
                if (_progress != value) {
                    _progress = value;
                    RaisePropertyChanged("Progress");
                }
            }
        }

        public string ImageInfo0 {
            get { return _imageInfos[0]; }
            set {
                if (_imageInfos[0] != value) {
                    _imageInfos[0] = value;
                    RaisePropertyChanged("ImageInfo0");
                }
            }
        }

        public string ImageInfo1 {
            get { return _imageInfos[1]; }
            set {
                if (_imageInfos[1] != value) {
                    _imageInfos[1] = value;
                    RaisePropertyChanged("ImageInfo1");
                }
            }
        }

        public ImageSource ImageSource0 {
            get { return _imageSources[0]; }
            set {
                if (_imageSources[0] != value) {
                    _imageSources[0] = value;
                    RaisePropertyChanged("ImageSource0");
                }
            }
        }

        public ImageSource ImageSource1 {
            get { return _imageSources[1]; }
            set {
                if (_imageSources[1] != value) {
                    _imageSources[1] = value;
                    RaisePropertyChanged("ImageSource1");
                }
            }
        }

        public ImageSource PreviousImageSource {
            get { return _previousImageSource; }
            set {
                if (_previousImageSource != value) {
                    _previousImageSource = value;
                    RaisePropertyChanged("PreviousImageSource");
                }
            }
        }

        public ImageSource NextImageSource {
            get { return _nextImageSource; }
            set {
                if (_nextImageSource != value) {
                    _nextImageSource = value;
                    RaisePropertyChanged("NextImageSource");
                }
            }
        }

        #endregion

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Methods

        public void SetDirectory(string directoryPath, List<string> types, int index) {
            // get all files
            CurrentDirectory = directoryPath;
            string[] files = types.SelectMany(f => Directory.GetFiles(CurrentDirectory, f)).ToArray();
            Array.Sort(files);

            // set files
            Files = files;

            // clear image cache
            _imageCache.Clear();

            // set index
            _index = -1;
            Index = index;
        }

        public void SetImage(string fileName, List<string> types) {
            // get all files
            CurrentDirectory = Path.GetDirectoryName(fileName);
            string[] files = types.SelectMany(f => Directory.GetFiles(CurrentDirectory, f)).ToArray();
            Array.Sort(files);

            // set files
            Files = files;

            // clear image cache
            _imageCache.Clear();

            // set index
            _index = -1;
            Index = Array.IndexOf(Files, fileName);
        }

        internal void DeleteFile(int p) {
            string fileToDelete = _files[p];

            // update files
            Files = Files.Where((val, idx) => idx != p).ToArray();

            // clear image cache
            _imageCache.Clear();

            // set index
            int oldIndex = _index;
            _index = -1;
            Index = oldIndex;

            if (Properties.Settings.Default.SendToRecycleBin) {
                // remove file to recycle bin
                FileSystem.DeleteFile(fileToDelete, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
            } else {
                // move file to deleteDestination
                FileSystem.MoveFile(fileToDelete, Path.Combine(CurrentDirectory, Properties.Settings.Default.DeleteDestination, Path.GetFileName(fileToDelete)));
            }
        }

        private void RaisePropertyChanged(string propertyName) {
            if (propertyName == "Index") {
                IndexChanged();
                return;
            }

            // take a copy to prevent thread issues
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void IndexChanged() {
            startAsync();

            if (_files.Length == 0) {
                Progress = 0;
            } else if (_files.Length <= 2) {
                Progress = 100;
            } else {
                Progress = (int)(100.0 * (double)Index / ((double)_files.Length - 2));
            }
        }

        private void startAsync() {
            if (_backgroundWorker.IsBusy == true) {
                cancelAsync();
            } else {
                // Start the asynchronous operation.
                _backgroundWorker.RunWorkerAsync();
            }
        }

        private void cancelAsync() {
            if (_backgroundWorker.WorkerSupportsCancellation == true) {
                // Cancel the asynchronous operation.
                _backgroundWorker.CancelAsync();
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e) {
            BackgroundWorker worker = sender as BackgroundWorker;
            ImageSource imageSource = null;

            for (int i = -1; i <= 2; i++) {
                if (worker.CancellationPending == true) {
                    e.Cancel = true;
                    break;
                } else {
                    int p = _index + i;

                    // load images
                    if (!_imageCache.TryGetValue(p, out imageSource)) {
                        if (p >= 0 && p < _files.Length) {
                            // Thread.Sleep(10000);
                            imageSource = BitmapFromUri(new Uri(_files[p]));
                            imageSource.Freeze();
                            _imageCache[p] = imageSource;
                        }
                    }

                    if (p == _index || p == _index + 1) {
                        worker.ReportProgress(p);
                    }
                }
            }

            if (e.Cancel) {
                return;
            }

            // load background image source
            if (_imageCache.TryGetValue(_index - 1, out imageSource)) {
                PreviousImageSource = imageSource;
                Debug.WriteLine("-1) Load " + (_index - 1) + " completed");
            }

            if (_imageCache.TryGetValue(_index + 2, out imageSource)) {
                NextImageSource = imageSource;
                Debug.WriteLine(" 2) Load " + (_index + 2) + " completed");
            }

            // clear old images
            foreach (var item in _imageCache.Where(kvp => kvp.Key < _index - 1 || kvp.Key > _index + 2).ToList()) {
                Debug.WriteLine("Remove " + item.Key);
                _imageCache.Remove(item.Key);
            }
        }

        public static ImageSource BitmapFromUri(Uri source) {
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = source;
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.EndInit();
            return bitmap;
        }

        void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e) {
            int p = e.ProgressPercentage;

            // set image0
            if (p == _index) {
                showImage0(p);
            }

            // set image1
            if (p == _index + 1) {
                showImage1(p);
            }
        }

        private void showImage0(int p) {
            if (_files.Length == 0) {
                ImageInfo0 = null;
                ImageSource0 = null;
            } else {
                string fileName = _files[p];
                BitmapImage bi = (BitmapImage)_imageCache[p];

                ImageInfo0 = (p + 1) + "/" + _files.Length + "\n"
                    + fileName + "\n"
                    + bi.PixelWidth + " x " + bi.PixelHeight;
                ImageSource0 = bi;
                Debug.WriteLine(" 0) Load " + p + " completed");
            }
        }

        private void showImage1(int p) {
            if (_files.Length < 2) {
                ImageInfo1 = null;
                ImageSource1 = null;
            } else {
                string fileName = _files[p];
                BitmapImage bi = (BitmapImage)_imageCache[p];

                ImageInfo1 = (p + 1) + "/" + _files.Length + "\n"
                    + fileName + "\n"
                    + bi.PixelWidth + " x " + bi.PixelHeight;
                ImageSource1 = bi;
                Debug.WriteLine(" 1) Load " + p + " completed");
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            // do nothing
            Debug.WriteLine("backgroundWorker1_RunWorkerCompleted");
        }

        #endregion
    }
}
