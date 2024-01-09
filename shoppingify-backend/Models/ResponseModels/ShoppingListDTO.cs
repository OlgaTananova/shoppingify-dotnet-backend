namespace shoppingify_backend.Models.ResponseModels
{
    public class ShoppingListDTO
    {
        public required string _id { get; set; }
        public required string Heading { get; set; }
        public required string Date { get; set; }
        public required string Owner { get; set; }
        public required string Status { get; set; }
        public required decimal SalesTax { get; set; } = 0.00M;
        public required List<ShoppingListItemDTO> Items { get; set; }
        public required bool IsDeleted { get; set; }

    }
}
