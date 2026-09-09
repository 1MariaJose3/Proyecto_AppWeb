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
    public partial class EditorVistaDinamica : System.Web.UI.Page
    {
        //private string RutaJson => Server.MapPath("~/Frontend/data/datos.json");
        private string RutaJson => Server.MapPath("~/data/datos.json");
        private CampusData datos;
        private string campusActual;
        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Response.Redirect("Login.aspx");
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
            Response.Redirect("EditarCampus.aspx"); 
        }

        private void CargarJson()
        {
            string jsonTexto = File.ReadAllText(RutaJson);
            datos = JsonConvert.DeserializeObject<CampusData>(jsonTexto);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            campusActual = ListaCampus.SelectedValue;

            if (Session["usuario"] == null)
            {
                Response.Redirect("Login.aspx");
            }

            if (!IsPostBack)
            {
                CargarJson();
                Session["datos"] = datos;
                LlenarDropdownMenu();
            }
            else
            {
                datos = Session["datos"] as CampusData;
                if (datos == null)
                {
                    CargarJson();
                    Session["datos"] = datos;
                }

                if (Session["menuSeleccionado"] is MenuItemm menu)
                {
                    MostrarEditorParaMenu(menu); // ✅ RECONSTRUYE TODO
                }
            }
        }

        private void MostrarEditorParaMenu(MenuItemm menu)
        {
            // Guardar en sesión para reconstrucción en cada postback
            Session["menuSeleccionado"] = menu;

            // Limpiar controles anteriores
            pnlEditorDinamico.Controls.Clear();

            // Mostrar título del menú
            pnlEditorDinamico.Controls.Add(new Literal { Text = $"<h3>Editar menú: {menu.titulo}</h3>" });

            if (menu.titulo.Equals("PROGRAMAS", StringComparison.OrdinalIgnoreCase) && menu.categorias != null)
            {
                // Obtener categoría seleccionada o usar la primera
                string categoriaSeleccionada = ViewState["categoriaSeleccionada"] as string ?? "0";
                int catIndex = int.TryParse(categoriaSeleccionada, out int cIdx) ? cIdx : 0;

                // Crear dropdown para categorías
                var ddlCategorias = new DropDownList { ID = "ddlCategorias", AutoPostBack = true };
                ddlCategorias.SelectedIndexChanged += ddlCategorias_SelectedIndexChanged;

                // Llenar categorías
                for (int i = 0; i < menu.categorias.Count; i++)
                {
                    var item = new ListItem(menu.categorias[i].nombre, i.ToString());
                    if (i == catIndex) item.Selected = true;
                    ddlCategorias.Items.Add(item);
                }

                pnlEditorDinamico.Controls.Add(new Literal { Text = "Categoría:<br/>" });
                pnlEditorDinamico.Controls.Add(ddlCategorias);
                pnlEditorDinamico.Controls.Add(new Literal { Text = "<br/><br/>" });

                // Validar categoría y mostrar programas
                if (catIndex >= 0 && catIndex < menu.categorias.Count)
                {
                    var categoria = menu.categorias[catIndex];
                    if (categoria.programas == null || categoria.programas.Count == 0)
                    {
                        pnlEditorDinamico.Controls.Add(new Literal { Text = "Esta categoría no tiene programas." });
                        return;
                    }

                    // Obtener programa seleccionado o usar el primero
                    string programaSeleccionado = ViewState["ddlProgramas"] as string ?? "0";
                    int progIndex = int.TryParse(programaSeleccionado, out int pIdx) ? pIdx : 0;

                    // Crear dropdown para programas
                    var ddlProgramas = new DropDownList { ID = "ddlProgramas", AutoPostBack = true };
                    ddlProgramas.SelectedIndexChanged += ddlProgramas_SelectedIndexChanged;

                    // Llenar programas
                    for (int i = 0; i < categoria.programas.Count; i++)
                    {
                        var item = new ListItem(categoria.programas[i].nombre, i.ToString());
                        if (i == progIndex) item.Selected = true;
                        ddlProgramas.Items.Add(item);
                    }

                    pnlEditorDinamico.Controls.Add(new Literal { Text = "Programa:<br/>" });
                    pnlEditorDinamico.Controls.Add(ddlProgramas);
                    pnlEditorDinamico.Controls.Add(new Literal { Text = "<br/><br/>" });

                    // Panel para detalles del programa
                    var pnlDetalles = pnlEditorDinamico.FindControl("pnlDetallesPrograma") as Panel;
                    if (pnlDetalles == null)
                    {
                        pnlDetalles = new Panel { ID = "pnlDetallesPrograma" };
                        pnlEditorDinamico.Controls.Add(pnlDetalles);
                    }
                    else
                    {
                        pnlDetalles.Controls.Clear();
                    }

                    // Mostrar detalles del programa seleccionado
                    if (progIndex >= 0 && progIndex < categoria.programas.Count)
                    {
                        var programa = categoria.programas[progIndex];
                        Session["programaSeleccionado"] = programa;
                        MostrarDetallesPrograma(programa);
                    }
                }
            }
            else if (menu.opciones != null && menu.opciones.Count > 0)
            {
                string opcionSeleccionada = ViewState["ddlOpcionesSimples"] as string ?? "0";
                int opcionIndex = int.TryParse(opcionSeleccionada, out int oIdx) ? oIdx : 0;

                var ddlOpcSimples = new DropDownList
                {
                    ID = "ddlOpcionesSimples",
                    AutoPostBack = true,
                };
                ddlOpcSimples.SelectedIndexChanged += ddlOpcionesSimples_SelectedIndexChanged;

                for (int i = 0; i < menu.opciones.Count; i++)
                {
                    var item = new ListItem(menu.opciones[i].titulo, i.ToString());
                    if (i == opcionIndex) item.Selected = true;
                    ddlOpcSimples.Items.Add(item);
                }

                pnlEditorDinamico.Controls.Add(new Literal { Text = "Opción simple:<br/>" });
                pnlEditorDinamico.Controls.Add(ddlOpcSimples);
                pnlEditorDinamico.Controls.Add(new Literal { Text = "<br/><br/>" });

                var pnlDetalles = new Panel { ID = "pnlDetallesOpcionSimple" };
                pnlEditorDinamico.Controls.Add(pnlDetalles);

                // **Llamada para mostrar detalles dentro del panel**
                if (opcionIndex >= 0 && opcionIndex < menu.opciones.Count)
                {
                    MostrarDetallesOpcionSimple(menu.opciones[opcionIndex]);
                }
            }
            else
            {
                pnlEditorDinamico.Controls.Add(new Literal { Text = "No hay opciones o categorías disponibles en este menú." });
            }
        }
        protected void Seleccionar_Campus(object sender, EventArgs e)
        {
            LlenarDropdownMenu();
            pnlEditorDinamico.Controls.Clear();
            //lblMensaje.Text = "";
        }

        private void LlenarDropdownMenu()
        {
            ddlMenu.Items.Clear();

            if (datos.campus.ContainsKey(campusActual))
            {
                var campus = datos.campus[campusActual];
                for (int i = 0; i < campus.menu.Count; i++)
                {
                    ddlMenu.Items.Add(new ListItem(campus.menu[i].titulo, i.ToString()));
                }
            }

            ddlMenu.Items.Insert(0, new ListItem("-- Seleccione un menú --", ""));
        }

        protected void ddlMenu_SelectedIndexChanged(object sender, EventArgs e)
        {
            pnlEditorDinamico.Controls.Clear();

            if (int.TryParse(ddlMenu.SelectedValue, out int index))
            {
                var menu = datos.campus[campusActual].menu[index];
                Session["menuSeleccionado"] = menu; // ✅ importante
                MostrarEditorParaMenu(menu);
            }
        }
        protected void ddlOpcionesSimples_SelectedIndexChanged(object sender, EventArgs e)
        {
            var ddl = sender as DropDownList;
            if (ddl != null)
                ViewState["ddlOpcionesSimples"] = ddl.SelectedValue;

            if (Session["menuSeleccionado"] is MenuItemm menu)
                MostrarEditorParaMenu(menu);
        }
        private void ddlCategoriasOpcion_SelectedIndexChanged(object sender, EventArgs e)
        {
            var ddl = sender as DropDownList;
            if (ddl != null)
                ViewState["CategoriaOpcionSeleccionada"] = ddl.SelectedValue;

            var menu = Session["menuSeleccionado"] as MenuItemm;
            if (menu != null)
                MostrarEditorParaMenu(menu);
        }

        private void MostrarDetallesOpcionSimple(OpcionSimple opcion)
        {
            var pnlDetalles = pnlEditorDinamico.FindControl("pnlDetallesOpcionSimple") as Panel;
            if (pnlDetalles == null) return;
            pnlDetalles.Controls.Clear();

            var msg = opcion.contenido?.mensajeRector;
            if (msg == null) return;

            // ================= IMAGEN BANNER ====================
            if (!string.IsNullOrEmpty(opcion.contenido.imagenBanner))
            {
                pnlDetalles.Controls.Add(new Literal { Text = "<b>Imagen Banner:</b><br/>" });
                // Obtiene la URL base del frontend desde web.config
                string frontendUrl = ConfigurationManager.AppSettings["FrontendUrl"];
                string rutaPublica = $"{frontendUrl}/vistas/{opcion.contenido.imagenBanner}";
                var img = new Image { ImageUrl = rutaPublica, Height = 120 };
                pnlDetalles.Controls.Add(img);
                pnlDetalles.Controls.Add(new Literal { Text = "<br/>" });
            }
            pnlDetalles.Controls.Add(new Literal { Text = "<b>Sube nuevo Banner:</b><br/>" });
            pnlDetalles.Controls.Add(new FileUpload { ID = "fuBanner" });
            pnlDetalles.Controls.Add(new Literal { Text = "<br/><br/>" });

            // ================= TÍTULO ====================
            pnlDetalles.Controls.Add(new Literal { Text = "<b>Título:</b><br/>" });
            pnlDetalles.Controls.Add(new TextBox
            {
                ID = "txtTitulo",
                Text = msg.titulo ?? string.Empty,
                Width = Unit.Percentage(100)
            });
            pnlDetalles.Controls.Add(new Literal { Text = "<br/><br/>" });

            // ================= PÁRRAFOS ====================
            if (msg.parrafos != null && msg.parrafos.Count > 0)
            {
                pnlDetalles.Controls.Add(new Literal { Text = "<b>Párrafos:</b><br/>" });
                pnlDetalles.Controls.Add(new TextBox
                {
                    ID = "txtParrafos",
                    TextMode = TextBoxMode.MultiLine,
                    Rows = 6,
                    Width = Unit.Percentage(100),
                    Text = string.Join("\n", msg.parrafos)
                });
                pnlDetalles.Controls.Add(new Literal { Text = "<br/><br/>" });
            }

            // ================= LISTA ====================
            if (msg.lista != null && msg.lista.Count > 0)
            {
                pnlDetalles.Controls.Add(new Literal { Text = "<b>Lista (uno por línea):</b><br/>" });
                pnlDetalles.Controls.Add(new TextBox
                {
                    ID = "txtLista",
                    TextMode = TextBoxMode.MultiLine,
                    Rows = 4,
                    Width = Unit.Percentage(100),
                    Text = string.Join("\n", msg.lista)
                });
                pnlDetalles.Controls.Add(new Literal { Text = "<br/><br/>" });
            }

            // ================= PÁRRAFOS FINALES ====================
            if (msg.parrafosFinales != null && msg.parrafosFinales.Count > 0)
            {
                pnlDetalles.Controls.Add(new Literal { Text = "<b>Párrafos Finales:</b><br/>" });
                pnlDetalles.Controls.Add(new TextBox
                {
                    ID = "txtParrafosFinales",
                    TextMode = TextBoxMode.MultiLine,
                    Rows = 4,
                    Width = Unit.Percentage(100),
                    Text = string.Join("\n", msg.parrafosFinales)
                });
                pnlDetalles.Controls.Add(new Literal { Text = "<br/><br/>" });
            }

            // ================= IFRAME MAPA ====================
            if (!string.IsNullOrEmpty(msg.iframeMapa))
            {
                pnlDetalles.Controls.Add(new Literal { Text = "<b>Iframe Mapa:</b><br/>" });
                pnlDetalles.Controls.Add(new TextBox
                {
                    ID = "txtIframe",
                    Width = Unit.Percentage(100),
                    Text = msg.iframeMapa
                });
                pnlDetalles.Controls.Add(new Literal { Text = "<br/><br/>" });
            }

            // ================= TÍTULO 2 ====================
            if (!string.IsNullOrEmpty(msg.titulo2))
            {
                pnlDetalles.Controls.Add(new Literal { Text = "<b>Título 2:</b><br/>" });
                pnlDetalles.Controls.Add(new TextBox
                {
                    ID = "txtTitulo2",
                    Width = Unit.Percentage(100),
                    Text = msg.titulo2
                });
                pnlDetalles.Controls.Add(new Literal { Text = "<br/><br/>" });
            }

            // ================= SECCIONES ====================
            if (msg.secciones != null && msg.secciones.Count > 0)
            {
                for (int i = 0; i < msg.secciones.Count; i++)
                {
                    var sec = msg.secciones[i];
                    pnlDetalles.Controls.Add(new Literal { Text = $"<h4>Sección {i + 1}</h4>" });

                    // Subtítulo
                    if (!string.IsNullOrEmpty(sec.subtitulo))
                    {
                        pnlDetalles.Controls.Add(new Literal { Text = "<b>Subtítulo:</b><br/>" });
                        pnlDetalles.Controls.Add(new TextBox { Text = sec.subtitulo, Width = 400, ID = $"txtSubtitulo_{i}" });
                        pnlDetalles.Controls.Add(new Literal { Text = "<br/><br/>" });
                    }

                    // Párrafos
                    if (sec.parrafos != null && sec.parrafos.Count > 0)
                    {
                        pnlDetalles.Controls.Add(new Literal { Text = "<b>Párrafos:</b><br/>" });
                        pnlDetalles.Controls.Add(new TextBox
                        {
                            Text = string.Join(Environment.NewLine, sec.parrafos),
                            TextMode = TextBoxMode.MultiLine,
                            Rows = 4,
                            Width = 500,
                            ID = $"txtParrafos_{i}"
                        });
                        pnlDetalles.Controls.Add(new Literal { Text = "<br/><br/>" });
                    }

                    // Lista
                    if (sec.lista != null && sec.lista.Count > 0)
                    {
                        pnlDetalles.Controls.Add(new Literal { Text = "<b>Lista:</b><br/>" });
                        pnlDetalles.Controls.Add(new TextBox
                        {
                            Text = string.Join(Environment.NewLine, sec.lista),
                            TextMode = TextBoxMode.MultiLine,
                            Rows = 4,
                            Width = 500,
                            ID = $"txtLista_{i}"
                        });
                        pnlDetalles.Controls.Add(new Literal { Text = "<br/><br/>" });
                    }

                    // Párrafos1
                    if (sec.parrafos1 != null && sec.parrafos1.Count > 0)
                    {
                        pnlDetalles.Controls.Add(new Literal { Text = "<b>Párrafos 1:</b><br/>" });
                        pnlDetalles.Controls.Add(new TextBox
                        {
                            Text = string.Join(Environment.NewLine, sec.parrafos1),
                            TextMode = TextBoxMode.MultiLine,
                            Rows = 4,
                            Width = 500,
                            ID = $"txtParrafos1_{i}"
                        });
                        pnlDetalles.Controls.Add(new Literal { Text = "<br/><br/>" });
                    }

                    // Párrafo1
                    if (sec.parrafo1 != null && sec.parrafo1.Count > 0)
                    {
                        pnlDetalles.Controls.Add(new Literal { Text = "<b>Párrafo 1:</b><br/>" });
                        pnlDetalles.Controls.Add(new TextBox
                        {
                            Text = string.Join(Environment.NewLine, sec.parrafo1),
                            TextMode = TextBoxMode.MultiLine,
                            Rows = 4,
                            Width = 400,
                            ID = $"txtParrafo1_{i}"
                        });
                        pnlDetalles.Controls.Add(new Literal { Text = "<br/><br/>" });
                    }

                    // Párrafo2
                    if (sec.parrafo2 != null && sec.parrafo2.Count > 0)
                    {
                        pnlDetalles.Controls.Add(new Literal { Text = "<b>Párrafo 2:</b><br/>" });
                        pnlDetalles.Controls.Add(new TextBox
                        {
                            Text = string.Join(Environment.NewLine, sec.parrafo2),
                            TextMode = TextBoxMode.MultiLine,
                            Rows = 4,
                            Width = 400,
                            ID = $"txtParrafo2_{i}"
                        });
                        pnlDetalles.Controls.Add(new Literal { Text = "<br/><br/>" });
                    }

                    // Subtítulo2
                    if (!string.IsNullOrEmpty(sec.subtitulo2))
                    {
                        pnlDetalles.Controls.Add(new Literal { Text = "<b>Subtítulo 2:</b><br/>" });
                        pnlDetalles.Controls.Add(new TextBox { Text = sec.subtitulo2, Width = 400, ID = $"txtSubtitulo2_{i}" });
                        pnlDetalles.Controls.Add(new Literal { Text = "<br/><br/>" });
                    }

                    // Enlace
                    if (!string.IsNullOrEmpty(sec.enlace))
                    {
                        pnlDetalles.Controls.Add(new Literal { Text = "<b>Enlace:</b><br/>" });
                        pnlDetalles.Controls.Add(new TextBox { Text = sec.enlace, Width = 400, ID = $"txtEnlace_{i}" });
                        pnlDetalles.Controls.Add(new Literal { Text = "<br/><br/>" });
                    }

                    // Nota
                    if (sec.nota != null && sec.nota.Count > 0)
                    {
                        pnlDetalles.Controls.Add(new Literal { Text = "<b>Nota:</b><br/>" });
                        pnlDetalles.Controls.Add(new TextBox
                        {
                            Text = string.Join(Environment.NewLine, sec.nota),
                            TextMode = TextBoxMode.MultiLine,
                            Rows = 4,
                            Width = 400,
                            ID = $"txtNota_{i}"
                        });
                        pnlDetalles.Controls.Add(new Literal { Text = "<br/><br/>" });
                    }

                    // Imágenes
                    void MostrarImagen(string url, string idFileUpload)
                    {
                        if (!string.IsNullOrEmpty(url))
                        {
                            Image img = new Image { ImageUrl = "/Frontend/vistas/" + url, Height = 100 };
                            pnlDetalles.Controls.Add(img);
                            pnlDetalles.Controls.Add(new Literal { Text = "<br/>" });
                        }
                        pnlDetalles.Controls.Add(new FileUpload { ID = idFileUpload });
                        pnlDetalles.Controls.Add(new Literal { Text = "<br/><br/>" });
                    }

                    if (!string.IsNullOrEmpty(sec.imagenHospedaje))
                    {
                        pnlDetalles.Controls.Add(new Literal { Text = "<b>Imagen Hospedaje:</b><br/>" });
                        MostrarImagen(sec.imagenHospedaje, $"fuHospedaje1_{i}");
                    }

                    if (!string.IsNullOrEmpty(sec.imagenHospedaje2))
                    {
                        pnlDetalles.Controls.Add(new Literal { Text = "<b>Imagen Hospedaje 2:</b><br/>" });
                        MostrarImagen(sec.imagenHospedaje2, $"fuHospedaje2_{i}");
                    }

                    if (!string.IsNullOrEmpty(sec.imagenHospedaj3))
                    {
                        pnlDetalles.Controls.Add(new Literal { Text = "<b>Imagen Hospedaje 3:</b><br/>" });
                        MostrarImagen(sec.imagenHospedaj3, $"fuHospedaje3_{i}");
                    }

                    // Imágenes Restaurantes
                    if (sec.imagenesRestaurantes != null && sec.imagenesRestaurantes.Count > 0)
                    {
                        pnlDetalles.Controls.Add(new Literal { Text = "<b>Imágenes Restaurantes:</b><br/>" });
                        for (int j = 0; j < sec.imagenesRestaurantes.Count; j++)
                        {
                            var url = sec.imagenesRestaurantes[j];
                            if (!string.IsNullOrEmpty(url))
                            {
                                Image img = new Image { ImageUrl = "/Frontend/vistas/" + url, Height = 80 };
                                pnlDetalles.Controls.Add(img);
                                pnlDetalles.Controls.Add(new Literal { Text = "<br/>" });
                            }
                            pnlDetalles.Controls.Add(new FileUpload { ID = $"fuRestaurante_{i}_{j}" });
                            pnlDetalles.Controls.Add(new Literal { Text = "<br/><br/>" });
                        }
                    }

                    // Subtitulos
                    if (!string.IsNullOrEmpty(sec.subtitulos))
                    {
                        pnlDetalles.Controls.Add(new Literal { Text = "<b>Subtítulos:</b><br/>" });
                        pnlDetalles.Controls.Add(new TextBox { Text = sec.subtitulos, Width = 400, ID = $"txtSubtitulos_{i}" });
                        pnlDetalles.Controls.Add(new Literal { Text = "<br/><br/>" });
                    }

                    // SubSECCIONES (anidadas)
                    if (sec.secciones != null && sec.secciones.Count > 0)
                    {
                        pnlDetalles.Controls.Add(new Literal { Text = $"<h5>Subsecciones de la Sección {i + 1}</h5>" });

                        for (int j = 0; j < sec.secciones.Count; j++)
                        {
                            var subsec = sec.secciones[j];
                            pnlDetalles.Controls.Add(new Literal { Text = $"<h6>Subsección {j + 1}</h6>" });

                            // Subtítulo
                            if (!string.IsNullOrEmpty(subsec.subtitulo))
                            {
                                pnlDetalles.Controls.Add(new Literal { Text = "<b>Subtítulo:</b><br/>" });
                                pnlDetalles.Controls.Add(new TextBox { Text = subsec.subtitulo, Width = 400, ID = $"txtSubSec_Subtitulo_{i}_{j}" });
                                pnlDetalles.Controls.Add(new Literal { Text = "<br/><br/>" });
                            }

                            // Subtítulos
                            if (!string.IsNullOrEmpty(subsec.subtitulos))
                            {
                                pnlDetalles.Controls.Add(new Literal { Text = "<b>Subtítulos:</b><br/>" });
                                pnlDetalles.Controls.Add(new TextBox { Text = subsec.subtitulos, Width = 400, ID = $"txtSubSec_Subtitulos_{i}_{j}" });
                                pnlDetalles.Controls.Add(new Literal { Text = "<br/><br/>" });
                            }

                            // Párrafos
                            if (subsec.parrafos != null && subsec.parrafos.Count > 0)
                            {
                                pnlDetalles.Controls.Add(new Literal { Text = "<b>Párrafos:</b><br/>" });
                                pnlDetalles.Controls.Add(new TextBox
                                {
                                    Text = string.Join(Environment.NewLine, subsec.parrafos),
                                    TextMode = TextBoxMode.MultiLine,
                                    Rows = 4,
                                    Width = 500,
                                    ID = $"txtSubSec_Parrafos_{i}_{j}"
                                });
                                pnlDetalles.Controls.Add(new Literal { Text = "<br/><br/>" });
                            }

                            // Lista
                            if (subsec.lista != null && subsec.lista.Count > 0)
                            {
                                pnlDetalles.Controls.Add(new Literal { Text = "<b>Lista:</b><br/>" });
                                pnlDetalles.Controls.Add(new TextBox
                                {
                                    Text = string.Join(Environment.NewLine, subsec.lista),
                                    TextMode = TextBoxMode.MultiLine,
                                    Rows = 4,
                                    Width = 500,
                                    ID = $"txtSubSec_Lista_{i}_{j}"
                                });
                                pnlDetalles.Controls.Add(new Literal { Text = "<br/><br/>" });
                            }

                            // Párrafos1
                            if (subsec.parrafos1 != null && subsec.parrafos1.Count > 0)
                            {
                                pnlDetalles.Controls.Add(new Literal { Text = "<b>Párrafos 1:</b><br/>" });
                                pnlDetalles.Controls.Add(new TextBox
                                {
                                    Text = string.Join(Environment.NewLine, subsec.parrafos1),
                                    TextMode = TextBoxMode.MultiLine,
                                    Rows = 4,
                                    Width = 500,
                                    ID = $"txtSubSec_Parrafos1_{i}_{j}"
                                });
                                pnlDetalles.Controls.Add(new Literal { Text = "<br/><br/>" });
                            }

                            // Párrafo1
                            if (subsec.parrafo1 != null && subsec.parrafo1.Count > 0)
                            {
                                pnlDetalles.Controls.Add(new Literal { Text = "<b>Párrafo 1:</b><br/>" });
                                pnlDetalles.Controls.Add(new TextBox
                                {
                                    Text = string.Join(Environment.NewLine, subsec.parrafo1),
                                    TextMode = TextBoxMode.MultiLine,
                                    Rows = 4,
                                    Width = 500,
                                    ID = $"txtSubSec_Parrafo1_{i}_{j}"
                                });
                                pnlDetalles.Controls.Add(new Literal { Text = "<br/><br/>" });
                            }

                            // Párrafo2
                            if (subsec.parrafo2 != null && subsec.parrafo2.Count > 0)
                            {
                                pnlDetalles.Controls.Add(new Literal { Text = "<b>Párrafo 2:</b><br/>" });
                                pnlDetalles.Controls.Add(new TextBox
                                {
                                    Text = string.Join(Environment.NewLine, subsec.parrafo2),
                                    TextMode = TextBoxMode.MultiLine,
                                    Rows = 4,
                                    Width = 500,
                                    ID = $"txtSubSec_Parrafo2_{i}_{j}"
                                });
                                pnlDetalles.Controls.Add(new Literal { Text = "<br/><br/>" });
                            }

                            // Subtítulo2
                            if (!string.IsNullOrEmpty(subsec.subtitulo2))
                            {
                                pnlDetalles.Controls.Add(new Literal { Text = "<b>Subtítulo 2:</b><br/>" });
                                pnlDetalles.Controls.Add(new TextBox { Text = subsec.subtitulo2, Width = 400, ID = $"txtSubSec_Subtitulo2_{i}_{j}" });
                                pnlDetalles.Controls.Add(new Literal { Text = "<br/><br/>" });
                            }

                            // Enlace
                            if (!string.IsNullOrEmpty(subsec.enlace))
                            {
                                pnlDetalles.Controls.Add(new Literal { Text = "<b>Enlace:</b><br/>" });
                                pnlDetalles.Controls.Add(new TextBox { Text = subsec.enlace, Width = 400, ID = $"txtSubSec_Enlace_{i}_{j}" });
                                pnlDetalles.Controls.Add(new Literal { Text = "<br/><br/>" });
                            }

                            // Nota
                            if (subsec.nota != null && subsec.nota.Count > 0)
                            {
                                pnlDetalles.Controls.Add(new Literal { Text = "<b>Nota:</b><br/>" });
                                pnlDetalles.Controls.Add(new TextBox
                                {
                                    Text = string.Join(Environment.NewLine, subsec.nota),
                                    TextMode = TextBoxMode.MultiLine,
                                    Rows = 4,
                                    Width = 500,
                                    ID = $"txtSubSec_Nota_{i}_{j}"
                                });
                                pnlDetalles.Controls.Add(new Literal { Text = "<br/><br/>" });
                            }

                            // Imágenes Hospedaje
                            void MostrarImagenSubsec(string url, string idFileUpload)
                            {
                                if (!string.IsNullOrEmpty(url))
                                {
                                    Image img = new Image { ImageUrl = "/Frontend/vistas/" + url, Height = 100 };
                                    pnlDetalles.Controls.Add(img);
                                    pnlDetalles.Controls.Add(new Literal { Text = "<br/>" });
                                }
                                pnlDetalles.Controls.Add(new FileUpload { ID = idFileUpload });
                                pnlDetalles.Controls.Add(new Literal { Text = "<br/><br/>" });
                            }

                            if (!string.IsNullOrEmpty(subsec.imagenHospedaje))
                            {
                                pnlDetalles.Controls.Add(new Literal { Text = "<b>Imagen Hospedaje:</b><br/>" });
                                MostrarImagenSubsec(subsec.imagenHospedaje, $"fuSubSec_Hospedaje1_{i}_{j}");
                            }

                            if (!string.IsNullOrEmpty(subsec.imagenHospedaje2))
                            {
                                pnlDetalles.Controls.Add(new Literal { Text = "<b>Imagen Hospedaje 2:</b><br/>" });
                                MostrarImagenSubsec(subsec.imagenHospedaje2, $"fuSubSec_Hospedaje2_{i}_{j}");
                            }

                            if (!string.IsNullOrEmpty(subsec.imagenHospedaj3))
                            {
                                pnlDetalles.Controls.Add(new Literal { Text = "<b>Imagen Hospedaje 3:</b><br/>" });
                                MostrarImagenSubsec(subsec.imagenHospedaj3, $"fuSubSec_Hospedaje3_{i}_{j}");
                            }

                            // Imágenes Restaurantes
                            if (subsec.imagenesRestaurantes != null && subsec.imagenesRestaurantes.Count > 0)
                            {
                                pnlDetalles.Controls.Add(new Literal { Text = "<b>Imágenes Restaurantes:</b><br/>" });

                                for (int k = 0; k < subsec.imagenesRestaurantes.Count; k++)
                                {
                                    var url = subsec.imagenesRestaurantes[k];
                                    if (!string.IsNullOrEmpty(url))
                                    {
                                        Image img = new Image { ImageUrl = "/Frontend/vistas/" + url, Height = 80 };
                                        pnlDetalles.Controls.Add(img);
                                        pnlDetalles.Controls.Add(new Literal { Text = "<br/>" });
                                    }
                                    pnlDetalles.Controls.Add(new FileUpload { ID = $"fuSubSec_Restaurante_{i}_{j}_{k}" });
                                    pnlDetalles.Controls.Add(new Literal { Text = "<br/><br/>" });
                                }
                            }

                            pnlDetalles.Controls.Add(new Literal { Text = "<hr style='border-top:1px dashed #ccc;'/>" });

                        }
                    }


                    pnlDetalles.Controls.Add(new Literal { Text = "<hr/>" });
                }
            }



            if (msg.categorias != null)
            {
                if (msg.categorias == null)
                    msg.categorias = new List<CategoriaSimple>();

                // TEXTO: Agregar nueva categoría
                pnlDetalles.Controls.Add(new Literal { Text = "<h4>Categorías:</h4>Nombre de nueva categoría: " });

                var txtNuevaCategoria = new TextBox { ID = "txtNuevaCategoria" };
                pnlDetalles.Controls.Add(txtNuevaCategoria);

                var btnAgregarCategoria = new Button
                {
                    ID = "btnAgregarCategoria",
                    Text = "Agregar categoría",
                    CommandName = "AgregarCategoria"
                };
                btnAgregarCategoria.Command += btnCategorias_Command;
                pnlDetalles.Controls.Add(btnAgregarCategoria);

                pnlDetalles.Controls.Add(new Literal { Text = "<br/><br/>" });

                if (ViewState["CategoriaOpcionSeleccionada"] == null && msg.categorias.Count > 0)
                    ViewState["CategoriaOpcionSeleccionada"] = "0";

                // SELECTOR de categorías existentes
                var ddlCategorias = new DropDownList
                {
                    ID = "CategoriaOpcion",
                    AutoPostBack = true
                };

                for (int i = 0; i < msg.categorias.Count; i++)
                {
                    var cat = msg.categorias[i];
                    ddlCategorias.Items.Add(new ListItem(cat.nombre, i.ToString()));
                }

                // Asignar índice seleccionado
                var selectedIndex = ViewState["CategoriaOpcionSeleccionada"]?.ToString() ?? "0";
                if (ddlCategorias.Items.FindByValue(selectedIndex) != null)
                    ddlCategorias.SelectedValue = selectedIndex;

                ddlCategorias.SelectedIndexChanged += ddlCategoriasOpcion_SelectedIndexChanged;
                pnlDetalles.Controls.Add(new Literal { Text = "Seleccionar categoría:<br/>" });
                pnlDetalles.Controls.Add(ddlCategorias);

                pnlDetalles.Controls.Add(new Literal { Text = "<br/>" });

                // BOTÓN eliminar categoría
                var btnEliminarCategoria = new Button
                {
                    ID = "btnEliminarCategoria",
                    Text = "Eliminar categoría seleccionada",
                    CommandName = "EliminarCategoria"
                };
                btnEliminarCategoria.Command += btnCategorias_Command;
                pnlDetalles.Controls.Add(btnEliminarCategoria);

                // --- MOSTRAR PROGRAMAS DE LA CATEGORÍA SELECCIONADA ---
                int catIndex = int.TryParse(selectedIndex, out var tempIndex) ? tempIndex : -1;
                if (catIndex >= 0 && catIndex < msg.categorias.Count)
                {
                    var categoria = msg.categorias[catIndex];
                    pnlDetalles.Controls.Add(new Literal { Text = $"<hr/><h4>Programas de la categoría: {categoria.nombre}</h4>" });

                    var pnlProgramas = new Panel { ID = "pnlProgramas" };
                    pnlDetalles.Controls.Add(pnlProgramas);

                    // Mostrar cada programa con campos editables
                    for (int i = 0; i < categoria.programas.Count; i++)
                    {
                        var programa = categoria.programas[i];
                        var pnlPrograma = new Panel { ID = "programaPanel_" + i, CssClass = "programa-panel" };

                        pnlPrograma.Controls.Add(new Literal { Text = $"Nombre: " });
                        var txtNombre = new TextBox { ID = $"txtProgramaNombre_{i}", Text = programa.nombre, Width = Unit.Percentage(90) };
                        pnlPrograma.Controls.Add(txtNombre);
                        pnlPrograma.Controls.Add(new Literal { Text = "<br/>" });

                        pnlPrograma.Controls.Add(new Literal { Text = $"Inicio: " });
                        var txtInicio = new TextBox { ID = $"txtProgramaInicio_{i}", Text = programa.inicio, Width = Unit.Percentage(90) };
                        pnlPrograma.Controls.Add(txtInicio);
                        pnlPrograma.Controls.Add(new Literal { Text = "<br/>" });

                        pnlPrograma.Controls.Add(new Literal { Text = $"Duración: " });
                        var txtDuracion = new TextBox { ID = $"txtProgramaDuracion_{i}", Text = programa.duracion, Width = Unit.Percentage(90) };
                        pnlPrograma.Controls.Add(txtDuracion);
                        pnlPrograma.Controls.Add(new Literal { Text = "<br/>" });

                        pnlPrograma.Controls.Add(new Literal { Text = $"Enlace: " });
                        var txtEnlace = new TextBox { ID = $"txtProgramaEnlace_{i}", Text = programa.enlace, Width = Unit.Percentage(90) };
                        pnlPrograma.Controls.Add(txtEnlace);
                        pnlPrograma.Controls.Add(new Literal { Text = "<br/>" });

                        pnlPrograma.Controls.Add(new Literal { Text = "Clase: " });

                        // Crear DropDownList dinámico
                        var ddlClase = new DropDownList
                        {
                            ID = $"ddlprogramaclase{i}",
                            Width = Unit.Percentage(90)
                        };

                        // Opciones fijas
                        ddlClase.Items.Add(new ListItem("Monterrey", "Monterrey"));
                        ddlClase.Items.Add(new ListItem("Mérida", "Merida"));
                        ddlClase.Items.Add(new ListItem("Guadalajara", "Guadalajara"));
                        ddlClase.Items.Add(new ListItem("Virtual", "Virtual"));
                        ddlClase.Items.Add(new ListItem("Puebla", "Puebla"));

                        // Seleccionar la ciudad actual si existe en la lista
                        var ciudadActual = programa.ciudad;
                        if (ddlClase.Items.FindByValue(ciudadActual) != null)
                        {
                            ddlClase.SelectedValue = ciudadActual;
                        }

                        pnlPrograma.Controls.Add(ddlClase);
                        pnlPrograma.Controls.Add(new Literal { Text = "<br/>" });


                        var btnEliminarPrograma = new Button
                        {
                            ID = $"btnEliminarPrograma_{i}",
                            Text = "Eliminar programa",
                            CommandName = "EliminarPrograma",
                            CommandArgument = i.ToString()
                        };
                        btnEliminarPrograma.Command += btnProgramas_Command;
                        pnlPrograma.Controls.Add(btnEliminarPrograma);

                        pnlProgramas.Controls.Add(pnlPrograma);
                        pnlProgramas.Controls.Add(new Literal { Text = "<br/>" });
                    }

                    // Controles para agregar nuevo programa
                    pnlDetalles.Controls.Add(new Literal { Text = "<hr/><h5>Agregar nuevo programa</h5>" });

                    var pnlNuevoPrograma = new Panel { ID = "pnlNuevoPrograma" };
                    pnlDetalles.Controls.Add(pnlNuevoPrograma);

                    var txtNuevoNombre = new TextBox { ID = "txtNuevoProgramaNombre", Width = Unit.Percentage(90) };
                    txtNuevoNombre.Attributes["placeholder"] = "Nombre";
                    pnlNuevoPrograma.Controls.Add(txtNuevoNombre);
                    pnlNuevoPrograma.Controls.Add(new Literal { Text = "<br/>" });

                    var txtNuevoInicio = new TextBox { ID = "txtNuevoProgramaInicio", Width = Unit.Percentage(90) };
                    txtNuevoInicio.Attributes["placeholder"] = "Inicio";
                    pnlNuevoPrograma.Controls.Add(txtNuevoInicio);
                    pnlNuevoPrograma.Controls.Add(new Literal { Text = "<br/>" });

                    var txtNuevoDuracion = new TextBox { ID = "txtNuevoProgramaDuracion", Width = Unit.Percentage(90) };
                    txtNuevoDuracion.Attributes["placeholder"] = "Duración";
                    pnlNuevoPrograma.Controls.Add(txtNuevoDuracion);
                    pnlNuevoPrograma.Controls.Add(new Literal { Text = "<br/>" });

                    var txtNuevoEnlace = new TextBox { ID = "txtNuevoProgramaEnlace", Width = Unit.Percentage(90) };
                    txtNuevoEnlace.Attributes["placeholder"] = "Enlace";
                    pnlNuevoPrograma.Controls.Add(txtNuevoEnlace);
                    pnlNuevoPrograma.Controls.Add(new Literal { Text = "<br/>" });

                    // Crear DropDownList para nueva ciudad
                    var ddlNuevoCiudad = new DropDownList
                    {
                        ID = "ddlNuevoProgramaCiudad",
                        Width = Unit.Percentage(90)
                    };

                    // Agregar opciones
                    ddlNuevoCiudad.Items.Add(new ListItem("Monterrey", "Monterrey"));
                    ddlNuevoCiudad.Items.Add(new ListItem("Mérida", "Merida"));
                    ddlNuevoCiudad.Items.Add(new ListItem("Guadalajara", "Guadalajara"));
                    ddlNuevoCiudad.Items.Add(new ListItem("Virtual", "Virtual"));
                    ddlNuevoCiudad.Items.Add(new ListItem("Puebla", "Puebla"));

                    // Opción inicial vacía como placeholder
                    ddlNuevoCiudad.Items.Insert(0, new ListItem("Selecciona una clase...", ""));

                    pnlNuevoPrograma.Controls.Add(ddlNuevoCiudad);
                    pnlNuevoPrograma.Controls.Add(new Literal { Text = "<br/>" });



                    var btnAgregarPrograma = new Button
                    {
                        ID = "btnAgregarPrograma",
                        Text = "Agregar programa",
                        CommandName = "AgregarPrograma"
                    };
                    btnAgregarPrograma.Command += btnProgramas_Command;
                    pnlNuevoPrograma.Controls.Add(btnAgregarPrograma);
                }
            }
        }

        private void btnCategorias_Command(object sender, CommandEventArgs e)
        {
            var menu = Session["menuSeleccionado"] as MenuItemm;
            if (menu == null) return;

            string opcionSeleccionada = ViewState["ddlOpcionesSimples"] as string ?? "0";
            int opcionIndex = int.TryParse(opcionSeleccionada, out int oIdx) ? oIdx : 0;
            if (menu.opciones.Count <= opcionIndex) return;

            var opcion = menu.opciones[opcionIndex];
            var msg = opcion.contenido?.mensajeRector;
            if (msg == null) return;
            if (msg.categorias == null)
                msg.categorias = new List<CategoriaSimple>();

            switch (e.CommandName)
            {
                case "AgregarCategoria":
                    var txt = pnlEditorDinamico.FindControl("txtNuevaCategoria") as TextBox;
                    if (txt != null && !string.IsNullOrWhiteSpace(txt.Text))
                    {
                        msg.categorias.Add(new CategoriaSimple
                        {
                            nombre = txt.Text.Trim(),
                            programas = new List<ProgramaSimple>()
                        });
                        ViewState["CategoriaOpcionSeleccionada"] = (msg.categorias.Count - 1).ToString();
                    }
                    break;

                case "EliminarCategoria":
                    if (ViewState["CategoriaOpcionSeleccionada"] != null)
                    {
                        int index = int.Parse(ViewState["CategoriaOpcionSeleccionada"].ToString());
                        if (index >= 0 && index < msg.categorias.Count)
                        {
                            msg.categorias.RemoveAt(index);
                            ViewState["CategoriaOpcionSeleccionada"] = null;
                        }
                    }
                    break;
            }

            // Refrescar
            MostrarEditorParaMenu(menu);
        }

        private void btnProgramas_Command(object sender, CommandEventArgs e)
        {
            var menu = Session["menuSeleccionado"] as MenuItemm;
            if (menu == null) return;

            string opcionSeleccionada = ViewState["ddlOpcionesSimples"] as string ?? "0";
            int opcionIndex = int.TryParse(opcionSeleccionada, out int oIdx) ? oIdx : 0;
            if (menu.opciones.Count <= opcionIndex) return;

            var opcion = menu.opciones[opcionIndex];
            var msg = opcion.contenido?.mensajeRector;
            if (msg == null) return;
            if (msg.categorias == null)
                msg.categorias = new List<CategoriaSimple>();

            int catIndex = -1;
            if (ViewState["CategoriaOpcionSeleccionada"] != null)
                int.TryParse(ViewState["CategoriaOpcionSeleccionada"].ToString(), out catIndex);

            if (catIndex < 0 || catIndex >= msg.categorias.Count) return;

            var categoria = msg.categorias[catIndex];

            switch (e.CommandName)
            {
                case "AgregarPrograma":
                    var txtNombre = pnlEditorDinamico.FindControl("txtNuevoProgramaNombre") as TextBox;
                    var txtInicio = pnlEditorDinamico.FindControl("txtNuevoProgramaInicio") as TextBox;
                    var txtDuracion = pnlEditorDinamico.FindControl("txtNuevoProgramaDuracion") as TextBox;
                    var txtEnlace = pnlEditorDinamico.FindControl("txtNuevoProgramaEnlace") as TextBox;
                    var ddlClase = pnlEditorDinamico.FindControl("ddlNuevoProgramaCiudad") as DropDownList;

                    if (txtNombre != null && txtInicio != null && txtDuracion != null && txtEnlace != null)
                    {
                        if (!string.IsNullOrWhiteSpace(txtNombre.Text))
                        {
                            categoria.programas.Add(new ProgramaSimple
                            {
                                nombre = txtNombre.Text.Trim(),
                                inicio = txtInicio.Text.Trim(),
                                duracion = txtDuracion.Text.Trim(),
                                enlace = txtEnlace.Text.Trim(),
                                ciudad = ddlClase?.SelectedValue
                            });

                            // Limpiar campos
                            txtNombre.Text = "";
                            txtInicio.Text = "";
                            txtDuracion.Text = "";
                            txtEnlace.Text = "";
                            ddlClase.SelectedIndex = 0;

                            ViewState["CategoriaOpcionSeleccionada"] = catIndex.ToString();
                        }
                    }
                    break;

                case "EliminarPrograma":
                    if (int.TryParse(e.CommandArgument.ToString(), out int progIndex))
                    {
                        if (progIndex >= 0 && progIndex < categoria.programas.Count)
                        {
                            categoria.programas.RemoveAt(progIndex);
                            ViewState["CategoriaOpcionSeleccionada"] = catIndex.ToString();
                        }
                    }
                    break;
            }

            // Refrescar la vista
            MostrarEditorParaMenu(menu);
        }

        private void GuardarOpcionSimple(MenuItemm menu, int opcionIndex)
        {
            var opcion = menu.opciones[opcionIndex];
            if (opcion.contenido == null)
                opcion.contenido = new ContenidoOpcionSimple();

            if (opcion.contenido.mensajeRector == null)
                opcion.contenido.mensajeRector = new Mensaje();

            var msg = opcion.contenido.mensajeRector;

            var txtTitulo = pnlEditorDinamico.FindControl("txtTitulo") as TextBox;
            if (txtTitulo != null)
                msg.titulo = txtTitulo.Text.Trim();

            var txtParrafosGeneral = pnlEditorDinamico.FindControl("txtParrafos") as TextBox;
            if (txtParrafosGeneral != null)
                msg.parrafos = txtParrafosGeneral.Text
                    .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(p => p.Trim())
                    .Where(p => !string.IsNullOrWhiteSpace(p))
                    .ToList();

            var txtListaGeneral = pnlEditorDinamico.FindControl("txtLista") as TextBox;
            if (txtListaGeneral != null)
                msg.lista = txtListaGeneral.Text
                    .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(p => p.Trim())
                    .Where(p => !string.IsNullOrWhiteSpace(p))
                    .ToList();

            var txtParrafosFinales = pnlEditorDinamico.FindControl("txtParrafosFinales") as TextBox;
            if (txtParrafosFinales != null)
                msg.parrafosFinales = txtParrafosFinales.Text
                    .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(p => p.Trim())
                    .Where(p => !string.IsNullOrWhiteSpace(p))
                    .ToList();

            var txtIframe = pnlEditorDinamico.FindControl("txtIframe") as TextBox;
            if (txtIframe != null)
                msg.iframeMapa = txtIframe.Text.Trim();

            var txtTitulo2 = pnlEditorDinamico.FindControl("txtTitulo2") as TextBox;
            if (txtTitulo2 != null)
                msg.titulo2 = txtTitulo2.Text.Trim();

            // Validar que hay categorías y una seleccionada
            if (msg.categorias == null || msg.categorias.Count == 0) return;

            int catIndex = -1;
            if (ViewState["CategoriaOpcionSeleccionada"] != null)
                int.TryParse(ViewState["CategoriaOpcionSeleccionada"].ToString(), out catIndex);

            if (catIndex < 0 || catIndex >= msg.categorias.Count) return;

            var categoria = msg.categorias[catIndex];

            // Buscar el panel contenedor de los programas
            var pnlProgramas = pnlEditorDinamico.FindControl("pnlProgramas") as Panel;
            if (pnlProgramas == null) return;

            // Crear una nueva lista para los programas actualizados
            var nuevosProgramas = new List<ProgramaSimple>();

            for (int i = 0; i < categoria.programas.Count; i++)
            {
                var txtNombre = pnlEditorDinamico.FindControl($"txtProgramaNombre_{i}") as TextBox;
                var txtInicio = pnlEditorDinamico.FindControl($"txtProgramaInicio_{i}") as TextBox;
                var txtDuracion = pnlEditorDinamico.FindControl($"txtProgramaDuracion_{i}") as TextBox;
                var txtEnlace = pnlEditorDinamico.FindControl($"txtProgramaEnlace_{i}") as TextBox;
                var ddlClase = pnlEditorDinamico.FindControl($"ddlprogramaclase{i}") as DropDownList;

                if (txtNombre != null && txtInicio != null && txtDuracion != null && txtEnlace != null)
                {
                    nuevosProgramas.Add(new ProgramaSimple
                    {
                        nombre = txtNombre.Text.Trim(),
                        inicio = txtInicio.Text.Trim(),
                        duracion = txtDuracion.Text.Trim(),
                        enlace = txtEnlace.Text.Trim(),
                        ciudad = ddlClase.SelectedValue
                    });
                }
            }

            // Reemplazar programas actualizados en la categoría
            categoria.programas = nuevosProgramas;


            // ---------------------
            // 🖼️ Imágenes
            // ---------------------
            void ProcesarImagen(string controlID, Action<string> asignarRuta)
            {
                var fileUpload = pnlEditorDinamico.FindControl(controlID) as FileUpload;
                if (fileUpload != null && fileUpload.HasFile)
                {
                    string nombreArchivo = Path.GetFileName(fileUpload.FileName);
                    string rutaRelativa = $"fotos/{campusActual}/{nombreArchivo}";

                    // 💡 Corrección: Obtiene la ruta física del frontend desde web.config
                    string frontendPath = ConfigurationManager.AppSettings["FrontendPhysicalPath"];
                    string rutaFisica = Path.Combine(frontendPath, rutaRelativa);

                    // Crear carpeta si no existe
                    string carpeta = Path.GetDirectoryName(rutaFisica);
                    if (!Directory.Exists(carpeta))
                        Directory.CreateDirectory(carpeta);

                    // Guardar imagen
                    fileUpload.SaveAs(rutaFisica);

                    // Asignar ruta relativa en JSON
                    asignarRuta(rutaRelativa);
                }
            }

            // Imagen banner
            ProcesarImagen("fuBanner", ruta => opcion.contenido.imagenBanner = ruta);

            // ---------------------
            // 📚 Leer datos de secciones
            // ---------------------
            if (msg.secciones != null && msg.secciones.Count > 0)
            {
                for (int i = 0; i < msg.secciones.Count; i++)
                {
                    var sec = msg.secciones[i];

                    var txtSubtitulo = pnlEditorDinamico.FindControl($"txtSubtitulo_{i}") as TextBox;
                    if (txtSubtitulo != null) sec.subtitulo = txtSubtitulo.Text;

                    var txtParrafosSec = pnlEditorDinamico.FindControl($"txtParrafos_{i}") as TextBox;
                    if (txtParrafosSec != null)
                        sec.parrafos = txtParrafosSec.Text
                            .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                            .Select(p => p.Trim())
                            .Where(p => !string.IsNullOrWhiteSpace(p))
                            .ToList();

                    var txtListaSec = pnlEditorDinamico.FindControl($"txtLista_{i}") as TextBox;
                    if (txtListaSec != null)
                        sec.lista = txtListaSec.Text
                            .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                            .Select(p => p.Trim())
                            .Where(p => !string.IsNullOrWhiteSpace(p))
                            .ToList();

                    var txtParrafos1 = pnlEditorDinamico.FindControl($"txtParrafos1_{i}") as TextBox;
                    if (txtParrafos1 != null)
                        sec.parrafos1 = txtParrafos1.Text
                            .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                            .Select(p => p.Trim())
                            .Where(p => !string.IsNullOrWhiteSpace(p))
                            .ToList();

                    var txtParrafo1 = pnlEditorDinamico.FindControl($"txtParrafo1_{i}") as TextBox;
                    if (txtParrafo1 != null)
                        sec.parrafo1 = txtParrafo1.Text
                            .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                            .Select(p => p.Trim())
                            .Where(p => !string.IsNullOrWhiteSpace(p))
                            .ToList();

                    var txtParrafo2 = pnlEditorDinamico.FindControl($"txtParrafo2_{i}") as TextBox;
                    if (txtParrafo2 != null)
                        sec.parrafo2 = txtParrafo2.Text
                            .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                            .Select(p => p.Trim())
                            .Where(p => !string.IsNullOrWhiteSpace(p))
                            .ToList();

                    var txtSubtitulo2 = pnlEditorDinamico.FindControl($"txtSubtitulo2_{i}") as TextBox;
                    if (txtSubtitulo2 != null) sec.subtitulo2 = txtSubtitulo2.Text;

                    var txtEnlaceSec = pnlEditorDinamico.FindControl($"txtEnlace_{i}") as TextBox;
                    if (txtEnlaceSec != null) sec.enlace = txtEnlaceSec.Text;

                    var txtNotaSec = pnlEditorDinamico.FindControl($"txtNota_{i}") as TextBox;
                    if (txtNotaSec != null)
                        sec.nota = txtNotaSec.Text
                            .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                            .Select(p => p.Trim())
                            .Where(p => !string.IsNullOrWhiteSpace(p))
                            .ToList();

                    var txtSubtitulosSec = pnlEditorDinamico.FindControl($"txtSubtitulos_{i}") as TextBox;
                    if (txtSubtitulosSec != null) sec.subtitulos = txtSubtitulosSec.Text;

                    // Procesar imágenes
                    ProcesarImagen($"fuHospedaje1_{i}", ruta => sec.imagenHospedaje = ruta);
                    ProcesarImagen($"fuHospedaje2_{i}", ruta => sec.imagenHospedaje2 = ruta);
                    ProcesarImagen($"fuHospedaje3_{i}", ruta => sec.imagenHospedaj3 = ruta);

                    // 🔽 Agrega este bloque para guardar las subsecciones anidadas
                    if (sec.secciones != null && sec.secciones.Count > 0)
                    {
                        for (int j = 0; j < sec.secciones.Count; j++)
                        {
                            var subsec = sec.secciones[j];

                            string GetText(string id) =>
                                (pnlEditorDinamico.FindControl(id) as TextBox)?.Text ?? "";

                            List<string> GetListFromTextArea(string id) =>
                                (pnlEditorDinamico.FindControl(id) as TextBox)?
                                .Text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                                .Select(p => p.Trim()).Where(p => !string.IsNullOrWhiteSpace(p)).ToList() ?? new List<string>();

                            // Campos tipo texto
                            subsec.subtitulo = GetText($"txtSubSec_Subtitulo_{i}_{j}");
                            subsec.subtitulos = GetText($"txtSubSec_Subtitulos_{i}_{j}");
                            subsec.subtitulo2 = GetText($"txtSubSec_Subtitulo2_{i}_{j}");
                            subsec.enlace = GetText($"txtSubSec_Enlace_{i}_{j}");

                            // Campos tipo lista
                            subsec.parrafos = GetListFromTextArea($"txtSubSec_Parrafos_{i}_{j}");
                            subsec.lista = GetListFromTextArea($"txtSubSec_Lista_{i}_{j}");
                            subsec.parrafos1 = GetListFromTextArea($"txtSubSec_Parrafos1_{i}_{j}");
                            subsec.parrafo1 = GetListFromTextArea($"txtSubSec_Parrafo1_{i}_{j}");
                            subsec.parrafo2 = GetListFromTextArea($"txtSubSec_Parrafo2_{i}_{j}");
                            subsec.nota = GetListFromTextArea($"txtSubSec_Nota_{i}_{j}");

                            // Imágenes hospedaje
                            ProcesarImagen($"fuSubSec_Hospedaje1_{i}_{j}", ruta => subsec.imagenHospedaje = ruta);
                            ProcesarImagen($"fuSubSec_Hospedaje2_{i}_{j}", ruta => subsec.imagenHospedaje2 = ruta);
                            ProcesarImagen($"fuSubSec_Hospedaje3_{i}_{j}", ruta => subsec.imagenHospedaj3 = ruta);

                            // Imágenes restaurantes (puede haber varias)
                            if (subsec.imagenesRestaurantes != null)
                            {
                                for (int k = 0; k < subsec.imagenesRestaurantes.Count; k++)
                                {
                                    string controlId = $"fuSubSec_Restaurante_{i}_{j}_{k}";
                                    ProcesarImagen(controlId, ruta =>
                                    {
                                        if (!string.IsNullOrEmpty(ruta))
                                            subsec.imagenesRestaurantes[k] = ruta;
                                    });
                                }
                            }
                        }
                    }
                }
            }
        }

        protected void ddlCategorias_SelectedIndexChanged(object sender, EventArgs e)
        {
            var ddl = sender as DropDownList;
            ViewState["categoriaSeleccionada"] = ddl.SelectedValue;

            if (Session["menuSeleccionado"] is MenuItemm menu)
            {
                MostrarEditorParaMenu(menu);
            }
        }

        protected void ddlProgramas_SelectedIndexChanged(object sender, EventArgs e)
        {
            var ddl = sender as DropDownList;
            ViewState["programaSeleccionado"] = ddl.SelectedValue;

            if (Session["menuSeleccionado"] is MenuItemm menu)
            {
                MostrarEditorParaMenu(menu);
            }
        }

        protected void ddlCompetencias_SelectedIndexChanged(object sender, EventArgs e)
        {
            var ddl = sender as DropDownList;
            if (ddl != null)
            {
                ViewState["CategoriaCompetenciaSeleccionada"] = ddl.SelectedValue; // actualizar esta
                ViewState["tipoCategoriaActual"] = ddl.SelectedValue; // si quieres mantenerlo también

                // Aquí debes refrescar el panel para que muestre la nueva categoría seleccionada
                if (int.TryParse(ddlMenu.SelectedValue, out int menuIdx))
                {
                    var menu = datos.campus[campusActual].menu[menuIdx];
                    if (int.TryParse((pnlEditorDinamico.FindControl("ddlCategorias") as DropDownList)?.SelectedValue, out int catIdx))
                    {
                        if (int.TryParse((pnlEditorDinamico.FindControl("ddlProgramas") as DropDownList)?.SelectedValue, out int progIdx))
                        {
                            var programa = menu.categorias[catIdx].programas[progIdx];
                            MostrarDetallesPrograma(programa);
                        }
                    }
                }
            }
        }

        private void MostrarDetallesPrograma(Programa programa)
        {
            var pnlDetalles = pnlEditorDinamico.FindControl("pnlDetallesPrograma") as Panel;
            if (pnlDetalles == null) return;

            // Limpiamos para regenerar los controles dinámicamente
            pnlDetalles.Controls.Clear();

            pnlDetalles.Controls.Add(new Literal { Text = "<h4>Detalles del Programa:</h4>" });

            // Nombre
            pnlDetalles.Controls.Add(new Literal { Text = "Nombre:<br/>" });
            pnlDetalles.Controls.Add(new TextBox { ID = "txtNombrePrograma", Text = programa.nombre, Width = 400 });
            pnlDetalles.Controls.Add(new Literal { Text = "<br/><br/>" });

            // Tipo
            pnlDetalles.Controls.Add(new Literal { Text = "Tipo:<br/>" });
            pnlDetalles.Controls.Add(new TextBox { ID = "txtTipoPrograma", Text = programa.tipo, Width = 400 });
            pnlDetalles.Controls.Add(new Literal { Text = "<br/><br/>" });

            // ...
            string frontendUrl = ConfigurationManager.AppSettings["FrontendUrl"];

            // Imagen
            pnlDetalles.Controls.Add(new Literal { Text = "Imagen actual:<br/>" });
            pnlDetalles.Controls.Add(new Image { ImageUrl = $"{frontendUrl}/" + programa.imagen, Width = 150 });
            pnlDetalles.Controls.Add(new Literal { Text = "<br/>Nueva imagen:<br/>" });
            pnlDetalles.Controls.Add(new FileUpload { ID = "fuImagenPrograma" });
            pnlDetalles.Controls.Add(new Literal { Text = "<br/><br/>" });
            // ...

            var info = programa.contenido?.mensajeRector;
            if (info != null)
            {
                pnlDetalles.Controls.Add(new Literal { Text = "<h2>Contenido del Programa</h2>" });

                // Imagen Banner
                pnlDetalles.Controls.Add(new Literal { Text = "Imagen Banner:<br/>" });
                pnlDetalles.Controls.Add(new Image { ImageUrl = $"{frontendUrl}/vistas/" + programa.contenido?.imagenBanner, Width = 150 });
                pnlDetalles.Controls.Add(new Literal { Text = "<br/>Nueva Imagen Banner:<br/>" });
                pnlDetalles.Controls.Add(new FileUpload { ID = "fuImagenBanner" });
                pnlDetalles.Controls.Add(new Literal { Text = "<br/><br/>" });
                // ...

                // Título
                pnlDetalles.Controls.Add(new Literal { Text = "Título:<br/>" });
                pnlDetalles.Controls.Add(new TextBox { ID = "txtTitulo", Text = info.titulo, Width = 400 });
                pnlDetalles.Controls.Add(new Literal { Text = "<br/><br/>" });

                // Subtítulo
                pnlDetalles.Controls.Add(new Literal { Text = "Subtítulo:<br/>" });
                pnlDetalles.Controls.Add(new TextBox { ID = "txtSubtitulo", Text = info.subtitulo, Width = 400 });
                pnlDetalles.Controls.Add(new Literal { Text = "<br/><br/>" });

                // Inicio, Duración, Modalidad, Horarios, Dirigido a
                string[] etiquetas = { "Inicio", "Duración", "Modalidad", "Horarios", "Dirigido A" };
                string[] valores = { info.inicio, info.duracion, info.modalidad, info.horarios, info.dirigidoA };
                string[] ids = { "txtInicio", "txtDuracion", "txtModalidad", "txtHorarios", "txtDirigidoA" };

                for (int i = 0; i < etiquetas.Length; i++)
                {
                    pnlDetalles.Controls.Add(new Literal { Text = etiquetas[i] + ":<br/>" });
                    pnlDetalles.Controls.Add(new TextBox { ID = ids[i], Text = valores[i], Width = 400 });
                    pnlDetalles.Controls.Add(new Literal { Text = "<br/><br/>" });
                }

                // Universidad
                if (info.universidad != null)
                {
                    pnlDetalles.Controls.Add(new Literal { Text = "<h2>Universidad Descripción </h2>" });
                    pnlDetalles.Controls.Add(new Literal { Text = "Universidad:<br/>" });
                    pnlDetalles.Controls.Add(new TextBox { ID = "txtUniversidadNombre", Text = info.universidad.nombre, Width = 400 });
                    pnlDetalles.Controls.Add(new Literal { Text = "<br/><br/>" });

                    pnlDetalles.Controls.Add(new Literal { Text = "Descripción:<br/>" });
                    pnlDetalles.Controls.Add(new TextBox { ID = "txtUniversidadDescripcion", Text = info.universidad?.descripcion?.ToString() ?? "", Width = 400, TextMode = TextBoxMode.MultiLine, Rows = 3 });
                    pnlDetalles.Controls.Add(new Literal { Text = "<br/><br/>" });
                }

                // Objetivo General
                if (info.objetivoGeneral != null)
                {
                    pnlDetalles.Controls.Add(new Literal { Text = "<h3>Objetivo General</h3>" });
                    pnlDetalles.Controls.Add(new Literal { Text = "Subtítulo:<br/>" });
                    pnlDetalles.Controls.Add(new TextBox { ID = "txtObjetivoGeneralSubtitulo", Text = info.objetivoGeneral.subtitulo ?? "", Width = 400 });
                    pnlDetalles.Controls.Add(new Literal { Text = "<br/><br/>" });

                    pnlDetalles.Controls.Add(new Literal { Text = "Descripción:<br/>" });
                    pnlDetalles.Controls.Add(new TextBox { ID = "txtObjetivoGeneralDescripcion", Text = info.objetivoGeneral.descripcion ?? "", Width = 400, TextMode = TextBoxMode.MultiLine, Rows = 3 });
                    pnlDetalles.Controls.Add(new Literal { Text = "<br/><br/>" });
                }

                // Plan de Estudios
                if (info.planEstudios != null)
                {
                    pnlDetalles.Controls.Add(new Literal { Text = "<h3>Plan de Estudios</h3>" });
                    pnlDetalles.Controls.Add(new Literal { Text = "Subtítulo:<br/>" });
                    pnlDetalles.Controls.Add(new TextBox { ID = "txtPlanEstudiosSubtitulo", Text = info.planEstudios.subtitulo ?? "", Width = 400 });
                    pnlDetalles.Controls.Add(new Literal { Text = "<br/><br/>" });

                    pnlDetalles.Controls.Add(new Literal { Text = "Diploma que se otorga:<br/>" });
                    pnlDetalles.Controls.Add(new TextBox { ID = "txtPlanEstudiosDiploma", Text = info.planEstudios.diploma ?? "", Width = 400 });
                    pnlDetalles.Controls.Add(new Literal { Text = "<br/><br/>" });
                }

                // Objetivo Específico
                if (info.objetivoEspecifico != null)
                {
                    pnlDetalles.Controls.Add(new Literal { Text = "<h3>Objetivo Específico</h3>" });
                    pnlDetalles.Controls.Add(new Literal { Text = "Subtítulo:<br/>" });
                    pnlDetalles.Controls.Add(new TextBox { ID = "txtObjetivoEspecificoSubtitulo", Text = info.objetivoEspecifico.subtitulo ?? "", Width = 400 });
                    pnlDetalles.Controls.Add(new Literal { Text = "<br/><br/>" });

                    if (info.objetivoEspecifico.lista != null)
                    {
                        for (int i = 0; i < info.objetivoEspecifico.lista.Count; i++)
                        {
                            pnlDetalles.Controls.Add(new Literal { Text = $"Item {i + 1}:<br/>" });
                            pnlDetalles.Controls.Add(new TextBox { ID = $"txtObjetivoEspecificoItem{i}", Text = info.objetivoEspecifico.lista[i], Width = 400 });
                            pnlDetalles.Controls.Add(new Literal { Text = "<br/><br/>" });
                        }
                    }
                }

                // Valor Curricular
                if (info.valorCurricular != null)
                {
                    pnlDetalles.Controls.Add(new Literal { Text = "<h3>Valor Curricular</h3>" });

                    pnlDetalles.Controls.Add(new Literal { Text = "Subtítulo:<br/>" });
                    pnlDetalles.Controls.Add(new TextBox { ID = "txtValorCurricularSubtitulo", Text = info.valorCurricular.subtitulo ?? "", Width = 400 });
                    pnlDetalles.Controls.Add(new Literal { Text = "<br/><br/>" });

                    pnlDetalles.Controls.Add(new Literal { Text = "Descripción:<br/>" });
                    pnlDetalles.Controls.Add(new TextBox { ID = "txtValorCurricularDescripcion", Text = info.valorCurricular.descripcion ?? "", Width = 400, TextMode = TextBoxMode.MultiLine, Rows = 3 });
                    pnlDetalles.Controls.Add(new Literal { Text = "<br/><br/>" });

                    pnlDetalles.Controls.Add(new Literal { Text = "Subtítulo 2:<br/>" });
                    pnlDetalles.Controls.Add(new TextBox { ID = "txtValorCurricularSubtitulo2", Text = info.valorCurricular.subtitulo2 ?? "", Width = 400 });
                    pnlDetalles.Controls.Add(new Literal { Text = "<br/><br/>" });

                    pnlDetalles.Controls.Add(new Literal { Text = "Empresa:<br/>" });
                    pnlDetalles.Controls.Add(new TextBox { ID = "txtValorCurricularEmpresa", Text = info.valorCurricular.empresa ?? "", Width = 400 });
                    pnlDetalles.Controls.Add(new Literal { Text = "<br/><br/>" });
                }

                // Perfil de Egreso
                if (info.perfilEgreso != null)
                {
                    pnlDetalles.Controls.Add(new Literal { Text = "<h3>Perfil de Egreso</h3>" });
                    pnlDetalles.Controls.Add(new Literal { Text = "Subtítulo:<br/>" });
                    pnlDetalles.Controls.Add(new TextBox { ID = "txtPerfilEgresoSubtitulo", Text = info.perfilEgreso.subtitulo ?? "", Width = 400 });
                    pnlDetalles.Controls.Add(new Literal { Text = "<br/><br/>" });

                    pnlDetalles.Controls.Add(new Literal { Text = "Descripción:<br/>" });
                    pnlDetalles.Controls.Add(new TextBox { ID = "txPerfilEgresoDescripcion", Text = info.perfilEgreso.descripcion ?? "", Width = 400, TextMode = TextBoxMode.MultiLine, Rows = 3 });
                    pnlDetalles.Controls.Add(new Literal { Text = "<br/><br/>" });
                }

                // Competencias profesionales
                if (info != null && info.competenciasProfesionales != null)
                {
                    var competencias = info.competenciasProfesionales;

                    pnlDetalles.Controls.Add(new Literal { Text = "<h3>Competencias Profesionales</h3>" });

                    // Título de la sección
                    pnlDetalles.Controls.Add(new Literal { Text = "Título de la sección:<br/>" });
                    pnlDetalles.Controls.Add(new TextBox
                    {
                        ID = "txtCompetenciasTitulo",
                        Text = competencias.tituloSeccion ?? "",
                        Width = 400
                    });
                    pnlDetalles.Controls.Add(new Literal { Text = "<br/><br/>" });

                    string categoriaSeleccionada = ViewState["CategoriaCompetenciaSeleccionada"] as string ?? "Conocimientos";


                    var ddlCategoriasCompetencia = new DropDownList
                    {
                        ID = "ddlCategoriaCompetencia",
                        AutoPostBack = true
                    };
                    ddlCategoriasCompetencia.Items.Add(new ListItem("Conocimientos", "Conocimientos"));
                    ddlCategoriasCompetencia.Items.Add(new ListItem("Habilidades", "Habilidades"));
                    ddlCategoriasCompetencia.Items.Add(new ListItem("Actitudes", "Actitudes"));
                    ddlCategoriasCompetencia.Items.Add(new ListItem("Valores", "Valores"));

                    ddlCategoriasCompetencia.SelectedIndexChanged += ddlCompetencias_SelectedIndexChanged;

                    var itemSeleccionado = ddlCategoriasCompetencia.Items.FindByValue(categoriaSeleccionada);
                    if (itemSeleccionado != null) itemSeleccionado.Selected = true;

                    ViewState["tipoCategoriaActual"] = categoriaSeleccionada;

                    pnlDetalles.Controls.Add(new Literal { Text = "<b>Selecciona una competencia:</b><br/>" });
                    pnlDetalles.Controls.Add(ddlCategoriasCompetencia);
                    pnlDetalles.Controls.Add(new Literal { Text = "<br/><br/>" });

                    // === Mostrar elementos de la categoría seleccionada ===
                    List<Conocimiento> lista = null;
                    string prefix = "";

                    switch (categoriaSeleccionada)
                    {
                        case "Conocimientos":
                            lista = competencias.conocimientos;
                            prefix = "txtConocimiento";
                            break;
                        case "Habilidades":
                            lista = competencias.habilidades;
                            prefix = "txtHabilidad";
                            break;
                        case "Actitudes":
                            lista = competencias.actitudes;
                            prefix = "txtActitud";
                            break;
                        case "Valores":
                            lista = competencias.valores;
                            prefix = "txtValor";
                            break;
                    }

                    if (lista != null)
                    {
                        pnlDetalles.Controls.Add(new Literal { Text = $"<b>{categoriaSeleccionada}:</b><br/>" });

                        for (int i = 0; i < lista.Count; i++)
                        {
                            var c = lista[i];

                            // Crear contenedor en línea
                            Panel filaPanel = new Panel();
                            filaPanel.Style.Add("display", "flex");
                            filaPanel.Style.Add("gap", "10px");
                            filaPanel.Style.Add("margin-bottom", "10px");

                            // TextBox principal (ej. Conocimiento)
                            var txtPrincipal = new TextBox
                            {
                                ID = $"{prefix}{i}",
                                Text = c.texto ?? "",
                                Width = 300
                            };
                            filaPanel.Controls.Add(txtPrincipal);

                            // Botón para eliminar esta fila
                            var btnEliminar = new Button
                            {
                                ID = $"btnEliminar_{categoriaSeleccionada}_{i}",
                                Text = "Eliminar",
                                CommandName = "Eliminar",
                                CommandArgument = i.ToString(),
                                CssClass = "btn btn-danger",
                                OnClientClick = "return confirm('¿Estás seguro de eliminar este elemento?');"
                            };
                            btnEliminar.Click += BtnEliminar_Click;
                            filaPanel.Controls.Add(btnEliminar);

                            // Botón para agregar sublista si no existe
                            if (c.sublista == null || c.sublista.Count == 0)
                            {
                                var btnAgregarSub = new Button
                                {
                                    ID = $"btnAgregarSub_{categoriaSeleccionada}_{i}",
                                    Text = "Agregar sublista",
                                    CssClass = "btn btn-info",
                                    CommandArgument = $"{categoriaSeleccionada}_{i}"
                                };
                                btnAgregarSub.Click += BtnAgregarSub_Click;
                                filaPanel.Controls.Add(btnAgregarSub);
                            }

                            // TextBox sublista (multilínea)
                            if (c.sublista != null && c.sublista.Count > 0)
                            {
                                string sublistaTexto = string.Join(Environment.NewLine, c.sublista);
                                var txtSublista = new TextBox
                                {
                                    ID = $"{prefix}{i}_sub",
                                    TextMode = TextBoxMode.MultiLine,
                                    Rows = Math.Max(2, c.sublista.Count),
                                    Width = 250,
                                    Text = sublistaTexto
                                };
                                filaPanel.Controls.Add(txtSublista);

                                // Botón para eliminar la sublista
                                var btnEliminarSub = new Button
                                {
                                    ID = $"btnEliminarSub_{categoriaSeleccionada}_{i}",
                                    Text = "Eliminar sublista",
                                    CssClass = "btn btn-warning",
                                    CommandArgument = $"{categoriaSeleccionada}_{i}"
                                };
                                btnEliminarSub.Click += BtnEliminarSub_Click;
                                filaPanel.Controls.Add(btnEliminarSub);
                            }

                            pnlDetalles.Controls.Add(filaPanel);
                        }
                        // Botón para agregar nueva lista
                        var btnAgregar = new Button
                        {
                            ID = $"btnAgregar_{categoriaSeleccionada}",
                            Text = "Agregar nueva",
                            CommandName = "Agregar",
                            CssClass = "btn btn-success"
                        };
                        btnAgregar.Click += BtnAgregar_Click;

                        pnlDetalles.Controls.Add(new Literal { Text = "<br/>" });
                        pnlDetalles.Controls.Add(btnAgregar);

                    }

                }

                // Titulo
                if (info.titulacion != null)
                {
                    pnlDetalles.Controls.Add(new Literal { Text = "<h3>Titulación</h3>" });

                    pnlDetalles.Controls.Add(new Literal { Text = "Título:<br/>" });
                    pnlDetalles.Controls.Add(new TextBox { ID = "txtTituloTitulacion", Text = info.titulacion.titulo, Width = 400 });
                    pnlDetalles.Controls.Add(new Literal { Text = "<br/><br/>" });

                    pnlDetalles.Controls.Add(new Literal { Text = "Descripción:<br/>" });
                    pnlDetalles.Controls.Add(new TextBox { ID = "txtDescripcionTitulacion", Text = info.titulacion.descripcion, Width = 400, TextMode = TextBoxMode.MultiLine, Rows = 3 });
                    pnlDetalles.Controls.Add(new Literal { Text = "<br/><br/>" });

                    pnlDetalles.Controls.Add(new Literal { Text = "Subtítulo Opciones:<br/>" });
                    pnlDetalles.Controls.Add(new TextBox { ID = "txtSubtituloOpcionesTitulacion", Text = info.titulacion.subtituloOpciones, Width = 400 });
                    pnlDetalles.Controls.Add(new Literal { Text = "<br/><br/>" });

                    if (info.titulacion.opciones != null)
                    {
                        pnlDetalles.Controls.Add(new Literal { Text = "Opciones:<br/>" });
                        for (int i = 0; i < info.titulacion.opciones.Count; i++)
                        {
                            pnlDetalles.Controls.Add(new TextBox { ID = $"txtOpcionTitulacion{i}", Text = info.titulacion.opciones[i], Width = 400 });
                            pnlDetalles.Controls.Add(new Literal { Text = "<br/>" });
                        }
                        pnlDetalles.Controls.Add(new Literal { Text = "<br/>" });
                    }

                    pnlDetalles.Controls.Add(new Literal { Text = "Subtítulo Entrega:<br/>" });
                    pnlDetalles.Controls.Add(new TextBox { ID = "txtSubtituloEntregaTitulacion", Text = info.titulacion.subtituloEntrega, Width = 400 });
                    pnlDetalles.Controls.Add(new Literal { Text = "<br/><br/>" });

                    if (info.titulacion.alFinalizar != null)
                    {
                        pnlDetalles.Controls.Add(new Literal { Text = "Al Finalizar:<br/>" });
                        for (int i = 0; i < info.titulacion.alFinalizar.Count; i++)
                        {
                            pnlDetalles.Controls.Add(new TextBox { ID = $"txtAlFinalizarTitulacion{i}", Text = info.titulacion.alFinalizar[i], Width = 400 });
                            pnlDetalles.Controls.Add(new Literal { Text = "<br/>" });
                        }
                        pnlDetalles.Controls.Add(new Literal { Text = "<br/>" });
                    }
                }

                // Programa Academico
                if (info.programaAcademico != null)
                {
                    pnlDetalles.Controls.Add(new Literal { Text = "<h3>Programa Académico</h3>" });

                    var programaAcademico = info.programaAcademico;
                    if (programaAcademico == null) return;

                    bool tieneSemestres = programaAcademico.semestres != null && programaAcademico.semestres.Count > 0;
                    bool tieneModulos = programaAcademico.modulos != null && programaAcademico.modulos.Count > 0;
                    bool esSemestre = tieneSemestres || !tieneModulos;



                    // Determinar cuántos índices hay que mostrar
                    int totalItems = esSemestre ? programaAcademico.semestres.Count : programaAcademico.modulos.Count;
                    int indiceSeleccionado = 0; // por defecto mostrar el primero

                    // Crear DropDownList para seleccionar semestre o módulo
                    var ddlIndice = new DropDownList
                    {
                        ID = "ddlSeleccionIndice",
                        AutoPostBack = true
                    };

                    for (int i = 0; i < totalItems; i++)
                    {
                        string texto = esSemestre ? $"Semestre {i + 1}" : $"Módulo {i + 1}";
                        ddlIndice.Items.Add(new ListItem(texto, i.ToString()));
                    }

                    // Intentar recuperar selección previa
                    string valorSeleccionado = Request.Form["ddlSeleccionIndice"];
                    if (!string.IsNullOrEmpty(valorSeleccionado) && int.TryParse(valorSeleccionado, out int idx))
                    {
                        if (idx >= 0 && idx < totalItems)
                            indiceSeleccionado = idx;
                    }

                    ddlIndice.SelectedIndex = indiceSeleccionado;

                    // Mostrar DropDownList
                    pnlDetalles.Controls.Add(new Literal
                    {
                        Text = $"<label>Seleccionar {(esSemestre ? "semestre" : "módulo")}:</label><br/>"
                    });
                    pnlDetalles.Controls.Add(ddlIndice);
                    pnlDetalles.Controls.Add(new Literal { Text = "<br/><br/>" });

                    // Botón Agregar
                    var btnAgregar = new Button
                    {
                        ID = "btnAgregarPrograma",
                        Text = esSemestre ? "Agregar Semestre" : "Agregar Módulo",
                        CssClass = "btn btn-success",
                        CommandName = esSemestre ? "AgregarSemestre" : "AgregarModulo"
                    };
                    btnAgregar.Command += BtnPrograma_Command;
                    pnlDetalles.Controls.Add(btnAgregar);
                    pnlDetalles.Controls.Add(new Literal { Text = "<br/><br/>" });

                    if (esSemestre)
                    {
                        if (programaAcademico.semestres == null)
                            programaAcademico.semestres = new List<List<string>>();

                        int i = indiceSeleccionado;

                        var semestre = programaAcademico.semestres[i];
                        pnlDetalles.Controls.Add(new Literal { Text = $"<b>Semestre {i + 1}:</b><br/>" });

                        // Botón Eliminar Semestre
                        var btnEliminar = new Button
                        {
                            ID = $"btnEliminarSemestre_{i}",
                            Text = "Eliminar Semestre",
                            CssClass = "btn btn-danger",
                            CommandName = "EliminarSemestre",
                            CommandArgument = i.ToString()
                        };
                        btnEliminar.Command += BtnPrograma_Command;
                        pnlDetalles.Controls.Add(btnEliminar);
                        pnlDetalles.Controls.Add(new Literal { Text = "<br/>" });

                        for (int j = 0; j < semestre.Count; j++)
                        {
                            var fila = new Panel();
                            fila.Style.Add("display", "flex");
                            fila.Style.Add("gap", "10px");

                            var txtMateria = new TextBox
                            {
                                ID = $"txtSemestre_{i}_Materia_{j}",
                                Text = semestre[j],
                                Width = 300
                            };
                            fila.Controls.Add(txtMateria);

                            var btnEliminarMateria = new Button
                            {
                                ID = $"btnEliminarMateria_{i}_{j}",
                                Text = "Eliminar",
                                CssClass = "btn btn-danger",
                                CommandName = "EliminarMateria",
                                CommandArgument = $"{i}_{j}"
                            };
                            btnEliminarMateria.Command += BtnPrograma_Command;
                            fila.Controls.Add(btnEliminarMateria);

                            pnlDetalles.Controls.Add(fila);
                        }

                        // Botón Agregar Materia
                        var btnAgregarMateria = new Button
                        {
                            ID = $"btnAgregarMateria_{i}",
                            Text = "Agregar materia",
                            CssClass = "btn btn-success",
                            CommandName = "AgregarMateria",
                            CommandArgument = i.ToString()
                        };
                        btnAgregarMateria.Command += BtnPrograma_Command;
                        pnlDetalles.Controls.Add(btnAgregarMateria);
                        pnlDetalles.Controls.Add(new Literal { Text = "<br/><br/>" });

                    }
                    else
                    {
                        if (programaAcademico.modulos == null)
                            programaAcademico.modulos = new List<List<Modulo>>();

                        int i = indiceSeleccionado;

                        var listaModulos = programaAcademico.modulos[i];
                        pnlDetalles.Controls.Add(new Literal { Text = $"<b>Módulo {i + 1}:</b><br/>" });

                        var btnEliminarModulo = new Button
                        {
                            ID = $"btnEliminarModulo_{i}",
                            Text = "Eliminar Módulo",
                            CssClass = "btn btn-danger",
                            CommandName = "EliminarModulo",
                            CommandArgument = i.ToString()
                        };
                        btnEliminarModulo.Command += BtnPrograma_Command;
                        pnlDetalles.Controls.Add(btnEliminarModulo);
                        pnlDetalles.Controls.Add(new Literal { Text = "<br/>" });

                        for (int j = 0; j < listaModulos.Count; j++)
                        {
                            var mod = listaModulos[j];
                            var panelModulo = new Panel
                            {
                                Style =
                                    {
                                        ["border"] = "1px solid #ccc",
                                        ["padding"] = "10px",
                                        ["margin-bottom"] = "10px",
                                        ["border-radius"] = "5px"
                                    }
                            };

                            panelModulo.Controls.Add(new Literal { Text = $"<b>Módulo {j + 1}:</b><br/>" });
                            panelModulo.Controls.Add(new Literal { Text = "Título:<br/>" });
                            panelModulo.Controls.Add(new TextBox
                            {
                                ID = $"txtModuloTitulo_{i}_{j}",
                                Text = mod.titulo ?? "",
                                Width = 300
                            });

                            panelModulo.Controls.Add(new Literal { Text = "<br/>Texto:<br/>" });
                            panelModulo.Controls.Add(new TextBox
                            {
                                ID = $"txtModuloTexto_{i}_{j}",
                                Text = mod.texto ?? "",
                                TextMode = TextBoxMode.MultiLine,
                                Rows = 3,
                                Width = 300
                            });

                            panelModulo.Controls.Add(new Literal { Text = "<br/>Nota:<br/>" });
                            panelModulo.Controls.Add(new TextBox
                            {
                                ID = $"txtModuloNota_{i}_{j}",
                                Text = mod.nota ?? "",
                                Width = 300
                            });

                            var btnEliminarModuloItem = new Button
                            {
                                ID = $"btnEliminarModuloItem_{i}_{j}",
                                Text = "Eliminar módulo",
                                CssClass = "btn btn-danger",
                                CommandName = "EliminarModuloItem",
                                CommandArgument = $"{i}_{j}"
                            };
                            btnEliminarModuloItem.Command += BtnPrograma_Command;
                            panelModulo.Controls.Add(btnEliminarModuloItem);

                            pnlDetalles.Controls.Add(panelModulo);
                        }

                        var btnAgregarModulo = new Button
                        {
                            ID = $"btnAgregarModulo_{i}",
                            Text = "Agregar módulo",
                            CssClass = "btn btn-success",
                            CommandName = "AgregarModuloItem",
                            CommandArgument = i.ToString()
                        };
                        btnAgregarModulo.Command += BtnPrograma_Command;
                        pnlDetalles.Controls.Add(btnAgregarModulo);
                        pnlDetalles.Controls.Add(new Literal { Text = "<br/><br/>" });

                    }
                }

                // Reconocimiento de Validez
                if (info.rvoe != null)
                {
                    pnlDetalles.Controls.Add(new Literal { Text = "<h3>RVOE</h3>" });

                    pnlDetalles.Controls.Add(new Literal { Text = "Título:<br/>" });
                    pnlDetalles.Controls.Add(new TextBox { ID = "txtTituloRvoe", Text = info.rvoe.titulo, Width = 400 });
                    pnlDetalles.Controls.Add(new Literal { Text = "<br/><br/>" });

                    pnlDetalles.Controls.Add(new Literal { Text = "Clave:<br/>" });
                    pnlDetalles.Controls.Add(new TextBox { ID = "txtClaveRvoe", Text = info.rvoe.clave, Width = 400 });
                    pnlDetalles.Controls.Add(new Literal { Text = "<br/>" });
                }
            }
        }

        protected void BtnPrograma_Command(object sender, CommandEventArgs e)
        {
            var programa = Session["programaSeleccionado"] as Programa;
            var info = programa?.contenido?.mensajeRector?.programaAcademico;
            if (info == null) return;

            // 🔧 Inicializar listas si están vacías
            if (info.semestres == null)
                info.semestres = new List<List<string>>();
            if (info.modulos == null)
                info.modulos = new List<List<Modulo>>();

            switch (e.CommandName)
            {
                case "AgregarSemestre":
                    info.semestres.Add(new List<string> { "Nueva materia" });
                    break;
                case "EliminarSemestre":
                    int idxSem = int.Parse(e.CommandArgument.ToString());
                    info.semestres.RemoveAt(idxSem);
                    break;
                case "AgregarMateria":
                    int idxSemMateria = int.Parse(e.CommandArgument.ToString());
                    info.semestres[idxSemMateria].Add("Nueva materia");
                    break;
                case "EliminarMateria":
                    var partes = e.CommandArgument.ToString().Split('_');
                    int i1 = int.Parse(partes[0]);
                    int j1 = int.Parse(partes[1]);
                    info.semestres[i1].RemoveAt(j1);
                    break;
                case "AgregarModulo":
                    info.modulos.Add(new List<Modulo> { new Modulo() });
                    break;
                case "EliminarModulo":
                    int idxMod = int.Parse(e.CommandArgument.ToString());
                    info.modulos.RemoveAt(idxMod);
                    break;
                case "AgregarModuloItem":
                    int idxLista = int.Parse(e.CommandArgument.ToString());
                    info.modulos[idxLista].Add(new Modulo());
                    break;
                case "EliminarModuloItem":
                    var partes2 = e.CommandArgument.ToString().Split('_');
                    int i2 = int.Parse(partes2[0]);
                    int j2 = int.Parse(partes2[1]);
                    info.modulos[i2].RemoveAt(j2);
                    break;
            }

            Session["programaSeleccionado"] = programa;
            MostrarDetallesPrograma(programa);
        }

        private Programa ObtenerProgramaActual()
        {
            datos = Session["datos"] as CampusData;
            if (datos == null) return null;

            if (!datos.campus.ContainsKey(campusActual))
                return null;

            var campus = datos.campus[campusActual];

            if (!int.TryParse(ddlMenu.SelectedValue, out int menuIndex) || menuIndex < 0 || menuIndex >= campus.menu.Count)
                return null;

            var menu = campus.menu[menuIndex];

            var ddlCategorias = pnlEditorDinamico.FindControl("ddlCategorias") as DropDownList;
            if (ddlCategorias == null || !int.TryParse(ddlCategorias.SelectedValue, out int categoriaIndex))
                return null;

            if (categoriaIndex < 0 || categoriaIndex >= menu.categorias.Count)
                return null;

            var categoria = menu.categorias[categoriaIndex];

            var ddlProgramas = pnlEditorDinamico.FindControl("ddlProgramas") as DropDownList;
            if (ddlProgramas == null || !int.TryParse(ddlProgramas.SelectedValue, out int programaIndex))
                return null;

            if (programaIndex < 0 || programaIndex >= categoria.programas.Count)
                return null;

            return categoria.programas[programaIndex];
        }

        private void GuardarEnSesion(Programa programaActualizado)
        {
            datos = Session["datos"] as CampusData;
            if (datos == null) return;

            if (!datos.campus.ContainsKey(campusActual))
                return;

            var campus = datos.campus[campusActual];

            if (!int.TryParse(ddlMenu.SelectedValue, out int menuIndex) || menuIndex < 0 || menuIndex >= campus.menu.Count)
                return;

            var menu = campus.menu[menuIndex];

            var ddlCategorias = pnlEditorDinamico.FindControl("ddlCategorias") as DropDownList;
            if (ddlCategorias == null || !int.TryParse(ddlCategorias.SelectedValue, out int categoriaIndex))
                return;

            if (categoriaIndex < 0 || categoriaIndex >= menu.categorias.Count)
                return;

            var categoria = menu.categorias[categoriaIndex];

            var ddlProgramas = pnlEditorDinamico.FindControl("ddlProgramas") as DropDownList;
            if (ddlProgramas == null || !int.TryParse(ddlProgramas.SelectedValue, out int programaIndex))
                return;

            if (programaIndex < 0 || programaIndex >= categoria.programas.Count)
                return;

            // Actualiza el programa en la lista
            categoria.programas[programaIndex] = programaActualizado;

            // Guarda de nuevo en sesión
            Session["datos"] = datos;
        }

        private IEnumerable<TextBox> BuscarTextBoxes(Control parent, string prefijo)
        {
            foreach (Control ctl in parent.Controls)
            {
                if (ctl is TextBox txt && txt.ID.StartsWith(prefijo) && !txt.ID.EndsWith("_sub"))
                {
                    yield return txt;
                }
                else if (ctl.HasControls())
                {
                    foreach (var inner in BuscarTextBoxes(ctl, prefijo))
                        yield return inner;
                }
            }
        }

        protected void BtnAgregar_Click(object sender, EventArgs e)
        {
            var btn = sender as Button;
            string tipo = btn.ID.Split('_')[1];

            // === Recuperar índices actuales ===
            datos = Session["datos"] as CampusData;
            if (!datos.campus.ContainsKey(campusActual)) return;

            var campus = datos.campus[campusActual];
            int menuIdx = int.Parse(ddlMenu.SelectedValue);
            int catIdx = int.Parse((pnlEditorDinamico.FindControl("ddlCategorias") as DropDownList)?.SelectedValue ?? "-1");
            int progIdx = int.Parse((pnlEditorDinamico.FindControl("ddlProgramas") as DropDownList)?.SelectedValue ?? "-1");

            var menu = campus.menu[menuIdx];

            GuardarPrograma(menu, catIdx, progIdx);

            var programa = menu.categorias[catIdx].programas[progIdx];
            var competencias = programa.contenido?.mensajeRector?.competenciasProfesionales;
            if (competencias == null) return;

            switch (tipo)
            {
                case "Conocimientos":
                    competencias.conocimientos.Add(new Conocimiento { texto = "", sublista = new List<string>() });
                    break;
                case "Habilidades":
                    competencias.habilidades.Add(new Conocimiento { texto = "", sublista = new List<string>() });
                    break;
                case "Actitudes":
                    competencias.actitudes.Add(new Conocimiento { texto = "", sublista = new List<string>() });
                    break;
                case "Valores":
                    competencias.valores.Add(new Conocimiento { texto = "", sublista = new List<string>() });
                    break;
            }

            GuardarEnSesion(programa); // Guarda cambios en sesión
            MostrarDetallesPrograma(programa); // Redibuja los controles
        }

        protected void BtnEliminar_Click(object sender, EventArgs e)
        {
            var btn = sender as Button;
            string[] partes = btn.ID.Split('_');
            string tipo = partes[1];
            int index = int.Parse(partes[2]);

            datos = Session["datos"] as CampusData;
            if (!datos.campus.ContainsKey(campusActual)) return;

            var campus = datos.campus[campusActual];
            int menuIdx = int.Parse(ddlMenu.SelectedValue);
            int catIdx = int.Parse((pnlEditorDinamico.FindControl("ddlCategorias") as DropDownList)?.SelectedValue ?? "-1");
            int progIdx = int.Parse((pnlEditorDinamico.FindControl("ddlProgramas") as DropDownList)?.SelectedValue ?? "-1");

            var menu = campus.menu[menuIdx];


            GuardarPrograma(menu, catIdx, progIdx);

            var programa = menu.categorias[catIdx].programas[progIdx];
            var competencias = programa.contenido?.mensajeRector?.competenciasProfesionales;
            if (competencias == null) return;

            switch (tipo)
            {
                case "Conocimientos":
                    if (index >= 0 && index < competencias.conocimientos.Count)
                        competencias.conocimientos.RemoveAt(index);
                    break;
                case "Habilidades":
                    if (index >= 0 && index < competencias.habilidades.Count)
                        competencias.habilidades.RemoveAt(index);
                    break;
                case "Actitudes":
                    if (index >= 0 && index < competencias.actitudes.Count)
                        competencias.actitudes.RemoveAt(index);
                    break;
                case "Valores":
                    if (index >= 0 && index < competencias.valores.Count)
                        competencias.valores.RemoveAt(index);
                    break;
            }

            GuardarEnSesion(programa);
            MostrarDetallesPrograma(programa); // Recarga controles
        }

        protected void BtnAgregarSub_Click(object sender, EventArgs e)
        {
            var btn = sender as Button;
            if (btn == null || string.IsNullOrEmpty(btn.CommandArgument)) return;

            // Recuperar índices
            string[] partes = btn.CommandArgument.Split('_');
            string tipo = partes[0];
            int index = int.Parse(partes[1]);

            datos = Session["datos"] as CampusData;
            if (!datos.campus.ContainsKey(campusActual)) return;

            var campus = datos.campus[campusActual];
            int menuIdx = int.Parse(ddlMenu.SelectedValue);
            int catIdx = int.Parse((pnlEditorDinamico.FindControl("ddlCategorias") as DropDownList)?.SelectedValue ?? "-1");
            int progIdx = int.Parse((pnlEditorDinamico.FindControl("ddlProgramas") as DropDownList)?.SelectedValue ?? "-1");

            var programa = campus.menu[menuIdx].categorias[catIdx].programas[progIdx];
            var competencias = programa.contenido?.mensajeRector?.competenciasProfesionales;
            if (competencias == null) return;

            List<Conocimiento> lista = null;
            switch (tipo)
            {
                case "Conocimientos":
                    lista = competencias.conocimientos;
                    break;
                case "Habilidades":
                    lista = competencias.habilidades;
                    break;
                case "Actitudes":
                    lista = competencias.actitudes;
                    break;
                case "Valores":
                    lista = competencias.valores;
                    break;
            }

            if (lista != null && index >= 0 && index < lista.Count)
            {
                if (lista[index].sublista == null || lista[index].sublista.Count == 0)
                    lista[index].sublista = new List<string> { "" }; // Se agrega una sublista vacía con un elemento inicial
            }

            GuardarEnSesion(programa);
            MostrarDetallesPrograma(programa); // Refresca la UI
        }

        protected void BtnEliminarSub_Click(object sender, EventArgs e)
        {
            var btn = sender as Button;
            if (btn == null || string.IsNullOrEmpty(btn.CommandArgument)) return;

            string[] partes = btn.CommandArgument.Split('_');
            string tipo = partes[0];
            int index = int.Parse(partes[1]);

            datos = Session["datos"] as CampusData;
            if (!datos.campus.ContainsKey(campusActual)) return;

            var campus = datos.campus[campusActual];
            int menuIdx = int.Parse(ddlMenu.SelectedValue);
            int catIdx = int.Parse((pnlEditorDinamico.FindControl("ddlCategorias") as DropDownList)?.SelectedValue ?? "-1");
            int progIdx = int.Parse((pnlEditorDinamico.FindControl("ddlProgramas") as DropDownList)?.SelectedValue ?? "-1");

            var programa = campus.menu[menuIdx].categorias[catIdx].programas[progIdx];
            var competencias = programa.contenido?.mensajeRector?.competenciasProfesionales;
            if (competencias == null) return;

            List<Conocimiento> lista = null;
            switch (tipo)
            {
                case "Conocimientos":
                    lista = competencias.conocimientos;
                    break;
                case "Habilidades":
                    lista = competencias.habilidades;
                    break;
                case "Actitudes":
                    lista = competencias.actitudes;
                    break;
                case "Valores":
                    lista = competencias.valores;
                    break;
            }

            if (lista != null && index >= 0 && index < lista.Count)
            {
                lista[index].sublista = null;
            }

            GuardarEnSesion(programa);
            MostrarDetallesPrograma(programa); // Refresca la UI
        }

        private void GuardarPrograma(MenuItemm menu, int catIdx, int progIdx)
        {
            var categoria = menu.categorias[catIdx];
            var programa = categoria.programas[progIdx];

            string carpetaCampus = $"fotos/{campusActual}/";

            // === Buscar controles principales ===
            var txtNombre = pnlEditorDinamico.FindControl("txtNombrePrograma") as TextBox;
            var txtTipo = pnlEditorDinamico.FindControl("txtTipoPrograma") as TextBox;
            var fuImagenPrograma = pnlEditorDinamico.FindControl("fuImagenPrograma") as FileUpload;
            var fuImagenBanner = pnlEditorDinamico.FindControl("fuImagenBanner") as FileUpload;

            if (txtNombre != null) programa.nombre = txtNombre.Text;
            if (txtTipo != null) programa.tipo = txtTipo.Text;

            // Obtén la ruta física del frontend desde el web.config
            string frontendPath = ConfigurationManager.AppSettings["FrontendPhysicalPath"];

            // === Guardar imagen principal ===
            if (fuImagenPrograma != null && fuImagenPrograma.HasFile)
            {
                string nombreArchivo = Path.GetFileName(fuImagenPrograma.FileName);

                // 💡 Corrección: Usa Path.Combine para crear la ruta física completa
                // Asume que 'carpetaCampus' es 'vistas/fotos/Monterrey/'
                string rutaFisica = Path.Combine(frontendPath, carpetaCampus, nombreArchivo);
                string rutaJson = carpetaCampus + nombreArchivo; // Esta ruta es la que se guardará en el JSON

                // Asegúrate de que la carpeta exista antes de guardar
                Directory.CreateDirectory(Path.GetDirectoryName(rutaFisica));

                fuImagenPrograma.SaveAs(rutaFisica);
                programa.imagen = rutaJson.Replace("\\", "/");
            }

            // === Guardar imagen banner ===
            if (fuImagenBanner != null && fuImagenBanner.HasFile)
            {
                string nombreArchivo = Path.GetFileName(fuImagenBanner.FileName);

                // 💡 Corrección: Usa Path.Combine para crear la ruta física completa
                // Asume que la ruta es 'vistas/fotos/Monterrey/...'
                string rutaFisica = Path.Combine(frontendPath, carpetaCampus, nombreArchivo);
                string rutaJson = carpetaCampus + nombreArchivo; // La ruta que se guardará en el JSON

                // Asegúrate de que la carpeta exista antes de guardar
                Directory.CreateDirectory(Path.GetDirectoryName(rutaFisica));

                if (programa.contenido == null)
                    programa.contenido = new ContenidoPrograma();

                programa.contenido.imagenBanner = rutaJson.Replace("\\", "/");

                fuImagenBanner.SaveAs(rutaFisica);
            }

            // === Validar contenido ===
            var info = programa.contenido?.mensajeRector;
            if (info == null)
                return;

            // === Guardar campos simples ===
            info.titulo = (pnlEditorDinamico.FindControl("txtTitulo") as TextBox)?.Text ?? "";
            info.subtitulo = (pnlEditorDinamico.FindControl("txtSubtitulo") as TextBox)?.Text ?? "";
            info.inicio = (pnlEditorDinamico.FindControl("txtInicio") as TextBox)?.Text ?? "";
            info.duracion = (pnlEditorDinamico.FindControl("txtDuracion") as TextBox)?.Text ?? "";
            info.modalidad = (pnlEditorDinamico.FindControl("txtModalidad") as TextBox)?.Text ?? "";
            info.horarios = (pnlEditorDinamico.FindControl("txtHorarios") as TextBox)?.Text ?? "";
            info.dirigidoA = (pnlEditorDinamico.FindControl("txtDirigidoA") as TextBox)?.Text ?? "";

            // === Universidad ===
            if (info.universidad != null)
            {
                info.universidad.nombre = (pnlEditorDinamico.FindControl("txtUniversidadNombre") as TextBox)?.Text ?? "";
                info.universidad.descripcion = (pnlEditorDinamico.FindControl("txtUniversidadDescripcion") as TextBox)?.Text ?? "";
            }

            // === Objetivo General ===
            if (info.objetivoGeneral != null)
            {
                info.objetivoGeneral.subtitulo = (pnlEditorDinamico.FindControl("txtObjetivoGeneralSubtitulo") as TextBox)?.Text ?? "";
                info.objetivoGeneral.descripcion = (pnlEditorDinamico.FindControl("txtObjetivoGeneralDescripcion") as TextBox)?.Text ?? "";
            }

            // === Plan de Estudios ===
            if (info.planEstudios != null)
            {
                info.planEstudios.subtitulo = (pnlEditorDinamico.FindControl("txtPlanEstudiosSubtitulo") as TextBox)?.Text ?? "";
                info.planEstudios.diploma = (pnlEditorDinamico.FindControl("txtPlanEstudiosDiploma") as TextBox)?.Text ?? "";
            }

            // === Objetivo Específico ===
            if (info.objetivoEspecifico != null)
            {
                info.objetivoEspecifico.subtitulo = (pnlEditorDinamico.FindControl("txtObjetivoEspecificoSubtitulo") as TextBox)?.Text ?? "";

                var lista = new List<string>();
                for (int i = 0; i < info.objetivoEspecifico.lista?.Count; i++)
                {
                    var txtItem = pnlEditorDinamico.FindControl($"txtObjetivoEspecificoItem{i}") as TextBox;
                    if (txtItem != null) lista.Add(txtItem.Text);
                }
                info.objetivoEspecifico.lista = lista;
            }

            // === Valor Curricular ===
            if (info.valorCurricular != null)
            {
                info.valorCurricular.subtitulo = (pnlEditorDinamico.FindControl("txtValorCurricularSubtitulo") as TextBox)?.Text ?? "";
                info.valorCurricular.descripcion = (pnlEditorDinamico.FindControl("txtValorCurricularDescripcion") as TextBox)?.Text ?? "";
                info.valorCurricular.subtitulo2 = (pnlEditorDinamico.FindControl("txtValorCurricularSubtitulo2") as TextBox)?.Text ?? "";
                info.valorCurricular.empresa = (pnlEditorDinamico.FindControl("txtValorCurricularEmpresa") as TextBox)?.Text ?? "";
            }

            // === Perfil de Egreso ===
            if (info.perfilEgreso != null)
            {
                info.perfilEgreso.subtitulo = (pnlEditorDinamico.FindControl("txtPerfilEgresoSubtitulo") as TextBox)?.Text ?? "";
                info.perfilEgreso.descripcion = (pnlEditorDinamico.FindControl("txPerfilEgresoDescripcion") as TextBox)?.Text ?? "";
            }

            if (info.competenciasProfesionales != null)
            {
                var competencias = info.competenciasProfesionales;

                // Guardar el título general de la sección
                competencias.tituloSeccion = (pnlEditorDinamico.FindControl("txtCompetenciasTitulo") as TextBox)?.Text ?? "";

                var categorias = new Dictionary<string, string>
                {
                    { "conocimientos", "txtConocimiento" },
                    { "habilidades", "txtHabilidad" },
                    { "actitudes", "txtActitud" },
                    { "valores", "txtValor" }
                };

                var pnlDetalles = pnlEditorDinamico.FindControl("pnlDetallesPrograma") as Panel;
                if (pnlDetalles == null) return;

                // Detectar la categoría actual (la visible que el usuario editó)
                string categoriaSeleccionada = ViewState["CategoriaCompetenciaSeleccionada"] as string ?? "Conocimientos";
                string claveCategoria = categoriaSeleccionada.ToLower();

                if (!categorias.TryGetValue(claveCategoria, out string prefijoActual))
                    return; // Seguridad: si el valor es inválido, salta

                var listaNueva = new List<Conocimiento>();
                foreach (var txt in BuscarTextBoxes(pnlDetalles, prefijoActual))
                {
                    string texto = txt.Text.Trim();
                    if (!string.IsNullOrWhiteSpace(texto))
                    {
                        var txtSub = pnlDetalles.FindControl(txt.ID + "_sub") as TextBox;
                        var sublista = new List<string>();

                        if (txtSub != null && !string.IsNullOrWhiteSpace(txtSub.Text))
                        {
                            sublista = txtSub.Text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries)
                                                  .Select(p => p.Trim())
                                                  .ToList();
                        }

                        listaNueva.Add(new Conocimiento
                        {
                            texto = texto,
                            sublista = sublista.Count > 0 ? sublista : null
                        });
                    }
                }

                // Reemplaza solo la lista correspondiente
                switch (claveCategoria)
                {
                    case "conocimientos":
                        competencias.conocimientos = listaNueva;
                        break;
                    case "habilidades":
                        competencias.habilidades = listaNueva;
                        break;
                    case "actitudes":
                        competencias.actitudes = listaNueva;
                        break;
                    case "valores":
                        competencias.valores = listaNueva;
                        break;
                }
            }

            GuardarEnSesion(programa);

            // === Titulación ===
            if (info.titulacion != null)
            {
                info.titulacion.titulo = (pnlEditorDinamico.FindControl("txtTituloTitulacion") as TextBox)?.Text ?? "";
                info.titulacion.descripcion = (pnlEditorDinamico.FindControl("txtDescripcionTitulacion") as TextBox)?.Text ?? "";
                info.titulacion.subtituloOpciones = (pnlEditorDinamico.FindControl("txtSubtituloOpcionesTitulacion") as TextBox)?.Text ?? "";
                info.titulacion.subtituloEntrega = (pnlEditorDinamico.FindControl("txtSubtituloEntregaTitulacion") as TextBox)?.Text ?? "";

                // Opciones
                var opciones = new List<string>();
                for (int i = 0; i < info.titulacion.opciones?.Count; i++)
                {
                    var txt = pnlEditorDinamico.FindControl($"txtOpcionTitulacion{i}") as TextBox;
                    if (txt != null) opciones.Add(txt.Text);
                }
                info.titulacion.opciones = opciones;

                // Al Finalizar
                var finalizar = new List<string>();
                for (int i = 0; i < info.titulacion.alFinalizar?.Count; i++)
                {
                    var txt = pnlEditorDinamico.FindControl($"txtAlFinalizarTitulacion{i}") as TextBox;
                    if (txt != null) finalizar.Add(txt.Text);
                }
                info.titulacion.alFinalizar = finalizar;
            }

            // === Programa Académico ===
            if (info.programaAcademico != null)
            {
                var programaAcademico = info.programaAcademico;

                bool esSemestre = programaAcademico.semestres != null &&
                                  (programaAcademico.modulos == null || programaAcademico.modulos.Count == 0);

                // Obtener índice del DropDownList dinámico
                var ddlIndice = pnlEditorDinamico.FindControl("ddlSeleccionIndice") as DropDownList;
                if (ddlIndice == null || !int.TryParse(ddlIndice.SelectedValue, out int indice)) return;

                if (esSemestre)
                {
                    if (programaAcademico.semestres == null || indice >= programaAcademico.semestres.Count) return;

                    var materias = programaAcademico.semestres[indice];
                    for (int j = 0; j < materias.Count; j++)
                    {
                        var txtMateria = pnlEditorDinamico.FindControl($"txtSemestre_{indice}_Materia_{j}") as TextBox;
                        if (txtMateria != null)
                            materias[j] = txtMateria.Text.Trim();
                    }

                    // 🧼 Limpiar módulos si no se usan
                    programaAcademico.modulos = null;

                    // Opcional: eliminar semestres si están vacíos
                    if (programaAcademico.semestres.All(s => s.All(string.IsNullOrWhiteSpace)))
                    {
                        programaAcademico.semestres = null;
                    }
                }
                else
                {
                    if (programaAcademico.modulos == null || indice >= programaAcademico.modulos.Count) return;

                    var modulos = programaAcademico.modulos[indice];
                    for (int j = 0; j < modulos.Count; j++)
                    {
                        var titulo = (pnlEditorDinamico.FindControl($"txtModuloTitulo_{indice}_{j}") as TextBox)?.Text.Trim() ?? "";
                        var texto = (pnlEditorDinamico.FindControl($"txtModuloTexto_{indice}_{j}") as TextBox)?.Text.Trim() ?? "";
                        var nota = (pnlEditorDinamico.FindControl($"txtModuloNota_{indice}_{j}") as TextBox)?.Text.Trim() ?? "";

                        modulos[j].titulo = titulo;
                        modulos[j].texto = texto;
                        modulos[j].nota = nota;
                    }

                    // 🧼 Limpiar semestres si no se usan
                    programaAcademico.semestres = null;

                    // Opcional: eliminar modulos si están vacíos
                    if (programaAcademico.modulos.All(grupo => grupo.All(mod =>
                        string.IsNullOrWhiteSpace(mod.titulo) &&
                        string.IsNullOrWhiteSpace(mod.texto) &&
                        string.IsNullOrWhiteSpace(mod.nota) &&
                        (mod.sublista == null || mod.sublista.All(string.IsNullOrWhiteSpace))
                    )))
                    {
                        programaAcademico.modulos = null;
                    }
                }
            }


            // === RVOE ===
            if (info.rvoe != null)
            {
                info.rvoe.titulo = (pnlEditorDinamico.FindControl("txtTituloRvoe") as TextBox)?.Text ?? "";
                info.rvoe.clave = (pnlEditorDinamico.FindControl("txtClaveRvoe") as TextBox)?.Text ?? "";
            }

        }
        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(ddlMenu.SelectedValue, out int menuIndex))
            {
                lblMensaje.Text = "Selecciona un menú válido.";
                lblMensaje.ForeColor = System.Drawing.Color.Red;
                return;
            }

            var menu = datos.campus[campusActual].menu[menuIndex];

            // 🧠 Recuperar índice de la opción simple seleccionada
            int opcionIndex = int.TryParse(ViewState["ddlOpcionesSimples"] as string, out int idx) ? idx : 0;

            // ✅ Guardar los cambios realizados en controles dinámicos de programas
            GuardarOpcionSimple(menu, opcionIndex);

            // ==========================
            // 🛠️ Declarar variables antes
            // ==========================
            var ddlCategorias = pnlEditorDinamico.FindControl("ddlCategorias") as DropDownList;
            var ddlProgramas = pnlEditorDinamico.FindControl("ddlProgramas") as DropDownList;

            int catIdx = -1;
            int progIdx = -1;

            bool modoPrograma = ddlCategorias != null && ddlProgramas != null &&
                                int.TryParse(ddlCategorias.SelectedValue, out catIdx) &&
                                int.TryParse(ddlProgramas.SelectedValue, out progIdx);

            // ===============================
            // 💾 Guardar archivo datos.json
            // ===============================
            var settings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore
            };

            File.WriteAllText(RutaJson, JsonConvert.SerializeObject(datos, settings));
            lblMensaje.Text = "Cambios guardados correctamente.";
            lblMensaje.ForeColor = System.Drawing.Color.Green;
        }

    }
}