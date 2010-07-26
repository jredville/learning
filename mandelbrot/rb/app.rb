$: << File.join(File.dirname(__FILE__), "..", "ext")
load_assembly 'System.Core'
load_assembly 'System.CoreEx'
load_assembly 'System.Reactive'


using_clr_extensions System
using_clr_extensions System::Linq

include System
include System::Linq

xaml = %Q{<Window x:Class="Mandelbrot.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        Title="MainWindow" Height="350" Width="525">
    <Grid>
        <Canvas x:Name="Canvas" />
    </Grid>
</Window>}

class Object
  def to_seq(type = Object)
    System::Linq::Enumerable.method(:of_type).of(type).call(self.to_a)
  end
end
 
range = (1..10).to_seq

observable = 
 Observable.to_observable(range).where(lambda {|next_val| next_val % 2 == 0})
 
dispoable = observable.subscribe(Action[Object].new {|val| puts "Next value: #{val}" }) 
