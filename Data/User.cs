using System.ComponentModel.DataAnnotations;

namespace UserAPI.Data
{
    public class User
    {
        [Key]
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public bool MarketingConsent { get; set; }
    }
}
