using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace DSKPrim.PanelTools_v2.StructuralApps.Panel
{
    public abstract class Panel
    {
        public abstract Document ActiveDocument { get; set; }
        public abstract Element ActiveElement { get; set; }

        public virtual List<XYZ> IntersectedWindows { get; set; } = null;

        public abstract string LongMark { get; set; }

        public abstract string ShortMark { get; set; }

        public abstract string Index { get; set; }


        /// <summary>
        /// Проверяет равенство двух панелей по значениям "Марка" и прочим логическим условиям
        /// </summary>
        /// <param name="panelMark">Панель для сравнения</param>
        /// <returns></returns>
        public virtual bool Equal(Panel panelMark)
        {
            if (LongMark == panelMark.LongMark)
            {
                return true;
            }
            else return false;
        }

        /// <summary>
        /// Заполняет значения короткой и длинной марки
        /// </summary>
        public abstract void CreateMarks();

        /// <summary>
        /// Метод для переназначения значения короткой марки после присвоения индекса
        /// </summary>
        public virtual void SetIndex(int index, bool overwrite = false)
        {
            if (overwrite)
            {
                ShortMark = ShortMark.Split('-')[0];
            }
            ShortMark = $"{ShortMark}-{index}";
            Guid ADSK_panelMark = new Guid("92ae0425-031b-40a9-8904-023f7389963b");
            Transaction transaction = new Transaction(ActiveDocument, $"Назначение индекса: {ShortMark}");
            transaction.Start();
            ActiveElement.get_Parameter(ADSK_panelMark).Set(ShortMark);
            transaction.Commit();

        }

        protected virtual void SetMarks()
        {
            Guid DNS_panelMark = new Guid("db2bee76-ce6f-4203-9fde-b8f34f3477b5");
            Guid DNS_panelMark1 = new Guid("61078a81-82f3-41e2-bcfd-ae64f9430577");

            Guid ADSK_panelMark = new Guid("92ae0425-031b-40a9-8904-023f7389963b");
            Guid ADSK_panelMark1 = new Guid("5d369dfb-17a2-4ae2-a1a1-bdfc33ba7405");

            Guid ADSK_panelNum = new Guid("a531f6df-1e58-48e0-8c14-77cf7c1809b8");

            Transaction transaction = new Transaction(ActiveDocument);

            transaction.Start($"Транзакция - {ActiveElement.Name}");



            try
            {
                ActiveElement.get_Parameter(DNS_panelMark).Set(LongMark);
            }
            catch (NullReferenceException)
            {
                ActiveElement.get_Parameter(DNS_panelMark1).Set(LongMark);
            }

            try
            {
                ActiveElement.get_Parameter(ADSK_panelMark).Set(ShortMark);
            }
            catch (NullReferenceException)
            {
                ActiveElement.get_Parameter(ADSK_panelMark1).Set(ShortMark);
            }


            transaction.Commit();
        }

    }
}
