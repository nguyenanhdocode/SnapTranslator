using SnapTranslator.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SnapTranslator.ViewModels
{
    public class SupportLanguageViewModel : INotifyPropertyChanged
    {
        #region Fields
        private const string LANGS_FILE_NAME = "SupportLangs.json";

        private List<Language> _languages = new List<Language>();
        #endregion

        #region Properties
        public List<Language> Languages
        {
            get { return _languages; }
            set 
            {
                _languages = value;
                OnPropertyChanged(nameof(Languages));
            }
        }

        #endregion

        public SupportLanguageViewModel()
        {
            Languages = LoadLanguages()
                .OrderBy(x => x.Name).ToList();
        }

        public List<Language> LoadLanguages()
        {
            string filename = Path.Combine(Environment.CurrentDirectory, LANGS_FILE_NAME);
            if (!File.Exists(filename))
                return new List<Language>();

            string jsonData = File.ReadAllText(filename);
            var result = JsonSerializer.Deserialize<List<Language>>(jsonData);

            return result ?? new List<Language>();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
