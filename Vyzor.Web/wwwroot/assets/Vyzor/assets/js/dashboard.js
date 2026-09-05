document.addEventListener("DOMContentLoaded", function () {

    const toggle = document.getElementById("sidebarToggle");
    const sidebar = document.querySelector(".vyzor-sidebar");

    if (!toggle || !sidebar) {
        return;
    }

    toggle.addEventListener("click", function () {
        sidebar.classList.toggle("show");
    });

});