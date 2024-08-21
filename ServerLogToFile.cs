
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Utils;


namespace Attendance
{
  public class ServerLogToFile
  {
    private static string m_logFile = string.Empty;
    private static DateTime m_lastDate = Tools.GetDateTimeNow();
    private static StringBuilder sb = new StringBuilder();
    private static object ThreadLock = new object();

    protected static string LogFile
    {
      get
      {
        if (Tools.GetDateTimeNow().Hour != ServerLogToFile.m_lastDate.Hour || string.IsNullOrEmpty(ServerLogToFile.m_logFile))
        {
          ServerLogToFile.m_lastDate = Tools.GetDateTimeNow();
          DirectoryInfo directoryInfo1 = new DirectoryInfo(Path.GetDirectoryName(Application.ExecutablePath) + "\\ServerLog");
          if (!directoryInfo1.Exists)
            directoryInfo1.Create();
          DirectoryInfo directoryInfo2 = new DirectoryInfo(directoryInfo1.FullName + "\\" + ServerLogToFile.m_lastDate.ToString("yyyy-MM-dd"));
          if (!directoryInfo2.Exists)
            directoryInfo2.Create();
          ServerLogToFile.m_logFile = directoryInfo2.FullName + "\\ServerLog" + ServerLogToFile.m_lastDate.Hour.ToString("00") + ".txt";
          if (File.Exists(ServerLogToFile.m_logFile))
            File.Delete(ServerLogToFile.m_logFile);
        }
        return ServerLogToFile.m_logFile;
      }
    }

    public static void WriteLog(string msg) => ServerLogToFile.WriteLog(msg, false);

    public static void WriteLogs(string msg) => ServerLogToFile.WriteLog(msg, true);

    public static void WriteLog(string msg, bool issave)
    {
      if (!Monitor.TryEnter(ServerLogToFile.ThreadLock, 3000))
        return;
      try
      {
        ServerLogToFile.sb.AppendLine(msg);
        if (!(ServerLogToFile.sb.Length > 1000 | issave))
          return;
        StreamWriter streamWriter = new StreamWriter(new FileInfo(ServerLogToFile.LogFile).FullName, true, Encoding.UTF8);
        streamWriter.Write(ServerLogToFile.sb.ToString());
        streamWriter.Flush();
        streamWriter.Close();
        ServerLogToFile.sb = new StringBuilder();
      }
      catch (ArgumentOutOfRangeException ex)
      {
        ServerLogToFile.sb = new StringBuilder();
      }
      catch (Exception ex)
      {
      }
      finally
      {
        Monitor.Exit(ServerLogToFile.ThreadLock);
      }
    }
  }
}
