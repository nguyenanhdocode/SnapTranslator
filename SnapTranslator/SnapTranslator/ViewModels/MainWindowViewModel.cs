using DesktopApp.Helpers;
using GalaSoft.MvvmLight.Messaging;
using SnapTranslator.Command;
using SnapTranslator.Helpers;
using SnapTranslator.Messages;
using SnapTranslator.Models;
using SnapTranslator.Services.Impl;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
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
using System.Windows.Threading;

namespace SnapTranslator.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        #region Fields
        private FixedDocument? _document;
        private bool _isShowDocViewer = false;
        private BitmapImage? _inputImage;
        private const string IMAGES_FILTER = "Image Files (*.jpg;*.jpeg;*.png;*.bmp;*.gif)|*.jpg;*.jpeg;*.png;*.bmp;*.gif";
        private Language? _selectedSrcLanguage, _selectedDestLanguage;
        private const string DEFAULT_SRC_LANG = "eng";
        private const string DEFAULT_DEST_LANG = "vie";
        private string? _inputText;
        private string? _outputText;
        private bool _isProccessing;
        #endregion

        #region Properties
        public event PropertyChangedEventHandler? PropertyChanged;
        public SupportLanguageViewModel SupportLanguageViewModel { get; private set; }
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

        public Language? SelectedSrcLanguage
        {
            get { return _selectedSrcLanguage; }
            set
            {
                _selectedSrcLanguage = value;
                InputText = string.Empty;
                OnPropertyChanged(nameof(SelectedSrcLanguage));
            }
        }

        public Language? SelectedDestLanguage
        {
            get { return _selectedDestLanguage; }
            set
            {
                _selectedDestLanguage = value;
                OutputText = string.Empty;
                OnPropertyChanged(nameof(SelectedDestLanguage));
            }
        }

        public string? InputText
        {
            get { return _inputText; }
            set
            {
                _inputText = value;
                OnPropertyChanged(nameof(InputText));
            }
        }

        public string? OutputText
        {
            get { return _outputText; }
            set
            {
                _outputText = value;
                OnPropertyChanged(nameof(OutputText));
            }
        }

        public bool IsProcessing
        {
            get { return _isProccessing; }
            set
            {
                _isProccessing = value;
                OnPropertyChanged(nameof(IsProcessing));
            }
        }

        public BitmapImage? InputImage
        {
            get { return _inputImage; }
            set
            {
                _inputImage = value;
                OnInputImageChange();
            }
        }
        #endregion

        #region Commands
        public ICommand BtnCapture_Cick_Command { get; private set; }
        public ICommand BtnClearImage_Cick_Command { get; private set; }
        public ICommand BtnSaveImage_Cick_Command { get; private set; }
        public ICommand BtnBrowseImage_Cick_Command { get; private set; }
        public ICommand BtnTranslate_Cick_Command { get; private set; }
        public ICommand BtnCopy_Click_Command { get; private set; }
        public ICommand Menu_HotKey_Command { get; private set; }

        #endregion

        public MainWindowViewModel()
        {
            BtnCapture_Cick_Command = new RelayCommand(OnBtnCapture_Click_Handler);
            BtnClearImage_Cick_Command = new RelayCommand(OnBtnClearImage_Click_Handler);
            BtnSaveImage_Cick_Command = new RelayCommand(OnBtnSaveImage_Click_Handler);
            BtnBrowseImage_Cick_Command = new RelayCommand(OnBtnBrowseImage_Click_Handler);
            BtnTranslate_Cick_Command = new RelayCommand(OnBtnTranslate_Click_Handler);
            BtnCopy_Click_Command = new RelayCommand(OnBtnCopy_Click_Handler);
            Menu_HotKey_Command = new RelayCommand(OnMenuHotkey_Click_Handler);

            Messenger.Default.Register<RestoreMainWindowMessage>(this, OnRestoreMainWindow);

            SupportLanguageViewModel = new SupportLanguageViewModel();

            SelectedSrcLanguage = SupportLanguageViewModel.Languages
                .SingleOrDefault(p => p.TessCode == DEFAULT_SRC_LANG);
            SelectedDestLanguage = SupportLanguageViewModel.Languages
                .SingleOrDefault(p => p.TessCode == DEFAULT_DEST_LANG);
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
            InputImage = null;
            IsShowDocViewer = false;
        }

        private void OnBtnSaveImage_Click_Handler(object? param)
        {
            if (InputImage == null)
                return;

            string datetime = DateTime.Now.ToString("ddMMyyyy_HHmmss");
            string fileName = $"SnapTranslator_{datetime}.png";

            string? savePath = DialogHelper.ShowSaveFileDialog(fileName);

            if (string.IsNullOrEmpty(savePath))
                return;

            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(InputImage));

            try
            {
                using (var stream = new FileStream(savePath, FileMode.Create))
                {
                    encoder.Save(stream);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã xảy ra lỗi khi lưu hình ảnh.\nChi tiết lỗi: {ex.Message}", "Lỗi"
                    , MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OnBtnBrowseImage_Click_Handler(object? param)
        {
            string? fileName = DialogHelper.ShowOpenFileDialog(IMAGES_FILTER);

            if (string.IsNullOrEmpty(fileName))
                return;

            if (SelectedSrcLanguage == null)
                return;

            try
            {
                var image = new BitmapImage(new Uri(fileName));

                InputImage = image;

                InputText = string.Empty;
                OutputText = string.Empty;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Không thể hiển thị hình ảnh.\nChi tiết lỗi: {ex.Message}", "Lỗi"
                    , MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void OnBtnTranslate_Click_Handler(object? param)
        {
            if (SelectedSrcLanguage == null || SelectedDestLanguage == null)
            {
                MessageBox.Show("Vui lòng chọn ngôn ngữ."
                    , "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (SelectedSrcLanguage.TessCode.Equals(SelectedDestLanguage.TessCode))
            {
                MessageBox.Show("Vui lòng chọn ngôn ngữ đích."
                    , "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrEmpty(InputText))
                return;

            var tran = new GoogleTranslateService(
                ConfigurationManager.AppSettings["GoogleTranslateAPIKey"] ?? string.Empty
                , ConfigurationManager.AppSettings["GoogleTranslateAPIUrl"] ?? string.Empty);

            IsProcessing = true;

            try
            {
                string? res = await tran.Translate(SelectedSrcLanguage.GoogleCode
                    , SelectedDestLanguage.GoogleCode
                    , InputText);
                OutputText = res ?? "";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã xảy ra lỗi trong quá trình dịch.\nChi tiết lỗi: {ex.Message}", "Lỗi"
                    , MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
            finally
            {
                IsProcessing = false;
            }
        }

        private void OnBtnCopy_Click_Handler(object? param)
        {
            Clipboard.SetText(OutputText);
        }

        private void OnRestoreMainWindow(RestoreMainWindowMessage message)
        {
            if (Application.Current.MainWindow == null)
                return;

            Application.Current.MainWindow.WindowState = WindowState.Normal;

            var pageContent = new PageContent();
            var fixedPage = new FixedPage();

            if (message.Image == null)
                return;

            InputImage = message.Image;

            InputText = string.Empty;
            OutputText = string.Empty;
        }

        private void OnMenuHotkey_Click_Handler(object? args)
        {
            var hotkeyWindow = new HotkeyWindow();
            hotkeyWindow.ShowDialog();
        }

        private async void OnInputImageChange()
        {
            if (InputImage == null)
            {
                IsShowDocViewer = false;
                return;
            }

            IsShowDocViewer = true;
            Document = ConvertBitmapImageToDocument(InputImage);

            if (SelectedSrcLanguage == null)
                return;

            InputText = string.Empty;
            OutputText = string.Empty;

            IsProcessing = true;
            await Task.Run(() =>
            {
                var ocr = new OCRHelper("tessdata", SelectedSrcLanguage.TessCode);
                InputText = ocr.Recognize(InputImage);
                IsProcessing = false;
            });
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
