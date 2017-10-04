namespace MvcCoreTest.Utils
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Linq;

    using Microsoft.AspNetCore.Hosting;

    public class CaptchaGenerator : ICaptchaGenerator
    {
        private static readonly string CaptchaSymbols = "abcdefghijklmnopqrstuvwxyz0123456789";

        private readonly object locker = new object();

        private readonly IHostingEnvironment environment;

        private byte[] captchaBase;

        public CaptchaGenerator(IHostingEnvironment environment)
        {
            this.environment = environment;
        }

        public Tuple<byte[], string> Generate()
        {
            var str = GetRandomString();
            using (var stream = new MemoryStream(this.GetCaptchaBase()))
            {
                using (var image = new Bitmap(stream))
                {
                    using (var graphics = Graphics.FromImage(image))
                    {
                        var font = new Font(FontFamily.GenericSerif, 20);
                        var brush = new SolidBrush(Color.Black);
                        graphics.DrawString(str, font, brush, 10, 10);
                        graphics.Save();
                    }

                    using (var outputStream = new MemoryStream(this.GetCaptchaBase()))
                    {
                        image.Save(outputStream, ImageFormat.Jpeg);
                        return new Tuple<byte[], string>(outputStream.ToArray(), str);
                    }
                }
            }
        }

        private static string GetRandomString()
        {
            var rnd = new Random();
            var str =
                Enumerable.Range(0, 4)
                    .Select(_ => CaptchaSymbols[rnd.Next(CaptchaSymbols.Length)])
                    .Aggregate(string.Empty, (s, c) => s + c);
            return str;
        }

        private byte[] GetCaptchaBase()
        {
            if (this.captchaBase == null)
            {
                lock (this.locker)
                {
                    if (this.captchaBase == null)
                    {
                        var path = Path.Combine(this.environment.WebRootPath, "images/captchabase.jpg");
                        this.captchaBase = File.ReadAllBytes(path);
                    }
                }
            }

            return this.captchaBase.ToArray();
        }
    }
}
