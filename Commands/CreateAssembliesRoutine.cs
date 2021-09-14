using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using DSKPrim.PanelTools_v2.StructuralApps.Assemblies;
using DSKPrim.PanelTools_v2.StructuralApps;
using DSKPrim.PanelTools_v2.StructuralApps.Panel;
using DSKPrim.PanelTools_v2.Utility;
using System.Diagnostics;

namespace DSKPrim.PanelTools_v2.Commands
{
    [Transaction(mode: TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    class CreateAssembliesRoutine
        : Routine
    {
        public override Document Document { get; set; }
        public override StructuralApps.Panel.Panel Behaviour { get; set; }

        public override void ExecuteRoutine(ExternalCommandData commandData)
        {
            Document = commandData.Application.ActiveUIDocument.Document;
            SingleStructDoc structDoc = SingleStructDoc.getInstance(Document);

            structDoc.PanelMarks.Sort(CompareElementIdsByZCoord);
            int counter = 1;
            foreach (var item in structDoc.PanelMarks)
            {
                
                Debug.WriteLine($"Итерация: {counter}//{structDoc.PanelMarks.Count}");
                Debug.WriteLine($"Панель: {item.ShortMark}");
                if (item is IAssembler)
                {
                    Outline outline = new Outline(item.ActiveElement.get_Geometry(new Options()).GetBoundingBox().Min, item.ActiveElement.get_Geometry(new Options()).GetBoundingBox().Max);
                    ElementFilter filter = new BoundingBoxIntersectsFilter(outline);
                    FilteredElementCollector fecIntersect = new FilteredElementCollector(Document).OfCategory(BuiltInCategory.OST_StructuralFraming).WhereElementIsNotElementType().WherePasses(filter);

                    List<Element> intersected = fecIntersect.ToList();


                    IAssembler assembler = (IAssembler)item;
                    assembler.SetAssemblyElements();
                    if (item is NS_Panel || item is VS_Panel)
                    {
                        foreach (var ints in intersected)
                        {
                            SetPanelBehaviour(ints);
                            if (Behaviour is NS_Panel || Behaviour is VS_Panel)
                            {
                                IAssembler assembler1 = (IAssembler)Behaviour;
                                assembler.TransferFromPanel(assembler1);
                            }

                        }
                    }
                    

                    Transaction transaction = new Transaction(Document, "CreateAssembly");
                    FailureHandlingOptions opts = transaction.GetFailureHandlingOptions();
                    IFailuresPreprocessor preprocessor = new WarningDiscard();
                    opts.SetFailuresPreprocessor(preprocessor);
                    transaction.SetFailureHandlingOptions(opts);

                    transaction.Start();
                    try
                    {
                        AssemblyInstance instance = AssemblyInstance.Create(Document, assembler.AssemblyElements, item.ActiveElement.Category.Id);
                        transaction.Commit();

                        transaction.Start();
                        try
                        {
                            instance.AssemblyTypeName = item.ShortMark;
                        }
                        catch (Autodesk.Revit.Exceptions.ArgumentException)
                        {
                            instance.AssemblyTypeName = $"{item.ShortMark} ID{item.ActiveElement.Id}";
                        }

                        transaction.Commit();

                    }


                    catch (Autodesk.Revit.Exceptions.ArgumentException)
                    {
                        Debug.WriteLine($"Произошла ошибка в панели {item.ShortMark} на уровне {item.ActiveElement.LevelId}");
                        AssemblyInstance instance = AssemblyInstance.Create(Document,new List<ElementId>() { item.ActiveElement.Id}, item.ActiveElement.Category.Id);
                        transaction.Commit();

                        transaction.Start();
                        try
                        {
                            instance.AssemblyTypeName = item.ShortMark;
                        }
                        catch (Autodesk.Revit.Exceptions.ArgumentException)
                        {
                            instance.AssemblyTypeName = $"{item.ShortMark} ID{item.ActiveElement.Id}";
                        }

                        transaction.Commit();
                    }
                        
                    
                }
                counter++;
            }

            //AssemblyBuilder assemblyBuilder = new AssemblyBuilder(Document);
            //assemblyBuilder.CreateAssemblies();

            structDoc.Dispose();
        }

        private int CompareElementIdsByZCoord(StructuralApps.Panel.Panel x, StructuralApps.Panel.Panel y)
        {
            Element elX = x.ActiveElement;
            Element elY = y.ActiveElement;

            BoundingBoxXYZ boxX = elX.get_Geometry(new Options()).GetBoundingBox();
            BoundingBoxXYZ boxY = elY.get_Geometry(new Options()).GetBoundingBox();

            if (boxX.Min.Z > boxY.Min.Z)
            {
                return 1;
            }
            else if (boxX.Min.Z == boxY.Min.Z)
            {
                return 0;
            }
            else
            {
                return -1;
            }

        }

    }
}
