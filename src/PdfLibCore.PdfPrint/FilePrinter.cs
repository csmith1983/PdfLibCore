using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Drawing.Imaging;
using System.IO;
using PdfLibCore;
using PdfLibCore.Enums;
using System.Runtime.Versioning;
using System.Threading.Tasks;

namespace PdfLibCore.PdfPrint
{
    [SupportedOSPlatform("windows")]
    public static class FilePrinter
    {
        public static Task PrintPdf(string filePath)
        {
            using var document = new PdfDocument(filePath);
            using var printDoc = new PrintDocument();
            var dpi = 300;
            var firstPage = document.Pages[0];
            printDoc.PrinterSettings = new PrinterSettings();
            printDoc.PrintController = new StandardPrintController();
            printDoc.DefaultPageSettings.Landscape = firstPage.Width > firstPage.Height;
            int pageIndex = 0;
            printDoc.PrintPage += (sender, e) =>
            {
                var pdfPage = document.Pages[pageIndex];
                double scaleFactor = dpi / 72.0;
                int scaledWidth = (int)(pdfPage.Width * scaleFactor);
                int scaledHeight = (int)(pdfPage.Height * scaleFactor);
                using (var pdfBitmap = new PdfiumBitmap(scaledWidth, scaledHeight, false))
                {
                    pdfPage.Render(pdfBitmap, (PageOrientations)0);
                    int width = pdfBitmap.Width;
                    int height = pdfBitmap.Height;
                    int stride = pdfBitmap.Stride;
                    IntPtr bufferPtr = pdfBitmap.Scan0;
                    using Bitmap bmp = new(width, height, stride, PixelFormat.Format32bppArgb, bufferPtr);
                    using var ms = new MemoryStream();
                    bmp.Save(ms, ImageFormat.Png);
                    ms.Position = 0;
                    using var pageImage = Image.FromStream(ms);
                    e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    e.Graphics.DrawImage(pageImage, e.PageBounds);
                }
                pageIndex++;
                e.HasMorePages = (pageIndex < document.Pages.Count);
            };
            printDoc.Print();
            return Task.CompletedTask;
        }
    }
}
