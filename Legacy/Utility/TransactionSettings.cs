using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DSKPrim.PanelTools.Utility
{
    

    public static class TransactionSettings
    {

        public static void CheckWorksets(Document document)
        {
            if (document.IsWorkshared)
            {
                ICollection<WorksetId> worksets = new FilteredWorksetCollector(document).ToWorksetIds();

                ICollection<WorksetId> checkedWorksets = WorksharingUtils.CheckoutWorksets(document, worksets);

                if (checkedWorksets.Count() != worksets.Count())
                {
                    throw new Exception("Рабочие наборы заняты. Освободите рабочие наборы");
                }

            }
        }

        public static void ReleaseWorksets(Document document)
        {
            WorksharingUtils.RelinquishOwnership(document, new RelinquishOptions(true), new TransactWithCentralOptions());
        }

        internal static void SetFailuresPreprocessor(Transaction transaction)
        {
            IFailuresPreprocessor preprocessor = new WarningDiscard();
            FailureHandlingOptions fho = transaction.GetFailureHandlingOptions();
            fho.SetFailuresPreprocessor(preprocessor);
            transaction.SetFailureHandlingOptions(fho);
        }

        

        internal class WarningDiscard : IFailuresPreprocessor
        {
            FailureProcessingResult
              IFailuresPreprocessor.PreprocessFailures(FailuresAccessor failuresAccessor)
            {
                IList<FailureMessageAccessor> fmas = failuresAccessor.GetFailureMessages();

                if (fmas.Count == 0)
                {
                    return FailureProcessingResult.Continue;
                }

                bool isResolved = false;

                foreach (FailureMessageAccessor fma in fmas)
                {
                    if (fma.HasResolutions())
                    {
                        failuresAccessor.ResolveFailure(fma);
                        isResolved = true;
                    }
                    
                    if (fma.GetSeverity() == FailureSeverity.Warning)
                    {
                        
                        failuresAccessor.DeleteWarning(fma);
                        isResolved = true;
                    }
                }
                
                if (isResolved)
                {
                    return FailureProcessingResult.ProceedWithCommit;
                }

                return FailureProcessingResult.Continue;
            }


        }
    }
}
