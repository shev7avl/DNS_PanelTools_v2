using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace DSKPrim.PanelTools_v2.Logger
{
    public class Logger: IDisposable
    {

        private static Logger LoggerInstance { get; set; }

        private StringBuilder Builder {get; set;}

        private static string Direction { get; set; }

        private Logger()
        {
            Builder = new StringBuilder();
            
            Direction = $@"C:\PM logs\";
        }

        public static Logger getInstance()
        {
            if (LoggerInstance is null)
            {
                LoggerInstance = new Logger();
            }
            return LoggerInstance;
        }

        public void WriteLog(object input)
        {
            Builder.AppendLine($"{DateTime.Now} - {input}");
        }

        public void DebugLog(object input)
        {
            Debug.WriteLine($"{DateTime.Now} - {input}");
        }

        public void LogMethodCall(object input)
        {
            Builder.AppendLine($"{DateTime.Now} - Вызван метод: {input}");
        }

        public void LogError(Exception e)
        {
            Builder.AppendLine($"В приложении было вызвано исключение { e}");
            Builder.AppendLine($"{e.Message}");
            Builder.AppendLine($"Работа была завершена с ошибкой");
            LoggerInstance.PrintLog();
            LoggerInstance.Dispose();
        }

        public void LogSuccessTime(Stopwatch stopWatch)
        {
            LoggerInstance.Separate();
            LoggerInstance.WriteLog("Процедура успешно завершена");
            stopWatch.Stop();
            // Get the elapsed time as a TimeSpan value.
            TimeSpan ts = stopWatch.Elapsed;

            // Format and display the TimeSpan value.
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);
            LoggerInstance.WriteLog($"Время выполнения: {elapsedTime}");
            LoggerInstance.PrintLog();
            LoggerInstance.Dispose();
        }

        public void PrintLog()
        {
            string TimeFormat = $"PM Journal {DateTime.Now.TimeOfDay.ToString().Split('.')[0].Replace(":", "")}_{DateTime.Now.Day}_{DateTime.Now.Month}_{DateTime.Now.Year}.txt";
            if (!Directory.Exists(Direction))
            {
                Directory.CreateDirectory(Direction);
            }           
            StreamWriter sw = new StreamWriter(Path.Combine(Direction, TimeFormat));
            sw.WriteLine(Builder.ToString());
            sw.Close();
        }

        public void Separate()
        {
            LoggerInstance.WriteLog("-----");
        }

        public void Dispose()
        {
            LoggerInstance = null;
            Builder = null;
            Direction = null;
        }
    }
}
