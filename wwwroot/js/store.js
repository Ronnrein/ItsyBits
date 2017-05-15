$(document).ready(function () {
    $("#canvas").town({
        clickHandler: function (plot) {
            if (plot.id === 0) {
                plot.click();
                return;
            }
            $("#PlotId").val(plot.id);
            $("#plot-text").html("Do you want to " +
                (plot.building !== null ? "replace " + plot.building.name : "place on plot " + plot.id) + "?");
        }
    });
});