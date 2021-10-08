﻿﻿using System;
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
        public static void CreatePartsSection(Document document, ElementId partId)
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
                        IList<Curve> curves = CreateCurveArray(partEl, face, plane, brick: true);
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

        public static IList<Curve> CreateCurveArray(Element partEl, Face face, Plane plane, bool brick=false)
        {
            BoundingBoxUV boxUV = plane.GetBoundingBoxUV();

            
            double LenU = partEl.get_Parameter(BuiltInParameter.DPART_LENGTH_COMPUTED).AsDouble();
            double HeiV = partEl.get_Parameter(BuiltInParameter.DPART_HEIGHT_COMPUTED).AsDouble();

            double conLenU = UnitUtils.ConvertFromInternalUnits(LenU, UnitTypeId.Millimeters);
            double conHeiV = UnitUtils.ConvertFromInternalUnits(HeiV, UnitTypeId.Millimeters);

            double conStepV;
            double conStepH;
            double conGap;
            IList<Curve> curves;

            if (brick)
            {
                conStepV = 240;
                conStepH = 70;
                conGap = 10;
                curves = CreateRectangle(boxUV, face, conStepV, conStepH, brick: true);
            }
            else
            {
                conStepV = 288;
                conStepH = 88;
                conGap = 12;
                curves = CreateRectangle(boxUV, face, conLenU, conHeiV);
            }
            
            


            //double offsetWidth = UnitUtils.ConvertToInternalUnits(300, UnitTypeId.Millimeters);
            //double offsetHeight = UnitUtils.ConvertToInternalUnits(100, UnitTypeId.Millimeters);

            if (brick)
            {
                int counter = 1;
                IList<Curve> lastBrick = curves;
                //Смещение прямоугольника по горизонтали
                for (double i = 0; i < conLenU - 300; i = i + conStepV + conGap)
                {
                    Curve curve1 = OffsetCurve(lastBrick[0], curves[0], conStepV + conGap);
                    Curve curve2 = OffsetCurve(lastBrick[1], curves[0], conStepV + conGap);
                    Curve curve3 = OffsetCurve(lastBrick[2], curves[0], conStepV + conGap);
                    Curve curve4 = OffsetCurve(lastBrick[3], curves[0], conStepV + conGap);

                    IList<Curve> newBrick = new List<Curve>() {
                        curve1,
                        curve2,
                        curve3,
                        curve4};

                    curves.Add(curve1);
                    curves.Add(curve2);
                    curves.Add(curve3);
                    curves.Add(curve4);

                    lastBrick = newBrick;
                }

                IList<Curve> lastRow = new List<Curve>();
                foreach (var item in curves)
                {
                    lastRow.Add(item);
                }
                int countH = 1;
                for (double i = 0; i <= conHeiV - 100; i = i + conStepH + conGap)
                {

                    DateTime execStart = new DateTime();
                    Debug.WriteLine($"------");
                    Debug.WriteLine($"Начали делать ряд {i}");
                    //foreach (Curve curve in lastRow)
                    //{
                    //    Curve offsetCurve = OffsetCurve(curve, curves[2], (conStepH + conGap)*countH);
                    //    curves.Add(offsetCurve);                     
                    //}

                    for (int cv = 0; cv < lastRow.Count-12; cv++)
                    {
                        if (cv % 2 == 1)
                        {
                            Curve offsetCurve = OffsetCurve(lastRow[cv], curves[2], (conStepH + conGap)*countH);
                            curves.Add(offsetCurve);
                        }
                        else
                        {
                            Curve offsetCurve = OffsetCurve(lastRow[cv], curves[2], (conStepH + conGap)*countH);
                            Curve offsetCurveH = OffsetCurve(offsetCurve, curves[0], (conStepV + conGap) / 2);
                            curves.Add(offsetCurveH);
                        }

                    }

                    DateTime execFinish = new DateTime();
                    Debug.WriteLine($"Закончили делать ряд {i}");
                    Debug.WriteLine($"Время выполнения: {execFinish-execStart} с");
                    Debug.WriteLine($"------");


                    //for (int cv = 0; cv < lastRow.Count; cv++)
                    //{
                    //    if (cv % 2 == 1)
                    //    {
                    //        Curve offsetCurve = OffsetCurve(lastRow[cv], curves[2], conStepH + conGap);
                    //        curves.Add(offsetCurve);
                    //        newRow.Add(offsetCurve);
                    //    }
                    //    else
                    //    {
                    //        Curve offsetCurve = OffsetCurve(lastRow[cv], curves[2], conStepH + conGap);
                    //        Curve offsetCurveH = OffsetCurve(offsetCurve, curves[0], (conStepV + conGap)/2);
                    //        curves.Add(offsetCurveH);
                    //        newRow.Add(offsetCurve);
                    //    }

                    //}
                    countH++;
                    counter++;

                }
            }
            else
            {
                Curve Left = curves[2];
                for (double i = 0; i < conLenU - 300; i = i + conStepV + conGap)
                {
                    Curve curve1 = OffsetCurve(Left, curves[0], conStepV);
                    curves.Add(curve1);
                    Left = OffsetCurve(curve1, curves[0], conGap);
                    curves.Add(Left);
                }

                Curve Top = curves[0];
                for (double i = 0; i <= conHeiV - 100; i = i + conStepH + conGap)
                {
                    Curve curve1 = OffsetCurve(Top, curves[2], conStepH);
                    curves.Add(curve1);
                    Top = OffsetCurve(curve1, curves[2], conGap);
                    curves.Add(Top);
                }
            }
            

            return curves;
        }

        private static List<Curve> CreateRectangle(BoundingBoxUV boxUV, Face face, double width, double heigth, bool brick = false)
        {
            ;

            //Определения базисных векторов
            UV origin = boxUV.Min;
            UV VerticalBase = new UV(boxUV.Min.U, boxUV.Max.V);
            UV HorizontalBase = new UV(boxUV.Max.U, boxUV.Min.V);
            XYZ originXYZ = face.Evaluate(origin);
            XYZ verticalBase = face.Evaluate(VerticalBase);
            XYZ horizontalBase = face.Evaluate(HorizontalBase);
            int horBasis = GetDirectionXYZ(originXYZ, horizontalBase);
            int vertBasis = GetDirectionXYZ(originXYZ, verticalBase);

            //Задание ширины и высоты
            double convWidth = UnitUtils.ConvertToInternalUnits(width, UnitTypeId.Millimeters);
            double convHeigth = UnitUtils.ConvertToInternalUnits(heigth, UnitTypeId.Millimeters);
            //Убираем начальное смещение
            double offsetWidth = UnitUtils.ConvertToInternalUnits(301.625, UnitTypeId.Millimeters);
            double offsetHeigth = UnitUtils.ConvertToInternalUnits(279.4, UnitTypeId.Millimeters);

            XYZ pt1 = new XYZ();
            XYZ pt2 = new XYZ();
            XYZ pt3 = new XYZ();

            if (horBasis == 0)
            {
                pt1 = new XYZ(originXYZ.X - offsetWidth, originXYZ.Y, originXYZ.Z+offsetHeigth);
                pt2 = new XYZ(pt1.X + convWidth, pt1.Y, pt1.Z);
            }
            else if (horBasis == 1)
            {
                pt1 = new XYZ(originXYZ.X , originXYZ.Y - offsetWidth, originXYZ.Z+offsetHeigth);
                pt2 = new XYZ(pt1.X, pt1.Y + convWidth, pt1.Z);
            }

            if (vertBasis == 2)
            {
                pt3 = new XYZ(pt1.X, pt1.Y, pt1.Z - convHeigth);
            }

            XYZ pt4 = new XYZ(pt2.X, pt2.Y, pt3.Z);

            Curve curveTop = Line.CreateBound(pt1, pt2);
            Curve curveLeft = Line.CreateBound(pt1, pt3);
            Curve curveRight = Line.CreateBound(pt2, pt4);
            Curve curveBot = Line.CreateBound(pt3, pt4);

            List<Curve> curves;
            if (brick)
            {
                curves = new List<Curve>() {
                    curveTop,
                    curveRight,
                    curveLeft,
                    curveBot
                    };
            }
            else
            {
                curves = new List<Curve>() {
                    curveTop,
                    curveRight,
                    curveLeft
                    };
            }
             
            
            return curves;
        }

        private static int GetDirectionXYZ(XYZ pointA, XYZ pointB)
        {
            double deltaX = Math.Abs(pointA.X - pointB.X);
            double deltaY = Math.Abs(pointA.Y - pointB.Y);
            double deltaZ = Math.Abs(pointA.Z - pointB.Z);

            if (deltaX > deltaY && deltaX > deltaZ) return 0;
            else if (deltaY > deltaX && deltaY > deltaZ) return 1;
            else return 2;
            
        }

        public static Curve OffsetCurve(Curve curve, Curve direction, double offsetValue)
        {
            double offset = UnitUtils.ConvertToInternalUnits(offsetValue, UnitTypeId.Millimeters);

            Curve offsetCurve;
            XYZ pt1;
            XYZ pt2;
            if (GetDirectionXYZ(direction.GetEndPoint(0), direction.GetEndPoint(1))==0)
            {
                pt1 = new XYZ(curve.GetEndPoint(0).X+offset, curve.GetEndPoint(0).Y, curve.GetEndPoint(0).Z);
                pt2 = new XYZ(curve.GetEndPoint(1).X+offset, curve.GetEndPoint(1).Y, curve.GetEndPoint(1).Z);
            }
            else if (GetDirectionXYZ(direction.GetEndPoint(0), direction.GetEndPoint(1)) == 1)
            {
                pt1 = new XYZ(curve.GetEndPoint(0).X, curve.GetEndPoint(0).Y+offset, curve.GetEndPoint(0).Z);
                pt2 = new XYZ(curve.GetEndPoint(1).X, curve.GetEndPoint(1).Y+offset, curve.GetEndPoint(1).Z);
            }
            else if (GetDirectionXYZ(direction.GetEndPoint(0), direction.GetEndPoint(1)) == 2)
            {
                pt1 = new XYZ(curve.GetEndPoint(0).X, curve.GetEndPoint(0).Y, curve.GetEndPoint(0).Z-offset);
                pt2 = new XYZ(curve.GetEndPoint(1).X, curve.GetEndPoint(1).Y, curve.GetEndPoint(1).Z-offset);
            }
            else
            {
                throw new Exception();
            }
            offsetCurve = Line.CreateBound(pt1, pt2);
            return offsetCurve;
        }

    }
}