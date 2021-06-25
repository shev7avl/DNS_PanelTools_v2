using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace DNS_PanelTools_v2.StructuralApps.Mark
{
    public interface IPanelMark
    {
        Document ActiveDocument { get; set; }
        Element ActiveElement { get; set; }

        string LongMark { get; set; }

        string ShortMark { get; set; }

        bool FrontPVL { get; set; }

        void SetFrontPVL();

        bool Equal(IPanelMark panelMark);

        /// <summary>
        /// Заполняет значения короткой и длинной марки
        /// </summary>
        void FillMarks();

        /// <summary>
        /// Метод для переназначения значения короткой марки после присвоения индекса
        /// </summary>
        void OverrideShortMark(string newMark);

        /// <summary>
        /// Передает заполненные поля в семейство
        /// </summary>
        void SetMarks();

    }
}
