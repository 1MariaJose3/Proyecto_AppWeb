using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PaginaMaestra
{
    public partial class UsuarioPanel : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            if (Session["usuario"] == null || Session["rol"] == null)
            {
                // Redirigir al login si no hay sesión
                Response.Redirect("Login.aspx");
                return;
            }

            if (Session["rol"].ToString() != "usuario")
            {
                // Redirigir si no tiene el rol correcto
                Response.Redirect("Login.aspx");
                return;
            }
        }

        protected void btnEditarCampus_Click(object sender, EventArgs e)
        {
            Response.Redirect("EditarCampus.aspx");

        }

        protected void btnEditarVistaDinamica_Click(object sender, EventArgs e)
        {
            Response.Redirect("EditorVistaDinamica.aspx");
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("Login.aspx");
        }
    }
}