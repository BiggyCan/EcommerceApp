(function () {

    // ==========================================
    // BIGGAME - ASISTENTE DE VOZ
    // WEB SPEECH API
    // ==========================================

    const SpeechRecognition =
        window.SpeechRecognition ||
        window.webkitSpeechRecognition;


    // ==========================================
    // PANEL DE MENSAJES
    // ==========================================

    const panel =
        document.createElement("div");

    panel.id =
        "biggame-voice-panel";

    Object.assign(
        panel.style,
        {
            position: "fixed",
            right: "22px",
            bottom: "94px",
            width: "330px",
            maxWidth: "calc(100vw - 44px)",
            padding: "15px 17px",
            borderRadius: "12px",
            background: "#081120",
            color: "#ffffff",
            fontFamily: "Arial, sans-serif",
            fontSize: "13px",
            lineHeight: "1.5",
            zIndex: "99998",
            boxShadow: "0 14px 40px rgba(0,0,0,.30)",
            display: "none"
        }
    );

    document.body.appendChild(
        panel
    );


    // ==========================================
    // BOTÓN FLOTANTE
    // ==========================================

    const button =
        document.createElement("button");

    button.id =
        "biggame-voice-button";

    button.type =
        "button";

    button.innerHTML =
        "🎙️";

    button.title =
        "Asistente de voz BigGame";

    button.setAttribute(
        "aria-label",
        "Activar asistente de voz"
    );

    Object.assign(
        button.style,
        {
            position: "fixed",
            right: "22px",
            bottom: "22px",
            width: "58px",
            height: "58px",
            border: "none",
            borderRadius: "50%",
            background:
                "linear-gradient(135deg, #2563eb, #06bfff)",
            color: "#ffffff",
            fontSize: "25px",
            cursor: "pointer",
            zIndex: "99999",
            boxShadow:
                "0 8px 24px rgba(37,99,235,.40)",
            transition: ".2s"
        }
    );

    document.body.appendChild(
        button
    );


    // ==========================================
    // MENSAJES
    // ==========================================

    function mostrarMensaje(
        mensaje,
        tiempo = 5000
    ) {

        panel.innerHTML =
            mensaje;

        panel.style.display =
            "block";

        clearTimeout(
            panel._timer
        );

        panel._timer =
            setTimeout(
                function () {

                    panel.style.display =
                        "none";

                },
                tiempo
            );
    }


    // ==========================================
    // VOZ DE RESPUESTA
    // ==========================================

    function hablar(
        mensaje
    ) {

        if (
            !("speechSynthesis" in window)
        ) {
            return;
        }

        window.speechSynthesis.cancel();

        const mensajeVoz =
            new SpeechSynthesisUtterance(
                mensaje
            );

        mensajeVoz.lang =
            "es-BO";

        mensajeVoz.rate =
            1;

        mensajeVoz.pitch =
            1;

        window.speechSynthesis.speak(
            mensajeVoz
        );
    }


    // ==========================================
    // NAVEGACIÓN
    // ==========================================

    function navegar(
        url,
        mensaje
    ) {

        mostrarMensaje(
            "✅ " + mensaje
        );

        hablar(
            mensaje
        );

        setTimeout(
            function () {

                window.location.href =
                    url;

            },
            650
        );
    }


    // ==========================================
    // BUSCADOR
    // ==========================================

    function buscar(
        texto
    ) {

        const termino =
            texto.trim();

        if (!termino) {

            mostrarMensaje(
                "⚠️ No entendí qué producto deseas buscar."
            );

            hablar(
                "No entendí qué producto deseas buscar."
            );

            return;
        }

        mostrarMensaje(
            "🔎 Buscando <strong>"
            +
            termino
            +
            "</strong>..."
        );

        hablar(
            "Buscando " + termino
        );

        setTimeout(
            function () {

                window.location.href =
                    "/Products?search="
                    +
                    encodeURIComponent(
                        termino
                    );

            },
            650
        );
    }


    // ==========================================
    // PROCESAR COMANDO
    // ==========================================

    function procesarComando(
        texto
    ) {

        const comando =
            texto
                .toLowerCase()
                .trim();

        mostrarMensaje(
            "🎙️ Escuché: <strong>"
            +
            texto
            +
            "</strong>"
        );


        // BUSCAR ALGO

        if (
            comando.startsWith("buscar ")
            ||
            comando.startsWith("busca ")
        ) {

            const termino =
                comando
                    .replace(
                        /^buscar\s+/,
                        ""
                    )
                    .replace(
                        /^busca\s+/,
                        ""
                    );

            buscar(
                termino
            );

            return;
        }


        // CARRITO

        if (
            comando.includes("carrito")
        ) {

            navegar(
                "/Cart",
                "Abriendo el carrito."
            );

            return;
        }


        // PEDIDOS

        if (
            comando.includes("pedido")
        ) {

            navegar(
                "/Orders/MyOrders",
                "Abriendo tus pedidos."
            );

            return;
        }


        // PLAYSTATION

        if (
            comando.includes("playstation")
            ||
            comando.includes("play station")
        ) {

            buscar(
                "PlayStation"
            );

            return;
        }


        // NINTENDO

        if (
            comando.includes("nintendo")
            ||
            comando.includes("switch")
        ) {

            buscar(
                "Nintendo"
            );

            return;
        }


        // XBOX

        if (
            comando.includes("xbox")
        ) {

            buscar(
                "Xbox"
            );

            return;
        }


        // POKEMON

        if (
            comando.includes("pokemon")
            ||
            comando.includes("pokémon")
        ) {

            buscar(
                "Pokémon"
            );

            return;
        }


        // WOLVERINE

        if (
            comando.includes("wolverine")
        ) {

            buscar(
                "Wolverine"
            );

            return;
        }


        // REPORTES

        if (
            comando.includes("reporte")
        ) {

            navegar(
                "/Reports",
                "Abriendo el centro de reportes."
            );

            return;
        }


        // ADMINISTRACIÓN

        if (
            comando.includes("administración")
            ||
            comando.includes("administracion")
            ||
            comando.includes("administrador")
        ) {

            navegar(
                "/Admin",
                "Abriendo el panel de administración."
            );

            return;
        }


        // INICIO

        if (
            comando.includes("inicio")
            ||
            comando.includes("tienda")
            ||
            comando.includes("página principal")
            ||
            comando.includes("pagina principal")
        ) {

            navegar(
                "/Products",
                "Volviendo a la tienda BigGame."
            );

            return;
        }


        // AYUDA

        if (
            comando.includes("ayuda")
            ||
            comando.includes("qué puedo decir")
            ||
            comando.includes("que puedo decir")
        ) {

            const ayuda =
                "Puedes decir buscar Wolverine, buscar PlayStation, abrir carrito, mis pedidos, Nintendo, Xbox, Pokémon, reportes o ir al inicio.";

            mostrarMensaje(
                "💡 " + ayuda,
                10000
            );

            hablar(
                ayuda
            );

            return;
        }


        // NO RECONOCIDO

        const mensaje =
            "No reconocí ese comando. Puedes decir ayuda para conocer los comandos disponibles.";

        mostrarMensaje(
            "❓ " + mensaje
        );

        hablar(
            mensaje
        );
    }


    // ==========================================
    // COMPROBAR WEB SPEECH API
    // ==========================================

    if (!SpeechRecognition) {

        button.addEventListener(
            "click",
            function () {

                mostrarMensaje(
                    "⚠️ Este navegador no permite utilizar SpeechRecognition. Prueba BigGame en Google Chrome o Microsoft Edge.",
                    10000
                );

            }
        );

        return;
    }


    // ==========================================
    // RECONOCIMIENTO
    // ==========================================

    const recognition =
        new SpeechRecognition();

    recognition.lang =
        "es-BO";

    recognition.continuous =
        false;

    recognition.interimResults =
        false;

    recognition.maxAlternatives =
        1;


    let escuchando =
        false;


    // ==========================================
    // PEDIR PERMISO DEL MICRÓFONO
    // ==========================================

    async function solicitarMicrofono() {

        if (
            !navigator.mediaDevices
            ||
            !navigator.mediaDevices.getUserMedia
        ) {

            throw new Error(
                "microphone-api-unavailable"
            );
        }

        const stream =
            await navigator.mediaDevices
                .getUserMedia(
                    {
                        audio: true
                    }
                );

        stream
            .getTracks()
            .forEach(
                function (track) {

                    track.stop();

                }
            );
    }


    // ==========================================
    // BOTÓN PRINCIPAL
    // ==========================================

    button.addEventListener(
        "click",
        async function () {

            if (escuchando) {

                recognition.stop();

                return;
            }


            mostrarMensaje(
                "🎙️ Solicitando acceso al micrófono..."
            );


            try {

                await solicitarMicrofono();

                mostrarMensaje(
                    "✅ Micrófono permitido. Habla ahora..."
                );


                setTimeout(
                    function () {

                        try {

                            recognition.start();

                        }
                        catch (error) {

                            console.error(
                                error
                            );

                            mostrarMensaje(
                                "⚠️ No se pudo iniciar el reconocimiento. Intenta nuevamente."
                            );

                        }

                    },
                    250
                );

            }
            catch (error) {

                console.error(
                    "Permiso de micrófono:",
                    error
                );


                if (
                    error.name === "NotAllowedError"
                    ||
                    error.name === "PermissionDeniedError"
                ) {

                    mostrarMensaje(
                        "🚫 El micrófono está bloqueado. Pulsa el icono junto a localhost en la barra de direcciones, permite Micrófono y recarga la página.",
                        12000
                    );

                    return;
                }


                if (
                    error.name === "NotFoundError"
                ) {

                    mostrarMensaje(
                        "⚠️ Windows no detecta ningún micrófono disponible.",
                        9000
                    );

                    return;
                }


                mostrarMensaje(
                    "⚠️ No se pudo acceder al micrófono. Revisa los permisos del navegador.",
                    9000
                );

            }

        }
    );


    // ==========================================
    // INICIO DE ESCUCHA
    // ==========================================

    recognition.onstart =
        function () {

            escuchando =
                true;

            button.innerHTML =
                "🔴";

            button.style.transform =
                "scale(1.12)";

            button.style.background =
                "linear-gradient(135deg, #dc2626, #ef4444)";

            mostrarMensaje(
                "🔴 Escuchando... di un comando."
            );
        };


    // ==========================================
    // RESULTADO
    // ==========================================

    recognition.onresult =
        function (event) {

            const texto =
                event
                    .results[0][0]
                    .transcript;

            procesarComando(
                texto
            );
        };


    // ==========================================
    // FINALIZAR
    // ==========================================

    recognition.onend =
        function () {

            escuchando =
                false;

            button.innerHTML =
                "🎙️";

            button.style.transform =
                "scale(1)";

            button.style.background =
                "linear-gradient(135deg, #2563eb, #06bfff)";
        };


    // ==========================================
    // ERRORES
    // ==========================================

    recognition.onerror =
        function (event) {

            escuchando =
                false;

            button.innerHTML =
                "🎙️";

            button.style.transform =
                "scale(1)";

            button.style.background =
                "linear-gradient(135deg, #2563eb, #06bfff)";


            if (
                event.error === "not-allowed"
            ) {

                mostrarMensaje(
                    "🚫 Permite el micrófono desde los permisos del navegador.",
                    10000
                );

                return;
            }


            if (
                event.error === "no-speech"
            ) {

                mostrarMensaje(
                    "🎙️ No escuché ninguna voz. Pulsa nuevamente e inténtalo otra vez."
                );

                return;
            }


            if (
                event.error === "network"
            ) {

                mostrarMensaje(
                    "🌐 El reconocimiento de voz tuvo un problema de red. Intenta nuevamente."
                );

                return;
            }


            mostrarMensaje(
                "⚠️ Error de reconocimiento: "
                +
                event.error
            );
        };


    console.log(
        "BigGame Voice Assistant cargado correctamente."
    );

})();