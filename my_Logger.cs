using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineNumber_checker
{
    public abstract class LogBase
    {
        public abstract void Log(string Messsage);
    }

    public class Logger : LogBase
    {
        String currentLogDateString;

        private String CurrentDirectory
        {
            get;
            set;
        }

        private String FileName
        {
            get;
            set;
        }

        private String FilePath
        {
            get;
            set;
        }

        public Logger()
        {
            UpdateLogDate();
        }

        public void UpdateLogDate()
        {
            currentLogDateString = DateTime.Now.Date.ToShortDateString();

            System.IO.Directory.CreateDirectory("C:\\ProgramData\\ApplicationLogs\\TN8_to_MES-app");

            this.CurrentDirectory = "C:\\ProgramData\\ApplicationLogs\\TN8_to_MES-app";
            this.FileName = "Log" + currentLogDateString.Replace('/', '-') + ".txt";
            this.FilePath = this.CurrentDirectory + "/" + this.FileName;
        }

        public override void Log(string Messsage)
        {
            UpdateLogDate();

            using (System.IO.StreamWriter w = System.IO.File.AppendText(this.FilePath))
            {
                w.Write("\r\nLog Entry : ");
                w.WriteLine("{0} {1}", DateTime.Now.ToLongTimeString(), DateTime.Now.ToLongDateString());
                w.WriteLine("Message: {0}", Messsage);
                w.WriteLine("-----------------------------------------------");
            }
        }
    }
}
