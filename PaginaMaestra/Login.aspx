<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="PaginaMaestra.Login" %>

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
            height: 100vh;
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
            max-width: 400px;
        }

        h2 {
            text-align: center;
            color: #004080;
            margin-bottom: 25px;
        }

        label {
            display: block;
            margin-bottom: 5px;
            color: #003366;
            font-weight: 600;
        }

        input[type="text"],
        input[type="password"] {
            width: 100%;
            padding: 10px;
            margin-bottom: 20px;
            border: 1px solid #b3cde0;
            border-radius: 6px;
            box-sizing: border-box;
        }

        input[type="submit"],
        .aspNetButton {
            width: 100%;
            padding: 10px;
            background-color: #0059b3;
            border: none;
            color: white;
            font-weight: bold;
            border-radius: 6px;
            cursor: pointer;
            transition: background-color 0.3s ease;
        }

        input[type="submit"]:hover,
        .aspNetButton:hover {
            background-color: #004080;
        }

        .mensaje-error {
            text-align: center;
            color: red;
            margin-bottom: 15px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <h2>Panel Administrativo</h2>

        <asp:Label ID="lblMensaje" runat="server" CssClass="mensaje-error" />

        <asp:Label Text="Usuario:" runat="server" AssociatedControlID="txtUsuario" />
        <asp:TextBox ID="txtUsuario" runat="server" MaxLength="10"/>

        <asp:Label Text="Contraseña:" runat="server" AssociatedControlID="txtContrasena" />
        <asp:TextBox ID="txtContrasena" runat="server" TextMode="Password" MaxLength="15" />

        <asp:Button ID="btnLogin" runat="server" Text="Iniciar Sesión" CssClass="aspNetButton" OnClick="btnLogin_Click" OnClientClick="return validarFormulario();" />

    </form>

</body>
</html>
