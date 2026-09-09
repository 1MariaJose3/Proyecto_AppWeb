<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Registro.aspx.cs" Inherits="PaginaMaestra.Registro" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8" />
    <title>Registro de Usuario</title>
    <link href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css" rel="stylesheet" />
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
            max-width: 700px;
        }

        h2, h3, h4 {
            color: #004080;
            text-align: center;
        }

        label {
            display: block;
            margin-bottom: 6px;
            color: #003366;
            font-weight: 600;
        }

        .form-control,
        .password-input,
        select {
            width: 100%;
            padding: 10px;
            border: 1px solid #b3cde0;
            border-radius: 6px;
            margin-bottom: 20px;
            box-sizing: border-box;
            font-size: 14px;
        }

        .aspNetButton, .btn-success {
            width: 100%;
            max-width: 300px;
            padding: 10px;
            background-color: #0059b3;
            border: none;
            color: white;
            font-weight: bold;
            border-radius: 6px;
            cursor: pointer;
            transition: background-color 0.3s ease;
            display: block;
            margin: 0 auto 20px auto;
        }

        .aspNetButton:hover, .btn-success:hover {
            background-color: #004080;
        }

        .mensaje-error {
            color: red;
            text-align: center;
            margin-bottom: 15px;
        }

        .table {
            width: 100%;
            border-collapse: collapse;
            margin-top: 20px;
        }

        .table th, .table td {
            border: 1px solid #cccccc;
            padding: 10px;
            text-align: center;
        }

        .table th {
            background-color: #004080;
            color: white;
        }

        .table tr:nth-child(even) {
            background-color: #f2f2f2;
        }

        .password-wrapper {
            position: relative;
            width: 100%;
            margin-bottom: 20px;
        }

        .password-input-container {
            position: relative;
            width: 100%;
        }

        .password-input {
            width: 100%;
            padding: 10px;
            padding-right: 40px;
            height: 40px;
            box-sizing: border-box;
            line-height: 20px; /* para alinear con el ícono */
        }


        .password-toggle-icon {
            position: absolute;
            top: 10px; /* en vez de 50% + translate */
            right: 12px;
            font-size: 18px;
            color: #6c757d;
            cursor: pointer;
            z-index: 5;
            width: 20px;
            height: 20px;
            display: flex;
            justify-content: center;
            align-items: center;
        }


        .password-requisitos {
            background-color: #e6f0ff;
            border: 1px solid #99c2ff;
            border-radius: 12px;
            padding: 16px 20px;
            display: none;
            color: #003366;
            animation: fadeIn 0.2s ease-in-out;
            margin-bottom: 20px;
        }

        .requisitos-grid {
            display: flex;
            flex-wrap: wrap;
            gap: 16px;
        }

        .requisitos-grid ul {
            list-style: none;
            padding-left: 0;
            flex: 1;
            min-width: 200px;
        }

        .requisitos-grid li {
            display: flex;
            align-items: center;
            margin-bottom: 8px;
            font-size: 14px;
        }

        .icono {
            width: 20px;
            height: 20px;
            font-size: 12px;
            border-radius: 50%;
            background-color: #dc3545;
            color: white;
            display: inline-flex;
            justify-content: center;
            align-items: center;
            margin-right: 10px;
        }

        .valido .icono {
            background-color: #007bff;
        }

        @keyframes fadeIn {
            from { opacity: 0; transform: translateY(-5px); }
            to { opacity: 1; transform: translateY(0); }
        }

        #pnlEditarUsuario {
            background-color: #e6f0ff;
            border: 1px solid #99c2ff;
            border-radius: 12px;
            padding: 25px;
            margin-top: 30px;
            box-shadow: 0 4px 10px rgba(0, 64, 128, 0.2);
        }

        .btn-sm {
            padding: 4px 10px;
            font-size: 13px;
            border-radius: 4px;
            margin: 2px;
        }

        .btn-warning {
            background-color: #ffc107;
            border: none;
            color: #212529;
        }

        .btn-warning:hover {
            background-color: #e0a800;
        }

        .btn-danger {
            background-color: #dc3545;
            border: none;
            color: white;
        }

        .btn-danger:hover {
            background-color: #c82333;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <h2>Registro de Usuario</h2>

        <asp:Label ID="lblMensaje" runat="server" CssClass="mensaje-error" />

        <asp:Label Text="Nuevo Usuario:" runat="server" AssociatedControlID="txtNuevoUsuario" />
        <asp:TextBox ID="txtNuevoUsuario" runat="server" MaxLength="10" CssClass="form-control" placeholder="Usuario solo letras y números" />

        <asp:Label Text="Contraseña:" runat="server" AssociatedControlID="txtNuevaContrasena" />
        <div class="password-wrapper" tabindex="-1">
            <div class="password-input-container">
                <asp:TextBox ID="txtNuevaContrasena" runat="server" CssClass="form-control password-input" TextMode="Password" MaxLength="15" placeholder="Contraseña" />
                <i id="togglePassword" class="fas fa-eye password-toggle-icon" tabindex="0" aria-label="Mostrar/Ocultar contraseña"></i>
            </div>

            <div class="password-requisitos" id="passwordRequisitos">
                <p><strong>La contraseña debe contener:</strong></p>
                <div class="requisitos-grid">
                    <ul>
                        <li id="valLongitud"><i class="icono fas fa-times"></i>Entre 1 y 15 caracteres</li>
                        <li id="valMayuscula"><i class="icono fas fa-times"></i>1 Mayúscula</li>
                        <li id="valMinuscula"><i class="icono fas fa-times"></i>1 Minúscula</li>
                    </ul>
                    <ul>
                        <li id="valNumero"><i class="icono fas fa-times"></i>1 Número</li>
                        <li id="valEspecial"><i class="icono fas fa-times"></i>1 Símbolo</li>
                    </ul>
                </div>
            </div>
        </div>


        <asp:Label Text="Rol:" runat="server" AssociatedControlID="ddlRol" />
        <asp:DropDownList ID="ddlRol" runat="server" CssClass="form-control">
            <asp:ListItem Text="Administrador" Value="admin" />
            <asp:ListItem Text="Usuario" Value="usuario" />
        </asp:DropDownList>

        <asp:Button ID="btnRegistrar" runat="server" Text="Registrar" CssClass="aspNetButton"
            OnClientClick="return validarFormulario();" OnClick="btnRegistrar_Click" Enabled="false" />

        <hr />

        <h3>Administradores Registrados</h3>
        <asp:GridView ID="gvUsuarios" runat="server" AutoGenerateColumns="False" DataKeyNames="id"
            OnRowDeleting="gvUsuarios_RowDeleting" OnRowCommand="gvUsuarios_RowCommand" CssClass="table">
            <Columns>
                <asp:BoundField DataField="id" HeaderText="ID" />
                <asp:BoundField DataField="usuario" HeaderText="Usuario" />
                <asp:BoundField DataField="rol" HeaderText="Rol" />
                <asp:TemplateField HeaderText="Acciones">
                    <ItemTemplate>
                        <asp:LinkButton ID="btnEditar" runat="server" CommandName="EditUser" CommandArgument='<%# Eval("id") %>'
                            Text="Editar" CssClass="btn btn-warning btn-sm" />
                        <asp:LinkButton ID="btnEliminar" runat="server" CommandName="Delete"
                            Text="Eliminar" CssClass="btn btn-danger btn-sm" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>

        <asp:Panel ID="pnlEditarUsuario" runat="server" Visible="false">
            <h4>Editar Usuario</h4>
            <asp:HiddenField ID="hfEditarId" runat="server" />
            <asp:TextBox ID="txtEditarUsuario" runat="server" placeholder="Usuario" CssClass="form-control" />
            <asp:TextBox ID="txtEditarContrasena" runat="server" placeholder="Nueva Contraseña (opcional)" CssClass="form-control password-input" TextMode="Password" />
            <asp:DropDownList ID="ddlEditarRol" runat="server" CssClass="form-control">
                <asp:ListItem Text="Administrador" Value="admin" />
                <asp:ListItem Text="Usuario" Value="usuario" />
            </asp:DropDownList>
            <asp:Button ID="btnGuardarEdicion" runat="server" Text="Guardar Cambios" CssClass="btn btn-success" OnClick="btnGuardarEdicion_Click" />
        </asp:Panel>

        <div style="margin-top: 30px; text-align: center;">
            <asp:Button ID="btnRegresarPanel" runat="server" Text="Regresar al Panel" CssClass="aspNetButton" OnClick="btnRegresarPanel_Click" />
            <asp:Button ID="btnLogout" runat="server" Text="Cerrar sesión" CssClass="aspNetButton" OnClick="btnLogout_Click" />
        </div>
    </form>

    <script>
        const passwordInput = document.getElementById('<%= txtNuevaContrasena.ClientID %>');
        const togglePassword = document.getElementById("togglePassword");
        const requisitos = document.getElementById("passwordRequisitos");
        const passwordWrapper = document.querySelector(".password-wrapper");

        // Mostrar panel cuando el input gana foco
        passwordInput.addEventListener("focus", () => {
            requisitos.style.display = "block";
        });

        // Mantener visible mientras el foco esté en input, icono o el panel
        passwordWrapper.addEventListener("focusout", () => {
            setTimeout(() => {
                if (!passwordWrapper.contains(document.activeElement)) {
                    requisitos.style.display = "none";
                }
            }, 100);
        });

        // Mostrar / ocultar contraseña
        togglePassword.addEventListener("click", () => {
            const isPassword = passwordInput.type === "password";
            passwordInput.type = isPassword ? "text" : "password";
            togglePassword.classList.toggle("fa-eye-slash", !isPassword);
            togglePassword.classList.toggle("fa-eye", isPassword);
            passwordInput.focus();
        });

        // Validación de reglas
        passwordInput.addEventListener("input", () => {
            const value = passwordInput.value;
            const reglas = {
                valLongitud: value.length >= 1 && value.length <= 15,
                valMayuscula: /[A-Z]/.test(value),
                valMinuscula: /[a-z]/.test(value),
                valNumero: /[0-9]/.test(value),
                valEspecial: /[!@#$%^&*]/.test(value)
            };
            for (const id in reglas) {
                const el = document.getElementById(id);
                const icon = el.querySelector(".icono");
                const ok = reglas[id];
                icon.className = "icono fas " + (ok ? "fa-check" : "fa-times");
                el.classList.toggle("valido", ok);
            }
        });


        const registrarBtn = document.getElementById('<%= btnRegistrar.ClientID %>');
        const usuarioInput = document.getElementById('<%= txtNuevoUsuario.ClientID %>');

        function validarUsuario() {
            return /^[a-zA-Z0-9]{1,10}$/.test(usuarioInput.value.trim());
        }

        function validarPassword() {
            const v = passwordInput.value;
            return v.length >= 1 && v.length <= 15 &&
                /[A-Z]/.test(v) && /[a-z]/.test(v) &&
                /[0-9]/.test(v) && /[!@#$%^&*]/.test(v);
        }

        function validarAmbosCampos() {
            registrarBtn.disabled = !(validarUsuario() && validarPassword());
        }

        usuarioInput.addEventListener('input', validarAmbosCampos);
        passwordInput.addEventListener('input', validarAmbosCampos);

        function validarFormulario() {
            if (!validarUsuario()) {
                alert("El usuario debe tener solo letras y números, entre 1 y 10 caracteres.");
                return false;
            }
            if (!validarPassword()) {
                alert("La contraseña debe cumplir con todos los requisitos.");
                return false;
            }
            return true;
        }

        window.onload = () => { registrarBtn.disabled = true; };
    </script>
</body>
</html>
