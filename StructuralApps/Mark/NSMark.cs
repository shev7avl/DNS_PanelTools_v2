using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace DNS_PanelTools_v2.StructuralApps.Mark
{
    class NSMark : IPanelMark
    {

        public Document ActiveDocument { get; set; }
        public Element ActiveElement { get; set; }

        public string LongMark { get; set; }

        public string ShortMark { get; set; }


        public NSMark(Document document, Element element)
        {
            ActiveDocument = document;
            ActiveElement = element;
            SetMarks();
        }

        private void MarkLogic()
        {
            LongMark = $"НС {GetPanelCode()}_{GetClosureCode()}";
            ShortMark = $"НС {LongMark.Split('_')[1]}";
        }


        private void SetMarks()
        {
            Guid DNS_panelMark = new Guid("db2bee76-ce6f-4203-9fde-b8f34f3477b5");
            Guid ADSK_panelMark = new Guid("92ae0425-031b-40a9-8904-023f7389963b");
            Transaction transaction = new Transaction(ActiveDocument);


            MarkLogic();

            transaction.Start($"Транзакция - {ActiveElement.Name}");
            ActiveElement.get_Parameter(DNS_panelMark).Set(LongMark);
            ActiveElement.get_Parameter(ADSK_panelMark).Set(ShortMark);

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
            string[] temp_i6 = ActiveElement.LookupParameter("Тип PVL_СТАРТ").AsValueString().Split(' ');
            string i6 = temp_i6[1];
            string[] temp_i7 = ActiveElement.LookupParameter("Тип PVL_ФИНИШ").AsValueString().Split(' ');
            string i7 = temp_i7[1];
            return $"{i1}.{i2}_{i3}.{i4}.{i5}_{i6}.{i7}";
        }
        private string GetClosureCode()
        {
            bool Closure1 = ActiveElement.LookupParameter("ПР1.ВКЛ").AsValueString() == "Да";
            bool Closure2 = ActiveElement.LookupParameter("ПР2.ВКЛ").AsValueString() == "Да";
            string window1 = "";

            if (Closure1)
            {
                string w1 = GetDoubleValueAsDecimeterString(ActiveElement, "ПР1.Отступ");
                string w2 = GetDoubleValueAsDecimeterString(ActiveElement, "ПР1.Ширина");
                string w3 = GetDoubleValueAsDecimeterString(ActiveElement, "ПР1.Высота");
                string w4 = GetDoubleValueAsDecimeterString(ActiveElement, "ПР1.ВысотаСмещение");
                window1 = $"{w2}.{w3}.{w4}.{w1}";
            }
            string window2 = "";
            if (Closure2)
            {
                string w1 = GetDoubleValueAsDecimeterString(ActiveElement, "ПР2.Отступ");
                string w2 = GetDoubleValueAsDecimeterString(ActiveElement, "ПР2.Ширина");
                string w3 = GetDoubleValueAsDecimeterString(ActiveElement, "ПР2.Высота");
                string w4 = GetDoubleValueAsDecimeterString(ActiveElement, "ПР2.ВысотаСмещение");
                window2 = $"{w2}.{w3}.{w4}.{w1}";
            }
            string windows = "";
            if (Closure1 && Closure2)
            {
                windows = $"{window1}_{window2}";
            }
            else if (Closure1 || Closure1)
            {
                windows = $"{window1}{window2}";
            }
            else
            {
                windows = "Г";
            }

            return windows;
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
