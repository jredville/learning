var JIM = JIM || {};
JIM.Numbers = {};
JIM.Numbers.numbers = function(spec, my) {
    var my = my || {};
    var base = spec.base;
    var that = {};

    var parseNumber = function(val) {
        var answer = [];
        console.info(base);
        while(val !== 0) {
            answer.push(val % base);
            val = Math.floor(val / base);
        }
        return answer.reverse();
    };

    var add = function(arg1,arg2) {
        
    };
    var proto = {
        
    };
    that.create = function(input) {
        var res = Object.create(proto, {val: {value: parseNumber(input), writable: false, enumerable: true, configurable: false}});
        return res;
    }
};
Numbers.Binary = {};
Numbers.Binary.parseNumber = Numbers.createParseNumber(2);
