using Encuestas.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Encuestas.DAL
{
    public class ConexionQuerys
    {
        //readonly string connectionString = "Server=localhost\\MSSQLSERVER;Database=master;Trusted_Connection=True;";
        readonly string connectionString = "Data Source=localhost;Initial Catalog=master;Integrated Security=True;";
        public bool UserRegister(string user, string pass)
        {
            if (IsUserRegistered(user))
            {
                return false;
            }
            string consulta = "Insert into Usuario(Usuario,Pass) values (@NombreUsuario,@Contraseña);";
            using SqlConnection connection = new SqlConnection(this.connectionString);
            connection.Open();

            using SqlCommand command = new SqlCommand(consulta, connection);
            command.Parameters.AddWithValue("@NombreUsuario", user);
            command.Parameters.AddWithValue("@Contraseña", pass);

            int count = command.ExecuteNonQuery();

            return count > 0;
        }
        public bool UserLogin(string user, string pass)
        {
            string consulta = "SELECT Pass FROM Usuario WHERE Usuario = @NombreUsuario";

            using SqlConnection connection = new SqlConnection(this.connectionString);
            connection.Open();

            using SqlCommand command = new SqlCommand(consulta, connection);
            command.Parameters.AddWithValue("@NombreUsuario", user);

            string storedHash = command.ExecuteScalar() as string;

            if (storedHash != null && BCrypt.Net.BCrypt.Verify(pass, storedHash))
            {
                return true;
            }
            return false;
        }
        private bool IsUserRegistered(string user)
        {
            string consulta = "SELECT COUNT(*) FROM Usuario WHERE Usuario = @NombreUsuario;";

            using SqlConnection connection = new SqlConnection(this.connectionString);
            connection.Open();

            using SqlCommand command = new SqlCommand(consulta, connection);
            command.Parameters.AddWithValue("@NombreUsuario", user);

            int count = Convert.ToInt32(command.ExecuteScalar());

            return count > 0;
        }
        public List<Encuesta> ObtenerEncuestas(string user)
        {
            List<Encuesta> productos = new List<Encuesta>();

            using (SqlConnection connection = new SqlConnection(this.connectionString))
            {
                connection.Open();

                string query = "SELECT e.IDEncuesta, e.Nombre, e.Descripcion,e.Link,e.IDUser FROM Encuesta e inner join Usuario u on e.IDUser = u.IDUser where e.eliminado = 0 and u.Usuario = @NombreUsuario;";
                using SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@NombreUsuario", user);

                using SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Encuesta producto = new Encuesta
                    {
                        IDEncuesta = reader.GetInt32(0),
                        Nombre = reader.GetString(1),
                        Descripcion = reader.GetString(2),
                        Link = reader.GetString(3),
                        IDUser = reader.GetInt32(4)
                    };

                    productos.Add(producto);
                }
            }

            return productos;
        }
        public bool GuardarEncuesta(string descripcion, string nombre,string usuario, List<CampoEncuesta> campos)
        {
            try
            {
                string consulta = "SELECT IDUser FROM Usuario WHERE Usuario = @NombreUsuario";

                using SqlConnection connection = new SqlConnection(this.connectionString);
                connection.Open();

                using SqlCommand command = new SqlCommand(consulta, connection);
                command.Parameters.AddWithValue("@NombreUsuario", usuario);

                int? idUsuario = command.ExecuteScalar() as int?;


                string consulta2 = "INSERT INTO Encuesta (Nombre, Descripcion,Link,IDUser) VALUES (@Nombre, @Descripcion,NEWID(),@IDUser); SELECT SCOPE_IDENTITY();";

                using SqlCommand command2 = new SqlCommand(consulta2, connection);
                command2.Parameters.AddWithValue("@Nombre", nombre);
                command2.Parameters.AddWithValue("@Descripcion", descripcion);
                command2.Parameters.AddWithValue("@IDUser", Convert.ToInt32(idUsuario));
                int ultimoIDInsertado = Convert.ToInt32(command2.ExecuteScalar());

                GuardarCampos(ultimoIDInsertado, campos);

                return true;
            }
            catch
            {
                return false;
            }
           
        }
        private bool GuardarCampos(int IDEncuesta, List<CampoEncuesta> campos)
        {
            foreach (CampoEncuesta campo in campos)
            {
                using (SqlConnection connection = new SqlConnection(this.connectionString))
                {
                    string consulta = "INSERT INTO Campo (Nombre, Titulo, EsRequerido, IDTipo, IDEncuesta) " +
                                      "VALUES(@Nombre, @Titulo, @EsRequerido, @IDTipo, @IDEncuesta); SELECT SCOPE_IDENTITY();";

                    using (SqlCommand cmd = new SqlCommand(consulta, connection))
                    {
                        cmd.Parameters.AddWithValue("@Nombre", campo.Nombre);
                        cmd.Parameters.AddWithValue("@Titulo", campo.Titulo);
                        cmd.Parameters.AddWithValue("@EsRequerido", campo.Requerido);
                        cmd.Parameters.AddWithValue("@IDTipo", campo.Tipo);
                        cmd.Parameters.AddWithValue("@IDEncuesta", IDEncuesta);

                        connection.Open();

                        // Ejecutar la consulta y obtener el último ID insertado
                        int ultimoIDInsertado = Convert.ToInt32(cmd.ExecuteScalar());

                        connection.Close();

                        // 'ultimoIDInsertado' ahora contiene el ID del nuevo registro insertado en la tabla Campo
                        Console.WriteLine($"Campo insertado con ID: {ultimoIDInsertado}");
                    }
                }
            }
            return true;
        }

        public List<TipoDatos> ObtenerTipoDatos()
        {
            List<TipoDatos> listado = new List<TipoDatos>();

            using (SqlConnection connection = new SqlConnection(this.connectionString))
            {
                connection.Open();

                string query = "SELECT IDTipo, Tipo FROM TipoDato;";
                using SqlCommand command = new SqlCommand(query, connection);

                using SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    TipoDatos tipoDatos = new TipoDatos
                    {
                        Id = reader.GetInt32(0),
                        Tipo = reader.GetString(1),
                    };

                    listado.Add(tipoDatos);
                }
            }

            return listado;
        }

        public Encuesta ObtenerEnlace(string guid)
        {
           Encuesta encuesta = new Encuesta();
            List<CampoEncuesta> campo = new List<CampoEncuesta>();

            using (SqlConnection connection = new SqlConnection(this.connectionString))
            {
                connection.Open();

                string query = "SELECT IDEncuesta, Nombre , Descripcion FROM Encuesta e WHERE e.Link = @Guid;";
                using SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Guid", guid);

                using SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    encuesta.IDEncuesta = reader.GetInt32(0);
                    encuesta.Nombre = reader.GetString(1);
                    encuesta.Descripcion = reader.GetString(2);
                }
            }

            using (SqlConnection connection = new SqlConnection(this.connectionString))
            {
                connection.Open();

                string query = "SELECT c.IDCampo, c.Nombre , c.Titulo , c.IDTipo, c.EsRequerido, td.Tipo  FROM Encuesta e inner join Campo c on e.IDEncuesta = c.IDEncuesta INNER JOIN TipoDato td on td.IDTipo = c.IDTipo WHERE e.Link = @guid;";
                using SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@guid", guid);

                using SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    CampoEncuesta list = new CampoEncuesta
                    {
                        IDCampo = reader.GetInt32(0),
                        Nombre = reader.GetString(1),
                        Titulo = reader.GetString(2),
                        Tipo = reader.GetInt32(3),
                        EsRequerido = reader.GetBoolean(4),
                        TipoText = reader.GetString(5),
                    };

                    campo.Add(list);
                }
                encuesta.Campos = campo;
            }

            return encuesta;
        }

        public bool GuardarRespuesta(List<CampoEncuesta> encuestas)
        {
            bool respuesta = true;
            try
            {
                foreach (var list in encuestas)
                {
                    using (SqlConnection connection = new SqlConnection(this.connectionString))
                    {
                        connection.Open();

                        string query = "INSERT INTO Respuesta (IDEncuesta, IDCampo, Respuesta) VALUES(@IdEncuesta, @IdCampo, @Respuesta);";
                        using SqlCommand command = new SqlCommand(query, connection);
                        command.Parameters.AddWithValue("@IdEncuesta", list.IDEncuesta);
                        command.Parameters.AddWithValue("@IdCampo", list.IDCampo);
                        command.Parameters.AddWithValue("@Respuesta", list.Nombre);

                        using SqlDataReader reader = command.ExecuteReader();
                    }
                }
            }catch (Exception ex)
            {
                respuesta = false;
            }

            return respuesta;
        }

        public Encuesta VerRespuestaEnlace(string guid)
        {
            Encuesta encuesta = new Encuesta();
            List<CampoEncuesta> campo = new List<CampoEncuesta>();

            using (SqlConnection connection = new SqlConnection(this.connectionString))
            {
                connection.Open();

                string query = "SELECT IDEncuesta, Nombre , Descripcion FROM Encuesta e WHERE e.Link = @Guid;";
                using SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Guid", guid);

                using SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    encuesta.IDEncuesta = reader.GetInt32(0);
                    encuesta.Nombre = reader.GetString(1);
                    encuesta.Descripcion = reader.GetString(2);
                }
            }

            using (SqlConnection connection = new SqlConnection(this.connectionString))
            {
                connection.Open();

                string query = "SELECT DISTINCT r.IDCampo, r.Respuesta, c.Titulo FROM Encuesta e join Respuesta r on e.IDEncuesta = r.IDEncuesta INNER JOIN Campo c on c.IDCampo  = r.IDCampo WHERE e.Link = @guid and e.eliminado=0";
                using SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@guid", guid);

                using SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    CampoEncuesta list = new CampoEncuesta
                    {
                        IDCampo = reader.GetInt32(0),
                        Nombre = reader.GetString(1),
                        Titulo = reader.GetString(2)
                    };

                    campo.Add(list);
                }
                encuesta.Campos = campo;
            }

            return encuesta;
        }

        
        public bool EliminiarEncuesta(int IdEncuesta)
        {
            bool respuesta = true;
            try
            {
                using (SqlConnection connection = new SqlConnection(this.connectionString))
                {
                    connection.Open();

                    string query = "UPDATE Encuesta SET eliminado=1 WHERE IDEncuesta = @IdEncuesta;";
                    using SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@IdEncuesta", IdEncuesta);

                    using SqlDataReader reader = command.ExecuteReader();
                }
            }
            catch (Exception ex)
            {
                respuesta = false;
            }

            return respuesta;
        }

        public bool GuardarEncuestaActualizada(string Nombre, string Descripcion, int IdEncuesta)
        {
            bool respuesta = true;
            try
            {
                using (SqlConnection connection = new SqlConnection(this.connectionString))
                {
                    connection.Open();

                    string query = "UPDATE Encuesta SET Nombre=@nombre, Descripcion = @desc WHERE IDEncuesta = @IdEncuesta;";
                    using SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@nombre", Nombre);
                    command.Parameters.AddWithValue("@desc", Descripcion);
                    command.Parameters.AddWithValue("@IdEncuesta", IdEncuesta);

                    using SqlDataReader reader = command.ExecuteReader();
                }
            }
            catch (Exception ex)
            {
                respuesta = false;
            }

            return respuesta;
        }
    }

}
