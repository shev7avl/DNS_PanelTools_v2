using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace DSKPrim.PanelTools_v2.StructuralApps.AssemblyDefiningSubelements
{
    class DefiningSubelement
    {
        private DefiningSubelement Instance;

        public List<Element> DefiningElements;

        private Document Document;

        private DefiningSubelement(Document document)
        {
            Document = document;
        
        }

        public DefiningSubelement getInstance(Document document)
        {
            if (Instance == null)
            {
                Instance = new DefiningSubelement(document);
            }
            return Instance;

        }

    }
}
