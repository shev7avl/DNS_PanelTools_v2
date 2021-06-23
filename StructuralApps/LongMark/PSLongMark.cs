using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace DNS_PanelTools_v2.StructuralApps.LongMark
{
    class PSLongMark : IPanelLongMark
    {
        public Document ActiveDocument { get; set; }
        public Element ActiveElement { get; set; }



        public PSLongMark(Document document, Element element)
        {
            ActiveDocument = document;
            ActiveElement = element;
        }
        public string LongMarkLogic()
        {

            string output = $"ПС-{GetPanelCode()}";
            return output;

        }

        public void SetLongMark()
        {
            string value = LongMarkLogic();
            Guid DNS_panelMark = new Guid("db2bee76-ce6f-4203-9fde-b8f34f3477b5");
            Guid ADSK_panelMark = new Guid("92ae0425-031b-40a9-8904-023f7389963b");
            Transaction transaction = new Transaction(ActiveDocument);

            transaction.Start($"Транзакция - {ActiveElement.Name}");
            ActiveElement.get_Parameter(ADSK_panelMark).Set(value);
            ActiveElement.get_Parameter(DNS_panelMark).Set(value);
            transaction.Commit();
        }


        private string GetPanelCode()
        {
            var elementFamily = ActiveElement as FamilyInstance;
            var familySymbol = elementFamily.Symbol;
            string i1 = ActiveElement.LookupParameter("СТАРТ").AsValueString();
            string i2 = ActiveElement.LookupParameter("ФИНИШ").AsValueString();
            string i3 = GetDoubleValueAsDecimeterString(ActiveElement, "ГабаритДлина");
            string i4 = GetDoubleValueAsDecimeterString(familySymbol, "ГабаритВысота");
            string i5 = GetDoubleValueAsDecimeterString(familySymbol, "ГабаритТолщина");
            
            string i6 = GetDoubleValueAsDecimeterString(ActiveElement, "WELDA_Отступ");
            string i7 = GetDoubleValueAsDecimeterString(ActiveElement, "WELDA_Шаг");
            string i8 = ActiveElement.LookupParameter("WELDA_Количество").AsValueString();

            string i9 = GetDoubleValueAsDecimeterString(ActiveElement, "TR 24_Сверху_Отступ");
            string i10 = GetDoubleValueAsDecimeterString(ActiveElement, "TR 24_Сверху_Шаг");
            string i11 = ActiveElement.LookupParameter("TR 24_Сверху_Количество").AsValueString();

           
            return $"{i1}.{i2}_{i3}.{i4}.{i5}_{i6}.{i7}.{i8}_{i9}.{i10}.{i11}";
        }
        


        private string GetDoubleValueAsDecimeterString(Element element, string lkp)
        {
        return Math.Round(Convert.ToDouble(element.LookupParameter(lkp).AsValueString()) / 10, 0).ToString();
        }
        private string GetDoubleValueAsDecimeterString(FamilySymbol elementFamily, string lkp)
        {
            return Math.Round(Convert.ToDouble(elementFamily.LookupParameter(lkp).AsValueString()) / 10, 0).ToString();
        }

        public void SetLongMark(Element element)
        {
            throw new NotImplementedException();
        }
    }
}
