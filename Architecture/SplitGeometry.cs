﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace DSKPrim.PanelTools_v2.Architecture
{
    public static class SplitGeometry
    {
        public static void CreateSketchPlane(Document document, ElementId partId)
        {

            Options options = new Options();
            Element partEl = document.GetElement(partId);
            ICollection<ElementId> partsId = new List<ElementId>() { partId };
            ICollection<ElementId> refiD = new List<ElementId>();

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
                    Debug.WriteLine("------------");
                    Debug.WriteLine("Нашли грань");
                    XYZ origin = face.Origin;
                    Debug.WriteLine($"Нашли начальную точку: {origin}");
                    XYZ normal = face.FaceNormal;
                    Debug.WriteLine($"Нашли нормаль к грани: {normal}");
                    using (Transaction transaction = new Transaction(document, "Creating a SketchPlane"))
                    {

                        transaction.Start();
                        Debug.WriteLine($"Начали транзакцию: {transaction.GetName()}");
                        Plane plane = Plane.CreateByNormalAndOrigin(normal, origin);
                        Debug.WriteLine($"Создали плоскость: {plane}");
                        IList<Curve> curves = CreateCurveArray(partEl, face, plane);
                        Debug.WriteLine($"Создали список кривых: {curves}");
                        SketchPlane sketchPlane = SketchPlane.Create(document, plane);
                        Debug.WriteLine($"Создали плоскость эскиза: {sketchPlane}");
                        Debug.WriteLine($"Пытаемся разрезать части");
                        PartMaker maker = PartUtils.DivideParts(document, partsId, refiD, curves, sketchPlane.Id);
                        transaction.Commit();
                    }
                    break;
                }
            }
        }

        public static IList<Curve> CreateCurveArray(Element partEl, Face face, Plane plane)
        {
            IList<Curve> curves = new List<Curve>();
            BoundingBoxUV boxUV = plane.GetBoundingBoxUV();

            UV origin = boxUV.Min;
            UV max = boxUV.Max;
            UV VerticalBase = new UV(boxUV.Min.U, boxUV.Max.V);
            UV HorizontalBase = new UV(boxUV.Max.U, boxUV.Min.V);

            XYZ originXYZ = face.Evaluate(origin);
            XYZ verticalBase = face.Evaluate(VerticalBase);
            XYZ horizontalBase = face.Evaluate(HorizontalBase);
            XYZ maxXYZ = face.Evaluate(max);

            double stepV = UnitUtils.ConvertToInternalUnits(288, UnitTypeId.Millimeters);
            double gapV = UnitUtils.ConvertToInternalUnits(12, UnitTypeId.Millimeters);

            double stepH = UnitUtils.ConvertToInternalUnits(88, UnitTypeId.Millimeters);
            double gapH = UnitUtils.ConvertToInternalUnits(12, UnitTypeId.Millimeters);

            double LenU = partEl.get_Parameter(BuiltInParameter.DPART_LENGTH_COMPUTED).AsDouble();
            double HeiV = partEl.get_Parameter(BuiltInParameter.DPART_HEIGHT_COMPUTED).AsDouble();

            Curve curveLeft = Line.CreateBound(originXYZ, verticalBase*0.5);
            Curve curveRight = Line.CreateBound(maxXYZ*0.5, horizontalBase*0.5);
            Curve curveBot = Line.CreateBound(originXYZ, horizontalBase*0.5);
            Curve curveTop = Line.CreateBound(verticalBase*0.5, maxXYZ*0.5);

            curves.Add(curveRight);
            curves.Add(curveTop);
            curves.Add(curveLeft);
            curves.Add(curveBot);

            //for (double i = 0; i < LenU; i = i + stepV + gapV)
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

    }
}