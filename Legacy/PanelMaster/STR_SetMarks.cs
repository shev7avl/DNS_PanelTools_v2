using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using DSKPrim.PanelTools.Legacy.Controllers;
using DSKPrim.PanelTools.Panel;
using Autofac;
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

            TransactionGroup transactionGroup = new TransactionGroup(Document, "Присвоение марок");
            transactionGroup.Start();

            Selector selector = new Selector();
            ICollection<Element> els = selector.CollectElements(commandData,
                new PanelSelectionFilter(),
                BuiltInCategory.OST_StructuralFraming);
            
            

            int positionEnum = 1;
            foreach (var item in els)
            {
                //string posEnum;

                //if (positionEnum < 10)
                //{
                //    posEnum = $"0{positionEnum}";
                //}
                //else posEnum = positionEnum.ToString();

                PrecastPanel panel = new PrecastPanel(item);
                MarkController markController = new MarkController(panel);
                markController.Create();
                markController.Write();

                positionEnum++;
            }
            transactionGroup.Assimilate();
            transactionGroup.Dispose();

            return Result.Succeeded;
        }

    }
}
