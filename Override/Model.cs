using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Override
{
    public class Portfolio
    {
        [Key]
        public Int64 Id { get; set; }
        public string source { get; set; }
        public string portfolio_code { get; set; }
        public string modified_user { get; set; }
        public string data { get; set; }  // Store raw JSON
                                          //public DateTime InsertDateTime { get; set; } = DateTime.Now;

        public DateTime updated_datetime { get; set; }
    }


    public class PortfolioRequest
    {
        [Required(ErrorMessage = "Source is required")]
        public string Source { get; set; }

        [Required(ErrorMessage = "Portfolio code is required")]
        [JsonPropertyName("portfolio_code")]  // Ensure it matches "portfolio_code" in JSON
        public string PortfolioCode { get; set; }  // Now a string

        [Required(ErrorMessage = "User is required")]
        public string User { get; set; }

        public JsonElement Data { get; set; }  // Handle the raw JSON
    }
}
