using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace DNS_PanelTools_v2.StructuralApps
{
    public static class Geometry
    {
        /// <summary>
        /// Проверка находится ли заданная точка внутри BoundingBox
        /// </summary>
        /// <param name="boundingBox">Выбранный BoundingBox</param>
        /// <param name="point">Выбранная точка</param>
        /// <returns></returns>
        public static bool IsPointInsideBbox(BoundingBoxXYZ boundingBox, XYZ point)
        {
            double maxX = boundingBox.Max.X;
            double maxY = boundingBox.Max.Y;
            double maxZ = boundingBox.Max.Z;

            double minX = boundingBox.Min.X;
            double minY = boundingBox.Min.Y;
            double minZ = boundingBox.Min.Z;

            bool XCheck = (point.X >= minX && point.X <= maxX);
            bool YCheck = (point.Y >= minY && point.Y <= maxY);
            bool ZCheck = (point.Z >= minZ && point.Z <= maxZ);

            return XCheck && YCheck && ZCheck;

        }


        /// <summary>
        /// Возвращает список элементов из выбранного файла заданной категории и с заданным именем, пересекающих выбранный элемент в активном документе
        /// </summary>
        /// <param name="element">Выбранный элемент в активном документе</param>
        /// <param name="document">Файл rvt в которой необходимо найти элементы (Подать связанный, если элементы в нём)</param>
        /// <param name="builtInCategory">К какой категории принадлежат искомые элементы</param>
        /// <param name="nameSubstring">Часть имени искомых элементов</param>
        /// <returns></returns>
        public static List<Element> FindPointIntersections(Element element, Document document, BuiltInCategory builtInCategory, string nameSubstring)
        {
            List<Element>  IntersectedElements = new List<Element>();
            Options options = new Options();
            BoundingBoxXYZ panelBbox = element.get_Geometry(options).GetBoundingBox();

            IEnumerable<Element> listWindows = new FilteredElementCollector(document).OfCategory(builtInCategory).WhereElementIsNotElementType().ToElements().Where(o => o.Name.Contains(nameSubstring));

            foreach (var item in listWindows)
            {
                LocationPoint locationPoint = (LocationPoint)item.Location;
                Debug.WriteLine($"{item.Name} попал в список");
                if (Geometry.IsPointInsideBbox(panelBbox, locationPoint.Point))
                {
                    IntersectedElements.Add(item);
                    Debug.WriteLine($"{item.Name} пересекается с {element.Name}");
                }
            }
            return IntersectedElements;
        }



    }
}
