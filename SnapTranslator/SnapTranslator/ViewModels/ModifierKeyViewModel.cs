using SnapTranslator.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnapTranslator.ViewModels
{
    public class ModifierKeyViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        #region Fields
        private enum Modifiers
        {
            None = 0x0,
            Alt = 0x1,
            Control = 0x2,
            Shift = 0x4,
            Win = 0x8
        }

        private List<ModifierKey> _modifierKeys = new List<ModifierKey>()
        {
            new ModifierKey("Ctrl + Shift"
                , (uint)(Modifiers.Control | Modifiers.Shift)
                , "Ctrl_Shift"),

            new ModifierKey("Ctrl + Alt"
                , (uint)(Modifiers.Control | Modifiers.Alt)
                , "Ctrl_Alt"),

            new ModifierKey("Ctrl + Alt + Shift"
                , (uint)(Modifiers.Control | Modifiers.Alt)
                , "Ctrl_Alt_Shift"),
        };

        #endregion

        #region Properties
        public List<ModifierKey> ModifierKeys
        {
            get { return _modifierKeys; }
            set
            {
                _modifierKeys = value;
                OnPropertyChanged(nameof(ModifierKeys));
            }
        }
        #endregion

        public ModifierKey? FindByCode(string code)
        {
            return ModifierKeys.SingleOrDefault(p => p.Code.Equals(code));
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
