﻿namespace Cutify.Models
{
    public class AppUser : BaseEntity
    {
        public string? UserImage { get; set; }
        public string? Name { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; } 
        public string? PhoneNumber { get; set; }

        public string? Address { get; set; }
    }
}
