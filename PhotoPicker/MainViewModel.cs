using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace PhotoPicker
{
    class MainViewModel : INotifyPropertyChanged
    {
        #region Construction

        public MainViewModel()
        {
            _backgroundWorker.WorkerReportsProgress = true;
            _backgroundWorker.WorkerSupportsCancellation = true;

            _backgroundWorker.DoWork += backgroundWorker1_DoWork;
            _backgroundWorker.ProgressChanged += backgroundWorker1_ProgressChanged;
            _backgroundWorker.RunWorkerCompleted += backgroundWorker1_RunWorkerCompleted;
        }

        #endregion

        #region Members
        private string[] _files = new string[0];
        private int _index = -1;
        private string[] _imageInfos = new string[2];
        private BitmapImage[] _imageSources = new BitmapImage[2];
        private BitmapImage _previousImageSource = null;
        private BitmapImage _nextImageSource = null;
        private Dictionary<int, BitmapImage> _imageCache = new Dictionary<int, BitmapImage>();
        private readonly BackgroundWorker _backgroundWorker = new BackgroundWorker();
        #endregion

        #region Properties

        public string[] Files
        {
            get { return _files; }
            set
            {
                if (_files != value)
                {
                    _files = value;
                    RaisePropertyChanged("Files");
                }
            }
        }

        public int Index
        {
            get { return _index; }
            set
            {
                if (_files == null || _files.Length <= 1)
                {
                    return;
                }

                int newValue = -1;

                if (value < 0)
                {
                    newValue = 0;
                }
                else if (value > _files.Length - 2)
                {
                    newValue = _files.Length - 2;
                }
                else
                {
                    newValue = value;
                }

                if (_index != newValue)
                {
                    _index = newValue;
                    RaisePropertyChanged("Index");
                }
            }
        }

        public string ImageInfo0
        {
            get { return _imageInfos[0]; }
            set
            {
                if (_imageInfos[0] != value)
                {
                    _imageInfos[0] = value;
                    RaisePropertyChanged("ImageInfo0");
                }
            }
        }

        public string ImageInfo1
        {
            get { return _imageInfos[1]; }
            set
            {
                if (_imageInfos[1] != value)
                {
                    _imageInfos[1] = value;
                    RaisePropertyChanged("ImageInfo1");
                }
            }
        }

        public BitmapImage ImageSource0
        {
            get { return _imageSources[0]; }
            set
            {
                if (_imageSources[0] != value)
                {
                    _imageSources[0] = value;
                    RaisePropertyChanged("ImageSource0");
                }
            }
        }

        public BitmapImage ImageSource1
        {
            get { return _imageSources[1]; }
            set
            {
                if (_imageSources[1] != value)
                {
                    _imageSources[1] = value;
                    RaisePropertyChanged("ImageSource1");
                }
            }
        }

        public BitmapImage PreviousImageSource
        {
            get { return _previousImageSource; }
            set
            {
                if (_previousImageSource != value)
                {
                    _previousImageSource = value;
                    RaisePropertyChanged("PreviousImageSource");
                }
            }
        }

        public BitmapImage NextImageSource
        {
            get { return _nextImageSource; }
            set
            {
                if (_nextImageSource != value)
                {
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

        public void setImage(string fileName, List<string> types)
        {
            // get all files
            String directory = Path.GetDirectoryName(fileName);
            string[] files = types.SelectMany(f => Directory.GetFiles(directory, f)).ToArray();
            Array.Sort(files);
            Files = files;

            // clear image cache
            _imageCache.Clear();

            // set index
            _index = -1;
            Index = Array.IndexOf(Files, fileName);
        }

        private void RaisePropertyChanged(string propertyName)
        {
            if (propertyName == "Index")
            {
                IndexChanged();
                return;
            }

            // take a copy to prevent thread issues
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void IndexChanged()
        {
            startAsync();
        }

        private void startAsync()
        {
            if (_backgroundWorker.IsBusy == true)
            {
                cancelAsync();
            }
            else
            {
                // Start the asynchronous operation.
                _backgroundWorker.RunWorkerAsync();
            }
        }

        private void cancelAsync()
        {
            if (_backgroundWorker.WorkerSupportsCancellation == true)
            {
                // Cancel the asynchronous operation.
                _backgroundWorker.CancelAsync();
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            BitmapImage bitmapImage = null;

            for (int i = -1; i <= 2; i++)
            {
                if (worker.CancellationPending == true)
                {
                    e.Cancel = true;
                    break;
                }
                else
                {
                    int p = _index + i;

                    // load images
                    if (!_imageCache.TryGetValue(p, out bitmapImage))
                    {
                        if (p >= 0 && p < _files.Length)
                        {
                            // Thread.Sleep(10000);
                            bitmapImage = new BitmapImage(new Uri(_files[p]));
                            bitmapImage.Freeze();
                            _imageCache[p] = bitmapImage;
                        }
                    }

                    if (p == _index || p == _index + 1)
                    {
                        worker.ReportProgress(p);
                    }
                }
            }

            if (e.Cancel)
            {
                return;
            }

            // load background image source
            if (_imageCache.TryGetValue(_index - 1, out bitmapImage))
            {
                PreviousImageSource = bitmapImage;
                Debug.WriteLine("Load " + (_index - 1) + " completed");
            }

            if (_imageCache.TryGetValue(_index + 2, out bitmapImage))
            {
                NextImageSource = bitmapImage;
                Debug.WriteLine("Load " + (_index + 2) + " completed");
            }

            // clear old images
            foreach (var item in _imageCache.Where(kvp => kvp.Key < _index - 1 || kvp.Key > _index + 2).ToList())
            {
                Debug.WriteLine("Remove " + item.Key);
                _imageCache.Remove(item.Key);
            }
        }

        void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            int p = e.ProgressPercentage;

            // set image0
            if (p == _index)
            {
                showImage0(p);
            }

            // set image1
            if (p == _index + 1)
            {
                showImage1(p);
            }
        }

        private void showImage0(int p)
        {
            string fileName = _files[p];
            BitmapImage bi = _imageCache[p];

            ImageInfo0 = (p + 1) + "/" + _files.Length + "\n"
                + fileName + "\n"
                + bi.PixelWidth + " x " + bi.PixelHeight;
            ImageSource0 = bi;
            Debug.WriteLine("Load " + p + " completed");
        }

        private void showImage1(int p)
        {
            string fileName = _files[p];
            BitmapImage bi = _imageCache[p];

            ImageInfo1 = (p + 1) + "/" + _files.Length + "\n"
                + fileName + "\n"
                + bi.PixelWidth + " x " + bi.PixelHeight;
            ImageSource1 = bi;
            Debug.WriteLine("Load " + p + " completed");
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // do nothing
            Debug.WriteLine("backgroundWorker1_RunWorkerCompleted");
        }

        #endregion
    }
}
