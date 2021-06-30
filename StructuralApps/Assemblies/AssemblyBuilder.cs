using DNS_PanelTools_v2.StructuralApps.Panel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using System.Diagnostics;

namespace DNS_PanelTools_v2.StructuralApps.Assemblies
{
    class AssemblyBuilder
    {
        Dictionary<int, IPanel> IndexMarkPairs;

        List<IPanel> MarksList;

        List<XYZ> frontPVLPts;

        Element ActiveElement;

        Document ActiveDoc;

        public AssemblyBuilder(Document document)
        {
            ActiveDoc = document;
            //ActiveElement = element;

            SingleStructDoc marksList = SingleStructDoc.getInstance(ActiveDoc);
            MarksList = marksList.GetPanelMarks();
            frontPVLPts = marksList.getPVLpts();
        }

        public void FillMxIdDict(string panelSubString)
        {

            IndexMarkPairs = new Dictionary<int, IPanel>();
            Debug.WriteLine("Словарь Марка - индекс");
            Debug.WriteLine("------Начало словаря------");

            AddDictEntry(panelSubString);

            //Перезаписываем марки панелей в интерфейсе
            SingleStructDoc marksList = SingleStructDoc.getInstance(ActiveDoc);
            marksList.Dispose();
            marksList = SingleStructDoc.getInstance(ActiveDoc);
            MarksList = marksList.GetPanelMarks();

            Debug.WriteLine("------Конец словаря------");
        }

        public void CreateAssembly()
        {
            TransactionGroup transactionGroup = new TransactionGroup(ActiveDoc, "Создание сборок");
            transactionGroup.Start();
            foreach (var item in MarksList)
            {
                IList<Subelement> subElements = item.ActiveElement.GetSubelements();
                ICollection<ElementId> elementIds = new List<ElementId>();
                
                if (subElements.Count > 0)
                {
                    foreach (var thing in subElements)
                    {
                        elementIds.Add(thing.Element.Id);
                    }
                }
                elementIds.Add(item.ActiveElement.Id);

                using (Transaction transaction = new Transaction(ActiveDoc, $"Создание сборки: {item.ShortMark}"))
                {
                    transaction.Start();
                    AssemblyInstance assembly = AssemblyInstance.Create(ActiveDoc, elementIds, item.ActiveElement.Category.Id);
                    //создание видов
                    View3D view = AssemblyViewUtils.Create3DOrthographic(ActiveDoc, assembly.Id);
                    ViewSheet sheet1 = AssemblyViewUtils.CreateSheet(ActiveDoc, assembly.Id, ElementId.InvalidElementId);
                    XYZ origin = new XYZ();
                    //размещение видов
                    Viewport.Create(ActiveDoc, sheet1.Id, view.Id, origin);
                    transaction.Commit();
                }
            }
            transactionGroup.Assimilate();
        }

        private bool PVLComingClause(Element element)
        {
            bool result = false;
            Options options = new Options();
            BoundingBoxXYZ elBB = element.get_Geometry(options).GetBoundingBox();

            foreach (var item in frontPVLPts)
            {
                if (IsPointInsideBbox(elBB, item))
                {
                    result = true;
                    break;
                }
            }
            return result;
        }

        private bool IsPointInsideBbox(BoundingBoxXYZ boundingBox, XYZ point)
        {
            double maxX = boundingBox.Max.X;
            double maxY = boundingBox.Max.Y;
            double maxZ = boundingBox.Max.Z;

            double minX = boundingBox.Min.X;
            double minY = boundingBox.Min.Y;
            double minZ = boundingBox.Min.Z;

            bool XCheck = (point.X >= minX && point.X <= maxX);
            bool YCheck = (point.Y >= minY && point.Y <= maxY);
            bool ZCheck = (point.Z >= minZ && point.Z <= maxZ);

            return XCheck && YCheck && ZCheck;

        }

        private void AddDictEntry(string panelSubString)
        {
            int counter = 1;
            foreach (var mark in MarksList)
            {
                if (mark.LongMark.Contains(panelSubString))
                {
                    if (PanelExists(mark))
                    {
                        int index = 0;
                        foreach (var key in IndexMarkPairs.Keys)
                        {
                            if (IndexMarkPairs[key].Equal(mark))
                            {
                                index = key;
                            }
                        }
                        mark.OverrideShortMark($"{mark.ShortMark} - {index}");
                    }
                    if (!PanelExists(mark))
                    {
                        Debug.WriteLine($"{mark.ShortMark} - {counter}");
                        IndexMarkPairs.Add(counter, mark);
                        mark.OverrideShortMark($"{mark.ShortMark} - {counter}");
                        counter++;
                    }
                }
            }
        }

        private bool PanelExists(IPanel item)
        {
            bool exists = false;
            foreach (int key in IndexMarkPairs.Keys)
            {
                if (item.Equal(IndexMarkPairs[key]))
                {
                    exists = true;
                    break;
                }
                else
                {
                    exists = false;
                }
            }
            return exists;
        }

    }
}
