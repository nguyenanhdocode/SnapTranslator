using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnapTranslator.Helpers
{
    public class FileHelper
    {
        public static string? ReadAllText(string filename)
        {
			try
			{
				return File.ReadAllText(filename);
			}
			catch (Exception)
			{
				return null;
			}
        }
    }
}
