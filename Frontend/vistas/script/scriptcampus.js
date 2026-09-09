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
    //http://18.219.188.248/PaginaMaestra/GetData.aspx
    cargarDatosJSON("http://localhost:50429/GetData.aspx")
        .then(data => {
            if (!data.campus || !data.campus[campusNombre]) {
                alert("Campus no encontrado o datos incorrectos");
                return;
            }
            const campus = data.campus[campusNombre];
            mostrarContenidoVistaCAMPUS(campus.menu, obtenerVistaActual());
        })
        .catch(err => {
            console.error("Error al cargar los datos:", err);
        });
}

// Ejecutar
iniciar();

const campusHandlers = {
  eventos: {
    clase: "eventos",
    funcion: mostrarEnterateEventos
  },
  tiemposdistancia: {
    clase: "tiemposdistancia",
    funcion: mostrarTiemposYDistancia
  },
  hospedaje: {
    clase: "hospedaje",
    funcion: mostrarHospedaje
  },
  restaurantes: {
    clase: "restaurantes",
    funcion: mostrarRestaurantes
  },
  turismo: {
    clase: "turismo",
    funcion: mostrarAtractivosTuristicos
  }
};


function mostrarContenidoVistaCAMPUS(menu, vistaNombre) {
  let contenido = null;
  let claveVista = null;

  for (const item of menu) {
    const opcion = item.opciones?.find(opt => opt.titulo === vistaNombre);
    if (opcion?.contenido) {
      contenido = opcion.contenido;
      claveVista = opcion.claveVista;
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

  const handler = campusHandlers[claveVista];
  if (handler) {
    aplicarClaseVista(mensajeContainer, handler.clase);
    handler.funcion(mensaje, mensajeContainer);
  } else {
    console.warn(`No se encontró handler para la claveVista: ${claveVista}`);
  }
}


function aplicarClaseVista(contenedor, clase) {
    contenedor.className = "";
    contenedor.classList.add(clase, "slide-fade"); 
}

function mostrarEnterateEventos(mensaje, contenedor) {
    let html = `
        <section class="enterate slide-fade">
            <div class="tabla-wrapper"><table class="tabla-elegante">
                <thead>
                    <tr>
                        <th colspan="3" class="titulo-tabla">${mensaje.titulo}</th>
                    </tr>
                    <tr class="encabezados">
                        <th>Programa</th>
                        <th>Inicio</th>
                        <th>Duración</th>
                    </tr>
                </thead>
                <tbody>`;

    const categorias = mensaje.categorias || [];
    categorias.forEach(categoria => {
        // ✅ Ordenar los programas por fecha de inicio (más reciente primero)
        const programasOrdenados = categoria.programas.slice().sort((a, b) => {
            const fechaA = parseFecha(a.inicio);
            const fechaB = parseFecha(b.inicio);
            return fechaA - fechaB;
        });
        html += `
            <tr class="categoria">
                <td colspan="3">${categoria.nombre}</td>
            </tr>`;

        programasOrdenados.forEach(programa => {
            html += `
                <tr class="fila-hover">
                    <td><a href="${programa.enlace}" target="_blank">${programa.nombre}</a></td>
                    <td>${programa.inicio}</td>
                    <td>${programa.duracion}</td>
                </tr>`;
        });
    });

    html += `
                </tbody>
            </table>
        </section>`;

    contenedor.innerHTML = html;
}

function parseFecha(fechaStr) {
    const mesesES = ["enero","febrero","marzo","abril","mayo","junio","julio","agosto","septiembre","octubre","noviembre","diciembre"];
    const mesesEN = ["January","February","March","April","May","June","July","August","September","October","November","December"];

    let fechaLower = fechaStr.toLowerCase();
    mesesES.forEach((mes, i) => {
        if (fechaLower.includes(mes)) {
            fechaLower = fechaLower.replace(mes, mesesEN[i]);
        }
    });

    return new Date(fechaLower);
}


function mostrarTiemposYDistancia(mensaje, contenedor) {
    let html = `
        <section class="tiempos-distancia-container slide-fade">
            <h3 class="titulo-campus">${mensaje.titulo}</h3>
            <div class="bloque-direccion">${mensaje.parrafos?.[0] || ""}</div>
            <div class="bloque-mapa">${mensaje.iframeMapa || ""}</div>
            <h4 class="titulo-campus">${mensaje.titulo2}</h4>
            <div class="bloque-estados">`;

    const secciones = mensaje.secciones || [];
    secciones.forEach(seccion => {
        html += `
            <div class="estado-box">
                <div class="titulo-estado">${seccion.subtitulo}</div>
                <ul class="lista-tiempos">`;

        seccion.parrafos.forEach(parrafo => {
            html += `<li>${parrafo}</li>`;
        });

        html += `
                </ul>
            </div>`;
    });

    html += `
            </div>
        </section>`;

    contenedor.innerHTML = html;
}

function mostrarHospedaje(mensaje, mensajeContainer) {
        let html = `<section class="mensaje-texto hospedaje">
            <h3>${mensaje.titulo}</h3>
            ${mensaje.parrafos.map(p => `<p>${p}</p>`).join("")}
        `;

        if (mensaje.secciones && Array.isArray(mensaje.secciones)) {
            mensaje.secciones.forEach(seccion => {
                html += `<div class="card-hospedaje">
                    <h4>${seccion.subtitulo}</h4>
                    ${seccion.parrafos?.map(p => `<p>${resaltarEtiquetas(p)}</p>`).join("") ?? ""}
                    ${seccion.lista ? `<ul>${seccion.lista.map(i => `<li>${i}</li>`).join("")}</ul>` : ""}
                    
                    ${generarGaleriaImagenes(seccion)}

                    ${seccion.enlace ? `<p><a href="${seccion.enlace}" target="_blank">Visitar sitio del hotel</a></p>` : ""}
                    ${seccion.nota ? `<div class="nota"><i class="fa-solid fa-circle-info"></i> <div>${seccion.nota.map(n => `<p>${n}</p>`).join("")}</div></div>` : ""}`;

                // Subsecciones (como CHN: Norte, Centro, Aeropuerto)
                if (seccion.secciones) {
                    const carruselId = `carrusel-${Math.random().toString(36).substring(2, 8)}`; // ID único
                    
                    html += `
                    <div id="${carruselId}" class="carousel slide" data-bs-ride="carousel">
                    <div class="carousel-inner">
                        ${seccion.secciones.map((sub, index) => `
                        <div class="carousel-item ${index === 0 ? 'active' : ''}">
                            <div class="card-hospedaje subtipo">
                            <h5>${sub.subtitulo2}</h5>
                            ${sub.parrafos.map(p => `<p>${resaltarEtiquetas(p)}</p>`).join("")}
                            ${sub.lista ? `<ul>${sub.lista.map(i => `<li>${i}</li>`).join("")}</ul>` : ""}
                            ${generarGaleriaImagenes(sub)}
                            ${sub.enlace ? `<p><a href="${sub.enlace}" target="_blank">Visitar sitio del hotel</a></p>` : ""}
                            ${sub.nota ? `<div class="nota"><i class="fa-solid fa-circle-info"></i> <div>${sub.nota.map(n => `<p>${n}</p>`).join("")}</div></div>` : ""}
                            </div>
                        </div>
                        `).join("")}
                    </div>
                    </div>

                    <!-- Controles debajo -->
                    <div class="carousel-controls-bottom">
                    <button type="button" data-bs-target="#${carruselId}" data-bs-slide="prev">
                        <i class="bi bi-chevron-left"></i>
                    </button>
                    <button type="button" data-bs-target="#${carruselId}" data-bs-slide="next">
                        <i class="bi bi-chevron-right"></i>
                    </button>
                    </div>
                    `;

                }
                html += `</div>`;
            });
        }

        html += `</section>`;
        mensajeContainer.innerHTML = html;
}

function resaltarEtiquetas(texto) {
    return texto
        .replace(/(Dirección:)/gi, '<strong>$1</strong>')
        .replace(/(Teléfono:)/gi, '<strong>$1</strong>')
        .replace(/(Telefono:)/gi, '<strong>$1</strong>')
        .replace(/(Correo:)/gi, '<strong>$1</strong>')
        .replace(/(Tarifas:)/gi, '<strong>$1</strong>')
        .replace(/(Tarifas Estándar:)/gi, '<strong>$1</strong>');
}

function generarGaleriaImagenes(obj) {
    if (!obj.imagenHospedaje && !obj.imagenHospedaje2 && !obj.imagenHospedaj3) return "";

    return `
        <div class="galeria-en-fila">
            ${obj.imagenHospedaje ? `<img src="${obj.imagenHospedaje}" alt="Hotel 1">` : ""}
            ${obj.imagenHospedaje2 ? `<img src="${obj.imagenHospedaje2}" alt="Hotel 2">` : ""}
            ${obj.imagenHospedaj3 ? `<img src="${obj.imagenHospedaj3}" alt="Hotel 3">` : ""}
        </div>
    `;
}

function mostrarRestaurantes(mensaje, contenedor) {
    let html = `<h3>${mensaje.titulo}</h3>`;

    // Párrafos introductorios
    if (mensaje.parrafos && Array.isArray(mensaje.parrafos)) {
        mensaje.parrafos.forEach(p => {
            html += `<p>${p}</p>`;
        });
    }

    // Secciones de restaurantes
    if (mensaje.secciones && Array.isArray(mensaje.secciones)) {
        mensaje.secciones.forEach(seccion => {
            html += `<div class="card-restaurante">`;
            html += `<h4>${seccion.subtitulos}</h4>`;

            seccion.parrafos.forEach(p => {
                html += `<p>${p}</p>`;
            });

            if (seccion.imagenesRestaurantes && seccion.imagenesRestaurantes.length > 0) {
                html += `<div class="galeria-restaurantes">`;
                seccion.imagenesRestaurantes.forEach(img => {
                    html += `<img src="${img}" alt="Imagen restaurante">`;
                });
                html += `</div>`;
            }

            html += `</div>`;
        });
    }

    contenedor.innerHTML = html;
}

function mostrarAtractivosTuristicos(mensaje, contenedor) {
    let html = `
    <section class="atractivos-tarjetas">
        <h2 class="titulo-atractivos">${mensaje.titulo}</h2>`;

    mensaje.secciones.forEach(sitio => {
        let infoUnida = "";

        // Texto para Ubicación, Horario, Duración y Costo Aproximado en negritas
        sitio.parrafo2.forEach(p => {
            const claves = ["Ubicación", "Horario", "Duración", "Costo Aproximado"];
            const clave = claves.find(c => p.startsWith(c));
            if (clave) {
                const valor = p.replace(`${clave}:`, "").trim();
                infoUnida += `<p><strong>${clave}:</strong> ${valor}</p>`;
            } else {
                infoUnida += `<p>${p}</p>`;
            }
        });

        // Lista con icono <i class="bi bi-coin"></i>
        const listaHTML = sitio.lista.map(item => `<p><i class="bi bi-cash-coin"></i>${item}</p>`).join("");

        infoUnida += listaHTML;

        html += `
        <div class="tarjeta-atractivo">
            <h3 class="subtitulo">${sitio.subtitulo}</h3>

            ${sitio.parrafo1.map(p => `<p class="texto-justificado">${p}</p>`).join("")}

            <div class="info-extra texto-justificado">
                ${infoUnida}
            </div>

            <div class="galeria-atractivo">
                ${sitio.imagenesRestaurantes.map(img => `<img src="${img}" alt="Imagen turística">`).join("")}
            </div>
        </div>`;
    });

    html += `</section>`;
    contenedor.innerHTML = html;
}
