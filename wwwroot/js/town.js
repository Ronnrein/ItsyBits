// Options
var canvasElement = "#canvas";
var canvasContainer = "#town";
var background = "/images/town/background.png";
var buildings = [
    new Building("/images/town/MHouse.png", new Vector2(310, 290)),
    new Building("/images/town/Store.png", new Vector2(860, 235)),
    new Building("/images/town/THouse.png", new Vector2(310, 290)),
    new Building("/images/town/MHouse2.png", new Vector2(0, 0))
];

// Variables
var canvas;
var ctx;
var backgroundImage;
var sources;

// Wait for document to be ready
$(document).ready(function() { init(); });

// Initialize
function init() {

    // Assign variables
    canvas = $(canvasElement);
    ctx = canvas[0].getContext("2d");
    backgroundImage = new Image();

    // Events
    $(window).resize(resizeCanvas);

    // Load images
    sources = [{ source: background, target: backgroundImage }];
    $.each(buildings, function(i, building) {
        sources.push({ source: building.imagePath, target: building.image })
    });
    loadImages(function() {

        // Startup
        resizeCanvas();
        update();
    });
}

// Function to update scene
function update() {
    render();
}

// Function to render elements
function render() {
    ctx.drawImage(backgroundImage, 0, 0);
    $.each(buildings, function(i, building) {
        ctx.drawImage(building.image, building.rect.x, building.rect.y);
    });
}

// Function to resize canvas to its containers size
function resizeCanvas() {
    canvas.attr("width", $(canvasContainer).width());
    canvas.attr("height", $(canvasContainer).height());
}

// Function to load images and assign them to image object
function loadImages(callback) {
    var loaded = 0;
    for(var i in sources) {
        sources[i].target.onload = function() {
            if (++loaded >= sources.length) {
                callback();
            }
        };
        sources[i].target.src = sources[i].source;
    }
}

// Rectangle class
function Rect(x, y, width, height) {
    this.x = x;
    this.y = y;
    this.width = width;
    this.height = height;
}

// Vector2 class
function Vector2(x, y) {
    this.x = x;
    this.y = y;
}

// Building class
function Building(image, position) {
    this.imagePath = image,
    this.image = new Image();
    this.rect = new Rect(
        position.x,
        position.y,
        this.image.naturalWidth,
        this.image.naturalHeight
    );
}