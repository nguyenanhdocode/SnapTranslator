using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnapTranslator.Models
{
    public class KeyCombination
    {
        public ModifierKey? ModifierKey { get; set; }
        public char Key { get; set; }

        public bool IsValid()
        {
            return ModifierKey != null && ModifierKey.Modifier > 0
                && Key != '\0';
        }

        public override string ToString()
        {
            return IsValid() ? $"{ModifierKey.Code}|{Key}" : string.Empty;
        }
    }
}
