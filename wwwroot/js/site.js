// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
document.addEventListener("DOMContentLoaded", () => {

    const toggleBtn = document.getElementById("themeToggle");
    const root = document.documentElement;

    if (!toggleBtn) return;

    // Load saved theme
    const savedTheme = localStorage.getItem("theme");
    if (savedTheme) {
        root.setAttribute("data-theme", savedTheme);
        toggleBtn.textContent = savedTheme === "dark" ? "☀️" : "🌙";
    }

    toggleBtn.addEventListener("click", () => {
        const currentTheme = root.getAttribute("data-theme");
        const newTheme = currentTheme === "dark" ? "light" : "dark";

        root.setAttribute("data-theme", newTheme);
        localStorage.setItem("theme", newTheme);

        toggleBtn.textContent = newTheme === "dark" ? "☀️" : "🌙";
    });

});
