using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnapTranslator.Helpers
{
    public class DialogHelper
    {
        public static string? ShowSaveFileDialog(string defaultFileName
            , string filter = "All Files (*.*)|*.*")
        {
            var dialog = new SaveFileDialog()
            {
                FileName = defaultFileName,
                Filter = filter
            };

            bool? result = dialog.ShowDialog();

            return result == true ? dialog.FileName : null;
        }

        public static string? ShowOpenFileDialog(string filter)
        {
            var dialog = new OpenFileDialog()
            {
                Filter = filter
            };

            bool? result = dialog.ShowDialog();

            return result == true ? dialog.FileName : null;
        }
    }
}
