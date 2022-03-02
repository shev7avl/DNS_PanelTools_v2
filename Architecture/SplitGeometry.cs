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
        public static void CreatePartsSection(Document document, ElementId wallId, bool straight=false)
        {
            AddinSettings settings = AddinSettings.GetSettings();


            Element partEl = document.GetElement(wallId);
            ICollection<ElementId> partsId = new List<ElementId>() { wallId };

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

            Part part = (Part)partEl;
            Element wall = document.GetElement(part.GetSourceElementIds().FirstOrDefault().HostElementId);

            LocationCurve curve = (LocationCurve)wall.Location;
            Line direction = (Line)curve.Curve;

            XYZ directionBase = direction.Direction;

            FaceArray faceArray = geomSolid.Faces;

            PlanarFace faceSplit = null;

            foreach (PlanarFace face in faceArray)
            {
                bool FrontClause = geomSolid.SurfaceArea < 10 && face.Area > 1;
                bool PanelClause = geomSolid.SurfaceArea > 10 && face.Area > 5;

                if (!face.FaceNormal.IsAlmostEqualTo(XYZ.BasisZ) && (FrontClause || PanelClause) && face.MaterialElementId != null)
                {
                    faceSplit = face;
                }
            }

            Debug.WriteLine("------------");
            Debug.WriteLine("Нашли грань");
            XYZ origin = faceSplit.Origin;
            Debug.WriteLine($"Нашли начальную точку: {origin}");
            XYZ normal = faceSplit.FaceNormal;
            Debug.WriteLine($"Нашли нормаль к грани: {normal}");

            Transaction transaction = new Transaction(document, "Creating a SketchPlane");
            TransactionSettings.SetFailuresPreprocessor(transaction);


            transaction.Start();

            document.Regenerate();
            Debug.WriteLine($"Начали транзакцию: {transaction.GetName()}");
            Plane plane = Plane.CreateByNormalAndOrigin(normal, origin);
            Debug.WriteLine($"Создали плоскость: {plane}");

            IList<Curve> curves = SetCurvesList(straight, partEl, geomSolid, directionBase, faceSplit, plane);

            SketchPlane sketchPlane = SketchPlane.Create(document, plane);
            Debug.WriteLine($"Создали плоскость эскиза: {sketchPlane}");
            transaction.Commit();

            Debug.WriteLine($"Пытаемся разрезать части");

            transaction.Start();
            ICollection<ElementId> refiD = new List<ElementId>();
            PartMaker maker = PartUtils.DivideParts(document, partsId, refiD, curves, sketchPlane.Id);
            transaction.Commit();
            //Gap setting
            transaction.Start();
            maker.get_Parameter(BuiltInParameter.PARTMAKER_PARAM_DIVISION_GAP).SetValueString($"{settings.GetTileModule().ModuleGap}");
            transaction.Commit();

        }

        private static IList<Curve> SetCurvesList(bool straight, Element partEl, Solid geomSolid, XYZ directionBase, PlanarFace faceSplit, Plane plane)
        {
            IList<Curve> curves = default;
            if (geomSolid.SurfaceArea >= 10)
            {
                if (!straight)
                {
                    curves = CreateBrickOutlay(partEl, faceSplit, plane, directionBase);
                    Debug.WriteLine($"Создали список кривых: {curves}");
                }
                else if (straight)
                {
                    curves = CreateTileOutlay(partEl, faceSplit, plane);
                    Debug.WriteLine($"Создали список кривых: {curves}");
                }
            }
            else if (geomSolid.SurfaceArea < 10)
            {
                curves = CreateFrontOutlay(partEl, faceSplit, plane);
            }

            return curves;
        }

        public static IList<Curve> CreateFrontOutlay(Element partEl, Face face, Plane plane)
        {
            double conStepH, conGap;
            PanelOutline(partEl, face, out double conLenU, out double conHeiV, out IList<Curve> curves);
            AddinSettings settings = AddinSettings.GetSettings();

            conStepH = settings.GetTileModule().ModuleHeight;
            conGap = settings.GetTileModule().ModuleGap;

            Curve Left = curves[2];
            Curve Top = curves[0];
            Line leftLine = (Line)Left;

            for (double i = 0; i <= conHeiV - (conStepH + conGap); i = i + conStepH + conGap)
            {
                Curve curve1 = OffsetCurve(Top, leftLine, conStepH + conGap);
                curves.Add(curve1);
                Top = curve1;
            }

            return curves;
        }

        public static IList<Curve> CreateBrickOutlay(Element partEl, Face face, Plane plane, XYZ directionBase)
        {

            AddinSettings settings = AddinSettings.GetSettings();
            PanelOutline(partEl, face, out double conLenU, out double conHeiV, out IList<Curve> curves);

            //TODO: тестируем новый алгоритм нарезки плитки
            //TODO: попробовать "клинкерную" раскладку

            double StepV = settings.GetTileModule().ModuleWidth;
            double StepH = settings.GetTileModule().ModuleHeight;        
            double Gap = settings.GetTileModule().ModuleGap;
            
            BoundingBoxXYZ partBbox = partEl.get_Geometry(new Options()).GetBoundingBox();

            Curve Left = curves[2];
            Curve Bot = curves[0];

            Curve directionCurve = Line.CreateBound(directionBase, XYZ.Zero);

            List<Curve> brickBaseLine = BrickLine((Line)Bot, (Line)Left, settings.GetTileModule());
            foreach (Curve item in brickBaseLine)
            {
                if (Geometry.InBox(partBbox, item.GetEndPoint(0)) && Geometry.InBox(partBbox, item.GetEndPoint(1)))
                {
                    curves.Add(item);
                }
                
            }

            for (double i = 0; i <= conLenU - (StepV+Gap); i = i + (StepV+Gap))
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

            }

            return curves;
        }

        internal static IList<Curve> CreateBrickOutlay(FacadeDescription facadeDescription)
        {

            AddinSettings settings = AddinSettings.GetSettings();

            PanelOutline((Part)facadeDescription.WallPart, facadeDescription.PlanarFace, out double conLenU, out double conHeiV, out IList<Curve> curves);

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
                if (Geometry.InBox(partBbox, item.GetEndPoint(0)) && Geometry.InBox(partBbox, item.GetEndPoint(1)))
                {
                    curves.Add(item);
                }

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

            }

            return curves;
        }


        internal static IList<Curve> CreateTileOutlay(FacadeDescription facadeDescription)
        {
            double conStepV, conStepH, conGap;
            
            PanelOutline((Part)facadeDescription.WallPart, facadeDescription.PlanarFace, out double conLenU, out double conHeiV, out IList<Curve> curves);

            AddinSettings settings = AddinSettings.GetSettings();
            conStepV = settings.GetTileModule().ModuleWidth; //было 288 стало 300
            conStepH = settings.GetTileModule().ModuleHeight; //было 88 стало 100
            conGap = settings.GetTileModule().ModuleGap; //было 12 осталось 12

            Curve Left = curves[2];
            Curve Top = curves[0];
            Line topLine = (Line)Top;
            Line leftLine = (Line)Left;

            for (double i = 0; i < conLenU - conStepV - conGap; i = i + conStepV + conGap)
            {
                Curve curve1 = OffsetCurve(Left, topLine, conStepV + conGap);
                curves.Add(curve1);
                Left = curve1;
            }

            for (double i = 0; i < conHeiV - (conStepH + conGap); i = i + conStepH + conGap)
            {
                Curve curve1 = OffsetCurve(Top, leftLine, conStepH + conGap);
                curves.Add(curve1);
                Top = curve1;
            }

            return curves;
        }

        public static IList<Curve> CreateTileOutlay(Element partEl, Face face, Plane plane)
        {
            double conStepV, conStepH, conGap;
            PanelOutline(partEl, face, out double conLenU, out double conHeiV, out IList<Curve> curves);

            AddinSettings settings = AddinSettings.GetSettings();
            conStepV = settings.GetTileModule().ModuleWidth; //было 288 стало 300
            conStepH = settings.GetTileModule().ModuleHeight; //было 88 стало 100
            conGap = settings.GetTileModule().ModuleGap; //было 12 осталось 12

            Curve Left = curves[2];
            Curve Top = curves[0];
            Line topLine = (Line)Top;
            Line leftLine = (Line)Left;

            for (double i = 0; i < conLenU - conStepV - conGap ; i = i + conStepV + conGap)
            {
                Curve curve1 = OffsetCurve(Left, topLine, conStepV + conGap);
                curves.Add(curve1);
                Left = curve1;
            }
            
            for (double i = 0; i < conHeiV - (conStepH + conGap); i = i + conStepH + conGap)
            {
                Curve curve1 = OffsetCurve(Top, leftLine, conStepH + conGap);
                curves.Add(curve1);
                Top = curve1;
            }

            return curves;
        }

        private static void PanelOutline(Element partEl, Face face, out double conLenU, out double conHeiV, out IList<Curve> curves)
        {

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

            XYZ pt1 = new XYZ();
            XYZ pt2 = new XYZ();
            XYZ pt3 = new XYZ();

            if (Math.Abs(horBasis.Direction.X) == 1)
            {
                if (horBasis.Direction.X == 1)
                {
                    pt1 = new XYZ(originXYZ.X, originXYZ.Y, originXYZ.Z);
                    pt2 = new XYZ(pt1.X + convWidth * horBasis.Direction.X, pt1.Y, pt1.Z);
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
                    pt2 = new XYZ(pt1.X, pt1.Y + convWidth * horBasis.Direction.Y, pt1.Z);
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
                    pt3 = new XYZ(pt1.X, pt1.Y, pt1.Z + convHeigth * verBasis.Direction.Z);
                }
                else
                {
                    pt3 = new XYZ(pt1.X, pt1.Y, pt1.Z - convHeigth * verBasis.Direction.Z);
                }
                
            }

            XYZ pt4 = new XYZ(pt2.X, pt2.Y, pt3.Z);

            Curve curveBot = Line.CreateBound(pt1, pt2);
            Curve curveLeft = Line.CreateBound(pt1, pt3);
            Curve curveRight = Line.CreateBound(pt2, pt4);

            List<Curve> curves = new List<Curve>() {
                    curveBot,
                    curveRight,
                    curveLeft
                    };

            return curves;
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