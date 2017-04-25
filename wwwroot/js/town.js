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
var animations = [
    new Animation("/images/misc/coin.png", new Vector2(200, 200), 10, 1, 10, 100, 100, 50),
    new Animation("/images/misc/dance.png", new Vector2(400, 400), 8, 10, 80, 110, 128, 50)
];
var scaleFactor = 1.05;
var backgroundColor = "#9DCF3B";
var minScale = 0.3;
var maxScale = 1;

// Variables
var canvas, ctx, backgroundImage, sources, dragStartPosition, dragged, lastMousePosition;
var currentScale = 1;
var lastUpdate = Date.now();

// Wait for document to be ready
$(document).ready(function() { init(); });

// Initialize
function init() {

    // Assign variables
    canvas = $(canvasElement);
    ctx = canvas[0].getContext("2d");
    backgroundImage = new Image();
    lastMousePosition = new Vector2(canvas.width / 2, canvas.height / 2);

    // Add extension methods to context
    trackTransforms(ctx);

    // Events
    $(window).resize(resizeCanvas);
    $(canvasElement).on("mousedown", mouseDown);
    $(canvasElement).on("mouseup", mouseUp);
    $(canvasElement).on("mousemove", mouseMove);
    $(canvasElement).on("mousewheel", mouseScroll);
    $(canvasElement).on("DOMMouseScroll", mouseScroll);

    // Load images
    sources = [{ source: background, target: backgroundImage }];
    $.each(buildings, function(i, building) {
        sources.push({ source: building.imagePath, target: building.image });
    });
    $.each(animations, function(i, animation) {
        sources.push({ source: animation.imagePath, target: animation.image });
    });
    loadImages(function() {

        // Set building rect to image size
        $.each(buildings, function(i, building) {
            building.rect.width = building.image.width;
            building.rect.height = building.image.height;
        });

        // Startup
        resizeCanvas();
        setInterval(update, 1000 / 60);
        update();
    });
}

// Function to update scene
function update() {
    var delta = Date.now() - lastUpdate;
    lastUpdate = Date.now();

    var hover = "None";
    $.each(buildings, function(i, building) {
        if (building.rect.contains(ctx.transformedPoint(lastMousePosition.x, lastMousePosition.y))) {
            hover = building.imagePath;
        }
    });
    $("#hover").html(hover);
    render(delta);
}

// Function to render elements
function render(delta) {
    var p1 = ctx.transformedPoint(0, 0);
    var p2 = ctx.transformedPoint(canvas[0].width, canvas[0].height);
    ctx.beginPath();
    ctx.rect(p1.x, p1.y, p2.x - p1.x, p2.y - p1.y);
    ctx.fillStyle = backgroundColor;
    ctx.fill();
    containBackground();
    ctx.drawImage(backgroundImage, 0, 0);
    $.each(buildings, function(i, building) {
        building.render();
    });
    $.each(animations, function(i, animation) {
        animation.render(delta);
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

// Gets called when mouse button is pushed down
function mouseDown(e) {
    lastMousePosition = new Vector2(e.offsetX, e.offsetY);
    dragStartPosition = ctx.transformedPoint(lastMousePosition.x, lastMousePosition.y);
    dragged = false;
}

// Gets called when mouse button is released
function mouseUp(e) {
    dragStartPosition = null;
}

// Gets called when mouse moves
function mouseMove(e) {
    lastMousePosition = new Vector2(e.offsetX, e.offsetY);
    dragged = true;
    var point = ctx.transformedPoint(lastMousePosition.x, lastMousePosition.y);
    if (dragStartPosition) {
        var trans = new Vector2(point.x - dragStartPosition.x, point.y - dragStartPosition.y);
        ctx.translate(trans.x, trans.y);
    }
    // DEBUG
    $("#mousex").html(Math.floor(point.x));
    $("#mousey").html(Math.floor(point.y));
}

// Gets called when mouse scrolls
function mouseScroll(e) {
    e = e.originalEvent;
    var delta = e.wheelDelta ? e.wheelDelta / 40 : e.detail ? -e.detail : 0;
    if (delta) {
        zoom(delta);
    }
    e.preventDefault();
    return false;
}

// Zooms given amount in our out
function zoom(amount) {
    var pt = ctx.transformedPoint(lastMousePosition.x, lastMousePosition.y);
    var factor = Math.pow(scaleFactor, amount);
    currentScale *= factor;
    if (currentScale <= maxScale && currentScale >= minScale) {
        ctx.translate(pt.x, pt.y);
        ctx.scale(factor, factor);
        ctx.translate(-pt.x, -pt.y);
        return;
    }
    currentScale /= factor;
}

// Function to contain the center of the screen within the background image
function containBackground() {
    var center = ctx.transformedPoint(canvas[0].width / 2, canvas[0].height / 2);
    if (center.x < 0) {
        ctx.translate(center.x, 0);
    }
    if (center.x > backgroundImage.width) {
        ctx.translate(-(backgroundImage.width - center.x), 0);
    }
    if (center.y < 0) {
        ctx.translate(0, center.y);
    }
    if (center.y > backgroundImage.height) {
        ctx.translate(0, -(backgroundImage.height - center.y));
    }
}

// Rectangle class
function Rect(x, y, width, height) {
    this.x = x;
    this.y = y;
    this.width = width;
    this.height = height;
    this.contains = function(point) {
        return point.x > this.x && point.x < this.x + this.width && point.y > this.y && point.y < this.y + this.height;
    };
}

// Vector2 class
function Vector2(x, y) {
    this.x = x;
    this.y = y;
}

// Building class
function Building(image, position) {
    this.imagePath = image;
    this.image = new Image();
    this.rect = new Rect(
        position.x,
        position.y,
        this.image.width,
        this.image.width
    );
    this.render = function() {
        ctx.drawImage(this.image, this.rect.x, this.rect.y);
    };
}

// Animation class
function Animation(image, position, columns, rows, frames, frameWidth, frameHeight, frameTime) {
    this.imagePath = image;
    this.image = new Image();
    this.position = position;
    this.rows = rows;
    this.columns = columns;
    this.frames = frames;
    this.frameWidth = frameWidth;
    this.frameHeight = frameHeight;
    this.frameTime = frameTime;
    this.currentFrame = 0;
    this.currentFrameTime = 0;
    this.render = function (delta) {
        this.currentFrameTime += delta;
        if (this.currentFrameTime >= this.frameTime) {
            this.currentFrameTime = 0;
            this.currentFrame += 1;
            if (this.currentFrame >= this.frames) {
                this.currentFrame = 0;
            }
        }
        ctx.drawImage(
            this.image,
            this.frameWidth * (this.currentFrame - (Math.floor(this.currentFrame / this.columns) * this.columns)),
            this.frameHeight * Math.floor(this.currentFrame / this.columns),
            this.frameWidth,
            this.frameHeight,
            this.position.x,
            this.position.y,
            this.frameWidth,
            this.frameHeight
        );
    }
}

// Function to add extension methods to context
// (http://phrogz.net/tmp/canvas_zoom_to_cursor.html)
function trackTransforms(ctx) {
    var svg = document.createElementNS("http://www.w3.org/2000/svg", 'svg');
    var xform = svg.createSVGMatrix();
    ctx.getTransform = function () { return xform; };
    var savedTransforms = [];
    var save = ctx.save;
    ctx.save = function () {
        savedTransforms.push(xform.translate(0, 0));
        return save.call(ctx);
    };
    var restore = ctx.restore;
    ctx.restore = function () {
        xform = savedTransforms.pop();
        return restore.call(ctx);
    };
    var scale = ctx.scale;
    ctx.scale = function (sx, sy) {
        xform = xform.scaleNonUniform(sx, sy);
        return scale.call(ctx, sx, sy);
    };
    var rotate = ctx.rotate;
    ctx.rotate = function (radians) {
        xform = xform.rotate(radians * 180 / Math.PI);
        return rotate.call(ctx, radians);
    };
    var translate = ctx.translate;
    ctx.translate = function (dx, dy) {
        xform = xform.translate(dx, dy);
        return translate.call(ctx, dx, dy);
    };
    var transform = ctx.transform;
    ctx.transform = function (a, b, c, d, e, f) {
        var m2 = svg.createSVGMatrix();
        m2.a = a; m2.b = b; m2.c = c; m2.d = d; m2.e = e; m2.f = f;
        xform = xform.multiply(m2);
        return transform.call(ctx, a, b, c, d, e, f);
    };
    var setTransform = ctx.setTransform;
    ctx.setTransform = function (a, b, c, d, e, f) {
        xform.a = a;
        xform.b = b;
        xform.c = c;
        xform.d = d;
        xform.e = e;
        xform.f = f;
        return setTransform.call(ctx, a, b, c, d, e, f);
    };
    var pt = svg.createSVGPoint();
    ctx.transformedPoint = function (x, y) {
        pt.x = x; pt.y = y;
        return pt.matrixTransform(xform.inverse());
    }
}