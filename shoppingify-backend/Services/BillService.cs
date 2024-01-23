using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Text;
using shoppingify_backend.Helpers.CustomExceptions;
using shoppingify_backend.Models.ResponseModels;
using System.Net.Http.Headers;
using shoppingify_backend.Models.ValidationModels;
using Microsoft.VisualBasic;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Text.Json;
using Newtonsoft.Json;
using System;
using System.Drawing;
using PdfiumViewer;
using System.Drawing.Imaging;
using shoppingify_backend.Helpers;


namespace shoppingify_backend.Services
{
    public interface IBillService
    {
        public Task<GPTShoppingListDTO> UploadBill();
    }
    public class BillService : IBillService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string? _OpenAIKey;

        public BillService(IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _httpContextAccessor = httpContextAccessor;
            _OpenAIKey = configuration["OpenAIKey"];

        }

        private string UploadPdf()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
            {
                throw new BadRequestException("Cannot read the uploaded file.");
            }
            var file = httpContext.Request.Form.Files[0];

            if (file.Length <= 0)
            {
                throw new BadRequestException("The uploaded file is empty.");
            }

            using (var memoryStream = new MemoryStream())
            {
                file.CopyTo(memoryStream);
                memoryStream.Position = 0;

                var extractedText = ExtractTextFromPdf.ExtractTextFromPdfMethod(memoryStream);

                // Check if the extracted text is satisfactory
                if (!string.IsNullOrWhiteSpace(extractedText))
                {
                    return extractedText;
                }
                else
                {
                    // Fall back to OCR
                    memoryStream.Position = 0;

                    var ocrText = ExtractTextFromPdfUsingOCR.ExtractTextFromPdfUsingOCRMethod(memoryStream);
                    if (string.IsNullOrWhiteSpace(ocrText))
                    {
                        throw new BadRequestException("Failed to parse the pdf file.");
                    }
                    return ocrText;
                }
            }
        }
        public async Task<GPTShoppingListDTO> UploadBill()
        {
            if (_OpenAIKey == null)
            {
                throw new BadRequestException("Cannot connect to the GPT service.");
            }
            // convert a bill to a text 
            string billText = UploadPdf();

            // make a request object 

            string content = $"Make a list of items from the bill: {billText}. The list must contain an item with the following properties: itemName (only essential information, no brands), itemUnits without numbers(if the item is not weighted item, then replace it with pcs),itemQuantity, itemPricePerUnit, itemPrice. The response must be an object with the following properties: items - list of items from the bill (objects with the properties mentioned before), dateOfPurchase - the  date of purchase indicated in the bill (in the following format: Month Day, Year), salesTax - the  sales tax from the bill.The response must be in JSON format and must not contain any other information.";
            var request = new
            {
                model = "gpt-3.5-turbo",
                messages = new[] { new { role = "user", content } },
                max_tokens = 3000,
                temperature = 0.2f,
            };

            //send a request to GPT to convert it to a shopping list
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _OpenAIKey);

            var prompt = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync("https://api.openai.com/v1/chat/completions", prompt);

            // convert the response of GPT to ShoppingListDTO

            string responseString = await response.Content.ReadAsStringAsync();

            if (string.IsNullOrWhiteSpace(responseString))
            {
                throw new BadRequestException("Failed to get a response from the service.");
            }

            //Extract the completed text from the response

            GPTResponse? responseObject = JsonConvert.DeserializeObject<GPTResponse>(responseString);
            if (responseObject is null || responseObject.Choices is null || responseObject.Choices.Count == 0)
            {
                throw new BadRequestException("Cannot parse the response from the service.");
            }

            string? generatedText = responseObject?.Choices[0]?.Message.Content;

            if (string.IsNullOrEmpty(generatedText))
            {
                throw new BadRequestException("Cannot read the response from the service.");
            }


            GPTShoppingListDTO? shoppingList = JsonConvert.DeserializeObject<GPTShoppingListDTO>(generatedText);

            if (shoppingList == null)
            {
                throw new BadRequestException("Failed to parse the shopping list.");
            }

            return shoppingList;

        }
    }
}
