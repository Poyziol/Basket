namespace db;
using System.Data.Common;
using System.Reflection;
using components;

public class Table : Rect
{
    protected DbConnection? Conn { get; set; }

    public DbConnection GetConn()
    {
        if (Conn == null)
        {
            throw new Exception("connexion requis");
        }
        return Conn;
    }

    public void SetConn(DbConnection dbconn)
    {
        Conn = dbconn;
    }

    public Table() { }

    public Table(DbDataReader reader)
    {
        foreach (var propertyInfo in GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            if (propertyInfo.CanWrite && propertyInfo.GetCustomAttribute<ColumnAttribute>() != null)
            {
                int ordinal = reader.GetOrdinal(propertyInfo.Name);
                if (ordinal >= 0 && !reader.IsDBNull(ordinal))
                {
                    var value = reader.GetValue(ordinal);
                    propertyInfo.SetValue(this, value);
                }
            }
        }
    }
    public void Init() { }

    // public void Save()
    // {
    //     if (Conn == null)
    //     {
    //         throw new InvalidOperationException("La connexion à la base de données n'est pas définie.");
    //     }

    //     List<PropertyInfo> properties = [.. GetType().GetProperties()
    //         .Where(p => p.CanRead && p.CanWrite && p.GetValue(this) != null
    //             && p.GetCustomAttribute<ColumnAttribute>() != null
    //             && p.GetCustomAttribute<AutoIncrementAttribute>() == null)];

    //     if (properties.Count == 0)
    //     {
    //         throw new InvalidOperationException("Aucun attribut défini, impossible d'insérer en base de données.");
    //     }

    //     string tableName = GetType().Name.ToLower();
    //     string columns = string.Join(", ", properties.Select(p => p.Name));
    //     string values = string.Join(", ", properties.Select(p => $"'{p.GetValue(this)}'"));

    //     string query = $"INSERT INTO {tableName} ({columns}) VALUES ({values})";

    //     try
    //     {
    //         using DbCommand cmd = Conn.CreateCommand();
    //         cmd.CommandText = query;
    //         cmd.ExecuteNonQuery();
    //     }
    //     catch (Exception ex)
    //     {
    //         Console.WriteLine($"Erreur lors de l'insertion : {ex.Message}");
    //     }
    // }

    public long? Save()
    {
        if (Conn == null) throw new InvalidOperationException("La connexion à la base de données n'est pas définie.");

        List<PropertyInfo> properties = [.. GetType().GetProperties()
        .Where(p => p.CanRead && p.CanWrite && p.GetValue(this) != null
            && p.GetCustomAttribute<ColumnAttribute>() != null
            && p.GetCustomAttribute<AutoIncrementAttribute>() == null)];

        if (properties.Count == 0) throw new InvalidOperationException("Aucun attribut défini, impossible d'insérer en base de données.");


        string tableName = GetType().Name.ToLower();
        string columns = string.Join(", ", properties.Select(p => p.Name));
        string values = string.Join(", ", properties.Select(p => $"'{p.GetValue(this)}'"));

        string insertQuery = $"INSERT INTO {tableName} ({columns}) VALUES ({values})";

        var autoIncrementProperty = GetType().GetProperties()
            .FirstOrDefault(p => p.GetCustomAttribute<AutoIncrementAttribute>() != null);

        try
        {
            using DbCommand cmd = Conn.CreateCommand();
            cmd.CommandText = insertQuery;
            cmd.ExecuteNonQuery();

            if (autoIncrementProperty != null)
            {
                string lastIdQuery = $"SELECT MAX({autoIncrementProperty.Name}) FROM {tableName}";
                cmd.CommandText = lastIdQuery;
                object? insertedId = cmd.ExecuteScalar();

                return (long)insertedId;
            }
        }
        catch (Exception ex)
        {
        }
        return null;
    }

    public static List<T> GetAll<T>(DbConnection conn) where T : Table
    {
        List<T> result = [];
        string query = $"SELECT * FROM {typeof(T).Name.ToLower()}";
        try
        {
            DbCommand stmt = conn.CreateCommand();
            stmt.CommandText = query;

            using DbDataReader reader = stmt.ExecuteReader();
            while (reader.Read())
            {
                var constructor = typeof(T).GetConstructor([typeof(DbDataReader)]);
                if (constructor != null)
                {
                    T instance = (T)constructor.Invoke([reader]);
                    instance.SetConn(conn);
                    result.Add(instance);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur lors de la sélection : {ex.Message}");
        }
        return result;
    }

    public static void PrintAll<T>(List<T> tables) where T : Table
    {
        foreach (Table table in tables)
        {
            Console.WriteLine(table.ToString());
        }
    }

    public override string ToString()
    {
        var properties = GetType().GetProperties();
        string result = string.Empty;

        foreach (var property in properties)
        {
            result += $"{property.Name}: {property.GetValue(this)} | ";
        }
        return result.TrimEnd(' ', '|');
    }
}

[AttributeUsage(AttributeTargets.Property)]
public class AutoIncrementAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Property)]
public class ColumnAttribute : Attribute { }
