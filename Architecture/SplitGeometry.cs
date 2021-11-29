﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using DSKPrim.PanelTools_v2.Utility;

namespace DSKPrim.PanelTools_v2.Architecture
{
    public static class SplitGeometry
    {
        public static void CreatePartsSection(Document document, ElementId partId)
        {
            Logger.Logger logger = Logger.Logger.getInstance();

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
                    logger.DebugLog("------------");
                    logger.DebugLog("Нашли грань");
                    XYZ origin = face.Origin;
                    logger.DebugLog($"Нашли начальную точку: {origin}");
                    XYZ normal = face.FaceNormal;
                    logger.DebugLog($"Нашли нормаль к грани: {normal}");

                        Transaction transaction = new Transaction(document, "Creating a SketchPlane");
                        IFailuresPreprocessor preprocessor = new WarningDiscard();
                        FailureHandlingOptions fho = transaction.GetFailureHandlingOptions();
                        fho.SetFailuresPreprocessor(preprocessor);
                        transaction.SetFailureHandlingOptions(fho);

                        transaction.Start();
                        document.Regenerate();
                        logger.DebugLog($"Начали транзакцию: {transaction.GetName()}");
                        Plane plane = Plane.CreateByNormalAndOrigin(normal, origin);
                        logger.DebugLog($"Создали плоскость: {plane}");

                        //IList<Curve> curves = CreateTileOutlay(partEl, face, plane);
                        IList<Curve> curves = CreateBrickOutlay(partEl, face, plane);

                        logger.DebugLog($"Создали список кривых: {curves}");
                        SketchPlane sketchPlane = SketchPlane.Create(document, plane);
                        logger.DebugLog($"Создали плоскость эскиза: {sketchPlane}");
                        logger.DebugLog($"Пытаемся разрезать части");
                        PartMaker maker = PartUtils.DivideParts(document, partsId, refiD, curves, sketchPlane.Id);
                        transaction.Commit();

                    break;
                }
            }
        }

        public static IList<Curve> CreateBrickOutlay(Element partEl, Face face, Plane plane)
        {
            double conLenU, conHeiV, conStepV, conStepH, conGap;
            IList<Curve> curves;
            PanelOutline(partEl, face, plane, out conLenU, out conHeiV, out curves);

            //TODO: тестируем новый алгоритм нарезки плитки
            //TODO: попробовать "клинкерную" раскладку

            conStepV = 288;
            conStepH = 88;
            conGap = 12;

            Curve Left = curves[2];
            Curve Top = curves[0];

            List<Curve> brickBaseLine = BrickLine((Line)Top, (Line)Left, conStepV, conStepH, conGap);
            foreach (Curve item in brickBaseLine)
            {
                curves.Add(item);
            }

            for (double i = 0; i < conLenU - (conStepV + conGap); i = i + conStepV + conGap)
            {
                List<Curve> temp = new List<Curve>();
                foreach (var item in brickBaseLine)
                {
                    temp.Add(OffsetCurve(item, Top, conStepV + conGap));
                    curves.Add(OffsetCurve(item, Top, conStepV + conGap));
                }
                brickBaseLine.Clear();
                foreach (var item in temp)
                {
                    brickBaseLine.Add(item);
                }
                temp.Clear();
            }


            for (double i = 0; i <= conHeiV - (conStepH + conGap); i = i + conStepH + conGap)
            {
                Curve curve1 = OffsetCurve(Top, Left, conStepH);
                curves.Add(curve1);
                Top = OffsetCurve(curve1, Left, conGap);
                curves.Add(Top);
            }

            return curves;
        }

        public static IList<Curve> CreateTileOutlay(Element partEl, Face face, Plane plane)
        {
            double conLenU, conHeiV, conStepV, conStepH, conGap;
            IList<Curve> curves;
            PanelOutline(partEl, face, plane, out conLenU, out conHeiV, out curves);

            //TODO: тестируем новый алгоритм нарезки плитки
            //TODO: попробовать "клинкерную" раскладку

            conStepV = 288;
            conStepH = 88;
            conGap = 12;

            Curve Left = curves[2];
            Curve Top = curves[0];

            for (double i = 0; i < conLenU - (conStepV+conGap); i = i + conStepV + conGap)
            {
                Curve curve1 = OffsetCurve(Left, curves[0], conStepV);
                curves.Add(curve1);
                Left = OffsetCurve(curve1, curves[0], conGap);
                curves.Add(Left);
            }

            
            for (double i = 0; i <= conHeiV - (conStepH+conGap); i = i + conStepH + conGap)
            {
                Curve curve1 = OffsetCurve(Top, curves[2], conStepH);
                curves.Add(curve1);
                Top = OffsetCurve(curve1, curves[2], conGap);
                curves.Add(Top);
            }

            return curves;
        }

        private static void PanelOutline(Element partEl, Face face, Plane plane, out double conLenU, out double conHeiV, out IList<Curve> curves)
        {
            Logger.Logger logger = Logger.Logger.getInstance();
            logger.LogMethodCall("CreateCurveArray");

            BoundingBoxUV boxUV = face.GetBoundingBox();

            double LenU = partEl.get_Parameter(BuiltInParameter.DPART_LENGTH_COMPUTED).AsDouble();
            double HeiV = partEl.get_Parameter(BuiltInParameter.DPART_HEIGHT_COMPUTED).AsDouble();

            conLenU = UnitUtils.ConvertFromInternalUnits(LenU, DisplayUnitType.DUT_MILLIMETERS);
            conHeiV = UnitUtils.ConvertFromInternalUnits(HeiV, DisplayUnitType.DUT_MILLIMETERS);
            curves = CreateRectangle(partEl, boxUV, face, conLenU, conHeiV);
        }

        private static List<Curve> CreateRectangle(Element partEl, BoundingBoxUV boxUV, Face face, double width, double heigth)
        {
            //Определения базисных векторов

            UV origin = boxUV.Min;
            UV VerticalBase = new UV(boxUV.Min.U, boxUV.Max.V);
            UV HorizontalBase = new UV(boxUV.Max.U, boxUV.Min.V);
            XYZ originXYZ = face.Evaluate(origin);
            XYZ verticalBase = face.Evaluate(VerticalBase);
            XYZ horizontalBase = face.Evaluate(HorizontalBase);
            Line horBasis = Line.CreateBound(originXYZ, horizontalBase);
            Line verBasis = Line.CreateBound(originXYZ, verticalBase);


            //Задание ширины и высоты
            double convWidth = UnitUtils.ConvertToInternalUnits(width, DisplayUnitType.DUT_MILLIMETERS);
            double convHeigth = UnitUtils.ConvertToInternalUnits(heigth, DisplayUnitType.DUT_MILLIMETERS);
            //Убираем начальное смещение
            double offsetWidth = UnitUtils.ConvertToInternalUnits(301.625, DisplayUnitType.DUT_MILLIMETERS);
            double offsetHeigth = UnitUtils.ConvertToInternalUnits(279.4, DisplayUnitType.DUT_MILLIMETERS);

            XYZ pt1 = new XYZ();
            XYZ pt2 = new XYZ();
            XYZ pt3 = new XYZ();

            if (Math.Abs(horBasis.Direction.X) == 1)
            {
                if (horBasis.Direction.X == 1)
                {
                    pt1 = new XYZ(originXYZ.X, originXYZ.Y, originXYZ.Z);
                    pt2 = new XYZ(pt1.X - convWidth * horBasis.Direction.X, pt1.Y, pt1.Z);
                }
                else
                {
                    pt1 = new XYZ(originXYZ.X, originXYZ.Y, originXYZ.Z);
                    pt2 = new XYZ(pt1.X + convWidth * horBasis.Direction.X, pt1.Y, pt1.Z);
                }
                
            }
            else if (Math.Abs(horBasis.Direction.Y) == 1)
            {
                if (horBasis.Direction.Y == 1)
                {
                    pt1 = new XYZ(originXYZ.X, originXYZ.Y, originXYZ.Z);
                    pt2 = new XYZ(pt1.X, pt1.Y - convWidth * horBasis.Direction.Y, pt1.Z);
                }
                else
                {
                    pt1 = new XYZ(originXYZ.X, originXYZ.Y, originXYZ.Z);
                    pt2 = new XYZ(pt1.X, pt1.Y + convWidth * horBasis.Direction.Y, pt1.Z);
                }
               
            }

            if (Math.Abs(verBasis.Direction.Z) == 1)
            {
                if (verBasis.Direction.Z == 1)
                {
                    pt3 = new XYZ(pt1.X, pt1.Y, pt1.Z - convHeigth * verBasis.Direction.Z);
                }
                else
                {
                    pt3 = new XYZ(pt1.X, pt1.Y, pt1.Z + convHeigth * verBasis.Direction.Z);
                }
                
            }

            XYZ pt4 = new XYZ(pt2.X, pt2.Y, pt3.Z);

            Curve curveTop = Line.CreateBound(pt1, pt2);
            Curve curveLeft = Line.CreateBound(pt1, pt3);
            Curve curveRight = Line.CreateBound(pt2, pt4);
            Curve curveBot = Line.CreateBound(pt3, pt4);

            List<Curve> curves = new List<Curve>() {
                    curveTop,
                    curveRight,
                    curveLeft
                    };

            return curves;
        }

        public static List<Curve> BrickLine(Line top, Line left, double width, double height, double stitchWidth)
        {
            List<Curve> brickLine = new List<Curve>();
            double fullWidth = width + stitchWidth;
            double fullHeight = height + stitchWidth;
            XYZ startPoint = top.GetEndPoint(0);
            do
            {
                XYZ pt1 = new XYZ(
                top.GetEndPoint(0).X + UnitUtils.ConvertToInternalUnits(fullWidth / 2, DisplayUnitType.DUT_MILLIMETERS) * top.Direction.X,
                top.GetEndPoint(0).Y + UnitUtils.ConvertToInternalUnits(fullWidth / 2, DisplayUnitType.DUT_MILLIMETERS) * top.Direction.Y,
                startPoint.Z
                );
                XYZ pt2 = new XYZ(
                    pt1.X,
                    pt1.Y,
                    pt1.Z - UnitUtils.ConvertToInternalUnits(fullHeight, DisplayUnitType.DUT_MILLIMETERS)
                    );
                XYZ pt3 = new XYZ(
                    pt1.X - UnitUtils.ConvertToInternalUnits(stitchWidth, DisplayUnitType.DUT_MILLIMETERS) * top.Direction.X,
                    pt1.Y - UnitUtils.ConvertToInternalUnits(stitchWidth, DisplayUnitType.DUT_MILLIMETERS) * top.Direction.Y,
                    pt1.Z
                    );
                XYZ pt4 = new XYZ(
                    pt3.X,
                    pt3.Y,
                    pt3.Z - UnitUtils.ConvertToInternalUnits(fullHeight, DisplayUnitType.DUT_MILLIMETERS)
                    );

                XYZ pt5 = new XYZ(
                top.GetEndPoint(0).X + UnitUtils.ConvertToInternalUnits(fullWidth, DisplayUnitType.DUT_MILLIMETERS) * top.Direction.X,
                top.GetEndPoint(0).Y + UnitUtils.ConvertToInternalUnits(fullWidth, DisplayUnitType.DUT_MILLIMETERS) * top.Direction.Y,
                pt2.Z
                );
                XYZ pt6 = new XYZ(
                    pt5.X,
                    pt5.Y,
                    pt5.Z - UnitUtils.ConvertToInternalUnits(fullHeight, DisplayUnitType.DUT_MILLIMETERS)
                    );
                XYZ pt7 = new XYZ(
                    pt5.X - UnitUtils.ConvertToInternalUnits(stitchWidth, DisplayUnitType.DUT_MILLIMETERS) * top.Direction.X,
                    pt5.Y - UnitUtils.ConvertToInternalUnits(stitchWidth, DisplayUnitType.DUT_MILLIMETERS) * top.Direction.Y,
                    pt5.Z
                    );
                XYZ pt8 = new XYZ(
                    pt7.X,
                    pt7.Y,
                    pt7.Z - UnitUtils.ConvertToInternalUnits(fullHeight, DisplayUnitType.DUT_MILLIMETERS)
                    );

                startPoint = new XYZ(
                    top.GetEndPoint(0).X,
                    top.GetEndPoint(0).Y,
                    pt1.Z - UnitUtils.ConvertToInternalUnits(fullHeight*2, DisplayUnitType.DUT_MILLIMETERS)
                    );
                brickLine.Add(Line.CreateBound(pt1, pt2));
                brickLine.Add(Line.CreateBound(pt3, pt4));
                brickLine.Add(Line.CreateBound(pt5, pt6));
                brickLine.Add(Line.CreateBound(pt7, pt8));

            } while (startPoint.Z - UnitUtils.ConvertToInternalUnits(fullHeight*2, DisplayUnitType.DUT_MILLIMETERS) >= left.GetEndPoint(1).Z);

            return brickLine;
        }
        //TODO: Переключить offsetCurve на метод API - Curve.CreateOffset
        public static Curve OffsetCurve(Curve curve, Curve direction, double offsetValue)
        {
            double offset = UnitUtils.ConvertToInternalUnits(offsetValue, DisplayUnitType.DUT_MILLIMETERS);

            Curve offsetCurve;
            Line directionLine = (Line)direction;
            XYZ pt1;
            XYZ pt2;
            if (Math.Abs(directionLine.Direction.X) == 1)
            {
                pt1 = new XYZ(curve.GetEndPoint(0).X+offset* directionLine.Direction.X, curve.GetEndPoint(0).Y, curve.GetEndPoint(0).Z);
                pt2 = new XYZ(curve.GetEndPoint(1).X+offset* directionLine.Direction.X, curve.GetEndPoint(1).Y, curve.GetEndPoint(1).Z);
            }
            else if (Math.Abs(directionLine.Direction.Y) == 1)
            {
                pt1 = new XYZ(curve.GetEndPoint(0).Y, curve.GetEndPoint(0).Y+offset* directionLine.Direction.Y, curve.GetEndPoint(0).Z);
                pt2 = new XYZ(curve.GetEndPoint(1).X, curve.GetEndPoint(1).Y+offset* directionLine.Direction.Y, curve.GetEndPoint(1).Z);
            }
            else if (Math.Abs(directionLine.Direction.Z) == 1)
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