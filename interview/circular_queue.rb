require 'rubygems'
require 'spec'

class Node
  def <<(val)
    @value = val
  end

  def has_value?
    !@value.nil?
  end

  def value
    @value
  end

  def clear
    @value = nil
  end
end

class CircularQueue
  attr_reader :capacity
  def initialize(capacity)
    @capacity = 5
    @ret = 0
    @add = 0
    @store = [Node.new] * @capacity
  end

  def size
    @add - @ret
  end
  
  def increment(val)
    return (val + 1) % @capacity
  end

  def enqueue(el)
    if size < capacity
      @store[@add] << el
      @add = increment(@add)
    else
      raise "too many items"
    end
  end

  def dequeue
    res = @store[@ret].value
    @store[@ret].clear
    @ret = increment(@ret)
    res
  end

  def to_s
    "(#@capacity)#{@store.inspect}"
  end
end

describe CircularQueue do
  before(:each) do
    @b = CircularQueue.new(5)
  end

  it 'inits with capacity' do
    b = CircularQueue.new(5)
    b.capacity.should == 5
  end

  it 'can enqueue items' do
    @b.enqueue(1)
    @b.size.should == 1
  end

  it 'dequeues in lifo order' do
    @b.enqueue(1)
    @b.enqueue(2)
    @b.dequeue.should == 2
  end

  it 'tracks the correct size' do
    @b.enqueue(1)
    @b.enqueue(2)
    lambda {@b.dequeue}.should change { @b.size }.from(2).to(1)
  end

  it 'cannot enqueue more than its capacity' do
    lambda { 6.times {|e| @b.enqueue(e)}}.should raise_error
  end
end
