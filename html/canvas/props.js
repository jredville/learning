var obj = {
    _x : 0, 
    get x() {return this._x;},
    set x(v) { this._x = v }
}
Object.defineProperties(obj,{ 
    toString: { 
                  value: function() {
                             var ans = ["[", this.name, "#",];
                             Object.keys(this).forEach(function(el) {
                                 ans.push(el, ":", this[el], ",");
                             }.bind(this));
                             ans.push("]");
                             return ans.join("");
                         }, 
                  enumerable:false},
    name: {
                  value:"obj",
                  enumerable:false
                  } 
});
console.log(obj);
var sealed = Object.create(obj, {name:{value:"sealed", enumerable:false}})
console.log(sealed);
