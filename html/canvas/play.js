var CanvasDemo = {};
CanvasDemo.Utils = {
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
    var stepx = this.canvas.width/2;
    var stepy = this.canvas.height/2;
    this.save();
    this.translate(stepx, stepy);

    this.beginPath();
    this.drawAxis(stepx,stepy);
    this.moveTo(-stepx,0);
    for (var x =-stepx; x < stepx; x++) {
        func(x);
    }
    this.stroke();
    this.restore();
}

CanvasRenderingContext2D.prototype.drawParamFunc = function(range, funcX, funcY) {
    funcX = funcX.bind(this);
    funcY = funcY.bind(this);
    this.strokeStyle = '#333';
    this.lineWidth = 1;
    var stepx = this.canvas.width/2;
    var stepy = this.canvas.height/2;
    var stept = (range[1] - range[0])/1000;
    this.save();
    this.translate(stepx, stepy);

    this.beginPath();
    this.drawAxis(stepx,stepy);
    this.moveTo(funcX(range[0]),funcY(range[0]));
    for (var t = range[0]; t < range[1]; t = t+stept) {
        this.lineTo(funcX(t),funcY(t));
    }
    this.stroke();
    this.restore();
}

CanvasDemo.Demos = {
    "Sin" : function() {
        var ctx = CanvasDemo.Utils.newCanvas();
        ctx.drawGrid(10);
        ctx.drawFunc(function(x) {
            this.lineTo(x,10 * Math.sin(0.08 * x));
        });
    },

    "Sin 2" : function() {
        var ctx = CanvasDemo.Utils.newCanvas();
        stepy = ctx.canvas.height/2;
        ctx.drawGrid(10);
        ctx.drawFunc(function(x) {
            this.lineTo(x,10*Math.sin(0.08 * x));
        });
        $("canvas:last").bind({
            drag : false,
            mousedown: function(ev) {
                this.drag = true;
            },
            mouseup: function(ev) {
                         this.drag = false;
                     },
            mouseleave: function(ev) {
                    this.drag = false;
                },
            mousemove: function(ev) {
                           if(this.drag === true) {
                               ctx.canvas.width = ctx.canvas.width;
                               ctx.drawGrid(10);
                               ctx.drawFunc(function(x) {
                                   this.lineTo(x, (ev.offsetY - stepy) * Math.sin(0.08 * x));    
                               });
                           }
                       }
        });
    },
    "Parameterized" : function() {
      var ctx = CanvasDemo.Utils.newCanvas();
      ctx.drawGrid(10);
      ctx.drawParamFunc([0,3*Math.PI], function(t) {
          return 20 * Math.sin(t);
      }, function(t) {
          return 20 * Math.cos(t);
      });

    },
    "Bind" : function() {
        var y = function(a,b,c) {
                console.log(a,b,c,this,this.d);
            return a + b + c + this.d;
        }

        var z = y.bind(null, 1,2);

        var obj = { d: 4};

        var a = z.bind(obj);

        console.log(a(3));
    },
    "Invocation" : function() {
        var myObject = {
            value: 0,
            increment: function(inc) {
                this.value += typeof inc === 'number' ? inc : 1;
            }
        };
        myObject.increment();
        console.log(myObject.value);
        myObject.increment(2);
        console.log(myObject.value);
        
        myObject.double = function () {
            var that = this;
            var helper = function() {
                that.value = that.value+that.value;
            };
            
            helper();
        };

        myObject.double();
        console.log(myObject.value);

        var Quo = function(string) {
            this.status = string;
        };

        Quo.prototype.get_status = function() {
            return this.status;
        };

        var myQuo = new Quo("confused");

        console.log(myQuo.get_status());
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
