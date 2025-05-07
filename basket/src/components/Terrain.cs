using System.Drawing.Drawing2D;
using db;

namespace components;

public class Terrain : Rect
{
    private int OffsetDemiCircle { get; set; } = 80;
    public DemiCircle DemiCircle1 { get; set; }
    public Rect MiniRect1 { get; set; }
    public DemiCircle DemiCircle2 { get; set; }
    public Rect MiniRect2 { get; set; }
    private int LimitLR { get; set; } = 60;
    public Circle CentralCircle { get; set; }
    private int CentralCircleWidth { get; set; } = 120;

    public Terrain()
    {
        Init();
    }
    public Terrain(int x, int y, int width, int height) : base(x, y, width, height)
    {
        Width = width;
        Init();
    }
    public void Init()
    {
        double CircleHeight = Height / 2.5;
        DemiCircle1 = new DemiCircle(X + LimitLR, Y - OffsetDemiCircle, Width - (LimitLR * 2), CircleHeight);
        DemiCircle2 = new DemiCircle(X + LimitLR, Y + Height + OffsetDemiCircle - CircleHeight, Width - (LimitLR * 2), CircleHeight)
        {
            Inverse = true
        };
        MiniRect1 = new(X + LimitLR, Y, Width - (LimitLR * 2), OffsetDemiCircle);
        MiniRect2 = new(X + LimitLR, Y + Height - OffsetDemiCircle, Width - (LimitLR * 2), OffsetDemiCircle);

        CentralCircle = new((int)(X + Width / 2 - CentralCircleWidth / 2), (int)(Y + Height / 2 - CentralCircleWidth / 2), CentralCircleWidth, CentralCircleWidth);
    }

    public bool Zone2(Equipe e)
    {
        Joueur j = e.GetPossesseur() ?? throw new Exception("cette equipe n'a pas de ballon");
        if (e.Haut == false)
        {
            if (j.IntersectWith(DemiCircle1) || j.IntersectWith(MiniRect1))
            {
                return true;
            }
        }
        else
        {
            if (j.IntersectWith(DemiCircle2) || j.IntersectWith(MiniRect2))
            {
                return true;
            }
        }
        return false;
    }

    public override Rectangle GetRect()
    {
        return new Rectangle((int)X + Joueur.JoueurWidth, (int)Y + Joueur.JoueurHeight, (int)(Width - Joueur.JoueurWidth * 2), (int)(Height - Joueur.JoueurHeight * 2));
    }
    public Rectangle GetPlayingArea()
    {
        return new Rectangle((int)X, (int)Y, (int)Width, (int)Height);
    }


    public new void Draw(Graphics g)
    {
        // base.Draw(g);
        g.DrawRectangle(GetPen(), this.GetPlayingArea());

        // supp
        DemiCircle1.Draw(g);
        g.DrawLine(GetPen(), new PointF((float)MiniRect1.X, (float)MiniRect1.Y), new PointF((float)MiniRect1.X, (float)(MiniRect1.Y + MiniRect1.Height)));
        g.DrawLine(GetPen(), new PointF((float)(MiniRect1.X + MiniRect1.Width), (float)MiniRect1.Y), new PointF((float)(MiniRect1.X + MiniRect1.Width), (float)(MiniRect1.Y + MiniRect1.Height)));
        // inf
        DemiCircle2.Draw(g);
        g.DrawLine(GetPen(), new PointF((float)MiniRect2.X, (float)MiniRect2.Y), new PointF((float)MiniRect2.X, (float)(MiniRect2.Y + MiniRect2.Height)));
        g.DrawLine(GetPen(), new PointF((float)(MiniRect2.X + MiniRect2.Width), (float)MiniRect2.Y), new PointF((float)(MiniRect2.X + MiniRect2.Width), (float)(MiniRect2.Y + MiniRect2.Height)));
        // centre
        g.DrawLine(GetPen(), new PointF((float)X, (float)(Y + Height / 2)), new PointF((float)(X + Width), (float)(Y + Height / 2)));
        CentralCircle.Draw(g);
        // but
        int butWidth = 10;
        int butHeight = 10;
        g.DrawArc(GetPen(), new Rectangle((int)(X + Width / 2 - butWidth / 2), (int)(Y - butHeight / 2), butWidth, butHeight), 0, 180);
        g.DrawArc(GetPen(), new Rectangle((int)(X + Width / 2 - butWidth / 2), (int)(Y + Height - butHeight / 2), butWidth, butHeight), 180, 180);
    }

}


public class DemiCircle : Circle
{

    public bool Inverse { get; set; }
    public DemiCircle(double x, double y, double width, double height) : base(x, y, width, height)
    {

    }
    public new void Draw(Graphics g)
    {
        int startAngle = 0;
        int endAngle = Inverse ? -180 : 180;
        g.DrawArc(GetPen(), GetRect(), startAngle, endAngle);
        // g.DrawRectangle(GetPen(), GetRect());
    }
}