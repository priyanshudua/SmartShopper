using System;
using System.Windows.Forms;

namespace ShoppingClient
{
    public partial class LoginForm : Form
    {
        private readonly ClientHandler clientHandler;

        private TextBox txtServerAddress;
        private TextBox txtAccountNumber;
        private Button btnConnect;
        private Button btnCancel;

        public LoginForm(ClientHandler clientHandler)
        {
            InitializeComponent();
            this.clientHandler = clientHandler;
            txtServerAddress.Text = "localhost"; // Default server address
        }

        private void InitializeComponent()
        {
            this.txtServerAddress = new System.Windows.Forms.TextBox();
            this.txtAccountNumber = new System.Windows.Forms.TextBox();
            this.btnConnect = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();

            this.SuspendLayout();

            // txtServerAddress
            this.txtServerAddress.Location = new System.Drawing.Point(10, 10);
            this.txtServerAddress.Size = new System.Drawing.Size(150, 20);
            this.Controls.Add(this.txtServerAddress);

            // txtAccountNumber
            this.txtAccountNumber.Location = new System.Drawing.Point(10, 40);
            this.txtAccountNumber.Size = new System.Drawing.Size(150, 20);
            this.Controls.Add(this.txtAccountNumber);

            // btnConnect
            this.btnConnect.Location = new System.Drawing.Point(10, 70);
            this.btnConnect.Size = new System.Drawing.Size(75, 23);
            this.btnConnect.Text = "Connect";
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            this.Controls.Add(this.btnConnect);

            // btnCancel
            this.btnCancel.Location = new System.Drawing.Point(85, 70);
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            this.Controls.Add(this.btnCancel);

            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private async void btnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                await clientHandler.ConnectAsync();
                var response = await clientHandler.SendMessageAsync($"CONNECT:{txtAccountNumber.Text}");

                if (!string.IsNullOrEmpty(response))
                {
                    string[] parts = response.Split(':');
                    if (parts.Length >= 2 && parts[0] == "CONNECTED")
                    {
                        MessageBox.Show($"Connected as {parts[1]}");
                        MainForm mainForm = new MainForm(clientHandler);
                        mainForm.Show();
                        this.Hide();
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("Login failed.");
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Connection error: " + ex.Message);
            }
        }



        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
