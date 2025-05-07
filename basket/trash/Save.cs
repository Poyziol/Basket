namespace trash;

using System.Data.Common;
using db;
public partial class Save
{
    private int Party { get; set; }
    private int Set { get; set; }
    private int Game { get; set; }
    private int Point { get; set; }
    private DbConnection Conn { get; set; }

    public Save()
    {
        Conn = new Connexion().Connect();
        Init();
    }
    private void Init()
    {
        NewParty();
        NewSet();
        NewGame();
        NewPoint();
    }
    public void Dispose()
    {
        Conn?.Dispose();
    }

    private int NewId(string query)
    {
        int result = 0;
        try
        {
            DbCommand stmt = Conn.CreateCommand();
            stmt.CommandText = query;
            result = Convert.ToInt32(stmt.ExecuteScalar());
        }
        catch (Exception e)
        {
            Console.WriteLine($"Erreur lors de l'insertion : {e.Message}");
        }
        return result;
    }
    public int Update(string query)
    {
        int result = 0;
        try
        {
            DbCommand stmt = Conn.CreateCommand();
            stmt.CommandText = query;
            result = stmt.ExecuteNonQuery();
        }
        catch (Exception e)
        {
            Console.WriteLine($"Erreur lors de l'insertion : {e.Message}");
        }
        return result;
    }

    public void WinParty(int winner)
    {
        string query = "UPDATE Party SET winner = " + winner + " , fin = now() WHERE idParty = " + Party;
        int nbRow = Update(query);
    }
    public void WinSet(int winner)
    {
        string query = "UPDATE Set SET winner = " + winner + " , fin = now() WHERE idSet = " + Set;
        int nbRow = Update(query);
    }

    public void WinGame(int winner)
    {
        string query = "UPDATE Game SET winner = " + winner + " , fin = now() WHERE idGame = " + Game;
        int nbRow = Update(query);
    }

    public void WinPoint(int winner)
    {
        string query = "UPDATE Point SET winner = " + winner + " , fin = now() WHERE idPoint = " + Point;
        int nbRow = Update(query);
    }

    public void NewParty()
    {
        string query = "INSERT INTO Party (debut) VALUES (now()) RETURNING idParty";
        Party = NewId(query);
    }
    public void NewSet()
    {
        string query = "INSERT INTO Set (debut,idParty) VALUES (now()," + Party + ") RETURNING idSet";
        Set = NewId(query);
    }
    public void NewGame()
    {
        string query = "INSERT INTO Game (debut,idSet) VALUES (now()," + Set + ") RETURNING idGame";
        Game = NewId(query);
    }
    public void NewPoint()
    {
        string query = "INSERT INTO Point (debut,idGame) VALUES (now()," + Game + ") RETURNING idPoint";
        Point = NewId(query);
    }
    // public List<DbPoint> GetPoints()
    // {
    //     List<DbPoint> result = [];
    //     string query = "SELECT * FROM Point JOIN Game ON Point.idGame = Game.idGame JOIN Set ON Game.idSet = Set.idSet JOIN Party ON Set.idParty = Party.idParty WHERE Party.idParty = " + Party;
    //     try
    //     {
    //         DbCommand stmt = Conn.CreateCommand();
    //         stmt.CommandText = query;
    //         using DbDataReader reader = stmt.ExecuteReader();
    //         while (reader.Read())
    //         {
    //             result.Add(new(reader));
    //         }
    //     }
    //     catch (Exception ex)
    //     {
    //         Console.WriteLine($"Erreur lors de la sélection : {ex.Message}");
    //     }
    //     return result;
    // }

}