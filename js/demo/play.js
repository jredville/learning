var CanvasDemo = {};

Object.defineProperty(Number.prototype, "re", {
  value: this.valueOf(),
  writable: false,
  enumerable: true,
  configurable: true
});
Object.defineProperty(Number.prototype, "im", {
  value: 0,
  writable: false,
  enumerable: true,
  configurable: true
});
  
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
  for (var x =-stepx;x < stepx; x++) {
    func(x);
  }
  this.stroke();
}


CanvasDemo.sin = function() {
  $("#content").append("<canvas />");
  var ctx = $("#content > canvas").last()[0].getContext('2d');
  ctx.drawGrid(10);
  ctx.drawFunc(function(x) {
	this.lineTo(x,10 * Math.sin(0.08 * x));
  });
}

CanvasDemo.drawGrid = function() {
  $("#content").append("<canvas />");
  var ctx = $("#content > canvas").last()[0].getContext('2d');
  ctx.drawGrid(10);
}

function info(obj) {
	var info = $("#info");
}

function demo(name, callback) {
  $($("<a class='demo'>"+name+"</a>").click(callback)).appendTo("#demos")
}

$(function() {
  demo("Sin", CanvasDemo.sin);
  demo("Grid", CanvasDemo.drawGrid);
  //demo("Sin Drag", CanvasDemo.sinDrag);
  });
