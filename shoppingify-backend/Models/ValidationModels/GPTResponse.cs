using System.Text.Json.Serialization;
namespace shoppingify_backend.Models.ValidationModels
{
    public class GPTResponse
    {
        public string Id { get; set; }
        public string Object { get; set; }
        public long Created { get; set; }

        public string Model { get; set; }

        public List<Choice> Choices { get; set; }

    }

    public class Choice
    {
        public int Index { get; set; }
        public Message Message { get; set; }
    }

    public class Message
    {
        public string Role { get; set; }
        public string Content { get; set; }
    }
}
