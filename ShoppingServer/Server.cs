using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ServerSide
{
    class Program
    {
        static Dictionary<string, int> products = new Dictionary<string, int>();
        static Dictionary<string, string> accounts = new Dictionary<string, string>();
        static List<string> orders = new List<string>();

        static void Main(string[] args)
        {
            InitializeProducts();
            InitializeAccounts();

            TcpListener server = null;
            try
            {
                // Set the TcpListener on port 12345.
                Int32 port = 6500;

                // TcpListener server = new TcpListener(port);
                server = new TcpListener(IPAddress.Any, port);

                // Start listening for client requests.
                server.Start();

                // Enter the listening loop.
                while (true)
                {
                    Console.WriteLine("Waiting for a connection... ");

                    // Perform a blocking call to accept requests.
                    TcpClient client = server.AcceptTcpClient();
                    Console.WriteLine("Connected!");

                    // Process the connection in a new thread.
                    Thread t = new Thread(new ParameterizedThreadStart(HandleClient));
                    t.Start(client);
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            finally
            {
                // Stop listening for new clients.
                server.Stop();
            }
        }

        static void HandleClient(object obj)
        {
            TcpClient client = (TcpClient)obj;
            NetworkStream stream = client.GetStream();
            string clientMsg = "";

            try
            {
                while (true)
                {
                    byte[] bytes = new byte[1024];
                    int bytesRead = stream.Read(bytes, 0, bytes.Length);
                    clientMsg = Encoding.UTF8.GetString(bytes, 0, bytesRead);
                    Console.WriteLine("Received: {0}", clientMsg);

                    string[] command = clientMsg.Split(':');

                    string response = "";

                    switch (command[0])
                    {
                        case "CONNECT":
                            response = HandleConnect(command[1]);
                            break;
                        case "GET_PRODUCTS":
                            response = GetProducts();
                            break;
                        case "GET_ORDERS":
                            response = GetOrders();
                            break;
                        case "PURCHASE":
                            response = PurchaseProduct(command[1]);
                            break;
                        case "DISCONNECT":
                            // No response needed. Just close the connection.
                            client.Close();
                            return;
                        default:
                            response = "INVALID_COMMAND";
                            break;
                    }

                    byte[] data = Encoding.UTF8.GetBytes(response);
                    stream.Write(data, 0, data.Length);
                    Console.WriteLine("Sent: {0}", response);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: {0}", e.Message);
                client.Close();
            }
        }

        static string HandleConnect(string accountNumber)
        {
            if (accounts.ContainsKey(accountNumber))
            {
                return "CONNECTED:" + accounts[accountNumber];
            }
            else
            {
                return "CONNECT_ERROR";
            }
        }

        static string GetProducts()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("PRODUCTS:");
            foreach (var product in products)
            {
                sb.Append(product.Key + "," + product.Value + "|");
            }
            return sb.ToString().TrimEnd('|');
        }

        static string GetOrders()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("ORDERS:");
            foreach (var order in orders)
            {
                sb.Append(order + "|");
            }
            return sb.ToString().TrimEnd('|');
        }

        static string PurchaseProduct(string productName)
        {
            if (products.ContainsKey(productName) && products[productName] > 0)
            {
                products[productName]--;
                orders.Add(productName);
                return "PURCHASE_SUCCESS";
            }
            else
            {
                return "NOT_AVAILABLE";
            }
        }

        static void InitializeProducts()
        {
            products.Add("Product1", new Random().Next(1, 4));
            products.Add("Product2", new Random().Next(1, 4));
            products.Add("Product3", new Random().Next(1, 4));
            products.Add("Product4", new Random().Next(1, 4));
            products.Add("Product5", new Random().Next(1, 4));
        }

        static void InitializeAccounts()
        {
            accounts.Add("001", "User1");
            accounts.Add("002", "User2");
            accounts.Add("003", "User3");
        }
    }
}
