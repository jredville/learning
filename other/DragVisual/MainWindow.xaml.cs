using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DragVisual
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
         private DragAdorner _adorner = null;
        private AdornerLayer _layer;
        private FrameworkElement _dragScope;
        private Point _startPoint;
        private bool _isDragging;


        public MainWindow()
        {
            InitializeComponent();
            foreach (FrameworkElement fe in Panel1.Children)
            {
                fe.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(DragElement_PreviewMouseLeftButtonDown);
                fe.PreviewMouseMove += new MouseEventHandler(DragElement_PreviewMouseMove);               
            }
            foreach (FrameworkElement fe in Panel2.Children)
            {
                fe.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(DragElement_PreviewMouseLeftButtonDown);
                fe.PreviewMouseMove += new MouseEventHandler(DragElement_PreviewMouseMove);
            }
            foreach (FrameworkElement fe in Panel3.Children)
            {
                fe.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(DragElement_PreviewMouseLeftButtonDown);
                fe.PreviewMouseMove += new MouseEventHandler(DragElement_PreviewMouseMove);
            }
            
            Panel1.Drop += new DragEventHandler(Panel_Drop);
            Panel2.Drop += new DragEventHandler(Panel_Drop);
            Panel3.Drop += new DragEventHandler(Panel_Drop);
        }

        void Panel_Drop(object sender, DragEventArgs e)
        {
            FrameworkElement fe = (FrameworkElement)e.Data.GetData(typeof(FrameworkElement));
            Panel sourcePanel = LogicalTreeHelper.GetParent(fe) as Panel;
            Panel targetPanel = e.Source as Panel;

            if (sourcePanel != null && targetPanel != null)
            {
                //remove the object from the source
                sourcePanel.Children.Remove(fe);

                //add it to the new parent
                fe.Width = fe.ActualWidth;
                fe.Height = fe.ActualHeight;               
                targetPanel.Children.Add(fe);
            }
        }

        void DragElement_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && !_isDragging)
            {
                Point position = e.GetPosition(null);

                if (Math.Abs(position.X - _startPoint.X) > SystemParameters.MinimumHorizontalDragDistance ||
                    Math.Abs(position.Y - _startPoint.Y) > SystemParameters.MinimumVerticalDragDistance)
                {
                     Drag(sender, e); 
                }
            }   
        }

        void DragElement_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _startPoint = e.GetPosition(null);
        }


        private void Drag(object sender, MouseEventArgs e)
        {

            // DragScope is every thing inside our main window
            _dragScope = Application.Current.MainWindow.Content as FrameworkElement;

            // The DragOver event 
            DragEventHandler draghandler = new DragEventHandler(Window1_DragOver);
            _dragScope.PreviewDragOver += draghandler; 

            //Create the adorner
            _adorner = new DragAdorner(_dragScope, (FrameworkElement)sender, true, 0.5, 0.7);
            _layer = AdornerLayer.GetAdornerLayer(_dragScope as Visual);
            _layer.Add(_adorner);

            _isDragging = true;
            //Finally drag drop 
            DataObject data = new DataObject(typeof(FrameworkElement), sender);
            DragDropEffects de = DragDrop.DoDragDrop(this, data, DragDropEffects.Move);

            // Clean up 
            AdornerLayer.GetAdornerLayer(_dragScope).Remove(_adorner);
            _adorner = null;

            _dragScope.PreviewDragOver -= draghandler;  
            _isDragging = false;
        }

        void Window1_DragOver(object sender, DragEventArgs args)
        {
            if (_adorner != null)
            {
                _adorner.LeftOffset = args.GetPosition(_dragScope).X ;
                _adorner.TopOffset = args.GetPosition(_dragScope).Y ;
            }
        }
       
    }
}