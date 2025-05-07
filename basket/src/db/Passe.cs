namespace db;
using System.Data.Common;

public class Passe : Table
{
    [Column]
    [AutoIncrement]
    public long IdPasse { get; set; }
    [Column]
    public long IdTir { get; set; }
    [Column]
    public long IdJoueur { get; set; }

    public Passe() { }
    public Passe(DbConnection conn)
    {
        SetConn(conn);
    }
    public Passe(DbDataReader reader) : base(reader) { }

}
