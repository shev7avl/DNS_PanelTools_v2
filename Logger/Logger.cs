using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSKPrim.PanelTools_v2.Logger
{
    public class Logger
    {

        private static Logger loggerInstance { get; set; }

        private StringBuilder Builder {get; set;}


        private Logger()
        {
            Builder = new StringBuilder();
        }

        public static Logger getInstance()
        {
            if (loggerInstance is null)
            {
                loggerInstance = new Logger();
            }
            return loggerInstance;
        }

        public void WriteLog(string input)
        {
            Builder.AppendLine(input);
        }

        public void PrintLog(string direction, string filename)
        {
            File.WriteAllText($"{direction}{filename}", Builder.ToString());
        }

    }
}
