var Fractal = {};
Fractal.Complex = function() {};
Fractal.Complex.prototype = {
  _re: 0,
  _im: 0,
  get re() { return this._re; },
  set re(v) { this._re = v; },
  
  get im() { return this._im; },
  set im(v) { this._im = v; },
  add: function(num) {
      return Fractal.Complex.create(this.re+num.re, this.im+num.im);
  },
  toString:  function() {
    return this.re + "+" + this.im + "i";
  },
  multiply: function(num) {
    /*x+iy * u+iv = xu + ixv + iyu - yv */
    return Fractal.Complex.create(this.re*num.re - this.im*num.im,
                   this.re*num.im+this.im*num.re);
  },
  magnitude: function() {
	return Math.sqrt((this.re*this.re)+(this.im*this.im));
  }
};

Fractal.Complex.create = function(real, imaginary) {
  var res = new Fractal.Complex();
  res.re = real;
  res.im = imaginary;
  return res;
};

Fractal.iterations = 1000;
Fractal.inJulia = function(start, c, limit) {
	function julia(val) {
		setTimeout(function() {
			return val.multiply(val).add(c);
		}, 10);
	}
	var velocity = 0;
	var current = start;
	for(var i = 0; i < Fractal.iterations; i++) {
		current = julia(current);
		if (current.magnitude() > limit) {
			velocity = i;
			break;
		}
	}
	return velocity;
};

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

Fractal.sin = function() {
  $("#content").append("<canvas />");
  var ctx = $("#content > canvas").last()[0].getContext('2d');
  ctx.drawGrid(10);
  ctx.drawFunc(function(x) {
	this.lineTo(x,10 * Math.sin(0.08 * x));
  });
}
Fractal.draw1 = function() {
	var ctx = $("#canvas1")[0].getContext('2d');
	console.log("setting bounds");
	var minx = -2;
	var miny = -1.5;
	var width = 4;
	var height = 3;
	ctx.save();
	/* 300px, desired width = 2, dx = 150 */
	var dx = ctx.canvas.width/width;
	console.info(dx);
	var dy = ctx.canvas.height/height;
	console.info(dy);
	ctx.setTransform(dx,0,0,-dy,-minx * dx,-miny * dy);
	var stepx = 1/dx;
	var stepy = 1/dy;
	var plot;
	ctx.fillStyle="#000";
	console.log("starting the loop");
	for (var x = minx; x < x+width; x+=stepx) {
		for (var y = miny; y < y+height; y+= stepy) {
			console.info("(" + x + "," + y + ")");
		    setTimeout( function() {
				var start = Fractal.Complex.create(x,y);
				console.log(start.toString());
				plot = Fractal.inJulia(start, Fractal.Complex.create(1,0), 3);
				if (plot === 0) {
					ctx.fillRect(x,y,stepx,stepy);
				}	
			}, 500);
		}
	}
	console.log("done");
}
function info(obj) {
	var info = $("#info");
}



$(function() {
  Fractal.sin();
  //Fractal.draw1();
  });
