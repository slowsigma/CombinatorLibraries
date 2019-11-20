using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace Nysa.Data.SqlClientDemo
{

    public static class Before
    {

        public static Order GetMostRecentOrder(String connectString)
        {
            var query =
@"DECLARE @OrderID INT; SET @OrderID = ISNULL(( SELECT TOP 1 OrderID FROM [dbo].[Orders] ORDER BY OrderID DESC ), 0);

SELECT ODR.OrderID, ISNULL(CST.CompanyName, ODR.ShipName), ODR.OrderDate FROM [dbo].[Orders] AS ODR WITH (NOLOCK) LEFT JOIN [dbo].[Customers] AS CST ON (ODR.CustomerID = CST.CustomerID) WHERE CaseID = @CaseID;

SELECT ORD.ProductID, PRD.ProductName, ORD.Quantity, ORD.UnitPrice FROM [dbo].[Order Detail] AS ORD INNER JOIN [dbo].[Products] AS PRD ON (ORD.ProductID = PRD.ProductID) WHERE ORD.OrderID = @OrderID ORDER BY ORD.ProductID;
";
                                                                                                                            // what?                     category
            var lastOrder = (Order)null;

            using (var connection = new SqlConnection(connectString))                                                       // using + SqlConnection     best practice + recall
            {
                connection.Open();                                                                                          // open-connection           mandatory ceremony

                using (var command = connection.CreateCommand())                                                            // using + create-command    best practice + recall
                {
                    command.CommandText = query;                                                                            // command-text-property     mandatory ceremony
                    command.CommandType = CommandType.Text;                                                                 // command-type-property     mandatory ceremony

                    using (var reader = command.ExecuteReader(CommandBehavior.Default))                                     // using + reader            best practice + recall
                    {
                        if (reader.Read())                                                                                  // read                      mandatory ceremony
                        {
                            lastOrder = new Order(reader.IsDBNull(0) ? -1              : reader.GetInt32(0),                // null-checking + get       mandatory ceremony + recall
                                                  reader.IsDBNull(1) ? (String)null    : reader.GetString(1),
                                                  reader.IsDBNull(2) ? (DateTime?)null : reader.GetDateTime(2),
                                                  null);

                            var detls = new List<OrderDetail>();

                            if (reader.NextResult())                                                                        // nextresult-method         mandatory ceremony
                            {
                                while (reader.Read())
                                {
                                    var detl = new OrderDetail(reader.IsDBNull(0) ? -1 : reader.GetInt32(0),
                                                               reader.IsDBNull(1) ? (String)null : reader.GetString(1),
                                                               reader.IsDBNull(2) ? (Int16?)null : reader.GetInt16(2),
                                                               reader.IsDBNull(3) ? (Decimal?)null : reader.GetDecimal(3));

                                    detls.Add(detl);
                                }
                            }

                            lastOrder = lastOrder.WithDetails(detls);
                        }
                    }
                }

                if (connection.State == ConnectionState.Open)                                                               // state-check + close       best practice + recall
                    connection.Close();
            }

            return lastOrder;
        }

    }

}
