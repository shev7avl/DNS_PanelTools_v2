using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using System.Diagnostics;
using DSKPrim.PanelTools.ProjectEnvironment;
using DSKPrim.PanelTools.Panel;

namespace DSKPrim.PanelTools.PanelMaster
{
    [Transaction(mode: TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    class STR_CreateOpenings
        : IExternalCommand
    {
        public Document Document { get; set; }

        Document LinkedArch;

        public BasePanel Behaviour { get; set; }


        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document = commandData.Application.ActiveUIDocument.Document;

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();


            Debug.WriteLine($"Активный документ: {Document.PathName}");

            Debug.WriteLine("Начали создание проемов в документе");

            IEnumerable<Element> fecLinksARCH = new FilteredElementCollector(Document).OfCategory(BuiltInCategory.OST_RvtLinks).WhereElementIsNotElementType().Where(doc => doc.Name.Contains("_АР"));

            Debug.WriteLine($"Найдено связей, содержащих \"_АР\" в названии: {fecLinksARCH.Count()}");

            if (fecLinksARCH.Count() == 0)
            {
                Debug.WriteLine("ОШИБКА: Не загружены корректные связи АР");
                throw new NullReferenceException("Не вижу связей");
            }

            Debug.WriteLine("Процедура начата");
            Debug.WriteLine("Создаём синглтон категории \"Каркас несущий\"");
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            StructuralEnvironment structDoc = StructuralEnvironment.GetInstance(Document);



            Debug.WriteLine($"Проанализировано панелей: {structDoc.PanelMarks.Count}");

            Debug.WriteLine($"{structDoc.PanelMarks.Count} panels analyzed");

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

                SingleArchDoc archDoc = SingleArchDoc.GetInstance(LinkedArch);

                List<IPerforable> intersected = new List<IPerforable>();

                int counter = els.Count;
                for (int i = 0; i < counter; i++)
                {
                    Element item = els[i];
                    BasePanel temp;
                    Routine.GetPanelBehaviour(Document, item, out temp);
                    Behaviour = temp;
                    if (Behaviour is IPerforable perforable)
                    {
                        IPerforable panel = perforable;
                        panel.GetOpeningsFromLink(LinkedArch, revitLink, out List<Element> IntersectedWindows);
                        if (IntersectedWindows.Count > 0)
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
                structDoc.Reset();
            }

            Debug.WriteLine(stopWatch);
            return Result.Succeeded;
        }
    }
}
