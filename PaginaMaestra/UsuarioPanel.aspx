<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UsuarioPanel.aspx.cs" Inherits="PaginaMaestra.UsuarioPanel" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
            <style>
        body {
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            background: linear-gradient(to bottom right, #004080, #6699cc);
            margin: 0;
            padding: 0;
            min-height: 100vh;
            display: flex;
            align-items: center;
            justify-content: center;
        }

        form {
            background-color: #ffffff;
            padding: 40px 30px;
            border-radius: 12px;
            box-shadow: 0 0 15px rgba(0, 0, 0, 0.2);
            width: 100%;
            max-width: 500px;
            text-align: center;
        }

        h2 {
            color: #004080;
            margin-bottom: 30px;
        }

        .aspNetButton {
            width: 100%;
            max-width: 300px;
            padding: 12px;
            margin: 10px auto;
            background-color: #0059b3;
            border: none;
            color: white;
            font-weight: bold;
            border-radius: 6px;
            cursor: pointer;
            transition: background-color 0.3s ease;
            display: block;
        }

        .aspNetButton:hover {
            background-color: #004080;
        }

        #lblMensaje {
            color: green;
            font-weight: bold;
            margin-bottom: 15px;
            display: block;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <h2>Panel de Administración </h2>
        <asp:Button ID="btnEditarCampus" runat="server" Text="Ir a Editar Campus" CssClass="aspNetButton" OnClick="btnEditarCampus_Click" />
        <asp:Button ID="btnEditarVistaDinamica" runat="server" Text="Ir a Editar Vista del Menu" CssClass="aspNetButton" OnClick="btnEditarVistaDinamica_Click" />
        <asp:Button ID="btnLogout" runat="server" Text="Cerrar sesión" CssClass="aspNetButton" OnClick="btnLogout_Click" />
    </form>
</body>
</html>
