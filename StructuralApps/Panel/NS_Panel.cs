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
    class NS_Panel : Panel, IPerforable
    {
        #region Fields
        public override Document ActiveDocument { get; set; }
        public override Element ActiveElement { get; set; }
        public override List<XYZ> IntersectedWindows { get; set; }

        public override string LongMark { get; set; }

        public override string ShortMark { get; set; }

        private bool FrontPVL { get; set; }

        private List<Element> frontPVLs { get; set; }
        #endregion

        #region Constructor
        public NS_Panel(Document document, Element element)
        {
            ActiveDocument = document;
            ActiveElement = element;
        }

        #endregion

        #region Public Methods

        #region IPerforable
        void IPerforable.Perforate(List<Element> IntersectedWindows)
        {

            TransactionGroup transaction = new TransactionGroup(ActiveDocument, $"Создание проемов - {ActiveElement.Name}");
            transaction.Start();
            if (IntersectedWindows.Count == 1)
            {
                Element window = IntersectedWindows[0];
                Utility.Openings.SetOpeningParams(ActiveDocument, ActiveElement, window);
            }
            else if (IntersectedWindows.Count == 2)
            {
                Element window1 = IntersectedWindows[0];
                Element window2 = IntersectedWindows[1];
                Utility.Openings.SetOpeningParams(ActiveDocument, ActiveElement, window1, window2);
            }
            transaction.Assimilate();
        }

        void IPerforable.GetOpenings(Document linkedArch, out List<Element> IntersectedWindows)
        {
            IntersectedWindows = Geometry.IntersectedOpenings(ActiveElement, linkedArch, windows: true);
        }
        #endregion

        #region Base_Panel

        public override void CreateMarks()
        {
            LongMark = $"НС {GetPanelCode()}_{GetClosureCode()}";
            ShortMark = $"НС {LongMark.Split('_')[1]}";
            SetMarks();
        }


        public override bool Equal(Panel panelMark)
        {
            NS_Panel panel = (NS_Panel)panelMark;
            panel.SetFrontPVL();
            if (LongMark == panel.LongMark && FrontPVL == panel.GetFrontPVL())
            {
                return true;
            }

            else return false;
        }

        #endregion

        private void SetFrontPVL()
        {
            SingleStructDoc singletonMarks = SingleStructDoc.getInstance(ActiveDocument);
            frontPVLs = new List<Element>();
            Options options = new Options();
            BoundingBoxXYZ elBB = ActiveElement.get_Geometry(options).GetBoundingBox();
            foreach (var item in singletonMarks.getPVLpts())
            {
                LocationPoint locationPoint = (LocationPoint) item.Location;
                XYZ xYZ = locationPoint.Point;
                if (Geometry.InBox(elBB, xYZ))
                {
                    frontPVLs.Add(item);
                }
            }
            
            FrontPVL = PVLComingClause(ActiveElement);
        }
        public bool GetFrontPVL()
        {
            return FrontPVL;
        }

        public List<Element> GetPVLList()
        {
            return frontPVLs;
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
            string i1 = ActiveElement.LookupParameter("СТАРТ").AsValueString();
            string i2 = ActiveElement.LookupParameter("ФИНИШ").AsValueString();
            string i3 = Marks.AsDecimString(ActiveElement, "ГабаритДлина");
            string i4 = Marks.AsDecimString(familySymbol, "ГабаритВысота");
            string i5 = Marks.AsDecimString(familySymbol, "ГабаритТолщина");
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

        private bool PVLComingClause(Element element)
        {
            bool result = false;
            Options options = new Options();
            BoundingBoxXYZ elBB = element.get_Geometry(options).GetBoundingBox();

            foreach (var item in frontPVLs)
            {
                LocationPoint location = (LocationPoint)item.Location;
                XYZ point = location.Point;
                if (Geometry.InBox(elBB, point))
                {
                    result = true;
                    break;
                }
            }
            return result;
        }

        #endregion

    }
}
