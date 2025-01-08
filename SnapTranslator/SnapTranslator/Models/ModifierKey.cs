using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnapTranslator.Models
{
    public class ModifierKey
    {
        public ModifierKey(string displayName, uint modifier, string code)
        {
            DisplayName = displayName;
            Modifier = modifier;
            Code = code;
        }
        public string DisplayName { get; set; } = string.Empty;
        public string Code = string.Empty;
        public uint Modifier {get; set; }
    }
}
