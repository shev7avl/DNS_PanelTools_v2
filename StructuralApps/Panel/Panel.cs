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

        public virtual void ReadMarks()
        {
            Guid DNS_panelMark1 = new Guid(Properties.Resource.DNS_Полная_марка_изделия);

            Guid ADSK_panelMark1 = new Guid(Properties.Resource.ADSK_Марка_изделия);

            Guid ADSK_panelNum = new Guid(Properties.Resource.ADSK_Номер_изделия);

            LongMark = ActiveElement.get_Parameter(DNS_panelMark1).ToString();
            ShortMark = ActiveElement.get_Parameter(ADSK_panelMark1).ToString();
            Index = ActiveElement.get_Parameter(ADSK_panelNum).ToString();

        }

        public virtual void SetMarks()
        {

            Guid DNS_panelMark1 = new Guid(Properties.Resource.DNS_Полная_марка_изделия);

            Guid ADSK_panelMark1 = new Guid(Properties.Resource.ADSK_Марка_изделия);

            Guid ADSK_panelNum = new Guid(Properties.Resource.ADSK_Номер_изделия);

            Transaction transaction = new Transaction(ActiveDocument);

            transaction.Start($"Транзакция - {ActiveElement.Name}");


                ActiveElement.get_Parameter(DNS_panelMark1).Set(LongMark);

                ActiveElement.get_Parameter(ADSK_panelMark1).Set(ShortMark);



            transaction.Commit();
        }

    }
}
