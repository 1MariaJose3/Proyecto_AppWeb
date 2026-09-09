using Newtonsoft.Json;
using PaginaMaestra.Clases;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;

namespace PaginaMaestra
{
    public partial class EditarCampus : System.Web.UI.Page
    {
        private string RutaJson => Server.MapPath("~/data/datos.json");
        private static CampusData datos;
        private static string campusActual;
        private void CargarJson()
        {
            datos = JsonConvert.DeserializeObject<CampusData>(File.ReadAllText(RutaJson));
        }

        protected void btnRegresarPanel_Click(object sender, EventArgs e)
        {
            string rol = Session["rol"] as string;

            if (rol == "admin")
            {
                Response.Redirect("AdminPanel.aspx");
            }
            else if (rol == "usuario")
            {
                Response.Redirect("UsuarioPanel.aspx");
            }
            else
            {
                Response.Redirect("Login.aspx");
            }
        }

        protected void btnIrVista_Click(object sender, EventArgs e)
        {
            Response.Redirect("EditorVistaDinamica.aspx");
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["usuario"] == null)
            {
                Response.Redirect("Login.aspx");
            }

            if (!IsPostBack)
            {
                campusActual = ListaCampus.SelectedValue;
                CargarJson();
                CargarTitulo(campusActual);
                CargarBarraFlotante(campusActual);
                CargarLogos(campusActual);
                MostrarHorario();
                LlenarMenus();
                CargarRedesSociales(campusActual);
                CargarVideo(campusActual);
                CargarCampusInfo(campusActual);
                CargarBotonesComunidad(campusActual);
                CargarOfertaAcademica(campusActual);
                CargarBotonesCirculares();
                LlenarDropdownCampusOfertas();
                CargarSiguenos(campusActual);
                CargarFooter(campusActual);

            }
        }
        protected void Seleccionar_Campus(object sender, EventArgs e)
        {
            campusActual = ListaCampus.SelectedValue;
            CargarTitulo(campusActual);
            CargarBarraFlotante(campusActual);
            CargarLogos(campusActual);
            MostrarHorario();
            LlenarMenus();
            LimpiarPaneles();
            CargarRedesSociales(campusActual);
            CargarVideo(campusActual);
            CargarCampusInfo(campusActual);
            CargarBotonesComunidad(campusActual);
            CargarOfertaAcademica(campusActual);
            CargarBotonesCirculares();
            LlenarDropdownCampusOfertas();
            CargarSiguenos(campusActual);
            CargarFooter(campusActual);

        }

        // Llena el dropdown con los keys del diccionario campusOfertas
        private void LlenarDropdownCampusOfertas()
        {
            if (datos == null || !datos.campus.ContainsKey(campusActual)) return;

            var campus = datos.campus[campusActual];

            if (campus.campusOfertas == null) return;

            ddlCampusOfertas.Items.Clear();

            foreach (var key in campus.campusOfertas.Keys)
            {
                ddlCampusOfertas.Items.Add(new ListItem(key, key));
            }

            if (ddlCampusOfertas.Items.Count > 0)
            {
                ddlCampusOfertas.SelectedIndex = 0;
                CargarDatosCampusOferta(ddlCampusOfertas.SelectedValue);
            }
        }

        protected void ddlCampusOfertas_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlCampusOfertas.SelectedIndex >= 0)
            {
                CargarDatosCampusOferta(ddlCampusOfertas.SelectedValue);
            }
        }

        private void CargarDatosCampusOferta(string campusOferta)
        {
            if (datos == null || !datos.campus.ContainsKey(campusActual)) return;

            var campus = datos.campus[campusActual];

            if (campus.campusOfertas == null || !campus.campusOfertas.ContainsKey(campusOferta)) return;

            var oferta = campus.campusOfertas[campusOferta];

            // Obtiene la URL base del frontend desde web.config
            string frontendUrl = ConfigurationManager.AppSettings["FrontendUrl"];

            // 💡 Corrección: Usa la URL pública completa para que el navegador la encuentre
            imgCampusOferta.ImageUrl = $"{frontendUrl}/" + oferta.imagen;
            txtEnlaceCampusOferta.Text = oferta.enlace;
            txtNombreImagenCampusOferta.Text = "";
            lblResultadoImagenCampusOferta.Text = "";
        }

        // Método para guardar imagen en /Frontend/fotos/{campusActual}/ y devolver ruta relativa para JSON
        private string GuardarImagess(FileUpload fileUpload, TextBox txtNombre, Label lblResultado, string subcarpeta)
        {
            try
            {
                string nombreArchivo = Path.GetFileName(fileUpload.FileName);
                string extension = Path.GetExtension(nombreArchivo);
                string nombreNuevo = txtNombre.Text.Trim();

                if (string.IsNullOrEmpty(nombreNuevo))
                    nombreNuevo = Path.GetFileNameWithoutExtension(nombreArchivo);

                string nombreFinal = nombreNuevo + extension;

                // Obtener la ruta física del frontend desde web.config
                string frontendPath = ConfigurationManager.AppSettings["FrontendPhysicalPath2"];

                // 💡 Corrección: Usar la ruta física combinada con la subcarpeta
                string rutaFisica = Path.Combine(frontendPath, "fotos", subcarpeta, nombreFinal);

                // Crear carpeta si no existe
                string carpetaFisica = Path.GetDirectoryName(rutaFisica);
                if (!Directory.Exists(carpetaFisica))
                    Directory.CreateDirectory(carpetaFisica);

                fileUpload.SaveAs(rutaFisica);

                lblResultado.ForeColor = System.Drawing.Color.Green;
                lblResultado.Text = "Imagen subida correctamente.";

                // La ruta que se guarda en el JSON debe ser relativa a la URL pública del frontend
                return $"fotos/{subcarpeta}/{nombreFinal}";
            }
            catch (Exception ex)
            {
                lblResultado.ForeColor = System.Drawing.Color.Red;
                lblResultado.Text = "Error al subir imagen: " + ex.Message;
                return null;
            }
        }
        protected void btnGuardarCambios_Click(object sender, EventArgs e)
        {
            if (!datos.campus.ContainsKey(campusActual)) return;
            var campus = datos.campus[campusActual];

            campus.titulo = txtTitulo.Text.Trim();

            // --- REDES FLOTANTES (botonesFlotantes) ---
            if (datos.campus.ContainsKey(campusActual))
            {
                var campusRedes = datos.campus[campusActual];
                if (campusRedes.botonesFlotantes == null)
                    campusRedes.botonesFlotantes = new List<BotonEnlace>();

                var nuevasRedes = new List<BotonEnlace>();

                foreach (RepeaterItem item in rptBarraFlotante.Items)
                {
                    var lblImagen = (Label)item.FindControl("lblImagenBarraFlotante"); // ahora es Label
                    var txtEnlace = (TextBox)item.FindControl("txtenlaceBarraFlotante");

                    if (lblImagen != null && txtEnlace != null)
                    {
                        string imagen = lblImagen.Text.Trim(); // obtenemos texto del Label
                        string enlace = txtEnlace.Text.Trim();

                        if (!string.IsNullOrEmpty(imagen) && !string.IsNullOrEmpty(enlace))
                        {
                            nuevasRedes.Add(new BotonEnlace
                            {
                                imagen = imagen,
                                enlace = enlace
                            });
                        }
                    }
                }

                campusRedes.botonesFlotantes = nuevasRedes;
            }

            // --- HORARIO DE ATENCIÓN ---
            if (campus.horarioAtencion == null)
                campus.horarioAtencion = new HorarioAtencion();

            campus.horarioAtencion.lunesViernes = txtLunesViernes.Text.Trim();
            campus.horarioAtencion.sabado = txtSabado.Text.Trim();
            campus.horarioAtencion.informes = txtInformes.Text.Trim();
            campus.horarioAtencion.diplomados = txtDiplomados.Text.Trim();
            campus.horarioAtencion.soporte = txtSoporte.Text
                .Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim()).ToList();


            var menuSeleccionado = ObtenerMenuSeleccionado();

            if (menuSeleccionado.categorias != null)
            {
                ActualizarCategoriasProgramas(menuSeleccionado);
            }

            if (menuSeleccionado.opciones != null && rptOpciones.Items.Count > 0)
            {
                var nuevasOpciones = new List<OpcionSimple>();
                foreach (RepeaterItem item in rptOpciones.Items)
                {
                    var txtTitulo = (TextBox)item.FindControl("txtTituloOpcion");
                    var txtEnlace = (TextBox)item.FindControl("txtEnlaceOpcion");

                    if (txtTitulo != null && !string.IsNullOrWhiteSpace(txtTitulo.Text))
                    {
                        string titulo = txtTitulo.Text.Trim();
                        string enlace = txtEnlace?.Text.Trim() ?? "#";

                        // Buscar si ya existía una opción con ese título
                        var opcionExistente = menuSeleccionado.opciones
                            .FirstOrDefault(o => o.titulo.Equals(titulo, StringComparison.OrdinalIgnoreCase));

                        nuevasOpciones.Add(new OpcionSimple
                        {
                            titulo = titulo,
                            enlace = enlace,
                            contenido = opcionExistente?.contenido // ← conservar el contenido si ya estaba
                        });
                    }
                }
                menuSeleccionado.opciones = nuevasOpciones;

            }

            // Aseguramos que exista la rama
            if (campus.campusinfo == null)
                campus.campusinfo = new CampusInfo();

            var info = campus.campusinfo;

            // Texto principal
            info.titulo = txtTituloCampusInfo.Text.Trim();

            // Descripciones
            info.descripcion = new List<string>();
            if (!string.IsNullOrWhiteSpace(txtDescripcion1.Text))
                info.descripcion.Add(txtDescripcion1.Text.Trim());

            if (!string.IsNullOrWhiteSpace(txtDescripcion2.Text))
                info.descripcion.Add(txtDescripcion2.Text.Trim());

            // Invitación
            if (info.invitacion == null)
                info.invitacion = new Invitacion();

            info.invitacion.texto = txtInvitacionTexto.Text.Trim();

            // --- BOTONES COMUNIDAD ---
            if (campus.botonesComunidad == null)
                campus.botonesComunidad = new List<BotonEnlace>();

            var nuevosBotonesComunidad = new List<BotonEnlace>();

            // 💡 Corrección: Usa la URL del frontend para comparar, no la ruta física.
            string frontendUrl = ConfigurationManager.AppSettings["FrontendUrl"].TrimEnd('/');

            for (int i = 0; i < rptBotonesComunidad.Items.Count; i++)
            {
                var item = rptBotonesComunidad.Items[i];

                var txtEnlace = (TextBox)item.FindControl("txtEnlaceBotonComunidad");
                var fuImagen = (FileUpload)item.FindControl("fuImagenBotonComunidad");
                var txtNombreImagen = (TextBox)item.FindControl("txtNombreImagenBotonComunidad");
                var lblResultado = (Label)item.FindControl("lblResultadoImgBotonComunidad");
                var imgActual = (Image)item.FindControl("imgBotonComunidad");

                // 💡 Corrección: La ruta que se guarda en el objeto BotonEnlace debe ser la ruta relativa.
                // La URL de la imagen actual en el control es una URL completa.
                // Debemos extraer la parte relativa.
                string rutaImagenFinal = string.Empty;

                // Si el control de imagen tiene una URL, la usamos como punto de partida.
                if (imgActual != null && !string.IsNullOrEmpty(imgActual.ImageUrl))
                {
                    // Extraemos la parte relativa de la URL completa.
                    // Por ejemplo, de "http://localhost/Frontend/fotos/Botones/imagen.jpg"
                    // queremos "fotos/Botones/imagen.jpg".
                    rutaImagenFinal = imgActual.ImageUrl.Replace(frontendUrl, "").TrimStart('/');
                }
                else
                {
                    // Si no hay imagen actual, usamos una por defecto.
                    rutaImagenFinal = "fotos/Botones/placeholder.jpg";
                }


                // Si se sube una nueva imagen, la guardamos y actualizamos la ruta.
                if (fuImagen.HasFile)
                {
                    string nuevaRuta = GuardarImagennn(fuImagen, txtNombreImagen, lblResultado, "Botones");
                    if (!string.IsNullOrEmpty(nuevaRuta))
                    {
                        // El método GuardarImagennn ya devuelve la ruta relativa correcta.
                        rutaImagenFinal = nuevaRuta;
                    }
                }

                // 💡 Corrección: Ahora la rutaImagenFinal contiene la ruta relativa correcta.
                nuevosBotonesComunidad.Add(new BotonEnlace
                {
                    imagen = rutaImagenFinal,
                    enlace = txtEnlace.Text.Trim()
                });
            }

            campus.botonesComunidad = nuevosBotonesComunidad;


            // Guardar cambios en Oferta Académica
            var oferta = campus.ofertaAcademica;
            int index = Convert.ToInt32(ddlOfertaSeleccion.SelectedValue);
            var itemOfertaRepeater = rptOfertaAcademica.Items[0]; // Solo hay un ítem visible

            var txtTituloOferta = (TextBox)itemOfertaRepeater.FindControl("txtTitulo");
            var txtDescripcionOferta = (TextBox)itemOfertaRepeater.FindControl("txtDescripcion");
            var txtEnlaceOferta = (TextBox)itemOfertaRepeater.FindControl("txtEnlace");
            var fuImagenOferta = (FileUpload)itemOfertaRepeater.FindControl("fuImagenOferta");
            var txtNombreImagenOferta = (TextBox)itemOfertaRepeater.FindControl("txtNuevoNombreImagenOferta");
            var lblResultadoOferta = (Label)itemOfertaRepeater.FindControl("lblResultadoImgOferta");

            var itemOferta = oferta.items[index];

            if (txtTituloOferta != null)
                itemOferta.titulo = txtTituloOferta.Text.Trim();
            if (txtDescripcionOferta != null)
                itemOferta.descripcion = txtDescripcionOferta.Text.Trim();
            if (txtEnlaceOferta != null)
                itemOferta.enlace = txtEnlaceOferta.Text.Trim();

            // Subir imagen nueva si se seleccionó una
            if (fuImagenOferta != null && fuImagenOferta.HasFile)
            {
                string nuevaRuta = GuardarImagenn(fuImagenOferta, txtNombreImagenOferta, lblResultadoOferta);
                if (!string.IsNullOrEmpty(nuevaRuta))
                    itemOferta.imagen = nuevaRuta;
            }

            // --- BOTONES CIRCULARES ---
            if (campus.botonesCirculares == null)
                campus.botonesCirculares = new List<BotonEnlace>();

            var nuevosBotonesCirculares = new List<BotonEnlace>();

            for (int i = 0; i < rptBotonesCirculares.Items.Count; i++)
            {
                var item = rptBotonesCirculares.Items[i];

                var txtEnlace = (TextBox)item.FindControl("txtEnlaceBotonCircular");
                var fuImagen = (FileUpload)item.FindControl("fuImagenBotonCircular");
                var txtNombreImagen = (TextBox)item.FindControl("txtNombreImagenBotonCircular");
                var lblResultado = (Label)item.FindControl("lblResultadoImgBotonCircular");
                var imgActual = (Image)item.FindControl("imgBotonCircular");

                // 💡 Declara una variable para la ruta final, que debe ser relativa.
                string rutaImagenFinal = string.Empty;

                // Si se subió una nueva imagen, la guardamos y obtenemos la ruta relativa.
                if (fuImagen.HasFile)
                {
                    string nuevaRuta = GuardarImagess(fuImagen, txtNombreImagen, lblResultado, "Botones");
                    if (!string.IsNullOrEmpty(nuevaRuta))
                        rutaImagenFinal = nuevaRuta;
                }
                // Si no se subió una nueva imagen, conservamos la ruta que ya existía.
                else
                {
                    // 💡 Esta es la parte crucial. Tomamos la URL completa del control
                    // y la limpiamos para obtener solo la parte relativa.
                    string rutaImagenCompleta = imgActual.ImageUrl;

                    if (!string.IsNullOrEmpty(rutaImagenCompleta))
                    {
                        // Reemplaza la URL base y luego cualquier prefijo "Frontend/" que pudiera quedar.
                        rutaImagenFinal = rutaImagenCompleta.Replace(frontendUrl, "").Replace("Frontend/", "").TrimStart('/');
                    }
                }

                nuevosBotonesCirculares.Add(new BotonEnlace
                {
                    imagen = rutaImagenFinal,
                    enlace = txtEnlace.Text.Trim()
                });
            }

            campus.botonesCirculares = nuevosBotonesCirculares;

            // ---- INICIO: Actualizar campusOfertas ----
            if (campus.campusOfertas != null && ddlCampusOfertas.SelectedIndex >= 0)
            {
                string ofertaKey = ddlCampusOfertas.SelectedValue;

                if (campus.campusOfertas.ContainsKey(ofertaKey))
                {
                    var ofertaCampus = campus.campusOfertas[ofertaKey];

                    // Actualizar enlace
                    ofertaCampus.enlace = txtEnlaceCampusOferta.Text.Trim();

                    // Procesar imagen si se subió
                    if (fuImagenCampusOferta.HasFile)
                    {
                        string nuevaRuta = GuardarImagess(fuImagenCampusOferta, txtNombreImagenCampusOferta, lblResultadoImagenCampusOferta, campusActual);
                        if (!string.IsNullOrEmpty(nuevaRuta))
                        {
                            ofertaCampus.imagen = nuevaRuta;
                        }
                    }

                    // Actualizamos el objeto campusOfertas
                    campus.campusOfertas[ofertaKey] = ofertaCampus;
                }
            }
            // ---- FIN: Actualizar campusOfertas ----

            // --- FOOTER ---
            if (campus.footer == null)
                campus.footer = new Footer();

            string nuevaRutaFooter = GuardarImagenn(fuImagenFOOTER, txtImagenFooter, Label3);
            if (!string.IsNullOrEmpty(nuevaRutaFooter))
                campus.footer.imagen = nuevaRutaFooter;

            campus.footer.derechos = txtFooterDerechos.Text.Trim();

            // Información: texto + teléfonos
            campus.footer.informacion = new List<Informacion>
                {
                    new Informacion
                    {
                        texto = txtFooterInfo1.Text.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
                                                   .Select(l => l.Trim()).ToList()
                    },
                    new Informacion
                    {
                        telefonos = txtFooterInfo2.Text.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
                                                       .Select(t => t.Trim()).ToList()
                    }
                };

            // Reemplaza solo el campus actualizado en el objeto completo
            datos.campus[campusActual] = campus;

            File.WriteAllText(
                RutaJson,
                JsonConvert.SerializeObject(
                    datos,
                    Formatting.Indented,
                    new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));

            CargarJson();
            LlenarMenus();
            CargarTitulo(campusActual);
            CargarBarraFlotante(campusActual);
            CargarLogos(campusActual);
            MostrarHorario();
            CargarRedesSociales(campusActual);
            CargarVideo(campusActual);
            CargarCampusInfo(campusActual);
            CargarBotonesComunidad(campusActual);
            CargarOfertaAcademica(campusActual);
            CargarSiguenos(campusActual);
            CargarFooter(campusActual);
        }

        //Eventos para el manejo de los botones de Oferta Academica 
        private void CargarBotonesCirculares()
        {
            if (datos.campus.TryGetValue(campusActual, out var campus))
            {
                rptBotonesCirculares.DataSource = campus.botonesCirculares;
                rptBotonesCirculares.DataBind();
            }
        }

        protected void rptBotonesCirculares_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var boton = (BotonEnlace)e.Item.DataItem;
                var img = (Image)e.Item.FindControl("imgBotonCircular");

                if (boton != null && img != null && !string.IsNullOrEmpty(boton.imagen))
                {
                    // Obtiene la URL base del frontend desde Web.config.
                    string frontendUrl = ConfigurationManager.AppSettings["FrontendUrl"].TrimEnd('/');

                    // Combina la URL base con la ruta relativa de la imagen.
                    string urlImagen = $"{frontendUrl}/{boton.imagen}";

                    // Asigna la URL completa al control de imagen.
                    img.ImageUrl = urlImagen;
                }
            }
        }
        protected void btnAgregarBotonCircular_Click(object sender, EventArgs e)
        {
            if (datos.campus.TryGetValue(campusActual, out var campus))
            {
                campus.botonesCirculares.Add(new BotonEnlace { imagen = "", enlace = "" });
                CargarBotonesCirculares();
            }
        }

        protected void rptBotonesCirculares_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Eliminar")
            {
                int index = Convert.ToInt32(e.CommandArgument);
                if (datos.campus.TryGetValue(campusActual, out var campus))
                {
                    if (index >= 0 && index < campus.botonesCirculares.Count)
                    {
                        campus.botonesCirculares.RemoveAt(index);
                        CargarBotonesCirculares();
                    }
                }
            }
        }

        private void GuardarBotonesCirculares()
        {
            if (datos.campus.TryGetValue(campusActual, out var campus))
            {
                // 💡 Creamos una nueva lista para guardar los botones actualizados.
                var nuevosBotonesCirculares = new List<BotonEnlace>();

                // Obtén la URL base del frontend, es clave para limpiar la URL.
                string frontendUrl = ConfigurationManager.AppSettings["FrontendUrl"].TrimEnd('/');

                for (int i = 0; i < rptBotonesCirculares.Items.Count; i++)
                {
                    var item = rptBotonesCirculares.Items[i];

                    var fuImagen = (FileUpload)item.FindControl("fuImagenBotonCircular");
                    var txtNombreImagen = (TextBox)item.FindControl("txtNombreImagenBotonCircular");
                    var txtEnlace = (TextBox)item.FindControl("txtEnlaceBotonCircular");
                    var lblResultado = (Label)item.FindControl("lblResultadoImgBotonCircular");
                    var imgActual = (Image)item.FindControl("imgBotonCircular");

                    // 💡 Variable para almacenar la ruta relativa de la imagen.
                    string rutaImagenFinal = string.Empty;

                    // Lógica para determinar la ruta de la imagen
                    if (fuImagen.HasFile)
                    {
                        // Si hay un archivo nuevo, lo guardamos y usamos la ruta relativa que nos devuelve el método.
                        rutaImagenFinal = GuardarImagess(fuImagen, txtNombreImagen, lblResultado, "Botones");
                    }
                    else
                    {
                        // Si no hay un archivo nuevo, tomamos la URL completa del control
                        // de imagen y la limpiamos para obtener la ruta relativa.
                        string rutaImagenCompleta = imgActual.ImageUrl;
                        if (!string.IsNullOrEmpty(rutaImagenCompleta))
                        {
                            rutaImagenFinal = rutaImagenCompleta.Replace(frontendUrl, "").TrimStart('/');
                            // A veces la ruta puede tener "Frontend/" si no se configuró bien en el ItemDataBound.
                            // Con este reemplazo, nos aseguramos de que no se guarde.
                            if (rutaImagenFinal.StartsWith("Frontend/"))
                            {
                                rutaImagenFinal = rutaImagenFinal.Substring("Frontend/".Length);
                            }
                        }
                    }

                    // Actualizamos el enlace del botón.
                    string enlaceFinal = txtEnlace.Text.Trim();

                    // 💡 Agregamos un nuevo objeto a la lista temporal con los datos correctos.
                    nuevosBotonesCirculares.Add(new BotonEnlace
                    {
                        imagen = rutaImagenFinal,
                        enlace = enlaceFinal
                    });
                }

                // 💡 Al final del bucle, reemplazamos la lista de botones del campus con la nueva lista.
                campus.botonesCirculares = nuevosBotonesCirculares;
            }
        }


        //Eventos para el manejo de los botones de Comunidad 
        private void CargarBotonesComunidad(string campus)
        {
            if (!datos.campus.ContainsKey(campus)) return;

            var campusObj = datos.campus[campus];

            if (campusObj.botonesComunidad == null)
                campusObj.botonesComunidad = new List<BotonEnlace>();

            rptBotonesComunidad.DataSource = campusObj.botonesComunidad;
            rptBotonesComunidad.DataBind();
        }

        protected void rptBotonesComunidad_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var boton = (BotonEnlace)e.Item.DataItem;
                var img = (Image)e.Item.FindControl("imgBotonComunidad");

                if (boton != null && img != null && !string.IsNullOrEmpty(boton.imagen))
                {
                    string frontendUrl = ConfigurationManager.AppSettings["FrontendUrl"].TrimEnd('/');
                    string frontendPath = ConfigurationManager.AppSettings["FrontendPhysicalPath2"].TrimEnd('\\', '/');

                    // Limpiar la ruta para que no se repita "Frontend/"
                    string relativePath = boton.imagen.Replace("\\", "/").TrimStart('~', '/');
                    if (relativePath.StartsWith("Frontend/", StringComparison.OrdinalIgnoreCase))
                        relativePath = relativePath.Substring("Frontend/".Length);

                    string urlImagen = $"{frontendUrl}/{relativePath}";
                    string rutaFisica = Path.Combine(frontendPath, relativePath.Replace("/", "\\"));

                    if (File.Exists(rutaFisica))
                    {
                        img.ImageUrl = urlImagen;
                    }
                    else
                    {
                        img.ImageUrl = "https://via.placeholder.com/100";
                        System.Diagnostics.Debug.WriteLine($"[BotonesComunidad] Imagen no encontrada en: {rutaFisica}");
                    }
                }
            }
        }

        private string GuardarImagennn(FileUpload fileUpload, TextBox txtNombre, Label lblResultado, string subcarpeta = "")
        {
            try
            {
                string nombreArchivo = Path.GetFileName(fileUpload.FileName);
                string extension = Path.GetExtension(nombreArchivo);
                string nombreNuevo = txtNombre.Text.Trim();

                if (string.IsNullOrEmpty(nombreNuevo))
                    nombreNuevo = Path.GetFileNameWithoutExtension(nombreArchivo);

                string nombreFinal = nombreNuevo + extension;

                // 💡 Corrección: Obtener la ruta física del frontend desde web.config
                string frontendPath = ConfigurationManager.AppSettings["FrontendPhysicalPath2"];

                // 💡 Corrección: Construir la ruta física completa usando Path.Combine
                string rutaFisica = Path.Combine(frontendPath, "fotos", subcarpeta, nombreFinal);

                // Crear carpeta si no existe
                string carpetaFisica = Path.GetDirectoryName(rutaFisica);
                if (!Directory.Exists(carpetaFisica))
                    Directory.CreateDirectory(carpetaFisica);

                fileUpload.SaveAs(rutaFisica);

                lblResultado.ForeColor = System.Drawing.Color.Green;
                lblResultado.Text = "Imagen subida correctamente.";

                // La ruta que se guarda en el JSON debe ser relativa a la carpeta del frontend
                return $"fotos/{subcarpeta}/{nombreFinal}";
            }
            catch (Exception ex)
            {
                lblResultado.ForeColor = System.Drawing.Color.Red;
                lblResultado.Text = "Error al subir imagen: " + ex.Message;
                return null;
            }
        }
        protected void btnAgregarBotonComunidad_Click(object sender, EventArgs e)
        {
            if (!datos.campus.ContainsKey(campusActual)) return;

            var campus = datos.campus[campusActual];

            if (campus.botonesComunidad == null)
                campus.botonesComunidad = new List<BotonEnlace>();

            // Agregar nuevo botón en blanco (con valores iniciales)
            campus.botonesComunidad.Add(new BotonEnlace
            {
                imagen = "fotos/Botones/placeholder.jpg", // Puedes usar una imagen genérica o vacía
                enlace = ""
            });

            CargarBotonesComunidad(campusActual);
        }
        protected void rptBotonesComunidad_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Eliminar")
            {
                int index = Convert.ToInt32(e.CommandArgument);

                if (datos.campus.ContainsKey(campusActual))
                {
                    var botones = datos.campus[campusActual].botonesComunidad;
                    if (botones != null && index >= 0 && index < botones.Count)
                    {
                        botones.RemoveAt(index);
                        CargarBotonesComunidad(campusActual);
                    }
                }
            }
        }


        //Eventos para el manejo de la barra flotante 
        private void CargarBarraFlotante(string campus)
        {
            if (!datos.campus.ContainsKey(campus)) return;

            var campusObj = datos.campus[campus];
            txtTitulo.Text = campusObj.titulo;

            if (campusObj.botonesFlotantes == null)
                campusObj.botonesFlotantes = new List<BotonEnlace>();

            rptBarraFlotante.DataSource = campusObj.botonesFlotantes;
            rptBarraFlotante.DataBind();
        }
        protected void rptBarraFlotante_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Eliminar")
            {
                int index = Convert.ToInt32(e.CommandArgument);

                if (!datos.campus.ContainsKey(campusActual)) return;
                var campusObj = datos.campus[campusActual];

                if (campusObj.botonesFlotantes != null && index >= 0 && index < campusObj.botonesFlotantes.Count)
                {
                    campusObj.botonesFlotantes.RemoveAt(index);
                    CargarBarraFlotante(campusActual);
                }
            }
        }


        //Eventos para el manejo de las opciones del menu
        private MenuItemm ObtenerMenuSeleccionado()
        {
            return datos.campus[campusActual].menu
                .FirstOrDefault(m => m.titulo.Equals(ddlMenu.SelectedValue, StringComparison.OrdinalIgnoreCase));
        }
        private void LlenarMenus()
        {
            ddlMenu.Items.Clear();
            foreach (var item in datos.campus[campusActual].menu)
                ddlMenu.Items.Add(new ListItem(item.titulo, item.titulo));
        }
        protected void Seleccionar_Menu(object sender, EventArgs e)
        {
            LimpiarPaneles();

            var menu = ObtenerMenuSeleccionado();
            if (menu == null) return;

            if (menu.opciones != null)
            {
                pnlOpciones.Visible = true;
                rptOpciones.DataSource = menu.opciones;
                rptOpciones.DataBind();

                // Llenar dropdown ddlSeleccionVista con títulos de opciones con contenido
                ddlSeleccionVista.Items.Clear();

                var campus = datos.campus[campusActual];
                var vistasDisponibles = campus.menu
                    .Where(m => m.opciones != null)
                    .SelectMany(m => m.opciones)
                    .Where(o => o.contenido != null)
                    .Select(o => o.titulo)
                    .Distinct()
                    .ToList();

                foreach (var vista in vistasDisponibles)
                {
                    ddlSeleccionVista.Items.Add(new ListItem(vista, vista));
                }

                if (ddlSeleccionVista.Items.Count == 0)
                {
                    ddlSeleccionVista.Items.Add(new ListItem("No hay vistas disponibles", ""));
                    ddlSeleccionVista.Enabled = false;
                }
                else
                {
                    ddlSeleccionVista.Enabled = true;
                }
            }
            else if (menu.categorias != null)
            {
                pnlCategorias.Visible = true;
                rptCategorias.DataSource = menu.categorias;
                rptCategorias.DataBind();

                // Agregamos aquí el llenado de dropdowns de modelos
                var campus = datos.campus[campusActual];
                var programasExistentes = campus.menu
                    .Where(m => m.categorias != null)
                    .SelectMany(m => m.categorias)
                    .SelectMany(c => c.programas ?? new List<Programa>())
                    .GroupBy(p => p.nombre) // alternativa a DistinctBy
                    .Select(g => g.First())
                    .ToList();

                for (int i = 0; i < rptCategorias.Items.Count; i++)
                {
                    var item = rptCategorias.Items[i];
                    var ddlModelos = (DropDownList)item.FindControl("ddlModeloPrograma");

                    if (ddlModelos != null)
                    {
                        ddlModelos.Items.Clear();
                        ddlModelos.Items.Add(new ListItem("Sin modelo", ""));

                        foreach (var prog in programasExistentes)
                        {
                            ddlModelos.Items.Add(new ListItem(prog.nombre, prog.nombre));
                        }
                    }
                }
            }
            else if (!string.IsNullOrEmpty(menu.href))
            {
                pnlHref.Visible = true;
                txtHref.Text = menu.href;
            }
        }
        protected void btnAgregarNuevoMenu_Click(object sender, EventArgs e)
        {
            string tipo = ddlTipoMenu.SelectedValue;
            string titulo = txtNuevoTituloMenu.Text.Trim();
            if (string.IsNullOrWhiteSpace(titulo)) return;

            var nuevoMenu = new MenuItemm { titulo = titulo };

            if (tipo == "opciones") nuevoMenu.opciones = new List<OpcionSimple>();
            else if (tipo == "categorias") nuevoMenu.categorias = new List<Categoria>();
            else if (tipo == "href") nuevoMenu.href = "#";

            if (!datos.campus.ContainsKey(campusActual)) return;

            datos.campus[campusActual].menu.Add(nuevoMenu);
            LlenarMenus();
            txtNuevoTituloMenu.Text = "";
        }
        protected void btnEliminarMenu_Click(object sender, EventArgs e)
        {
            var menus = datos.campus[campusActual].menu;
            var menuEliminar = menus.FirstOrDefault(m => m.titulo.Equals(ddlMenu.SelectedValue, StringComparison.OrdinalIgnoreCase));
            if (menuEliminar != null)
            {
                menus.Remove(menuEliminar);
                LlenarMenus();
                LimpiarPaneles();
            }
        }
        private void LimpiarPaneles()
        {
            pnlOpciones.Visible = false;
            pnlCategorias.Visible = false;
            pnlHref.Visible = false;
        }
        protected void btnAgregarOpcion_Click(object sender, EventArgs e)
        {
            var menu = ObtenerMenuSeleccionado();
            if (menu?.opciones == null || string.IsNullOrWhiteSpace(txtNuevaOpcion.Text)) return;

            string vistaSeleccionada = ddlSeleccionVista.SelectedValue;
            ContenidoOpcionSimple nuevoContenido = null;

            if (!string.IsNullOrEmpty(vistaSeleccionada))
            {
                var campus = datos.campus[campusActual];
                var opcionReferencia = campus.menu
                    .Where(m => m.opciones != null)
                    .SelectMany(m => m.opciones)
                    .FirstOrDefault(o => o.titulo == vistaSeleccionada);

                if (opcionReferencia?.contenido != null)
                {
                    // Clonar contenido para evitar referencias
                    nuevoContenido = new ContenidoOpcionSimple
                    {
                        imagenBanner = opcionReferencia.contenido.imagenBanner,
                        mensajeRector = opcionReferencia.contenido.mensajeRector != null ? new Mensaje
                        {
                            titulo = opcionReferencia.contenido.mensajeRector.titulo,
                            parrafos = opcionReferencia.contenido.mensajeRector.parrafos != null ? new List<string>(opcionReferencia.contenido.mensajeRector.parrafos) : null,
                            lista = opcionReferencia.contenido.mensajeRector.lista != null ? new List<string>(opcionReferencia.contenido.mensajeRector.lista) : null,
                            parrafosFinales = opcionReferencia.contenido.mensajeRector.parrafosFinales != null ? new List<string>(opcionReferencia.contenido.mensajeRector.parrafosFinales) : null,
                            secciones = opcionReferencia.contenido.mensajeRector.secciones != null ? new List<Seccion>(opcionReferencia.contenido.mensajeRector.secciones) : null
                        } : null
                    };
                }
            }

            menu.opciones.Add(new OpcionSimple
            {
                titulo = txtNuevaOpcion.Text.Trim(),
                enlace = txtNuevoEnlace.Text.Trim(),
                contenido = nuevoContenido
            });

            txtNuevaOpcion.Text = "";
            txtNuevoEnlace.Text = "";

            rptOpciones.DataSource = menu.opciones;
            rptOpciones.DataBind();
        }
        protected void btnEliminarOpcion_Command(object sender, CommandEventArgs e)
        {
            var menu = ObtenerMenuSeleccionado();
            if (menu?.opciones == null) return;

            if (int.TryParse(e.CommandArgument.ToString(), out int index))
            {
                if (index >= 0 && index < menu.opciones.Count)
                {
                    menu.opciones.RemoveAt(index);
                    rptOpciones.DataSource = menu.opciones;
                    rptOpciones.DataBind();
                }
            }
        }
        protected void btnAgregarCategoria_Click(object sender, EventArgs e)
        {
            var menu = ObtenerMenuSeleccionado();
            menu.categorias.Add(new Categoria
            {
                nombre = txtNuevaCategoria.Text.Trim(),
                programas = new List<Programa>() // no List<string>()
            });
        }
        protected void rptCategorias_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            var menu = ObtenerMenuSeleccionado();
            int catIndex = e.Item.ItemIndex;

            if (menu?.categorias == null || catIndex >= menu.categorias.Count) return;

            var categoria = menu.categorias[catIndex];

            if (e.CommandName == "eliminarcategoria")
            {
                menu.categorias.RemoveAt(catIndex);
            }
            else if (e.CommandName == "agregarprograma")
            {
                var txtNuevoPrograma = (TextBox)e.Item.FindControl("txtNuevoPrograma");
                var ddlModelo = (DropDownList)e.Item.FindControl("ddlModeloPrograma");

                if (!string.IsNullOrWhiteSpace(txtNuevoPrograma?.Text))
                {
                    if (categoria.programas == null)
                        categoria.programas = new List<Programa>();

                    Programa modelo = null;
                    string nombreModelo = ddlModelo?.SelectedValue;

                    if (!string.IsNullOrEmpty(nombreModelo))
                    {
                        var campus = datos.campus[campusActual];
                        modelo = campus.menu
                            .Where(m => m.categorias != null)
                            .SelectMany(m => m.categorias)
                            .SelectMany(c => c.programas ?? new List<Programa>())
                            .FirstOrDefault(p => p.nombre == nombreModelo);
                    }

                    var nuevoPrograma = new Programa
                    {
                        nombre = txtNuevoPrograma.Text.Trim(),
                        enlace = modelo?.enlace ?? "#",
                        imagen = modelo?.imagen,
                        tipo = modelo?.tipo,
                        contenido = ClonarContenidoPrograma(modelo?.contenido)
                    };

                    categoria.programas.Add(nuevoPrograma);
                }
            }

            // Volvemos a enlazar para reflejar cambios
            rptCategorias.DataSource = menu.categorias;
            rptCategorias.DataBind();
        }
        protected void rptProgramas_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "eliminarprograma")
            {
                // Obtener el repeater hijo que disparó el evento
                Repeater rptProgramas = (Repeater)source;

                // Obtener el RepeaterItem padre del repeater categorías para saber la categoría actual
                RepeaterItem categoriaItem = (RepeaterItem)rptProgramas.NamingContainer;

                int catIndex = categoriaItem.ItemIndex;

                var menu = ObtenerMenuSeleccionado();

                if (menu?.categorias == null || catIndex >= menu.categorias.Count) return;

                var categoria = menu.categorias[catIndex];

                if (int.TryParse(e.CommandArgument.ToString(), out int progIndex))
                {
                    if (categoria.programas != null && progIndex >= 0 && progIndex < categoria.programas.Count)
                    {
                        categoria.programas.RemoveAt(progIndex);
                    }
                }

                // Rebind repeater para reflejar cambios
                rptCategorias.DataSource = menu.categorias;
                rptCategorias.DataBind();
            }
        }
        private ContenidoPrograma ClonarContenidoPrograma(ContenidoPrograma original)
        {
            if (original == null)
                return null;

            var clon = new ContenidoPrograma
            {
                imagenBanner = original.imagenBanner,
                mensajeRector = original.mensajeRector == null ? null : new InfoPrograma
                {
                    titulo = original.mensajeRector.titulo,
                    subtitulo = original.mensajeRector.subtitulo,
                    inicio = original.mensajeRector.inicio,
                    duracion = original.mensajeRector.duracion,
                    modalidad = original.mensajeRector.modalidad,
                    horarios = original.mensajeRector.horarios,
                    dirigidoA = original.mensajeRector.dirigidoA,
                    universidad = original.mensajeRector.universidad == null ? null : new Universidad
                    {
                        nombre = original.mensajeRector.universidad.nombre,
                        descripcion = original.mensajeRector.universidad.descripcion
                    },
                    objetivoGeneral = original.mensajeRector.objetivoGeneral == null ? null : new ObjetivoGeneral
                    {
                        subtitulo = original.mensajeRector.objetivoGeneral.subtitulo,
                        descripcion = original.mensajeRector.objetivoGeneral.descripcion,
                    },
                    objetivoEspecifico = original.mensajeRector.objetivoEspecifico == null ? null : new ObjetivoEspecifico
                    {
                        subtitulo = original.mensajeRector.objetivoEspecifico.subtitulo,
                        lista = original.mensajeRector.objetivoEspecifico.lista == null ? null : new List<string>(original.mensajeRector.objetivoEspecifico.lista)
                    },
                    valorCurricular = original.mensajeRector.valorCurricular == null ? null : new ValorCurricular
                    {
                        subtitulo = original.mensajeRector.valorCurricular.subtitulo,
                        descripcion = original.mensajeRector.valorCurricular.descripcion,
                        subtitulo2 = original.mensajeRector.valorCurricular.subtitulo2,
                        empresa = original.mensajeRector.valorCurricular.empresa
                    },
                    planEstudios = original.mensajeRector.planEstudios == null ? null : new PlanEstudios
                    {
                        subtitulo = original.mensajeRector.planEstudios.subtitulo,
                        diploma = original.mensajeRector.planEstudios.diploma
                    },
                    perfilEgreso = original.mensajeRector.perfilEgreso == null ? null : new PerfilEgreso
                    {
                        subtitulo = original.mensajeRector.perfilEgreso.subtitulo,
                        descripcion = original.mensajeRector.perfilEgreso.descripcion
                    },
                    competenciasProfesionales = original.mensajeRector.competenciasProfesionales == null ? null : new CompetenciasProfesionales
                    {
                        tituloSeccion = original.mensajeRector.competenciasProfesionales.tituloSeccion,
                        conocimientos = original.mensajeRector.competenciasProfesionales.conocimientos == null ? null : new List<Conocimiento>(original.mensajeRector.competenciasProfesionales.conocimientos),
                        habilidades = original.mensajeRector.competenciasProfesionales.habilidades == null ? null : new List<Conocimiento>(original.mensajeRector.competenciasProfesionales.habilidades),
                        actitudes = original.mensajeRector.competenciasProfesionales.actitudes == null ? null : new List<Conocimiento>(original.mensajeRector.competenciasProfesionales.actitudes),
                        valores = original.mensajeRector.competenciasProfesionales.valores == null ? null : new List<Conocimiento>(original.mensajeRector.competenciasProfesionales.valores)
                    },
                    titulacion = original.mensajeRector.titulacion == null ? null : new Titulacion
                    {
                        titulo = original.mensajeRector.titulacion.titulo,
                        descripcion = original.mensajeRector.titulacion.descripcion,
                        subtituloOpciones = original.mensajeRector.titulacion.subtituloOpciones,
                        opciones = original.mensajeRector.titulacion.opciones == null ? null : new List<string>(original.mensajeRector.titulacion.opciones),
                        subtituloEntrega = original.mensajeRector.titulacion.subtituloEntrega,
                        alFinalizar = original.mensajeRector.titulacion.alFinalizar == null ? null : new List<string>(original.mensajeRector.titulacion.alFinalizar)
                    },
                    programaAcademico = original.mensajeRector.programaAcademico == null ? null : new ProgramaAcademico
                    {
                        tituloSeccion = original.mensajeRector.programaAcademico.tituloSeccion,
                        semestres = original.mensajeRector.programaAcademico.semestres != null
                            ? original.mensajeRector.programaAcademico.semestres
                                .Select(sem => sem != null ? new List<string>(sem) : new List<string>())
                                .ToList()
                            : new List<List<string>>(),

                        modulos = original.mensajeRector.programaAcademico.modulos != null
                            ? original.mensajeRector.programaAcademico.modulos
                                .Select(mods => mods != null ? ClonarListaModulos(mods) : new List<Modulo>())
                                .ToList()
                            : new List<List<Modulo>>()
                    },
                    rvoe = original.mensajeRector.rvoe == null ? null : new Rvoe
                    {
                        titulo = original.mensajeRector.rvoe.titulo,
                        clave = original.mensajeRector.rvoe.clave
                    }
                }
            };

            return clon;
        }
        private List<Modulo> ClonarListaModulos(List<Modulo> modulos)
        {
            if (modulos == null) return new List<Modulo>();
            return modulos.Select(m => new Modulo
            {
                titulo = m.titulo,
                texto = m.texto,
                sublista = m.sublista != null ? new List<string>(m.sublista) : null,
                nota = m.nota
            }).ToList();
        }
        private void ActualizarCategoriasProgramas(MenuItemm menu)
        {
            if (menu?.categorias == null) return;

            var campus = datos.campus[campusActual]; // para buscar los programas originales
            var programasOriginales = campus.menu
                .Where(m => m.categorias != null)
                .SelectMany(m => m.categorias)
                .SelectMany(c => c.programas ?? new List<Programa>())
                .ToList();

            for (int i = 0; i < rptCategorias.Items.Count; i++)
            {
                var item = rptCategorias.Items[i];
                var txtCategoria = (TextBox)item.FindControl("txtCategoria");
                var rptProgramas = (Repeater)item.FindControl("rptProgramas");

                if (txtCategoria == null) continue;

                var categoria = menu.categorias[i];
                categoria.nombre = txtCategoria.Text.Trim();

                if (rptProgramas != null)
                {
                    var programasActualizados = new List<Programa>();

                    foreach (RepeaterItem progItem in rptProgramas.Items)
                    {
                        var txtPrograma = (TextBox)progItem.FindControl("txtPrograma");
                        if (txtPrograma != null && !string.IsNullOrWhiteSpace(txtPrograma.Text))
                        {
                            string nombreProgramaNuevo = txtPrograma.Text.Trim();

                            // Buscar el programa original que coincida por nombre (o alguna clave)
                            var programaOriginal = programasOriginales
                                .FirstOrDefault(p => p.nombre == nombreProgramaNuevo);

                            if (programaOriginal != null)
                            {
                                // Clonar el programa original para mantener toda la info
                                programasActualizados.Add(new Programa
                                {
                                    nombre = nombreProgramaNuevo,
                                    enlace = programaOriginal.enlace,
                                    imagen = programaOriginal.imagen,
                                    tipo = programaOriginal.tipo,
                                    contenido = ClonarContenidoPrograma(programaOriginal.contenido) // si tienes este método
                                });
                            }
                            else
                            {
                                // Si no existe en originales, crear uno nuevo con datos básicos
                                programasActualizados.Add(new Programa
                                {
                                    nombre = nombreProgramaNuevo,
                                    enlace = "#",
                                    imagen = null,
                                    tipo = null,
                                    contenido = null
                                });
                            }
                        }
                    }

                    categoria.programas = programasActualizados;
                }
            }
        }

        //Eventos para Horarios
        private void MostrarHorario()
        {
            var horario = datos.campus[campusActual].horarioAtencion;
            if (horario == null) return;

            txtLunesViernes.Text = horario.lunesViernes;
            txtSabado.Text = horario.sabado;
            txtInformes.Text = horario.informes;
            txtDiplomados.Text = horario.diplomados;
            txtSoporte.Text = string.Join("\n", horario.soporte ?? new List<string>());
        }


        //Evento para FOOTER
        private void CargarFooter(string campus)
        {
            if (!datos.campus.ContainsKey(campus)) return;

            var footer = datos.campus[campus].footer;
            if (footer == null) return;

            string frontendUrl = ConfigurationManager.AppSettings["FrontendUrl"].TrimEnd('/');

            // Mostrar imagen con URL absoluta correcta
            if (!string.IsNullOrEmpty(footer.imagen))
                Image2.ImageUrl = $"{frontendUrl}/{footer.imagen.Replace("\\", "/")}";

            // Mostrar derechos
            txtFooterDerechos.Text = footer.derechos ?? "";

            // Mostrar información
            txtFooterInfo1.Text = "";
            txtFooterInfo2.Text = "";

            foreach (var info in footer.informacion)
            {
                if (info.texto != null && info.texto.Count > 0)
                    txtFooterInfo1.Text = string.Join(Environment.NewLine, info.texto);
                else if (info.telefonos != null && info.telefonos.Count > 0)
                    txtFooterInfo2.Text = string.Join(Environment.NewLine, info.telefonos);
            }
        }
        protected void btnSubirImagenFooter_Click(object sender, EventArgs e)
        {
            if (!datos.campus.ContainsKey(campusActual)) return;
            var campus = datos.campus[campusActual];

            if (campus.footer == null)
                campus.footer = new Footer();

            // Asumo que GuardarImagenn ya está corregido para usar FrontendPhysicalPath2
            string nuevaRuta = GuardarImagenn(fuImagenFOOTER, txtImagenFooter, Label3);
            if (!string.IsNullOrEmpty(nuevaRuta))
            {
                campus.footer.imagen = nuevaRuta;
                File.WriteAllText(RutaJson, JsonConvert.SerializeObject(datos, Formatting.Indented));
            }

            CargarJson();
            CargarFooter(campusActual);
        }


        //Evento para el manejo de la seccion SIGUENOS
        private void CargarSiguenos(string campus)
        {
            if (datos.campus.ContainsKey(campus) && !string.IsNullOrEmpty(datos.campus[campus].siguenos))
            {
                string frontendUrl = ConfigurationManager.AppSettings["FrontendUrl"].TrimEnd('/');
                // Mostrar con URL pública
                Image1.ImageUrl = $"{frontendUrl}/{datos.campus[campus].siguenos.Replace("\\", "/")}";
            }
        }
        protected void btnSubirImagenSiguenos_Click(object sender, EventArgs e)
        {
            CargarJson();

            if (!fuImagenSiguenos.HasFile)
            {
                Label2.Text = "No se seleccionó una imagen.";
                Label2.ForeColor = System.Drawing.Color.Red;
                return;
            }

            string ext = Path.GetExtension(fuImagenSiguenos.FileName).ToLower();
            string[] permitidas = { ".jpg", ".jpeg", ".png", ".gif" };
            if (!permitidas.Contains(ext))
            {
                Label2.Text = "Formato no permitido.";
                Label2.ForeColor = System.Drawing.Color.Red;
                return;
            }

            string nombre = txtImagenSiguenos.Text.Trim();
            if (string.IsNullOrWhiteSpace(nombre))
                nombre = Path.GetFileNameWithoutExtension(fuImagenSiguenos.FileName);

            string rutaRelativa = $"fotos/{campusActual}/{nombre}{ext}";

            string frontendPhysicalPath = ConfigurationManager.AppSettings["FrontendPhysicalPath2"];
            string rutaCompleta = Path.Combine(frontendPhysicalPath, rutaRelativa.Replace("/", "\\"));

            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(rutaCompleta));
                fuImagenSiguenos.SaveAs(rutaCompleta);

                if (datos.campus.ContainsKey(campusActual))
                {
                    datos.campus[campusActual].siguenos = rutaRelativa;

                    File.WriteAllText(
                        RutaJson,
                        JsonConvert.SerializeObject(datos, Formatting.Indented,
                            new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));

                    string frontendUrl = ConfigurationManager.AppSettings["FrontendUrl"].TrimEnd('/');
                    Image1.ImageUrl = $"{frontendUrl}/{rutaRelativa.Replace("\\", "/")}";

                    Label2.Text = "Imagen 'Síguenos' actualizada correctamente.";
                    Label2.ForeColor = System.Drawing.Color.Green;
                }
                else
                {
                    Label2.Text = "Campus no encontrado en el JSON.";
                    Label2.ForeColor = System.Drawing.Color.Red;
                }
            }
            catch (Exception ex)
            {
                Label2.Text = "Error al guardar la imagen: " + ex.Message;
                Label2.ForeColor = System.Drawing.Color.Red;
            }
        }


        //Evento para el manejo de la seccion OFERTA ACADEMICA
        private void MostrarItemSeleccionado(string campus)
        {
            int index = Convert.ToInt32(ddlOfertaSeleccion.SelectedValue);
            var oferta = datos.campus[campus].ofertaAcademica;

            var itemSeleccionado = new List<ItemOferta> { oferta.items[index] };
            rptOfertaAcademica.DataSource = itemSeleccionado;
            rptOfertaAcademica.DataBind();

            // Obtener la URL base del frontend desde web.config
            string frontendUrl = ConfigurationManager.AppSettings["FrontendUrl"];

            // Asignar la ruta de imagen con la URL completa
            if (rptOfertaAcademica.Items.Count > 0)
            {
                var img = (Image)rptOfertaAcademica.Items[0].FindControl("imagenOferta");
                if (img != null)
                {
                    // 💡 Corrección: Construir la URL completa para el navegador
                    img.ImageUrl = $"{frontendUrl}/" + oferta.items[index].imagen;
                }
            }
        }
        protected void ddlOfertaSeleccion_SelectedIndexChanged(object sender, EventArgs e)
        {
            MostrarItemSeleccionado(campusActual);
        }
        private void CargarOfertaAcademica(string campus)
        {
            var oferta = datos.campus[campus].ofertaAcademica;

            if (oferta == null || oferta.items == null) return;

            // Llenar DropDownList
            ddlOfertaSeleccion.Items.Clear();
            for (int i = 0; i < oferta.items.Count; i++)
            {
                ddlOfertaSeleccion.Items.Add(new ListItem(oferta.items[i].titulo, i.ToString()));
            }

            // Mostrar el primer ítem por defecto
            if (ddlOfertaSeleccion.Items.Count > 0)
            {
                ddlOfertaSeleccion.SelectedIndex = 0;
                MostrarItemSeleccionado(campus);
            }
        }
        protected void rptOfertaAcademica_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int index = Convert.ToInt32(ddlOfertaSeleccion.SelectedValue);
            var oferta = datos.campus[campusActual].ofertaAcademica;

            if (e.CommandName == "Eliminar")
            {
                oferta.items.RemoveAt(index);
            }
            else if (e.CommandName == "Guardar")
            {
                var txtTitulo = (TextBox)e.Item.FindControl("txtTitulo");
                var txtDescripcion = (TextBox)e.Item.FindControl("txtDescripcion");
                var txtEnlace = (TextBox)e.Item.FindControl("txtEnlace");
                var fuImagen = (FileUpload)e.Item.FindControl("fuImagenOferta");
                var txtNombreImagen = (TextBox)e.Item.FindControl("txtNuevoNombreImagenOferta");
                var lblResultado = (Label)e.Item.FindControl("lblResultadoImgOferta");

                var item = oferta.items[index];
                item.titulo = txtTitulo.Text.Trim();
                item.descripcion = txtDescripcion.Text.Trim();
                item.enlace = txtEnlace.Text.Trim();

                // Subir nueva imagen si hay
                if (fuImagen.HasFile)
                {
                    string nuevaRuta = GuardarImagenn(fuImagen, txtNombreImagen, lblResultado);
                    if (!string.IsNullOrEmpty(nuevaRuta))
                        item.imagen = nuevaRuta;
                }
            }

            // Guardar cambios al JSON
            File.WriteAllText(RutaJson, JsonConvert.SerializeObject(datos, Formatting.Indented));

            CargarJson();
            CargarOfertaAcademica(campusActual);
        }
        private string GuardarImagenn(FileUpload fileUpload, TextBox nombreTxt, Label mensaje)
        {
            if (!fileUpload.HasFile)
            {
                mensaje.Text = "No se seleccionó archivo.";
                mensaje.ForeColor = System.Drawing.Color.Red;
                return "";
            }

            string ext = Path.GetExtension(fileUpload.FileName).ToLower();
            string[] permitidas = { ".jpg", ".jpeg", ".png", ".gif" };
            if (!permitidas.Contains(ext))
            {
                mensaje.Text = "Formato no permitido.";
                mensaje.ForeColor = System.Drawing.Color.Red;
                return "";
            }

            string nombre = nombreTxt.Text.Trim();
            if (string.IsNullOrWhiteSpace(nombre))
                nombre = Path.GetFileNameWithoutExtension(fileUpload.FileName);

            string rutaRelativa = $"fotos/{campusActual}/{nombre}{ext}";

            // 💡 Corrección: Obtener la ruta física del frontend desde web.config
            string frontendPath = ConfigurationManager.AppSettings["FrontendPhysicalPath2"];

            // 💡 Corrección: Construir la ruta física completa usando Path.Combine
            string rutaCompleta = Path.Combine(frontendPath, rutaRelativa);

            fileUpload.SaveAs(rutaCompleta);

            mensaje.Text = "Imagen subida correctamente.";
            mensaje.ForeColor = System.Drawing.Color.Green;

            return rutaRelativa;
        }
        protected void btnAgregarOferta_Click(object sender, EventArgs e)
        {
            var nuevoItem = new ItemOferta
            {
                titulo = txtNuevoTitulo.Text.Trim(),
                descripcion = txtNuevoDescripcion.Text.Trim(),
                enlace = txtNuevoEnlace.Text.Trim(),
                imagen = GuardarImagenn(fuNuevaImagen, txtNombreNuevaImagen, lblResultadoNuevaImagen)
            };

            datos.campus[campusActual].ofertaAcademica.items.Add(nuevoItem);

            File.WriteAllText(RutaJson, JsonConvert.SerializeObject(datos, Formatting.Indented));

            CargarJson();
            CargarOfertaAcademica(campusActual);

            txtNuevoTitulo.Text = "";
            txtNuevoDescripcion.Text = "";
            txtNuevoEnlace.Text = "";
            txtNombreNuevaImagen.Text = "";
        }

        //Evento para el manejo de la seccion CAMPUS
        private void CargarCampusInfo(string campus)
        {
            if (!datos.campus.TryGetValue(campus, out var camp) || camp.campusinfo == null) return;

            var info = camp.campusinfo;

            // Texto
            txtTituloCampusInfo.Text = info.titulo;
            txtDescripcion1.Text = info.descripcion?.ElementAtOrDefault(0) ?? "";
            txtDescripcion2.Text = info.descripcion?.ElementAtOrDefault(1) ?? "";
            txtInvitacionTexto.Text = info.invitacion?.texto ?? "";

            // Imágenes
            // 💡 Corrección: Usar la URL completa del frontend para mostrar la imagen
            string frontendUrl = ConfigurationManager.AppSettings["FrontendUrl"];
            imgPrincipal.ImageUrl = $"{frontendUrl}/{info.imagenPrincipal}";
            imgImagenFrase.ImageUrl = $"{frontendUrl}/{info.imagenFrase}";
        }
        private string GuardarImagen(FileUpload fu, TextBox txtNombre, Image imgCtrl, Label lblEstado)
        {
            // ... (El código de validación inicial está bien, no lo modificaremos)

            // Nombre final
            string ext = Path.GetExtension(fu.FileName).ToLower();
            string nombreFinal = string.IsNullOrWhiteSpace(txtNombre.Text)
                ? Path.GetFileName(fu.FileName)
                : txtNombre.Text.Trim().EndsWith(ext, StringComparison.OrdinalIgnoreCase)
                    ? txtNombre.Text.Trim()
                    : txtNombre.Text.Trim() + ext;

            // Carpeta destino
            string carpetaRel = $"fotos/{campusActual}/";

            // 💡 Corrección: Usar la ruta física del web.config para guardar el archivo
            string frontendPhysicalPath = ConfigurationManager.AppSettings["FrontendPhysicalPath2"];
            string rutaServidor = Path.Combine(frontendPhysicalPath, carpetaRel, nombreFinal);

            Directory.CreateDirectory(Path.GetDirectoryName(rutaServidor));
            fu.SaveAs(rutaServidor);

            // Actualiza UI (aquí también hay que corregir la URL de la imagen)
            string frontendUrl = ConfigurationManager.AppSettings["FrontendUrl"];
            imgCtrl.ImageUrl = $"{frontendUrl}/{carpetaRel}{nombreFinal}";
            lblEstado.ForeColor = System.Drawing.Color.Green;
            lblEstado.Text = "Imagen subida.";

            return carpetaRel + nombreFinal; // ruta que va al JSON
        }
        protected void btnSubirImagenPrincipal_Click(object sender, EventArgs e)
        {
            if (!datos.campus.TryGetValue(campusActual, out var camp)) return;

            string ruta = GuardarImagen(
                fuImagenPrincipal,
                txtNuevoNombreImagenPrincipal,
                imgPrincipal,
                lblResultadoImgPrincipal);

            if (ruta == null) return;

            if (camp.campusinfo == null)
                camp.campusinfo = new CampusInfo();

            camp.campusinfo.imagenPrincipal = ruta;

            File.WriteAllText(RutaJson, JsonConvert.SerializeObject(datos, Formatting.Indented));

            CargarCampusInfo(campusActual); // refresca campos y muestra la imagen nueva

        }
        protected void btnSubirImagenFrase_Click(object sender, EventArgs e)
        {
            if (!datos.campus.TryGetValue(campusActual, out var camp)) return;

            string ruta = GuardarImagen(
                fuImagenFrase,
                txtNuevoNombreImagenFrase,
                imgImagenFrase,
                lblResultadoImgFrase);

            if (ruta == null) return;

            if (camp.campusinfo == null)
                camp.campusinfo = new CampusInfo();
            camp.campusinfo.imagenFrase = ruta;

            File.WriteAllText(RutaJson, JsonConvert.SerializeObject(datos, Formatting.Indented));

            CargarCampusInfo(campusActual); // refresca campos y muestra la imagen nueva

        }

        //Evento para el manejo del video
        private void CargarVideo(string campus)
        {
            // Obtener la URL base del frontend desde web.config
            string frontendUrl = ConfigurationManager.AppSettings["FrontendUrl"];

            if (datos.campus.TryGetValue(campus, out var campusObj) &&
                !string.IsNullOrWhiteSpace(campusObj.video))
            {
                // 💡 Corrección: Usar la URL completa para el navegador
                srcVideo.Attributes["src"] = $"{frontendUrl}{campusObj.video}";
                videoActual.Visible = true;
            }
            else
            {
                videoActual.Visible = false;
            }
        }
        protected void btnSubirVideo_Click(object sender, EventArgs e)
        {
            if (!fileupload1.HasFile)
            {
                lblResultadoVideo.ForeColor = System.Drawing.Color.Red;
                lblResultadoVideo.Text = "Selecciona un archivo para subir.";
                return;
            }

            string extension = Path.GetExtension(fileupload1.FileName).ToLower();
            string[] extensionesPermitidas = { ".mp4", ".webm", ".ogg" };
            if (!extensionesPermitidas.Contains(extension))
            {
                lblResultadoVideo.ForeColor = System.Drawing.Color.Red;
                lblResultadoVideo.Text = "Solo se permiten archivos .mp4, .webm o .ogg.";
                return;
            }

            string nombreFinal = txtnuevonombrevideo.Text.Trim();
            if (string.IsNullOrWhiteSpace(nombreFinal))
                nombreFinal = Path.GetFileName(fileupload1.FileName);
            else if (!nombreFinal.EndsWith(extension, StringComparison.OrdinalIgnoreCase))
                nombreFinal += extension;

            // Usar ruta física base desde web.config (FrontendPhysicalPath2)
            string frontendPhysicalPath = ConfigurationManager.AppSettings["FrontendPhysicalPath2"];

            // Construir ruta física completa para guardar video
            string carpetaVideos = Path.Combine(frontendPhysicalPath, "videos");
            if (!Directory.Exists(carpetaVideos))
                Directory.CreateDirectory(carpetaVideos);

            string rutaServidor = Path.Combine(carpetaVideos, nombreFinal);

            try
            {
                fileupload1.SaveAs(rutaServidor);

                if (datos.campus.TryGetValue(campusActual, out var campus))
                {
                    // Guardar ruta relativa sin barra inicial para frontend y JSON
                    campus.video = $"videos/{nombreFinal}";

                    File.WriteAllText(RutaJson, JsonConvert.SerializeObject(datos, Formatting.Indented));
                }

                lblResultadoVideo.ForeColor = System.Drawing.Color.Green;
                lblResultadoVideo.Text = "Video subido y actualizado correctamente.";

                CargarVideo(campusActual);
            }
            catch (Exception ex)
            {
                lblResultadoVideo.ForeColor = System.Drawing.Color.Red;
                lblResultadoVideo.Text = "Error al subir video: " + ex.Message;
            }
        }


        //Eventos para el manejo de las redes sociales
        protected void rptRedesSociales_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "EliminarRed")
            {
                var campus = datos.campus[campusActual];

                if (int.TryParse(e.CommandArgument.ToString(), out int index))
                {
                    if (index >= 0 && index < campus.redes_sociales.Count)
                    {
                        campus.redes_sociales.RemoveAt(index);
                        File.WriteAllText(RutaJson, JsonConvert.SerializeObject(datos, Formatting.Indented));
                        CargarJson();
                        CargarRedesSociales(campusActual);
                    }
                }
            }
        }
        protected void btnAgregarRed_Click(object sender, EventArgs e)
        {
            string nombre = txtNuevaRedNombre.Text.Trim();
            string url = txtNuevaRedUrl.Text.Trim();

            if (!string.IsNullOrEmpty(nombre) && !string.IsNullOrEmpty(url))
            {
                // Crear nueva red
                RedSocial nuevaRed = new RedSocial
                {
                    nombre = nombre,
                    url = url
                };

                // Agregar a la lista del campus
                var campus = datos.campus[campusActual];
                campus.redes_sociales.Add(nuevaRed);

                // Limpiar campos
                txtNuevaRedNombre.Text = "";
                txtNuevaRedUrl.Text = "";

                // Recargar repeater
                CargarRedesSociales(campusActual);
            }
        }
        private void CargarRedesSociales(string campus)
        {
            if (datos.campus.TryGetValue(campus, out var info))
            {
                rptRedesSociales.DataSource = info.redes_sociales;
                rptRedesSociales.DataBind();
            }
        }

        //Eventos para el manejo de los Logos
        protected void btnSubirImagen_Click(object sender, EventArgs e)
        {
            if (!SubirLogo.HasFile)
            {
                lblresultadoimagen.Text = "Por favor, seleccione un archivo para subir.";
                lblresultadoimagen.ForeColor = System.Drawing.Color.Red;
                return;
            }

            try
            {
                // Obtener ruta física base desde web.config
                string frontendPath = ConfigurationManager.AppSettings["FrontendPhysicalPath2"];

                // Crear carpeta física destino: C:\Sitio Web\Frontend\fotos\{campusActual}
                string carpetaDestino = Path.Combine(frontendPath, "fotos", campusActual);
                if (!Directory.Exists(carpetaDestino))
                    Directory.CreateDirectory(carpetaDestino);

                // Definir nuevo nombre del archivo
                string nuevoNombre = string.IsNullOrWhiteSpace(txtNuevoNombre.Text) ? SubirLogo.FileName : txtNuevoNombre.Text.Trim();
                nuevoNombre = Path.GetFileName(nuevoNombre); // Evitar rutas maliciosas

                // Ruta física completa para guardar el archivo
                string rutaCompleta = Path.Combine(carpetaDestino, nuevoNombre);
                SubirLogo.SaveAs(rutaCompleta);

                // Obtener lista de logos del campus
                var logos = datos.campus[campusActual].logos;

                // Definir ruta relativa para JSON y frontend
                string rutaRelativa = $"fotos/{campusActual}/{nuevoNombre}";

                // Actualizar o agregar la ruta relativa en la lista
                int index = logos.FindIndex(l => l.EndsWith(Logos.SelectedValue, StringComparison.OrdinalIgnoreCase));
                if (index != -1)
                    logos[index] = rutaRelativa;
                else
                    logos.Add(rutaRelativa);

                // Guardar cambios en el archivo JSON
                File.WriteAllText(RutaJson, JsonConvert.SerializeObject(datos, Formatting.Indented, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                }));

                CargarJson();
                CargarLogos(campusActual);

                lblresultadoimagen.Text = "Imagen subida y logo actualizado correctamente.";
                lblresultadoimagen.ForeColor = System.Drawing.Color.Green;
            }
            catch (Exception ex)
            {
                lblresultadoimagen.Text = "Error al subir la imagen: " + ex.Message;
                lblresultadoimagen.ForeColor = System.Drawing.Color.Red;
            }
        }
        private void CargarLogos(string campus)
        {
            Logos.Items.Clear();
            if (datos.campus.TryGetValue(campus, out var info) && info.logos != null)
            {
                foreach (var logo in info.logos)
                    Logos.Items.Add(new ListItem(Path.GetFileName(logo), logo));

                if (Logos.Items.Count > 0)
                {
                    Logos.SelectedIndex = 0;
                    Logos_SelectedIndexChanged(null, null);
                }
                else
                {
                    imgPreview.Visible = false;
                }
            }
        }
        protected void Logos_SelectedIndexChanged(object sender, EventArgs e)
        {
            string rutaRelativa = Logos.SelectedValue;

            if (string.IsNullOrEmpty(rutaRelativa))
            {
                imgPreview.Visible = false;
                return;
            }

            string frontendUrl = ConfigurationManager.AppSettings["FrontendUrl"].TrimEnd('/');

            // Construir URL completa para mostrar imagen
            imgPreview.ImageUrl = $"{frontendUrl}/{rutaRelativa.Replace("\\", "/")}";
            imgPreview.Visible = true;
        }


        //Evento para el manejo del titulo de la pagina
        private void CargarTitulo(string campus)
        {
            if (datos.campus.TryGetValue(campus, out var info))
                txtTitulo.Text = info.titulo ?? "";
        }
        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Response.Redirect("Login.aspx");
        }
    }
}