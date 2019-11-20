using System;
using System.Collections.Generic;
using System.Text;

namespace Nysa.Data.SqlClientDemo
{

    public class Order
    {
        public Int32        OrderId     { get; private set; }
        public String       Recipient   { get; private set; }
        public DateTime?    OrderDate   { get; private set; }
        public IReadOnlyList<OrderDetail> Details { get; private set; }

        public Order(Int32 orderId, String recipient, DateTime? orderDate, IReadOnlyList<OrderDetail> details)
        {
            this.OrderId    = orderId;
            this.Recipient  = recipient;
            this.OrderDate  = orderDate;
            this.Details    = details;
        }

        public Order WithDetails(IReadOnlyList<OrderDetail> details)
            => new Order(this.OrderId, this.Recipient, this.OrderDate, details);
    }

}
