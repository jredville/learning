using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media.Animation;
using System.Xml;
using DnD;

namespace Samples.DragDrop
{
	public class DefaultDropTargetAdvisor : DropTargetBase
	{
        public DefaultDropTargetAdvisor()
        {
            SupportedFormat = "ABCFormat";
        }

        public override void OnDropCompleted(IDataObject obj, Point dropPoint)
		{
			UIElement elt = ExtractElement(obj);

			(TargetUI as Panel).Children.Add(elt);
		}

        public override UIElement GetVisualFeedback(IDataObject obj)
		{
			Button elt = ExtractElement(obj) as Button;
			elt.Width = 50;
			elt.Height = 60;
			elt.Opacity = 0.5;
			elt.IsHitTestVisible = false;

			DoubleAnimation anim = new DoubleAnimation(0.75, new Duration(TimeSpan.FromMilliseconds(500)));
			anim.From = 0.25;
			anim.AutoReverse = true;
			anim.RepeatBehavior = RepeatBehavior.Forever;
			elt.BeginAnimation(UIElement.OpacityProperty, anim);

			return elt;
		}

	}
}
