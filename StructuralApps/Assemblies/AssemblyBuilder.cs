using DNS_PanelTools_v2.StructuralApps.Mark;
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
        Dictionary<int, IPanelMark> IndexMarkPairs;

        List<IPanelMark> MarksList;

        List<XYZ> frontPVLPts;

        Element ActiveElement;

        Document ActiveDoc;

        public AssemblyBuilder(Document document)
        {
            ActiveDoc = document;
            //ActiveElement = element;

            SingletonMarksList marksList = SingletonMarksList.getInstance(ActiveDoc);
            MarksList = marksList.GetPanelMarks();
            frontPVLPts = marksList.getPVLpts();
        }

        public void FillMxIdDict(string panelSubString)
        {

            IndexMarkPairs = new Dictionary<int, IPanelMark>();
            Debug.WriteLine("Словарь Марка - индекс");
            Debug.WriteLine("------Начало словаря------");

            AddDictEntry(panelSubString);

            Debug.WriteLine("------Конец словаря------");
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
                    mark.SetFrontPVL();
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

        private bool PanelExists(IPanelMark item)
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
