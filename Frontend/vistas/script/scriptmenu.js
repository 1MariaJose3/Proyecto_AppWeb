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


function mostrarProximosInicios(inicios) {
    const iniciosContainer = document.getElementById("proximos-inicios");
    if (!iniciosContainer || !Array.isArray(inicios)) return;

    const iniciosHTML = inicios.map(inicio => `
        <div class="inicio-item">
            <p class="inicio-dia">${inicio.dia}</p>
            <p class="inicio-detalle">
                ${inicio.mes}<br>
                <a href="${inicio.enlace}">${inicio.nombre}</a>
            </p>
        </div>
    `).join("");

    iniciosContainer.innerHTML = `
        <h3>PRÓXIMOS INICIOS</h3>
        ${iniciosHTML}
        <a href="#" class="btn-ver-todos" id="btn-ver-todos">Ver todos</a>
    `;
}

function extraerProximosIniciosDesdeMensaje(mensaje, campusActual, cantidad = 3) {
    const meses = [
        "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio",
        "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre"
    ];

    const hoy = new Date();
    const proximos = [];

    (mensaje.categorias || []).forEach(categoria => {
        // Solo las categorías POSGRADOS y DIPLOMADOS (sin importar mayúsculas)
        const nombreCat = categoria.nombre?.toUpperCase() || "";
        if (nombreCat !== "POSGRADOS" && nombreCat !== "DIPLOMADOS") return;

        (categoria.programas || []).forEach(programa => {
            // Filtrar por ciudad/campus (insensible a mayúsculas)
            if (!programa.ciudad || programa.ciudad.toLowerCase() !== campusActual.toLowerCase()) return;

            const fecha = parseFecha(programa.inicio);
            if (!isNaN(fecha) && fecha >= hoy) {
                proximos.push({
                    fecha,
                    dia: fecha.getDate().toString().padStart(2, "0"),
                    mes: meses[fecha.getMonth()],
                    nombre: programa.nombre,
                    enlace: programa.enlace
                });
            }
        });
    });

    return proximos.sort((a, b) => a.fecha - b.fecha).slice(0, cantidad);
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



function mostrarHorarioAtencion(horario) {
    const horarioContainer = document.getElementById("horario-atencion");
    if (!horarioContainer || !horario) return;

    horarioContainer.innerHTML = `
        <h3>HORARIO DE ATENCIÓN</h3>
        <p><span class="icono-horario"><i class="bi bi-clock"></i></span>Lunes a Viernes: ${horario.lunesViernes}</p>
        <p><span class="icono-horario"><i class="bi bi-calendar-week"></i></span>Sábado: ${horario.sabado}</p>
        <p><span class="icono-horario"><i class="bi bi-telephone-forward"></i></span>Informes: ${horario.informes}</p>
        <p><span class="icono-horario"><i class="bi bi-tools"></i></span>Soporte Técnico: ${horario.soporte}</p>  
        <p><span class="icono-horario"><i class="bi bi-mortarboard-fill"></i></span>Diplomados: ${horario.diplomados}</p>
    `;
}


// Función principal
function iniciar() {
    const campusNombre = obtenerNombreCampus();
    console.log("CampusNombre final:", campusNombre);

    cargarDatosJSON("http://localhost:50429/GetData.aspx")
        .then(data => {
            if (!data.campus || !data.campus[campusNombre]) {
                alert("Campus no encontrado o datos incorrectos");
                return;
            }

            const campus = data.campus[campusNombre];
            establecerTitulo(campus.titulo);

            // CORRECCIÓN: Usar campus.contenido.mensajeRector en lugar de mensajeRector indefinido
            const vistaActual = obtenerVistaActual();
            let mensajeRector = null;

            // Buscar en el menú la opción con claveVista = "eventos"
            for (const menu of campus.menu || []) {
                for (const opcion of menu.opciones || []) {
                    if (opcion.claveVista === "eventos") {
                        mensajeRector = opcion.contenido?.mensajeRector;
                        break;
                    }
                }
                if (mensajeRector) break;
            }

            // Mostrar PRÓXIMOS INICIOS si se encuentra mensajeRector
            if (mensajeRector) {
                const inicios = extraerProximosIniciosDesdeMensaje(mensajeRector, campusNombre, 3);
                mostrarProximosInicios(inicios);
            } else {
                console.warn("No se encontró mensajeRector");
            }

            mostrarHorarioAtencion(campus.horarioAtencion);

            // Mostrar banner dinámico
            const banner = document.getElementById("banner-container");
            if (banner && campus.imagenBanner) {
                banner.innerHTML = `<img class="banner" src="/vistas/${campus.imagenBanner}" alt="Banner">`;
            }
        })
        .catch(err => {
            console.error("Error al cargar los datos:", err);
        });
}


// Ejecutar
iniciar();