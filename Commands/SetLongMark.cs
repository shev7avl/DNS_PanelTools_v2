using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using DNS_PanelTools_v2.StructuralApps.LongMark;
using DNS_PanelTools_v2.StructuralApps;
using System.Diagnostics;

namespace DNS_PanelTools_v2.Commands
{
    [Transaction(mode: TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class SetLongMark : IExternalCommand
    {
        public Document Document;
        public IPanelLongMark Behaviour { get; set; }
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
                Debug.WriteLine(Behaviour.LongMarkLogic());
                Behaviour.SetLongMark();

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
                NSLongMark nS = new NSLongMark(Document, element);
                Behaviour = nS;
            }
            if (type == StructureType.Panels.VS.ToString())
            {
                VSLongMark vS = new VSLongMark(Document, element);
                Behaviour = vS;
            }
            if (type == StructureType.Panels.BP.ToString())
            {
                BPLongMark bP = new BPLongMark(Document, element);
                Behaviour = bP;
            }
            if (type == StructureType.Panels.PS.ToString())
            {
                PSLongMark pS = new PSLongMark(Document, element);
                Behaviour = pS;
            }
            if (type == StructureType.Panels.PP.ToString())
            {
                PPLongMark pP = new PPLongMark(Document, element);
                Behaviour = pP;
            }

        }
    }
}
