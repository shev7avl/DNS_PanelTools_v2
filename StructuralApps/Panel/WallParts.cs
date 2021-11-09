using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using DSKPrim.PanelTools_v2.Architecture;
using DSKPrim.PanelTools_v2.StructuralApps;
using DSKPrim.PanelTools_v2.StructuralApps.Panel;
using DSKPrim.PanelTools_v2.Utility;

namespace DSKPrim.PanelTools_v2.StructuralApps.Panel
{
    public class WallParts : Panel, IPerforable
    {
        public override Document ActiveDocument { get; set; }

        public override Element ActiveElement { get; set; }

        public override string ShortMark { get; set; }

        public override string LongMark { get; set; }

        public override string Index { get; set; }

        public override List<XYZ> IntersectedWindows { get; set; }

        private readonly Document LinkedDocSTR;


        public WallParts(Document document, Document linkedDocSTR, Document linkedDocARCH, Element element)
        {
            ActiveDocument = document;
            LinkedDocSTR = linkedDocSTR;
            ActiveElement = element;
        }

        public WallParts(Document document, Element element)
        {
            ActiveDocument = document;
            ActiveElement = element;
        }

        void IPerforable.GetOpeningsFromLink(Document linkedArch, RevitLinkInstance revitLink, out List<Element> IntersectedWindows)
        {
            SingleArchDoc archDoc = SingleArchDoc.getInstance(linkedArch);
            List<Element> windows = archDoc.Windows;

            IntersectedWindows = new List<Element>();

            LocationCurve location = (LocationCurve)ActiveElement.Location;
            Curve curve = location.Curve;

            XYZ Start = curve.GetEndPoint(0);
            XYZ End = curve.GetEndPoint(1);

            Options options = new Options();

            foreach (Element window in windows)
            {
                FamilyInstance windowFamInst = window as FamilyInstance;
                Element hostWall = windowFamInst.Host;
                BoundingBoxXYZ boundingBoxHostWall = hostWall.get_Geometry(options).GetBoundingBox();
                if (Geometry.InBox(boundingBoxHostWall, Start) || Geometry.InBox(boundingBoxHostWall, End))
                {
                    IntersectedWindows.Add(window);
                }
            }
        }

        void IPerforable.Perforate(List<Element> IntersectedWindows, RevitLinkInstance revitLink)
        {
            IEnumerable<Element> familySymbols = new FilteredElementCollector(ActiveDocument).OfCategory(BuiltInCategory.OST_Windows).WhereElementIsElementType().Where(o => o.Name.Contains("DNS_ПроемДляПлитки"));
            FamilySymbol familySymbol = (FamilySymbol)familySymbols.First();

            foreach (Element window in IntersectedWindows)
            {
                FamilyInstance windowFamInst = window as FamilyInstance;
                Element hostWall = windowFamInst.Host;
                Level level = ActiveDocument.GetElement(hostWall.LevelId) as Level;

                LocationPoint locationPoint = (LocationPoint)window.Location;

                using (Transaction transaction = new Transaction(ActiveDocument, "Create window"))
                {
                    transaction.Start();
                    ActiveDocument.Create.NewFamilyInstance(locationPoint.Point, familySymbol, ActiveElement, level, StructuralType.NonStructural);
                    transaction.Commit();
                }
            }


        }


        public override void CreateMarks()
        {
            Logger.Logger logger = Logger.Logger.getInstance();
            Options options = new Options();
            LocationCurve curve = (LocationCurve)ActiveElement.Location;
            XYZ Start = curve.Curve.GetEndPoint(0);
            XYZ End = curve.Curve.GetEndPoint(0);

            SingleStructDoc marksList = SingleStructDoc.getInstance(LinkedDocSTR);
            List<Panel> panels = marksList.PanelMarks;



            foreach (Panel item in panels)
            {
                BoundingBoxXYZ boundingBox = item.ActiveElement.get_Geometry(options).GetBoundingBox();

                if (Geometry.InBox(boundingBox, Start) && Geometry.InBox(boundingBox, End))
                {
                    logger.DebugLog($"{ActiveElement.Name} пересекается с: {item.LongMark}");
                    Transaction transaction = new Transaction(ActiveDocument, $"Назначение марки: {item.LongMark}");
                    transaction.Start();

                    ActiveElement.LookupParameter("DNS_Код изделия полный").Set(item.LongMark);
                    ActiveElement.LookupParameter("DNS_Марка элемента").Set(item.LongMark);
                    ActiveElement.LookupParameter("ADSK_Марка конструкции").Set(item.LongMark);
                    transaction.Commit();
                }
            }
        }

        public void SplitToParts()
        {
            Logger.Logger logger = Logger.Logger.getInstance();
            ICollection<ElementId> elementIdsToDivide;

            ICollection<ElementId> elementIds = new List<ElementId>()
            {
                ActiveElement.Id
            };
            logger.DebugLog($"------------");
            logger.DebugLog($"@Начало транзакции");
            logger.DebugLog($"@");
            if (PartUtils.AreElementsValidForCreateParts(ActiveDocument, elementIds))
            {
                using (Transaction transaction = new Transaction(ActiveDocument, "Parts creation"))
                {
                    transaction.Start();
                    PartUtils.CreateParts(ActiveDocument, elementIds);
                    transaction.Commit();
                }                
            }
            ElementFilter filter = new ElementCategoryFilter(BuiltInCategory.OST_Parts);
            elementIdsToDivide = ActiveElement.GetDependentElements(filter);

            logger.DebugLog($"@Части для {ActiveElement.Name} Созданы успешно");
            logger.DebugLog($"@");
            logger.DebugLog($"@Конец транзакции");
            logger.DebugLog($"------------");

            foreach (var item in elementIdsToDivide)
            {
                SplitGeometry.CreatePartsSection(ActiveDocument, item);
            }
        }

        public void ExcludeStitches()
        {
            ElementFilter categoryFilter = new ElementCategoryFilter(BuiltInCategory.OST_Parts);
            List<ElementId> partsId = (List<ElementId>)ActiveElement.GetDependentElements(categoryFilter);
            using (Transaction t = new Transaction(ActiveDocument, "excludin stitches"))
            {
                t.Start();
                foreach (ElementId id in partsId)
                {
                    Element item = ActiveDocument.GetElement(id);
                    if (item.get_Parameter(BuiltInParameter.DPART_AREA_COMPUTED).AsDouble() < 0.1)
                    {
                        Part part = (Part)item;
                        part.Excluded = true;
                    }
                }
                t.Commit();

            }
           
        }
        
    }
}
