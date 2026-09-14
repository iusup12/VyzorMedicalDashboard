(function () {
    'use strict';

    const ajaxHeaders = {
        'X-Requested-With': 'XMLHttpRequest'
    };

    let isLoading = false;

    async function loadDoctors(url, pushState = true) {

        if (isLoading) {
            return;
        }

        isLoading = true;

        const currentResults =
            document.querySelector('[data-doctor-results]');

        if (!currentResults) {
            console.error(
                'Element [data-doctor-results] was not found.'
            );

            isLoading = false;
            return;
        }

        currentResults.classList.add('opacity-50');

        try {

            const response = await fetch(url, {
                method: 'GET',
                headers: ajaxHeaders,
                credentials: 'same-origin'
            });

            if (!response.ok) {
                console.error(
                    'Doctors request failed:',
                    response.status,
                    response.statusText
                );

                return;
            }

            const html = await response.text();

            const parser = new DOMParser();

            const documentHtml =
                parser.parseFromString(html, 'text/html');

            const newResults =
                documentHtml.querySelector(
                    '[data-doctor-results]'
                );

            if (!newResults) {
                console.error(
                    '[data-doctor-results] was not found in response.'
                );

                return;
            }

            currentResults.replaceWith(newResults);

            if (pushState) {
                window.history.pushState(
                    {},
                    '',
                    url
                );
            }

            window.scrollTo({
                top: 0,
                behavior: 'smooth'
            });

        } catch (error) {

            console.error(
                'Error loading doctors:',
                error
            );

        } finally {

            isLoading = false;

            const updatedResults =
                document.querySelector(
                    '[data-doctor-results]'
                );

            if (updatedResults) {
                updatedResults.classList.remove(
                    'opacity-50'
                );
            }
        }
    }


    /*
     * FILTERS
     */
    document.addEventListener(
        'submit',
        function (event) {

            const form =
                event.target.closest(
                    '[data-doctor-filter]'
                );

            if (!form) {
                return;
            }

            event.preventDefault();

            const formData =
                new FormData(form);

            const params =
                new URLSearchParams();

            for (const [key, value] of formData.entries()) {

                if (
                    value === null ||
                    value === undefined ||
                    value === ''
                ) {
                    continue;
                }

                params.append(key, value);
            }

            params.set(
                'PageNumber',
                '1'
            );

            params.set(
                'PageSize',
                getCurrentPageSize()
            );

            const action =
                form.getAttribute('action') ||
                '/Doctors';

            const url =
                buildDoctorsUrl(action, params);

            loadDoctors(url, true);
        }
    );


    /*
     * PAGINATION
     */
    document.addEventListener(
        'click',
        function (event) {

            const link =
                event.target.closest(
                    '[data-doctor-page]'
                );

            if (!link) {
                return;
            }

            if (
                event.ctrlKey ||
                event.metaKey ||
                event.shiftKey ||
                event.altKey ||
                event.button !== 0
            ) {
                return;
            }

            const href =
                link.getAttribute('href');

            if (!href) {
                return;
            }

            event.preventDefault();

            console.log(
                'Doctor pagination:',
                href
            );

            loadDoctors(
                href,
                true
            );
        }
    );


    /*
     * SORTING
     */
    document.addEventListener(
        'click',
        function (event) {

            const link =
                event.target.closest(
                    '[data-doctor-sort]'
                );

            if (!link) {
                return;
            }

            if (
                event.ctrlKey ||
                event.metaKey ||
                event.shiftKey ||
                event.altKey ||
                event.button !== 0
            ) {
                return;
            }

            const href =
                link.getAttribute('href');

            if (!href) {
                return;
            }

            event.preventDefault();

            console.log(
                'Doctor sorting:',
                href
            );

            loadDoctors(
                href,
                true
            );
        }
    );


    /*
     * BACK / FORWARD
     */
    window.addEventListener(
        'popstate',
        function () {

            loadDoctors(
                window.location.href,
                false
            );
        }
    );


    /*
     * HELPERS
     */

    function getCurrentPageSize() {

        const pageSize =
            document.querySelector(
                '[name="PageSize"]'
            );

        if (pageSize && pageSize.value) {
            return pageSize.value;
        }

        return '10';
    }


    function buildDoctorsUrl(
        action,
        params
    ) {

        const separator =
            action.includes('?')
                ? '&'
                : '?';

        return (
            action +
            separator +
            params.toString()
        );
    }

})();