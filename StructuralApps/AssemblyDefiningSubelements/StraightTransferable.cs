using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace DSKPrim.PanelTools_v2.StructuralApps.AssemblyDefiningSubelements
{
    public class StraightTransferable : DefiningBase, ITransferable
    {
        public override Document Document { get; set; }
        public override Element Element { get; set; }
        public override string Name { get; set; }

        public void Create(Element element)
        {
            throw new NotImplementedException();

        }

        public bool IsTransferable(Element element)
        {
            BoundingBoxXYZ bbox = element.get_Geometry(new Options()).GetBoundingBox();
            Level currentLevel = (Level)Document.GetElement(element.LevelId);
            double levelZ = currentLevel.Elevation;
            double elemenMinZ = bbox.Min.Z;

            if (element.Name == "F_1" && elemenMinZ < levelZ) return true;
            else return false;
        }
    }
}
