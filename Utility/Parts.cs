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
        public static void SplitToParts(Document document, Element element)
        {
            Logger.Logger logger = Logger.Logger.getInstance();
            ICollection<ElementId> elementIdsToDivide;

            Wall familyInstance = (Wall)element;
            
            XYZ directionalBasis = familyInstance.Orientation;

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

            logger.DebugLog($"@Части для {element.Name} Созданы успешно");
            logger.DebugLog($"@");
            logger.DebugLog($"@Конец транзакции");
            logger.DebugLog($"------------");

            foreach (var item in elementIdsToDivide)
            {
                SplitGeometry.CreatePartsSection(document, item);
            }
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
            using (Transaction t = new Transaction(document, "excludin stitches"))
            {
                t.Start();
                foreach (ElementId id in partsId)
                {
                    Element item = document.GetElement(id);
                    if (item.get_Parameter(BuiltInParameter.DPART_AREA_COMPUTED).AsDouble() < 0.1)
                    {
                        Part part = (Part)item;
                        part.Excluded = true;
                    }
                }
                document.Regenerate();
                t.Commit();

            }

        }

    }
}
