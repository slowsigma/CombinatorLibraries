using System;
using System.Collections.Generic;
using System.Text;

using Nysa.Data.SqlClient;

namespace Nysa.Data.SqlClientDemo
{

    public static class After
    {

        public static Order GetMostRecentOrder(String connectString)
        {
            var query =
@"DECLARE @OrderID INT; SET @OrderID = ISNULL(( SELECT TOP 1 OrderID FROM [dbo].[Orders] ORDER BY OrderID DESC ), 0);

SELECT ODR.OrderID, ISNULL(CST.CompanyName, ODR.ShipName), ODR.OrderDate FROM [dbo].[Orders] AS ODR WITH (NOLOCK) LEFT JOIN [dbo].[Customers] AS CST ON (ODR.CustomerID = CST.CustomerID) WHERE CaseID = @CaseID;

SELECT ORD.ProductID, PRD.ProductName, ORD.Quantity, ORD.UnitPrice FROM [dbo].[Order Detail] AS ORD INNER JOIN [dbo].[Products] AS PRD ON (ORD.ProductID = PRD.ProductID) WHERE ORD.OrderID = @OrderID ORDER BY ORD.ProductID;
";

            return With.Row(Of.Int32, Of.String, Of.DateTime)
                       .Make((i, r, d) => new Order(i.GetValueOrDefault(-1), r, d, null))
                       .ReadObject()
                       .Then(With.Row(Of.Int32, Of.String, Of.Int16, Of.Decimal)
                                 .Make((i, s, q, p) => new OrderDetail(i, s, q, p))
                                 .ReadObjects(),
                             (c, h) => c.WithDetails(h))
                       .ForQuery(query)
                       .ExecuteOn(connectString)();
        }

    }
}
