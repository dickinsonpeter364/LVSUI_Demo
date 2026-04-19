using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Text;

namespace ComMatcherTest
{
    public class MatcherHelper
    {
        static byte[] LoadImageToBytes(string filePath, out int width, out int height, out int channels)
        {
            using (Bitmap bmp = new Bitmap(filePath))
            {
                width = bmp.Width;
                height = bmp.Height;
                channels = 3;

                BitmapData data = bmp.LockBits(
                    new Rectangle(0, 0, width, height),
                    ImageLockMode.ReadOnly,
                    PixelFormat.Format24bppRgb
                );

                int stride = data.Stride;
                int totalBytes = Math.Abs(stride) * height;
                byte[] rawBytes = new byte[totalBytes];
                Marshal.Copy(data.Scan0, rawBytes, 0, totalBytes);
                bmp.UnlockBits(data);

                if (stride == width * channels) return rawBytes;

                byte[] compactBytes = new byte[width * height * channels];
                for (int y = 0; y < height; y++)
                {
                    Array.Copy(rawBytes, y * stride, compactBytes, y * width * channels, width * channels);
                }

                return compactBytes;
            }
        }
    }
}
