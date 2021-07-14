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
    public class BP_Panel : Base_Panel
    {
        #region Fields
        public override Document ActiveDocument { get; set; }

        public override Element ActiveElement { get; set; }

        public override string LongMark { get; set; }

        public override string ShortMark { get; set; }

        #endregion

        #region Constructor
        public BP_Panel(Document document, Element element)
        {
            ActiveDocument = document;
            ActiveElement = element;

        }
        #endregion

        #region Public Methods

        public override void CreateMarks()
        {
            LongMark = $"БП {GetPanelCode()}";
            ShortMark = LongMark.Split('_')[0];
            SetMarks();
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
            string i3 = Marks.AsDecimString(familySymbol, "Плита_Длина");
            string i4 = Marks.AsDecimString(familySymbol, "Плита_Ширина");
            string i5 = Marks.AsDecimString(familySymbol, "Плита_Толщина");
            string i6 = Marks.AsDecimString(familySymbol, "Кронштейн_Отступ");
            string i7 = Marks.AsDecimString(familySymbol, "Кронштейн_Шаг");
            string i8 = familySymbol.LookupParameter("Кронштейн_Количество").AsValueString();
            string i9 = familySymbol.LookupParameter("Отверстия_ПривязкаСлева").AsValueString();
            string i10 = familySymbol.LookupParameter("Отверстия_ПривязкаСправа").AsValueString();
            string i11 = familySymbol.LookupParameter("Отверстия_ПривязкаСпереди").AsValueString();
            string i12 = familySymbol.LookupParameter("Отверстия_РасстояниеМеждуПоY").AsValueString();

            return $"{i3}.{i4}.{i5}_{i6}.{i7}.{i8}.1_{i9}.{i10}_{i11}.{i12}";
        }
        #endregion

    }  
}
