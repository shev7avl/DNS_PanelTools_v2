using Autodesk.Revit.DB;
using DSKPrim.PanelTools_v2.Architecture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSKPrim.PanelTools_v2.Utility
{
    public static class Parts
    {
        /// <summary>
        /// Разрезает стену на части
        /// </summary>
        /// <param name="document">Активный документ с АКР</param>
        /// <param name="element">Стена фасада</param>
        public static void SplitToParts(Document document, Element element, bool straight=false)
        {
            Logger.Logger logger = Logger.Logger.getInstance();
            ICollection<ElementId> elementIdsToDivide;

            ICollection<ElementId> elementIds = new List<ElementId>()
            {
                element.Id
            };
            logger.DebugLog($"------------");
            logger.DebugLog($"@Начало транзакции");
            logger.DebugLog($"@");
            if (PartUtils.AreElementsValidForCreateParts(document, elementIds))
            {
                using (Transaction transaction = new Transaction(document, "Parts creation"))
                {
                    transaction.Start();
                    PartUtils.CreateParts(document, elementIds);
                    transaction.Commit();
                }
            }
            ElementFilter filter = new ElementCategoryFilter(BuiltInCategory.OST_Parts);
            elementIdsToDivide = element.GetDependentElements(filter);


                SplitGeometry.CreatePartsSection(document, elementIdsToDivide.FirstOrDefault(), straight);

            logger.DebugLog($"@Части для {element.Name} Созданы успешно");
            logger.DebugLog($"@");
            logger.DebugLog($"@Конец транзакции");
            logger.DebugLog($"------------");
        }

        /// <summary>
        /// Исключает части из разрезки по их площади. Нужно для исключения швов плитки
        /// </summary>
        /// <param name="document"></param>
        /// <param name="element"></param>
        public static void ExcludeStitches(Document document, Element element)
        {
            ElementFilter categoryFilter = new ElementCategoryFilter(BuiltInCategory.OST_Parts);
            List<ElementId> partsId = (List<ElementId>)element.GetDependentElements(categoryFilter);
            List<Element> pts = new List<Element>();

            foreach (var id in partsId)
            {
                Element item = document.GetElement(id);
                pts.Add(item);
            }

            pts.Sort(CompareElementsByArea);
            pts.Reverse();

            

            using (Transaction t = new Transaction(document, "excludin stitches"))
            {
                t.Start();
                        Part part = (Part)pts.First();
                        part.Excluded = true;   
                document.Regenerate();
                t.Commit();

            }
        }

        private static int CompareElementsByArea (Element x, Element y)
        {
            double areaX = x.get_Parameter(BuiltInParameter.DPART_AREA_COMPUTED).AsDouble();
            double areaY = y.get_Parameter(BuiltInParameter.DPART_AREA_COMPUTED).AsDouble();
            if (areaX > areaY)
            {
                return 1;
            }
            else if (areaY > areaX)
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }
    }
}
