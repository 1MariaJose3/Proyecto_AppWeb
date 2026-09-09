# Sitio Web
# 🎓 Sitio Web Institucional - EP de México

Este repositorio contiene el código fuente desarrollado como parte del proyecto de tesina. Se trata del sitio web institucional de **EP de México**, diseñado para cargar la información de los diferentes campus de forma dinámica mediante la integración de un frontend web y un backend de servicios.

## 📂 Estructura del Proyecto
* **`Frontend/`**: Interfaz de usuario estructurada en HTML, CSS, JavaScript y Bootstrap.
* **`PaginaMaestra/`**: Servicio backend desarrollado en ASP.NET (C#) encargado de procesar la información de los campus y los formularios de contacto.

## 🛠️ Tecnologías
* **Frontend:** HTML5, CSS3, JavaScript (ES6+), Bootstrap 5.
* **Backend:** ASP.NET (C#), `.aspx`, IIS Express.

## 🚀 Ejecución en entorno local
1. **Backend:** Abrir la carpeta `PaginaMaestra` en Visual Studio, establecer `GetData.aspx` como página de inicio y presionar `F5`.
2. **Frontend:** Abrir la carpeta `Frontend` en Visual Studio Code, iniciar con **Live Server** e ingresar a la URL especificando el campus deseado (ejemplo: `http://127.0.0.1:5500/index.html?campus=MONTERREY`).
