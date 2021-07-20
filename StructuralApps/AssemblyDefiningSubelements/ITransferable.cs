using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace DSKPrim.PanelTools_v2.StructuralApps.AssemblyDefiningSubelements
{
    public interface ITransferable
    {
        void Create(Element element);

        bool IsTransferable(Element element);
       
    }
}
