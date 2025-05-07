using System.Drawing.Drawing2D;
using components;

namespace components;

public class Circle : Rect
{

    public Circle(double x, double y, double width, double height) : base(x, y, width, height)
    {

    }
    public new void Draw(Graphics g)
    {
        g.DrawEllipse(GetPen(), this.GetRect());
        // g.FillEllipse(GetPen().Brush, this.GetRect());
    }
    public override Region GetRegion()
    {
        GraphicsPath path = new();
        path.AddEllipse(GetRect());
        Region result = new(path);
        path.Dispose();
        return result;
    }
}