//using Tesseract;
//using PdfiumViewer;
using System.IO;
using System.Drawing;
using System.Text;

namespace shoppingify_backend.Helpers
{
    //public static class ExtractTextFromPdfUsingOCR
    //{
    //    public static string ExtractTextFromPdfUsingOCRMethod(MemoryStream pdfStream)
    //    {
    //        var text = new StringBuilder();
    //        using (var document = PdfDocument.Load(pdfStream))
    //        {
    //            using (var engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default))
    //            {
    //                // Extract each page from the document and convert it to an image
    //                for (int pageIndex = 0; pageIndex < document.PageCount; pageIndex++)
    //                {
    //                    using (var image = document.Render(pageIndex, 300, 300, PdfRenderFlags.Annotations))
    //                    {
    //                        // Convert image to byte array
    //                        byte[] imageBytes;
    //                        using (var ms = new MemoryStream())
    //                        {
    //                            image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
    //                            imageBytes = ms.ToArray();
    //                        }

    //                        // Load the image into Pix using Pix.LoadFromMemory
    //                        using (var pix = Pix.LoadFromMemory(imageBytes))
    //                        {
    //                            using (var page = engine.Process(pix))
    //                            {
    //                                text.AppendLine(page.GetText());
    //                            }
    //                        }
    //                    }
    //                }
    //            }
    //        }
    //        return text.ToString();
    //    }
    //}
}
