$.fn.town = function(options) {

    // Options
    var aspectRatio = isMobile() ? 0.75 : 2;
    var background = new Sprite("/images/town/background2.png", new Vector2(0, 0));
    var cursor = new Sprite("/images/town/cursor.png", new Vector2(-2000, -2000), null, "/images/town/cursorPointer.png");
    var plots = [
        new Plot(0, new Sprite("/images/town/store.png", new Vector2(2020, 862), null, "/images/town/storehover.png"), "/store")
    ];
    var sprites = [
        new Sprite("/images/misc/swayingTree.png", new Vector2(800, 300), new Animation(4, 1, 4, 140)),
        new Sprite("/images/misc/swayingTree.png", new Vector2(1030, 540), new Animation(4, 1, 4, 140)),
        new Sprite("/images/misc/swayingTree.png", new Vector2(190, 830), new Animation(4, 1, 4, 140)),
        new Sprite("/images/misc/swayingTree.png", new Vector2(1600, 1450), new Animation(4, 1, 4, 140)),
        new Sprite("/images/misc/swayingTree.png", new Vector2(540, 1100), new Animation(4, 1, 4, 140)),
        new Sprite("/images/misc/swayingTree.png", new Vector2(1760, 220), new Animation(4, 1, 4, 140)),
        new Sprite("/images/misc/swayingTree.png", new Vector2(2050, 760), new Animation(4, 1, 4, 140)),
        new Sprite("/images/misc/bush.png", new Vector2(1300, 200), new Animation(4, 1, 4, 220)),
        new Sprite("/images/misc/bush.png", new Vector2(340, 730), new Animation(4, 1, 4, 220)),
        new Sprite("/images/misc/bush.png", new Vector2(1860, 840), new Animation(4, 1, 4, 220))
    ];
    var scaleFactor = 1.05;
    var backgroundColor = "#3b7dcf";
    var minScale = 0.3;
    var maxScale = 1;
    var buttonZoom = 3;

    // Set default settings
    var settings = $.extend({
        clickHandler: handleBuildingClick
    }, options);

    // Variables
    var sources, dragStartPosition, dragged, clickedPlot;
    var canvas = this;
    var ctx = canvas[0].getContext("2d");
    var currentScale = 1;
    var lastUpdate = Date.now();
    var lastMousePosition = cursor.rect;

    // Add extension methods to context
    trackTransforms(ctx);

    // Events
    $(window).resize(resizeCanvas);
    canvas.on("mousedown touchstart", mouseDown);
    canvas.on("mouseup touchend mouseleave", mouseUp);
    canvas.on("mousemove touchmove", mouseMove);
    canvas.on("mousewheel DOMMouseScroll", mouseScroll);
    $("#zoom-in").click(function() { zoom(buttonZoom) });
    $("#zoom-out").click(function() { zoom(-buttonZoom) });

    // Load images
    getData(function () {
        $.each(plots, function(i, plot) {
            sprites.unshift(plot.sprite);
        });
        sprites.unshift(background);
        sprites.push(cursor);
        sources = [];
        $.each(sprites, function(i, sprite) {
            sources.push({ source: sprite.imagePath, target: sprite.image });
            if (sprite.hoverImagePath != null) {
                sources.push({ source: sprite.hoverImagePath, target: sprite.hoverImage });
            }
        });
        loadImages(function() {

            // Set sprite rect to image size
            $.each(sprites, function(i, sprite) {
                sprite.rect.width = sprite.image.width;
                sprite.rect.height = sprite.image.height;
            });

            $.each(plots, function(i, plot) {
                plot.sprite.rect.x -= plot.sprite.rect.width / 2;
                plot.sprite.rect.y -= plot.sprite.rect.height;
            });

            // Remove cursor from array to render it independently
            sprites.splice(sprites.indexOf(cursor));

            // Startup
            resizeCanvas();
            update();
        });
    });

    // Function to update scene
    function update() {
        window.requestAnimationFrame(update);
        var delta = Date.now() - lastUpdate;
        lastUpdate = Date.now();
        cursor.hover = false;
        $.each(sprites, function(i, sprite) {
            sprite.hover = false;
        });
        $.each(sprites, function(i, sprite) {
            if (sprite !== cursor) {
                sprite.hover = sprite.rect.contains(ctx.transformedPoint(lastMousePosition.x, lastMousePosition.y));
                if (sprite.hoverImagePath != null && sprite.hover) {
                    cursor.hover = true;
                    return false;
                }
            }
        });
        $.each(plots, function (i, plot) {
            if (clickedPlot === plot.id) {
                plot.sprite.hover = true;
            }
        });
        cursor.rect.x = lastMousePosition.x;
        cursor.rect.y = lastMousePosition.y;
        containBackground();
        render(delta);
    }

    // Function to render elements
    function render(delta) {
        ctx.save();
        ctx.setTransform(1, 0, 0, 1, 0, 0);
        ctx.beginPath();
        ctx.rect(0, 0, canvas[0].width, canvas[0].height);
        ctx.fillStyle = backgroundColor;
        ctx.fill();
        ctx.restore();

        $.each(sprites, function(i, sprite) {
            sprite.render(delta);
        });

        // Render cursor
        if (!isMobile()) {
            ctx.save();
            ctx.setTransform(1, 0, 0, 1, 0, 0);
            cursor.render(delta);
            ctx.restore();
        }
    }

    // Function to get user data
    function getData(callback) {
        $.getJSON("/building/plots", function(data) {
            $.each(data, function (i, v) {
                var image = "/images/" + (v.buildingId === 0 ? "town/empty" : "buildings/" + v.spritePath + "/town");
                var url = v.buildingId === 0 ? "/store" : "/building/details/" + v.buildingId;
                plots.push(new Plot(
                    v.id,
                    new Sprite(
                        image + ".png",
                        new Vector2(v.positionX, v.positionY),
                        null,
                        image + "hover.png"
                    ),
                    url
                ));
            });
            callback();
        });
    }

    // Function to resize canvas to its containers size
    function resizeCanvas() {
        ctx.canvas.width = canvas.width();
        ctx.canvas.height = canvas.width() / aspectRatio;
    }

    // Function to load images and assign them to image object
    function loadImages(callback) {
        var loaded = 0;
        for (var i in sources) {
            sources[i].source += "?cache=" + new Date().getTime();
            sources[i].target.onload = function () {
                if (++loaded >= sources.length) {
                    callback();
                }
            };
            sources[i].target.src = sources[i].source;
        }
    }

    // Gets called when mouse button is pushed down
    function mouseDown(e) {
        e.stopPropagation();
        e.preventDefault();
        e = addTouchOffset(e);
        lastMousePosition = new Vector2(e.offsetX, e.offsetY);
        dragStartPosition = ctx.transformedPoint(lastMousePosition.x, lastMousePosition.y);
        dragged = false;
    }

    // Gets called when mouse button is released
    function mouseUp(e) {
        e.stopPropagation();
        e.preventDefault();
        dragStartPosition = null;
        if (!dragged) {
            $.each(plots, function (i, plot) {
                if (plot.sprite.rect.contains(ctx.transformedPoint(lastMousePosition.x, lastMousePosition.y))) {
                    if (plot.id !== 0) {
                        clickedPlot = plot.id;
                    }
                    settings.clickHandler(plot);
                    return false;
                }
            });
        }
    }

    // Gets called when mouse moves
    function mouseMove(e) {
        e.stopPropagation();
        e.preventDefault();
        e = addTouchOffset(e);
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
        var pt = isMobile() ? new Vector2(background.rect.width / 2, background.rect.height / 2) : ctx.transformedPoint(lastMousePosition.x, lastMousePosition.y);
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
        var box = new Rect(
            background.rect.width / 4,
            background.rect.height / 4,
            background.rect.width / 2,
            background.rect.height / 2
        );
        var changed = false;
        if (center.x < box.x) {
            ctx.translate(center.x - box.x, 0);
            changed = true;
        }
        if (center.x > box.x + box.width) {
            ctx.translate(center.x - (box.x + box.width), 0);
            changed = true;
        }
        if (center.y < box.y) {
            ctx.translate(0, center.y - box.y);
            changed = true;
        }
        if (center.y > box.y + box.height) {
            ctx.translate(0, center.y - (box.y + box.height));
            changed = true;
        }
        if (dragStartPosition && changed) {
            dragStartPosition = ctx.transformedPoint(lastMousePosition.x, lastMousePosition.y);
        }
    }

    // Add touch offset to event
    function addTouchOffset(e) {
        if (e.offsetX === undefined) {
            e.offsetX = e.originalEvent.touches[0].pageX - e.originalEvent.touches[0].target.offsetLeft;
            e.offsetY = e.originalEvent.touches[0].pageY - e.originalEvent.touches[0].target.offsetTop;
        }
        return e;
    }

    // Whether the device is mobile or not
    function isMobile() {
        return /(android|bb\d+|meego).+mobile|avantgo|bada\/|blackberry|blazer|compal|elaine|fennec|hiptop|iemobile|ip(hone|od)|ipad|iris|kindle|Android|Silk|lge |maemo|midp|mmp|netfront|opera m(ob|in)i|palm( os)?|phone|p(ixi|re)\/|plucker|pocket|psp|series(4|6)0|symbian|treo|up\.(browser|link)|vodafone|wap|windows (ce|phone)|xda|xiino/i.test(navigator.userAgent) || /1207|6310|6590|3gso|4thp|50[1-6]i|770s|802s|a wa|abac|ac(er|oo|s\-)|ai(ko|rn)|al(av|ca|co)|amoi|an(ex|ny|yw)|aptu|ar(ch|go)|as(te|us)|attw|au(di|\-m|r |s )|avan|be(ck|ll|nq)|bi(lb|rd)|bl(ac|az)|br(e|v)w|bumb|bw\-(n|u)|c55\/|capi|ccwa|cdm\-|cell|chtm|cldc|cmd\-|co(mp|nd)|craw|da(it|ll|ng)|dbte|dc\-s|devi|dica|dmob|do(c|p)o|ds(12|\-d)|el(49|ai)|em(l2|ul)|er(ic|k0)|esl8|ez([4-7]0|os|wa|ze)|fetc|fly(\-|_)|g1 u|g560|gene|gf\-5|g\-mo|go(\.w|od)|gr(ad|un)|haie|hcit|hd\-(m|p|t)|hei\-|hi(pt|ta)|hp( i|ip)|hs\-c|ht(c(\-| |_|a|g|p|s|t)|tp)|hu(aw|tc)|i\-(20|go|ma)|i230|iac( |\-|\/)|ibro|idea|ig01|ikom|im1k|inno|ipaq|iris|ja(t|v)a|jbro|jemu|jigs|kddi|keji|kgt( |\/)|klon|kpt |kwc\-|kyo(c|k)|le(no|xi)|lg( g|\/(k|l|u)|50|54|\-[a-w])|libw|lynx|m1\-w|m3ga|m50\/|ma(te|ui|xo)|mc(01|21|ca)|m\-cr|me(rc|ri)|mi(o8|oa|ts)|mmef|mo(01|02|bi|de|do|t(\-| |o|v)|zz)|mt(50|p1|v )|mwbp|mywa|n10[0-2]|n20[2-3]|n30(0|2)|n50(0|2|5)|n7(0(0|1)|10)|ne((c|m)\-|on|tf|wf|wg|wt)|nok(6|i)|nzph|o2im|op(ti|wv)|oran|owg1|p800|pan(a|d|t)|pdxg|pg(13|\-([1-8]|c))|phil|pire|pl(ay|uc)|pn\-2|po(ck|rt|se)|prox|psio|pt\-g|qa\-a|qc(07|12|21|32|60|\-[2-7]|i\-)|qtek|r380|r600|raks|rim9|ro(ve|zo)|s55\/|sa(ge|ma|mm|ms|ny|va)|sc(01|h\-|oo|p\-)|sdk\/|se(c(\-|0|1)|47|mc|nd|ri)|sgh\-|shar|sie(\-|m)|sk\-0|sl(45|id)|sm(al|ar|b3|it|t5)|so(ft|ny)|sp(01|h\-|v\-|v )|sy(01|mb)|t2(18|50)|t6(00|10|18)|ta(gt|lk)|tcl\-|tdg\-|tel(i|m)|tim\-|t\-mo|to(pl|sh)|ts(70|m\-|m3|m5)|tx\-9|up(\.b|g1|si)|utst|v400|v750|veri|vi(rg|te)|vk(40|5[0-3]|\-v)|vm40|voda|vulc|vx(52|53|60|61|70|80|81|83|85|98)|w3c(\-| )|webc|whit|wi(g |nc|nw)|wmlb|wonu|x700|yas\-|your|zeto|zte\-/i.test(navigator.userAgent.substr(0, 4));
    }

    // Default building click handler
    function handleBuildingClick(building) {
        building.click();
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
    function Plot(id, sprite, url = null) {
        this.id = id;
        this.sprite = sprite;
        this.url = url;
        this.render = function (delta) {
            sprite.render(delta);
        };
        this.click = function() {
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
        this.render = function(delta) {
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
        this.render = function(delta, image, position) {
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
};

