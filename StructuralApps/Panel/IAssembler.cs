using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using DSKPrim.PanelTools_v2.StructuralApps.AssemblyDefiningSubelements;

namespace DSKPrim.PanelTools_v2.StructuralApps.Panel
{
    public interface IAssembler
    {
        List<ElementId> AssemblyElements { get; set; }

        List<ITransferable> OutList { get; set; }

        void SetAssemblyElements();

        void TransferFromPanel(Panel panel);

        event EventHandler TransferRequested;

        void TransferHandler(object senger, EventArgs e);

    }
}
