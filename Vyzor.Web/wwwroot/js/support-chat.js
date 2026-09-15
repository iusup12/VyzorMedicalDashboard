
(function () {
    'use strict';

    const config = window.supportChatConfig;

    if (!config || !config.hubUrl) {
        console.error('Support chat configuration not found.');
        return;
    }

    const chatForm = document.getElementById('support-chat-form');
    const messageInput = document.querySelector('.chat_form');
    const messagesContainer = document.getElementById('support-chat-messages');

    if (!chatForm || !messageInput || !messagesContainer) {
        console.error('Support chat elements not found.');
        return;
    }

    const connection = new signalR.HubConnectionBuilder()
        .withUrl(config.hubUrl)
        .withAutomaticReconnect()
        .build();

    function escapeHtml(value) {
        const div = document.createElement('div');
        div.textContent = value ?? '';
        return div.innerHTML;
    }

    function formatTime(dateValue) {
        const date = new Date(dateValue);

        if (Number.isNaN(date.getTime())) {
            return '';
        }

        return date.toLocaleTimeString([], {
            hour: '2-digit',
            minute: '2-digit'
        });
    }

    function removeEmptyState() {
        const emptyState =
            messagesContainer.querySelector('.chat-empty-state');

        if (emptyState) {
            emptyState.remove();
        }
    }

    function scrollToBottom() {
        const chatBody = document.querySelector('.chat-body');

        if (chatBody) {
            chatBody.scrollTop = chatBody.scrollHeight;
        }

        messagesContainer.scrollTop =
            messagesContainer.scrollHeight;
    }

    function appendMessage(message) {
        if (!message) {
            return;
        }

        removeEmptyState();

        const isSupport =
            message.isFromSupport ??
            message.IsFromSupport ??
            false;

        const text =
            message.message ??
            message.Message ??
            '';

        const createdAt =
            message.createdAtUtc ??
            message.CreatedAtUtc ??
            new Date().toISOString();

        const time = formatTime(createdAt);

        const element = document.createElement('div');

        if (isSupport) {
            element.className = 'chats';

            element.innerHTML = `
    < div class="chat-avatar" >
                    <img src="/assets/Doccure/img/doctor-dashboard/profile-06.jpg"
                         class="dreams_chat"
                         alt="Support">
                </div>

                <div class="chat-content">
                    <div class="chat-profile-name">
                        <h6>
                            Vyzor Support
                            <span>${escapeHtml(time)}</span>
                        </h6>
                    </div>

                    <div class="message-content">
                        ${escapeHtml(text)}
                    </div>
                </div>
`;
        } else {
            element.className = 'chats chats-right';

            element.innerHTML = `
    < div class="chat-avatar" >
                    <img src="/assets/Doccure/img/doctor-dashboard/profile-06.jpg"
                         class="dreams_chat"
                         alt="You">
                </div>

                <div class="chat-content">
                    <div class="chat-profile-name text-end justify-content-end">
                        <h6>
                            You
                            <span>${escapeHtml(time)}</span>
                        </h6>
                    </div>

                    <div class="message-content">
                        ${escapeHtml(text)}
                    </div>
                </div>
`;
        }

        messagesContainer.appendChild(element);

        scrollToBottom();
    }

    function appendDoctorRecommendations(data) {
        if (!data) {
            return;
        }

        const doctors =
            data.doctors ??
            data.Doctors ??
            [];

        const specialization =
            data.specialization ??
            data.Specialization ??
            '';

        if (!Array.isArray(doctors) || doctors.length === 0) {
            return;
        }

        removeEmptyState();

        const wrapper = document.createElement('div');
        wrapper.className = 'chats';

        let cards = '';

        doctors.forEach(function (doctor) {
            const id = doctor.id ?? doctor.Id;
            const name = doctor.fullName ?? doctor.FullName ?? '';
            const specializationName =
                doctor.specializationName ??
                doctor.SpecializationName ??
                specialization;

            const imageUrl =
                doctor.imageUrl ??
                doctor.ImageUrl ??
                '/assets/Doccure/img/doctor-dashboard/profile-06.jpg';

            const price =
                doctor.appointmentPrice ??
                doctor.AppointmentPrice ??
                0;

            const rating =
                doctor.rating ??
                doctor.Rating ??
                0;

            const appointmentUrl =
                config.appointmentUrl +
                '?doctorId=' +
                encodeURIComponent(id);

            cards += `
    < div class="support-doctor-card mb-3" >
        <div class="d-flex align-items-center gap-3">

            <div class="avatar avatar-lg">
                <img src="${escapeHtml(imageUrl)}"
                    class="avatar-img rounded-circle"
                    alt="${escapeHtml(name)}">
            </div>

            <div class="flex-grow-1">
                <h6 class="mb-1">
                    ${escapeHtml(name)}
                </h6>

                <p class="mb-1 text-muted">
                    ${escapeHtml(specializationName)}
                </p>

                <small>
                    Rating: ${escapeHtml(String(rating))}
                    · ${escapeHtml(String(price))} €
                </small>
            </div>

            <a href="${escapeHtml(appointmentUrl)}"
                class="btn btn-primary btn-sm">
                Book
            </a>

        </div>
                </div >
    `;
        });

        wrapper.innerHTML = `
    < div class="chat-avatar" >
                <img src="/assets/Doccure/img/doctor-dashboard/profile-06.jpg"
                     class="dreams_chat"
                     alt="Support">
            </div>

            <div class="chat-content">
                <div class="chat-profile-name">
                    <h6>Vyzor Support</h6>
                </div>

                <div class="message-content">
                    ${specialization
                        ? `Doctors for <strong>${escapeHtml(specialization)}</strong>:`
                        : 'Recommended doctors:'}

                    <div class="mt-3">
                        ${cards}
                    </div>
                </div>
            </div>
`;

        messagesContainer.appendChild(wrapper);

        scrollToBottom();
    }

    connection.on('ChatJoined', function (chatId) {
        console.log('Joined support chat:', chatId);
    });

    connection.on('ReceiveMessage', function (message) {
        console.log('ReceiveMessage:', message);

        appendMessage(message);
    });

    connection.on('DoctorRecommendation', function (data) {
        console.log('DoctorRecommendation:', data);

        appendDoctorRecommendations(data);
    });

    connection.onreconnecting(function (error) {
        console.warn('Support chat reconnecting...', error);
    });

    connection.onreconnected(function (connectionId) {
        console.log(
            'Support chat reconnected:',
            connectionId
        );

        connection.invoke('JoinChat')
            .catch(function (error) {
                console.error(
                    'JoinChat after reconnect failed:',
                    error
                );
            });
    });

    connection.onclose(function (error) {
        console.error(
            'Support chat connection closed:',
            error
        );
    });

    chatForm.addEventListener('submit', async function (event) {
        event.preventDefault();

        const message = messageInput.value.trim();

        if (!message) {
            return;
        }

        if (connection.state !== signalR.HubConnectionState.Connected) {
            console.error(
                'SignalR is not connected. Current state:',
                connection.state
            );
            return;
        }

        const button = chatForm.querySelector('.send-btn');

        if (button) {
            button.disabled = true;
        }

        try {
            console.log('Sending message:', message);

            await connection.invoke(
                'SendMessage',
                message
            );

            console.log('Message sent successfully.');

            messageInput.value = '';
            messageInput.focus();
        }
        catch (error) {
            console.error(
                'SendMessage failed:',
                error
            );
        }
        finally {
            if (button) {
                button.disabled = false;
            }
        }
    });

    const searchForms = [
        document.getElementById('chat-search-form'),
        document.getElementById('message-search-form')
    ];

    searchForms.forEach(function (form) {
        if (!form) {
            return;
        }

        form.addEventListener('submit', function (event) {
            event.preventDefault();
        });
    });

    async function start() {
        try {
            console.log('Starting support chat...');

            await connection.start();

            console.log('Support chat connected.');

            await connection.invoke('JoinChat');
        }
        catch (error) {
            console.error(
                'Support chat connection failed:',
                error
            );
        }
    }

    start();
})();

