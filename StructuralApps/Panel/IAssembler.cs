using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace DSKPrim.PanelTools_v2.StructuralApps.Panel
{
    public delegate void TransferHandler(object sender, EventArgs e);
    public interface IAssembler
    {
        List<ElementId> AssemblyElements { get; set; }

        List<ElementId> OutList { get; set; }

        List<ElementId> PVLList { get; set; }

        IAssembler TransferPal { get; set; }

        void SetAssemblyElements();

        void TransferFromPanel(IAssembler panel);

        event TransferHandler TransferRequested;

        void InTransferHandler(object senger, EventArgs e);

        void ExTransferHandler(object senger, EventArgs e);

    }
}
