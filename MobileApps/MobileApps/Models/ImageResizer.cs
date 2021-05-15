using System;
using System.IO;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace MobileApps.Models
{
    public static class ImageResizer
    {
        private const string serverUrl = "http://188.225.83.42:7000";
        public static string ResizeImage(int size, int quality, FileResult streamImage)
        {
            using (var input = streamImage.OpenReadAsync().Result)
            {
                using (var inputStream = new SKManagedStream(input))
                {
                    using (var original = SKBitmap.Decode(inputStream))
                    {
                        int width, height;
                        if (original.Width > original.Height)
                        {
                            width = size;
                            height = original.Height * size / original.Width;
                        }
                        else
                        {
                            width = original.Width * size / original.Height;
                            height = size;
                        }

                        using (var resized = original.Resize(new SKImageInfo(width, height), SKBitmapResizeMethod.Lanczos3))
                        {
                            if (resized == null) throw new Exception("Error resized.");

                            using (var image = SKImage.FromBitmap(resized))
                            {
                                string outputPath = streamImage.FullPath.Remove(streamImage.FullPath.Length - streamImage.FileName.Length, streamImage.FileName.Length);
                                using (var output =
                                    File.OpenWrite($"{outputPath}/{DateTime.Now:O}_BadDrive.jpg"))
                                {
                                    image.Encode(SKEncodedImageFormat.Jpeg, quality).SaveTo(output);
                                    return output.Name;
                                }
                            }
                        }
                    }
                }
            }
        }

        public static ImageSource GetResizeImageSourceFromUrl(int size, int quality, string url)
        {
            var httpClient = new System.Net.Http.HttpClient();
            var bytes = httpClient.GetByteArrayAsync(serverUrl + url).Result;
            var stream = new MemoryStream(bytes);
            var bitmap = SKBitmap.Decode(stream);
            int width, height;
            //Считаем новые размеры исходя из оригенального размера
            if (bitmap.Width > bitmap.Height)
            {
                width = size;
                height = bitmap.Height * size / bitmap.Width;
            }
            else
            {
                width = bitmap.Width * size / bitmap.Height;
                height = size;
            }
            //Создаем картинку с новым размером 
            using (var resized = bitmap.Resize(new SKImageInfo(width, height), SKBitmapResizeMethod.Lanczos3))
            {
                if (resized == null) throw new Exception("Error resized.");

                using (var image = SKImage.FromBitmap(resized))
                {
                    //return ImageSource.FromStream(image.Encode().AsStream);
                    SKData encodedData = image.Encode(SKEncodedImageFormat.Jpeg, 100);
                    string folder = Environment.GetFolderPath(System.Environment.SpecialFolder.MyPictures);
                    string filename = $"{Guid.NewGuid()}.jpeg";
                    string imagePath = Path.Combine(FileSystem.CacheDirectory, filename);
                    var bitmapImageStream = File.Open(imagePath, 
                                                    FileMode.Create, 
                                                    FileAccess.Write, 
                                                    FileShare.None);
                    encodedData.SaveTo(bitmapImageStream);
                    bitmapImageStream.Flush(true);
                    bitmapImageStream.Dispose();
                    return ImageSource.FromFile(imagePath);
                }
            }
        }
    }
}