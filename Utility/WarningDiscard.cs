using Autodesk.Revit.DB;
using System.Collections.Generic;

namespace DSKPrim.PanelTools.Utility
{
    

    public static class TransactionSettings
    {
        public static void SetFailuresPreprocessor(Transaction transaction)
        {
            IFailuresPreprocessor preprocessor = new WarningDiscard();
            FailureHandlingOptions fho = transaction.GetFailureHandlingOptions();
            fho.SetFailuresPreprocessor(preprocessor);
            transaction.SetFailureHandlingOptions(fho);
        }

        private class WarningDiscard : IFailuresPreprocessor
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
