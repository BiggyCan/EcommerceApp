(function () {

    if (!("serviceWorker" in navigator)) {
        console.log(
            "Service Worker no disponible en este navegador."
        );

        return;
    }


    window.addEventListener(
        "load",
        async function () {

            try {

                const registration =
                    await navigator
                        .serviceWorker
                        .register(
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

})();