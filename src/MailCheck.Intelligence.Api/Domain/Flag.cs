using System.Collections.Generic;

namespace MailCheck.Intelligence.Api.Domain
{
    public partial class Flag
    {
        public Flag(string name, string description)
        {
            Name = name;
            Description = description;
        }

        public string Name { get; set; }
        public string Description { get; set; }
    }
}