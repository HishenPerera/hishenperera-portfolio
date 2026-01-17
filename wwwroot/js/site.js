document.addEventListener("DOMContentLoaded", function () {
    const html = document.documentElement;

    // THEME TOGGLE
    const toggleBtn = document.getElementById("themeToggle");
    const mobileToggleBtn = document.getElementById("mobileThemeToggle");

    function updateTheme(theme) {
        html.setAttribute("data-theme", theme);
        localStorage.setItem("theme", theme);

        document.querySelectorAll(".theme-btn .sun-icon").forEach(i => i.style.opacity = theme === "dark" ? "1" : "0");
        document.querySelectorAll(".theme-btn .moon-icon").forEach(i => i.style.opacity = theme === "dark" ? "0" : "1");
    }

    let savedTheme = localStorage.getItem("theme") || "light";
    updateTheme(savedTheme);

    toggleBtn.addEventListener("click", () => {
        updateTheme(html.getAttribute("data-theme") === "light" ? "dark" : "light");
    });

    mobileToggleBtn.addEventListener("click", () => {
        updateTheme(html.getAttribute("data-theme") === "light" ? "dark" : "light");
    });

    // MOBILE MENU TOGGLE
    const mobileMenuBtn = document.getElementById("mobileMenuBtn");
    const mobileMenu = document.getElementById("mobileMenu");

    function closeMobileMenu() {
        mobileMenu.classList.remove("show");
        mobileMenuBtn.classList.remove("active");
    }

    mobileMenuBtn.addEventListener("click", () => {
        mobileMenu.classList.toggle("show");
        mobileMenuBtn.classList.toggle("active"); // rotate icon
    });

    // Close mobile menu when link or button clicked
    mobileMenu.querySelectorAll(".nav-link, .btn-contact, .theme-btn").forEach(item => {
        item.addEventListener("click", closeMobileMenu);
    });

    // CLOSE MOBILE MENU ON WINDOW RESIZE
    window.addEventListener("resize", () => {
        if (window.innerWidth >= 992) {
            closeMobileMenu();
        }
    });
});
