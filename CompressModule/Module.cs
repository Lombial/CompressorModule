using Ionic.Zlib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

#pragma warning disable CS8603

namespace CompressModule
{
    public static class Module
    {
        static Dictionary<string, CompressedImage> _images = [];
        public static void LoadImage(string path, string key)
        {
            if (path[(path.Length - 4)..] == ".png")
            {
                BitmapImage bitmap = new();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(path, UriKind.Relative);
                bitmap.EndInit();
                _images.Add(key, new(bitmap));
            }
        }
        public static Image GetImage(string key) => _images.TryGetValue(key, out CompressedImage? value) ? new() { Source = value.Get() } : null;
        public static void DeleteImage(string key) => _images.Remove(key);
        public static void WriteImage(string key, string path)
        {
            if (_images.TryGetValue(key, out CompressedImage? value))
                value.Write(path);
        }
        public static void ReadImage(string path, string key)
        {

                _images.Add(key, new(path));

        }
    }
    internal class CompressedImage
    {
        int size;
        byte[] data = [];
        public CompressedImage(BitmapSource img)
        {
            int width = img.PixelWidth;
            int height = img.PixelHeight;
            int stride = img.PixelWidth * (img.Format.BitsPerPixel / 8);
            int size = img.PixelHeight * stride;
            byte[] data = new byte[size];
            Color[,] pixels = new Color[width, height];
            img.CopyPixels(data, stride, 0);
            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                {
                    int ind = (j * stride) + (i * 4);
                    pixels[i, j] = Color.FromArgb(data[ind + 3], data[ind + 2], data[ind + 1], data[ind]);
                }
            using MemoryStream ms = new();
            using BinaryWriter bw = new(ms);
            using DeflateStream ds = new(bw.BaseStream, 0);
            using BinaryWriter bw2 = new(ds);
            bw2.Write(width);
            bw2.Write(height);
            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                {
                    bw2.Write(pixels[i, j].R);
                    bw2.Write(pixels[i, j].G);
                    bw2.Write(pixels[i, j].B);
                    bw2.Write(pixels[i, j].A);
                }
            this.data = ms.ToArray();
            this.size = pixels.Length;
            MessageBox.Show($"{this.data.Length} {pixels.Length * 4}");
        }
        public CompressedImage(string path)
        {
            using FileStream fs = new(path, FileMode.Open);
            using BinaryReader br = new(fs);
            size = br.ReadInt32();
            int length = br.ReadInt32();
            data = new byte[length];
            for (int i = 0; i < length; i++)
                data[i] = br.ReadByte();
        }
        public BitmapSource Get()
        {
            byte[] bytes = new byte[size];
            for (int i = 0; i < data.Length; i++)
                bytes[i] = data[i];
            using MemoryStream ms = new(bytes);
            using DeflateStream ds = new(ms, CompressionMode.Decompress, true);
            using BinaryReader br2 = new(ds);
            int width = br2.ReadInt32();
            int height = br2.ReadInt32();
            Color[,] pixels = new Color[width, height];
            for (int i = 0; i < width; i++)
                for (int j = 0; j < height ; j++)
                {
                    byte r = br2.ReadByte();
                    byte g = br2.ReadByte();
                    byte b = br2.ReadByte();
                    byte a = br2.ReadByte();
                    pixels[i, j] = Color.FromArgb(a, r, g, b);
                }
            WriteableBitmap wb = new(width, height, 96, 96, PixelFormats.Bgra32, null);
            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                    wb.WritePixels(new(i, j, 1, 1), new byte[] { pixels[i, j].B, pixels[i, j].G, pixels[i, j].R, pixels[i, j].A }, 4, 0);
            return wb;
        }
        public void Write(string path)
        {
            using FileStream fs = new(path, FileMode.Create);
            using BinaryWriter bw = new(fs);
            bw.Write(size);
            bw.Write(data.Length);
            foreach (byte i in data)
                bw.Write(i);
        }
    }
}
