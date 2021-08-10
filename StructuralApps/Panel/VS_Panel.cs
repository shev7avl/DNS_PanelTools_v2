using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using DSKPrim.PanelTools_v2.Utility;

namespace DSKPrim.PanelTools_v2.StructuralApps.Panel
{
    class VS_Panel : Panel, IPerforable
    {
        #region Fields
        public override Document ActiveDocument { get; set; }
        public override Element ActiveElement { get; set; }
        public override List<XYZ> IntersectedWindows { get; set; }
        public override string LongMark { get; set; }
        public override string ShortMark { get; set; }

        public override string Index { get; set; }

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

            Guid ADSK_panelNum = new Guid("a531f6df-1e58-48e0-8c14-77cf7c1809b8");
            if (ActiveElement.get_Parameter(ADSK_panelNum).AsString() == "")
            {
                Index = $"{ActiveElement.Id}-Id";
            }
            else
            {
                Index = ActiveElement.get_Parameter(ADSK_panelNum).AsString();
            }

            ShortMark = $"{LongMark.Split('_')[0]} - {Index}";


            SetMarks();

        }

        void IPerforable.Perforate(List<Element> IntersectedWindows, RevitLinkInstance revitLink)
        {
            TransactionGroup transaction = new TransactionGroup(ActiveDocument, $"Создание проемов - {ActiveElement.Name}");
            transaction.Start();
            if (IntersectedWindows.Count == 1)
            {
                Element window = IntersectedWindows[0];
                Utility.Openings.SetOpeningParams(ActiveDocument, revitLink, ActiveElement, window);
            }
            else if (IntersectedWindows.Count == 2)
            {
                Element window1 = IntersectedWindows[0];
                Element window2 = IntersectedWindows[1];
                Utility.Openings.SetOpeningParams(ActiveDocument, revitLink, ActiveElement, window1, window2);
            }
            transaction.Assimilate();
        }

        void IPerforable.GetOpeningsFromLink(Document linkedArch, RevitLinkInstance revitLink, out List<Element> IntersectedWindows)
        {
            IntersectedWindows = Geometry.IntersectedOpenings(ActiveElement, revitLink, linkedArch, windows: false);
        }

        #endregion

        #region Private Methods
        
        private string GetPanelCode()
        {
            var elementFamily = ActiveElement as FamilyInstance;
            var familySymbol = elementFamily.Symbol;
            string i3 = Marks.AsDecimString(ActiveElement, "ГабаритДлина");
            string i4 = Marks.AsDecimString(familySymbol, "ГабаритВысота");
            string i5 = Marks.AsDecimString(familySymbol, "ГабаритТолщина");
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
                string w1 = Marks.AsDecimString(ActiveElement, "ПР1.Отступ");
                string w2 = Marks.AsDecimString(ActiveElement, "ПР1.Ширина");
                string w3 = Marks.AsDecimString(ActiveElement, "ПР1.Высота");
                string w4 = Marks.AsDecimString(ActiveElement, "ПР1.ВысотаСмещение");
                window1 = $"{w2}.{w3}.{w4}.{w1}";
            }
            string window2 = "";
            if (Closure2)
            {
                string w1 = Marks.AsDecimString(ActiveElement, "ПР2.Отступ");
                string w2 = Marks.AsDecimString(ActiveElement, "ПР2.Ширина");
                string w3 = Marks.AsDecimString(ActiveElement, "ПР2.Высота");
                string w4 = Marks.AsDecimString(ActiveElement, "ПР2.ВысотаСмещение");
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
