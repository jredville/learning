var CanvasDemo = {};
CanvasDemo.Utils = {
    demo : function(name, callback) {
               $($("<a class='demo' href=\"#\">"+name+"</a>").click(callback)).appendTo("#demos")
           },

    demos : function(obj) {
                var res = function() {
                    for(var d in obj) {
                        CanvasDemo.Utils.demo(d, obj[d]);
                    }
                };
                return res;
            },

    newCanvas : function(selector) {
                    if (selector === undefined) {
                        selector = "#content";
                    }
                    return $("<canvas />").appendTo(selector)[0].getContext('2d');
                }
};

CanvasRenderingContext2D.prototype.drawGrid = function(step) {
    var w = this.canvas.width;
    var h = this.canvas.height;

    // Set line styles
    this.strokeStyle = '#ccc';
    this.beginPath();
    for (var x = 0.5; x < w; x += step) {
        for (var y = 0.5; y < h; y += step) {
            this.line(x,0,x,h);
            this.line(0,y,w,y);
        }
    }
    this.stroke();
}
CanvasRenderingContext2D.prototype.drawAxis = function(stepx, stepy) {
    this.line(-stepx,0,stepx,0);
    this.line(0,-stepy,0, stepy);
}

CanvasRenderingContext2D.prototype.line = function(fromX, fromY, toX, toY) {
    this.moveTo(fromX,fromY);
    this.lineTo(toX,toY);
}
CanvasRenderingContext2D.prototype.drawFunc = function(func) {
    func = func.bind(this);
    this.strokeStyle = '#333';
    this.lineWidth = 1;
    stepx = this.canvas.width/2;
    stepy = this.canvas.height/2;
    this.translate(stepx, stepy);

    this.beginPath();
    this.drawAxis(stepx,stepy);
    this.moveTo(-stepx,0);
    for (var x =-stepx; x < stepx; x++) {
        func(x);
    }
    this.stroke();
}

CanvasDemo.Demos = {
    "Sin" : function() {
        var ctx = CanvasDemo.Utils.newCanvas();
        ctx.drawGrid(10);
        ctx.drawFunc(function(x) {
            this.lineTo(x,10 * Math.sin(0.08 * x));
        });
    },
    "MDC1" : function() {
        var ctx = CanvasDemo.Utils.newCanvas();
        ctx.fillStyle = "rgb(200,0,0)";  
        ctx.fillRect (10, 10, 55, 50);  

        ctx.fillStyle = "rgba(0, 0, 200, 0.5)";  
        ctx.fillRect (30, 30, 55, 50); 
    },
    "Draw Grid" : function() {
        var ctx = CanvasDemo.Utils.newCanvas();
        ctx.drawGrid(10);

    },
    "Mouseclick 1" : function() {
        var ctx = CanvasDemo.Utils.newCanvas();
        ctx.drawGrid(10);
        ctx.startX = 0;
        ctx.startY = 0;
        $("canvas:last").mousedown(function(ev) {
                this.strokeStyle = '#000';
                this.beginPath();
                this.moveTo(this.startX,this.startY);
                this.lineTo(ev.offsetX, ev.offsetY);
                this.startX = ev.offsetX;
                this.startY = ev.offsetY;
                this.stroke();
            }.bind(ctx));

    },
    "Mouseclick 2" : function() {
        var ctx = CanvasDemo.Utils.newCanvas();
        ctx.drawGrid(10);
        stepx = ctx.canvas.width/2;
        stepy = ctx.canvas.height/2;
        ctx.translate(stepx, stepy);

        ctx.beginPath();
        ctx.drawAxis(stepx,stepy);
        ctx.stroke();
        $("canvas:last").mousedown(function(ev) {
                this.strokeStyle = '#000';
                this.beginPath();
                this.moveTo(0,0);
                this.lineTo(ev.offsetX-stepx, ev.offsetY-stepy);
                this.stroke();
            }.bind(ctx));

    },
    "Gradient" : function() {
        var ctx = CanvasDemo.Utils.newCanvas();
        var myGradient = ctx.createLinearGradient(0,0,ctx.canvas.width,ctx.canvas.height);
        myGradient.addColorStop(0, "#ff0000");
        myGradient.addColorStop(0.5, "#00ff00");
        myGradient.addColorStop(1, "#0000ff");

        ctx.fillStyle  = myGradient;
        ctx.fillRect(0,0,ctx.canvas.width,ctx.canvas.height);
    },

    "Image Data" : function() {
        var context = CanvasDemo.Utils.newCanvas();
        var image = context.getImageData(0, 0, context.canvas.width, context.canvas.height);
        var pixels = context.canvas.width*context.canvas.height;
        var imageData = image.data; // here we detach the pixels array from DOM
        var i;
        while(--pixels){
            i = pixels;

            imageData[4*i+0] = (i % 255); // Red value
            imageData[4*i+1] = ((10 * i) % 255); // Green value
            imageData[4*i+2] = ((5 * i) % 255);
            imageData[4*i+3] = 255;
        }
        context.putImageData(image, 0, 0);
    }};

$(CanvasDemo.Utils.demos(CanvasDemo.Demos));
