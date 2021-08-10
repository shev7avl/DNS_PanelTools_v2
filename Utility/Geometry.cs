using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using DSKPrim.PanelTools_v2.StructuralApps;

namespace DSKPrim.PanelTools_v2.Utility
{
    public static class Geometry
    {
        /// <summary>
        /// Проверка находится ли заданная точка внутри BoundingBox
        /// </summary>
        /// <param name="boundingBox">Выбранный BoundingBox</param>
        /// <param name="point">Выбранная точка</param>
        /// <returns></returns>
        public static bool InBox(BoundingBoxXYZ boundingBox, XYZ point)
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
        public static List<Element> IntersectedOpenings(Element element, RevitLinkInstance revitLink, Document document, bool windows)
        {
            List<Element>  IntersectedElements = new List<Element>();
            Options options = new Options();
            BoundingBoxXYZ panelBbox = element.get_Geometry(options).GetBoundingBox();

            SingleArchDoc archDoc = SingleArchDoc.getInstance(document);

            List<Element> elems;
            if (windows)
            {
                elems = archDoc.Windows;
            }
            else
            {
                elems = archDoc.Doors;
            }

            XYZ transform = revitLink.GetTransform().Origin;

            foreach (var item in elems)
            {
                LocationPoint locationPointBase = (LocationPoint)item.Location;
                
                XYZ newPoint = new XYZ(locationPointBase.Point.X + transform.X, locationPointBase.Point.Y + transform.Y, locationPointBase.Point.Z + transform.Z);

                if (Geometry.InBox(panelBbox, newPoint))
                {
                    IntersectedElements.Add(item);
                }
            }
            return IntersectedElements;
        }



    }
}
