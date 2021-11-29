using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using DSKPrim.PanelTools_v2.StructuralApps.Views;
using DSKPrim.PanelTools_v2.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DSKPrim.PanelTools_v2.Commands
{
    [Transaction(mode: TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    class STR_CreateDrawings : Autodesk.Revit.UI.IExternalCommand
    {
        
        public Document Document { get ; set; }
        //ИДЕЯ
        // Что если парсить json на предмет листов, видов и шаблонов?

        private class AssemblySelectionFilter : ISelectionFilter
        {
            public bool AllowElement(Element elem)
            {

                if (elem.GetType() == typeof(AssemblyInstance))
                {
                    return true;
                }

                else return false;
            }

            public bool AllowReference(Reference reference, XYZ position)
            {
                throw new NotImplementedException();
            }
        }

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document = commandData.Application.ActiveUIDocument.Document;

            Logger.Logger logger = Logger.Logger.getInstance();
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();


            Selection selection = commandData.Application.ActiveUIDocument.Selection;

            IList<Reference> list_Assemblies = selection.PickObjects(ObjectType.Element, new AssemblySelectionFilter(), "Выберите сборки в проекте");


            logger.DebugLog(Document.PathName);


            foreach (var reference in list_Assemblies)
            {
                Element element = Document.GetElement(reference.ElementId);
                AssemblyInstance assemblyInstance = (AssemblyInstance)element;
                try
                {
                    if (assemblyInstance.Name.Contains("НС"))
                    {
                        CreateNS(assemblyInstance);
                    }
                    else if (assemblyInstance.Name.Contains("ВС"))
                    {
                        CreateVS(assemblyInstance);
                    }
                    else if (assemblyInstance.Name.Contains("ПП"))
                    {
                        CreatePP(assemblyInstance);
                    }
                    else if (assemblyInstance.Name.Contains("БП"))
                    {
                        CreateBP(assemblyInstance);
                    }
                    else if (assemblyInstance.Name.Contains("ПС"))
                    {
                        CreatePS(assemblyInstance);
                    }

                    logger.DebugLog(assemblyInstance.Name);
                }
                catch (Exception e)
                {
                    logger.DebugLog($"Ошибка{e.Message} : {assemblyInstance.Name} - {assemblyInstance.Id}");
                }
            }

            logger.LogSuccessTime(stopWatch);

            return Result.Succeeded;
        }

        //TODO: Переделать на абстрактную фабрику
        private void CreateNS(AssemblyInstance item)
        {
            Transaction transaction = new Transaction(Document, $"Создание листов панели {item.Name}");
            FailureHandlingOptions opts = transaction.GetFailureHandlingOptions();
            opts.SetFailuresPreprocessor(new WarningDiscard());
            transaction.SetFailureHandlingOptions(opts);

            //Получаем нужные элементы, шаблоны видов
            ElementId elementId = item.Id;
            ViewsSingleton views = ViewsSingleton.getInstance(Document);

            //Создаём листы
            using (transaction)
            {
                
                List<ViewSheet> sheets = new List<ViewSheet>();
                
                transaction.Start();
                Utility.Sheets.CreateSheets(Document, elementId, 9, out sheets);

                //Создаем виды и размещаем на листах
                //1 лист
                Utility.Sheets.Create3DSheet(Document, elementId, sheets[0]);
                //2 лист
                Utility.Sheets.CreateCasingDrawing(Document, elementId, sheets[1]);
                //3 лист
                Utility.Sheets.CreateInsulationDrawing(Document, elementId, sheets[2]);
                //4 лист
                Utility.Sheets.CreateJointDrawing(Document, elementId, sheets[3]);
                //5 лист	
                Utility.Sheets.CreateRebarDrawing(Document, elementId, sheets[4], Internal: true);
                //6 лист
                Utility.Sheets.CreateRebarDrawing(Document, elementId, sheets[5], Internal: false);
                //7 лист
                Utility.Sheets.CreateScheduleDrawing(Document, elementId, sheets[6]);
                //8 лист
                Utility.Sheets.CreateMeshDrawing(Document, elementId, sheets[7], Internal: true);
                //9 лист
                Utility.Sheets.CreateMeshDrawing(Document, elementId, sheets[8], Internal: false);

                Document.Regenerate();
                transaction.Commit();

                transaction.Start();
                for (int i = 0; i < sheets.Count; i++)
                {
                    Utility.Sheets.SetSheetParameters(Document, elementId, sheets[i], i+1);
                }
                transaction.Commit();
            }
        }

        private void CreateVS(AssemblyInstance item)
        {
            Transaction transaction = new Transaction(Document, $"Создание листов панели {item.Name}");
            FailureHandlingOptions opts = transaction.GetFailureHandlingOptions();
            opts.SetFailuresPreprocessor(new WarningDiscard());
            transaction.SetFailureHandlingOptions(opts);

            //Создаём листы
            using (transaction)
            {
                List<ViewSheet> sheets = new List<ViewSheet>();

                transaction.Start();

                Utility.Sheets.CreateSheets(Document, item.Id, 5, out sheets);

                //Создаем виды и размещаем на листах
                //1 лист
                Utility.Sheets.Create3DSheet(Document, item.Id, sheets[0]);
                //2 лист
                Utility.Sheets.CreateCasingDrawing(Document, item.Id, sheets[1]);
                //3 лист	
                Utility.Sheets.CreateRebarDrawing(Document, item.Id, sheets[2]);
                //4 лист
                Utility.Sheets.CreateScheduleDrawing(Document, item.Id, sheets[3]);
                //5 лист
                Utility.Sheets.CreateMeshDrawing(Document, item.Id, sheets[4]);

                Document.Regenerate();
                transaction.Commit();

                transaction.Start();
                for (int i = 0; i < sheets.Count; i++)
                {
                    Utility.Sheets.SetSheetParameters(Document, item.Id, sheets[i], i + 1);
                }
                transaction.Commit();
            }
        }

        private void CreatePP(AssemblyInstance item)
        {
            Transaction transaction = new Transaction(Document, $"Создание листов панели {item.Name}");
            FailureHandlingOptions opts = transaction.GetFailureHandlingOptions();
            opts.SetFailuresPreprocessor(new WarningDiscard());
            transaction.SetFailureHandlingOptions(opts);
            //Получаем нужные элементы, шаблоны видов

            //Создаём листы
            using (transaction)
            {
                List<ViewSheet> sheets = new List<ViewSheet>();

                transaction.Start();

                Utility.Sheets.CreateSheets(Document, item.Id, 1, out sheets);

                //Создаем виды и размещаем на листах
                //1 лист
                Utility.Sheets.Create3DSheetPP(Document, item.Id, sheets[0]);

                Document.Regenerate();
                transaction.Commit();

                transaction.Start();
                for (int i = 0; i < sheets.Count; i++)
                {
                    Utility.Sheets.SetSheetParameters(Document, item.Id, sheets[i], i + 1);
                }
                transaction.Commit();
            }
        }

        private void CreateBP(AssemblyInstance item)
        {

            Transaction transaction = new Transaction(Document, $"Создание листов панели {item.Name}");
            FailureHandlingOptions opts = transaction.GetFailureHandlingOptions();
            opts.SetFailuresPreprocessor(new WarningDiscard());
            transaction.SetFailureHandlingOptions(opts);

            //Создаём листы
            using (transaction)
            {
                List<ViewSheet> sheets = new List<ViewSheet>();

                transaction.Start();

                Utility.Sheets.CreateSheets(Document, item.Id, 4, out sheets);

                Utility.Sheets.Create3DSheet(Document, item.Id, sheets[0]);

                Utility.Sheets.CreateCasingDrawing(Document, item.Id, sheets[1], true);

                Utility.Sheets.CreateRebarDrawing(Document, item.Id, sheets[2]);

                Utility.Sheets.CreateScheduleDrawing(Document, item.Id, sheets[3]);

                Document.Regenerate();
                transaction.Commit();

                transaction.Start();
                for (int i = 0; i < sheets.Count; i++)
                {
                    Utility.Sheets.SetSheetParameters(Document, item.Id, sheets[i], i + 1);
                }
                transaction.Commit();
            }
        }
        private void CreatePS(AssemblyInstance item)
        {

            Transaction transaction = new Transaction(Document, $"Создание листов панели {item.Name}");
            FailureHandlingOptions opts = transaction.GetFailureHandlingOptions();
            opts.SetFailuresPreprocessor(new WarningDiscard());
            transaction.SetFailureHandlingOptions(opts);

            //Создаём листы
            using (transaction)
            {
                List<ViewSheet> sheets = new List<ViewSheet>();

                transaction.Start();

                Utility.Sheets.CreateSheets(Document, item.Id, 5, out sheets);

                //Создаем виды и размещаем на листах
                //1 лист
                Utility.Sheets.Create3DSheet(Document, item.Id, sheets[0]);
                //2 лист
                Utility.Sheets.CreateCasingDrawing(Document, item.Id, sheets[1]);
                //3 лист	
                Utility.Sheets.CreateRebarDrawing(Document, item.Id, sheets[2]);
                //4 лист
                Utility.Sheets.CreateScheduleDrawing(Document, item.Id, sheets[3]);
                //5 лист
                Utility.Sheets.CreateMeshDrawing(Document, item.Id, sheets[4]);

                Document.Regenerate();
                transaction.Commit();

                transaction.Start();
                for (int i = 0; i < sheets.Count; i++)
                {
                    Utility.Sheets.SetSheetParameters(Document, item.Id, sheets[i], i + 1);
                }
                transaction.Commit();
            }


        }

        
    }
}
