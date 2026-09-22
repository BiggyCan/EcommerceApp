const CACHE_NAME = "biggame-cache-v1";

const APP_SHELL = [
    "/",
    "/Products",
    "/manifest.json",
    "/icons/biggame-icon.svg"
];


self.addEventListener(
    "install",
    function (event) {

        event.waitUntil(
            caches
                .open(CACHE_NAME)
                .then(
                    function (cache) {

                        return cache.addAll(
                            APP_SHELL
                        );

                    }
                )
        );

        self.skipWaiting();
    }
);


self.addEventListener(
    "activate",
    function (event) {

        event.waitUntil(
            caches
                .keys()
                .then(
                    function (cacheNames) {

                        return Promise.all(
                            cacheNames
                                .filter(
                                    function (cacheName) {

                                        return cacheName
                                            !== CACHE_NAME;

                                    }
                                )
                                .map(
                                    function (cacheName) {

                                        return caches.delete(
                                            cacheName
                                        );

                                    }
                                )
                        );

                    }
                )
        );

        self.clients.claim();
    }
);


self.addEventListener(
    "fetch",
    function (event) {

        if (
            event.request.method
            !== "GET"
        ) {
            return;
        }


        event.respondWith(

            fetch(
                event.request
            )
                .then(
                    function (response) {

                        if (
                            !response
                            ||
                            response.status !== 200
                            ||
                            response.type === "opaque"
                        ) {
                            return response;
                        }


                        const responseCopy =
                            response.clone();


                        caches
                            .open(CACHE_NAME)
                            .then(
                                function (cache) {

                                    cache.put(
                                        event.request,
                                        responseCopy
                                    );

                                }
                            );


                        return response;

                    }
                )
                .catch(
                    function () {

                        return caches.match(
                            event.request
                        );

                    }
                )

        );

    }
);