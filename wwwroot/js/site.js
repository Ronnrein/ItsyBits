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
    setBreadcrumbs();
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

// Set the breadcrumbs element based on url
function setBreadcrumbs() {
    var el = $("<div class='ui breadcrumb'>");
    var dividerHtml = "<div class='divider'>/</div>";
    var split = window.location.pathname.split("/");
    el.append("<a class='section' href='/'>Home</a>");
    var path = "";
    $.each(split, function (i, v) {
        if (v === "") {
            return true;
        }
        el.append(dividerHtml);
        console.log(split[i]);
        if (split[i + 1] === undefined || !isNaN(split[i + 1])) {
            el.append("<p class='section'>" + document.title.split(" - ")[0] + "</p>");
            return false;
        }
        path += "/" + v;
        el.append("<a class='section' href='" + path + "'>" + v.charAt(0).toUpperCase() + v.slice(1) +"</a>");
    });
    $("#breadcrumbs").html(el);
}

// Sets menu items to be active if links to current page
function setActiveMenuItem() {
    $(".active-check a.item").each(function () {
        if ($(this).attr("href") === window.location.pathname) {
            $(this).addClass("active");
        }
    });
}