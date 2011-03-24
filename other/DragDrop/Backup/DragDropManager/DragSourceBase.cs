using System.Windows;
using System.Windows.Markup;

namespace DnD
{
    public abstract class DragSourceBase
    {

        string supportedFormat = "SampleFormat";
        private UIElement _sourceElt;

		public virtual UIElement SourceUI
		{
            get
            {
                return _sourceElt;
            }
            set
            {
                _sourceElt = value;
            }
		}

        public virtual DragDropEffects SupportedEffects
		{
            get { return DragDropEffects.Copy | DragDropEffects.Move; }
		}

        public virtual string SupportedFormat
        {
            get { return supportedFormat; }
            set { supportedFormat = value; }
        }
  

        public virtual DataObject GetDataObject(UIElement draggedElt)
        {
            string serializedElt = XamlWriter.Save(draggedElt);
            DataObject obj = new DataObject();
            obj.SetData(supportedFormat,serializedElt);
            return obj;
        }

		public abstract void FinishDrag(UIElement draggedElt, DragDropEffects finalEffects);
        public abstract bool IsDraggable(UIElement dragElt);
    }
}
