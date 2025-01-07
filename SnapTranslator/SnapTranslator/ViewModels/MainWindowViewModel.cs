using GalaSoft.MvvmLight.Messaging;
using SnapTranslator.Command;
using SnapTranslator.Messages;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        #region Fields
        private FixedDocument? _document;
        private bool _isShowDocViewer = false;
        private BitmapImage? _screenShot;
        #endregion

        #region Properties
        public event PropertyChangedEventHandler? PropertyChanged;
        public FixedDocument? Document
        {
            get { return _document; }
            set
            {
                _document = value;
                OnPropertyChanged(nameof(Document));
            }
        }

        public bool IsShowDocViewer
        {
            get { return _isShowDocViewer; }
            set
            {
                _isShowDocViewer = value;
                OnPropertyChanged(nameof(IsShowDocViewer));
            }
        }
        #endregion

        #region Commands
        public ICommand BtnCapture_Cick_Command { get; private set; }
        public ICommand BtnClearImage_Cick_Command { get; private set; }
        public ICommand BtnSaveImage_Cick_Command { get; private set; }
        public ICommand BtnBrowseImage_Cick_Command { get; private set; }
        public ICommand BtnTranslate_Cick_Command { get; private set; }
        public ICommand BtnCopy_Cick_Command { get; private set; }
        #endregion

        public MainWindowViewModel()
        {
            BtnCapture_Cick_Command = new RelayCommand(OnBtnCapture_Click_Handler);
            BtnClearImage_Cick_Command = new RelayCommand(OnBtnClearImage_Click_Handler);
            BtnSaveImage_Cick_Command = new RelayCommand(OnBtnSaveImage_Click_Handler);
            BtnBrowseImage_Cick_Command = new RelayCommand(OnBtnBrowseImage_Click_Handler);
            BtnTranslate_Cick_Command = new RelayCommand(OnBtnTranslate_Click_Handler);
            BtnCopy_Cick_Command = new RelayCommand(OnBtnCopy_Click_Handler);

            Messenger.Default.Register<RestoreMainWindowMessage>(this, OnRestoreMainWindow);
        }

        private void OnBtnCapture_Click_Handler(object? param)
        {
            var snapWindow = new SnapWindow();
            snapWindow.Show();
            Application.Current.MainWindow.WindowState = WindowState.Minimized;
        }

        private void OnBtnClearImage_Click_Handler(object? param)
        {
            Document = null;
            IsShowDocViewer = false;
        }

        private void OnBtnSaveImage_Click_Handler(object? param)
        {
            if (_screenShot != null)
            {
                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(_screenShot));



                using (var stream = new FileStream("F:\\test.png", FileMode.Create))
                {
                    encoder.Save(stream);
                }
            }
        }

        private void OnBtnBrowseImage_Click_Handler(object? param)
        {

        }

        private void OnBtnTranslate_Click_Handler(object? param)
        {

        }

        private void OnBtnCopy_Click_Handler(object? param)
        {

        }

        private void OnRestoreMainWindow(RestoreMainWindowMessage message)
        {
            if (Application.Current.MainWindow == null)
                return;

            Application.Current.MainWindow.WindowState = WindowState.Normal;

            var pageContent = new PageContent();
            var fixedPage = new FixedPage();

            if (message.Image != null)
            {
                IsShowDocViewer = true;
                _screenShot = message.Image;
                Document = ConvertBitmapImageToDocument(message.Image);
            }
        }

        private FixedDocument ConvertBitmapImageToDocument(BitmapImage bitmapImage)
        {
            var pageContent = new PageContent();
            var fixedPage = new FixedPage();

            fixedPage.Width = bitmapImage.Width;
            fixedPage.Height = bitmapImage.Height;

            Image image = new Image
            {
                Source = bitmapImage,
                Stretch = Stretch.Uniform
            };

            FixedPage.SetLeft(image, 0);
            FixedPage.SetTop(image, 0);
            fixedPage.Children.Add(image);

            ((IAddChild)pageContent).AddChild(fixedPage);

            var doc = new FixedDocument();
            doc.Pages.Add(pageContent);

            return doc;
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
