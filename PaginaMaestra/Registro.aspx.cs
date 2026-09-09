using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;


namespace PaginaMaestra
{
    public partial class Registro : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["usuario"] == null)
            {
                Response.Redirect("Login.aspx");
            }


            if (!IsPostBack)
            {
                CargarUsuarios();
            }
        }

        protected void btnRegistrar_Click(object sender, EventArgs e)
        {
            string usuario = txtNuevoUsuario.Text.Trim();
            string contrasena = txtNuevaContrasena.Text.Trim();
            string rol = ddlRol.SelectedValue;

            lblMensaje.ForeColor = System.Drawing.Color.Red;

            // Validaciones (igual que antes) ...
            if (string.IsNullOrEmpty(usuario) || string.IsNullOrEmpty(contrasena))
            {
                lblMensaje.Text = "Por favor, completa todos los campos.";
                return;
            }

            if (!Regex.IsMatch(usuario, @"^[a-zA-Z0-9]{1,10}$"))
            {
                lblMensaje.Text = "El usuario solo debe contener letras y números, entre 1 y 10 caracteres.";
                return;
            }

            if (!Regex.IsMatch(contrasena, @"^[A-Za-z0-9!@#$%^&*()_+\-=\[\]{};':""\\|,.<>\/?]{1,15}$"))
            {
                lblMensaje.Text = "La contraseña puede incluir letras, números y símbolos especiales, hasta 15 caracteres.";
                return;
            }

            bool tieneMayuscula = Regex.IsMatch(contrasena, @"[A-Z]");
            bool tieneMinuscula = Regex.IsMatch(contrasena, @"[a-z]");
            bool tieneNumero = Regex.IsMatch(contrasena, @"[0-9]");
            bool tieneEspecial = Regex.IsMatch(contrasena, @"[!@#$%^&*]");

            if (!(tieneMayuscula && tieneMinuscula && tieneNumero && tieneEspecial))
            {
                lblMensaje.Text = "La contraseña debe contener al menos una mayúscula, una minúscula, un número y un símbolo.";
                return;
            }

            // Hashear contraseña con PBKDF2 + salt
            string hashedPassword = HashPassword(contrasena);

            string connStr = System.Configuration.ConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString;

            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                try
                {
                    conn.Open();

                    string checkQuery = "SELECT COUNT(*) FROM usuarios WHERE usuario = @usuario";
                    using (MySqlCommand checkCmd = new MySqlCommand(checkQuery, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@usuario", usuario);
                        int count = Convert.ToInt32(checkCmd.ExecuteScalar());

                        if (count > 0)
                        {
                            lblMensaje.Text = "El usuario ya existe. Intenta con otro.";
                            return;
                        }
                    }

                    string insertQuery = "INSERT INTO usuarios (usuario, contrasena, rol) VALUES (@usuario, @contrasena, @rol)";
                    using (MySqlCommand insertCmd = new MySqlCommand(insertQuery, conn))
                    {
                        insertCmd.Parameters.AddWithValue("@usuario", usuario);
                        insertCmd.Parameters.AddWithValue("@contrasena", hashedPassword);
                        insertCmd.Parameters.AddWithValue("@rol", rol);
                        insertCmd.ExecuteNonQuery();
                    }

                    lblMensaje.ForeColor = System.Drawing.Color.Green;
                    lblMensaje.Text = "Usuario registrado exitosamente.";
                    CargarUsuarios();
                }
                catch (Exception ex)
                {
                    lblMensaje.Text = "Error al registrar: " + ex.Message;
                }
            }
        }

        private string HashPassword(string password)
        {
            byte[] salt = new byte[16];
            using (var rng = new System.Security.Cryptography.RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }

            using (var pbkdf2 = new System.Security.Cryptography.Rfc2898DeriveBytes(password, salt, 10000))
            {
                byte[] hash = pbkdf2.GetBytes(20);
                byte[] hashBytes = new byte[36];
                Array.Copy(salt, 0, hashBytes, 0, 16);
                Array.Copy(hash, 0, hashBytes, 16, 20);

                return Convert.ToBase64String(hashBytes);
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

        private void CargarUsuarios()
        {
            string connStr = System.Configuration.ConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString;

            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                conn.Open();
                string query = "SELECT id, usuario, contrasena, rol FROM usuarios ORDER BY id ASC";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        gvUsuarios.DataSource = reader;
                        gvUsuarios.DataBind();
                    }
                }
            }
        }

        protected void gvUsuarios_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int id = Convert.ToInt32(gvUsuarios.DataKeys[e.RowIndex].Value);

            string connStr = System.Configuration.ConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString;

            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                conn.Open();

                // Verifica si el usuario es "admin", y no lo deja eliminar
                string getUserQuery = "SELECT usuario FROM usuarios WHERE id = @id";
                string usuario = "";

                using (MySqlCommand cmdGet = new MySqlCommand(getUserQuery, conn))
                {
                    cmdGet.Parameters.AddWithValue("@id", id);
                    usuario = cmdGet.ExecuteScalar()?.ToString();
                }

                if (usuario == "admin")
                {
                    lblMensaje.ForeColor = System.Drawing.Color.Red;
                    lblMensaje.Text = "No se puede eliminar al administrador principal.";
                    return;
                }

                // Eliminar usuario
                string deleteQuery = "DELETE FROM usuarios WHERE id = @id";
                using (MySqlCommand cmd = new MySqlCommand(deleteQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }

                lblMensaje.ForeColor = System.Drawing.Color.Green;
                lblMensaje.Text = "Usuario eliminado correctamente.";
            }

            CargarUsuarios();
        }

        protected void gvUsuarios_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "EditUser")
            {
                int id = Convert.ToInt32(e.CommandArgument);
                string connStr = ConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString;

                using (MySqlConnection conn = new MySqlConnection(connStr))
                {
                    conn.Open();
                    string query = "SELECT usuario, rol FROM usuarios WHERE id = @id";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                hfEditarId.Value = id.ToString();
                                txtEditarUsuario.Text = reader["usuario"].ToString();
                                ddlEditarRol.SelectedValue = reader["rol"].ToString();
                                pnlEditarUsuario.Visible = true;
                            }
                        }
                    }
                }
            }
        }

        protected void btnGuardarEdicion_Click(object sender, EventArgs e)
        {
            int id = Convert.ToInt32(hfEditarId.Value);
            string nuevoUsuario = txtEditarUsuario.Text.Trim();
            string nuevaContrasena = txtEditarContrasena.Text.Trim();
            string nuevoRol = ddlEditarRol.SelectedValue;

            string connStr = ConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString;

            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                conn.Open();
                string query;

                if (!string.IsNullOrEmpty(nuevaContrasena))
                {
                    // Solo actualiza contraseña si se proporcionó
                    string hashed = HashPassword(nuevaContrasena);
                    query = "UPDATE usuarios SET usuario = @usuario, contrasena = @contrasena, rol = @rol WHERE id = @id";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@usuario", nuevoUsuario);
                        cmd.Parameters.AddWithValue("@contrasena", hashed);
                        cmd.Parameters.AddWithValue("@rol", nuevoRol);
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery();
                    }
                }
                else
                {
                    // No cambia la contraseña
                    query = "UPDATE usuarios SET usuario = @usuario, rol = @rol WHERE id = @id";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@usuario", nuevoUsuario);
                        cmd.Parameters.AddWithValue("@rol", nuevoRol);
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery();
                    }
                }
            }

            lblMensaje.ForeColor = System.Drawing.Color.Green;
            lblMensaje.Text = "Usuario actualizado correctamente.";
            pnlEditarUsuario.Visible = false;
            CargarUsuarios();
        }


        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Response.Redirect("Login.aspx");
        }

        protected void btnRegresarPanel_Click(object sender, EventArgs e)
        {
            Response.Redirect("AdminPanel.aspx");
        }

    }
}