﻿using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSKPrim.PanelTools_v2.Utility
{
    public class WarningDiscard : IFailuresPreprocessor
    {
        FailureProcessingResult
          IFailuresPreprocessor.PreprocessFailures(FailuresAccessor failuresAccessor)
        {
            String transactionName = failuresAccessor.GetTransactionName();

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
            }

            if (isResolved)
            {
                return FailureProcessingResult.ProceedWithCommit;
            }

            return FailureProcessingResult.Continue;
        }
    }
}