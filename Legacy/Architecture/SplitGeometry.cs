using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Autodesk.Revit.DB;
using DSKPrim.PanelTools.Facade;
using DSKPrim.PanelTools.ProjectEnvironment;
using DSKPrim.PanelTools.Utility;

namespace DSKPrim.PanelTools.Architecture
{
    public static class SplitGeometry
    {
        
        internal static IList<Curve> CreateBrickOutlay(FacadeDescription facadeDescription)
        {

            AddinSettings settings = AddinSettings.GetSettings();

            IList<Curve> curves = PanelOutline(facadeDescription, out double conLenU, out double conHeiV);

            //TODO: тестируем новый алгоритм нарезки плитки
            //TODO: попробовать "клинкерную" раскладку

            double StepV = settings.GetTileModule().ModuleWidth;
            double StepH = settings.GetTileModule().ModuleHeight;
            double Gap = settings.GetTileModule().ModuleGap;

            BoundingBoxXYZ partBbox = facadeDescription.WallPart.get_Geometry(new Options()).GetBoundingBox();

            Curve Left = curves[2];
            Curve Bot = curves[0];

            Curve directionCurve = Line.CreateBound(facadeDescription.DirectionalBasis, XYZ.Zero);

            List<Curve> brickBaseLine = BrickLine((Line)Bot, (Line)Left, settings.GetTileModule());
            foreach (Curve item in brickBaseLine)
            {
                    curves.Add(item);
            }

            for (double i = 0; i <= conLenU - (StepV + Gap); i = i + (StepV + Gap))
            {
                List<Curve> temp = new List<Curve>();
                foreach (Curve item in brickBaseLine)
                {
                    //TODO: Проверить метод OffsetCurve. При смещении линии длиной 0.32 c направлением (0,0,1) вдоль вектора (0,-1,0) даёт линию длиной 48 с направлением (-1,0,0.006)
                    Curve curve1 = OffsetCurve(item, directionCurve, StepV + Gap);
                    temp.Add(curve1);

                    if (Geometry.InBox(partBbox, curve1.GetEndPoint(0)) && Geometry.InBox(partBbox, curve1.GetEndPoint(1)))
                    {
                        curves.Add(curve1);
                    }
                }
                brickBaseLine.Clear();
                foreach (var item in temp)
                {
                    brickBaseLine.Add(item);
                }
                temp.Clear();
            }


            for (double i = 0; i <= conHeiV; i = i + StepH + Gap)
            {
                Curve curve2 = OffsetCurve(Bot, Left, StepH + Gap);
                curves.Add(curve2);
                Bot = curve2;

                double diff = conHeiV - i;
                if (diff < StepH + Gap)
                {
                    Curve topCurve = OffsetCurve(Bot, Left, diff);
                    curves.Add(topCurve);
                }
            }

            return curves;
        }


        internal static IList<Curve> CreateTileOutlay(FacadeDescription facadeDescription)
        {
            double conStepV, conStepH, conGap;

            IList<Curve> curves = PanelOutline(facadeDescription, out double conLenU, out double conHeiV);

            AddinSettings settings = AddinSettings.GetSettings();
            double StepV = settings.GetTileModule().ModuleWidth; //было 288 стало 300
            double StepH = settings.GetTileModule().ModuleHeight; //было 88 стало 100
            double Gap = settings.GetTileModule().ModuleGap; //было 12 осталось 12

            Curve Left = curves[2];
            Curve Bot = curves[0];
            Line topLine = (Line)Bot;
            Line leftLine = (Line)Left;

            for (double i = 0; i < conLenU - StepV - Gap; i = i + StepV + Gap)
            {
                Curve curve1 = OffsetCurve(Left, topLine, StepV + Gap);
                curves.Add(curve1);
                Left = curve1;
            }

            for (double i = 0; i <= conHeiV; i = i + StepH + Gap)
            {
                Curve curve2 = OffsetCurve(Bot, Left, StepH + Gap);
                curves.Add(curve2);
                Bot = curve2;

                double diff = conHeiV - i;
                if (diff < StepH + Gap)
                {
                    Curve topCurve = OffsetCurve(Bot, Left, diff);
                    curves.Add(topCurve);
                }
            }

            return curves;
        }

        internal static IList<Curve> CreateFrontOutlay(FacadeDescription facadeDescription)
        {
            double conStepH, conGap;

            IList<Curve> curves = PanelOutline(facadeDescription, out double conLenU, out double conHeiV);

            AddinSettings settings = AddinSettings.GetSettings();

            conStepH = settings.GetTileModule().ModuleHeight; //было 88 стало 100
            conGap = settings.GetTileModule().ModuleGap; //было 12 осталось 12

            Curve Left = curves[2];
            Curve Top = curves[0];

            Line leftLine = (Line)Left;

            for (double i = 0; i < conHeiV - (conStepH + conGap); i = i + conStepH + conGap)
            {
                Curve curve1 = OffsetCurve(Top, leftLine, conStepH + conGap);
                curves.Add(curve1);
                Top = curve1;
            }

            return curves;
        }
        private static IList<Curve> PanelOutline(FacadeDescription facadeDescription, out double conLenU, out double conHeiV)
        {
            //TODO: переключить считывание габаритов с части на стену
            Element wallElement = facadeDescription.WallElement;


            double LenU = wallElement.get_Parameter(BuiltInParameter.CURVE_ELEM_LENGTH).AsDouble();
            double HeiV = wallElement.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM).AsDouble();

            conLenU = UnitUtils.ConvertFromInternalUnits(LenU, DisplayUnitType.DUT_MILLIMETERS);
            conHeiV = UnitUtils.ConvertFromInternalUnits(HeiV, DisplayUnitType.DUT_MILLIMETERS);

            return CreateRectangle(facadeDescription, conLenU, conHeiV);
        }

        

        private static List<Curve> CreateRectangle(FacadeDescription facadeDescription, double width, double heigth)
        {
            //Определения базисных векторов

            Face face = facadeDescription.PlanarFace;

            XYZ[] scaledPoints = ScalePointsOnPanel(face, width, heigth);

            Curve curveBot = Line.CreateBound(scaledPoints[0], scaledPoints[1]);
            Curve curveLeft = Line.CreateBound(scaledPoints[0], scaledPoints[2]);
            Curve curveRight = Line.CreateBound(scaledPoints[1], scaledPoints[3]);

            List<Curve> curves = new List<Curve>() {
                    curveBot,
                    curveRight,
                    curveLeft
                    };

            return curves;
        }

        private static XYZ[] ScalePointsOnPanel(Face face, double width, double heigth)
        {
            XYZ[] points = GetOriginPoints(face);
            Line[] basicLines = GetBasicLines(points);
            AddinSettings settings = AddinSettings.GetSettings();
            TileModule tileModule = settings.GetTileModule();


            double convWidth = UnitUtils.ConvertToInternalUnits(width, DisplayUnitType.DUT_MILLIMETERS);
            double convHeigth = UnitUtils.ConvertToInternalUnits(heigth, DisplayUnitType.DUT_MILLIMETERS);
            double convHalfGap = UnitUtils.ConvertToInternalUnits(tileModule.ModuleGap, DisplayUnitType.DUT_MILLIMETERS);

            XYZ[] scaledPoints = new XYZ[4];

            bool AlongXAxis = Math.Abs(basicLines[0].Direction.X) == 1;
            bool AlongYAxis = Math.Abs(basicLines[0].Direction.Y) == 1;

            if (AlongXAxis)
            {
                if (basicLines[0].Direction.X == 1)
                {
                    scaledPoints[0] = new XYZ(points[0].X - convHalfGap, points[0].Y, points[0].Z);
                    scaledPoints[1] = new XYZ(scaledPoints[0].X + convWidth * basicLines[0].Direction.X, scaledPoints[0].Y, scaledPoints[0].Z);
                }
                else
                {
                    scaledPoints[0] = new XYZ(points[0].X - convHalfGap, points[0].Y, points[0].Z);
                    scaledPoints[1] = new XYZ(scaledPoints[0].X + convWidth * basicLines[0].Direction.X, scaledPoints[0].Y, scaledPoints[0].Z);
                }

            }
            else if (AlongYAxis)
            {
                if (basicLines[0].Direction.Y == 1)
                {
                    scaledPoints[0] = new XYZ(points[0].X, points[0].Y - convHalfGap, points[0].Z);
                    scaledPoints[1] = new XYZ(scaledPoints[0].X, scaledPoints[0].Y + convWidth * basicLines[0].Direction.Y, scaledPoints[0].Z);
                }
                else
                {
                    scaledPoints[0] = new XYZ(points[0].X, points[0].Y - convHalfGap, points[0].Z);
                    scaledPoints[1] = new XYZ(scaledPoints[0].X, scaledPoints[0].Y + convWidth * basicLines[0].Direction.Y, scaledPoints[0].Z);
                }

            }
            if (Math.Abs(basicLines[1].Direction.Z) == 1)
            {
                if (basicLines[1].Direction.Z == 1)
                {
                    scaledPoints[2] = new XYZ(scaledPoints[0].X, scaledPoints[0].Y, scaledPoints[0].Z + convHeigth * basicLines[1].Direction.Z);
                }
                else
                {
                    scaledPoints[2] = new XYZ(scaledPoints[0].X, scaledPoints[0].Y, scaledPoints[0].Z - convHeigth * basicLines[1].Direction.Z);
                }

            }

            scaledPoints[3] = new XYZ(scaledPoints[1].X, scaledPoints[1].Y, scaledPoints[2].Z);

            return scaledPoints;
        }

        /// <summary>
        /// index: 0 - origin,
        /// index: 1 - VerticalBase,
        /// index: 2 - HorizontalBase,
        /// </summary>
        /// <param name="face"></param>
        /// <returns></returns>
        private static XYZ[] GetOriginPoints(Face face)
        {
            BoundingBoxUV boxUV = face.GetBoundingBox();
            UV origin = boxUV.Min;
            UV VerticalBase = new UV(boxUV.Min.U, boxUV.Max.V);
            UV HorizontalBase = new UV(boxUV.Max.U, boxUV.Min.V);

            XYZ[] points = new XYZ[3];

            points[0] = face.Project(face.Evaluate(origin)).XYZPoint;
            points[1] = face.Project(face.Evaluate(VerticalBase)).XYZPoint;
            points[2] = face.Project(face.Evaluate(HorizontalBase)).XYZPoint;

            return points;
        }

        /// <summary>
        /// index: 0 - horizontalBasis,
        /// index: 1 - verticalBasis
        /// </summary>
        /// <param name="face"></param>
        /// <returns></returns>
        private static Line[] GetBasicLines(XYZ[] points)
        {
            Line[] basicLines = new Line[2];

            basicLines[0] = Line.CreateBound(points[0], points[2]); //horizontal
            basicLines[1] = Line.CreateBound(points[0], points[1]); //vertical
            return basicLines;
        }

        /// <summary>
        /// Функция для создания первого вертикального ряда клинкерной нарезки плитки. Для создания требуются опорные линии эскиза - нижняя и левая.
        /// </summary>
        /// <param name="bot">Нижняя линяя эскиза разрезки на части. Нужны для определения направления смещения и для базовой точки эскиза (левой нижней)</param>
        /// <param name="left">Левая линия эскиза разрезки на части. Нужна для определения высоты эскиза и организации цикла копирования по вертикали</param>
        /// <param name="width">Ширина плитки</param>
        /// <param name="height">Высота плитки</param>
        /// <param name="stitchWidth">Толщина шва</param>
        /// <returns>Возвращает список линии класса Curve, описывающие вертикальные линии кирпичной раскладки плитки (первый левый ряд, снизу вверх).</returns>
        internal static List<Curve> BrickLine(Line bot, Line left, TileModule tileModule)
        {
            List<Curve> brickLine = new List<Curve>();
            double fullWidth = tileModule.ModuleWidth + tileModule.ModuleGap;
            double fullHeight = tileModule.ModuleHeight + tileModule.ModuleGap;
            XYZ startPoint = bot.GetEndPoint(0);


            do
            {
                XYZ pt1 = new XYZ(
                bot.GetEndPoint(0).X + UnitUtils.ConvertToInternalUnits(fullWidth / 2, DisplayUnitType.DUT_MILLIMETERS) * bot.Direction.X,
                bot.GetEndPoint(0).Y + UnitUtils.ConvertToInternalUnits(fullWidth / 2, DisplayUnitType.DUT_MILLIMETERS) * bot.Direction.Y,
                startPoint.Z
                );
                XYZ pt2 = new XYZ(
                    pt1.X,
                    pt1.Y,
                    pt1.Z + UnitUtils.ConvertToInternalUnits(fullHeight, DisplayUnitType.DUT_MILLIMETERS)
                    );


                XYZ pt5 = new XYZ(
                bot.GetEndPoint(0).X + UnitUtils.ConvertToInternalUnits(fullWidth, DisplayUnitType.DUT_MILLIMETERS) * bot.Direction.X,
                bot.GetEndPoint(0).Y + UnitUtils.ConvertToInternalUnits(fullWidth, DisplayUnitType.DUT_MILLIMETERS) * bot.Direction.Y,
                pt2.Z
                );
                XYZ pt6 = new XYZ(
                    pt5.X,
                    pt5.Y,
                    pt5.Z + UnitUtils.ConvertToInternalUnits(fullHeight, DisplayUnitType.DUT_MILLIMETERS)
                    );

                startPoint = new XYZ(
                    bot.GetEndPoint(0).X,
                    bot.GetEndPoint(0).Y,
                    pt1.Z + UnitUtils.ConvertToInternalUnits(fullHeight*2, DisplayUnitType.DUT_MILLIMETERS)
                    );
                brickLine.Add(Line.CreateBound(pt1, pt2));
                brickLine.Add(Line.CreateBound(pt5, pt6));

            } while (startPoint.Z <= left.GetEndPoint(1).Z);

            return brickLine;
        }

        public static Curve OffsetCurve(Curve curve, Curve direction, double offsetValue)
        {

            double offset = UnitUtils.ConvertToInternalUnits(offsetValue, DisplayUnitType.DUT_MILLIMETERS);

            Curve offsetCurve;
            Line directionLine = (Line)direction;
            XYZ pt1;
            XYZ pt2;
            if (Math.Abs(directionLine.Direction.X) == 1)
            {
                pt1 = new XYZ(curve.GetEndPoint(0).X + offset * directionLine.Direction.X, curve.GetEndPoint(0).Y, curve.GetEndPoint(0).Z);
                pt2 = new XYZ(curve.GetEndPoint(1).X + offset * directionLine.Direction.X, curve.GetEndPoint(1).Y, curve.GetEndPoint(1).Z);
            }
            else if (Math.Abs(directionLine.Direction.Y) == 1)
            {
                pt1 = new XYZ(curve.GetEndPoint(0).X, curve.GetEndPoint(0).Y + offset * directionLine.Direction.Y, curve.GetEndPoint(0).Z);
                pt2 = new XYZ(curve.GetEndPoint(1).X, curve.GetEndPoint(1).Y + offset * directionLine.Direction.Y, curve.GetEndPoint(1).Z);
            }
            else if (Math.Abs(directionLine.Direction.Z) == 1)
            {
                pt1 = new XYZ(curve.GetEndPoint(0).X, curve.GetEndPoint(0).Y, curve.GetEndPoint(0).Z + offset * directionLine.Direction.Z);
                pt2 = new XYZ(curve.GetEndPoint(1).X, curve.GetEndPoint(1).Y, curve.GetEndPoint(1).Z + offset * directionLine.Direction.Z);
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