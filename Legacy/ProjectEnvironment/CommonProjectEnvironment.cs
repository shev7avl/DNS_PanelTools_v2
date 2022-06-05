using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSKPrim.PanelTools.ProjectEnvironment
{
    public class CommonProjectEnvironment
    {
        private static CommonProjectEnvironment Instance;

        private static ViewsEnvironment ViewsEnvironment;

        private static StructuralEnvironment StructuralEnvironment;

        private CommonProjectEnvironment(Document document)
        {
            if (ViewsEnvironment is null)
            {
                ViewsEnvironment = ViewsEnvironment.getInstance(document);
            }
            if (StructuralEnvironment is null)
            {
                StructuralEnvironment = StructuralEnvironment.GetInstance(document);
            }
        }

        public StructuralEnvironment GetStructuralEnvironment()
        { return StructuralEnvironment; }

        public ViewsEnvironment GetViewsEnvironment()
        { return ViewsEnvironment; }

        public static CommonProjectEnvironment GetInstance(Document document)
        {
            if (Instance is null)
            {
                Instance = new CommonProjectEnvironment(document);
            }
            return Instance;
        }

        public static List<RevitLinkInstance> FindLinkedDocuments(Document document)
        { 

            return new FilteredElementCollector(document).
                OfClass(typeof(RevitLinkInstance)).
                WhereElementIsNotElementType().
                Cast<RevitLinkInstance>().
                ToList();

        }

        public void Reset()
        {
            ViewsEnvironment.Reset();
            ViewsEnvironment = null;
            StructuralEnvironment.Reset();
            StructuralEnvironment = null;
            Instance = null;
        }
    }
}
