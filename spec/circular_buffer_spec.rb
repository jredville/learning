require 'spec_helper'
require 'circular_buffer'

describe "CircularBuffer" do
    before(:each) do
        @cap = 5
        @buf = CircularBuffer.new(@cap)
    end

    it "takes an initial capacity" do
        lambda {CircularBuffer.new}.should raise_error
        lambda {CircularBuffer.new 4}.should_not raise_error
    end

        it "changes count when adding item" do
            lambda {@buf.enque(10)}.should change {@buf.count}
        end

        it "can deque in order" do
            @buf.enque 1
            @buf.enque 2
            @buf.deque.should == 1
            @buf.deque.should == 2
        end
    describe "when empty" do
        it "throws OutOfRangeException  when read is attempted" do
            lambda { @buf.deque}.should raise_error OutOfRangeException
        end
        
        it "can have objects added" do
            @buf.enque(1).should == @buf
            (@buf << 1).should == @buf
        end
        

        it "is empty" do
            @buf.should be_empty
        end

        it "has full capacity" do
            @buf.capacity.should == @cap
        end

        it "is not full" do
            @buf.should_not be_full
        end
    end

    describe "when partially full" do
        before(:each) do
            @items = [1,2]
            @items.each {|i| @buf.enque(i) }
        end

        it "is not empty" do
            @buf.should_not be_empty
        end

        it "is not full" do
            @buf.should_not be_full
        end

        it "can deque objects" do
            @buf.deque.should == @items.first
        end

        it "has a number of items" do
            @buf.count.should == @items.count
        end

        it "has full capacity" do
            @buf.capacity.should == @cap
        end
    end

    describe "when full" do
        before(:each) do
            @items = [1,2,3,4,5]
            @items.each {|i| @buf.enque(i) }
        end

        it "is not empty" do
            @buf.should_not be_empty
        end

        it "is not full" do
            @buf.should be_full
        end

        it "can deque objects" do
            @buf.deque.should == @items.first
        end

        it "has a number of items" do
            @buf.count.should == @cap
        end

        it "has full capacity" do
            @buf.capacity.should == @cap
        end

        it "cannot enque objects" do
            lambda {@buf.enque(10)}.should raise_error OutOfRangeException
        end
    end
end
