
(function () {
    'use strict';

    const ajaxHeaders = {
        'X-Requested-With': 'XMLHttpRequest'
    };

    document.addEventListener('DOMContentLoaded', function () {

        const form =
            document.getElementById('appointment-booking-form');

        const button =
            document.getElementById('add-to-cart-button');

        const buttonText =
            document.getElementById('add-to-cart-text');

        const buttonLoading =
            document.getElementById('add-to-cart-loading');

        const buttonIcon =
            document.getElementById('add-to-cart-icon');

        const message =
            document.getElementById('appointment-cart-message');

        const dateInput =
            document.getElementById('appointment-date');

        if (!form || !button || !dateInput) {
            return;
        }

        form.addEventListener('submit', async function (event) {

            event.preventDefault();

            hideMessage();

            const doctorIdInput =
                form.querySelector('[name="DoctorId"]');

            if (!doctorIdInput) {
                showMessage(
                    'danger',
                    'Doctor was not specified.'
                );

                return;
            }

            const doctorId =
                parseInt(doctorIdInput.value, 10);

            if (!doctorId || doctorId <= 0) {
                showMessage(
                    'danger',
                    'Invalid doctor.'
                );

                return;
            }

            if (!dateInput.value) {
                showMessage(
                    'danger',
                    'Please select appointment date and time.'
                );

                dateInput.focus();

                return;
            }

            const selectedDate =
                new Date(dateInput.value);

            if (Number.isNaN(selectedDate.getTime())) {
                showMessage(
                    'danger',
                    'Invalid appointment date.'
                );

                return;
            }

            if (selectedDate <= new Date()) {
                showMessage(
                    'danger',
                    'Appointment date must be in the future.'
                );

                dateInput.focus();

                return;
            }

            const tokenInput =
                form.querySelector(
                    'input[name="__RequestVerificationToken"]'
                );

            if (!tokenInput) {
                showMessage(
                    'danger',
                    'Security token was not found.'
                );

                return;
            }

            setLoading(true);

            try {

                const body =
                    new URLSearchParams();

                body.append(
                    'doctorId',
                    doctorId.toString()
                );

                body.append(
                    'appointmentDate',
                    dateInput.value
                );

                // ВАЖНО:
                // AntiForgeryToken отправляем в BODY,
                // потому что AppointmentCartController
                // использует [ValidateAntiForgeryToken].
                body.append(
                    '__RequestVerificationToken',
                    tokenInput.value
                );

                const response = await fetch(
                    '/Admin/AppointmentCart/Add',
                    {
                        method: 'POST',

                        headers: {
                            ...ajaxHeaders,
                            'Content-Type':
                                'application/x-www-form-urlencoded; charset=UTF-8'
                        },

                        body: body.toString()
                    }
                );

                let data = null;

                try {
                    data = await response.json();
                }
                catch {
                    data = null;
                }

                if (!response.ok) {

                    console.error(
                        'Appointment cart request failed:',
                        response.status,
                        data
                    );

                    showMessage(
                        'danger',
                        data?.message ||
                        `Request failed (${response.status}).`
                    );

                    return;
                }

                if (!data || !data.success) {

                    showMessage(
                        'danger',
                        data?.message ||
                        'Unable to add appointment to cart.'
                    );

                    return;
                }

                updateCartCount(data.count);

                showMessage(
                    'success',
                    data.message ||
                    'Appointment added to cart.'
                );

                setTimeout(function () {

                    window.location.href =
                        '/Admin/AppointmentCart';

                }, 700);

            }
            catch (error) {

                console.error(
                    'Appointment cart error:',
                    error
                );

                showMessage(
                    'danger',
                    'An error occurred while adding the appointment to cart.'
                );

            }
            finally {

                setLoading(false);
            }
        });


        function setLoading(isLoading) {

            button.disabled = isLoading;

            if (isLoading) {

                buttonText.classList.add('d-none');

                buttonLoading.classList.remove('d-none');

                buttonIcon.classList.add('d-none');

            }
            else {

                buttonText.classList.remove('d-none');

                buttonLoading.classList.add('d-none');

                buttonIcon.classList.remove('d-none');
            }
        }


        function showMessage(type, text) {

            message.className =
                `alert alert-${type}`;

            message.textContent = text;

            message.classList.remove('d-none');
        }


        function hideMessage() {

            message.className =
                'alert d-none';

            message.textContent = '';
        }


        function updateCartCount(count) {

            document
                .querySelectorAll(
                    '[data-appointment-cart-count]'
                )
                .forEach(function (element) {

                    element.textContent =
                        count ?? 0;
                });
        }

    });

})();

