using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace DSKPrim.PanelTools_v2.StructuralApps.Panel
{
    public abstract class Panel
    {
        public abstract Document ActiveDocument { get; set; }
        public abstract Element ActiveElement { get; set; }

        public virtual List<XYZ> IntersectedWindows { get; set; } = null;

        public abstract string LongMark { get; set; }

        public abstract string ShortMark { get; set; }

        public abstract string Index { get; set; }


        /// <summary>
        /// Проверяет равенство двух панелей по значениям "Марка" и прочим логическим условиям
        /// </summary>
        /// <param name="panelMark">Панель для сравнения</param>
        /// <returns></returns>
        public virtual bool Equal(Panel panelMark)
        {
            if (LongMark == panelMark.LongMark)
            {
                return true;
            }
            else return false;
        }

        /// <summary>
        /// Заполняет значения короткой и длинной марки
        /// </summary>
        public abstract void CreateMarks();

        /// <summary>
        /// Метод для переназначения значения короткой марки после присвоения индекса
        /// </summary>
        public virtual void SetIndex(int index, bool overwrite = false)
        {



            if (overwrite)
            {
                ShortMark = ShortMark.Split('-')[0];
            }
            ShortMark = $"{ShortMark}-{index}";
            Guid ADSK_panelMark = new Guid("92ae0425-031b-40a9-8904-023f7389963b");
            Transaction transaction = new Transaction(ActiveDocument, $"Назначение индекса: {ShortMark}");
            transaction.Start();
            ActiveElement.get_Parameter(ADSK_panelMark).Set(ShortMark);
            transaction.Commit();

        }

        public virtual void ReadMarks()
        {
            Logger.Logger logger = Logger.Logger.getInstance();

            Guid DNS_panelMark1 = new Guid(Properties.Resource.DNS_Полная_марка_изделия);

            Guid ADSK_panelMark1 = new Guid(Properties.Resource.ADSK_Марка_изделия);

            Guid ADSK_panelNum = new Guid(Properties.Resource.ADSK_Номер_изделия);

            try
            {
                LongMark = ActiveElement.get_Parameter(DNS_panelMark1).ToString();
            }
            catch (NullReferenceException)
            {
                logger.WriteLog("ОШИБКА: Не найден параметр DNS_Полная_марка_изделия");
                logger.WriteLog($"Нужный GUID: {Properties.Resource.DNS_Полная_марка_изделия}");
                logger.WriteLog("Проверьте шаблон и ФОП");
                logger.WriteLog("Завершение процедуры с ошибкой");
                logger.PrintLog();
                throw;
            }

            try
            {
                ShortMark = ActiveElement.get_Parameter(ADSK_panelMark1).AsString();
            }
            catch (Exception)
            {
                logger.WriteLog("ОШИБКА: Не найден параметр ADSK_Марка_изделия");
                logger.WriteLog($"Нужный GUID: {Properties.Resource.ADSK_Марка_изделия}");
                logger.WriteLog("Проверьте шаблон и ФОП");
                logger.WriteLog("Завершение процедуры с ошибкой");
                logger.PrintLog();
            }

            try
            {
                Index = ActiveElement.get_Parameter(ADSK_panelNum).AsString();
            }
            catch (Exception)
            {
                logger.WriteLog("ОШИБКА: Не найден параметр ADSK_Номер_изделия");
                logger.WriteLog($"Нужный GUID: {Properties.Resource.ADSK_Номер_изделия}");
                logger.WriteLog("Проверьте шаблон и ФОП");
                logger.WriteLog("Завершение процедуры с ошибкой");
                logger.PrintLog();
            }
            
            

        }

        public virtual void SetMarks()
        {

            Guid DNS_panelMark1 = new Guid(Properties.Resource.DNS_Полная_марка_изделия);

            Guid ADSK_panelMark1 = new Guid(Properties.Resource.ADSK_Марка_изделия);

            Logger.Logger logger = Logger.Logger.getInstance();

            Transaction transaction = new Transaction(ActiveDocument);

            transaction.Start($"Транзакция - {ActiveElement.Name}");

            if (ActiveElement.get_Parameter(DNS_panelMark1).AsString().Length == 0)
            {
                try
                {

                    ActiveElement.get_Parameter(DNS_panelMark1).Set(LongMark);

                }
                catch (NullReferenceException)
                {
                    logger.WriteLog("ОШИБКА: У элемента не найден параметр DNS_Полная_марка_изделия");
                    logger.WriteLog($"Нужный GUID: {Properties.Resource.DNS_Полная_марка_изделия}");
                    logger.WriteLog($"Ошибка в элементе с ID {ActiveElement.Id}");
                    logger.WriteLog("Проверьте шаблон, ФОП и корректность элемента");
                    logger.WriteLog("Завершение процедуры с ошибкой");
                    logger.PrintLog();
                    throw;
                }
            }
            if (ActiveElement.get_Parameter(new Guid(Properties.Resource.ADSK_Марка_изделия)).AsString().Length == 0)
            {
                try
                {


                    ActiveElement.get_Parameter(new Guid(Properties.Resource.ADSK_Марка_изделия)).Set(ShortMark);


                }
                catch (Exception)
                {
                    logger.WriteLog("ОШИБКА: У элемента не найден параметр ADSK_Марка_изделия");
                    logger.WriteLog($"Нужный GUID: {Properties.Resource.ADSK_Марка_изделия}");
                    logger.WriteLog($"Ошибка в элементе с ID {ActiveElement.Id}");
                    logger.WriteLog("Проверьте шаблон, ФОП и корректность элемента");
                    logger.WriteLog("Завершение процедуры с ошибкой");
                    logger.PrintLog();
                    throw;
                }
            }
            transaction.Commit();
        }

    }
}
