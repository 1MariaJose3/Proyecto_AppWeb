<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EditarCampus.aspx.cs" Inherits="PaginaMaestra.EditarCampus" ValidateRequest="false" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <link rel="stylesheet" type="text/css" href="styles/style.css" />
</head>
<body>
    <form id="form1" runat="server">
        <div class="seccion">
            <h2>Editar Información de Ep de Mexico</h2>
            <!-- Selección de Campus -->
            <asp:Label Text="Selecciona el Campus a Modificar" runat="server" AssociatedControlID="ListaCampus" />
            <br />
            <asp:DropDownList ID="ListaCampus" runat="server" AutoPostBack="true" OnSelectedIndexChanged="Seleccionar_Campus">
                <asp:ListItem Text="Monterrey" Value="Monterrey" />
                <asp:ListItem Text="Puebla" Value="Puebla" />
                <asp:ListItem Text="Guadalajara" Value="Guadalajara" />
                <asp:ListItem Text="Merida" Value="Merida" />
                <asp:ListItem Text="Virtual" Value="Virtual" />
            </asp:DropDownList>
            <br />
            <br />

            <!-- Título -->
            <asp:Label Text="Título de la página:" runat="server" AssociatedControlID="txtTitulo" />
            <br />
            <asp:TextBox ID="txtTitulo" runat="server" Width="400px" />
        </div>

        <!-- Redes Flotantes -->
        <div class="seccion">
            <h2>Redes Flotantes</h2>
            <asp:Repeater ID="rptBarraFlotante" runat="server" OnItemCommand="rptBarraFlotante_ItemCommand">
                <ItemTemplate>
                    <div style="margin-bottom: 10px;">
                        <!-- Mostrar el nombre del botón sin permitir edición -->
                        <asp:Label ID="lblImagenBarraFlotante" runat="server" Text='<%# Eval("imagen") %>' Width="200px" />

                        <!-- Enlace sí editable -->
                        <asp:TextBox ID="txtenlaceBarraFlotante" runat="server" Text='<%# Eval("enlace") %>' Width="500px" />

                        <!-- Botón para eliminar -->
                        <asp:Button ID="btnEliminarPlataforma" runat="server" Text="Eliminar" CommandName="Eliminar" CommandArgument='<%# Container.ItemIndex %>' />
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>


        <!-- Horarior de Atención  -->
        <div class="seccion">
            <h2>Horario de Atención</h2>
            <asp:Label Text="Lunes a Viernes:" runat="server" />
            <asp:TextBox ID="txtLunesViernes" runat="server" Width="300px" /><br />
            <asp:Label Text="Sábado:" runat="server" />
            <asp:TextBox ID="txtSabado" runat="server" Width="300px" /><br />
            <asp:Label Text="Informes:" runat="server" />
            <asp:TextBox ID="txtInformes" runat="server" Width="300px" /><br />
            <asp:Label Text="Soporte (uno por línea):" runat="server" />
            <asp:TextBox ID="txtSoporte" runat="server" TextMode="MultiLine" Rows="3" Width="300px" /><br />
            <asp:Label Text="Diplomados:" runat="server" />
            <asp:TextBox ID="txtDiplomados" runat="server" Width="300px" /><br />
        </div>

        <div class="seccion">
            <h2>Menu</h2>
            <!-- Logos -->
            <asp:Label Text="Selecciona el logo a reemplazar:" runat="server" AssociatedControlID="Logos" />
            <br />
            <asp:DropDownList ID="Logos" runat="server" AutoPostBack="true" OnSelectedIndexChanged="Logos_SelectedIndexChanged" />
            <br />
            <asp:Image ID="imgPreview" runat="server" Width="100px" Style="background-color: #00264d;" />
            <br />
            <asp:Label Text="Selecciona la imagen nueva:" runat="server" AssociatedControlID="SubirLogo" />
            <asp:FileUpload ID="SubirLogo" runat="server" onchange="mostrarVistaPrevia(this)" />
            <br />
            <asp:Label ID="lblNombre" runat="server" Text="Nuevo nombre (ej. nuevoLogo.png):" AssociatedControlID="txtNuevoNombre" />
            <asp:TextBox ID="txtNuevoNombre" runat="server" />
            <asp:Button ID="btnSubirImagen" runat="server" Text="Subir Imagen" OnClick="btnSubirImagen_Click" />
            <asp:Label ID="lblresultadoimagen" runat="server" ForeColor="Green" />


            <!-- menú principal -->
            <h3>Agregar nuevo menú</h3>
            <asp:DropDownList ID="ddlTipoMenu" runat="server">
                <asp:ListItem Text="Submenu de opcion simple" Value="opciones" />
                <asp:ListItem Text="Submenu con categorías y programas" Value="categorias" />
                <asp:ListItem Text="Enlace directo" Value="href" />
            </asp:DropDownList>
            <br />

            <asp:TextBox ID="txtNuevoTituloMenu" runat="server" Placeholder="título del nuevo menú" Width="300px" />
            <asp:Button ID="btnAgregarNuevoMenu" runat="server" Text="Agregar Nuevo Menú" OnClick="btnAgregarNuevoMenu_Click" />
            <br />
            <br />

            <!-- selección del menú -->
            <h3>Seleccionar el menú a modificar</h3>
            <asp:Label runat="server" Text="selecciona un menú:" AssociatedControlID="ddlMenu" />
            <asp:DropDownList ID="ddlMenu" runat="server" AutoPostBack="true" OnSelectedIndexChanged="Seleccionar_Menu" />
            <asp:Button ID="btnEliminarMenu" runat="server" Text="Eliminar Menu" OnClick="btnEliminarMenu_Click" />
            <br />
            <br />

            <!-- Panel: opciones simples -->
            <asp:Panel ID="pnlOpciones" runat="server" Visible="false">
                <!-- Repeater opciones existentes -->
                <asp:Repeater ID="rptOpciones" runat="server">
                    <ItemTemplate>
                        <div style="margin-bottom: 10px;">
                            <asp:TextBox ID="txtTituloOpcion" runat="server"
                                Text='<%# Eval("titulo") %>' Width="300px"
                                Placeholder="Título" />
                            <asp:TextBox ID="txtEnlaceOpcion" runat="server"
                                Text='<%# Eval("enlace") %>' Width="300px"
                                Placeholder="Enlace" />
                            <asp:Button ID="btnEliminarOpcion" runat="server" Text="Eliminar"
                                CommandName="Eliminar" CommandArgument='<%# Container.ItemIndex %>'
                                OnCommand="btnEliminarOpcion_Command" />
                        </div>
                    </ItemTemplate>
                </asp:Repeater>

                <!-- Agregar nueva opción -->
                <asp:TextBox ID="txtNuevaOpcion" runat="server" Placeholder="Nuevo título de opción" Width="300px" />
                <asp:TextBox ID="TextBox1" runat="server" Placeholder="Nuevo enlace" Width="300px" />

                <!-- Dropdown para seleccionar vista/estructura -->
                <asp:DropDownList ID="ddlSeleccionVista" runat="server" />

                <asp:Button ID="btnAgregarOpcion" runat="server" Text="Agregar Opción" OnClick="btnAgregarOpcion_Click" />
            </asp:Panel>

            <!-- Panel: categorías y programas -->
            <asp:Panel ID="pnlCategorias" runat="server" Visible="false">
                <asp:Repeater ID="rptCategorias" runat="server" OnItemCommand="rptCategorias_ItemCommand">
                    <ItemTemplate>
                        <!-- Nombre de la categoría -->
                        <asp:TextBox ID="txtCategoria" runat="server" Text='<%# Eval("nombre") %>' Width="300px" />
                        <asp:Button ID="btnEliminarCategoria" runat="server" Text="Eliminar categoría"
                            CommandName="eliminarcategoria" CommandArgument='<%# Container.ItemIndex %>' />
                        <br />

                        <!-- Repeater de programas -->
                        <asp:Repeater ID="rptProgramas" runat="server" DataSource='<%# Eval("programas") %>' OnItemCommand="rptProgramas_ItemCommand">
                            <ItemTemplate>
                                <asp:TextBox ID="txtPrograma" runat="server" Text='<%# Eval("nombre") %>' Width="300px" />
                                <asp:Button ID="btnEliminarPrograma" runat="server" Text="Eliminar programa"
                                    CommandName="eliminarprograma" CommandArgument='<%# Container.ItemIndex %>' />
                                <br />
                            </ItemTemplate>
                        </asp:Repeater>


                        <!-- Agregar nuevo programa -->
                        <asp:TextBox ID="txtNuevoPrograma" runat="server" Placeholder="Nuevo programa" Width="300px" />
                        <asp:DropDownList ID="ddlModeloPrograma" runat="server" />
                        <asp:Button ID="btnAgregarPrograma" runat="server" Text="Agregar programa"
                            CommandName="agregarprograma" CommandArgument='<%# Container.ItemIndex %>' />
                        <hr />
                    </ItemTemplate>
                </asp:Repeater>

                <!-- Agregar nueva categoría -->
                <asp:TextBox ID="txtNuevaCategoria" runat="server" Placeholder="Nueva categoría" Width="300px" />
                <asp:Button ID="btnAgregarCategoria" runat="server" Text="Agregar categoría" OnClick="btnAgregarCategoria_Click" />
            </asp:Panel>

            <!-- Panel: href directo -->
            <asp:Panel ID="pnlHref" runat="server" Visible="false">
                <h3>Editar enlace</h3>
                <asp:TextBox ID="txtHref" runat="server" Width="400px" Placeholder="enlace (url)" />
            </asp:Panel>

            <!-- Redes Sociales -->
            <h3>Redes Sociales</h3>
            <asp:Repeater ID="rptRedesSociales" runat="server" OnItemCommand="rptRedesSociales_ItemCommand">
                <ItemTemplate>
                    <b>Nombre:</b>
                    <asp:TextBox ID="txtNombreRed" runat="server" Text='<%# Eval("nombre") %>' Width="150px" />
                    <b>URL:</b>
                    <asp:TextBox ID="txtUrlRed" runat="server" Text='<%# Eval("url") %>' Width="400px" />
                    <asp:Button ID="btnEliminarIndividual" runat="server" Text="Eliminar" CommandName="EliminarRed" CommandArgument='<%# Container.ItemIndex %>' />
                    <asp:HiddenField ID="hdnIdRed" runat="server" Value='<%# Container.ItemIndex %>' />
                    <br />
                    <br />
                </ItemTemplate>
            </asp:Repeater>

            <!-- Agregar nueva red -->
            <b>Nombre:</b>
            <asp:TextBox ID="txtNuevaRedNombre" runat="server" Placeholder="Nombre de la red" Width="150px" />
            <b>URL:</b>
            <asp:TextBox ID="txtNuevaRedUrl" runat="server" Placeholder="URL de la red social" Width="400px" />
            <asp:Button ID="btnAgregarRed" runat="server" Text="Agregar red social" OnClick="btnAgregarRed_Click" />
        </div>


        <div class="seccion">
            <!-- video -->
            <h2>Video</h2>
            <video id="videoActual" runat="server" width="320" height="240" controls>
                <source id="srcVideo" runat="server" type="video/mp4" />
                Tu navegador no soporta el elemento de video.
            </video>
            <br />
            <asp:Label Text="Selecciona el nuevo video:" runat="server" AssociatedControlID="fileupload1" />
            <asp:FileUpload ID="fileupload1" runat="server" />
            <br />
            <asp:Label ID="label1" runat="server" Text="Nuevo nombre (ej. nuevovideo.mp4):" AssociatedControlID="txtnuevonombrevideo" />
            <asp:TextBox ID="txtnuevonombrevideo" runat="server" placeholder="nombre del nuevo video" Width="300px" />
            <asp:Button ID="btnSubirVideo" runat="server" Text="Subir Video" OnClick="btnSubirVideo_Click" />
            <asp:Label ID="lblResultadoVideo" runat="server" ForeColor="Green" />
        </div>

        <div class="seccion">
            <!-- Seccion Campusinfo -->
            <h2>Descripcion del Campus </h2>
            <asp:Label runat="server" Text="Título:" /><br />
            <asp:TextBox ID="txtTituloCampusInfo" runat="server" Width="100%" /><br />

            <asp:Label runat="server" Text="Imagen Principal:" /><br />
            <asp:Image ID="imgPrincipal" runat="server" Width="200px" /><br />
            <asp:Label Text="Selecciona la nueva imagen:" runat="server" AssociatedControlID="fuImagenPrincipal" />
            <asp:FileUpload ID="fuImagenPrincipal" runat="server" /><br />
            <asp:TextBox ID="txtNuevoNombreImagenPrincipal" runat="server" Width="300px" Placeholder="Nuevo nombre imagen (ej. imagen.png)" />
            <asp:Button ID="btnSubirImagenPrincipal" runat="server" Text="Subir Imagen" OnClick="btnSubirImagenPrincipal_Click" />
            <asp:Label ID="lblResultadoImgPrincipal" runat="server" ForeColor="Green" /><br />

            <asp:Label runat="server" Text="Descripción 1:" /><br />
            <asp:TextBox ID="txtDescripcion1" runat="server" Width="100%" TextMode="MultiLine" Rows="2" /><br />

            <asp:Label runat="server" Text="Descripción 2:" /><br />
            <asp:TextBox ID="txtDescripcion2" runat="server" Width="100%" TextMode="MultiLine" Rows="2" /><br />

            <asp:Label runat="server" Text="Texto Invitación:" /><br />
            <asp:TextBox ID="txtInvitacionTexto" runat="server" Width="100%" TextMode="MultiLine" Rows="2" /><br />

            <asp:Label runat="server" Text="Imagen Frase:" /><br />
            <asp:Image ID="imgImagenFrase" runat="server" Width="200px" /><br />
            <asp:Label Text="Selecciona la nueva imagen:" runat="server" AssociatedControlID="fuImagenFrase" />
            <asp:FileUpload ID="fuImagenFrase" runat="server" /><br />
            <asp:TextBox ID="txtNuevoNombreImagenFrase" runat="server" Width="300px" Placeholder="Nuevo nombre imagen frase" />
            <asp:Button ID="btnSubirImagenFrase" runat="server" Text="Subir Imagen Frase" OnClick="btnSubirImagenFrase_Click" />
            <asp:Label ID="lblResultadoImgFrase" runat="server" ForeColor="Green" />
        </div>

        <!-- Botones Comunidad -->
        <div class="seccion">
            <h2>Botones Comunidad</h2>
            <asp:Repeater ID="rptBotonesComunidad" runat="server" OnItemCommand="rptBotonesComunidad_ItemCommand" OnItemDataBound="rptBotonesComunidad_ItemDataBound">
                <ItemTemplate>
                    <div style="margin-bottom: 15px; border: 1px solid #ccc; padding: 10px; border-radius: 10px;">
                        <!-- Vista previa de imagen -->
                        <asp:Image ID="imgBotonComunidad" runat="server" Width="100" />

                        <!-- Subir nueva imagen -->
                        <asp:Label Text="Nueva imagen:" runat="server" /><br />
                        <asp:FileUpload ID="fuImagenBotonComunidad" runat="server" /><br />
                        <br />

                        <!-- Nombre de nueva imagen -->
                        <asp:Label Text="Nuevo nombre (sin extensión):" runat="server" /><br />
                        <asp:TextBox ID="txtNombreImagenBotonComunidad" runat="server" Width="200px" /><br />
                        <br />

                        <!-- Resultado imagen (opcional) -->
                        <asp:Label ID="lblResultadoImgBotonComunidad" runat="server" ForeColor="Green" />

                        <!-- Enlace -->
                        <asp:Label Text="Enlace:" runat="server" /><br />
                        <asp:TextBox ID="txtEnlaceBotonComunidad" runat="server" Text='<%# Eval("enlace") %>' Width="300px" /><br />
                        <br />

                        <!-- Eliminar -->
                        <asp:Button ID="btnEliminarBotonComunidad" runat="server" Text="Eliminar" CommandName="Eliminar" CommandArgument='<%# Container.ItemIndex %>' CssClass="btn" />
                    </div>
                </ItemTemplate>
            </asp:Repeater>
            <asp:Button ID="btnAgregarBotonComunidad" runat="server" Text="Agregar nuevo botón" OnClick="btnAgregarBotonComunidad_Click" CssClass="btn" />
        </div>

        <!-- Oferta Academica -->
        <div class="seccion">
            <h2>Oferta Académica</h2>

            <!-- Selección de ítem existente -->
            <asp:Label Text="Selecciona un ítem para editar o eliminar:" runat="server" /><br />
            <asp:DropDownList ID="ddlOfertaSeleccion" runat="server" AutoPostBack="true"
                OnSelectedIndexChanged="ddlOfertaSeleccion_SelectedIndexChanged" />

            <!-- Repeater para editar/eliminar el ítem seleccionado -->
            <asp:Repeater ID="rptOfertaAcademica" runat="server" OnItemCommand="rptOfertaAcademica_ItemCommand">
                <ItemTemplate>
                    <asp:HiddenField ID="hfIndex" runat="server" Value='<%# Container.ItemIndex %>' />
                    <br />

                    <asp:Label Text="Título:" runat="server" /><br />
                    <asp:TextBox ID="txtTitulo" runat="server" Text='<%# Eval("titulo") %>' Width="300px" /><br />

                    <asp:Label Text="Descripción:" runat="server" /><br />
                    <asp:TextBox ID="txtDescripcion" runat="server" Text='<%# Eval("descripcion") %>' Width="300px" /><br />

                    <asp:Label Text="Enlace:" runat="server" /><br />
                    <asp:TextBox ID="txtEnlace" runat="server" Text='<%# Eval("enlace") %>' Width="300px" /><br />

                    <asp:Label runat="server" Text="Imagen actual:" /><br />
                    <asp:Image ID="imagenOferta" runat="server" Width="200px" /><br />

                    <asp:Label Text="Selecciona la nueva imagen:" runat="server" AssociatedControlID="fuImagenOferta" /><br />
                    <asp:FileUpload ID="fuImagenOferta" runat="server" /><br />

                    <asp:TextBox ID="txtNuevoNombreImagenOferta" runat="server" Width="300px"
                        Placeholder="Nuevo nombre imagen Oferta" /><br />

                    <asp:Button ID="btnEliminar" runat="server" Text="Eliminar este ítem"
                        CommandName="Eliminar" CommandArgument='<%# Container.ItemIndex %>' ForeColor="White" /><br />

                    <asp:Label ID="lblResultadoImgOferta" runat="server" ForeColor="Green" /><br />
                </ItemTemplate>
            </asp:Repeater>

            <hr />

            <!-- Agregar nuevo ítem -->
            <h4>Agregar Nuevo Ítem</h4>
            <asp:TextBox ID="txtNuevoTitulo" runat="server" Placeholder="Título nuevo" Width="300px" /><br />
            <asp:TextBox ID="txtNuevoDescripcion" runat="server" Placeholder="Descripción nueva" Width="300px" /><br />
            <asp:TextBox ID="txtNuevoEnlace" runat="server" Placeholder="Enlace nuevo" Width="300px" /><br />

            <asp:Label Text="Selecciona imagen:" runat="server" /><br />
            <asp:FileUpload ID="fuNuevaImagen" runat="server" /><br />
            <asp:TextBox ID="txtNombreNuevaImagen" runat="server" Width="300px" Placeholder="Nombre imagen" /><br />

            <asp:Label ID="lblResultadoNuevaImagen" runat="server" ForeColor="Green" /><br />

            <asp:Button ID="btnAgregarOferta" runat="server" Text="Agregar Ítem"
                OnClick="btnAgregarOferta_Click" />
        </div>

        <!-- Botones Circulares Ofertas -->
        <div class="seccion">
            <h2>Botones Circulares</h2>
            <asp:Repeater ID="rptBotonesCirculares" runat="server" OnItemCommand="rptBotonesCirculares_ItemCommand" OnItemDataBound="rptBotonesCirculares_ItemDataBound">
                <ItemTemplate>
                    <div style="margin-bottom: 15px; border: 1px solid #ccc; padding: 10px; border-radius: 10px;">
                        <!-- Imagen actual -->
                        <asp:Image ID="imgBotonCircular" runat="server" Width="100" />

                        <!-- Subir nueva imagen -->
                        <br />
                        <asp:Label Text="Nueva imagen:" runat="server" /><br />
                        <asp:FileUpload ID="fuImagenBotonCircular" runat="server" /><br />
                        <br />

                        <!-- Nuevo nombre de imagen -->
                        <asp:Label Text="Nuevo nombre (sin extensión):" runat="server" /><br />
                        <asp:TextBox ID="txtNombreImagenBotonCircular" runat="server" Width="200px" /><br />
                        <br />

                        <!-- Resultado imagen -->
                        <asp:Label ID="lblResultadoImgBotonCircular" runat="server" ForeColor="Green" />

                        <!-- Enlace editable -->
                        <asp:Label Text="Enlace:" runat="server" /><br />
                        <asp:TextBox ID="txtEnlaceBotonCircular" runat="server" Text='<%# Eval("enlace") %>' Width="300px" /><br />
                        <br />

                        <!-- Botón para eliminar -->
                        <asp:Button ID="btnEliminarBotonCircular" runat="server" Text="Eliminar" CommandName="Eliminar" CommandArgument='<%# Container.ItemIndex %>' CssClass="btn" />
                    </div>
                </ItemTemplate>
            </asp:Repeater>

            <!-- Botón para agregar nuevo -->
            <asp:Button ID="btnAgregarBotonCircular" runat="server" Text="Agregar nuevo botón" OnClick="btnAgregarBotonCircular_Click" CssClass="btn" />
        </div>

        <!-- Campus Ofertas -->
        <div class="seccion">
            <h2>Ofertas de Otros Campus</h2>
            <asp:Label runat="server">Selecciona un campus:</asp:Label>
            <asp:DropDownList ID="ddlCampusOfertas" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlCampusOfertas_SelectedIndexChanged"></asp:DropDownList>
            <br />
            <asp:Image ID="imgCampusOferta" runat="server" Width="150" /><br />
            <asp:FileUpload ID="fuImagenCampusOferta" runat="server" />
            <br />
            <asp:Label Text="Nuevo nombre (sin extensión):" runat="server" /><br />
            <asp:TextBox ID="txtNombreImagenCampusOferta" runat="server" Width="200px" Placeholder="Nuevo nombre imagen (sin extensión)" />
            <br />
            <asp:Label Text="Enlace:" runat="server" /><br />
            <asp:TextBox ID="txtEnlaceCampusOferta" runat="server" Width="300px" />
            <asp:Label ID="lblResultadoImagenCampusOferta" runat="server" ForeColor="Green" />
        </div>

        <!-- Siguenos -->
        <div class="seccion">
            <!-- Síguenos -->
            <h2>Imagen "Síguenos"</h2>
            <asp:Image ID="Image1" runat="server" Width="200px" /><br />
            <asp:Label Text="Selecciona la nueva imagen:" runat="server" AssociatedControlID="fuImagenSiguenos" />
            <asp:FileUpload ID="fuImagenSiguenos" runat="server" /><br />
            <asp:TextBox ID="txtImagenSiguenos" runat="server" Width="300px" Placeholder="Nuevo nombre imagen Siguenos" />
            <asp:Button ID="btnSubirImagenSiguenos" runat="server" Text="Subir Imagen Siguenos" OnClick="btnSubirImagenSiguenos_Click" />
            <asp:Label ID="Label2" runat="server" ForeColor="Green" />
        </div>

        <!-- Footer -->
        <div class="seccion">
            <!-- Footer -->
            <h2>Footer</h2>
            <asp:Image ID="Image2" runat="server" Width="200px" /><br />
            <asp:Label Text="Selecciona la nueva imagen:" runat="server" AssociatedControlID="fuImagenFOOTER" />
            <asp:FileUpload ID="fuImagenFOOTER" runat="server" /><br />
            <asp:TextBox ID="txtImagenFooter" runat="server" Width="300px" Placeholder="Nuevo nombre imagen Footer" />
            <asp:Button ID="btnSubirImagenFooter" runat="server" Text="Subir Imagen Siguenos" OnClick="btnSubirImagenFooter_Click" />
            <asp:Label ID="Label3" runat="server" ForeColor="Green" />

            <br />
            <asp:Label runat="server" Text="Derechos:" /><br />
            <asp:TextBox ID="txtFooterDerechos" runat="server" Width="100%" TextMode="MultiLine" Rows="2" /><br />
            <br />

            <asp:Label runat="server" Text="Información del Footer:" /><br />
            <asp:TextBox ID="txtFooterInfo1" runat="server" Width="100%" TextMode="MultiLine" Rows="4" /><br />
            <asp:TextBox ID="txtFooterInfo2" runat="server" Width="100%" TextMode="MultiLine" Rows="2" /><br />
            <br />
        </div>

        <br />
        <br />
        <div id="botonesFijos">
            <asp:Button ID="btnGuardar" runat="server" Text="Guardar Cambios" CssClass="btn btn-primary" OnClick="btnGuardarCambios_Click" />
            <asp:Button ID="btnRegresarPanel" runat="server" Text="Regresar al Panel" CssClass="btn btn-info" OnClick="btnRegresarPanel_Click" />
            <asp:Button ID="btnIrVista" runat="server" Text="Editar Vistas del Menu" CssClass="btn btn-success" OnClick="btnIrVista_Click" />
            <asp:Button ID="btnLogout" runat="server" Text="Cerrar sesión" CssClass="btn btn-secondary" OnClick="btnLogout_Click" />
        </div>

    </form>

    <script>
        // Guardar el scroll en el almacenamiento local antes de enviar el formulario
        window.addEventListener("beforeunload", function () {
            localStorage.setItem("scrollY", window.scrollY);
        });

        // Restaurar el scroll al cargar la página
        window.addEventListener("load", function () {
            const scrollY = localStorage.getItem("scrollY");
            if (scrollY) window.scrollTo(0, parseInt(scrollY));
        });
    </script>

    <script type="text/javascript">
        function mostrarVistaPrevia(input) {
            if (input.files && input.files[0]) {
                var lector = new FileReader();
                lector.onload = function (e) {
                    document.getElementById('<%= imgPreview.ClientID %>').src = e.target.result;
                };
                lector.readAsDataURL(input.files[0]);
            }
        }
    </script>
</body>
</html>
