using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using DNS_PanelTools_v2.StructuralApps.Mark;
using DNS_PanelTools_v2.StructuralApps;
using System.Diagnostics;

namespace DNS_PanelTools_v2.Commands
{
    [Transaction(mode: TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class SetMarks : IExternalCommand
    {
        public Document Document;
        public IPanelMark Behaviour { get; set; }
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document = commandData.Application.ActiveUIDocument.Document;

            TransactionGroup transactionGroup = new TransactionGroup(Document, "Присвоение длинной марки - ");
            transactionGroup.Start("Группа транзакций");
            FilteredElementCollector fec = new FilteredElementCollector(Document).OfCategory(BuiltInCategory.OST_StructuralFraming).WhereElementIsNotElementType();
            List<Element> els = fec.ToElements().Cast<Element>().ToList();
            foreach (var item in els)
            {             
                SetPanelBehaviour(item);
                Behaviour.SetMarks();
            }
            transactionGroup.Assimilate();
            transactionGroup.Dispose();
            

            return Result.Succeeded;
        }

        protected void SetPanelBehaviour(Element element)
        {
            StructureType structureType = new StructureType(element);
            string type = structureType.GetPanelType(element);
            if (type == StructureType.Panels.NS.ToString())
            {
                NSMark nS = new NSMark(Document, element);
                Behaviour = nS;
            }
            if (type == StructureType.Panels.VS.ToString())
            {
                VSMark vS = new VSMark(Document, element);
                Behaviour = vS;
            }
            if (type == StructureType.Panels.BP.ToString())
            {
                BPMark bP = new BPMark(Document, element);
                Behaviour = bP;
            }
            if (type == StructureType.Panels.PS.ToString())
            {
                PSMark pS = new PSMark(Document, element);
                Behaviour = pS;
            }
            if (type == StructureType.Panels.PP.ToString())
            {
                PPMark pP = new PPMark(Document, element);
                Behaviour = pP;
            }

        }
    }
}
