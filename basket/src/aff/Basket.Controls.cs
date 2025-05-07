using components;
using db;

namespace aff;

public partial class Basket
{

    private Joueur? JoueurMobile { get; set; }
    private bool SelectionPasse { get; set; }

    private void SetControls()
    {
        this.KeyPreview = true;
        this.KeyDown += Basket_KeyDown;
        this.KeyUp += Baskett_KeyUp;
        this.MouseClick += Basket_MouseClick;
    }

    private void Basket_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.P)
        {
            // PasseOn();
            PasseAvant();
        }
        if (e.KeyCode == Keys.T)
        {
            TirOn();
        }
        if (e.KeyCode == Keys.Y)
        {
            TirOn(false);
        }
        MoveOn(e);
    }

    private void Baskett_KeyUp(object? sender, KeyEventArgs e)
    {
        MoveOff(e);
    }
    private void Basket_MouseClick(object? sender, MouseEventArgs e)
    {
        PassePerformed(e);
        ChangeJoueurMobile(e);
    }

    private void PasseAvant()
    {
        Joueur j = GetPossesseur();
        Equipe e = GetEquipeActif();
        Joueur cible = e.Avant(j);
        Passe ??= new Passe(Conn);
        Passe.IdJoueur = j.IdJoueur;
        j.Passe(cible);
    }

    private void ChangeJoueurMobile(MouseEventArgs e)
    {
        Joueur? j = GetSelected(e);
        if (j == null) return;
        JoueurMobile?.Stop();
        JoueurMobile = j;
    }

    private Joueur? GetSelected(MouseEventArgs e)
    {
        foreach (Joueur joueur in Equipe1.Joueurs) if (joueur.Include(new Point(e.X, e.Y))) return joueur;
        foreach (Joueur joueur in Equipe2.Joueurs) if (joueur.Include(new Point(e.X, e.Y))) return joueur;
        return null;
    }
    private void PasseOn()
    {
        SelectionPasse = true;
    }
    private void PassePerformed(MouseEventArgs e)
    {
        if (SelectionPasse)
        {
            Passe ??= new Passe(Conn);
            Equipe equipe = GetEquipeActif();
            Joueur? j1 = equipe.GetPossesseur();
            Joueur? j2 = GetSelected(e);
            if (j1 != null && j2 != null)
            {
                j1.Passe(j2);
                if (j1.IdEquipe == j2.IdEquipe)
                {
                    Passe.IdJoueur = j1.IdJoueur;
                }
                else
                {
                    Passe = null;
                }
                SelectionPasse = false;
            }

        }
    }
    private void TirOn(bool but = true)
    {
        try
        {
            long? idTir = 0;
            Equipe equipe = GetEquipeActif();
            if (but)
            {
                //
                Joueur? possesseur = GetPossesseur();
                PointF jCenter = possesseur.Center();
                if (possesseur.IdEquipe == 1 && jCenter.Y < Terrain.Y + (Terrain.Height / 2))
                {
                    Console.WriteLine("---");
                    Equipe2.Point++;
                    EquipeSuivant();
                    return;
                }
                else
                {
                    if (possesseur.IdEquipe != 1 && jCenter.Y > Terrain.Y + (Terrain.Height / 2))
                    {
                        Console.WriteLine("---");
                        Equipe1.Point++;
                        EquipeSuivant();
                        return;
                    }
                }

                //
                idTir = equipe.GetPossesseur()?.Tir(Balle);
                equipe.Point += Balle.Point;
                if (Passe != null && Balle.Point > 0)
                {
                    equipe.NbPasseCourant++;
                    foreach (Joueur j in equipe.Joueurs) if (j.IdJoueur == Passe.IdJoueur) j.Decisive();
                    Passe.IdTir = idTir != null ? (int)idTir : 0;
                    Passe.Save();
                }
            }
            else
            {
                idTir = equipe.GetPossesseur()?.Tir(0);
            }
            equipe.NbTirCourant++;

            EquipeSuivant();
        }
        catch (Exception ex)
        {
            Console.WriteLine("TirOn - " + ex.Message);
        }
    }
    private void MoveOn(KeyEventArgs e)
    {
        if (JoueurMobile == null) return;
        if (e.KeyCode == Keys.W) JoueurMobile.Up = true;
        if (e.KeyCode == Keys.S) JoueurMobile.Down = true;
        if (e.KeyCode == Keys.D) JoueurMobile.Right = true;
        if (e.KeyCode == Keys.A) JoueurMobile.Left = true;
    }
    private void MoveOff(KeyEventArgs e)
    {
        if (JoueurMobile == null) return;
        if (e.KeyCode == Keys.W) JoueurMobile.Up = false;
        if (e.KeyCode == Keys.S) JoueurMobile.Down = false;
        if (e.KeyCode == Keys.D) JoueurMobile.Right = false;
        if (e.KeyCode == Keys.A) JoueurMobile.Left = false;
    }

}