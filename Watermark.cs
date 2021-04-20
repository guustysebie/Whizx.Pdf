using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Whizx.Pdf {
    public static class Watermark {

        public static byte[] AddWaterMarkImage(byte[] pdf, byte[] image,float imageWidth, float imageHeight) {
            var pdfReader = new PdfReader(pdf);
            using var ms = new MemoryStream();
            var stamp = new PdfStamper(pdfReader, ms);
            var img = Image.GetInstance(image);
            img.Alignment = Image.UNDERLYING;
            img.ScaleAbsolute(imageWidth,imageHeight);
            img.SetAbsolutePosition(0, 0); 
            for (var page = 1; page <= pdfReader.NumberOfPages; page++) {
                var waterMark = stamp.GetUnderContent(page);
                waterMark.AddImage(img);
            }
            stamp.FormFlattening = true;
            stamp.Close();
            return ms.ToArray();
        }
        
        private static byte[] AddWatermark(byte[] pdf, BaseFont bf, string text) {
            using var ms = new MemoryStream(10 * 1024);
            using(var reader = new PdfReader(pdf)) {
                var times = reader.NumberOfPages;
                using(var stamper = new PdfStamper(reader, ms))
                {
                    for (var i = 1; i <= times; i++)
                    {
                        var dc = stamper.GetOverContent(i);
                        AddWaterMark(dc, text, bf, 48, 35, new BaseColor(70, 70, 255), reader.GetPageSizeWithRotation(i));
                    }
                    stamper.Close();
                }
            }
            return ms.ToArray();
        }
        
        private static void AddWaterMark(PdfContentByte dc, string text, BaseFont font, float fontSize, float angle, BaseColor color, Rectangle realPageSize, Rectangle rect = null)
        {
            var gstate = new PdfGState { FillOpacity = 0.1f, StrokeOpacity = 0.3f };
            dc.SaveState();
            dc.SetGState(gstate);
            dc.SetColorFill(color);
            dc.BeginText();
            dc.SetFontAndSize(font, fontSize);
            var ps = rect ?? realPageSize;
            var x = (ps.Right + ps.Left) / 2;
            var y = (ps.Bottom + ps.Top) / 2;
            
            dc.ShowTextAligned(Element.ALIGN_CENTER, text, x, y, angle);
            dc.EndText();
            dc.RestoreState();
        }
    }
}