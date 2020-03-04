using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Emarketing.Models
{
    public class ProductDetails
    {
        public int product_id { get; set; }
        public string product_name { get; set; }
        public string product_image { get; set; }
        public Nullable<int> product_price { get; set; }
        public Nullable<int> product_fk_user { get; set; }
        public string product_des { get; set; }
        public Nullable<int> product_fk_category { get; set; }
        public int category_id { get; set; }
        public string category_name { get; set; }
        public string Tuser_name { get; set; }
        public string Tuser_image { get; set; }
        public string Tuser_contact { get; set; }

    }
}