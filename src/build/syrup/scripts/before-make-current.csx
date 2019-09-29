#r "System.IO"
#r "System.Drawing"
#r "System.Windows.Forms"
using System.Windows.Forms;

Console.WriteLine("Execute file: befor-make-current.csx");
var result = new SyrupExecuteResult();
Process[] pname = Process.GetProcessesByName("LukeSearch");
var number = pname.Length;
if (number != 0)
{
    MessageBox.Show(
        "Program LukeSearch jest uruchomiona.\r\nProszę go zamknać a nastepnie ponownie spróbować aktywować danę wersję aplikacji.",
        "Syrup - Error",
        MessageBoxButtons.OK,
        MessageBoxIcon.Error);
        Syrup.ExecuteResult.AbortProcess("LukeSearch is running");

}
