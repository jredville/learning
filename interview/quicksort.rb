def quicksort(arr)
  puts arr.inspect
  return arr if arr.length<=1
  pivot=arr.pop
  low = []
  high = []
  arr.each do |el|
  	if el < pivot
  	  low << el
  	else
	  high << el
  	end
  end
  return quicksort(low) + [pivot] + quicksort(high)
end


arr = [5,23,1,5,16,199,13,5]
puts quicksort(arr).inspect