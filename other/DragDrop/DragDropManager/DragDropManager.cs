

using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Runtime.InteropServices;
using System.Windows.Media;

namespace DnD
{
	public static class DragDropManager
	{
		private static UIElement _draggedElt;
		private static bool _isMouseDown = false;
		private static Point _dragStartPoint;
        private static Point _offsetPoint;
		private static DropPreviewAdorner _overlayElt;

		#region Dependency Properties

        public static readonly DependencyProperty DragSourceProperty =
                DependencyProperty.RegisterAttached("DragSource", typeof(DragSourceBase), typeof(DragDropManager),
                new FrameworkPropertyMetadata(new PropertyChangedCallback(OnDragSourceChanged)));

        public static DragSourceBase GetDragSource(DependencyObject depObj)
        {
            return depObj.GetValue(DragSourceProperty) as DragSourceBase;
        }

        public static void SetDragSource(DependencyObject depObj, bool isSet)
        {
            depObj.SetValue(DragSourceProperty, isSet);
        }

		public static readonly DependencyProperty DropTargetProperty =
			DependencyProperty.RegisterAttached("DropTarget", typeof(DropTargetBase), typeof(DragDropManager),
			new FrameworkPropertyMetadata(new PropertyChangedCallback(OnDropTargetChanged)));

		public static void SetDropTarget(DependencyObject depObj, bool isSet)
		{
			depObj.SetValue(DropTargetProperty, isSet);
		}

		public static DropTargetBase GetDropTarget(DependencyObject depObj)
		{
			return depObj.GetValue(DropTargetProperty) as DropTargetBase;
		}

		#endregion

		#region Property Change handlers
      
        private static void OnDragSourceChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs args)
        {
            UIElement sourceElt = depObj as UIElement;
            if (args.NewValue != null && args.OldValue == null)
            {
                sourceElt.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(DragSource_PreviewMouseLeftButtonDown);
                sourceElt.PreviewMouseMove += new MouseEventHandler(DragSource_PreviewMouseMove);
                sourceElt.PreviewMouseUp += new MouseButtonEventHandler(DragSource_PreviewMouseUp);

                // Set the Drag source UI
                DragSourceBase advisor = args.NewValue as DragSourceBase;
                advisor.SourceUI = sourceElt;
            }
            else if (args.NewValue == null && args.OldValue != null)
            {
                sourceElt.PreviewMouseLeftButtonDown -= DragSource_PreviewMouseLeftButtonDown;
                sourceElt.PreviewMouseMove -= DragSource_PreviewMouseMove;
                sourceElt.PreviewMouseUp -= DragSource_PreviewMouseUp;
            }
        }

		static void DragSource_PreviewMouseUp(object sender, MouseButtonEventArgs e)
		{
			_isMouseDown = false;
			Mouse.Capture(null);
		}

		private static void OnDropTargetChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs args)
		{
			UIElement targetElt = depObj as UIElement;
			if (args.NewValue != null && args.OldValue == null)
			{
				targetElt.PreviewDragEnter += new DragEventHandler(DropTarget_PreviewDragEnter);
				targetElt.PreviewDragOver += new DragEventHandler(DropTarget_PreviewDragOver);
				targetElt.PreviewDragLeave += new DragEventHandler(DropTarget_PreviewDragLeave);
				targetElt.PreviewDrop += new DragEventHandler(DropTarget_PreviewDrop);

				targetElt.AllowDrop = true;

				// Set the Drag source UI
				DropTargetBase advisor = args.NewValue as DropTargetBase;
				advisor.TargetUI = targetElt;
			}
			else if (args.NewValue == null && args.OldValue != null)
			{
				targetElt.PreviewDragEnter -= DropTarget_PreviewDragEnter;
				targetElt.PreviewDragOver -= DropTarget_PreviewDragOver;
				targetElt.PreviewDragLeave -= DropTarget_PreviewDragLeave;
				targetElt.PreviewDrop -= DropTarget_PreviewDrop;


				targetElt.AllowDrop = false;
			}
		}


		#endregion

		/* ____________________________________________________________________
		 *		Drop Target events 
		 * ____________________________________________________________________
		 */
		static void DropTarget_PreviewDrop(object sender, DragEventArgs e)
		{
			if (UpdateEffects(sender, e) == false) return;

			DropTargetBase advisor = GetDropTarget(sender as DependencyObject);
			Point dropPoint = e.GetPosition(sender as UIElement);

			// Calculate displacement for (Left, Top)
			Point offset = e.GetPosition(_overlayElt);
			dropPoint.X = dropPoint.X - offset.X;
			dropPoint.Y = dropPoint.Y - offset.Y;

			advisor.OnDropCompleted(e.Data, dropPoint);
			RemovePreviewAdorner();
            _offsetPoint = new Point(0, 0);

		}

		static void DropTarget_PreviewDragLeave(object sender, DragEventArgs e)
		{

			if (UpdateEffects(sender, e) == false) return;

            DropTargetBase advisor = GetDropTarget(sender as DependencyObject);
            Point mousePoint = MouseUtilities.GetMousePosition(advisor.TargetUI);
            
            //Console.WriteLine("Inside DropTarget_PreviewDragLeave1" + mousePoint.X.ToString() + "|" + mousePoint.Y.ToString());
            //giving a tolerance of 2 so that the adorner is removed when the mouse is moved fast.
            //this might still be small...in that case increase the tolerance
            if ((mousePoint.X < 2) || (mousePoint.Y < 2)||
                (mousePoint.X > ((FrameworkElement)(advisor.TargetUI)).ActualWidth - 2) ||
                (mousePoint.Y > ((FrameworkElement)(advisor.TargetUI)).ActualHeight - 2))
            {
                RemovePreviewAdorner();
            }
			e.Handled = true;
		}

		static void DropTarget_PreviewDragOver(object sender, DragEventArgs e)
		{
			if (UpdateEffects(sender, e) == false) return;
			// Update position of the preview Adorner
			Point position = e.GetPosition(sender as UIElement);

			_overlayElt.Left = position.X - _offsetPoint.X;
			_overlayElt.Top = position.Y - _offsetPoint.Y;
			
			e.Handled = true;
		}

		static void DropTarget_PreviewDragEnter(object sender, DragEventArgs e)
		{

			if (UpdateEffects(sender, e) == false) return;

			// Setup the preview Adorner
			UIElement feedbackUI = GetDropTarget(sender as DependencyObject).GetVisualFeedback(e.Data);
            _offsetPoint = GetOffsetPoint(e.Data);

            DropTargetBase advisor = GetDropTarget(sender as DependencyObject);

            Point mousePoint = MouseUtilities.GetMousePosition(advisor.TargetUI);

           // Console.WriteLine("Inside DropTarget_PreviewDragEnter" + mousePoint.X.ToString() + "|" + mousePoint.Y.ToString());

            //giving a tolerance of 2 so that the adorner is created when the mouse is moved fast.
            //this might still be small...in that case increase the tolerance
            if ((mousePoint.X < 2) || (mousePoint.Y < 2) ||
                (mousePoint.X > ((FrameworkElement)(advisor.TargetUI)).ActualWidth - 2) ||
                (mousePoint.Y > ((FrameworkElement)(advisor.TargetUI)).ActualHeight - 2) ||
                 (_overlayElt == null))
            {
                CreatePreviewAdorner(sender as UIElement, feedbackUI);
            }

			e.Handled = true;
		}

        static Point GetOffsetPoint(IDataObject obj)
        {
            Point p = (Point)obj.GetData("OffsetPoint");
            return p;
        }

		static bool UpdateEffects(object uiObject, DragEventArgs e)
		{
			DropTargetBase advisor = GetDropTarget(uiObject as DependencyObject);
			if (advisor.IsValidDataObject(e.Data) == false) return false;

			if ((e.AllowedEffects & DragDropEffects.Move) == 0 &&
				(e.AllowedEffects & DragDropEffects.Copy) == 0)
			{
				e.Effects = DragDropEffects.None;
				return true;
			}

			if ((e.AllowedEffects & DragDropEffects.Move) != 0 &&
				(e.AllowedEffects & DragDropEffects.Copy) != 0)
			{
                if ((e.KeyStates & DragDropKeyStates.ControlKey) != 0)
                {
                }
				e.Effects = ((e.KeyStates & DragDropKeyStates.ControlKey) != 0) ?
					DragDropEffects.Copy : DragDropEffects.Move;
			}

			return true;
		}

		/* ____________________________________________________________________
		 *		Drag Source events 
		 * ____________________________________________________________________
		 */
		static void DragSource_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			// Make this the new drag source
			DragSourceBase advisor = GetDragSource(sender as DependencyObject);

			if (advisor.IsDraggable(e.Source as UIElement) == false) return;

			_draggedElt = e.Source as UIElement;
			_dragStartPoint = e.GetPosition(GetTopContainer());

            _offsetPoint = e.GetPosition(_draggedElt);
			_isMouseDown = true;

		}

		static void DragSource_PreviewMouseMove(object sender, MouseEventArgs e)
		{
			if (_isMouseDown && IsDragGesture(e.GetPosition(GetTopContainer())))
			{
				DragStarted(sender as UIElement);
			}
		}

		static void DragStarted(UIElement uiElt)
		{
			_isMouseDown = false;
			Mouse.Capture(uiElt);

			DragSourceBase advisor = GetDragSource(uiElt as DependencyObject);
			DataObject data = advisor.GetDataObject(_draggedElt);
            
            data.SetData("OffsetPoint", _offsetPoint);

			DragDropEffects supportedEffects = advisor.SupportedEffects;

			// Perform DragDrop

			DragDropEffects effects = System.Windows.DragDrop.DoDragDrop(_draggedElt, data, supportedEffects);
			advisor.FinishDrag(_draggedElt, effects);

			// Clean up
			RemovePreviewAdorner();
			Mouse.Capture(null);
			_draggedElt = null;
		}

		static bool IsDragGesture(Point point)
		{
			bool hGesture = Math.Abs(point.X - _dragStartPoint.X) > SystemParameters.MinimumHorizontalDragDistance;
			bool vGesture = Math.Abs(point.Y - _dragStartPoint.Y) > SystemParameters.MinimumVerticalDragDistance;

			return (hGesture | vGesture);
		}

		/* ____________________________________________________________________
		 *		Utility functions
		 * ____________________________________________________________________
		 */
		static UIElement GetTopContainer()
		{
           // return  LogicalTreeHelper.FindLogicalNode(Application.Current.MainWindow, "canvas") as UIElement;

			return Application.Current.MainWindow.Content as UIElement;
		}

		private static void CreatePreviewAdorner(UIElement adornedElt, UIElement feedbackUI)
		{
			// Clear if there is an existing preview adorner
			RemovePreviewAdorner();

			AdornerLayer layer = AdornerLayer.GetAdornerLayer(GetTopContainer());
			_overlayElt = new DropPreviewAdorner(feedbackUI, adornedElt);
			layer.Add(_overlayElt);
		}

		private static void RemovePreviewAdorner()
		{
			if (_overlayElt != null)
			{
				AdornerLayer.GetAdornerLayer(GetTopContainer()).Remove(_overlayElt);
				_overlayElt = null;
			}
		}

	}

    public class MouseUtilities
    {
        [System.Runtime.InteropServices.StructLayout(LayoutKind.Sequential)]
        private struct Win32Point
        {
            public Int32 X;
            public Int32 Y;
        };

        [DllImport("user32.dll")]
        private static extern bool GetCursorPos(ref Win32Point pt);

        [DllImport("user32.dll")]
        private static extern bool ScreenToClient(IntPtr hwnd, ref Win32Point pt);

        public static Point GetMousePosition(Visual relativeTo)
        {
            Win32Point mouse = new Win32Point();
            GetCursorPos(ref mouse);

            System.Windows.Interop.HwndSource presentationSource =
                (System.Windows.Interop.HwndSource)PresentationSource.FromVisual(relativeTo);

            ScreenToClient(presentationSource.Handle, ref mouse);

            GeneralTransform transform = relativeTo.TransformToAncestor(presentationSource.RootVisual);

            Point offset = transform.Transform(new Point(0, 0));

            return new Point(mouse.X - offset.X, mouse.Y - offset.Y);
        }
    };

}
