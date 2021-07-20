using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace DSKPrim.PanelTools_v2.StructuralApps.AssemblyDefiningSubelements
{
    public class PShapedTransferable : DefiningBase, ITransferable
    {

        public override Document Document { get; set; }

        public override Element Element { get; set; }
        public override string Name { get; set; }

        public void Create(Element element)
        {
            throw new NotImplementedException();
        }

        public bool IsTransferable(Element element)
        {
            throw new NotImplementedException();
        }
    }
}
