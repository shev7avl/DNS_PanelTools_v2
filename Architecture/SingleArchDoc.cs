using DSKPrim.PanelTools_v2.StructuralApps.Panel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace DSKPrim.PanelTools_v2.StructuralApps
{
    public class SingleArchDoc
    {
        private static SingleArchDoc instance;

        private Document Document;

        private List<Element> Windows;

        private List<Element> Doors;

        private SingleArchDoc(Document document)
        {
            Document = document;

            IEnumerable<Element> elements = new FilteredElementCollector(Document).OfCategory(BuiltInCategory.OST_Windows).WhereElementIsNotElementType().ToElements().Where<Element>(o => o.Name.Contains("DNS_"));
            IEnumerable<Element> doors = new FilteredElementCollector(Document).OfCategory(BuiltInCategory.OST_Doors).WhereElementIsNotElementType().ToElements();

            Windows = elements.ToList();
            Doors = doors.ToList();
        }

        public static SingleArchDoc getInstance(Document document)
        {
            if (instance == null)
            {
                instance = new SingleArchDoc(document);
            }
            return instance;
        }

        public List<Element> getWindows()
        {
            return Windows;
        }

        public List<Element> getDoors()
        {
            return Doors;
        }

        public void Dispose()
        {
            Document = null;
            instance = null;
            Windows = null;
        }

       

    }
}
