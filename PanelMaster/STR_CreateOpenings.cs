using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using System.Diagnostics;
using DSKPrim.PanelTools.ProjectEnvironment;
using DSKPrim.PanelTools.Panel;
using DSKPrim.PanelTools.Utility;

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

            if (TransactionSettings.WorksetsUnavailable(Document))
            {
                message = "Рабочие наборы недоступны. Освободите ВСЕ рабочие наборы";
                return Result.Failed;
            }


            IEnumerable<Element> fecLinksARCH = new FilteredElementCollector(Document).OfCategory(BuiltInCategory.OST_RvtLinks).WhereElementIsNotElementType().Where(doc => doc.Name.Contains("_АР"));


            if (fecLinksARCH.Count() == 0)
            {
                message = "ОШИБКА: Не загружены корректные связи АР";
                return Result.Failed;
            }

            StructuralEnvironment structDoc = StructuralEnvironment.GetInstance(Document);

            Selector selector = new Selector();
            List<Element> els = selector.CollectElements(commandData, new PanelSelectionFilter(), BuiltInCategory.OST_StructuralFraming).ToList();

            try
            {
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
            }
            catch (Exception e)
            {
                message = $"Ошибка {e.Message}";
                return Result.Failed;
            }
            

            return Result.Succeeded;
        }
    }
}
