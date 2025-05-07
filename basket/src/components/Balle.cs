using System.Drawing.Drawing2D;
using db;

namespace components;

public class Balle : Rect
{
    public Joueur? Possesseur { get; set; }
    public int Point { get; set; }
    public bool EnTir { get; set; }

    public Balle()
    {
        Width = 10;
        Height = 10;
    }

    public new void Draw(Graphics g)
    {
        // g.DrawEllipse(GetPen(), this.GetRect());
        g.FillEllipse(GetPen().Brush, this.GetRect());
    }
    public override Region GetRegion()
    {
        GraphicsPath path = new();
        path.AddEllipse(GetRect());
        Region result = new(path);
        path.Dispose();
        return result;
    }

    public void Move()
    {
        // throw new NotImplementedException();
    }

    public override string ToString()
    {
        return $"Possesseur : " + Possesseur.Nom;
    }
}