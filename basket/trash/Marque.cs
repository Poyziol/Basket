using System.Drawing.Drawing2D;

using components;
namespace trash;

public class Marque
{
    public int Valeur { get; set; }
    public Point[] Points { get; set; }
    public Pen Pen { get; set; }
    public Font Fontt { get; set; }

    public Color StructureColor { get; set; }

    public Marque(Point[] points)
    {
        Points = points;
        Pen = new(Color.Black, 1);
        Fontt = new("Arial", 12, FontStyle.Regular);
        StructureColor = Color.Black;
    }

    public void Draw(Graphics g)
    {
        PointF center = Center();
        center.X -= 6;
        // center.Y -= 5;
        Pen.Color = StructureColor;
        g.DrawPolygon(Pen, Points);
        g.DrawString(Valeur.ToString(), Fontt, Pen.Brush, center);
    }

    public virtual void Move()
    {
    }

    public bool IntersectWith(Rect rect)
    {
        bool result = true;
        Region r1 = GetRegion();
        Region r2 = new(rect.GetRect());
        Graphics g = Graphics.FromImage(new Bitmap(1, 1));
        r2.Intersect(r1);
        if (r2.IsEmpty(g))
        {
            result = false;
        }
        r1.Dispose();
        r2.Dispose();
        g.Dispose();
        return result;
    }
    public Region GetRegion()
    {
        GraphicsPath path = new();
        path.AddPolygon(Points);
        Region result = new(path);
        path.Dispose();
        return result;
    }
    public PointF Center()
    {
        GraphicsPath path = new();
        path.AddPolygon(Points);
        if (path == null) throw new ArgumentNullException(nameof(path));
        RectangleF bounds = path.GetBounds();
        float centerX = bounds.Left + bounds.Width / 2;
        float centerY = bounds.Top + bounds.Height / 2;
        return new PointF(centerX, centerY);
    }
}