using System.IO;
using System.Runtime.CompilerServices;
using Aspenlaub.Net.GitHub.CSharp.Vishizhukel.Interfaces.Application;

[assembly: InternalsVisibleTo("Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Integration.Test")]
namespace Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.Helpers;

public class ApplicationLogger : IApplicationLogger {
    internal static string LogFileName = @"C:\Temp\WebView2ApplicationLogger.log";
    private static readonly object LogFileLocker = new();

    public ApplicationLogger() {
        lock (LogFileLocker) {
            if (!File.Exists(LogFileName)) { return; }

            File.WriteAllText(LogFileName, LogFileName + Environment.NewLine);
            File.Copy(LogFileName, LogFileName.Replace(".log", ".cpy"), true);
        }
    }

    public void LogMessage(string message) {
        var timeStamp = DateTime.Now;
        lock (LogFileLocker) {
            File.AppendAllText(LogFileName, timeStamp.ToString("HH:mm:ss.fff") + @" " + message + Environment.NewLine);
            try {
                File.Copy(LogFileName, LogFileName.Replace(".log", ".cpy"), true);
                // ReSharper disable once EmptyGeneralCatchClause
            } catch {
            }
        }
    }
}