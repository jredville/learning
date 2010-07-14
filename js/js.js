function gradRect() {
  var ctx = document.getElementById('canvas').getContext('2d');
  for (var i=0;i<6;i++){
    for (var j=0;j<6;j++){
      ctx.fillStyle = 'rgb(' + Math.floor(255-42.5*i) + ',' +
                       Math.floor(255-42.5*j) + ',0)';
      ctx.fillRect(j*25,i*25,25,25);
    }
  }
}

function gradArc() {
    var ctx = document.getElementById('canvas').getContext('2d');
    for (var i=0;i<6;i++){
      for (var j=0;j<6;j++){
        ctx.strokeStyle = 'rgb(0,' + Math.floor(255-42.5*i) + ',' +
                         Math.floor(255-42.5*j) + ')';
        ctx.beginPath();
        ctx.arc(12.5+j*25,12.5+i*25,10,0,Math.PI*2,true);
        ctx.stroke();
      }
    }
  }

function quadrants() {
  var ctx = document.getElementById('canvas').getContext('2d');
  // draw background
  ctx.fillStyle = '#FD0';
  ctx.fillRect(0,0,75,75);
  ctx.fillStyle = '#6C0';
  ctx.fillRect(75,0,75,75);
  ctx.fillStyle = '#09F';
  ctx.fillRect(0,75,75,75);
  ctx.fillStyle = '#F30';
  ctx.fillRect(75,75,150,150);
  ctx.fillStyle = '#FFF';

  // set transparency value
  ctx.globalAlpha = 0.2;

  // Draw semi transparent circles
  for (var i=0;i<7;i++){
      ctx.beginPath();
      ctx.arc(75,75,10+10*i,0,Math.PI*2,true);
      ctx.fill();
  }
}

function bars() {
  var ctx = document.getElementById('canvas').getContext('2d');

  // Draw background
  ctx.fillStyle = 'rgb(255,221,0)';
  ctx.fillRect(0,0,150,37.5);
  ctx.fillStyle = 'rgb(102,204,0)';
  ctx.fillRect(0,37.5,150,37.5);
  ctx.fillStyle = 'rgb(0,153,255)';
  ctx.fillRect(0,75,150,37.5);
  ctx.fillStyle = 'rgb(255,51,0)';
  ctx.fillRect(0,112.5,150,37.5);

  // Draw semi transparent rectangles
  for (var i=0;i<10;i++){
    ctx.fillStyle = 'rgba(255,255,255,'+(i+1)/10+')';
    for (var j=0;j<4;j++){
      ctx.fillRect(5+i*14,5+j*37.5,14,27.5)
    }
  }
}
    function grads() {  
      var ctx = document.getElementById('canvas').getContext('2d');  
      
      // Create gradients  
      var lingrad = ctx.createLinearGradient(0,0,0,150);  
      lingrad.addColorStop(0, '#00ABEB');  
      lingrad.addColorStop(0.5, '#fff');  
      //lingrad.addColorStop(0.5, '#26C000');  
      //lingrad.addColorStop(1, '#fff');  
     
     var lingrad2 = ctx.createLinearGradient(0,50,0,95);  
     lingrad2.addColorStop(0.5, '#000');  
     lingrad2.addColorStop(1, 'rgba(0,0,0,0)');  
     
    // assign gradients to fill and stroke styles  
     ctx.fillStyle = lingrad;  
     ctx.strokeStyle = lingrad2;  
       
    // draw shapes  
   ctx.fillRect(10,10,130,130);  
     ctx.strokeRect(50,50,50,50);  
     
   }

function grid(step, ctx) {
  w = ctx.canvas.width;
  h = ctx.canvas.height;
        
  dx = step; //pixels per unit
  dy = step; //pixels per unit
  // Set line styles
  ctx.strokeStyle = '#999';
  ctx.lineWidth = 1;
  for (var x = dx; x < w; x += dx) {
    for (var y = dy; y < h; y += dy) {
      ctx.beginPath();
      ctx.moveTo(x,0);
      ctx.lineTo(x,h);
      ctx.stroke();
      ctx.beginPath();
      ctx.moveTo(0,y);
      ctx.lineTo(w, y);
      ctx.stroke();
    }
  }
}
function error(txt) {
  var el = document.getElementById('error');
  var err = document.createElement("p");
  err.textContent = txt;
  el.appendChild(err);
}
function draw() {
  var ctx = document.getElementById('canvas').getContext('2d');
  grid(10, ctx);
  ctx.strokeStyle = '#333';
  ctx.lineWidth = 2;
  ctx.translate(0, ctx.canvas.height/2);
  ctx.beginPath();
  ctx.moveTo(0,0);
  ctx.lineTo(ctx.canvas.width,0);
  ctx.stroke();
  ctx.beginPath();
  ctx.moveTo(0,0);
  for (var x =0;x < ctx.canvas.width; x++) {
    ctx.lineTo(x, 100*Math.sin(0.08*x) + 10 * Math.cos(x));
  }
  ctx.stroke();
}

