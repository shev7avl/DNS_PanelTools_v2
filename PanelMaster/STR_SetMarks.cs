using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using DSKPrim.PanelTools.ProjectEnvironment;
using System.Diagnostics;
using Autodesk.Revit.Attributes;
using DSKPrim.PanelTools.Panel;
using DSKPrim.PanelTools.Utility;

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

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            StructuralEnvironment structDoc = StructuralEnvironment.GetInstance(Document);

            TransactionGroup transactionGroup = new TransactionGroup(Document, "Присвоение длинной марки - ");
            transactionGroup.Start("Группа транзакций");
            FilteredElementCollector fec = new FilteredElementCollector(Document).OfCategory(BuiltInCategory.OST_StructuralFraming).WhereElementIsNotElementType();
            List<Element> els = fec.ToElements().Cast<Element>().ToList();

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

                Routine.GetPanelBehaviour(Document, item, out panel) ;
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
