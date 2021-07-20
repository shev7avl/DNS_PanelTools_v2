using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace DSKPrim.PanelTools_v2.Architecture
{
    public static class SplitGeometry_bu
    {
        public static void CreateSketchPlane(Document document, ElementId partId)
        {
            
            ICollection<ElementId> partsId = new List<ElementId>() { partId };
            ICollection<ElementId> refiD = new List<ElementId>();

            
            PlanarFace face = getPlanarFace(document, partId);

            XYZ origin = face.Origin;

            XYZ normal = face.FaceNormal;

            using (Transaction transaction = new Transaction(document, "Creating a SketchPlane"))
            {
                transaction.Start();

                Plane plane = Plane.CreateByNormalAndOrigin(normal, origin);

                IList<Curve> curves = CreateCurveArray(document.GetElement(partId), face, plane);

                SketchPlane sketchPlane = SketchPlane.Create(document, plane);

                PartMaker maker = PartUtils.DivideParts(document, partsId, refiD, curves, sketchPlane.Id);

                transaction.Commit();
            }
        }
        /// <summary>
        /// Получаем грань с части, на которой будем производить разрезку на плитку
        /// </summary>
        /// <param name="document">Активный документ</param>
        /// <param name="partId">Id части, которую будем резать</param>
        /// <returns></returns>
        private static PlanarFace getPlanarFace(Document document, ElementId partId)
        {
            
            PlanarFace result = null;

            Element partEl = document.GetElement(partId);
            Options options = new Options();
            GeometryElement geometryObject = partEl.get_Geometry(options);
            Solid geomSolid = null;
            foreach (GeometryObject item in geometryObject)
            {
                if (item is Solid solid)
                {
                    geomSolid = solid;
                }
            }

            FaceArray faceArray = geomSolid.Faces;
            foreach (PlanarFace face in faceArray)
            {
                if (!face.FaceNormal.IsAlmostEqualTo(XYZ.BasisZ) && face.Area > 5)
                {
                    result = face;
                    break;
                }
            }
            return result;
        }

        public static IList<Curve> CreateCurveArray(Element partEl, Face face, Plane plane)
        {
            IList <Curve> curves = new List<Curve>();
            BoundingBoxUV boxUV = plane.GetBoundingBoxUV();

            UV origin = boxUV.Min;
            //UV max = boxUV.Max;
            //UV VerticalBase = new UV(boxUV.Min.U, boxUV.Max.V);
            //UV HorizontalBase = new UV(boxUV.Max.U, boxUV.Min.V);

            double LenU = partEl.get_Parameter(BuiltInParameter.DPART_LENGTH_COMPUTED).AsDouble();
            double HeiV = partEl.get_Parameter(BuiltInParameter.DPART_HEIGHT_COMPUTED).AsDouble();

            XYZ originXYZ = face.Evaluate(origin);
            XYZ verticalBase = originXYZ + new XYZ(0, 0, HeiV * 0.5);

            int orient = GetXYOrientation(partEl.get_Geometry(new Options()).GetBoundingBox());
            XYZ horizontalBase;
            XYZ maxXYZ;
            if (orient == 1)
            {
                horizontalBase = originXYZ + new XYZ(0, LenU*0.5, 0);
                maxXYZ = originXYZ + new XYZ(0, LenU*0.5, HeiV*0.5);
                
            }
            else
            {
                horizontalBase = originXYZ + new XYZ(LenU * 0.5, 0, 0);
                maxXYZ = originXYZ + new XYZ(LenU * 0.5, 0, HeiV * 0.5);
            }
           
           

            //XYZ verticalBase = face.Evaluate(VerticalBase);
            //XYZ horizontalBase = face.Evaluate(HorizontalBase);
            //XYZ maxXYZ = face.Evaluate(max);

            double stepV = 288 / 304.8;
            double gapV = 12 / 304.8;

            double stepH = 88 / 304.8;
            double gapH = 12 / 304.8;

            

            Curve curveLeft = Line.CreateBound(originXYZ, verticalBase);
            Curve curveRight = Line.CreateBound(maxXYZ, horizontalBase);
            Curve curveBot = Line.CreateBound(originXYZ, horizontalBase);
            Curve curveTop = Line.CreateBound(verticalBase, maxXYZ);

            curves.Add(curveRight);
            curves.Add(curveTop);
            curves.Add(curveLeft);
            curves.Add(curveBot);

            //for (double i = 0; i < LenU; i=i+stepV+gapV)
            //{
            //    curves.Add(curveLeft);
            //    Curve curve1 = curveLeft.CreateOffset(stepV, -horizontalBase);
            //    curves.Add(curve1);
            //    curveLeft = curve1.CreateOffset(gapV, -horizontalBase);
            //}
            
            //for (double i = 0; i < HeiV; i = i + stepH + gapH)
            //{
            //    curves.Add(curveBot);
            //    Curve curve1 = curveBot.CreateOffset(stepH, verticalBase);
            //    curves.Add(curve1);
            //    curveBot = curve1.CreateOffset(gapH, verticalBase);
            //}

            return curves;
        }
        /// <summary>
        /// Возвращает 1 если линейный объект располагается вдоль оси Х, возвращает 
        /// </summary>
        /// <param name="boxXYZ"></param>
        /// <returns></returns>
        private static int GetXYOrientation(BoundingBoxXYZ boxXYZ)
        {
            var result = 0;

            XYZ max = boxXYZ.Max;
            XYZ min = boxXYZ.Min;

            double maxX = max.X;
            double maxY = max.Y;

            double minX = min.X;
            double minY = min.Y;

            double deltaX = Math.Abs(maxX - minX);
            double deltaY = Math.Abs(maxY - minY);

            if (deltaX > 1 && deltaY < 1)
            {
                result += 1;
            }
            
            else if (deltaX < 1 && deltaY > 1)
            {
                result -= 1;
            }
            return result;
        }

    }
}
