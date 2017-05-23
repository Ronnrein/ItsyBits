$(document).ready(function () {
    $("#canvas").town({
        clickHandler: function (plot) {
            if (plot.id === 0) {
                plot.click();
                return;
            }
            $("#PlotId").val(plot.id);
            $("#plot-text").html("Do you want to " + (plot.occupied ? "replace " : "place ") + " on plot " + plot.id + "?");
        }
    });
});