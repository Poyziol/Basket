namespace db;

using aff;

public class Stat : Form
{
    private System.ComponentModel.IContainer components = null;
    private ListBox ListBox1 { get; set; }
    private ListBox ListBox2 { get; set; }
    private Button Btn { get; set; }
    private Basket Basket;

    public Stat(Basket basket)
    {
        Basket = basket;
        Init();
    }

    private void Init()
    {
        StartPosition = FormStartPosition.CenterScreen;
        components = new System.ComponentModel.Container();
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new System.Drawing.Size(620, 720);
        Text = "Statistiques";
        SetBtn();
        SetList();
        Show();
        UpdateList();
    }

    private void SetBtn()
    {
        Btn = new Button()
        {
            Text = "update",
            Name = "update",
            Size = new Size(100, 30)
        };
        Btn.Location = new Point(Width - Btn.Width - 30, Height - Btn.Height - 50);
        Btn.Click += new EventHandler(Update);
        Controls.Add(Btn);
    }

    private void SetList()
    {
        ListBox1 = new ListBox();
        ListBox1.SetBounds(0, 0, 300, 720);

        ListBox2 = new ListBox();
        ListBox2.SetBounds(320, 0, 300, 720);

        Controls.Add(ListBox1);
        Controls.Add(ListBox2);
    }

    private void Update(object? sender, EventArgs e)
    {
        UpdateList();
    }

    private void UpdateList()
    {
        ListBox1.Items.Clear();
        ListBox2.Items.Clear();

        if (Basket.GetEquipe1() == null || Basket.GetEquipe2() == null)
        {
            ListBox1.Items.Add("Aucune équipe trouvée.");
            ListBox2.Items.Add("Aucune équipe trouvée.");
            return;
        }

        AjouterStatsEquipe1();
        AjouterStatsEquipe2();
    }

    private void AjouterStatsEquipe1()
    {
        ListBox1.Items.AddRange(Basket.GetEquipe1().GetStat().Split('\n'));
        foreach (Joueur joueur in Basket.GetEquipe1().Joueurs)
        {
            ListBox1.Items.AddRange(joueur.GetStat().Split("\n"));
        }
    }
    private void AjouterStatsEquipe2()
    {
        ListBox2.Items.AddRange(Basket.GetEquipe2().GetStat().Split('\n'));
        foreach (Joueur joueur in Basket.GetEquipe2().Joueurs)
        {
            ListBox2.Items.AddRange(joueur.GetStat().Split("\n"));
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }
}
