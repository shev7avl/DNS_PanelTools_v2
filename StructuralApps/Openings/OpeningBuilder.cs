using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using DNS_PanelTools_v2.Operations;

namespace DNS_PanelTools_v2.StructuralApps.Openings
{
    public class OpeningBuilder
    {
        private Document ActiveDocument;

        private Document LinkedDocument;

        private List<Element> IntersectedWindows;

        public OpeningBuilder(Document document, Document linkedDoc)
        {
            ActiveDocument = document;
            LinkedDocument = linkedDoc;
        }


        

        public void CreateOpening(Element element)
        {
            Operations.Openings.FindIntersectedWindows_Struct(LinkedDocument, element, out IntersectedWindows);

            TransactionGroup transactionGroup = new TransactionGroup(ActiveDocument, $"Создание проемов - {element.Name}");
            if (IntersectedWindows.Count>0 && IntersectedWindows.Count<=2)
            {

                transactionGroup.Start();

                Element window = IntersectedWindows[0];

                Operations.Openings.SetOpeningParams(ActiveDocument, element, window);


                transactionGroup.Assimilate();
            }
        }

        

       
        

        

    }
}
