function forEach(array, action) {
	for(i=0;i<array.length;i++)
		action(array[i]);
}

function negate(func) {
	return function(x) {
		return !func.apply(null, arguments)
	}
}

function reduce(combine, base, array) {
	forEach(array, function(element) {
		base = combine(base, element);
	});
	return base;
}

function map(func, array) {
	var result = [];
	foreach(array, function(element) {
		result.push(func(element));
	});
	return result;
}

//extras
function count(array, predicate) {
	return reduce(predicate, 0, array);
}

function equals(x) {
	return function(element) {return x===element;};
}

//details
function countZeroes(array) {
	return count(equals(0), array);
}

// The .bind method from Prototype.js 
Function.prototype.bind = function(){ 
  var fn = this, args = Array.prototype.slice.call(arguments), object = args.shift(); 
  return function(){ 
    return fn.apply(object, 
      args.concat(Array.prototype.slice.call(arguments))); 
  }; 
};