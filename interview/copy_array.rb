#require 'rubygems'
#require 'spec'
class Rect
  def initialize(x,y,w,h)
    @x = x
    @y = y
    @w = w
    @h = h
  end

  attr_reader :x,:y,:w,:h
end

class Image
  attr_reader :h,:w
  def initialize(h,w)
    @h = h
    @w = w
    @store= [0] * (h*w)
    return unless block_given?
    (0...h).each do |y|
      (0...w).each do |x|
        self[x,y] = yield x,y
      end
    end
  end

  def [](x,y)
    @store[x+(y*w)]
  end

  def []=(x,y,val)
    @store[x+(y*w)] = val
  end

  def display
    res = []
    (0...h).each do |y|
      r= ["["]
      (0...w).each do |x|
        r << self[x,y]
      end
      r << "]"
      res << r.join(" ")
    end
    puts res.join("\n")
  end
  
  def copy(x,y,w1,h1,dx,dy)
    temp = Image.new(h1,w1)
    (0...w1).each do |x1|
      (0...h1).each do |y1|
        temp[x1,y1] = self[x+x1,y+y1]
      end
    end
    (0...w1).each do |x1|
      (0...h1).each do |y1|
        self[x+x1+dx,y+y1+dy] = temp[x1,y1]
      end
    end
  end

end

#describe CircularQueue do
  
#end

i = Image.new(3,4) {|x,y| x+y }


i.display

i.copy(0,0,2,2,2,1)
puts 'copying'
i.display