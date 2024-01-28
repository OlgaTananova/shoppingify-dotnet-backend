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
using iText.Layout.Element;
using Microsoft.AspNetCore.Components.Routing;
using static iText.Svg.SvgConstants;
using System.IO;
using shoppingify_backend.Models;
using Microsoft.EntityFrameworkCore;
using shoppingify_backend.Models.Entities;
using iText.Svg.Renderers.Impl;


namespace shoppingify_backend.Services
{
    public interface IBillService
    {
        public Task<GPTShoppingListDTO> UploadBill();
        public Task<AddShoppingListDTO> UploadShoppingList(UploadShoppingListModel uploadShoppingList);
    }
    public class BillService : IBillService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string? _OpenAIKey;
        private readonly string? _GPTRequestPath;
        private readonly ApplicationContext _context;
        private readonly IUserResolverService _userResolverService;
        private readonly AuthContext _authContext;

        public BillService(IHttpContextAccessor httpContextAccessor, IConfiguration configuration, ApplicationContext context, IUserResolverService userResolverService, AuthContext authContext)
        {
            _httpContextAccessor = httpContextAccessor;
            _OpenAIKey = configuration["OpenAI:Key"];
            _GPTRequestPath = configuration["OpenAI:RequestPath"];
            _context = context;
            _userResolverService = userResolverService;
            _authContext = authContext;
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
            //if (_OpenAIKey == null)
            //{
            //    throw new BadRequestException("Cannot connect to the GPT service.");
            //}
            //// convert a bill to a text 
            //string billText = UploadPdf();

            //// make a request object 

            //string content = $"Make a list of items from the bill: {billText}. The list must contain an item with the following properties: itemName (only essential information, no brands), itemUnits without numbers(if the item is not weighted item, then replace it with pcs), itemQuantity as a decimal number, itemPricePerUnit as a decimal number, itemPrice as a decimal number. The response must be an object with the following properties: items - list of items from the bill (objects with the properties mentioned before), dateOfPurchase - the  date of purchase indicated in the bill (in the following format: Month Day, Year), salesTax - the  sales tax from the bill.The response must be in JSON format and must not contain any other information.";

            //var request = new
            //{
            //    model = "gpt-3.5-turbo",
            //    messages = new[] { new { role = "user", content } },
            //    max_tokens = 3000,
            //    temperature = 0.2f,
            //};

            ////send a request to GPT to convert it to a shopping list
            //HttpClient client = new HttpClient();
            //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _OpenAIKey);

            //var prompt = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

            //HttpResponseMessage response = await client.PostAsync(_GPTRequestPath, prompt);

            //// convert the response of GPT to ShoppingListDTO

            //string responseString = await response.Content.ReadAsStringAsync();

            //if (string.IsNullOrWhiteSpace(responseString))
            //{
            //    throw new BadRequestException("Failed to get a response from the service.");
            //}

            ////Extract the completed text from the response

            //GPTResponse? responseObject = JsonConvert.DeserializeObject<GPTResponse>(responseString);
            //if (responseObject is null || responseObject.Choices is null || responseObject.Choices.Count == 0)
            //{
            //    throw new BadRequestException("Cannot parse the response from the service.");
            //}

            //string? generatedText = responseObject?.Choices[0]?.Message.Content;

            //if (string.IsNullOrEmpty(generatedText))
            //{
            //    throw new BadRequestException("Cannot read the response from the service.");
            //}

            //GPTShoppingListDTO? shoppingList = JsonConvert.DeserializeObject<GPTShoppingListDTO>(generatedText);

            //if (shoppingList == null)
            //{
            //    throw new BadRequestException("Failed to parse the shopping list.");
            //}

            //shoppingList.Items.ForEach((item) =>
            //{
            //    item.DatabaseItemName. 
            //});

            //return shoppingList;

            GPTShoppingListDTO shoppingList = MockGPTResponseGenerator.GenerateResponse();

            shoppingList.Items.ForEach(async (item) =>
            {
                var name = await _context.Items.FirstOrDefaultAsync((i) => item.ItemName.Contains(i.ItemName));
                item.DatabaseItemName = name is not null ? name.ItemName : "Noname"; 
            });
            return shoppingList;
        }
        public async Task<AddShoppingListDTO> UploadShoppingList(UploadShoppingListModel uploadShoppingList)
        {
            // Check if there is no active SL 

            var activeShoppingList = await _context.ShoppingLists.FirstOrDefaultAsync(sl => sl.Status == ShoppingListStatus.Active);
            if (activeShoppingList is not null)
            {
                throw new BadRequestException("The active shopping list already exists.");

            }
            // Parse the date of the list
            DateTime dateOfPurchase;

            if(!DateTime.TryParse(uploadShoppingList.Date, out dateOfPurchase))
            {
                dateOfPurchase = DateTime.Now;
            }

            // Check if there are items in the uploaded shopping list.
            if (uploadShoppingList.Items.Count == 0)
            {
                throw new BadRequestException("There are no items in your shopping list.");
            }

            // Create a new shopping list

            string userIdStr = _userResolverService.GetCurrentUserId();
            Guid userIdGuid;
            if(!Guid.TryParse(userIdStr, out userIdGuid))
            {
                throw new BadRequestException("Failed to parser the user's id.");
            }


            ShoppingList shoppingList = new ShoppingList
            {
                OwnerId = userIdGuid,
                Date = dateOfPurchase,
                SalesTax = uploadShoppingList.SalesTax,
                Status = ShoppingListStatus.Active,
                ShoppingListItems = new List<ShoppingListItem>()
            };


            // Create new shopping list items within the new shopping list

            foreach(var si in uploadShoppingList.Items)
            {   
                Guid categoryIdGuid;
                Guid itemIdGuid;
                // Parse itemId and categoryId
                if(!Guid.TryParse(si.CategoryId, out categoryIdGuid) || !Guid.TryParse(si.ItemId, out itemIdGuid))
                {
                    throw new BadRequestException("Failed to parse the item's category id and/or item id.");
                }

                // Find the category and item

                Item? item = await _context.Items.FindAsync(itemIdGuid);
                Category? category = await _context.Categories.FindAsync(categoryIdGuid);

                if (item is null || category is null)
                {
                    throw new NotFoundException("Cannot find the category and/or item while creating a new shopping list item.");
                }

                ShoppingListItem shoppingListItem = new ShoppingListItem
                {
                    ItemId = itemIdGuid,
                    CategoryId = categoryIdGuid,
                    ShoppingListId = shoppingList.Id,
                    OwnerId = userIdGuid,
                    ShoppingList = shoppingList,
                    Item = item,
                    Category = category,
                    Status = ItemStatus.Completed,
                    Quantity = si.Quantity,
                    PricePerUnit = si.PricePerUnit,
                    Price = si.Quantity * si.PricePerUnit,
                    Units = si.Units,
                };
                shoppingList.ShoppingListItems.Add(shoppingListItem);
            }

            _context.ShoppingLists.Update(shoppingList);
            var result = await _context.SaveChangesAsync();

            if (result <= 0)
            {
                throw new BadRequestException("Failed to create a new shopping list or/and add shopping items to it.");
            }

            // return a new SL
            return new AddShoppingListDTO
            {
                Message = "A new shopping list was successfully created.",
                AddedShoppingList = MappingHandler.MapToShoppingListDTO(shoppingList),
            };
        }
    }
}
