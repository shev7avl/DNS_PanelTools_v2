using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using DNS_PanelTools_v2.Utility;

namespace DNS_PanelTools_v2.StructuralApps.Panel
{
    public class PP_Panel : Base_Panel
    {
        public override Document ActiveDocument { get; set; }
        public override Element ActiveElement { get; set; }

        public override string LongMark { get; set; }

        public override string ShortMark { get; set; }


        public PP_Panel(Document document, Element element)
        {
            ActiveDocument = document;
            ActiveElement = element;
        }
        public override void CreateMarks()
        {
            LongMark = $"ПП {GetPanelCode()}{GetClosureCode()}";
            ShortMark = LongMark.Split('_')[0];
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
            string i3 = Marks.AsDecimString(ActiveElement, "ADSK_Размер_Длина");
            string i4 = Marks.AsDecimString(ActiveElement, "ADSK_Размер_Ширина");
            string i5 = ActiveElement.LookupParameter("КодНагрузки").AsValueString();
            return $"{i3}.{i4}-{i5}";
        }
        private string GetClosureCode()
        {
            var elementFamily = ActiveElement as FamilyInstance;
            var familySymbol = elementFamily.Symbol;

            bool closureBool = familySymbol.LookupParameter("Вырезы").AsValueString() == "Да";

            string closureCode = "";

            if (closureBool)
            {
                string w1 = Marks.AsDecimString(ActiveElement, "Вырезы_Отступ_Начало");
                string w2 = Marks.AsDecimString(familySymbol, "Вырезы_Шаг");
                string w3 = ActiveElement.LookupParameter("Отверстия_Количество").AsValueString();
                string w4 = Marks.AsDecimString(ActiveElement, "Вырезы_Отступ_Конец");
                closureCode = $"_{w1}.{w2}.{w3}.{w4}";
            }

            return closureCode;
        }

    }
}
