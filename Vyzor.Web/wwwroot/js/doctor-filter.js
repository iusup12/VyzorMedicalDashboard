$(document).ready(function () {

    const form = $("#doctor-filter-form");

    if (!form.length) {
        return;
    }

    function loadDoctors(url) {

        const results = $("#doctor-results");

        results.addClass("loading");

        $.ajax({
            url: url,
            type: "GET",
            success: function (response) {

                results.html(response);
            },
            error: function (xhr) {

                console.error("Ошибка загрузки врачей:", xhr.status);

                results.html(
                    '<div class="alert alert-danger">' +
                    'Не удалось загрузить список врачей.' +
                    '</div>'
                );
            },
            complete: function () {

                results.removeClass("loading");
            }
        });
    }

    form.on("submit", function (e) {

        e.preventDefault();

        const url = form.attr("action") || "/Doctors";

        const query = form.serialize();

        loadDoctors(url + "?" + query);
    });

});