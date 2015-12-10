using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TestGenerationAutomation.Models
{
    public class MenuItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ItemType Type { get; set; }
        public Size Size { get; set; }
        public Dictionary<Size, decimal> Prices { get; set; }
    }

    public enum ItemType
    {
        Drink = 1,
        ExtraShot = 2
    }

    public enum Size
    {
        Small = 1,
        Medium = 2,
        Large = 3
    }
}