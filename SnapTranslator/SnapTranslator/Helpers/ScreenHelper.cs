using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SnapTranslator.Helpers
{
    public class ScreenHelper
    {
        #region Consts, Stuctrues
        const int SRCCOPY = 0x00CC0020;

        [StructLayout(LayoutKind.Sequential)]
        private struct BITMAPINFO
        {
            public int biSize;
            public int biWidth;
            public int biHeight;
            public short biPlanes;
            public short biBitCount;
            public int biCompression;
            public int biSizeImage;
            public int biXPelsPerMeter;
            public int biYPelsPerMeter;
            public int biClrUsed;
            public int biClrImportant;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct BITMAPINFOHEADER
        {
            public uint biSize;
            public int biWidth;
            public int biHeight;
            public ushort biPlanes;
            public ushort biBitCount;
            public uint biCompression;
            public uint biSizeImage;
            public int biXPelsPerMeter;
            public int biYPelsPerMeter;
            public uint biClrUsed;
            public uint biClrImportant;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct BITMAP
        {
            public int bmType;
            public int bmWidth;
            public int bmHeight;
            public int bmWidthBytes;
            public ushort bmPlanes;
            public ushort bmBitsPixel;
            public IntPtr bmBits;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        #endregion

        #region Win32 Hooks
        [DllImport("gdi32.dll")]
        private static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int nWidth, int nHeight);

        [DllImport("gdi32.dll")]
        private static extern IntPtr CreateCompatibleDC(IntPtr hdc);

        [DllImport("gdi32.dll")]
        private static extern bool DeleteDC(IntPtr hdc);

        [DllImport("gdi32.dll")]
        private static extern bool DeleteObject(IntPtr hObject);

        [DllImport("gdi32.dll")]
        private static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);

        [DllImport("gdi32.dll")]
        private static extern bool BitBlt(IntPtr hdcDest, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, int dwRop);

        [DllImport("user32.dll")]
        private static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("user32.dll")]
        private static extern int GetSystemMetrics(int nIndex);

        [DllImport("gdi32.dll")]
        private static extern int GetObject(IntPtr hObject, int nCount, ref BITMAP lpObject);

        [DllImport("gdi32.dll")]
        private static extern int GetDIBits(IntPtr hdc, IntPtr hBitmap, uint uStartScan, uint cScanLines,
        [Out] byte[] lpvBits, ref BITMAPINFO lpbmi, uint uUsage);

        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        #endregion

        public static BitmapImage CaptureScreen(int x, int y, int width, int height)
        {
            IntPtr hdcSrc = GetDC(IntPtr.Zero);
            IntPtr hdcDest = CreateCompatibleDC(hdcSrc);
            IntPtr hBitmap = CreateCompatibleBitmap(hdcSrc, width, height);
            IntPtr hOld = SelectObject(hdcDest, hBitmap);

            BitBlt(hdcDest, 0, 0, width, height, hdcSrc, x, y, SRCCOPY);
            byte[] byteArray = HBitmapToByteArray(hBitmap, width, height);

            SelectObject(hdcDest, hOld);
            DeleteDC(hdcDest);
            ReleaseDC(IntPtr.Zero, hdcSrc);
            DeleteObject(hBitmap);

            return ByteArrayToBitmapImage(byteArray, width, height);
        }

        static byte[] HBitmapToByteArray(IntPtr hBitmap, int width, int height)
        {
            // Define the BITMAPINFO structure
            BITMAPINFO bmi = new BITMAPINFO
            {
                biSize = Marshal.SizeOf(typeof(BITMAPINFOHEADER)),
                biWidth = width,
                biHeight = -height, // Negative to indicate a top-down bitmap
                biPlanes = 1,
                biBitCount = 32, // 32 bits per pixel (BGRA format)
                biCompression = 0 // BI_RGB (no compression)
            };

            int byteCount = width * height * 4; // 4 bytes per pixel (BGRA)
            byte[] pixels = new byte[byteCount];

            // Get the raw pixel data from the HBITMAP
            IntPtr hdc = GetDC(IntPtr.Zero);
            int result = GetDIBits(hdc, hBitmap, 0, (uint)height, pixels, ref bmi, 0);
            ReleaseDC(IntPtr.Zero, hdc);

            if (result == 0)
            {
                throw new Exception("Failed to retrieve bitmap data.");
            }

            return pixels;
        }

        public static BitmapImage ByteArrayToBitmapImage(byte[] pixelData, int width, int height)
        {
            // Create a WriteableBitmap with the specified dimensions
            var writeableBitmap = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgra32, null);

            // Write the pixel data into the WriteableBitmap
            writeableBitmap.WritePixels(new Int32Rect(0, 0, width, height), pixelData, width * 4, 0);

            // Convert WriteableBitmap to BitmapImage
            return WriteableBitmapToBitmapImage(writeableBitmap);
        }

        private static BitmapImage WriteableBitmapToBitmapImage(WriteableBitmap writeableBitmap)
        {
            // Save the WriteableBitmap to a memory stream in PNG format
            BitmapImage bitmapImage;
            using (var memoryStream = new MemoryStream())
            {
                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(writeableBitmap));
                encoder.Save(memoryStream);

                // Load the PNG image from the memory stream into a BitmapImage
                bitmapImage = new BitmapImage();
                memoryStream.Seek(0, SeekOrigin.Begin);
                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = memoryStream;
                bitmapImage.EndInit();
                bitmapImage.Freeze(); // Make it thread-safe
            }

            return bitmapImage;
        }

        public static (double dpiX, double dpiY) GetDPI(Window window)
        {
            var source = PresentationSource.FromVisual(window);
            if (source == null || source.CompositionTarget == null)
                return (1.0, 10.0);

            var dpiX = source.CompositionTarget.TransformToDevice.M11;
            var dpiY = source.CompositionTarget.TransformToDevice.M22;
            return (dpiX, dpiY);
        }

        public static (double Left, double Top, double Right, double Bottom) GetWindowPosition(Window window)
        {
            var hwnd = new System.Windows.Interop.WindowInteropHelper(window).Handle;
            if (GetWindowRect(hwnd, out RECT rect))
            {
                return (rect.Left, rect.Top, rect.Right, rect.Bottom);
            }
            throw new InvalidOperationException("Unable to get window rectangle.");
        }
    }
}
