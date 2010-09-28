var CanvasDemo = {};
  
CanvasRenderingContext2D.prototype.drawGrid = function(step) {
  var w = this.canvas.width;
  var h = this.canvas.height;
  
  // Set line styles
  this.strokeStyle = '#ccc';
  this.beginPath();
  for (var x = 0.5; x < w; x += step) {
    for (var y = 0.5; y < h; y += step) {
      this.moveTo(x,0);
      this.lineTo(x,h);
      this.moveTo(0,y);
      this.lineTo(w, y);
    }
  }
  this.stroke();
}
CanvasRenderingContext2D.prototype.drawAxis = function(stepx, stepy) {
  this.moveTo(-stepx,0);
  this.lineTo(stepx,0);
  this.moveTo(0,-stepy);
  this.lineTo(0, stepy);
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

CanvasDemo.newCanvas = function(selector) {
	if (selector === undefined) {
		selector = "#content";
	}
	return $("<canvas />").appendTo(selector)[0].getContext('2d');
}

CanvasDemo.sin = function() {
  var ctx = CanvasDemo.newCanvas();
  ctx.drawGrid(10);
  ctx.drawFunc(function(x) {
	this.lineTo(x,10 * Math.sin(0.08 * x));
  });
}
CanvasDemo.mdc1 = function() {
	var ctx = CanvasDemo.newCanvas();
	ctx.fillStyle = "rgb(200,0,0)";  
    ctx.fillRect (10, 10, 55, 50);  
  
    ctx.fillStyle = "rgba(0, 0, 200, 0.5)";  
    ctx.fillRect (30, 30, 55, 50); 
}
CanvasDemo.drawGrid = function() {
  var ctx = CanvasDemo.newCanvas();
  ctx.drawGrid(10);

}
CanvasDemo.gradient = function() {
	var ctx = CanvasDemo.newCanvas();
	var myGradient = ctx.createLinearGradient(0,0,ctx.canvas.width,ctx.canvas.height);
	myGradient.addColorStop(0, "#ff0000");
	myGradient.addColorStop(0.5, "#00ff00");
	myGradient.addColorStop(1, "#0000ff");
	
	ctx.fillStyle  = myGradient;
	ctx.fillRect(0,0,ctx.canvas.width,ctx.canvas.height);
}

CanvasDemo.imageData = function() {
	var context = CanvasDemo.newCanvas();
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
}
function info(obj) {
	var info = $("#info");
}

function demo(name, callback) {
  $($("<a class='demo' href=\"#\">"+name+"</a>").click(callback)).appendTo("#demos")
}

function demos(obj) {
	for(var d in obj) {
		demo(d, obj[d]);
	}
}
$(function() {
  demos({"Sin": CanvasDemo.sin,
		"Grid": CanvasDemo.drawGrid,
		"MDC1": CanvasDemo.mdc1,
		"Image Data": CanvasDemo.imageData,
		"Gradient": CanvasDemo.gradient});
  
  });
