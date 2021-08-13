using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using SkiaSharp;

namespace SkImageResizer
{
    public class SKImageProcess
    {
        /// <summary>
        /// 進行圖片的縮放作業
        /// </summary>
        /// <param name="sourcePath">圖片來源目錄路徑</param>
        /// <param name="destPath">產生圖片目的目錄路徑</param>
        /// <param name="scale">縮放比例</param>
        public void ResizeImages(string sourcePath, string destPath, double scale)
        {
            if (!Directory.Exists(destPath))
            {
                Directory.CreateDirectory(destPath);
            }

            var allFiles = FindImages(sourcePath);
            foreach (var filePath in allFiles)
            {
                var bitmap = SKBitmap.Decode(filePath);
                var imgPhoto = SKImage.FromBitmap(bitmap);
                var imgName = Path.GetFileNameWithoutExtension(filePath);

                var sourceWidth = imgPhoto.Width;
                var sourceHeight = imgPhoto.Height;

                var destinationWidth = (int)(sourceWidth * scale);
                var destinationHeight = (int)(sourceHeight * scale);

                using var scaledBitmap = bitmap.Resize(
                    new SKImageInfo(destinationWidth, destinationHeight),
                    SKFilterQuality.High);
                using var scaledImage = SKImage.FromBitmap(scaledBitmap);
                using var data = scaledImage.Encode(SKEncodedImageFormat.Jpeg, 100);
                using var s = File.OpenWrite(Path.Combine(destPath, imgName + ".jpg"));
                data.SaveTo(s);
            }
        }

        public async Task ResizeImagesAsync(string sourcePath, string destPath, double scale)
        {
            if (!Directory.Exists(destPath))
            {
                Directory.CreateDirectory(destPath);
            }

            await Task.Yield();
            var LstAsync = new List<Task>();
            var allFiles = FindImages(sourcePath);
            foreach (var filePath in allFiles)
            {
                //var bitmap = SKBitmap.Decode(filePath);
                //var imgPhoto = SKImage.FromBitmap(bitmap);
                //var imgName = Path.GetFileNameWithoutExtension(filePath);

                //var sourceWidth = imgPhoto.Width;
                //var sourceHeight = imgPhoto.Height;
                //var destinationWidth = (int)(sourceWidth * scale);
                //var destinationHeight = (int)(sourceHeight * scale);

                //using var scaledBitmap = bitmap.Resize(
                //    new SKImageInfo(destinationWidth, destinationHeight),
                //    SKFilterQuality.High);
                //using var scaledImage = SKImage.FromBitmap(scaledBitmap);
                //using var data = scaledImage.Encode(SKEncodedImageFormat.Jpeg, 100);
                //using var s = File.OpenWrite(Path.Combine(destPath, imgName + ".jpg"));
                //data.SaveTo(s);
                LstAsync.Add(Task.Run(async () =>
                {
                    var 圖片名稱 = Path.GetFileNameWithoutExtension(filePath);
                    Task<SKData> 欲儲存檔案 = Task.Run(() => 轉化過程Async(filePath, scale));
                    var 儲存位置 = File.OpenWrite(Path.Combine(destPath, 圖片名稱 + ".jpg"));
                    var 檔案 = await 欲儲存檔案;
                    檔案.SaveTo(儲存位置);
                }
                )
                );
            }
            await Task.WhenAll(LstAsync);
        }

        private async Task<SKData> 轉化過程Async(string 來源位置, double 縮放大小)
        {
            //var bitmap = SKBitmap.Decode(來源位置);
            //var imgPhoto = SKImage.FromBitmap(bitmap);
            //var imgName = Path.GetFileNameWithoutExtension(來源位置);

            //var sourceWidth = imgPhoto.Width;
            //var sourceHeight = imgPhoto.Height;
            //var destinationWidth = (int)(sourceWidth * 縮放大小);
            //var destinationHeight = (int)(sourceHeight * 縮放大小);

            //using var scaledBitmap = bitmap.Resize(
            //    new SKImageInfo(destinationWidth, destinationHeight),
            //    SKFilterQuality.High);
            //using var scaledImage = SKImage.FromBitmap(scaledBitmap);
            //using var data = scaledImage.Encode(SKEncodedImageFormat.Jpeg, 100);
            //return data;

            Task<SKBitmap> 原圖解碼 = Task.Run(() => SKBitmap.Decode(來源位置));
            var 解碼圖 = await 原圖解碼;
            var 圖片 = SKImage.FromBitmap(解碼圖);
            Task<SKBitmap> 大小調整圖 = Task.Run(() => 解碼圖.Resize
            (new SKImageInfo((int)(圖片.Width * 縮放大小), (int)(圖片.Height * 縮放大小)), SKFilterQuality.High));

            Task<SKImage> 已調整大小圖 = Task.Run(async () => SKImage.FromBitmap(await 大小調整圖));
            Task<SKData> 編碼圖 = Task.Run(async () => (await 已調整大小圖).Encode(SKEncodedImageFormat.Jpeg, 100));
            return await 編碼圖;
        }

        /// <summary>
        /// 清空目的目錄下的所有檔案與目錄
        /// </summary>
        /// <param name="destPath">目錄路徑</param>
        public void Clean(string destPath)
        {
            if (!Directory.Exists(destPath))
            {
                Directory.CreateDirectory(destPath);
            }
            else
            {
                var allImageFiles = Directory.GetFiles(destPath, "*", SearchOption.AllDirectories);

                foreach (var item in allImageFiles)
                {
                    File.Delete(item);
                }
            }
        }

        /// <summary>
        /// 找出指定目錄下的圖片
        /// </summary>
        /// <param name="srcPath">圖片來源目錄路徑</param>
        /// <returns></returns>
        public List<string> FindImages(string srcPath)
        {
            //var LstTask = new List<Task>();

            List<string> files = new List<string>();
            files.AddRange(Directory.GetFiles(srcPath, "*.png", SearchOption.AllDirectories));
            files.AddRange(Directory.GetFiles(srcPath, "*.jpg", SearchOption.AllDirectories));
            files.AddRange(Directory.GetFiles(srcPath, "*.jpeg", SearchOption.AllDirectories));
            //await Task.WhenAll(LstTask);
            return files;
        }
    }
}