using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnapTranslator.Services
{
    interface ITranslateSevice
    {
        Task<string?> Translate(string source, string target, string text);
    }
}
