using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using Newtonsoft.Json;

namespace PaginaMaestra
{
    public partial class GetData : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Clear();
            Response.ContentType = "application/json";

            string jsonContent = "";
            int statusCode = 200;

            try
            {
                // Define la ruta del archivo JSON
                string filePath = Server.MapPath("~/data/datos.json");

                // Verifica si el archivo existe
                if (File.Exists(filePath))
                {
                    jsonContent = File.ReadAllText(filePath);
                }
                else
                {
                    statusCode = 404;
                    jsonContent = "{\"error\": \"Archivo datos.json no encontrado.\"}";
                }
            }
            catch (Exception ex)
            {
                statusCode = 500;
                jsonContent = "{\"error\": \"Hubo un error al procesar el archivo JSON: " + ex.Message + "\"}";
            }

            // Aquí se establece el código de estado y se envía la respuesta una sola vez
            Response.StatusCode = statusCode;
            Response.Write(jsonContent);
            Response.End();
        }
    }
}