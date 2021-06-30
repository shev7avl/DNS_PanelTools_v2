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
    public class PS_Panel : IPanel
    {
        public override Document ActiveDocument { get; set; }
        public override Element ActiveElement { get; set; }

        public override string LongMark { get; set; }

        public override string ShortMark { get; set; }


        public override void OverrideShortMark(string newMark)
        {
            ShortMark = newMark;
            Guid ADSK_panelMark = new Guid("92ae0425-031b-40a9-8904-023f7389963b");
            Transaction transaction = new Transaction(ActiveDocument, $"Назначение индекса: {newMark}");
            transaction.Start();
            ActiveElement.get_Parameter(ADSK_panelMark).Set(ShortMark);
            transaction.Commit();

        }

        public PS_Panel(Document document, Element element)
        {
            ActiveDocument = document;
            ActiveElement = element;
        }
        public override void CreateMarks()
        {

            LongMark = $"ПС {GetPanelCode()}";
            ShortMark = $"ПС {LongMark.Split('_')[1]}";
            SetMarks();
        }

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
            string i1 = ActiveElement.LookupParameter("СТАРТ").AsValueString();
            string i2 = ActiveElement.LookupParameter("ФИНИШ").AsValueString();
            string i3 = Marks.GetDoubleValueAsDecimeterString(ActiveElement, "ГабаритДлина");
            string i4 = Marks.GetDoubleValueAsDecimeterString(familySymbol, "ГабаритВысота");
            string i5 = Marks.GetDoubleValueAsDecimeterString(familySymbol, "ГабаритТолщина");
            
            string i6 = Marks.GetDoubleValueAsDecimeterString(ActiveElement, "WELDA_Отступ");
            string i7 = Marks.GetDoubleValueAsDecimeterString(ActiveElement, "WELDA_Шаг");
            string i8 = ActiveElement.LookupParameter("WELDA_Количество").AsValueString();

            string i9 = Marks.GetDoubleValueAsDecimeterString(ActiveElement, "TR 24_Сверху_Отступ");
            string i10 = Marks.GetDoubleValueAsDecimeterString(ActiveElement, "TR 24_Сверху_Шаг");
            string i11 = ActiveElement.LookupParameter("TR 24_Сверху_Количество").AsValueString();

           
            return $"{i1}.{i2}_{i3}.{i4}.{i5}_{i6}.{i7}.{i8}_{i9}.{i10}.{i11}";
        }
        
    }
}
