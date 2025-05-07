namespace db;
using System.Data.Common;
using System.Text;
using components;

public class Equipe : Table
{
    [Column]
    [AutoIncrement]
    public long IdEquipe { get; set; }
    [Column]
    public string? Nom { get; set; }
    public int Point { get; set; }
    public int NbPasseCourant { get; set; }
    public int NbTirCourant { get; set; }
    public List<Joueur> Joueurs { get; set; }
    public Terrain Terrain { get; set; }
    public bool Haut { get; set; }
    public List<Point> Formation1 { get; set; } =
        [
            new(0,0),
            new(200,0),
            new(0,200),
            new(200,200),
            new(100,100),
        ];

    private Random Rand { get; set; } = new();

    public Equipe()
    {
    }
    public Equipe(DbConnection conn)
    {
        SetConn(conn);
    }
    public Equipe(DbDataReader reader) : base(reader)
    {
    }

    public void Init()
    {
        Joueurs = GetJoueurs();
    }
    public void SetHaut(bool haut)
    {
        Haut = haut;
        foreach (Joueur joueur in Joueurs)
        {
            if (haut) joueur.BalleHaut = false;
            else joueur.BalleHaut = true;
        }
        Formation();
    }
    public new void SetColor(Color color)
    {
        base.SetColor(color);
        foreach (Joueur joueur in Joueurs) joueur.SetColor(color);
    }
    public void Formation()
    {
        if (Terrain == null) throw new Exception("besoin d'un terrain");

        int cX = (int)(Terrain.Width / 2 - (Formation1[1].X / 2) - Joueur.JoueurWidth / 2);
        int cY = (int)(Terrain.Height / 2 - (Formation1[2].Y + Joueur.JoueurHeight + ((Terrain.Height / 2 - Formation1[2].Y) / 2)));

        List<Point> result = [.. Formation1];
        if (Haut)
        {
            for (int i = 0; i < result.Count; i++)
            {
                result[i] = new((int)(result[i].X + Terrain.X) + cX, (int)(result[i].Y + Terrain.Y) + cY);
            }
        }
        else
        {
            for (int i = 0; i < result.Count; i++)
            {
                result[i] = new((int)(result[i].X + Terrain.X) + cX, (int)(result[i].Y + Terrain.Y + Terrain.Height / 2) + cY);
            }
        }
        for (int i = 0; i < result.Count; i++)
        {
            Joueurs[i].X = result[i].X;
            Joueurs[i].Y = result[i].Y;
        }
    }

    public void PrendreBalle(Balle balle)
    {
        Joueurs[Random(0, Joueurs.Count)].PrendreBalle(balle);
    }
    public void PerdreBalle()
    {
        foreach (Joueur joueur in Joueurs)
        {
            joueur.Balle = null;
        }
    }
    public Joueur? GetPossesseur()
    {
        foreach (Joueur joueur in Joueurs)
        {
            if (joueur.Balle != null)
            {
                return joueur;
            }
        }
        return null;
    }
    public bool PossedeBalle()
    {
        if (GetPossesseur() == null) return false;
        return true;
    }
    public void PasseInterne()
    {
        int cible = Random(0, Joueurs.Count);
        Joueur possesseur = GetPossesseur() ?? throw new Exception("l'equipe: " + Nom + ", n'a pas de ballon");
        if (Joueurs[cible].Equals(possesseur))
        {
            cible++;
            if (cible >= Joueurs.Count) cible = 0;
        }
        possesseur.Passe(Joueurs[cible]);
    }

    public int Random(int min, int max)
    {
        float rand = Rand.NextSingle();
        int result = (int)(rand * (max - min));
        return result + min;
    }

    // public Joueur Avant(Joueur exclu)
    // {
    //     Joueur result = Joueurs[0] == exclu ? Joueurs[1] : Joueurs[0];
    //     foreach (Joueur jo in Joueurs)
    //     {
    //         if (Haut)
    //         {
    //             if (jo != exclu && result.Y < jo.Y)
    //             {
    //                 result = jo;
    //             }
    //         }
    //         else
    //         {
    //             if (jo != exclu && result.Y > jo.Y)
    //             {
    //                 result = jo;
    //             }
    //         }
    //     }
    //     return result;
    // }

    public Joueur Avant(Joueur exclu)
    {
        Joueur result = Joueurs[0] == exclu ? Joueurs[1] : Joueurs[0];
        foreach (Joueur jo in Joueurs)
        {
            if (jo != exclu && DistanceBut(result) > DistanceBut(jo))
            {
                result = jo;
            }

        }
        return result;
    }

    public double DistanceBut(Joueur j)
    {
        double Xbut = Terrain.X + (Terrain.Width / 2);
        double Ybut = Haut ? (Terrain.Y + Terrain.Height) : Terrain.Y;
        PointF jCenter = j.Center();
        double vertical = Ybut - jCenter.Y;
        double horizontal = Xbut - jCenter.X;

        return Math.Sqrt(Math.Pow(vertical, 2) + Math.Pow(horizontal, 2));
    }

    public List<Joueur> GetJoueurs()
    {
        List<Joueur> result = [];
        string query = "SELECT * FROM Joueur WHERE IdEquipe = " + IdEquipe;
        try
        {
            DbCommand stmt = GetConn().CreateCommand();
            stmt.CommandText = query;
            using DbDataReader reader = stmt.ExecuteReader();
            while (reader.Read())
            {
                Joueur joueur = new(reader);
                joueur.SetConn(GetConn());
                result.Add(joueur);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur lors de la sélection : {ex.Message}");
        }
        return result;
    }

    public int TotalPoint()
    {
        int result = 0;
        string query = "SELECT sum(Valeur) total FROM Tir WHERE IdEquipe = " + IdEquipe;

        DbCommand stmt = GetConn().CreateCommand();
        stmt.CommandText = query;
        try
        {
            result = Convert.ToInt32(stmt.ExecuteScalar());
        }
        catch (Exception)
        {
        }
        return result;
    }
    public int NbTir(int? point = null)
    {
        int result = 0;
        string query = "SELECT count(idTir) FROM Tir WHERE IdEquipe = " + IdEquipe;
        if (point.HasValue)
        {
            query += " AND Valeur = " + point.Value;
        }

        DbCommand stmt = GetConn().CreateCommand();
        stmt.CommandText = query;
        try
        {
            result = Convert.ToInt32(stmt.ExecuteScalar());
        }
        catch (Exception)
        {
        }
        return result;
    }
    public int NbPasse()
    {
        int result = 0;

        foreach (Joueur j in Joueurs)
        {
            result += j.NbPasse();
        }

        return result;
    }

    public double PourcentageTir(int point)
    {
        int totalTir = NbTir();
        int nbPoint = NbTir(point);

        if (totalTir == 0) return 0;

        return (double)(nbPoint * 100) / totalTir;
    }

    public double PourcentageTir(bool ok = true)
    {
        string query = "SELECT count(idTir) FROM Tir WHERE Valeur > 0 AND IdEquipe = " + IdEquipe;
        DbCommand stmt = GetConn().CreateCommand();
        stmt.CommandText = query;
        int nbPoint = Convert.ToInt32(stmt.ExecuteScalar());
        int totalTir = NbTir();

        if (totalTir == 0) return 0;

        double pourcentage = (double)(nbPoint * 100) / totalTir;
        return ok ? pourcentage : 100 - pourcentage;
    }
    public string GetStat()
    {
        int totalTirs = NbTir();
        int tirRatee = NbTir(0);
        int tirReussi = totalTirs - tirRatee;

        StringBuilder stats = new();
        stats.AppendLine($"Équipe : {Nom}");
        stats.AppendLine($"    Total points : {TotalPoint()}");
        stats.AppendLine($"    Nb tirs : {totalTirs}");
        stats.AppendLine($"    Nb passe : {NbPasse()}");
        stats.AppendLine($"    Tirs réussis : {tirReussi} ");
        stats.AppendLine($"    Tirs ratés :  {tirRatee} ");
        stats.AppendLine($"    1 point : {NbTir(1)} ");
        stats.AppendLine($"    2 points : {NbTir(2)} ");
        stats.AppendLine($"    3 points : {NbTir(3)} ");


        return stats.ToString();
    }

    public new void Draw(Graphics g)
    {
        foreach (Joueur joueur in Joueurs)
        {
            joueur.Draw(g);
        }
    }

}
