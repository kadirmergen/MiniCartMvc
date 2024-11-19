﻿using System.ComponentModel;

namespace MiniCartMvc.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = ""; 
        
        [DisplayName("Açıklama")]
        public string Description { get; set; } = "";
        public double Price { get; set; }
        public int Stock { get; set; }
        public string? Image { get; set; } = "";
        public bool IsApproved { get; set; }

        public int CategoryId { get; set; }
        public Category? Category { get; set; }
    }
}