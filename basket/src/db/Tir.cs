namespace db;
using System.Data.Common;

public class Tir : Table
{
    [Column]
    [AutoIncrement]
    public long IdTir { get; set; }
    [Column]
    public long IdEquipe { get; set; }
    [Column]
    public long IdJoueur { get; set; }
    [Column]
    public long Valeur { get; set; }

    public Tir() { }
    public Tir(DbConnection conn)
    {
        SetConn(conn);
    }
    public Tir(DbDataReader reader) : base(reader) { }

}
