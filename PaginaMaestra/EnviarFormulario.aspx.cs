using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;

namespace PaginaMaestra
{
    public partial class EnviarFormulario : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Solo procesar si es POST
            if (Request.HttpMethod == "POST")
            {
                // Opcional: solo para verificar que recibes POST
                // Response.Write("Método HTTP recibido: POST<br>");

                // 1. Sanitizar entradas
                string nombre = Limpiar(Request.Form["nombre"]);
                string apellidos = Limpiar(Request.Form["apellidos"]);
                string celular = Limpiar(Request.Form["celular"]);
                string email = Limpiar(Request.Form["email"]);
                string categoria = Limpiar(Request.Form["categoria"]);
                string programa = Limpiar(Request.Form["programa"]);
                string campus = Limpiar(Request.Form["campus"]);
                string estado = Limpiar(Request.Form["estado"]);
                string mensaje = Limpiar(Request.Form["mensaje"]);

                // 2. Validar campos requeridos
                if (string.IsNullOrWhiteSpace(nombre) ||
                    string.IsNullOrWhiteSpace(apellidos) ||
                    string.IsNullOrWhiteSpace(celular) ||
                    string.IsNullOrWhiteSpace(email) ||
                    string.IsNullOrWhiteSpace(categoria) ||
                    string.IsNullOrWhiteSpace(programa) ||
                    string.IsNullOrWhiteSpace(campus) ||
                    string.IsNullOrWhiteSpace(estado) ||
                    string.IsNullOrWhiteSpace(mensaje))
                {
                    Response.Write("Todos los campos obligatorios deben ser completados.");
                    Response.End();
                    return;
                }

                // 3. Validar reCAPTCHA
                string captchaResponse = Request.Form["g-recaptcha-response"];
                if (string.IsNullOrWhiteSpace(captchaResponse))
                {
                    Response.Write("Por favor verifica el captcha.");
                    Response.End();
                    return;
                }

                bool captchaValido = ValidarCaptcha(captchaResponse);
                if (!captchaValido)
                {
                    Response.Write("Falló la verificación del captcha.");
                    Response.End();
                    return;
                }

                // 4. Construir contenido del mensaje
                string asunto = "Nueva solicitud de información";
                string contenido = HttpUtility.HtmlDecode(
                    $"Nombre: {nombre} \nApellidos: {apellidos}\nCelular: {celular} \nEmail: {email}\nCategoría: {categoria}\nPrograma: {programa}\nCampus: {campus}\nEstado: {estado}\nMensaje:\n{mensaje}"
                );



                try
                {
                    //// 5. Enviar correo usando cuenta de Outlook 
                    //MailMessage mail = new MailMessage();
                    //mail.From = new MailAddress("marijo.010203@hotmail.com", "Formulario Web");
                    //mail.ReplyToList.Add(new MailAddress(email));
                    //mail.To.Add("marijo.010203@hotmail.com"); 
                    //mail.Subject = asunto;
                    //mail.Body = contenido;

                    //SmtpClient smtp = new SmtpClient("smtp.office365.com", 587);
                    //smtp.Credentials = new NetworkCredential("marijo.010203@hotmail.com", "SailorMoon");
                    //smtp.EnableSsl = true;
                    //smtp.Send(mail);

                    //Response.Write("Gracias por tu mensaje. Te responderemos pronto.");

                    // 5. Enviar correo usando cuenta de Gmail
                    // 1. Leer credenciales del archivo web.config
                    string correoEmisor = ConfigurationManager.AppSettings["CorreoEmisor"];
                    string claveCorreo = ConfigurationManager.AppSettings["ClaveCorreo"];

                    // 2. Crear el mensaje
                    MailMessage mail = new MailMessage();
                    mail.From = new MailAddress(correoEmisor, "Formulario Web");
                    mail.ReplyToList.Add(new MailAddress(email));
                    mail.To.Add(correoEmisor); // Puedes cambiarlo por otro destinatario si lo deseas
                    mail.Subject = asunto;
                    mail.Body = contenido;

                    // 3. Configurar SMTP para Gmail
                    SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
                    smtp.Credentials = new NetworkCredential(correoEmisor, claveCorreo);
                    smtp.EnableSsl = true;

                    smtp.Send(mail);
                    

                }
                catch (Exception ex)
                {
                    Response.Write("Error al enviar el mensaje: " + ex.Message);
                }

                Response.End();
            }
            else
            {
                // Si el método no es POST
                Response.Write("Acceso no permitido.");
                Response.End();
            }
        }


        private string Limpiar(string input)
        {
            return string.IsNullOrWhiteSpace(input) ? "" : Server.HtmlEncode(input.Trim());
        }

        private bool ValidarCaptcha(string captchaResponse)
        {
            string secretKey = "6LfIhpcrAAAAAJ9RcV4gPqpfihPwGDcQ2VVrtDtz"; // <- tu clave secreta

            using (var client = new WebClient())
            {
                var googleReply = client.DownloadString($"https://www.google.com/recaptcha/api/siteverify?secret={secretKey}&response={captchaResponse}");

                var js = new JavaScriptSerializer();
                dynamic json = js.Deserialize<dynamic>(googleReply);

                return json["success"];
            }
        }
    }

}
