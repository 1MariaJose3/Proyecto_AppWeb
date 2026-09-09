using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PaginaMaestra
{
    public partial class AdminPanel : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            if (Session["usuario"] == null || Session["rol"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            if (Session["rol"].ToString() != "admin")
            {
                Response.Redirect("Login.aspx");
                return;
            }

        }

        protected void btnEditarCampus_Click(object sender, EventArgs e)
        {

            Response.Redirect("EditarCampus.aspx");
        }

        protected void btnNuevoAdmin_Click(object sender, EventArgs e)
        {

            Response.Redirect("Registro.aspx");
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("Login.aspx");
        }

        protected void btnEditarVistaDinamica_Click(object sender, EventArgs e)
        {
            Response.Redirect("EditorVistaDinamica.aspx");
        }
    }
}