if(!Function.prototype.method) {
    Function.prototype.method = function(name, func) {
        if(!this.prototype[name]) {
            this.prototype[name] = func;
        }
        return this;
    }    
}

var j = {};
j.utils = {
    demo : function(name, callback) {
               $($("<a class='demo' href=\"javascript:void;\">"+name+"</a>").click(callback)).appendTo("#demos")
           },

    demos : function(obj) {
                var res = function() {
                    for(var d in obj) {
                        j.utils.demo(d, obj[d]);
                    }
                };
                return res;
            }
    }
