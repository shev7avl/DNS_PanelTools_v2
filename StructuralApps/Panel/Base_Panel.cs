using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace DNS_PanelTools_v2.StructuralApps.Panel
{
    public abstract class Base_Panel
    {
        public abstract Document ActiveDocument { get; set; }
        public abstract Element ActiveElement { get; set; }

        public virtual List<XYZ> IntersectedWindows { get; set; } = null;

        public abstract string LongMark { get; set; }

        public abstract string ShortMark { get; set; }

        /// <summary>
        /// Проверяет равенство двух панелей по значениям "Марка" и прочим логическим условиям
        /// </summary>
        /// <param name="panelMark">Панель для сравнения</param>
        /// <returns></returns>
        public virtual bool Equal(Base_Panel panelMark)
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
        public virtual void OverrideShortMark(string newMark)
        {
            ShortMark = newMark;
            Guid ADSK_panelMark = new Guid("92ae0425-031b-40a9-8904-023f7389963b");
            Transaction transaction = new Transaction(ActiveDocument, $"Назначение индекса: {newMark}");
            transaction.Start();
            ActiveElement.get_Parameter(ADSK_panelMark).Set(ShortMark);
            transaction.Commit();

        }


    }
}
