
var canvas = document.getElementById('myCanvas');
var context = canvas.getContext('2d');

var imageObj = new Image();
var imageObj2 = new Image();
var imageObj3 = new Image();
var imageObj4 = new Image();
var imageObj5 = new Image();
var imageObj6 = new Image();




imageObj.onload = function () {
    context.drawImage(imageObj, 0, 0, 1250, 889);
    context.drawImage(imageObj2, 310, 290, 324, 189);
    context.drawImage(imageObj3, 860, 235, 300, 190);
    context.drawImage(imageObj4, 0, 0, 120, 88);
    context.drawImage(imageObj5, 0, 0, 125, 88);
    context.drawImage(imageObj6, 900, 550, 158, 124);


};

imageObj.src = "~/images/town/background.png";
imageObj2.src = "MHouse.png";
imageObj3.src = "Store.png";
imageObj4.src = "THouse.png";
imageObj5.src = "MHouse2.png";
