namespace aff;
using db;
public partial class Basket
{
    private System.ComponentModel.IContainer components = null;
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    private void InitWindow()
    {
        StartPosition = FormStartPosition.CenterScreen;
        components = new System.ComponentModel.Container();
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new System.Drawing.Size(1540, 1000);
        Text = "tennis";
        Stat = new(this);
    }
}