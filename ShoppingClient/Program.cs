
using System;
using System.Windows.Forms;

namespace ShoppingClient
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Create a ClientHandler instance
            ClientHandler clientHandler = new ClientHandler("localhost", 6500);

            // Pass the ClientHandler instance to the LoginForm constructor
            LoginForm loginForm = new LoginForm(clientHandler);
            if (loginForm.ShowDialog() != DialogResult.OK)
            {
                // Pass the ClientHandler instance to the MainForm constructor
                Application.Run(new MainForm(clientHandler));
            }
            else
            {
                MessageBox.Show("Login failed. Exiting application.");
                Environment.Exit(1);
            }
        }
    }
}
