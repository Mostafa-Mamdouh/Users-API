using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace UserAPI.Data
{
    public class RequesDto
    { 
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public bool MarketingConsent { get; set; }
    }
}
