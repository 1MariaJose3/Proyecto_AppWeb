<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EditorVistaDinamica.aspx.cs" Inherits="PaginaMaestra.EditorVistaDinamica" ValidateRequest="false" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <style>
        body {
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            background: linear-gradient(to bottom right, #004080, #6699cc);
            margin: 0;
            padding: 30px;
        }

        form {
            background-color: #ffffff;
            padding: 30px;
            border-radius: 12px;
            box-shadow: 0 0 10px rgba(0, 0, 0, 0.15);
            max-width: 1200px;
            margin: auto;
        }

        h2, h3, h4 {
            color: #003366;
            margin-top: 40px;
            border-bottom: 2px solid #b3cde0;
            padding-bottom: 6px;
        }

        label, .aspLabel {
            font-weight: bold;
            color: #00264d;
            display: inline-block;
            margin-top: 15px;
            margin-bottom: 5px;
        }

        input[type="text"],
        input[type="password"],
        textarea,
        select,
        .aspNetTextBox,
        .aspNetDropDownList {
            width: 100%;
            max-width: 500px;
            padding: 10px;
            margin-bottom: 15px;
            border: 1px solid #b3cde0;
            border-radius: 6px;
            box-sizing: border-box;
        }

        input[type="file"] {
            margin-bottom: 15px;
        }

        input[type="submit"],
        button,
        .aspNetButton,
        asp\:Button {
            padding: 10px 20px;
            background-color: #0059b3;
            color: white;
            font-weight: bold;
            border: none;
            border-radius: 6px;
            cursor: pointer;
            margin-top: 10px;
            margin-bottom: 20px;
            transition: background-color 0.3s ease;
        }

        input[type="submit"]:hover,
        button:hover,
        .aspNetButton:hover,
        asp\:Button:hover {
            background-color: #004080;
        }

        .seccion {
            background-color: #f9f9f9;
            padding: 25px;
            border: 1px solid #cccccc;
            border-radius: 10px;
            margin-bottom: 30px;
        }

        hr {
            margin: 30px 0;
            border: 0;
            border-top: 1px solid #ccc;
        }

        img {
            border: 1px solid #ccc;
            border-radius: 6px;
            margin-bottom: 10px;
        }

        video {
            border-radius: 6px;
            margin-top: 10px;
        }

        textarea {
            resize: vertical;
        }

        #botonesFijos {
            position: fixed;
            bottom: 20px;
            right: 20px;
            z-index: 9999;
            background: rgba(255, 255, 255, 0.9);
            padding: 10px;
            border-radius: 8px;
            box-shadow: 0 2px 10px rgba(0, 0, 0, 0.2);

            display: flex;              /* Activar flexbox */
            flex-direction: column;     /* Dirección vertical */
            align-items: stretch;       /* Opcional: que ocupen todo el ancho */
            gap: 10px;                  /* Espacio entre botones */
        }

        #botonesFijos input[type="submit"],
        #botonesFijos button {
            width: 100%;               /* Que los botones ocupen todo el contenedor */
        }


    </style>

    <script type="text/javascript">
        window.onload = function () {
            if (sessionStorage.scrollTop != "undefined") {
                document.documentElement.scrollTop = sessionStorage.scrollTop;
                document.body.scrollTop = sessionStorage.scrollTop;
            }
        };

        window.onbeforeunload = function () {
            sessionStorage.scrollTop = document.documentElement.scrollTop || document.body.scrollTop;
        };
    </script>

</head>
<body>
    <form id="form1" runat="server">
        <!-- Editar Vistas -->
        <h2>Edicion de vistas</h2>

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

        <!-- Selección de Menu -->
        <asp:Label ID="lblMenuPrincipal" runat="server" Text="Selecciona una sección del menú:"></asp:Label><br />
        <asp:DropDownList ID="ddlMenu" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlMenu_SelectedIndexChanged" />
        <br />
        <br />
        <asp:Panel ID="pnlEditorDinamico" runat="server" />
        <br />

        <div id="botonesFijos">
            <asp:Button ID="btnGuardar" runat="server" Text="Guardar Cambios" CssClass="btn btn-primary" OnClick="btnGuardar_Click" />
            <asp:Button ID="btnRegresarPanel" runat="server" Text="Regresar al Panel" CssClass="btn btn-info" OnClick="btnRegresarPanel_Click" />
            <asp:Button ID="btnIrVista" runat="server" Text="Editar Página Principal" CssClass="btn btn-success" OnClick="btnIrVista_Click" />
            <asp:Button ID="btnLogout" runat="server" Text="Cerrar sesión" CssClass="btn btn-secondary" OnClick="btnLogout_Click" />
            <asp:Label ID="lblMensaje" runat="server" ForeColor="Green" />
        </div>

    </form>
</body>
</html>
