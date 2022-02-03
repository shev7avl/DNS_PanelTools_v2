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
        public override Document ActiveDocument { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override Element ActiveElement { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override AssemblyInstance AssemblyInstance { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override string LongMark { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override string ShortMark { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override string Index { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override void CreateMarks()
        {
            throw new NotImplementedException();
        }
    }
}
