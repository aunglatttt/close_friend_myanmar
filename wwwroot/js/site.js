// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
document.addEventListener("DOMContentLoaded", function () {
    let arrow = document.querySelectorAll(".arrow");
    for (var i = 0; i < arrow.length; i++) {
        arrow[i].addEventListener("click", (e) => {
            let arrowParent = e.target.parentElement.parentElement;
            arrowParent.classList.toggle("showMenu");
        });
    }

    let sidebar = document.querySelector(".sidebar");
    let sidebarBtn = document.querySelector(".bi-list");

    // Check localStorage for sidebar state
    const isSidebarClosed = localStorage.getItem('sidebarClosed') === 'true';
    if (isSidebarClosed) {
        sidebar.classList.add("close");
    }

    sidebarBtn.addEventListener("click", () => {
        sidebar.classList.toggle("close");
        // Save the state to localStorage
        localStorage.setItem('sidebarClosed', sidebar.classList.contains("close"));
    });
});





