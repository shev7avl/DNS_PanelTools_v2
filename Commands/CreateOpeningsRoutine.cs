using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using System.Diagnostics;
using DSKPrim.PanelTools_v2.StructuralApps.Panel;
using DSKPrim.PanelTools_v2.StructuralApps;

namespace DSKPrim.PanelTools_v2.Commands
{
    [Transaction(mode: TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    class CreateOpeningsRoutine
        : Routine
    {
        public override Document Document { get; set; }

        Document LinkedArch;

        public override StructuralApps.Panel.Panel Behaviour { get; set; }

        public override void ExecuteRoutine(ExternalCommandData commandData)
        {
            Document = commandData.Application.ActiveUIDocument.Document;

            Logger.Logger logger = Logger.Logger.getInstance();
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();


            logger.WriteLog($"Активный документ: {Document.PathName}");
            logger.Separate();
            logger.WriteLog("Начали создание проемов в документе");
            logger.Separate();

            IEnumerable <Element> fecLinksARCH = new FilteredElementCollector(Document).OfCategory(BuiltInCategory.OST_RvtLinks).WhereElementIsNotElementType().Where(doc => doc.Name.Contains("_АР"));

            logger.WriteLog($"Найдено связей, содержащих \"_АР\" в названии: {fecLinksARCH.Count()}");

            if (fecLinksARCH.Count() == 0)
            {
                logger.WriteLog("ОШИБКА: Не загружены корректные связи АР");
                throw new NullReferenceException("Не вижу связей");
            }

            logger.WriteLog("Процедура начата");
            logger.WriteLog("Создаём синглтон категории \"Каркас несущий\"");
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            
            SingleStructDoc structDoc = SingleStructDoc.getInstance(Document);

            

            logger.WriteLog($"Проанализировано панелей: {structDoc.PanelMarks.Count}");

            logger.DebugLog($"{structDoc.PanelMarks.Count} panels analyzed");

            FilteredElementCollector fecStruct = new FilteredElementCollector(Document).OfCategory(BuiltInCategory.OST_StructuralFraming).WhereElementIsNotElementType();
            FilteredElementCollector fecWalls = new FilteredElementCollector(Document).OfCategory(BuiltInCategory.OST_Walls).WhereElementIsNotElementType();

            List<Element> els;
            if (fecStruct.GetElementCount() == 0)
            {
                els = fecWalls.Cast<Element>().ToList();
            }
            else
            {
                els = fecStruct.Cast<Element>().ToList();
            }

            foreach (RevitLinkInstance link in fecLinksARCH.Cast<RevitLinkInstance>().ToList())
            {
                RevitLinkInstance revitLink = link;
                LinkedArch = revitLink.GetLinkDocument();

                SingleArchDoc archDoc = SingleArchDoc.getInstance(LinkedArch);

                List<IPerforable> intersected = new List<IPerforable>();

                int counter = els.Count;
                for (int i = 0; i < counter; i++)
                {
                    Element item = els[i];
                    SetPanelBehaviour(item);
                    if (Behaviour is IPerforable perforable)
                    {
                        IPerforable panel = perforable;
                        panel.GetOpeningsFromLink(LinkedArch, revitLink, out List<Element> IntersectedWindows);
                        if (IntersectedWindows.Count>0)
                        {
                            intersected.Add(panel);
                        }
                    }
                }

                foreach (IPerforable item in intersected)
                {
                    item.GetOpeningsFromLink(LinkedArch, revitLink, out List<Element> IntersectedWindows);
                    item.Perforate(IntersectedWindows, revitLink);
                }

                archDoc.Dispose();
                structDoc.Dispose();
            }

            logger.LogSuccessTime(stopWatch);
        }
        
    }
}
