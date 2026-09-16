
(function () {
    'use strict';

    const ajaxHeaders = {
        'X-Requested-With': 'XMLHttpRequest'
    };

    document.addEventListener('DOMContentLoaded', function () {

        initRemoveButtons();
        initPayButton();
        updateCartCount();

    });


    // =========================================================
    // REMOVE APPOINTMENT
    // =========================================================

    function initRemoveButtons() {

        document.addEventListener('click', async function (event) {

            const button = event.target.closest(
                '[data-appointment-cart-remove]'
            );

            if (!button) {
                return;
            }

            event.preventDefault();

            const id = button.dataset.appointmentCartRemove;

            if (!id) {
                return;
            }

            const token = getAntiForgeryToken();

            if (!token) {
                showMessage(
                    'Security token was not found.',
                    'danger'
                );
                return;
            }

            button.disabled = true;

            try {

                const formData = new FormData();

                formData.append(
                    '__RequestVerificationToken',
                    token
                );

                formData.append(
                    'id',
                    id
                );

                const response = await fetch(
                    '/Admin/AppointmentCart/Remove',
                    {
                        method: 'POST',
                        headers: ajaxHeaders,
                        credentials: 'same-origin',
                        body: formData
                    }
                );

                const data = await readJson(response);

                if (!response.ok || data.success === false) {
                    throw new Error(
                        data.message ||
                        'Could not remove appointment.'
                    );
                }

                const row = button.closest(
                    '[data-appointment-cart-item]'
                );

                if (row) {
                    row.remove();
                }

                updateSummary(data.summary);
                updateCartBadges(data.count);

                showMessage(
                    data.message ||
                    'Appointment removed from cart.',
                    'success'
                );

                checkEmptyCart();

            }
            catch (error) {

                console.error(
                    'Appointment cart remove error:',
                    error
                );

                showMessage(
                    error.message ||
                    'Could not remove appointment.',
                    'danger'
                );

                button.disabled = false;
            }
        });
    }


    // =========================================================
    // PAY & BOOK
    // =========================================================

    function initPayButton() {

        document.addEventListener('click', async function (event) {

            const button = event.target.closest(
                '#appointment-cart-pay'
            );

            if (!button) {
                return;
            }

            event.preventDefault();

            console.log(
                'Appointment cart Pay button clicked.'
            );

            const token = getAntiForgeryToken();

            if (!token) {

                console.error(
                    'Anti-forgery token was not found.'
                );

                showMessage(
                    'Security token was not found. Please refresh the page.',
                    'danger'
                );

                return;
            }

            const originalHtml = button.innerHTML;

            button.disabled = true;

            button.innerHTML =
                '<span class="spinner-border spinner-border-sm me-2" role="status"></span>' +
                'Processing...';


            try {

                const formData = new FormData();

                formData.append(
                    '__RequestVerificationToken',
                    token
                );


                const response = await fetch(
                    '/Admin/AppointmentCart/Pay',
                    {
                        method: 'POST',
                        headers: ajaxHeaders,
                        credentials: 'same-origin',
                        body: formData
                    }
                );


                console.log(
                    'Pay response:',
                    response.status
                );


                const data = await readJson(response);


                if (!response.ok || data.success === false) {

                    throw new Error(
                        data.message ||
                        'Payment could not be completed.'
                    );
                }


                showMessage(
                    data.message ||
                    'Payment successful. Appointments have been booked.',
                    'success'
                );


                // Cart is now empty because PayAsync
                // converted cart items into real appointments.

                clearCartItems();

                updateCartBadges(0);

                updateSummary({
                    itemsCount: 0,
                    subTotal: 0,
                    discountPercent: 0,
                    discountAmount: 0,
                    total: 0,
                    subscriptionName: null
                });


                button.innerHTML =
                    '<i class="ti ti-check me-1"></i>' +
                    'Paid & Booked';


                button.disabled = true;


                // Show empty-cart state.
                showEmptyCartState();


            }
            catch (error) {

                console.error(
                    'Appointment cart payment error:',
                    error
                );


                showMessage(
                    error.message ||
                    'Payment could not be completed.',
                    'danger'
                );


                button.disabled = false;

                button.innerHTML = originalHtml;
            }

        });
    }


    // =========================================================
    // CART COUNT
    // =========================================================

    async function updateCartCount() {

        try {

            const response = await fetch(
                '/Admin/AppointmentCart/Count',
                {
                    method: 'GET',
                    headers: ajaxHeaders,
                    credentials: 'same-origin'
                }
            );

            if (!response.ok) {
                return;
            }

            const data = await response.json();

            updateCartBadges(data.count);

        }
        catch (error) {

            console.warn(
                'Could not load appointment cart count.',
                error
            );
        }
    }


    // =========================================================
    // SUMMARY
    // =========================================================

    function updateSummary(summary) {

        if (!summary) {
            return;
        }


        setText(
            '#appointment-cart-count',
            summary.itemsCount
        );


        setText(
            '#appointment-cart-subtotal',
            formatPrice(summary.subTotal)
        );


        setText(
            '#appointment-cart-discount',
            formatPrice(summary.discountAmount)
        );


        setText(
            '#appointment-cart-final-total',
            formatPrice(summary.total)
        );


        const subscriptionElement =
            document.querySelector(
                '#appointment-cart-subscription'
            );

        if (subscriptionElement) {

            subscriptionElement.textContent =
                summary.subscriptionName ||
                'No subscription';
        }
    }


    // =========================================================
    // BADGES
    // =========================================================

    function updateCartBadges(count) {

        const value = Number(count) || 0;

        document
            .querySelectorAll(
                '[data-appointment-cart-count], .appointment-cart-count'
            )
            .forEach(function (element) {

                element.textContent = value;
            });
    }


    // =========================================================
    // EMPTY CART
    // =========================================================

    function clearCartItems() {

        document
            .querySelectorAll(
                '[data-appointment-cart-item]'
            )
            .forEach(function (item) {

                item.remove();
            });
    }


    function checkEmptyCart() {

        const items =
            document.querySelectorAll(
                '[data-appointment-cart-item]'
            );

        if (items.length === 0) {
            showEmptyCartState();
        }
    }


    function showEmptyCartState() {

        const container =
            document.querySelector(
                '#appointment-cart-items'
            );

        if (!container) {
            return;
        }


        if (
            container.querySelector(
                '[data-appointment-cart-empty]'
            )
        ) {
            return;
        }


        const emptyElement =
            document.createElement('div');

        emptyElement.className =
            'text-center py-5';

        emptyElement.setAttribute(
            'data-appointment-cart-empty',
            ''
        );

        emptyElement.innerHTML = `
            <div class="mb-3">
                <i class="ti ti-calendar-off fs-1 text-muted"></i>
            </div>

            <h5>Appointment cart is empty</h5>

            <p class="text-muted mb-0">
                Add an appointment to continue.
            </p>
        `;

        container.appendChild(
            emptyElement
        );
    }


    // =========================================================
    // HELPERS
    // =========================================================

    function getAntiForgeryToken() {

        const token =
            document.querySelector(
                '#appointment-cart-antiforgery input[name="__RequestVerificationToken"]'
            );

        if (token) {
            return token.value;
        }


        const fallbackToken =
            document.querySelector(
                'input[name="__RequestVerificationToken"]'
            );

        return fallbackToken
            ? fallbackToken.value
            : null;
    }


    async function readJson(response) {

        const text =
            await response.text();

        if (!text) {
            return {};
        }

        try {

            return JSON.parse(text);

        }
        catch {

            console.error(
                'Server returned non-JSON response:',
                text
            );

            throw new Error(
                'Server returned an invalid response.'
            );
        }
    }


    function setText(selector, value) {

        const element =
            document.querySelector(selector);

        if (element) {
            element.textContent = value;
        }
    }


    function formatPrice(value) {

        const number =
            Number(value) || 0;

        return number.toLocaleString(
            'ru-RU',
            {
                minimumFractionDigits: 2,
                maximumFractionDigits: 2
            }
        ) + ' ₽';
    }


    function showMessage(message, type) {

        const element =
            document.querySelector(
                '#appointment-cart-message'
            );

        if (!element) {

            console.log(
                `[${type}] ${message}`
            );

            return;
        }


        element.className =
            `alert alert-${type} mt-3`;

        element.textContent =
            message;

        element.classList.remove(
            'd-none'
        );


        window.setTimeout(
            function () {

                element.classList.add(
                    'd-none'
                );

            },
            5000
        );
    }

})();

