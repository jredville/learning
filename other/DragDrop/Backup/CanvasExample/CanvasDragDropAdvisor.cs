using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Xml;
using DnD;

namespace Samples.DragDrop
{
    public class CanvasDragSource:DragSourceBase
    {
        public CanvasDragSource()
        {
            SupportedFormat = "CanvasExample";
        }

        public override void FinishDrag(UIElement draggedElt, DragDropEffects finalEffects)
        {
            if ((finalEffects & DragDropEffects.Move) == DragDropEffects.Move)
            {
                (SourceUI as Canvas).Children.Remove(draggedElt);
            }
        }

        public override bool IsDraggable(UIElement dragElt)
        {
            return (!(dragElt is Canvas));
        }
    }

    public class CanvasDropTarget:DropTargetBase
    {
        public CanvasDropTarget()
        {
            SupportedFormat = "CanvasExample";
        }

        public override void OnDropCompleted(IDataObject obj, Point dropPoint)
        {
            Canvas canvas = TargetUI as Canvas;
            UIElement elt = ExtractElement(obj);
            canvas.Children.Add(elt);
            Canvas.SetLeft(elt, dropPoint.X);
            Canvas.SetTop(elt, dropPoint.Y);
        }
    }
}