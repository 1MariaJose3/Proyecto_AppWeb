// 1. Obtener el valor de 'campus' de la URL
function obtenerNombreCampus() {
    const params = new URLSearchParams(window.location.search);
    return params.get("campus");
}

// 2. Obtener el nombre de la vista actual (ej. vista.html => "Bienvenida")
function obtenerVistaActual() {
    const params = new URLSearchParams(window.location.search);
    return params.get("vista");
}

// 3. Cargar el archivo JSON
function cargarDatosJSON(url) {
    return fetch(url).then(response => response.json());
}

function iniciar() {
    let campusNombre = obtenerNombreCampus();
    cargarDatosJSON("http://localhost:50429/GetData.aspx")
        .then(data => {
            if (!data.campus || !data.campus[campusNombre]) {
                alert("Campus no encontrado o datos incorrectos");
                return;
            }
            const campus = data.campus[campusNombre];
            mostrarContenidoVistaCONOCENOS(campus.menu, obtenerVistaActual());
        })
        .catch(err => {
            console.error("Error al cargar los datos:", err);
        });
}

// Ejecutar
iniciar();

const vistaHandlers = {
    bienvenida: {
        clase: "bienvenida",
        funcion: mostrarBienvenida
    },
    epmexico: {
        clase: "epmexico",
        funcion: mostrarEPMexico
    },
    epmonterrey: {
        clase: "epmonterrey",
        funcion: mostrarEPMonterrey
    },
    ceremonias: {
        clase: "ceremonias",
        funcion: mostrarCeremonias
    },
    catedraticos: {
        clase: "catedraticos",
        funcion: mostrarCatedraticos
    }
};


function mostrarContenidoVistaCONOCENOS(menu, vistaNombre) {
    let contenido = null;
    let claveVista = null;

    for (const item of menu) {
        const opcion = item.opciones?.find(opt => opt.titulo === vistaNombre);
        if (opcion?.contenido && opcion.claveVista) {
            contenido = opcion.contenido;
            claveVista = opcion.claveVista.toLowerCase(); // Por seguridad
            break;
        }
    }

    if (!contenido || !claveVista) return;

    const banner = document.getElementById("banner-container");
    if (banner && contenido.imagenBanner) {
        banner.innerHTML = `<img class="banner" src="/vistas/${contenido.imagenBanner}" alt="Banner">`;
    }

    const mensaje = contenido.mensajeRector;
    const mensajeContainer = document.getElementById("mensaje-rector");
    if (!mensajeContainer || !mensaje) return;

    const handler = vistaHandlers[claveVista];

    if (handler) {
        aplicarClaseVista(mensajeContainer, handler.clase);
        handler.funcion(mensaje, mensajeContainer);
    } else {
        console.warn(`No se encontró handler para claveVista: ${claveVista}`);
    }
}


function aplicarClaseVista(contenedor, clase) {
    contenedor.className = ""; // Limpia todas las clases actuales
    contenedor.classList.add(clase, "slide-fade"); // Agrega la clase según la vista
}


function mostrarBienvenida(mensaje, contenedor) {
    let html = `<h3>${mensaje.titulo}</h3>`;
    const total = mensaje.parrafos.length;
    mensaje.parrafos.forEach((parrafo, index) => {
        if (index >= total - 3) {
            html += `<p><strong>${parrafo}</strong></p>`;
        } else {
            html += `<p>${parrafo}</p>`;
        }
    });
    contenedor.innerHTML = html;
}

function mostrarEPMexico(mensaje, contenedor) {
    let html = `<div class="epmexico-contenido">`;

    // Título
    if (mensaje.titulo) {
        html += `<h3>${mensaje.titulo}</h3>`;
    }

    // Párrafos iniciales
    if (Array.isArray(mensaje.parrafos)) {
        mensaje.parrafos.forEach(p => {
            html += `<p>${p}</p>`;
        });
    }

    // Campus como tarjetas con ícono
    if (Array.isArray(mensaje.lista) && mensaje.lista.length > 0) {
        html += `<h4>Campus EP de México</h4>`;
        html += `<div class="campus-list">`;
        mensaje.lista.forEach(item => {
            html += `
                <div class="campus-item">
                    <span class="campus-icon">🎓</span>
                    <div class="campus-text">${item}</div>
                </div>
            `;
        });
        html += `</div>`;
    }

    // Párrafos finales
    if (Array.isArray(mensaje.parrafosFinales)) {
        mensaje.parrafosFinales.forEach(pf => {
            html += `<p>${pf}</p>`;
        });
    }

    html += `</div>`;
    contenedor.innerHTML = html;
}

function mostrarEPMonterrey(mensaje, contenedor) {
    let html = `<div class="epmonterrey-contenido slide-fade">`;

    // Título
    if (mensaje.titulo) {
        html += `<h3>${mensaje.titulo}</h3>`;
    }

    // Párrafos
    if (Array.isArray(mensaje.parrafos)) {
        mensaje.parrafos.forEach(p => {
            html += `<p>${p}</p>`;
        });
    }

    html += `</div>`;
    contenedor.innerHTML = html;
}

function mostrarCeremonias(mensaje, contenedor) {
    let html = `<div class="ceremonias-contenido slide-fade">`;

    if (mensaje.titulo) {
        html += `<h3 class="ceremonias-titulo">${mensaje.titulo}</h3>`;
    }

    if (Array.isArray(mensaje.secciones)) {
        mensaje.secciones.forEach(seccion => {
            html += `<div class="ceremonia-card">`;

            // Subtítulo con ícono
            if (seccion.subtitulo) {
                html += `
                    <div class="ceremonia-subtitulo">
                        <span class="ceremonia-icon">🎓</span>
                        <h4>${seccion.subtitulo}</h4>
                    </div>
                `;
            }

            // Párrafos normales
            if (Array.isArray(seccion.parrafos)) {
                seccion.parrafos.forEach(p => {
                    if (p.includes("La excelencia es el arte que se alcanza")) {
                        html += `<p><strong><em>${p}</em></strong></p>`;
                    } else if (p.includes("Aristóteles Año")) {
                        html += `<p style="text-align: right;"><strong><em>${p}</em></strong></p>`;
                    } else {
                        html += `<p>${p}</p>`;
                    }
                });
            }

            // Lista
            if (Array.isArray(seccion.lista)) {
                html += `<ul>`;
                seccion.lista.forEach(item => {
                    html += `<li>${item}</li>`;
                });
                html += `</ul>`;
            }

            // Párrafos adicionales (parrafos1)
            if (Array.isArray(seccion.parrafos1)) {
                seccion.parrafos1.forEach(p => {
                    html += `<p>${p}</p>`;
                });
            }

            html += `</div>`; // .ceremonia-card
        });
    }

    html += `</div>`;
    contenedor.innerHTML = html;
}

function mostrarCatedraticos(mensaje, contenedor) {
    let html = `<div class="catedraticos-contenido slide-fade">`;

    // Título principal
    if (mensaje.titulo) {
        html += `<h3 class="catedraticos-titulo">${mensaje.titulo}</h3>`;
    }

    // Párrafos
    if (Array.isArray(mensaje.parrafos)) {
        mensaje.parrafos.forEach(parrafo => {
            html += `<p>${parrafo}</p>`;
        });
    }

    html += `</div>`;
    contenedor.innerHTML = html;
}




