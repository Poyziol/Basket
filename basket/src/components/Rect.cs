using System.Drawing.Drawing2D;

namespace components;

public class Rect
{
    public double X { get; set; }
    public double Y { get; set; }
    public double Width { get; set; }
    public double Height { get; set; }
    public double Speed { get; set; }
    public bool Right { get; set; }
    public bool Left { get; set; }
    public bool Up { get; set; }
    public bool Down { get; set; }
    public Color StructureColor { get; set; }
    public Pen Pen { get; set; }
    public Font Font { get; set; }

    public Rect() { }
    public Rect(double x, double y, double width, double height)
    {
        X = x;
        Y = y;
        Width = width;
        Height = height;
        Speed = 1;
    }

    public void Move()
    {
        if (Right) X += Speed;
        if (Left) X -= Speed;
        if (Down) Y += Speed;
        if (Up) Y -= Speed;
    }
    public void Stop()
    {
        Up = false;
        Down = false;
        Left = false;
        Right = false;
    }

    public Font GetFont()
    {
        if (Font == null)
        {
            Font = new("Arial", 8, FontStyle.Regular);
        }
        return Font;
    }
    public Pen GetPen()
    {
        if (Pen == null)
        {
            Pen = new(GetStructureColor(), 1);
        }
        return Pen;
    }
    public void SetColor(Color color)
    {
        StructureColor = color;
        Pen = new Pen(StructureColor);
    }
    public Color GetStructureColor()
    {
        if (StructureColor.IsEmpty)
        {
            StructureColor = Color.FromArgb(0, 0, 0);
        }
        return StructureColor;
    }
    public void Draw(Graphics g)
    {
        g.DrawRectangle(GetPen(), this.GetRect());
    }

    public void Goto(Point point)
    {
        X = point.X - Width / 2;
        Y = point.Y - Height / 2;
    }

    public Point Center()
    {
        return new Point((int)(X + Width / 2), (int)(Y + Height / 2));
    }

    public bool OnTopOf(Rect rect)
    {
        return this.Center().Y < rect.Center().Y;
    }
    public bool OnBottomOf(Rect rect)
    {
        return this.Center().Y > rect.Center().Y;
    }
    public bool OnLeftOf(Rect rect)
    {
        return this.Center().X < rect.Center().X;
    }
    public bool OnRightOf(Rect rect)
    {
        return this.Center().X > rect.Center().X;
    }

    public virtual Rectangle GetRect()
    {
        return new Rectangle((int)X, (int)Y, (int)Width, (int)Height);
    }

    public virtual Region GetRegion()
    {
        GraphicsPath path = new();
        path.AddRectangle(GetRect());
        Region result = new(path);
        path.Dispose();
        return result;
    }
    public bool IntersectWith(Rect rect)
    {
        bool result = true;
        Region r1 = GetRegion();
        Region r2 = rect.GetRegion();
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

    public bool Include(Point point)
    {
        return GetRegion().IsVisible(point);
    }
}