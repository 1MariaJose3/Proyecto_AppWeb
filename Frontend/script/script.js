// 1. Obtener el valor de 'campus' de la URL
function obtenerNombreCampus() {
    const params = new URLSearchParams(window.location.search);
    return params.get("campus");
}

// 2. Cargar el archivo JSON
function cargarDatosJSON(url) {
    return fetch(url).then(response => response.json());
}

// Función principal
function iniciar() {
    const campusNombre = obtenerNombreCampus();
    const path = window.location.pathname;

    //http://localhost:51649/Frontend/data/datos.json
    //http://localhost:50429/GetData.aspx
    //http://18.219.188.248/PaginaMaestra/GetData.aspx
    cargarDatosJSON("http://localhost:50429/GetData.aspx")
        .then(data => {
            if (!data.campus || !data.campus[campusNombre]) {
                alert("Campus no encontrado o datos incorrectos");
                return;
            }

            const campus = data.campus[campusNombre];
            establecerTitulo(campus.titulo);

            if (path.endsWith("index.html") || path === "/" ) {
                // Carga completa solo en index.html
                cargarBotonesFlotantes(campus.botonesFlotantes)
                mostrarLogos(campus.logos);
                mostrarMenu(campus.menu, false);
                mostrarRedesSociales(campus.redes_sociales, false);
                insertarVideo(campus.video);
                mostrarCampusInfo(campus.campusinfo);
                cargarBotones(campus.botonesComunidad)
                mostrarOfertaAcademica(campus.ofertaAcademica);
                mostrarBotonesCirculares(campus.botonesCirculares);
                campusOfertas(campus.campusOfertas);   
                mostrarSiguenos(campus.siguenos);
                mostrarFooter(campus.footer);
            } else if (path.includes("/vistas/")) {
                // Por ejemplo, solo cargar menú y footer en vistas
                cargarBotonesFlotantes(campus.botonesFlotantes)
                mostrarLogos(campus.logos);
                mostrarMenu(campus.menu, false);
                mostrarRedesSociales(campus.redes_sociales, false);
            }

            // Listener offcanvas (si existe)
            const offcanvasElem = document.getElementById('menuOffcanvas');
            if (offcanvasElem) {
                offcanvasElem.addEventListener('show.bs.offcanvas', () => {
                    mostrarMenu(campus.menu, true, campus.titulo);
                    mostrarRedesSociales(campus.redes_sociales, true);
                });
            }
        })
        .catch(err => {
            console.error("Error al cargar los datos:", err);
        });
}

// Ejecutar al cargar
iniciar();

function cargarBotonesFlotantes(botones) {
  const contenedor = document.querySelector('.redes-flotantes-abajo');
  contenedor.innerHTML = ''; // Limpiar antes

  botones.forEach(boton => {
    const a = document.createElement('a');
    a.href = boton.enlace;
    a.title = boton.imagen;

    // Asignar clase e icono según plataforma fija
    let clase = '';
    let icono = '';

    switch (boton.imagen.toLowerCase()) {
      case 'próximos eventos':
        clase = 'pb';
        icono = 'bi bi-calendar-event';
        break;
      case 'whatsapp':
        clase = 'wa';
        icono = 'fab fa-whatsapp';
        break;
      default:
        clase = 'default';
        icono = 'fas fa-link'; // ícono genérico por si acaso
    }

    a.className = `boton-red ${clase}`;
    a.innerHTML = `<i class="${icono}"></i><span>${boton.imagen.toLowerCase() === 'whatsapp' ? 'Contáctanos' : 'Próximos Eventos'}</span>`;

    contenedor.appendChild(a);
  });
}
// Variable global para guardar mega menús creados (solo para escritorio)
const megaMenusCreados = {};
function mostrarMenu(menuData, esOffcanvas = false, tituloMenu = "") {
    const contenedor = esOffcanvas
        ? document.getElementById("menuOffcanvasContent")
        : document.getElementById("menuEscritorio");

        if (esOffcanvas && tituloMenu) {
            const tituloOffcanvas = document.getElementById("menuOffcanvasLabel");
            if (tituloOffcanvas) {
                tituloOffcanvas.textContent = tituloMenu;
            }
        }


    contenedor.innerHTML = "";

    const esEscritorio = !esOffcanvas;

    const megaMenuGlobalContainer = document.getElementById("megaMenuGlobalContainer");
    if (esEscritorio) {
        megaMenuGlobalContainer.innerHTML = "";
        for (const key in megaMenusCreados) {
            delete megaMenusCreados[key];
        }
    }

    menuData.forEach(item => {
        const menuItem = document.createElement("div");
        menuItem.classList.add("menu_item");

        if (!item.opciones && !item.categorias) {
            const enlace = document.createElement("a");

            const campusActual = obtenerNombreCampus();
            let url = item.enlace || "#";

            if (item.titulo === "CONTACTOS") {
                // Forzar ruta absoluta para el index fuera de vistas
                url = `/index.html?campus=${encodeURIComponent(campusActual)}#contacto`;
            } else if (url.startsWith("#")) {
                url = `../index.html?campus=${encodeURIComponent(campusActual)}${url}`;
            } else if (url === "index.html") {
                url = `../index.html?campus=${encodeURIComponent(campusActual)}`;
            } else if (url.includes("#")) {
                const [base, hash] = url.split("#");
                url = `${base}?campus=${encodeURIComponent(campusActual)}#${hash}`;
            } else {
                url = `${url}?campus=${encodeURIComponent(campusActual)}`;
            }

            enlace.href = url;
            enlace.classList.add("btn_principal");
            enlace.textContent = item.titulo;

            menuItem.appendChild(enlace);
            contenedor.appendChild(menuItem);
            return;
        }

        const botonPrincipal = document.createElement("button");
        botonPrincipal.textContent = item.titulo;
        botonPrincipal.classList.add("btn_principal");

        // --- MEGA MENÚ PROGRAMAS ---
        if (item.titulo === "PROGRAMAS" && item.categorias) {
            menuItem.classList.add("mega_menu");

            if (esEscritorio) {
                // Crear mega menú solo si no existe
                if (!megaMenusCreados["PROGRAMAS"]) {
                    const megaContenedor = document.createElement("div");
                    megaContenedor.classList.add("mega_contenedor", "fuera_del_header");

                    const contCategorias = document.createElement("div");
                    contCategorias.classList.add("mega_categorias");

                    const contMaestrias = document.createElement("div");
                    const contDiplomados = document.createElement("div");
                    const contEspecialidades = document.createElement("div");

                    contMaestrias.classList.add("programas_maestrias");
                    contDiplomados.classList.add("programas_diplomados");
                    contEspecialidades.classList.add("programas_especialidades");

                    const megaContenido = document.createElement("div");
                    megaContenido.classList.add("mega_contenido");
                    megaContenido.innerHTML = `
                        <div class="mega_columna"><h4>MAESTRÍAS</h4></div>
                        <div class="mega_columna"><h4>DIPLOMADOS</h4></div>
                        <div class="mega_columna"><h4>ESPECIALIDADES</h4></div>
                    `;
                    megaContenido.children[0].appendChild(contMaestrias);
                    megaContenido.children[1].appendChild(contDiplomados);
                    megaContenido.children[2].appendChild(contEspecialidades);

                    item.categorias.forEach(categoria => {
                        const catBtn = document.createElement("button");
                        catBtn.innerHTML = `${categoria.nombre} <i class="fas fa-circle-chevron-right"></i>`;
                        catBtn.classList.add("categoria_btn");

                        catBtn.addEventListener("mouseenter", () => {
                            // Limpiar contenedores
                            contMaestrias.innerHTML = "";
                            contDiplomados.innerHTML = "";
                            contEspecialidades.innerHTML = "";

                            categoria.programas.forEach(programa => {
                                const campusActual = obtenerNombreCampus();
                                const url = `${programa.enlace}?campus=${encodeURIComponent(campusActual)}&vista=${encodeURIComponent(programa.nombre)}`;

                                const card = document.createElement("div");
                                card.classList.add("programa_card");
                                card.innerHTML = `
                                    <a href="${url}" class="enlace_escritorio" rel="noopener noreferrer">
                                        <img src="${programa.imagen}" alt="${programa.nombre}">
                                    </a>
                                    <a href="${url}" class="enlace_movil" rel="noopener noreferrer">${programa.nombre}</a>
                                `;

                                switch (programa.tipo) {
                                    case "maestria":
                                        contMaestrias.appendChild(card);
                                        break;
                                    case "diplomado":
                                        contDiplomados.appendChild(card);
                                        break;
                                    case "especialidad":
                                        contEspecialidades.appendChild(card);
                                        break;
                                }
                            });

                        });

                        contCategorias.appendChild(catBtn);
                    });

                    megaContenedor.appendChild(contCategorias);
                    megaContenedor.appendChild(megaContenido);

                    megaMenuGlobalContainer.appendChild(megaContenedor);

                    // Guardar referencia
                    megaMenusCreados["PROGRAMAS"] = megaContenedor;

                    // Eventos para mostrar/ocultar mega menú PROGRAMAS
                    const ocultarMegaMenu = () => megaContenedor.classList.remove("activo");

                    botonPrincipal.addEventListener("mouseenter", () => {
                        Object.values(megaMenusCreados).forEach(menu => menu.classList.remove("activo"));
                        megaContenedor.classList.add("activo");
                    });

                    botonPrincipal.addEventListener("mouseleave", () => {
                        setTimeout(() => {
                            if (!megaContenedor.matches(':hover') && !botonPrincipal.matches(':hover')) {
                                ocultarMegaMenu();
                            }
                        }, 100);
                    });

                    megaContenedor.addEventListener("mouseenter", () => {
                        megaContenedor.classList.add("activo");
                    });

                    megaContenedor.addEventListener("mouseleave", () => {
                        setTimeout(() => {
                            if (!botonPrincipal.matches(':hover') && !megaContenedor.matches(':hover')) {
                                ocultarMegaMenu();
                            }
                        }, 100);
                    });
                }

                menuItem.appendChild(botonPrincipal);
            } else {
                // Versión móvil offcanvas (sin cambios)
                const submenu = document.createElement("div");
                submenu.classList.add("submenu");

                item.categorias.forEach(categoria => {
                    const catItem = document.createElement("div");
                    catItem.classList.add("submenu_opcion");
                    catItem.textContent = categoria.nombre;

                    const programasSubmenu = document.createElement("div");
                    programasSubmenu.classList.add("submenu_programas");
                    programasSubmenu.style.display = "none";

                    categoria.programas.forEach(programa => {
                        const progLink = document.createElement("a");

                        const campusActual = obtenerNombreCampus();
                        const url = `${programa.enlace}?campus=${encodeURIComponent(campusActual)}&vista=${encodeURIComponent(programa.nombre)}`;
                        
                        progLink.href = url;
                        progLink.textContent = programa.nombre;
                        progLink.classList.add("submenu_programa_opcion");
                        programasSubmenu.appendChild(progLink);
                    });

                    catItem.addEventListener("click", (e) => {
                        e.stopPropagation();
                        programasSubmenu.style.display = (programasSubmenu.style.display === "none") ? "block" : "none";
                    });

                    submenu.appendChild(catItem);
                    submenu.appendChild(programasSubmenu);
                });

                botonPrincipal.addEventListener("click", (e) => {
                    e.stopPropagation();
                    submenu.classList.toggle("activo");
                });

                menuItem.appendChild(botonPrincipal);
                menuItem.appendChild(submenu);
            }

            contenedor.appendChild(menuItem);
            return;
        }

        // --- MEGA MENÚ CONÓCENOS Y CAMPUS ---
        if ((item.titulo === "CONÓCENOS" || item.titulo === "CAMPUS") && item.opciones) {
            menuItem.classList.add("mega_menu");

            if (esEscritorio) {
                if (!megaMenusCreados[item.titulo]) {
                    const megaContenedor = document.createElement("div");
                    megaContenedor.classList.add("mega_contenedor", "fuera_del_header");

                    const contCategorias = document.createElement("div");
                    contCategorias.classList.add("mega_categorias_horizontal");

                    item.opciones.forEach(opcion => {
                        const catBtn = document.createElement("button");
                        catBtn.classList.add("categoria_btn", "horizontal_btn");
                        catBtn.innerHTML = `
                            <span class="texto_boton">${opcion.titulo}</span>
                            <i class="fas fa-circle-chevron-right"></i>
                        `;

                        catBtn.addEventListener("click", () => {
                            const campusActual = obtenerNombreCampus();
                            const url = `${opcion.enlace}?campus=${encodeURIComponent(campusActual)}&vista=${encodeURIComponent(opcion.titulo)}`;
                            window.location.href = url;
                        });

                        contCategorias.appendChild(catBtn);
                    });

                    megaContenedor.appendChild(contCategorias);

                    megaMenuGlobalContainer.appendChild(megaContenedor);

                    megaMenusCreados[item.titulo] = megaContenedor;

                    const ocultarMegaMenu = () => megaContenedor.classList.remove("activo");

                    botonPrincipal.addEventListener("mouseenter", () => {
                        Object.values(megaMenusCreados).forEach(menu => menu.classList.remove("activo"));
                        megaContenedor.classList.add("activo");
                    });

                    botonPrincipal.addEventListener("mouseleave", () => {
                        setTimeout(() => {
                            if (!megaContenedor.matches(':hover') && !botonPrincipal.matches(':hover')) {
                                ocultarMegaMenu();
                            }
                        }, 100);
                    });

                    megaContenedor.addEventListener("mouseenter", () => {
                        megaContenedor.classList.add("activo");
                    });

                    megaContenedor.addEventListener("mouseleave", () => {
                        setTimeout(() => {
                            if (!botonPrincipal.matches(':hover') && !megaContenedor.matches(':hover')) {
                                ocultarMegaMenu();
                            }
                        }, 100);
                    });
                }

                menuItem.appendChild(botonPrincipal);
            } else {
                // Móvil offcanvas
                const submenu = document.createElement("div");
                submenu.classList.add("submenu");

                item.opciones.forEach(opcion => {
                    const enlace = document.createElement("a");
                    enlace.textContent = opcion.titulo;

                    const campusActual = obtenerNombreCampus();
                    enlace.href = `${opcion.enlace}?campus=${encodeURIComponent(campusActual)}&vista=${encodeURIComponent(opcion.titulo)}`;
                    
                    enlace.classList.add("submenu_opcion");
                    submenu.appendChild(enlace);
                });

                botonPrincipal.addEventListener("click", (e) => {
                    e.stopPropagation();
                    submenu.classList.toggle("activo");
                });

                menuItem.appendChild(botonPrincipal);
                menuItem.appendChild(submenu);
            }

            contenedor.appendChild(menuItem);
            return;
        }

        // --- Enlace normal con href ---
        if (item.enlace) {
            const enlace = document.createElement("a");
            enlace.href = item.enlace;
            enlace.classList.add("btn_principal");
            enlace.textContent = item.titulo;
            menuItem.appendChild(enlace);
            contenedor.appendChild(menuItem);
        }
    });
}

function establecerTitulo(titulo) {
    document.title = titulo;
}

function mostrarLogos(logos) {
    const logoContainer = document.querySelector(".logo_menu");
    logoContainer.innerHTML = "";
    const fragment = document.createDocumentFragment();

    logos.forEach(src => {
        const img = document.createElement("img");
        img.src = src;
        img.alt = "Logo " + src;
        fragment.appendChild(img);
    });

    logoContainer.appendChild(fragment);
}

function mostrarRedesSociales(redes, esOffcanvas = false) {
    const redesContainer = esOffcanvas
        ? document.getElementById("redesOffcanvasContent")
        : document.getElementById("redesEscritorio");

    redesContainer.innerHTML = "";
    const fragment = document.createDocumentFragment();

    redes.forEach(red => {
        const a = document.createElement("a");
        a.href = red.url;
        a.target = "_blank";
        a.classList.add("btn_redes");

        const icon = document.createElement("i");
        if (red.nombre === "Facebook") icon.classList.add("fab", "fa-facebook-f");
        if (red.nombre === "Instagram") icon.classList.add("fab", "fa-instagram");
        if (red.nombre === "TikTok") icon.classList.add("fab", "fa-tiktok");

        a.appendChild(icon);
        fragment.appendChild(a);
    });

    redesContainer.appendChild(fragment);
}

function insertarVideo(src) {
    const videoCampus = document.getElementById('videoCampus');
    if (!videoCampus) {
        console.warn("No existe el elemento #videoCampus en esta página, se omite cargar video");
        return;
    }
    const source = document.createElement('source');
    source.src = src;
    source.type = 'video/mp4';

    videoCampus.innerHTML = ''; // Limpia posibles fuentes anteriores
    videoCampus.appendChild(source);
    videoCampus.load(); // Recarga el video
    console.log("Video cargado:", src);
}

function mostrarCampusInfo(campusInfo) {
    const contenedor = document.querySelector(".campus-contenedor");
    if (!contenedor) {
        console.error("No se encontró el contenedor .campus-contenedor");
        return;
    }

    contenedor.innerHTML = "";

    // === CONTENEDOR PRINCIPAL CON ANIMACIÓN DESDE ABAJO ===
    const campusDiv = document.createElement("div");
    campusDiv.classList.add("campus", "container");
    campusDiv.setAttribute("data-aos", "fade-up");
    campusDiv.setAttribute("data-aos-duration", "1200");

    // === TÍTULO ===
    const h1 = document.createElement("h1");
    h1.textContent = campusInfo.titulo;
    campusDiv.appendChild(h1);

    // === FILA CON IMAGEN Y TEXTO ===
    const row = document.createElement("div");
    row.classList.add("row", "align-items-center", "gx-5");

    // === IMAGEN DERECHA ===
    const colImg = document.createElement("div");
    colImg.classList.add("col-12", "col-lg-6", "order-1", "order-lg-2", "text-center");

    const img = document.createElement("img");
    img.src = campusInfo.imagenPrincipal;
    img.alt = "Imagen principal del campus";
    img.classList.add("campus_foto");

    colImg.appendChild(img);

    // === TEXTO IZQUIERDA ===
    const colTexto = document.createElement("div");
    colTexto.classList.add("col-12", "col-lg-6", "campus_texto", "order-2", "order-lg-1");

    campusInfo.descripcion.forEach(parrafo => {
        const p = document.createElement("p");
        const palabrasClave = ["EP de México"];
        let textoFormateado = parrafo;

        palabrasClave.forEach(palabra => {
            const regex = new RegExp(`(${palabra})`, "g");
            textoFormateado = textoFormateado.replace(regex, `<strong>$1</strong>`);
        });

        p.innerHTML = textoFormateado;
        colTexto.appendChild(p);
    });

    // === LLAMADO CON FLECHAS E INVITACIÓN ===
    const h4 = document.createElement("h4");
    h4.classList.add("llamado");

    const iconoIzq = document.createElement("i");
    iconoIzq.className = campusInfo.invitacion.iconoIzquierdo + " flecha-icono";

    const iconoDer = document.createElement("i");
    iconoDer.className = campusInfo.invitacion.iconoDerecho + " flecha-icono";


    const span = document.createElement("span");
    let textoInvitacion = campusInfo.invitacion.texto;
    textoInvitacion = textoInvitacion.replace(
        "Gran Comunidad EP de México",
        "<strong>Gran Comunidad EP de México</strong>"
    );
    textoInvitacion = textoInvitacion.replace(
        "¡Únete y sé parte de la",
        "¡Únete y sé parte de la<br>"
    );

    const textoContainer = document.createElement("div");
    textoContainer.classList.add("llamado-texto");
    span.innerHTML = textoInvitacion;
    textoContainer.appendChild(span);

    h4.appendChild(iconoIzq);
    h4.appendChild(textoContainer);
    h4.appendChild(iconoDer);
    colTexto.appendChild(h4);

    // Agregar columnas al row
    row.appendChild(colImg);
    row.appendChild(colTexto);

    campusDiv.appendChild(row);
    contenedor.appendChild(campusDiv);

    // === FRASE FINAL ===
    const fraseImg = document.createElement("img");
    fraseImg.classList.add("frase");
    fraseImg.src = campusInfo.imagenFrase;
    fraseImg.alt = "Frase académica";

    contenedor.appendChild(fraseImg);
}

function cargarBotones(botones) {
  const contenedor = document.querySelector('.img-ccro');
  if (!contenedor || !botones || !Array.isArray(botones)) return;

  contenedor.innerHTML = '';

  const titulos = ['Comunidad', 'Revista', 'OCCCS'];
  const iconos = ['fa-users', 'fa-book-open', 'fa-landmark'];
  const acciones = ['Ver más', 'Explorar', 'Acceder'];
  const backStyles = ['back-style-1', 'back-style-2', 'back-style-3'];

  botones.forEach((btn, i) => {
    const card = document.createElement('div');
    card.className = 'flip-card';
    card.setAttribute('data-aos', 'zoom-in');
    card.setAttribute('data-aos-duration', '1500');
    card.setAttribute('data-aos-easing', 'ease-in-out');
    card.setAttribute('data-aos-once', 'true');

    card.innerHTML = `
      <div class="flip-inner">
        <div class="flip-front">
          <a href="${btn.enlace}" target="${btn.enlace.startsWith('http') ? '_blank' : '_self'}">
            <img class="btnimg" src="${btn.imagen}" alt="${titulos[i] || 'Botón'}" />
          </a>
        </div>
        <div class="flip-back ${backStyles[i] || 'back-style-1'}">
          <div class="back-content">
            <i class="fa-solid ${iconos[i] || 'fa-book-open'} fa-2x"></i>
            <p class="back-title">${titulos[i] || ''}</p>
            <span class="back-action">${acciones[i] || 'Ver más'}</span>
          </div>
        </div>
      </div>
    `;

    contenedor.appendChild(card);
  });

  AOS.refresh(); // Reinicia AOS en caso de que ya esté inicializado
}

function mostrarOfertaAcademica(oferta) {
    const contenedor = document.querySelector("#ofertaAcademica");

    if (!contenedor) {
        console.error("No se encontró el contenedor #ofertaAcademica");
        return;
    }

    contenedor.innerHTML = ""; // Limpiar contenido anterior

    // Título centrado
    const titulo = document.createElement("h1");
    titulo.textContent = oferta.tituloSeccion;
    titulo.className = "oferta_titulo";
    contenedor.appendChild(titulo);

    // Fila para los ítems
    const row = document.createElement("div");
    row.className = "row justify-content-center g-4";

    oferta.items.forEach((item, index) => {
        const col = document.createElement("div");
        col.className = "col-12 col-md-6 d-flex justify-content-center";

        const card = document.createElement("div");
        card.className = "oferta_item position-relative";
        card.setAttribute("data-aos", "fade-up");
        card.setAttribute("data-aos-duration", "1000");
        card.setAttribute("data-aos-delay", `${index * 100}`); // pequeño retraso para efecto escalonado

        // Imagen
        const img = document.createElement("img");
        img.src = item.imagen;
        img.alt = item.titulo;
        img.className = "img-fluid";

        // Enlace que contiene el botón
        const enlace = document.createElement("a");
        enlace.href = item.enlace;
        enlace.className = "text-decoration-none";
        enlace.style.position = "absolute";
        enlace.style.right = "40px";
        enlace.style.bottom = "30px";
        enlace.style.zIndex = "2";
        enlace.style.width = "260px";

        // Botón dividido
        const boton = document.createElement("button");
        boton.className = "boton_tarjeta";

        const contenidoSup = document.createElement("div");
        contenidoSup.className = "contenido_superior";

        const spanTitulo = document.createElement("span");
        spanTitulo.className = "titulo";
        spanTitulo.textContent = item.titulo;

        const direccion = document.createElement("div");
        direccion.className = "direccion";
        direccion.innerHTML = '<i class="fas fa-arrow-right"></i>';

        contenidoSup.appendChild(spanTitulo);
        contenidoSup.appendChild(direccion);

        const descripcion = document.createElement("div");
        descripcion.className = "descripcion";
        descripcion.textContent = item.descripcion;

        // Armar estructura
        boton.appendChild(contenidoSup);
        boton.appendChild(descripcion);
        enlace.appendChild(boton);
        card.appendChild(img);
        card.appendChild(enlace);
        col.appendChild(card);
        row.appendChild(col);
    });

    contenedor.appendChild(row);

    // Reinicializa AOS en caso de que ya se haya ejecutado antes
    AOS.refresh();
}

function mostrarBotonesCirculares(botones) {
    const contenedor = document.querySelector(".container-btnbajos");
    if (!contenedor || !botones) return;

    contenedor.innerHTML = ""; // Limpiar contenido anterior si lo hay

    botones.forEach((boton, index) => {
        const enlace = document.createElement("a");
        enlace.href = boton.enlace;
        enlace.className = "btn-circle";

        const img = document.createElement("img");
        img.src = boton.imagen;

        enlace.appendChild(img);
        contenedor.appendChild(enlace);
    });
}

function campusOfertas(campusData) {
  const campusItems = document.querySelectorAll(".campus-item");

  campusItems.forEach(item => {
    const nombreCampus = item.getAttribute("data-campus");
    const datos = campusData[nombreCampus];

    if (datos) {
      const img = item.querySelector(".campus-image");
      const enlace = item.querySelector(".campus-label");

      if (img) img.src = datos.imagen;
      if (enlace) enlace.href = datos.enlace;
    } else {
      console.warn(`No se encontraron datos para el campus: ${nombreCampus}`);
    }
  });
}

function mostrarSiguenos(imagenSrc) {
    const contenedor = document.querySelector(".siguenos-contenedor");

    if (!contenedor) {
        console.error("No se encontró el contenedor .siguenos-contenedor");
        return;
    }

    contenedor.innerHTML = ""; 

    const img = document.createElement("img");
    img.className = "siguenos";
    img.src = imagenSrc;
    img.alt = "Síguenos";
    img.setAttribute("data-aos", "fade-up");
    img.setAttribute("data-aos-duration", "1200");    

    contenedor.appendChild(img);
}

function mostrarFooter(footerData) {
    const contenedor = document.querySelector(".pie_contenedor");
    if (!contenedor) return console.error("No se encontró el contenedor .pie_contenedor");
    contenedor.innerHTML = "";

    const row = document.createElement("div");
    row.className = "row justify-content-between align-items-center w-100";

    // Columna izquierda
    const colIzq = document.createElement("div");
    colIzq.className = "col-12 col-md-6 text-center mb-4 columna_izquierda";
    colIzq.setAttribute("data-aos", "fade-right");
    colIzq.setAttribute("data-aos-duration", "1000");

    const divImagenes = document.createElement("div");
    divImagenes.className = "imagenes";
    const logo = document.createElement("img");
    logo.src = footerData.imagen;
    logo.alt = "EP Footer Logo";
    divImagenes.appendChild(logo);

    const derechosDiv = document.createElement("div");
    derechosDiv.className = "derechos mt-3";
    const small = document.createElement("small");
    const bold = document.createElement("b");
    bold.innerHTML = footerData.derechos.replace(/\n/g, "<br>");
    small.appendChild(bold);
    derechosDiv.appendChild(small);

    colIzq.appendChild(divImagenes);
    colIzq.appendChild(derechosDiv);

    // Columna derecha (info)
    const colDer = document.createElement("div");
    colDer.className = "col-12 col-md-6 info";
    colDer.setAttribute("data-aos", "fade-left");
    colDer.setAttribute("data-aos-duration", "1000");

    // Ubicación
    const ubicacionItem = document.createElement("div");
    ubicacionItem.className = "item mb-4";
    const iconUbicacion = document.createElement("i");
    iconUbicacion.className = "fa-solid fa-location-dot";
    const divUbicacion = document.createElement("div");
    const tituloUbicacion = document.createElement("h2");
    tituloUbicacion.textContent = footerData.informacion[0].titulo;
    const pUbicacion = document.createElement("p");
    pUbicacion.innerHTML = footerData.informacion[0].texto.join("<br>");
    divUbicacion.appendChild(tituloUbicacion);
    divUbicacion.appendChild(pUbicacion);
    ubicacionItem.appendChild(iconUbicacion);
    ubicacionItem.appendChild(divUbicacion);

    // Teléfonos
    const telefonoItem = document.createElement("div");
    telefonoItem.className = "item";
    const iconTelefono = document.createElement("i");
    iconTelefono.className = "fa-solid fa-phone-volume";
    const pTelefonos = document.createElement("p");
    pTelefonos.innerHTML = footerData.informacion[1].telefonos.join("<br>");
    telefonoItem.appendChild(iconTelefono);
    telefonoItem.appendChild(pTelefonos);

    colDer.appendChild(ubicacionItem);
    colDer.appendChild(telefonoItem);

    row.appendChild(colIzq);
    row.appendChild(colDer);
    contenedor.appendChild(row);
}

