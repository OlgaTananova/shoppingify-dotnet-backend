namespace shoppingify_backend.Models
{
    public class ShoppingListDTO
    {
        public string _id { get; set; }
        public string Heading { get; set; }
        public string Date { get; set; }
        public string Owner { get; set; }
        public string Status { get; set; }
        public decimal SalesTax { get; set; } = 0.00M;
        public List<ShoppingListItemDTO> Items { get; set; }
       
    }
}
