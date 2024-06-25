using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShoppingClient
{
    public partial class MainForm : Form
    {
        private ComboBox cmbProducts;
        private Button btnPurchase;
        private Button btnShowOrders;
        private ListBox listBoxOrders;

        private readonly ClientHandler clientHandler;

        public MainForm(ClientHandler handler)
        {
            InitializeComponent();
            clientHandler = handler;
            Load += MainForm_Load;
            FormClosing += MainForm_FormClosing;
        }

        private async void MainForm_Load(object sender, EventArgs e)
        {
            await clientHandler.ConnectAsync();

            var products = await clientHandler.GetProductsAsync();
            foreach (var product in products)
            {
                cmbProducts.Items.Add(product.Split(',')[0]);
            }
            cmbProducts.SelectedIndex = 0; // Select the first item by default
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            clientHandler.Disconnect();
        }


        private async void btnPurchase_Click(object sender, EventArgs e)
        {
            string selectedProduct = cmbProducts.SelectedItem?.ToString();
            if (selectedProduct != null)
            {
                await SendMessageAsync($"PURCHASE:{selectedProduct}");
                var response = await ReadMessageAsync();
                if (response == "PURCHASE_SUCCESS")
                {
                    MessageBox.Show("Purchase successful!");
                }
                else if (response == "NOT_AVAILABLE")
                {
                    MessageBox.Show("Product not available.");
                }
            }
        }

        private async void btnShowOrders_Click(object sender, EventArgs e)
        {
            await SendMessageAsync("GET_ORDERS");
            var response = await ReadMessageAsync();
            if (response.StartsWith("ORDERS"))
            {
                var orders = response.Split(':')[1].Split('|');
                foreach (var order in orders)
                {
                    listBoxOrders.Items.Add(order);
                }
            }
        }

        private async Task SendMessageAsync(string message)
        {
            using (TcpClient client = new TcpClient("localhost", 6500)) // Assuming server port is 12345
            using (NetworkStream stream = client.GetStream())
            {
                byte[] data = Encoding.UTF8.GetBytes(message);
                await stream.WriteAsync(data, 0, data.Length);
            }
        }

        private async Task<string> ReadMessageAsync()
        {
            using (TcpClient client = new TcpClient("localhost", 6500)) // Assuming server port is 12345
            using (NetworkStream stream = client.GetStream())
            {
                byte[] buffer = new byte[1024];
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                return Encoding.UTF8.GetString(buffer, 0, bytesRead);
            }
        }

        private void InitializeComponent()
        {
            this.cmbProducts = new System.Windows.Forms.ComboBox();
            this.btnPurchase = new System.Windows.Forms.Button();
            this.btnShowOrders = new System.Windows.Forms.Button();
            this.listBoxOrders = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // cmbProducts
            // 
            this.cmbProducts.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbProducts.FormattingEnabled = true;
            this.cmbProducts.Location = new System.Drawing.Point(12, 12);
            this.cmbProducts.Name = "cmbProducts";
            this.cmbProducts.Size = new System.Drawing.Size(200, 21);
            this.cmbProducts.TabIndex = 0;
            // 
            // btnPurchase
            // 
            this.btnPurchase.Location = new System.Drawing.Point(12, 39);
            this.btnPurchase.Name = "btnPurchase";
            this.btnPurchase.Size = new System.Drawing.Size(200, 23);
            this.btnPurchase.TabIndex = 1;
            this.btnPurchase.Text = "Purchase";
            this.btnPurchase.UseVisualStyleBackColor = true;
            this.btnPurchase.Click += new System.EventHandler(this.btnPurchase_Click);
            // 
            // btnShowOrders
            // 
            this.btnShowOrders.Location = new System.Drawing.Point(12, 68);
            this.btnShowOrders.Name = "btnShowOrders";
            this.btnShowOrders.Size = new System.Drawing.Size(200, 23);
            this.btnShowOrders.TabIndex = 2;
            this.btnShowOrders.Text = "Show Orders";
            this.btnShowOrders.UseVisualStyleBackColor = true;
            this.btnShowOrders.Click += new System.EventHandler(this.btnShowOrders_Click);
            // 
            // listBoxOrders
            // 
            this.listBoxOrders.FormattingEnabled = true;
            this.listBoxOrders.Location = new System.Drawing.Point(12, 97);
            this.listBoxOrders.Name = "listBoxOrders";
            this.listBoxOrders.Size = new System.Drawing.Size(200, 173);
            this.listBoxOrders.TabIndex = 3;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(224, 282);
            this.Controls.Add(this.listBoxOrders);
            this.Controls.Add(this.btnShowOrders);
            this.Controls.Add(this.btnPurchase);
            this.Controls.Add(this.cmbProducts);
            this.Name = "MainForm";
            this.Text = "Shopping App";
            this.ResumeLayout(false);

        }
    }
}
