namespace db;

using System.Data.Common;
using System.Data.Odbc;

public class Connexion
{
    private readonly string DSN = "HFSQL_DSN";
    private readonly string Username = "Admin";
    private readonly string Password = "mdpprom15";

    public DbConnection Connect()
    {
        string connectionString = "DSN=" + DSN + ";UID=" + Username + ";PWD=" + Password + ";";

        DbConnection? conn = new OdbcConnection(connectionString);
        try
        {
            conn.Open();
            Console.WriteLine("Connected to HFSQL");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur : {ex.Message}");
        }

        return conn;
    }

}
