using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using DNS_PanelTools_v2.Operations;

namespace DNS_PanelTools_v2.StructuralApps.Panel
{
    class VS_Panel : IPanel
    {
        #region Fields
        public override Document ActiveDocument { get; set; }
        public override Element ActiveElement { get; set; }
        public override List<XYZ> IntersectedWindows { get; set; }
        public override string LongMark { get; set; }
        public override string ShortMark { get; set; }

        #endregion

        #region Constructor
        public VS_Panel(Document document, Element element)
        {
            ActiveDocument = document;
            ActiveElement = element;
        }
        #endregion

        #region Public Methods
        public override void CreateMarks()
        {

            LongMark = $"ВС {GetPanelCode()}_{GetClosureCode()}";
            ShortMark = $"{LongMark.Split('_')[0]}";
            SetMarks();

        }

        public override void OverrideShortMark(string newMark)
        {
            ShortMark = newMark;
            Guid ADSK_panelMark = new Guid("92ae0425-031b-40a9-8904-023f7389963b");
            Transaction transaction = new Transaction(ActiveDocument, $"Назначение индекса: {newMark}");
            transaction.Start();
            ActiveElement.get_Parameter(ADSK_panelMark).Set(ShortMark);
            transaction.Commit();

        }
        #endregion

        #region Private Methods
        private void SetMarks()
        {
            Guid DNS_panelMark = new Guid("db2bee76-ce6f-4203-9fde-b8f34f3477b5");
            Guid ADSK_panelMark = new Guid("92ae0425-031b-40a9-8904-023f7389963b");
            Transaction transaction = new Transaction(ActiveDocument);

            transaction.Start($"Транзакция - {ActiveElement.Name}");
            ActiveElement.get_Parameter(DNS_panelMark).Set(LongMark);
            ActiveElement.get_Parameter(ADSK_panelMark).Set(ShortMark);

            transaction.Commit();
        }
        private string GetPanelCode()
        {
            var elementFamily = ActiveElement as FamilyInstance;
            var familySymbol = elementFamily.Symbol;
            string i3 = Marks.GetDoubleValueAsDecimeterString(ActiveElement, "ГабаритДлина");
            string i4 = Marks.GetDoubleValueAsDecimeterString(familySymbol, "ГабаритВысота");
            string i5 = Marks.GetDoubleValueAsDecimeterString(familySymbol, "ГабаритТолщина");
            string[] temp_i6 = ActiveElement.LookupParameter("Тип PVL_СТАРТ").AsValueString().Split(' ');
            string i6 = temp_i6[1];
            string[] temp_i7 = ActiveElement.LookupParameter("Тип PVL_ФИНИШ").AsValueString().Split(' ');
            string i7 = temp_i7[1];
            return $"{i3}.{i4}.{i5}_{i6}.{i7}";
        }
        private string GetClosureCode()
        {
            bool Closure1 = ActiveElement.LookupParameter("ПР1.ВКЛ").AsValueString() == "Да";
            bool Closure2 = ActiveElement.LookupParameter("ПР2.ВКЛ").AsValueString() == "Да";
            string window1 = "";

            if (Closure1)
            {
                string w1 = Marks.GetDoubleValueAsDecimeterString(ActiveElement, "ПР1.Отступ");
                string w2 = Marks.GetDoubleValueAsDecimeterString(ActiveElement, "ПР1.Ширина");
                string w3 = Marks.GetDoubleValueAsDecimeterString(ActiveElement, "ПР1.Высота");
                string w4 = Marks.GetDoubleValueAsDecimeterString(ActiveElement, "ПР1.ВысотаСмещение");
                window1 = $"{w2}.{w3}.{w4}.{w1}";
            }
            string window2 = "";
            if (Closure2)
            {
                string w1 = Marks.GetDoubleValueAsDecimeterString(ActiveElement, "ПР2.Отступ");
                string w2 = Marks.GetDoubleValueAsDecimeterString(ActiveElement, "ПР2.Ширина");
                string w3 = Marks.GetDoubleValueAsDecimeterString(ActiveElement, "ПР2.Высота");
                string w4 = Marks.GetDoubleValueAsDecimeterString(ActiveElement, "ПР2.ВысотаСмещение");
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
        #endregion

    }
}
