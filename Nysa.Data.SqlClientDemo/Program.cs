using System;

namespace Nysa.Data.SqlClientDemo
{
    class Program
    {
        private static readonly String _ConnectString = @"Data Source=MYPC/MYINSTANCE;Initial Catalog=Northwind;Integrated Security=True;Application Name=NysaDataSqlClientDemo";

        private static void OutputResult(Order order)
        {
            if (order == null)
            {
                Console.WriteLine("No order found.");
            }
            else
            {
                Console.WriteLine($"OrderId: {order.OrderId}; Recipient: '{order.Recipient}'; Date: {order.OrderDate}");

                foreach (var detl in order.Details)
                    Console.WriteLine($"-- ProductId: {detl.ProductId}; ProductName: {detl.ProductName}; Quantity: {detl.Quantity}; Price: {detl.Price}");
            }
        }


        static void Main(string[] args)
        {
            Console.WriteLine("((Using system libraries only))");
            OutputResult(Before.GetMostRecentOrder(_ConnectString));

            Console.WriteLine();
            Console.WriteLine();

            Console.WriteLine("((Using combinator library))");
            OutputResult(After.GetMostRecentOrder(_ConnectString));

            Console.WriteLine("Done");
            Console.ReadLine();
        }
    }
}
