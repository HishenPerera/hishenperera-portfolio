document.addEventListener("DOMContentLoaded", function () {
    const html = document.documentElement;

    // THEME TOGGLE
    const toggleBtn = document.getElementById("themeToggle");
    const mobileToggleBtn = document.getElementById("mobileThemeToggle");

    function updateTheme(theme) {
        html.setAttribute("data-theme", theme);
        localStorage.setItem("theme", theme);

        document.querySelectorAll(".theme-btn .sun-icon").forEach(i => i.style.opacity = theme === "dark" ? "0" : "1");
        document.querySelectorAll(".theme-btn .moon-icon").forEach(i => i.style.opacity = theme === "dark" ? "1" : "0");
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

const typingElements = document.querySelectorAll(".typing-text");

typingElements.forEach(el => {
    const words = JSON.parse(el.getAttribute("data-words"));
    let wordIndex = 0;
    let letterIndex = 0;
    let typing = true;

    const typeSpeed = 100;   // typing speed in ms
    const eraseSpeed = 50;   // deleting speed in ms
    const delayBetweenWords = 1500; // pause after a word is typed

    // Add a span for the blinking cursor if not already
    if (!el.querySelector(".cursor")) {
        const cursor = document.createElement("span");
        cursor.classList.add("cursor");
        cursor.textContent = "|";
        el.appendChild(cursor);
    }

    const cursorSpan = el.querySelector(".cursor");

    function type() {
        const currentWord = words[wordIndex];

        if (typing) {
            el.textContent = currentWord.slice(0, letterIndex + 1);
            el.appendChild(cursorSpan); // keep cursor at end
            letterIndex++;

            if (letterIndex === currentWord.length) {
                typing = false;
                setTimeout(type, delayBetweenWords);
            } else {
                setTimeout(type, typeSpeed);
            }
        } else {
            el.textContent = currentWord.slice(0, letterIndex - 1);
            el.appendChild(cursorSpan);
            letterIndex--;

            if (letterIndex === 0) {
                typing = true;
                wordIndex = (wordIndex + 1) % words.length;
                setTimeout(type, typeSpeed);
            } else {
                setTimeout(type, eraseSpeed);
            }
        }
    }

    type();
});

// Remove toast manually
    function dismissToast() {
        const toast = document.getElementById('toastSuccess');
        if (toast) toast.remove();
    }

    // Auto-remove after 5 seconds
    window.addEventListener('DOMContentLoaded', () => {
        const toast = document.getElementById('toastSuccess');
        if (toast) {
            setTimeout(() => {
                toast.remove();
            }, 5000); // 5 seconds
        }
    });    

