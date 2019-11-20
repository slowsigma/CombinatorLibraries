using System;
using System.Collections.Generic;
using System.Text;

namespace Nysa.Data.SqlClientDemo
{

    public class OrderDetail
    {
        public Int32    ProductId   { get; private set; }
        public String   ProductName { get; private set; }
        public Int16    Quantity    { get; private set; }
        public Decimal  Price       { get; private set; }
        public OrderDetail(Int32? productId, String productName, Int16? quantity, Decimal? price)
        {
            this.ProductId      = productId.GetValueOrDefault(-1);
            this.ProductName    = productName;
            this.Quantity       = quantity.GetValueOrDefault(0);
            this.Price          = price.GetValueOrDefault(0);
        }
    }

}
