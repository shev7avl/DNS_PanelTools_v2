using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace DNS_PanelTools_v2.StructuralApps.LongMark
{
    class BPLongMark : IPanelLongMark
    {
        public Document ActiveDocument { get; set; }
        public Element ActiveElement { get; set; }



        public BPLongMark(Document document, Element element)
        {
            ActiveDocument = document
            ; ActiveElement = element;
        }
        public string LongMarkLogic()
        {

            string output = $"БП-{GetPanelCode()}";

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
            string i3 = GetDoubleValueAsDecimeterString(familySymbol, "Плита_Длина");
            string i4 = GetDoubleValueAsDecimeterString(familySymbol, "Плита_Ширина");
            string i5 = GetDoubleValueAsDecimeterString(familySymbol, "Плита_Толщина");
            string i6 = GetDoubleValueAsDecimeterString(familySymbol, "Кронштейн_Отступ");
            string i7 = GetDoubleValueAsDecimeterString(familySymbol, "Кронштейн_Шаг");
            string i8 = familySymbol.LookupParameter("Кронштейн_Количество").AsValueString();
            string i9 = familySymbol.LookupParameter("Отверстия_ПривязкаСлева").AsValueString();
            string i10 = familySymbol.LookupParameter("Отверстия_ПривязкаСправа").AsValueString();
            string i11 = familySymbol.LookupParameter("Отверстия_ПривязкаСпереди").AsValueString();
            string i12 = familySymbol.LookupParameter("Отверстия_РасстояниеМеждуПоY").AsValueString();

            return $"{i3}.{i4}.{i5}_{i6}.{i7}.{i8}.1_{i9}.{i10}_{i11}.{i12}";
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
