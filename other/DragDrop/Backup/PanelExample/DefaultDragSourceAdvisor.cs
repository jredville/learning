using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using DnD;

namespace Samples.DragDrop
{
    public class DefaultDragSourceAdvisor : DragSourceBase
    {

        public DefaultDragSourceAdvisor()
        {
            SupportedFormat = "ABCFormat";
        }

    	public  override void FinishDrag(UIElement draggedElt, DragDropEffects finalEffects)
		{
			if ((finalEffects & DragDropEffects.Move) == DragDropEffects.Move)
			{
				Panel panel = SourceUI as Panel;
				if (panel != null)
				{
					panel.Children.Remove(draggedElt);
				}
			}
		}


		public override  bool IsDraggable(UIElement dragElt)
		{
			return (dragElt is Button);
		}
	}
}
