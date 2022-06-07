using Autodesk.Revit.DB;
using DSKPrim.PanelTools.Legacy.Panel;
using DSKPrim.PanelTools.Panel;
using DSKPrim.PanelTools.Precast;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSKPrim.PanelTools.Legacy.Builders.MarkBuilder
{

    public class LongMarkSchema
    {
        private readonly ParameterMap _instanceMap;
        private readonly ParameterMap _symbolMap;
        private readonly PrecastPanel _panel;

        private StructureType PanelType { get { return _panel.StructureCategory.StructureType; } }

        public LongMarkSchema(PrecastPanel panel)
        {
            var elementFamily = panel.ActiveElement as FamilyInstance;
            var familySymbol = elementFamily.Symbol;

            _instanceMap = elementFamily.ParametersMap;
            _symbolMap = familySymbol.ParametersMap;
            _panel = panel;
        }

        public string GetMark()
        {
            switch (this.PanelType)
            {
                case StructureType.NOT_A_PANEL: 
                    return "";
                    break;
                case StructureType.NS_PANEL:
                    return GetNSMarkSchema();
                    break;
                case StructureType.VS_PANEL:
                    return GetVSMarkSchema();
                    break;
                case StructureType.PP_PANEL:
                    return GetPPMarkSchema();
                    break;
                case StructureType.BP_PANEL:
                    try
                    {
                        return GetBPMarkSchema();
                    }
                    catch (Exception)
                    {
                        return GetBPSupMarkSchema();
                        
                    }
                    break;
                case StructureType.PS_PANEL:
                    return GetPSMarkSchema();
                    break;
                default:
                    return "";
                    break;
            }
        }

        private string GetNSMarkSchema()
        {
            return String.Format("{0}.{1}_{2}.{3}.{4}_{5}.{6}_{7}",
                        _instanceMap.get_Item("СТАРТ").AsValueString(),
                        _instanceMap.get_Item("ФИНИШ").AsValueString(),
                        ParameterAsDecimeter(_instanceMap.get_Item("ГабаритДлина")),
                        ParameterAsDecimeter(_symbolMap.get_Item("ГабаритВысота")),
                        ParameterAsDecimeter(_symbolMap.get_Item("ГабаритТолщина")),
                        _instanceMap.get_Item("Тип PVL_СТАРТ").AsString().Split(' ')[1],
                        _instanceMap.get_Item("Тип PVL_ФИНИШ").AsString().Split(' ')[1],
                        GetClosureCode());
        }

        private string GetVSMarkSchema()
        {
            return String.Format("{0}.{1}.{2}_{3}.{4}_{5}",
                    ParameterAsDecimeter(_instanceMap.get_Item("ГабаритДлина")),
                    ParameterAsDecimeter(_symbolMap.get_Item("ГабаритВысота")),
                    ParameterAsDecimeter(_symbolMap.get_Item("ГабаритТолщина")),
                    _instanceMap.get_Item("Тип PVL_СТАРТ").AsString().Split(' ')[1],
                    _instanceMap.get_Item("Тип PVL_ФИНИШ").AsString().Split(' ')[1],
                    GetClosureCode());
        }

        private string GetPPMarkSchema()
        {
            return String.Format("{0}.{1}-{2}{3}",
                        ParameterAsDecimeter(_instanceMap.get_Item("ADSK_Размер_Длина")),
                        ParameterAsDecimeter(_instanceMap.get_Item("ADSK_Размер_Ширина")),
                        _instanceMap.get_Item("КодНагрузки").AsString(),
                        GetCutCode());
        }

        private string GetBPMarkSchema()
        {
            return String.Format("{0}.{1}.{2}_{3}.{4}.{5}.1_{6}.{7}_{8}.{9}",
                        ParameterAsDecimeter(_symbolMap.get_Item("Плита_Длина")),
                        ParameterAsDecimeter(_symbolMap.get_Item("Плита_Ширина")),
                        ParameterAsDecimeter(_symbolMap.get_Item("Плита_Толщина")),
                        ParameterAsDecimeter(_symbolMap.get_Item("Кронштейн_Отступ")),
                        ParameterAsDecimeter(_symbolMap.get_Item("Кронштейн_Шаг")),
                        _symbolMap.get_Item("Кронштейн_Количество").AsValueString(),
                        _symbolMap.get_Item("Отверстия_ПривязкаСлева").AsValueString(),
                        _symbolMap.get_Item("Отверстия_ПривязкаСправа").AsValueString(),
                        _symbolMap.get_Item("Отверстия_ПривязкаСпереди").AsValueString(),
                        _symbolMap.get_Item("Отверстия_РасстояниеМеждуПоY").AsValueString()
                        );
        }

        private string GetBPSupMarkSchema()
        {
            return String.Format("{0}.{1}.{2}_{3}.{4}.{5}.{6}",
                            ParameterAsDecimeter(_symbolMap.get_Item("Плита_Длина")),
                            ParameterAsDecimeter(_symbolMap.get_Item("Плита_Ширина")),
                            ParameterAsDecimeter(_symbolMap.get_Item("Плита_Толщина")),
                            ParameterAsDecimeter(_symbolMap.get_Item("Кронштейн1_Шаг")),
                            ParameterAsDecimeter(_symbolMap.get_Item("Кронштейн2_Шаг")),
                            ParameterAsDecimeter(_symbolMap.get_Item("Кронштейн3_Шаг")),
                            ParameterAsDecimeter(_symbolMap.get_Item("Кронштейн4_Шаг"))
                            );
        }

        private string GetPSMarkSchema()
        {
            return String.Format("{0}.{1}_{2}.{3}.{4}_{5}.{6}.{7}_{8}.{9}.{10}",
                        _symbolMap.get_Item("СТАРТ").AsValueString(),
                        _symbolMap.get_Item("ФИНИШ").AsValueString(),
                        ParameterAsDecimeter(_instanceMap.get_Item("ГабаритДлина")),
                        ParameterAsDecimeter(_symbolMap.get_Item("ГабаритВысота")),
                        ParameterAsDecimeter(_symbolMap.get_Item("ГабаритТолщина")),

                        ParameterAsDecimeter(_instanceMap.get_Item("WELDA_Отступ")),
                        ParameterAsDecimeter(_instanceMap.get_Item("WELDA_Шаг")),
                        _instanceMap.get_Item("WELDA_Количество").AsValueString(),
                        ParameterAsDecimeter(_instanceMap.get_Item("TR 24_Сверху_Отступ")),
                        ParameterAsDecimeter(_instanceMap.get_Item("TR 24_Сверху_Шаг")),
                        _instanceMap.get_Item("TR 24_Сверху_Количество").AsValueString()
                        );
        }

        private string GetClosureCode()
        {

            bool Closure1 = _instanceMap.get_Item("ПР1.ВКЛ").AsValueString() == "Да";
            bool Closure2 = _instanceMap.get_Item("ПР2.ВКЛ").AsValueString() == "Да";

            string window1 = "";
            if (Closure1)
            {
                window1 = String.Format("{0}.{1}.{2}.{3}",
                ParameterAsDecimeter(_instanceMap.get_Item("ПР1.Отступ")),
                ParameterAsDecimeter(_instanceMap.get_Item("ПР1.Ширина")),
                ParameterAsDecimeter(_instanceMap.get_Item("ПР1.Высота")),
                ParameterAsDecimeter(_instanceMap.get_Item("ПР1.ВысотаСмещение"))
                );
            }
            string window2 = "";
            if (Closure2)
            {
                window2 = String.Format("{0}.{1}.{2}.{3}",
                ParameterAsDecimeter(_instanceMap.get_Item("ПР2.Отступ")),
                ParameterAsDecimeter(_instanceMap.get_Item("ПР2.Ширина")),
                ParameterAsDecimeter(_instanceMap.get_Item("ПР2.Высота")),
                ParameterAsDecimeter(_instanceMap.get_Item("ПР2.ВысотаСмещение"))
                );
            }

            if (Closure1 && Closure2)
            {
                return String.Format("{0}_{1}", window1, window2);
            }
            else if (Closure1 || Closure1)
            {
                return String.Format("{0}{1}", window1, window2);
            }
            else
            {
                return "Г";
            }
        }

        private string GetCutCode()
        {
            if (_symbolMap.get_Item("Вырезы").AsValueString() == "Да")
            {
                return String.Format("_{0}.{1}.{2}.{3}",
                    ParameterAsDecimeter(_instanceMap.get_Item("Вырезы_Отступ_Начало")),
                    ParameterAsDecimeter(_symbolMap.get_Item("Вырезы_Шаг")),
                    _instanceMap.get_Item("Отверстия_Количество").AsValueString(),
                    ParameterAsDecimeter(_instanceMap.get_Item("Вырезы_Отступ_Конец"))
                    );
            }
            else

                return string.Empty;
        }
        private static string ParameterAsDecimeter(Parameter parameter)
        {
            if (parameter is null)
            {
                return "0";
            }
            return Math.Round(Convert.ToDouble(parameter.AsValueString()) / 10, 0).ToString();
        }
    }
}
