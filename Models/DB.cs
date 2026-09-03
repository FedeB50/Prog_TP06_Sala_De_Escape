namespace TP06.Models;
using Microsoft.Data.SqlClient;
using Dapper;
using TP06.Models;
public class DB
{
    string _connectionString = @"Server=localhost;DataBase=Tp06_Barg;Integrated Security=True;TrustServerCertificate=True;";
    public void InsertarUsuario(string nombre)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Execute("INSERT INTO Usuarios (nombre) VALUES (@Nombre)", new { Nombre = nombre });
        }
    }
    public int GetIdUsuario()
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            return connection.QueryFirstOrDefault<int>("SELECT MAX(id) FROM Usuarios");
        }
    }
    public void InsertarPartida(int idUsuario)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Execute("INSERT INTO Partidas (idUsuarios, idSalaActual, estado) VALUES (@IdUsuario, 1, 'No iniciada')", new { IdUsuario = idUsuario});
        }
    }
    public void ActualizarSala(int idPartida, int salaActual)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Execute("UPDATE Partidas SET salaActual = @SalaActual WHERE id = @IdPartida", new { IdPartida = idPartida, SalaActual = salaActual });
        }
    }
    public void ActualizarEstado(int idPartida, string estado)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Execute("UPDATE Partidas SET estado = @Estado WHERE id = @IdPartida", new { IdPartida = idPartida, Estado = estado });
        }
    }
    public Salas GetSala(int idSala)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            return connection.QueryFirstOrDefault<Salas>("SELECT * FROM Salas WHERE id = @IdSala", new { IdSala = idSala });
        }
    }
}
