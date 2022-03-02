using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSKPrim.PanelTools.Panel
{
    class Facade_Panel : BasePanel
    {
        public override Document ActiveDocument { get ; set ; }
        public override Element ActiveElement { get ; set ; }
        public override AssemblyInstance AssemblyInstance { get ; set ; }
        public override string LongMark { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override string ShortMark { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override string Index { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public Facade_Panel(Document document, Element element)
        {
            ActiveDocument = document;
            ActiveElement = element;
        }

        public override void CreateMarks()
        {
            throw new NotImplementedException();
        }
    }
}
