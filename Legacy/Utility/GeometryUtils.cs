using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using System.IO;
using DSKPrim.PanelTools.ProjectEnvironment;

namespace DSKPrim.PanelTools.Utility
{
    public static class GeometryUtils
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

        private static List<string> ParseWindowFamilyNames()
        {
            string windowFamilyPath = "\\\\ir-dsk\\DNSDevelopment\\1_ДСК Приморье\\13. Проектный институт\\ДНС ПРОЕКТ РАБОЧАЯ\\BIM\\00_DNSPanel\\02_DNS_Семейства\\1. АР\\Окна";
            List<string> windowFamilyNames = new List<string>();

            string[] filePaths = Directory.GetFiles(windowFamilyPath);
            foreach (var item in filePaths)
            {
                string name = item.Split('\\').Last().Replace(".rfa", "");
                windowFamilyNames.Add(name);
            }

            return windowFamilyNames;
        }

        private static List<string> ParseDoorFamilyNames()
        {
            string doorFamilyPath = "\\\\ir-dsk\\DNSDevelopment\\1_ДСК Приморье\\13. Проектный институт\\ДНС ПРОЕКТ РАБОЧАЯ\\BIM\\00_DNSPanel\\02_DNS_Семейства\\1. АР\\Двери";
            List<string> doorFamilyNames = new List<string>();

            string[] filePaths = Directory.GetFiles(doorFamilyPath);
            foreach (var item in filePaths)
            {
                string name = item.Split('\\').Last().Replace(".rfa", "");
                doorFamilyNames.Add(name);
            }

            return doorFamilyNames;
        }
        

        private static bool LevelEq(Element i1, Element i2)
        {
            string l1 = i1.get_Parameter(BuiltInParameter.FAMILY_LEVEL_PARAM).AsValueString();
            string l2 = i2.get_Parameter(BuiltInParameter.FAMILY_LEVEL_PARAM).AsValueString();

            string lv1 = l1.Substring(l1.Length - 2);
            string lv2 = l2.Substring(l2.Length - 2);

            return lv1 == lv2;
        }

    }
}
