$(document).ready(function () {
    $("#canvas").town({
        persistantClick: true,
        clickHandler: function(building) {
            if (building.plot === 0) {
                building.click();
                return;
            }
        }
    });
});