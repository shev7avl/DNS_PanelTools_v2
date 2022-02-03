using Autodesk.Revit.DB;
using System;
using System.Collections;
using System.Linq;

namespace DSKPrim.PanelTools.Global
{
    public class ViewParser
    {
        public Document Document;
        public string ViewTemplateName;
        public int PageNum;
        public Element View3D
        { 
            get {
                try
                {
                    return new FilteredElementCollector(Document).OfClass(typeof(View3D)).Where(o => o.Name.Contains(ViewTemplateName)).FirstOrDefault();
                }
                catch (NullReferenceException)
                {
                    throw new InvalidOperationException();
                }
            } 
        }

        public Element View
        {
            get
            {
                try
                {
                    return new FilteredElementCollector(Document).OfClass(typeof(View)).Where(o => o.Name.Contains(ViewTemplateName)).FirstOrDefault();
                }
                catch (NullReferenceException)
                {
                    throw new InvalidOperationException();
                }
            }
        }

        public Element ViewSchedule
        {
            get
            {
                try
                {
                    return new FilteredElementCollector(Document).OfClass(typeof(ViewSchedule)).Where(o => o.Name.Contains(ViewTemplateName)).FirstOrDefault();
                }
                catch (NullReferenceException)
                {
                    throw new InvalidOperationException();
                }
            }
        }

        public ViewParser(Document document, int pageNum, string viewTemplateName)
        {
            this.Document = document;
            this.PageNum = pageNum;
            this.ViewTemplateName = viewTemplateName;
        }
    }

    public class ViewPositions : IEnumerable
    {
        private readonly ViewParser[] _viewParsers;

        public ViewPositions(ViewParser[] viewParsers)
        {
            _viewParsers = new ViewParser[viewParsers.Length];

            for (int i = 0; i < viewParsers.Length; i++)
            {
                _viewParsers[i] = viewParsers[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)GetEnumerator();
        }

        public ViewEnum GetEnumerator()
        {
            return new ViewEnum(_viewParsers);
        }

    }

    public class ViewEnum : IEnumerator
    {
        public ViewParser[] _viewParsers;

        int position = -1;

        public ViewEnum(ViewParser[] viewParsers)
        {
            _viewParsers = viewParsers;
        }

        object IEnumerator.Current
        {
            get
            {
                return Current;
            }
        }

        public ViewParser Current
        {
            get
            {
                try
                {
                    return _viewParsers[position];
                }
                catch (IndexOutOfRangeException)
                {
                    throw new InvalidOperationException();
                }
            }
        }

        public bool MoveNext()
        {
            position++;
            return (position < _viewParsers.Length);
        }

        public void Reset()
        {
            position = -1;
        }
    }
}
