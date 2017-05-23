$(document).ready(function() {

    // Semantic events
    $(".progressbar").progress();
    $(".tooltip").popup();
    $(".dropdown").dropdown();
    $(".ui.sidebar").sidebar("attach events", "#sidebar-button");
    $(".tabular .item").tab();
    $(".ui.accordion").accordion();
    $(".message .close").on("click", function () {
        $(this).closest(".message").transition("fade");
    });

    // Custom events
    $("#notifications-button").click(clearNotifications);

    // Call functions
    setActiveMenuItem();
});

// Clear unread status from notifications
function clearNotifications() {
    if ($(this).find(".label").length) {
        var that = this;
        $.get("/user/notifications", function () {

            // Remove label
            $(that).find(".label").remove();
        });
    }
}

// Sets menu items to be active if links to current page
function setActiveMenuItem() {
    $(".active-check a.item").each(function () {
        if ($(this).attr("href") === window.location.pathname) {
            $(this).addClass("active");
        }
    });
}