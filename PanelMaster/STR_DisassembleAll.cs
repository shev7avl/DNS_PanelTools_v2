using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using DSKPrim.PanelTools.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace DSKPrim.PanelTools.PanelMaster
{
    [Transaction(mode: TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]

    

    class STR_DisassembleAll : IExternalCommand
    {
        public Document Document { get; set; }

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            commandData.Application.Application.FailuresProcessing += new EventHandler<Autodesk.Revit.DB.Events.FailuresProcessingEventArgs>(FailuresProcessing);

            Document = commandData.Application.ActiveUIDocument.Document;

            if (TransactionSettings.WorksetsUnavailable(Document))
            {
                message = "Рабочие наборы недоступны. Освободите ВСЕ рабочие наборы";
                return Result.Failed;
            }

            Utility.Assemblies.DisassembleAll(Document);

            return Result.Succeeded;
        }

        private void FailuresProcessing(object sender, Autodesk.Revit.DB.Events.FailuresProcessingEventArgs e)
        {
            FailuresAccessor failuresAccessor = e.GetFailuresAccessor();
            //failuresAccessor
            String transactionName = failuresAccessor.GetTransactionName();

            IList<FailureMessageAccessor> fmas = failuresAccessor.GetFailureMessages();
            if (fmas.Count == 0)
            {
                e.SetProcessingResult(FailureProcessingResult.Continue);
                return;
            }

            if (transactionName.Equals("Разбираем сборки"))
            {
                foreach (FailureMessageAccessor fma in fmas)
                {
                    failuresAccessor.ResolveFailure(fma);
                }

                e.SetProcessingResult(FailureProcessingResult.ProceedWithCommit);
                return;
            }

            e.SetProcessingResult(FailureProcessingResult.Continue);
        }
    }


    

}
