namespace db;
using System.Data.Common;
using System.Drawing;
using System.Text;
using components;

public class Joueur : Table
{

    [Column]
    [AutoIncrement]
    public long IdJoueur { get; set; }
    [Column]
    public long IdEquipe { get; set; }
    [Column]
    public string? Nom { get; set; }
    public int Point { get; set; }
    public int NbTirCourant { get; set; }
    public Balle? Balle { get; set; }
    public bool BalleHaut { get; set; }
    public int NbPasseCourant { get; set; }
    public static int JoueurWidth { get; set; } = 42;
    public static int JoueurHeight { get; set; } = 16;

    public Joueur()
    {
        Init();
    }
    public Joueur(DbConnection conn)
    {
        Init();
        SetConn(conn);
    }
    public Joueur(DbDataReader reader) : base(reader)
    {
        Init();
    }

    private void Init()
    {
        Width = JoueurWidth;
        Height = JoueurHeight;
        Speed = 3;
    }

    public override bool Equals(object? obj)
    {
        if (obj == null || GetType() != obj.GetType()) return false;
        Joueur j = (Joueur)obj;
        if (this.IdJoueur != j.IdJoueur) return false;
        return true;
    }

    public void Decisive()
    {
        NbPasseCourant++;
    }
    public long? Tir(Balle balle)
    {
        return Tir(balle.Point);
    }
    public long? Tir(int point)
    {
        long? result;
        Tir data = new(GetConn())
        {
            IdEquipe = IdEquipe,
            IdJoueur = IdJoueur,
            Valeur = point
        };
        result = data.Save();
        Point += point;
        NbTirCourant++;
        return result;
    }

    public void PrendreBalle(Balle balle)
    {
        this.Balle = balle;
        balle.Possesseur = this;
    }

    public void Passe(Joueur joueur)
    {
        if (Balle == null) throw new Exception("aucun ballon a passer");
        joueur.PrendreBalle(this.Balle);
        this.Balle = null;
    }

    public int TotalPoint()
    {
        int result = 0;
        string query = "SELECT sum(valeur) total FROM Tir WHERE IdJoueur = " + IdJoueur;

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
        string query = "SELECT count(idTir) nb FROM Tir WHERE IdJoueur = " + IdJoueur;

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
        string query = "SELECT count(idPasse) nb FROM Passe WHERE IdJoueur = " + IdJoueur;

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
    public double PourcentageTir(int point)
    {
        int totalTir = NbTir();
        int nbPoint = NbTir(point);

        if (totalTir == 0) return 0;

        return (double)(nbPoint * 100) / totalTir;
    }
    public double PourcentageTir(bool ok = true)
    {
        string query = "SELECT count(idTir) nb FROM Tir WHERE Valeur > 0 AND IdJoueur = " + IdJoueur;
        DbCommand stmt = GetConn().CreateCommand();
        stmt.CommandText = query;
        int nbPoint = Convert.ToInt32(stmt.ExecuteScalar());
        int totalTir = NbTir();

        if (totalTir == 0) return 0;

        double pourcentage = (double)(nbPoint * 100) / totalTir;

        if (!ok) return 100 - pourcentage;
        return pourcentage;
    }

    public string GetStat()
    {
        int totalTirs = NbTir();
        int tirRatee = NbTir(0);
        int tirReussi = totalTirs - tirRatee;

        StringBuilder stats = new();
        stats.AppendLine($"Joueur : {Nom}");
        stats.AppendLine($"    Total points : {TotalPoint()}");
        stats.AppendLine($"    Nb tirs : {totalTirs}");
        stats.AppendLine($"    Nb passe : {NbPasse()}");
        stats.AppendLine($"    Tirs réussis : {tirReussi} ");
        stats.AppendLine($"    Tirs ratés :  {tirRatee} ");
        stats.AppendLine($"    1 point : {NbTir(1)}");
        stats.AppendLine($"    2 points : {NbTir(2)} ");
        stats.AppendLine($"    3 points : {NbTir(3)} ");

        return stats.ToString();
    }

    public new void Draw(Graphics g)
    {
        g.FillRectangle(GetPen().Brush, GetRect());
        float y;
        if (BalleHaut) y = (float)(Y + Height + 2);
        else y = (float)(Y - 12);
        g.DrawString(Nom, GetFont(), GetPen().Brush, (float)(X), y);
        DrawBalle(g);
    }
    public void DrawBalle(Graphics g)
    {
        if (Balle == null) return;
        int y;
        if (BalleHaut) y = (int)(Y - Balle.Height / 2) - 2;
        else y = (int)(Y + Height + Balle.Height / 2) + 2;
        Balle.Goto(new Point((int)(X + Width / 2), y));
        Balle.Draw(g);
    }

}

