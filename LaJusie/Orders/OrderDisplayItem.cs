using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LaJusie.Model;

namespace LaJusie.Orders
{
    internal class OrderDisplayItem
    {
        public LaJusie.Model.Orders Order { get; set; }
        public LaJusie.Model.Clients Client { get; set; }
        public LaJusie.Model.Items Item { get; set; }
        public List<JalousieDisplayItem> Jalousies { get; set; }
        public decimal TotalOrderPrice { get; set; }
    }
}
