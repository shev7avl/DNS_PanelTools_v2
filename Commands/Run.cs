using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using System.Threading;

namespace DSKPrim.PanelTools_v2.Commands
{


    [Transaction(mode: TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    class Run : IExternalCommand
    {
        private Routine RoutineBehaviour;

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Logger.Logger logger = Logger.Logger.getInstance();
            
            SetRoutineBehaviour();
            logger.WriteLog($"Выбрана задача {RoutineBehaviour}");
            logger.WriteLog("Старт задачи");
            //for working instance
            //try
            //{
            //    RoutineBehaviour.ExecuteRoutine(commandData);
            //    return Result.Succeeded;
            //}
            //catch (Exception e)
            //{
            //    logger.LogError(e);
            //    return Result.Failed;
            //}

            //for debug
            RoutineBehaviour.ExecuteRoutine(commandData);
            return Result.Succeeded;
        }

        private void SetRoutineBehaviour()
        {
            GUI.PanelWizard landing = new GUI.PanelWizard();
            if (landing.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                RoutineBehaviour = landing.GetRoutine();
            }
            //Some code here that sets behaviour

        }
    }


}
