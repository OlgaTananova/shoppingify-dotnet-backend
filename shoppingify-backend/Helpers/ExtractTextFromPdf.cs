using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf;
using System.Text;

namespace shoppingify_backend.Helpers
{
    public static class ExtractTextFromPdf
    {
        public static string ExtractTextFromPdfMethod(MemoryStream pdfStream)
        {
            var text = new StringBuilder();
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(pdfStream));

            for (int page = 1; page <= pdfDoc.GetNumberOfPages(); ++page)
            {
                var strategy = new SimpleTextExtractionStrategy();
                var currentText = PdfTextExtractor.GetTextFromPage(pdfDoc.GetPage(page), strategy);
                text.Append(currentText);
            }

            pdfDoc.Close();
            return text.ToString();
        }
    }
}
