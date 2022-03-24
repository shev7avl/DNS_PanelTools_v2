using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;

namespace DSKPrim.PanelTools.Facade
{
    internal class FacadeDescription
    {
        internal Element WallElement { get; private set; }
        internal Solid Solid { get; private set; }
        internal PlanarFace PlanarFace { get; private set; }
        internal Plane Plane { get; private set; }
        internal XYZ DirectionalBasis { get; private set;  }
        internal Element WallPart { get; private set;  }

        internal FacadeDescription(Document document, ElementId wallElementId, bool CreateParts = true)
        {
            WallElement = document.GetElement(wallElementId);
            Solid = SelectSolid();
            PlanarFace = SelectPlanarFace();
            Plane = SelectPlane();
            DirectionalBasis = SelectDirectionalBasis();

            if (CreateParts)
            {
                WallPart = SetWallPart(document);
            }
        }

        private Plane SelectPlane()
        {
            XYZ origin = PlanarFace.Origin;
            XYZ normal = PlanarFace.FaceNormal;
            return Plane.CreateByNormalAndOrigin(normal, origin);
        }

        private PlanarFace SelectPlanarFace()
        {
            FaceArray faceArray = Solid.Faces;

            PlanarFace faceSplit = null;

            foreach (PlanarFace face in faceArray)
            {
                bool FrontClause = Solid.SurfaceArea < 10 && face.Area > 1;
                bool PanelClause = Solid.SurfaceArea > 10 && face.Area > 5;

                if (!face.FaceNormal.IsAlmostEqualTo(XYZ.BasisZ) && (FrontClause || PanelClause) && face.MaterialElementId != null)
                {
                    faceSplit = face;
                }
            }

            return faceSplit;
        }

        private XYZ SelectDirectionalBasis()
        {
            LocationCurve curve = (LocationCurve)WallElement.Location;
            Line direction = (Line)curve.Curve;
            return direction.Direction;
        }

        private Solid SelectSolid()
        {
            Options options = new Options();
            GeometryElement geometryObject = WallElement.get_Geometry(options);

            Solid geomSolid = null;
            foreach (GeometryObject item in geometryObject)
            {
                if (item is Solid solid)
                {
                    geomSolid = solid;
                }
            }

            return geomSolid;
        }

        private Element SetWallPart(Document document)
        {
            ICollection<ElementId> elementIds = new List<ElementId>()
            {
                WallElement.Id
            };

            if (PartUtils.AreElementsValidForCreateParts(document, elementIds))
            {
                using (Transaction transaction = new Transaction(document, "Parts creation"))
                {
                    transaction.Start();
                    PartUtils.CreateParts(document, elementIds);
                    transaction.Commit();
                }
            }

            ElementFilter filter = new ElementCategoryFilter(BuiltInCategory.OST_Parts);

            if (WallElement.GetDependentElements(filter).Count > 0)
            {
                return document.GetElement(WallElement.GetDependentElements(filter)[0]);
            }
            else
            {
                throw new ArgumentNullException(paramName: "FacadeDescription.WallPartId", message: "Не удалось создать часть из стены") ;
            }
            
        }
    }
}