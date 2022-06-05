using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Autodesk.Revit.DB;

namespace DSKPrim.PanelTools.ProjectEnvironment
{
    public class SingleArchDoc
    {
        private static SingleArchDoc instance;

        private Document Document;

        public List<Element> Windows { get; private set; }

        public List<Element> Doors { get; private set; }

        public Transform Transform;

        private SingleArchDoc(Document document)
        {

            if (document is null)
            {
                Debug.WriteLine("ПРЕДУПРЕЖДЕНИЕ: Связь не загружена");
            }
            else
            {
                Debug.WriteLine($"Анализируем файл АР: {document.Title}");

                Document = document;

                    IEnumerable<Element> windows = new FilteredElementCollector(Document).OfCategory(BuiltInCategory.OST_Windows).WhereElementIsNotElementType().ToElements().Where<Element>(o => o.Name.Contains("DNS_"));
                    IEnumerable<Element> doors = new FilteredElementCollector(Document).OfCategory(BuiltInCategory.OST_Doors).WhereElementIsNotElementType().ToElements().Where<Element>(o => !o.Name.Contains("стекл"));
                    Windows = windows.ToList();
                    Doors = doors.ToList();
            }

            
        }

        public static SingleArchDoc GetInstance(Document document)
        {
            if (instance == null)
            {
                instance = new SingleArchDoc(document);
            }
            return instance;
        }

       

        public void Dispose()
        {
            Document = null;
            instance = null;
            Windows = null;
        }

       

    }
}
