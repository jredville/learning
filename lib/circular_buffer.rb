class CircularBuffer
    attr_reader :capacity
    def initialize(capacity)
        @capacity=capacity
        @count = 0
        @read = 0
        @write = 0
        @store = []
    end

    def count
        @write - @read
    end

    def deque
        raise OutOfRangeException.new if empty?
        res = @store[@read]
        @read += 1
        res
    end

    def enque(obj)
        raise OutOfRangeException.new if full?
        @store[@write] = obj
        @write += 1
        self
    end
    alias_method :<<, :enque

    def empty?
        @read == @write 
    end

    def full?
        count == @capacity
    end
end
class OutOfRangeException < StandardError;end

