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
            mostrarContenidoVistaPROGRAMAS(campus.menu, obtenerVistaActual()); 
        })
        .catch(err => {
            console.error("Error al cargar los datos:", err);
        });
}


// Ejecutar
iniciar();

function mostrarContenidoVistaPROGRAMAS(menu, vistaNombre) {
    let contenido = null;

    for (const item of menu) {
        if (item.titulo !== "PROGRAMAS") continue;

        const categorias = item.categorias ?? [item];
        for (const cat of categorias) {
            const programa = cat.programas?.find(p => p.nombre === vistaNombre);
            if (programa?.contenido) {
                contenido = programa.contenido;
                break;
            }
        }

        if (contenido) break;
    }

    if (!contenido) return;

    // Mostrar banner
    const banner = document.getElementById("banner-container");
    if (banner && contenido.imagenBanner) {
        banner.innerHTML = `<img class="banner" src="/vistas/${contenido.imagenBanner}" alt="Banner del programa">`;
    }

    const mensaje = contenido.mensajeRector;
    const container = document.getElementById("mensaje-rector");
    container.className = "";
    container.classList.add("programas", "slide-fade");

    // Inicio de HTML
    let html = ` 
        <div class="seccion-infoPrograma">
            <h1>${mensaje.titulo}</h1>
            ${mensaje.subtitulo ? `<h3>${mensaje.subtitulo}</h3>` : ""}
            ${mensaje.inicio ? `<h4><strong>Inicio:</strong> ${mensaje.inicio}</h4>` : ""}
            ${mensaje.duracion ? `<h4><strong>Duración:</strong> ${mensaje.duracion}</h4>` : ""}
            ${mensaje.cupo ? `<h3>${mensaje.cupo}</h3>` : ""}
            ${mensaje.modalidad ? `<h4><strong>Modalidad:</strong> ${mensaje.modalidad}</h4>` : ""}
            ${mensaje.horarios ? `<h4><strong>Días y Horarios:</strong> ${mensaje.horarios}</h4>` : ""}
            ${mensaje.dirigidoA ? `<h4><strong>Dirigido a:</strong> ${mensaje.dirigidoA}</h4>` : ""}
        </div>
    `;

    if (mensaje.universidad) {
        html += `
            <div class="seccion-Universidad">
                <h3>${mensaje.universidad.nombre}</h3>
                ${Array.isArray(mensaje.universidad.descripcion)
                    ? mensaje.universidad.descripcion.map(p => `<p>${p}</p>`).join('')
                    : `<p>${mensaje.universidad.descripcion}</p>`}
            </div>        
        `;
    }

    if (mensaje.objetivoGeneral) {
        html += `
            <div class="seccion-objetivoGeneral">
                <h3>${mensaje.objetivoGeneral.subtitulo}</h3>
                <p>${mensaje.objetivoGeneral.descripcion}</p>
            </div>    
        `;
    }

    if (mensaje.objetivoEspecifico) {
        html += `
            <div class="seccion-objetivoEspecifico">
                <h3>${mensaje.objetivoEspecifico.subtitulo}</h3>
                <ul>${mensaje.objetivoEspecifico.lista.map(item => `<li>${item}</li>`).join("")}</ul>
            </div>   
        `;
    }

    if (mensaje.planEstudios) {
        html += `
            <div class="seccion-planEstudios">
                <h3>${mensaje.planEstudios.subtitulo}</h3>
                <p>${mensaje.planEstudios.diploma}</p>
            </div>  
        `;
    }

    if (mensaje.perfilEgreso) {
        html += `
            <div class="seccion-perfilEgreso">
                <h3>${mensaje.perfilEgreso.subtitulo}</h3>
                <p>${mensaje.perfilEgreso.descripcion}</p>
            </div> 
        `;
    }

    if (mensaje.valorCurricular) {
        html += `
            <div class="seccion-valorCurricular">
                <h3>${mensaje.valorCurricular.subtitulo}</h3>
                <p>${mensaje.valorCurricular.descripcion}</p>
                <h4>${mensaje.valorCurricular.subtitulo2}</h4>
                <p>${mensaje.valorCurricular.empresa}</p>
            </div> 
        `;
    }

    if (mensaje.titulacion) {
        html += `
            <div class="seccion-titulacion">
                <h3>${mensaje.titulacion.titulo}</h3>
                <p>${mensaje.titulacion.descripcion}</p>
                <h4>${mensaje.titulacion.subtituloOpciones}</h4>
                <ul>${mensaje.titulacion.opciones.map(o => `<li>${o}</li>`).join('')}</ul>
                <h4>${mensaje.titulacion.subtituloEntrega}</h4>
                <ul>${mensaje.titulacion.alFinalizar.map(r => `<li>${r}</li>`).join('')}</ul>
            </div> 
        `;
    }

    if (mensaje.competenciasProfesionales) {
    const competencias = mensaje.competenciasProfesionales;
    const claves = ["conocimientos", "habilidades", "actitudes", "valores"];
    const etiquetas = {
        conocimientos: "🧠 Conocimientos",
        habilidades: "💡 Habilidades",
        actitudes: "✨ Actitudes",
        valores: "🌟 Valores"
    };

    html += `
    <div class="tabs-competencias">
    <h3 class="titulo-tabs">${competencias.tituloSeccion}</h3>

    <div class="tab-buttons">
        ${claves.map((clave, i) =>
        competencias[clave]
            ? `<button class="tab-btn${i === 0 ? ' active' : ''}" onclick="mostrarTab(this, 'tab-${clave}')">${etiquetas[clave]}</button>`
            : ''
        ).join('')}
    </div>

    <div class="tab-content">
        ${claves.map((clave, i) => {
        if (!competencias[clave]) return "";
        const items = competencias[clave].map(item => {
            if (typeof item === "string") {
            return `<li>${item}</li>`;
            } else if (typeof item === "object" && item.texto) {
            const sub = item.sublista
                ? `<ul class="sublista">${item.sublista.map(sub => `<li>🔹 ${sub}</li>`).join('')}</ul>`
                : "";
            return `<li>${item.texto}${sub}</li>`;
            }
            return "";
        }).join("");

        return `
        <div class="tab-panel${i === 0 ? ' active' : ''}" id="tab-${clave}">
            <ul>${items}</ul>
        </div>`;
        }).join('')}
    </div>
    </div>`;
    }

    const programa = mensaje.programaAcademico ?? mensaje.contenidoAcademico;
    if (programa) {
        const esSemestral = Array.isArray(programa.semestres);
        const esModular = Array.isArray(programa.modulos);
        const lista = esSemestral ? programa.semestres : esModular ? programa.modulos : [];
    
        html += `
            <div class="seccion-acordeon ${esModular ? 'modulos' : ''}">
                <h3 class="titulo-programa">${programa.tituloSeccion}</h3>
                <div class="accordion" id="acordeonPrograma">
                    ${lista.map((bloque, i) => {
                        const tituloBloque = esSemestral
                            ? `Semestre ${i + 1}`
                            : `Módulo ${i + 1}`;
    
                        const contenido = bloque.map(item => {
                            if (esSemestral && typeof item === "string") {
                                return `<li>📗 ${item}</li>`;
                            } else if (esModular && typeof item === "object" && item.texto) {
                                const sub = item.sublista
                                    ? `<ul class="sublista">${item.sublista.map(sub => `<li>🔹 ${sub}</li>`).join('')}</ul>`
                                    : "";
                                const titulo = item.titulo
                                    ? `<strong class="titulo-item">${item.titulo}</strong><br/>`
                                    : "";
                                return `<li class="item-texto">${titulo}${item.texto}${sub}</li>`;
                            }
                            return "";
                        }).join("");
    
                        return `
                            <div class="accordion-item">
                                <h2 class="accordion-header" id="heading-${i}">
                                    <button class="accordion-button collapsed" type="button"
                                        onclick="toggleAccordion(this, 'collapse-${i}')">
                                        📘 ${tituloBloque}
                                    </button>
                                </h2>
                                <div id="collapse-${i}" class="accordion-collapse collapse" aria-labelledby="heading-${i}">
                                    <div class="accordion-body">
                                        <ul class="lista-contenido">
                                            ${contenido}
                                        </ul>
                                    </div>
                                </div>
                            </div>
                        `;
                    }).join("")}
                </div>
            </div>
        `;
    }
    

    if (mensaje.rvoe) {
        html += `
            <h3>${mensaje.rvoe.titulo}</h3>
            <p>${mensaje.rvoe.clave}</p>
        `;
    }

    container.innerHTML = html;
}

function toggleAccordion(button, targetId) {
    const target = document.getElementById(targetId);
    const isOpen = target.classList.contains("show");

    if (isOpen) {
        // Cierra la actual
        target.classList.remove("show");
        button.classList.add("collapsed");
        button.setAttribute("aria-expanded", "false");
    } else {
        // Abre la actual
        target.classList.add("show");
        button.classList.remove("collapsed");
        button.setAttribute("aria-expanded", "true");
    }
}

function mostrarTab(boton, idContenido) {
  const contenedor = boton.closest('.tabs-competencias');

  // Quitar clase 'active' de todos los botones
  contenedor.querySelectorAll('.tab-btn').forEach(b => b.classList.remove('active'));
  // Ocultar todos los paneles
  contenedor.querySelectorAll('.tab-panel').forEach(p => p.classList.remove('active'));

  // Activar el botón y mostrar el contenido seleccionado
  boton.classList.add('active');
  contenedor.querySelector(`#${idContenido}`).classList.add('active');
}
