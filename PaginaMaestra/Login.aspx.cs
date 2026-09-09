using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PaginaMaestra
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Session.Timeout = 5; //minutos

            if (!IsPostBack)
            {
                Session["intentos"] = 0;
                //string hash = HashearContrasena("Sailor"); // O cualquier otra contraseña
                //lblMensaje.Text = "Hash generado: " + hash;
            }

        }

        private string HashearContrasena(string contrasena)
        {
            // Generar salt
            byte[] salt = new byte[16];
            using (var rng = new System.Security.Cryptography.RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }

            // Generar hash con salt
            var pbkdf2 = new System.Security.Cryptography.Rfc2898DeriveBytes(contrasena, salt, 10000);
            byte[] hash = pbkdf2.GetBytes(20);

            // Combinar salt + hash
            byte[] hashBytes = new byte[36];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);

            // Convertir a Base64 para almacenar en la BD
            return Convert.ToBase64String(hashBytes);
        }


        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string usuario = txtUsuario.Text.Trim();
            string contrasena = txtContrasena.Text.Trim();

            if (string.IsNullOrEmpty(usuario) || string.IsNullOrEmpty(contrasena))
            {
                lblMensaje.Text = "Por favor, ingresa usuario y contraseña.";
                return;
            }

            if (Session["bloqueadoHasta"] != null && DateTime.Now < (DateTime)Session["bloqueadoHasta"])
            {
                lblMensaje.Text = "Tu cuenta está bloqueada. Intenta de nuevo en unos minutos.";
                return;
            }

            string connStr = System.Configuration.ConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString;

            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                try
                {
                    conn.Open();

                    // Ahora solo buscamos por usuario y traemos el hash + rol
                    string query = "SELECT contrasena, rol FROM usuarios WHERE usuario = @usuario";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@usuario", usuario);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string storedHash = reader["contrasena"].ToString();
                                string rol = reader["rol"].ToString();

                                // Aquí usamos tu método para comparar
                                if (VerifyPassword(contrasena, storedHash))
                                {
                                    Session["intentosFallidos"] = null;
                                    Session["bloqueadoHasta"] = null;
                                    Session["usuario"] = usuario;
                                    Session["rol"] = rol;

                                    if (rol == "admin")
                                        Response.Redirect("AdminPanel.aspx");
                                    else
                                        Response.Redirect("UsuarioPanel.aspx");

                                    return;
                                }
                            }

                            // Si no entra al if anterior, usuario o contraseña es incorrecto
                            int intentos = Session["intentosFallidos"] != null ? (int)Session["intentosFallidos"] : 0;
                            intentos++;
                            Session["intentosFallidos"] = intentos;

                            if (intentos >= 3)
                            {
                                Session["bloqueadoHasta"] = DateTime.Now.AddMinutes(5);
                                lblMensaje.Text = "Tu cuenta ha sido bloqueada temporalmente. Intenta más tarde.";
                            }
                            else
                            {
                                lblMensaje.Text = "Usuario o contraseña incorrectos.";
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    lblMensaje.Text = "Error al conectar con la base de datos.";
                }
            }
        }

        private bool VerifyPassword(string enteredPassword, string storedHash)
        {
            byte[] hashBytes = Convert.FromBase64String(storedHash);
            byte[] salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, 16);

            byte[] storedHashBytes = new byte[20];
            Array.Copy(hashBytes, 16, storedHashBytes, 0, 20);

            using (var pbkdf2 = new System.Security.Cryptography.Rfc2898DeriveBytes(enteredPassword, salt, 10000))
            {
                byte[] enteredHash = pbkdf2.GetBytes(20);
                for (int i = 0; i < 20; i++)
                {
                    if (enteredHash[i] != storedHashBytes[i])
                        return false;
                }
            }
            return true;
        }

    }
}