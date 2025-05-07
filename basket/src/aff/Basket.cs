using components;
using db;
using System.Data.Common;

namespace aff;

public partial class Basket : Form
{
    DbConnection Conn { get; set; }
    private Terrain Terrain;
    private Balle Balle;
    private Equipe Equipe1 { get; set; }
    private Equipe Equipe2 { get; set; }
    private Stat Stat { get; set; }
    private Passe? Passe { get; set; }
    private Thread animationThread;

#pragma warning disable CS8618
    public Basket(DbConnection conn)
#pragma warning restore CS8618
    {
        InitWindow();
        Conn = conn;
        DoubleBuffered = true;
        int TerrainWidth = 460;
        int TerrainHeight = 780;
        int TerrainX = Width / 2 - TerrainWidth / 2;
        int TerrainY = Height / 2 - TerrainHeight / 2 - 20;
        Terrain = new(TerrainX, TerrainY, TerrainWidth, TerrainHeight);
        Balle = new();
        SetControls();
        Init();
        StartAnimation();
    }

    private void Init()
    {
        List<Equipe> equipes = Table.GetAll<Equipe>(Conn);
        Equipe1 = equipes[0];
        Equipe2 = equipes[1];
        Equipe1.Terrain = Terrain;
        Equipe2.Terrain = Terrain;
        Equipe1.Init();
        Equipe2.Init();
        Equipe1.SetColor(Color.FromArgb(40, 250, 40));
        Equipe2.SetColor(Color.FromArgb(0, 0, 250));
        InitRules();
    }

    public void StartAnimation()
    {
        animationThread = new(Animate)
        {
            IsBackground = true
        };
        animationThread.Start();
    }

    private void Animate()
    {
        while (true)
        {
            UpdateGame();
            try
            {
                Invoke(new Action(Invalidate));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                // break;
            }
            Thread.Sleep(10);
        }
    }
    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
        Graphics g = e.Graphics;

        Terrain.Draw(g);
        Equipe1.Draw(g);
        Equipe2.Draw(g);
        Balle.Draw(g);

        WriteInfo(g);
    }

    private void UpdateGame()
    {
        Task.Run(() =>
        {
            try
            {
                Run();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        });
    }

    public Equipe GetEquipe1()
    {
        return Equipe1;
    }
    public Equipe GetEquipe2()
    {
        return Equipe2;
    }

}