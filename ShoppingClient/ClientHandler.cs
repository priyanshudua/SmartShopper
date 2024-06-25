using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingClient
{
    public class ClientHandler
    {
        private readonly string serverAddress;
        private readonly int serverPort;
        private TcpClient client;
        private NetworkStream stream;

        public ClientHandler(string serverAddress, int serverPort)
        {
            this.serverAddress = serverAddress;
            this.serverPort = serverPort;
        }

        public async Task ConnectAsync()
        {
            client = new TcpClient();
            await client.ConnectAsync(serverAddress, serverPort);
            stream = client.GetStream();
        }

        public async Task<List<string>> GetProductsAsync()
        {
            await SendMessageAsync("GET_PRODUCTS");
            var response = await ReadMessageAsync();
            List<string> products = response.Split(':')[1].Split('|').ToList();
            return products;
        }

        public async Task<bool> PurchaseProductAsync(string productName)
        {
            await SendMessageAsync("PURCHASE:" + productName);
            var response = await ReadMessageAsync();
            return response == "PURCHASE_SUCCESS";
        }

        public async Task<List<string>> GetOrdersAsync()
        {
            await SendMessageAsync("GET_ORDERS");
            var response = await ReadMessageAsync();
            List<string> orders = response.Split(':')[1].Split('|').ToList();
            return orders;
        }

        public void Disconnect()
        {
            stream.Close();
            client.Close();
        }

        public async Task<String> SendMessageAsync(string message)
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            await stream.WriteAsync(data, 0, data.Length);
            return "CONNECTED:";
        }

        private async Task<string> ReadMessageAsync()
        {
            byte[] buffer = new byte[1024];
            int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
            return Encoding.UTF8.GetString(buffer, 0, bytesRead);
        }
    }
}
