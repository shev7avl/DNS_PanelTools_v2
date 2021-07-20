using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace DSKPrim.PanelTools_v2.StructuralApps.AssemblyDefiningSubelements
{
    public class FrontPVLTransferable : DefiningBase, ITransferable
    {
        public override Document Document { get; set; }
        public override Element Element { get; set; }
        public override string Name { get; set; }

        private FrontPVLTransferable(Element element)
        {
            this.Element = element;
            this.Name = element.Name;
        }

        public void Create(Element element)
        {
            if (IsTransferable(element))
            {
                new FrontPVLTransferable(element);
            }
        }

        public bool IsTransferable(Element element)
        {
            if (element.Name.Contains("PVL_Торцевая")) return true;
            else return false;   
        }
    }
}
