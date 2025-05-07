using db;

namespace aff;

public partial class Basket
{
    private Pen Pen { get; set; }
    private Font Fontt { get; set; }

    public void WriteInfo(Graphics g)
    {
        WriteScore(g);
    }

    private void WriteScore(Graphics g)
    {
        int x = 10;
        int y = 10;

        g.DrawString(Equipe1.Nom + " - point : " + Equipe1.Point + " / tir : " + Equipe1.NbTirCourant + " / Passe : " + Equipe1.NbPasseCourant, GetFont(), GetPen().Brush, new PointF(x, y));
        y += 20;
        y += 10;

        foreach (Joueur joueur in Equipe1.Joueurs)
        {
            g.DrawString(joueur.Nom + " - point : " + joueur.Point + " / tir : " + joueur.NbTirCourant + " / Passe : " + joueur.NbPasseCourant, GetFont(), GetPen().Brush, new PointF(x, y));
            y += 20;
        }

        g.DrawString(" ------ ", GetFont(), GetPen().Brush, new PointF(x, y));
        y += 20;

        g.DrawString(Equipe2.Nom + " - point : " + Equipe2.Point + " / tir : " + Equipe2.NbTirCourant + " / Passe : " + Equipe2.NbPasseCourant, GetFont(), GetPen().Brush, new PointF(x, y));
        y += 20;
        y += 10;

        foreach (Joueur joueur in Equipe2.Joueurs)
        {
            g.DrawString(joueur.Nom + " - point : " + joueur.Point + " / tir : " + joueur.NbTirCourant + " / Passe : " + joueur.NbPasseCourant, GetFont(), GetPen().Brush, new PointF(x, y));
            y += 20;
        }
    }

    private Pen GetPen()
    {
        if (Pen == null) return new Pen(Color.Black, 1);
        return Pen;
    }
    private Font GetFont()
    {
        if (Fontt == null) return new Font("Arial", 12, FontStyle.Regular);
        return Fontt;
    }

}
