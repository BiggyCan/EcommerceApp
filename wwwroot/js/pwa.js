(function () {

    "use strict";


    // ==========================================
    // CONFIGURACIÓN
    // ==========================================

    const SESSION_KEY =
        "biggame_pwa_prompt_mostrado";


    let deferredPrompt = null;

    let installModal = null;


    // ==========================================
    // REGISTRAR SERVICE WORKER
    // ==========================================

    if ("serviceWorker" in navigator) {

        window.addEventListener(
            "load",
            async function () {

                try {

                    const registration =
                        await navigator.serviceWorker.register(
                            "/service-worker.js"
                        );


                    console.log(
                        "BigGame PWA registrada correctamente:",
                        registration.scope
                    );

                }
                catch (error) {

                    console.error(
                        "Error al registrar la PWA:",
                        error
                    );

                }

            }
        );

    }


    // ==========================================
    // COMPROBAR SI YA ESTÁ INSTALADA
    // ==========================================

    function estaInstalada() {

        return (
            window.matchMedia(
                "(display-mode: standalone)"
            ).matches
            ||
            window.navigator.standalone === true
        );

    }


    // ==========================================
    // COMPROBAR SI YA MOSTRAMOS EL AVISO
    // ==========================================

    function avisoYaMostrado() {

        try {

            return (
                sessionStorage.getItem(
                    SESSION_KEY
                )
                ===
                "1"
            );

        }
        catch (error) {

            return false;

        }

    }


    // ==========================================
    // MARCAR AVISO COMO MOSTRADO
    // ==========================================

    function marcarAvisoComoMostrado() {

        try {

            sessionStorage.setItem(
                SESSION_KEY,
                "1"
            );

        }
        catch (error) {

            console.warn(
                "No se pudo guardar el estado del aviso PWA.",
                error
            );

        }

    }


    // ==========================================
    // CREAR VENTANA DE INSTALACIÓN
    // ==========================================

    function crearVentanaInstalacion() {

        if (installModal) {
            return installModal;
        }


        const overlay =
            document.createElement(
                "div"
            );


        overlay.id =
            "biggame-pwa-install";


        overlay.innerHTML = `
            <div class="biggame-pwa-backdrop"></div>

            <div class="biggame-pwa-box">

                <button
                    type="button"
                    class="biggame-pwa-close"
                    aria-label="Cerrar"
                >
                    ×
                </button>

                <div class="biggame-pwa-icon">
                    🎮
                </div>

                <div class="biggame-pwa-content">

                    <div class="biggame-pwa-label">
                        APLICACIÓN BIGGAME
                    </div>

                    <h2>
                        Instala BigGame
                    </h2>

                    <p>
                        Añade BigGame a tu teléfono para acceder
                        rápidamente a la tienda como una aplicación.
                    </p>

                    <div class="biggame-pwa-benefits">

                        <span>
                            ✓ Acceso desde tu pantalla de inicio
                        </span>

                        <span>
                            ✓ Experiencia tipo aplicación
                        </span>

                        <span>
                            ✓ Acceso más rápido
                        </span>

                    </div>

                    <div class="biggame-pwa-actions">

                        <button
                            type="button"
                            class="biggame-pwa-install-button"
                        >
                            📲 Instalar BigGame
                        </button>

                        <button
                            type="button"
                            class="biggame-pwa-later-button"
                        >
                            Ahora no
                        </button>

                    </div>

                </div>

            </div>
        `;


        const style =
            document.createElement(
                "style"
            );


        style.textContent = `

            #biggame-pwa-install {
                position: fixed;
                inset: 0;
                z-index: 999999;
                display: none;
                align-items: center;
                justify-content: center;
                padding: 20px;
                font-family:
                    Arial,
                    Helvetica,
                    sans-serif;
            }


            #biggame-pwa-install.show {
                display: flex;
            }


            .biggame-pwa-backdrop {
                position: absolute;
                inset: 0;

                background:
                    rgba(
                        4,
                        8,
                        18,
                        .72
                    );

                backdrop-filter:
                    blur(4px);
            }


            .biggame-pwa-box {
                width: 100%;
                max-width: 390px;

                position: relative;
                z-index: 1;

                overflow: hidden;

                padding:
                    28px
                    24px
                    24px;

                border:
                    1px solid
                    rgba(
                        255,
                        255,
                        255,
                        .16
                    );

                border-radius: 22px;

                background:
                    linear-gradient(
                        145deg,
                        #081120,
                        #0f2450
                    );

                color: white;

                box-shadow:
                    0
                    24px
                    70px
                    rgba(
                        0,
                        0,
                        0,
                        .45
                    );

                animation:
                    biggameInstallIn
                    .28s
                    ease-out;
            }


            @keyframes biggameInstallIn {

                from {
                    opacity: 0;

                    transform:
                        translateY(22px)
                        scale(.97);
                }

                to {
                    opacity: 1;

                    transform:
                        translateY(0)
                        scale(1);
                }

            }


            .biggame-pwa-close {
                position: absolute;
                top: 12px;
                right: 14px;

                width: 34px;
                height: 34px;

                border: none;
                border-radius: 50%;

                background:
                    rgba(
                        255,
                        255,
                        255,
                        .1
                    );

                color: white;

                font-size: 24px;
                line-height: 1;

                cursor: pointer;
            }


            .biggame-pwa-icon {
                width: 72px;
                height: 72px;

                margin:
                    0
                    auto
                    18px;

                display: flex;
                align-items: center;
                justify-content: center;

                border-radius: 20px;

                background:
                    linear-gradient(
                        135deg,
                        #06bfff,
                        #2563eb
                    );

                font-size: 37px;

                box-shadow:
                    0
                    10px
                    30px
                    rgba(
                        37,
                        99,
                        235,
                        .4
                    );
            }


            .biggame-pwa-content {
                text-align: center;
            }


            .biggame-pwa-label {
                margin-bottom: 7px;

                color: #60a5fa;

                font-size: 10px;
                font-weight: 900;

                letter-spacing: 1.2px;
            }


            .biggame-pwa-content h2 {
                margin:
                    0
                    0
                    10px;

                color: white;

                font-size: 27px;
                font-weight: 900;
            }


            .biggame-pwa-content p {
                margin:
                    0
                    auto
                    18px;

                max-width: 320px;

                color: #cbd5e1;

                font-size: 14px;
                line-height: 1.5;
            }


            .biggame-pwa-benefits {
                margin-bottom: 22px;

                display: flex;
                flex-direction: column;
                gap: 8px;

                color: #dbeafe;

                font-size: 12px;

                text-align: left;
            }


            .biggame-pwa-benefits span {
                padding:
                    9px
                    11px;

                border-radius: 9px;

                background:
                    rgba(
                        255,
                        255,
                        255,
                        .06
                    );
            }


            .biggame-pwa-actions {
                display: flex;
                flex-direction: column;
                gap: 9px;
            }


            .biggame-pwa-install-button {
                min-height: 49px;

                border: none;
                border-radius: 11px;

                background:
                    linear-gradient(
                        135deg,
                        #06bfff,
                        #2563eb
                    );

                color: white;

                font-size: 14px;
                font-weight: 900;

                cursor: pointer;
            }


            .biggame-pwa-install-button:active {
                transform:
                    scale(.98);
            }


            .biggame-pwa-later-button {
                min-height: 43px;

                border:
                    1px solid
                    rgba(
                        255,
                        255,
                        255,
                        .14
                    );

                border-radius: 11px;

                background:
                    rgba(
                        255,
                        255,
                        255,
                        .06
                    );

                color: #e2e8f0;

                font-size: 13px;
                font-weight: 700;

                cursor: pointer;
            }


            @media (max-width: 480px) {

                #biggame-pwa-install {
                    align-items: flex-end;
                    padding: 12px;
                }


                .biggame-pwa-box {
                    max-width: none;

                    border-radius:
                        22px
                        22px
                        18px
                        18px;
                }

            }

        `;


        document.head.appendChild(
            style
        );


        document.body.appendChild(
            overlay
        );


        const installButton =
            overlay.querySelector(
                ".biggame-pwa-install-button"
            );


        const laterButton =
            overlay.querySelector(
                ".biggame-pwa-later-button"
            );


        const closeButton =
            overlay.querySelector(
                ".biggame-pwa-close"
            );


        installButton.addEventListener(
            "click",
            async function () {

                marcarAvisoComoMostrado();


                if (!deferredPrompt) {

                    cerrarVentana();

                    return;

                }


                try {

                    deferredPrompt.prompt();


                    const result =
                        await deferredPrompt.userChoice;


                    console.log(
                        "Resultado instalación BigGame:",
                        result.outcome
                    );


                    deferredPrompt =
                        null;


                    cerrarVentana();

                }
                catch (error) {

                    console.error(
                        "Error al instalar BigGame:",
                        error
                    );


                    cerrarVentana();

                }

            }
        );


        laterButton.addEventListener(
            "click",
            function () {

                marcarAvisoComoMostrado();

                cerrarVentana();

            }
        );


        closeButton.addEventListener(
            "click",
            function () {

                marcarAvisoComoMostrado();

                cerrarVentana();

            }
        );


        installModal =
            overlay;


        return overlay;

    }


    // ==========================================
    // MOSTRAR VENTANA
    // ==========================================

    function mostrarVentana() {

        if (estaInstalada()) {
            return;
        }


        if (avisoYaMostrado()) {

            console.log(
                "El aviso PWA ya fue mostrado durante esta sesión."
            );

            return;

        }


        const modal =
            crearVentanaInstalacion();


        marcarAvisoComoMostrado();


        window.setTimeout(
            function () {

                modal.classList.add(
                    "show"
                );

            },
            700
        );

    }


    // ==========================================
    // CERRAR VENTANA
    // ==========================================

    function cerrarVentana() {

        if (!installModal) {
            return;
        }


        installModal.classList.remove(
            "show"
        );

    }


    // ==========================================
    // CHROME DETECTA QUE SE PUEDE INSTALAR
    // ==========================================

    window.addEventListener(
        "beforeinstallprompt",
        function (event) {

            event.preventDefault();


            deferredPrompt =
                event;


            console.log(
                "BigGame está disponible para instalar."
            );


            mostrarVentana();

        }
    );


    // ==========================================
    // INSTALACIÓN COMPLETADA
    // ==========================================

    window.addEventListener(
        "appinstalled",
        function () {

            console.log(
                "BigGame fue instalada correctamente."
            );


            deferredPrompt =
                null;


            marcarAvisoComoMostrado();


            cerrarVentana();

        }
    );


})();