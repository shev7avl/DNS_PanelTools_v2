using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace DNS_PanelTools_v2.StructuralApps.LongMark
{
    class PPLongMark : IPanelLongMark
    {
        public Document ActiveDocument { get; set; }
        public Element ActiveElement { get; set; }



        public PPLongMark(Document document, Element element)
        {
            ActiveDocument = document
            ; ActiveElement = element;
        }
        public string LongMarkLogic()
        {

            string output = $"ПП-{GetPanelCode()}{GetClosureCode()}";

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
            string i3 = GetDoubleValueAsDecimeterString(ActiveElement, "ADSK_Размер_Длина");
            string i4 = GetDoubleValueAsDecimeterString(ActiveElement, "ADSK_Размер_Ширина");
            string i5 = ActiveElement.LookupParameter("КодНагрузки").AsValueString();
            return $"{i3}.{i4}_{i5}";
        }
        private string GetClosureCode()
        {
            var elementFamily = ActiveElement as FamilyInstance;
            var familySymbol = elementFamily.Symbol;

            bool closureBool = familySymbol.LookupParameter("Вырезы").AsValueString() == "Да";

            string closureCode = "";

            if (closureBool)
            {
                string w1 = GetDoubleValueAsDecimeterString(ActiveElement, "Вырезы_Отступ_Начало");
                string w2 = GetDoubleValueAsDecimeterString(familySymbol, "Вырезы_Шаг");
                string w3 = ActiveElement.LookupParameter("Отверстия_Количество").AsValueString();
                string w4 = GetDoubleValueAsDecimeterString(ActiveElement, "Вырезы_Отступ_Конец");
                closureCode = $"_{w1}.{w2}.{w3}.{w4}";
            }

            return closureCode;
        }


        private string GetDoubleValueAsDecimeterString(Element element, string lkp)
        {
        return Math.Round(Convert.ToDouble(element.LookupParameter(lkp).AsValueString()) / 10, 0).ToString();
        }
        private string GetDoubleValueAsDecimeterString(FamilySymbol elementFamily, string lkp)
        {
            return Math.Round(Convert.ToDouble(elementFamily.LookupParameter(lkp).AsValueString()) / 10, 0).ToString();
        }

    }
}
