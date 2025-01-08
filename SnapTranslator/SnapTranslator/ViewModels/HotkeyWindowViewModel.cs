using SnapTranslator.Command;
using SnapTranslator.Helpers;
using SnapTranslator.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace SnapTranslator.ViewModels
{
    public class HotkeyWindowViewModel : INotifyPropertyChanged
    {
        #region Fields
        private KeyCombination _captureHotKey;
        private List<char> _virtualKeys = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray()
            .ToList();
        private const string HK_FOLDER_NAME = "Hotkeys";
        private const string CAPTURE_HK_FILE_NAME = "Capture.txt";
        #endregion

        #region Properties
        public event PropertyChangedEventHandler? PropertyChanged;
        public KeyCombination CaptureHotKey
        {
            get { return _captureHotKey; }
            set
            {
                _captureHotKey = value;
            }
        }
        public ModifierKeyViewModel ModifierKeyViewModel { get; private set; }

        public List<char> VirtualKeys
        {
            get { return _virtualKeys; }
            set
            {
                _virtualKeys = value;
                OnPropertyChanged(nameof(VirtualKeys));
            }
        }
        #endregion

        #region Commands
        public ICommand BtnSave_Click_Command { get; private set; }
        #endregion

        public HotkeyWindowViewModel()
        {
            BtnSave_Click_Command = new RelayCommand(BtnSave_Click_Handler);
            ModifierKeyViewModel = new ModifierKeyViewModel();

            _captureHotKey = new KeyCombination
            {
                ModifierKey = ModifierKeyViewModel.ModifierKeys[0],
            };

            CaptureHotKey = ReadKeyCombinationFromFile(Path.Combine(HK_FOLDER_NAME, CAPTURE_HK_FILE_NAME))
                ?? new KeyCombination { ModifierKey = ModifierKeyViewModel.ModifierKeys[0] };
        }

        private void BtnSave_Click_Handler(object? args)
        {
            string? type = args?.ToString();

            if (type == null)
                return;

            if (type.Equals("Capture"))
            {
                try
                {
                    File.WriteAllText(Path.Combine(HK_FOLDER_NAME, CAPTURE_HK_FILE_NAME)
                        , CaptureHotKey.ToString());
                    MessageBox.Show("Cập nhật phím tắt thành công.", "Thông báo"
                        , MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Đã xảy ra lỗi khi lưu cấu hình.\nChi tiết lỗi: {ex.Message}", "Lỗi"
                    , MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public KeyCombination? ReadKeyCombinationFromFile(string filename)
        {
            var str = FileHelper.ReadAllText(filename);

            if (string.IsNullOrEmpty(str)) return null;

            var key = new KeyCombination();

            var parts = str.Split("|").ToList();

            if (parts.Count != 2 || parts[1].Length != 1)
            {
                return null;
            }

            key.ModifierKey = ModifierKeyViewModel.FindByCode(parts[0]);
            key.Key = parts[1].Length == 1 ? parts[1].ToCharArray()[0] : '\0';

            return key;
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
