using System;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace UserAPI.Data
{

 

    public class UserDto
    { 
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [JsonIgnore(Condition =JsonIgnoreCondition.WhenWritingNull )]
        public string Email { get; set; }
        public bool MarketingConsent { get; set; }


    }
}
