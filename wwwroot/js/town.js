// Options
var canvasElement = "#canvas";
var background = new Sprite("/images/town/background.png", new Vector2(0, 0));
var cursor = new Sprite("/images/town/cursor.png", new Vector2(-2000, -2000), null, "/images/town/cursorPointer.png");
var buildings = [
    new Building(new Sprite("/images/town/store.png", new Vector2(1720, 480), null, "/images/town/storehover.png"), "/store")
];
var sprites = [
    new Sprite("/images/misc/coin.png", new Vector2(400, 500), new Animation(10, 1, 10, 50)),
    new Sprite("/images/misc/dance.png", new Vector2(400, 400), new Animation(8, 10, 80, 50))
];
var scaleFactor = 1.05;
var backgroundColor = "#9DCF3B";
var minScale = 0.3;
var maxScale = 1;

// Variables
var canvas, ctx, sources, dragStartPosition, dragged, canvasContainer;
var currentScale = 1;
var lastUpdate = Date.now();
var lastMousePosition = cursor.rect;

// Wait for document to be ready
$(document).ready(function() { init(); });

// Initialize
function init() {

    // Assign variables
    canvas = $(canvasElement);
    canvasContainer = $(canvasElement).parent();
    ctx = canvas[0].getContext("2d");

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
    getData(function () {
        $.each(buildings, function(i, building) {
            sprites.unshift(building.sprite);
        });
        sprites.unshift(background);
        sprites.push(cursor);
        sources = [];
        $.each(sprites, function (i, sprite) {
            sources.push({ source: sprite.imagePath, target: sprite.image });
            if (sprite.hoverImagePath != null) {
                sources.push({ source: sprite.hoverImagePath, target: sprite.hoverImage });
            }
        });
        loadImages(function () {

            // Set building rect to image size
            $.each(sprites, function (i, sprite) {
                sprite.rect.width = sprite.image.width;
                sprite.rect.height = sprite.image.height;
            });

            // Remove cursor from array to render it independently
            sprites.splice(sprites.indexOf(cursor));

            // Startup
            resizeCanvas();
            setInterval(update, 1000 / 60);
            update();
        });
    });
}

// Function to update scene
function update() {
    var delta = Date.now() - lastUpdate;
    lastUpdate = Date.now();
    cursor.hover = false;
    $.each(sprites, function (i, sprite) {
        if (sprite !== cursor) {
            sprite.hover = sprite.rect.contains(ctx.transformedPoint(lastMousePosition.x, lastMousePosition.y));
            if (sprite.hoverImagePath != null && sprite.hover) {
                cursor.hover = true;
            }
        }
    });
    cursor.rect.x = lastMousePosition.x;
    cursor.rect.y = lastMousePosition.y;
    containBackground();
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
    $.each(sprites, function (i, sprite) {
        sprite.render(delta);
    });

    // Render cursor
    ctx.save();
    ctx.setTransform(1, 0, 0, 1, 0, 0);
    cursor.render(delta);
    ctx.restore();
}

// Function to get user data
function getData(callback) {
    $.getJSON("/building/plots", function(data) {
        $.each(data, function (i, v) {
            var image = v.buildings == null ? "empty" : "building" + v.buildings[0].id;
            buildings.push(new Building(
                new Sprite(
                    "/images/town/" + image + ".png",
                    new Vector2(v.positionX, v.positionY),
                    null,
                    "/images/town/" + image + "hover.png"
                ),
                "/building/details/" + v.id
            ));
        });
        callback();
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
    if (!dragged) {
        $.each(buildings, function(i, building) {
            if (building.sprite.hover) {
                building.click();
            }
        });
    }
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
    if (center.x > background.rect.width) {
        ctx.translate(-(background.rect.width - center.x), 0);
    }
    if (center.y < 0) {
        ctx.translate(0, center.y);
    }
    if (center.y > background.rect.height) {
        ctx.translate(0, -(background.rect.height - center.y));
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
function Building(sprite, url = null) {
    this.sprite = sprite;
    this.url = url;
    this.render = function(delta) {
        sprite.render(delta);
    };
    this.click = function () {
        if (this.url != null) {
            window.location = this.url;
        }
    }
}

function Sprite(image, position, animation = null, hoverImage = null) {
    this.imagePath = image;
    this.image = new Image();
    this.animation = animation;
    this.hoverImagePath = hoverImage;
    this.hoverImage = new Image();
    this.hover = false;
    this.rect = new Rect(
        position.x,
        position.y,
        0,
        0
    );
    this.render = function (delta) {
        var img = this.hoverImagePath != null && this.hover ? this.hoverImage : this.image;
        if (this.animation != null) {
            animation.render(delta, img, this.rect);
            return;
        }
        ctx.drawImage(img, this.rect.x, this.rect.y);
    }
}

// Animation class
function Animation(columns, rows, frames, frameTime) {
    this.rows = rows;
    this.columns = columns;
    this.frames = frames;
    this.frameTime = frameTime;
    this.currentFrame = 0;
    this.currentFrameTime = 0;
    this.render = function (delta, image, position) {
        this.currentFrameTime += delta;
        if (this.currentFrameTime >= this.frameTime) {
            this.currentFrameTime = 0;
            this.currentFrame += 1;
            if (this.currentFrame >= this.frames) {
                this.currentFrame = 0;
            }
        }
        ctx.drawImage(
            image,
            image.width / this.columns * (this.currentFrame - (Math.floor(this.currentFrame / this.columns) * this.columns)),
            image.height / this.rows * Math.floor(this.currentFrame / this.columns),
            image.width / this.columns,
            image.height / this.rows,
            position.x,
            position.y,
            image.width / this.columns,
            image.height / this.rows
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