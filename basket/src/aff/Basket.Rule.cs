namespace aff;

using components;
using db;

public partial class Basket
{

    private bool Running { get; set; } = true;
    private void InitRules()
    {
        Running = true;
        Equipe1.SetHaut(true);
        Equipe2.SetHaut(false);
        Equipe1.PrendreBalle(Balle);
    }

    private void Run()
    {
        if (!Running) return;

        Moves();
        BallePoint();
        HorsTerrain();
        Console.WriteLine("Balle.Point : " + Balle.Point);
    }
    public void BallePoint()
    {
        Equipe eActif = GetEquipeActif();
        if (Terrain.Zone2(eActif))
        {
            Balle.Point = 2;
        }
        else
        {
            Balle.Point = 3;
        }
    }
    public void HorsTerrain()
    {
        Joueur? j = GetPossesseur();
        if (j == null) return;
        if (!j.IntersectWith(Terrain))
        {
            Equipe1.Formation();
            Equipe2.Formation();
            EquipeSuivant();
        }

    }
    private void Moves()
    {
        foreach (Joueur joueur in Equipe1.Joueurs) joueur.Move();
        foreach (Joueur joueur in Equipe2.Joueurs) joueur.Move();
    }
    private void EquipeSuivant()
    {
        if (Equipe1.PossedeBalle())
        {
            Equipe1.PerdreBalle();
            Equipe2.PrendreBalle(Balle);
        }
        else
        {
            Equipe2.PerdreBalle();
            Equipe1.PrendreBalle(Balle);
        }
        Passe = null;
    }

    public Equipe GetEquipeActif()
    {
        if (Equipe1.PossedeBalle()) return Equipe1;
        return Equipe2;
    }
    public Joueur? GetPossesseur()
    {
        return GetEquipeActif().GetPossesseur();
    }

}