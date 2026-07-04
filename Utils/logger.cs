using System;
using System.Windows.Forms;

namespace BeamMPTools.Utils
{
    public static class Logger
    {
        public static TextBox? OutputBox
        {
            get;
            set;
        }

        public static void Info(string message)
        {
            Log("INFO", message);
        }

        public static void Error(string message)
        {
            Log("ERROR", message);
        }

        private static void Log(
            string level,
            string message)
        {
            TextBox? box = OutputBox;

            if (box == null)
                return;

            string logEntry =
                $"[{DateTime.Now:HH:mm:ss}] " +
                $"[{level}] {message}" +
                $"{Environment.NewLine}";

            if (box.InvokeRequired)
            {
                box.Invoke(
                    new Action(() =>
                    {
                        box.AppendText(logEntry);
                    }));
            }
            if (box == null) 
            { MessageBox.Show("Logger OutputBox is NULL"); 
              return; 
            }
            else
            {
                box.AppendText(logEntry);
            }
        }
    }
}
