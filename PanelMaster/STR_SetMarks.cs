using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using DSKPrim.PanelTools.Panel;
using DSKPrim.PanelTools.ProjectEnvironment;
using DSKPrim.PanelTools.Utility;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DSKPrim.PanelTools.PanelMaster
{
    [Transaction(mode: TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class STR_SetMarks : IExternalCommand
    {
        public Document Document { get; set; }

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document = commandData.Application.ActiveUIDocument.Document;

            if (TransactionSettings.WorksetsUnavailable(Document))
            {
                message = "Рабочие наборы недоступны. Освободите ВСЕ рабочие наборы";
                return Result.Failed;
            }

            StructuralEnvironment structDoc = StructuralEnvironment.GetInstance(Document);

            TransactionGroup transactionGroup = new TransactionGroup(Document, "Присвоение длинной марки - ");
            transactionGroup.Start("Группа транзакций");

            Selector selector = new Selector();
            ICollection<Element> els = selector.CollectElements(commandData, new PanelSelectionFilter(), BuiltInCategory.OST_StructuralFraming);
            
            int positionEnum = 1;
            foreach (var item in els)
            {
                string posEnum;

                if (positionEnum < 10)
                {
                    posEnum = $"0{positionEnum}";
                }
                else posEnum = positionEnum.ToString();

                BasePanel panel;

                Routine.GetPanelBehaviour(Document, item, out panel);
                if (panel != null)
                {
                    if (panel.LongMark is null ||
                        panel.ShortMark is null)
                    {
                        panel.CreateMarks();
                    }
                    else
                    {
                        panel.ReadMarks();
                    }

                    panel.SetMarks(posEnum);
                }
                positionEnum++;
            }
            transactionGroup.Assimilate();
            transactionGroup.Dispose();
            structDoc.Reset();

            return Result.Succeeded;
        }

    }
}
