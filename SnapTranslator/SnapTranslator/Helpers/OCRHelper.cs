using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Tesseract;

namespace DesktopApp.Helpers
{
    public class OCRHelper
    {
        public string TrainedDataPath { get; private set; } = string.Empty;
        public string Lang { get; private set; } = string.Empty;

        public OCRHelper(string trainedDataPath, string lang)
        {
            if (string.IsNullOrEmpty(trainedDataPath))
                throw new ArgumentException($"Invalid TrainedDataPath");

            if (string.IsNullOrEmpty(lang))
                throw new ArgumentException($"Invalid Lang");

            TrainedDataPath = trainedDataPath;
            Lang = lang;
        }

        public string Recognize(BitmapImage bitmap)
        {
            string result = string.Empty;
            using (var engine = new TesseractEngine(TrainedDataPath, Lang))
            {
                var imgBytes = BitmapImageToByteArray(bitmap);

                if (imgBytes == null)
                    throw new Exception("Image byte array is null");

                var pix = Pix.LoadFromMemory(imgBytes);

                using (var page = engine.Process(pix))
                {
                    result = page.GetText();
                }
            }

            return result;
        }

        public static byte[] BitmapImageToByteArray(BitmapImage bitmapImage)
        {
            if (bitmapImage == null)
                throw new ArgumentNullException(nameof(bitmapImage));

            using (MemoryStream memoryStream = new MemoryStream())
            {
                BitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmapImage));
                encoder.Save(memoryStream);
                return memoryStream.ToArray();
            }
        }
    }
}
