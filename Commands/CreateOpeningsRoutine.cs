using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using System.Diagnostics;
using DSKPrim.PanelTools_v2.StructuralApps.Panel;
using DSKPrim.PanelTools_v2.StructuralApps;

namespace DSKPrim.PanelTools_v2.Commands
{
    [Transaction(mode: TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    class CreateOpeningsRoutine
        : Routine
    {
        public override Document Document { get; set; }

        Document LinkedArch;

        public override StructuralApps.Panel.Panel Behaviour { get; set; }

        public override void ExecuteRoutine(ExternalCommandData commandData)
        {
            Document = commandData.Application.ActiveUIDocument.Document;

            IEnumerable<Element> fecLinksARCH = new FilteredElementCollector(Document).OfCategory(BuiltInCategory.OST_RvtLinks).WhereElementIsNotElementType().Where(doc => doc.Name.Contains("_АР"));

            FilteredElementCollector fecStruct = new FilteredElementCollector(Document).OfCategory(BuiltInCategory.OST_StructuralFraming).WhereElementIsNotElementType();
            FilteredElementCollector fecWalls = new FilteredElementCollector(Document).OfCategory(BuiltInCategory.OST_Walls).WhereElementIsNotElementType();

            LinkedArch = fecLinksARCH.Cast<RevitLinkInstance>().ToList()[0].GetLinkDocument();

            List<Element> els;
            if (fecStruct.GetElementCount() == 0)
            {
                els = fecWalls.Cast<Element>().ToList();
            }
            else
            {
                els = fecStruct.Cast<Element>().ToList();
            }

            foreach (Element item in els)
            {
                SetPanelBehaviour(item);
                if (Behaviour is IPerforable perforable)
                {
                    Debug.WriteLine($"Панель: {item.Name}; Id: {item.Id}");
                    IPerforable panel = perforable;
                    panel.GetOpenings(LinkedArch, out List<Element> IntersectedWindows);
                    panel.Perforate(IntersectedWindows);
                }
            }
        }

    }
}
