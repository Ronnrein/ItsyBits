$(document).ready(function() {
    $(".progressbar").progress();
    $(".tooltip").popup();
    $(".animal-action").submit(doAnimalAction);
});

// Intercept submit and do it through ajax
function doAnimalAction() {
    var that = this;
    $.post($(this).attr("action"), function () {

        // Update the progressbar
        $(that).find(".progressbar").progress({
            percent: 100
        }).attr("data-content", "100%");
    });
    return false;
}