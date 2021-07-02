using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using DNS_PanelTools_v2.Architecture;
using DNS_PanelTools_v2.StructuralApps.Panel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNS_PanelTools_v2.Commands
{
    [Transaction(mode: TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    class ARCH_PlaceWindow : IExternalCommand
    {
        Document ActiveDocument;
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            ActiveDocument = commandData.Application.ActiveUIDocument.Document;
            IEnumerable<Element> fecLinksARCH = new FilteredElementCollector(ActiveDocument).OfCategory(BuiltInCategory.OST_RvtLinks).WhereElementIsNotElementType().Where(doc => doc.Name.Contains("_АР"));
            IEnumerable<Element> fecLinksSTRUCT = new FilteredElementCollector(ActiveDocument).OfCategory(BuiltInCategory.OST_RvtLinks).WhereElementIsNotElementType().Where(doc => doc.Name.Contains("_КР"));
            FilteredElementCollector fecWalls = new FilteredElementCollector(ActiveDocument).OfCategory(BuiltInCategory.OST_Walls).WhereElementIsNotElementType();

            Document linkedDocARCH = fecLinksARCH.Cast<RevitLinkInstance>().ToList()[0].GetLinkDocument();
            Document linkedDocSTR = fecLinksSTRUCT.Cast<RevitLinkInstance>().ToList()[0].GetLinkDocument();

            Debug.WriteLine(linkedDocARCH.PathName);

            foreach (Element item in fecWalls)
            {
                WallParts wallParts = new WallParts(ActiveDocument, linkedDocSTR, linkedDocARCH, item);
                Utility.Openings.GetWindows_Arch(linkedDocARCH, item, out List<Element> IntersectedWindows);
                foreach (Element window in IntersectedWindows)
                {
                    LocationPoint locationPoint = (LocationPoint) window.Location;
                    Utility.Openings.CreateFacadeOpening(ActiveDocument, locationPoint.Point, window, item);
                }
                Debug.WriteLine(item.Name);
            }
           
            return Result.Succeeded;
        }
    }

}
