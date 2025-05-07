namespace tennis;
using System.Data.Common;
using aff;
using components;
using db;

static class Program
{
    static void Main()
    {
        DbConnection conn = new Connexion().Connect();

        ApplicationConfiguration.Initialize();
        Application.EnableVisualStyles();
        Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
        Application.Run(new Basket(conn));

        conn.Close();
        Console.WriteLine("Disconnected...");
    }
}