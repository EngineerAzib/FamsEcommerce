using FamsEcommerce.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FamsEcommerce
{
    public class ProductCartViewModel
    {
        public IEnumerable<Product> Products { get; set; }
        public IEnumerable<CartTable> CartItems { get; set; }
    }
}