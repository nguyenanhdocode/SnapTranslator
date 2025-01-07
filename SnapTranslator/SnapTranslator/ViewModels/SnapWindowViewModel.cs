using Accessibility;
using GalaSoft.MvvmLight.Messaging;
using SnapTranslator.Command;
using SnapTranslator.Helpers;
using SnapTranslator.Messages;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;

namespace SnapTranslator.ViewModels
{
    public class SnapWindowViewModel : INotifyPropertyChanged
    {
        #region Fields
        private BitmapImage? _bitmapImage;
        #endregion

        #region Properties
        public event PropertyChangedEventHandler? PropertyChanged;
        public BitmapImage? BitmapImage
        {
            get { return _bitmapImage; }
            set
            {
                _bitmapImage = value;
                OnPropertyChanged(nameof(BitmapImage));
            }
        }
        #endregion

        #region Commmands
        public ICommand CloseCommand { get; private set; }
        public ICommand OkCommand { get; private set; }
        #endregion

        public SnapWindowViewModel()
        {
            CloseCommand = new RelayCommand(OnClose);
            OkCommand = new RelayCommand(OnOk);
        }

        #region Methods

        private void OnClose(object? param)
        {
            var window = param as Window;
            if (window != null)
            {
                Messenger.Default.Send<RestoreMainWindowMessage>(new RestoreMainWindowMessage()
                {
                    Image = BitmapImage
                });
                window.Close();
            }
        }

        private void OnOk(object? param)
        {
            var window = param as Window;
            if (window == null)
                throw new ArgumentNullException("Window must be not null.");

            var rect = ScreenHelper.GetWindowPosition(window);
            window.WindowState = WindowState.Minimized;

            int x = (int)rect.Left;
            int y = (int)rect.Top;

            var dpi = ScreenHelper.GetDPI(window);

            int width = (int)(window.ActualWidth * dpi.dpiX);
            int height = (int)(window.ActualHeight * dpi.dpiY);

            var bitmap = ScreenHelper.CaptureScreen(x
                , y
                , width
                , height);

            BitmapImage = bitmap;

            Messenger.Default.Send<RestoreMainWindowMessage>(new RestoreMainWindowMessage()
            {
                Image = BitmapImage
            });

            window.Close();
        }
        #endregion

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
