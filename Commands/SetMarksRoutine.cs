using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using DSKPrim.PanelTools_v2.StructuralApps.Panel;
using DSKPrim.PanelTools_v2.StructuralApps;
using System.Diagnostics;

namespace DSKPrim.PanelTools_v2.Commands
{

    public class SetMarksRoutine : Routine
    {
        public override Document Document { get; set; }
        public override StructuralApps.Panel.Panel Behaviour { get; set; }


        public override void ExecuteRoutine(ExternalCommandData commandData)
        {
            Document = commandData.Application.ActiveUIDocument.Document;

            TransactionGroup transactionGroup = new TransactionGroup(Document, "Присвоение длинной марки - ");
            transactionGroup.Start("Группа транзакций");
            FilteredElementCollector fec = new FilteredElementCollector(Document).OfCategory(BuiltInCategory.OST_StructuralFraming).WhereElementIsNotElementType();
            List<Element> els = fec.ToElements().Cast<Element>().ToList();
            foreach (var item in els)
            {
                SetPanelBehaviour(item);
                Behaviour.CreateMarks();
            }
            transactionGroup.Assimilate();
            transactionGroup.Dispose();

        }

        
    }
}
