(function ($) {
    "use strict";

    const SIDEBAR_KEY = "sidebar-collapsed";

    // ===== Restore sidebar state on page load =====
    $(document).ready(function () {
        const isCollapsed = localStorage.getItem(SIDEBAR_KEY);

        if (isCollapsed === "true") {
            $("body").addClass("sidebar-toggled");
            $(".sidebar").addClass("toggled");
            $('.sidebar .collapse').collapse('hide');
        }
    });

    // ===== Toggle sidebar =====
    $("#sidebarToggle, #sidebarToggleTop").on("click", function () {

        $("body").toggleClass("sidebar-toggled");
        $(".sidebar").toggleClass("toggled");

        if ($(".sidebar").hasClass("toggled")) {
            $('.sidebar .collapse').collapse('hide');
            localStorage.setItem(SIDEBAR_KEY, "true");
        } else {
            localStorage.setItem(SIDEBAR_KEY, "false");
        }
    });

    // ===== Scroll to top button =====
    $(document).on("scroll", function () {
        if ($(this).scrollTop() > 100) {
            $(".scroll-to-top").fadeIn();
        } else {
            $(".scroll-to-top").fadeOut();
        }
    });

})(jQuery);
