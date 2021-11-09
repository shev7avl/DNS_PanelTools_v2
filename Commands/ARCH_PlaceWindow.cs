using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using DSKPrim.PanelTools_v2.Architecture;
using DSKPrim.PanelTools_v2.StructuralApps.Panel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSKPrim.PanelTools_v2.Commands
{
    [Transaction(mode: TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    class ARCH_PlaceWindow : Routine
    {
        public override StructuralApps.Panel.Panel Behaviour { get; set; }
        public override Document Document { get; set ; }

        public override void ExecuteRoutine(ExternalCommandData commandData)
        {
            Document = commandData.Application.ActiveUIDocument.Document;
            Logger.Logger logger = Logger.Logger.getInstance();
            IEnumerable<Element> fecLinksARCH = new FilteredElementCollector(Document).OfCategory(BuiltInCategory.OST_RvtLinks).WhereElementIsNotElementType().Where(doc => doc.Name.Contains("_АР"));
            IEnumerable<Element> fecLinksSTRUCT = new FilteredElementCollector(Document).OfCategory(BuiltInCategory.OST_RvtLinks).WhereElementIsNotElementType().Where(doc => doc.Name.Contains("_КР"));
            FilteredElementCollector fecWalls = new FilteredElementCollector(Document).OfCategory(BuiltInCategory.OST_Walls).WhereElementIsNotElementType();

            Document linkedDocARCH = fecLinksARCH.Cast<RevitLinkInstance>().ToList()[0].GetLinkDocument();
            Document linkedDocSTR = fecLinksSTRUCT.Cast<RevitLinkInstance>().ToList()[0].GetLinkDocument();

            logger.DebugLog(linkedDocARCH.PathName);

            foreach (Element item in fecWalls)
            {
                WallParts wallParts = new WallParts(Document, linkedDocSTR, linkedDocARCH, item);
                Utility.Openings.GetWindows_Arch(linkedDocARCH, item, out List<Element> IntersectedWindows);
                foreach (Element window in IntersectedWindows)
                {
                    LocationPoint locationPoint = (LocationPoint)window.Location;
                    Utility.Openings.CreateFacadeOpening(Document, item);
                }
                logger.DebugLog(item.Name);
            }
        }
    }

}
