$(document).ready(function() {

    // Semantic events
    $(".progressbar").progress();
    $(".tooltip").popup();
    $(".dropdown").dropdown();
    $(".ui.sidebar").sidebar("attach events", "#sidebar-button");
    $(".tabular .item").tab();
    $('.ui.accordion').accordion();
    $(".message .close").on("click", function () {
        $(this).closest(".message").transition("fade");
    });

    // Custom events
    $(".animal-action").submit(doAnimalAction);
    $("#notifications-button").click(clearNotifications);

    // Call functions
    setActiveMenuItem();
});

// Intercept submit and do it through ajax
function doAnimalAction() {
    var that = this;
    $.post($(this).attr("action"), function() {

        // Update the progressbar
        $(that).find(".progressbar").progress({
            percent: 100
        }).attr("data-content", "100%");
    });
    return false;
}

// Clear unread status from notifications
function clearNotifications() {
    if ($(this).find(".label").length) {
        var that = this;
        $.post("/user/clearnotifications", function () {

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