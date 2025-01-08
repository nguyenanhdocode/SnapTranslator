using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SnapTranslator.Models
{
    public class Language
    {
        public string Name { get; set; } = string.Empty;
        public string TessCode { get; set; } = string.Empty;
        public string GoogleCode { get; set; } = string.Empty;
    }
}
