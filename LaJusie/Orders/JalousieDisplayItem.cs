using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LaJusie.Model;

namespace LaJusie.Orders
{
    internal class JalousieDisplayItem
    {
        public LaJusie.Model.Jalousies Jalousie { get; set; }
        public LaJusie.Model.Type Type { get; set; }
        public LaJusie.Model.Materials Material { get; set; }
        public int Price { get; set; }
        public decimal Area { get; set; }  
        public decimal TotalPrice { get; set; }
    }
}
