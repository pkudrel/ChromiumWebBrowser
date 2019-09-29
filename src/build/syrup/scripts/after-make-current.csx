#r "System.IO"

Console.WriteLine("Execute file: after-make-current.csx");

string mainDirPath = System.IO.Path.Combine(Syrup.CurrentAppPath, "main");
string mainExePath = System.IO.Path.Combine(mainDirPath, "LukeSearch.exe");
string fileEditorExePath = System.IO.Path.Combine(mainDirPath, "LukeSearch.FileEditor.exe");
string appDirPath = System.IO.Directory.GetParent(Syrup.CurrentAppPath).FullName;
string workDirPath = System.IO.Directory.GetParent(appDirPath).FullName;
string mainLinkPath = System.IO.Path.Combine(workDirPath, "LukeSearch.lnk");
string fileEditorLinkPath = System.IO.Path.Combine(workDirPath, "FileEditor.lnk");

string appName = Syrup.CurrentAppName;

// create shortcut - LukeSearch
Syrup.CreateShortcut(mainExePath, mainDirPath, mainLinkPath, "Link to " + appName);
// create shortcut - LukeSearch.FileEditor.exe 
Syrup.CreateShortcut(fileEditorExePath, mainDirPath, fileEditorLinkPath, "Link to FileEditor");
