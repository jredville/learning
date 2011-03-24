using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Documents;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace DragVisual 
{
    class DragAdorner : Adorner
    {
        protected UIElement _child;
        protected UIElement _owner;
        protected double _xCenter;
        protected double _yCenter;

        public DragAdorner(UIElement owner) : base(owner) { }

        public DragAdorner(UIElement owner, UIElement adornElement, bool useVisualBrush, double opacity, double scaleRatio)
            : base(owner)
        {
            _owner = owner;
            if (useVisualBrush)
            {
                VisualBrush brush = new VisualBrush(adornElement);
                brush.Opacity = opacity;
                Rectangle r = new Rectangle();
                r.RadiusX = 3;
                r.RadiusY = 3;

                r.Width = adornElement.DesiredSize.Width * scaleRatio;
                r.Height = adornElement.DesiredSize.Height * scaleRatio;

                _xCenter = r.Width / 2;
                _yCenter = r.Height / 2;

                r.Fill = brush;
                _child = r;

            }
            else
                _child = adornElement;
        }


        private double _leftOffset;
        public double LeftOffset
        {
            get { return _leftOffset; }
            set
            {
                _leftOffset = value - _xCenter;
                UpdatePosition();
            }
        }

        private double _topOffset;
        public double TopOffset
        {
            get { return _topOffset; }
            set
            {
                _topOffset = value - _yCenter;

                UpdatePosition();
            }
        }

        private void UpdatePosition()
        {
            AdornerLayer adorner = (AdornerLayer)this.Parent;
            if (adorner != null)
            {
                adorner.Update(this.AdornedElement);
            }
        }

        protected override Visual GetVisualChild(int index)
        {
            return _child;
        }

        protected override int VisualChildrenCount
        {
            get
            {
                return 1;
            }
        }


        protected override Size MeasureOverride(Size finalSize)
        {
            _child.Measure(finalSize);
            return _child.DesiredSize;
        }
        protected override Size ArrangeOverride(Size finalSize)
        {
            _child.Arrange(new Rect(_child.DesiredSize));
            return finalSize;
        }
        
        public override GeneralTransform GetDesiredTransform(GeneralTransform transform)
        {
            GeneralTransformGroup result = new GeneralTransformGroup();
            result.Children.Add(base.GetDesiredTransform(transform));
            result.Children.Add(new TranslateTransform(_leftOffset, _topOffset));
            return result;
        }
    }
}
