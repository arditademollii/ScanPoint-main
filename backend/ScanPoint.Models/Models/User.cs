using System;
using System.Collections.Generic;

namespace ScanPoint.Models.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string Role { get; set; }

        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
        public string? ProfileImagePath { get; set; }

        // ✅ Soft Delete
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }

        // Optional FK për punonjësit (Cashier/Manager)
        public Guid? ShopId { get; set; }
        public Shop Shop { get; set; }

        // Vetëm për Admin
        public List<Shop> Shops { get; set; } = new List<Shop>();

        
       
    }
}