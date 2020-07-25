// C# example.
using UnityEditor;
using System.Diagnostics;

public class Linux
{
    public static void BuildGame(string path = null)
    {
        if (string.IsNullOrEmpty(path))
        {
            // Get filename.
            path = EditorUtility.SaveFolderPanel("Choose Location of Built Game", "", "");
        }
        
        string[] levels = new string[] {
            "Assets/Scenes/Login.unity",
            "Assets/Scenes/Game.unity",
            "Assets/Scenes/Options.unity",
        };

        // Build player.
        BuildPipeline.BuildPlayer(levels, path + "/Screeps3D/Screeps3D", BuildTarget.StandaloneLinux64, 
            BuildOptions.Development 
        );
        
        // "C:\Program Files\7-Zip\7zG.exe" a -ttar -so archive.tar Screeps3D | "C:\Program Files\7-Zip\7zG.exe" a -si archive.tgz"

        // Copy a file from the project folder to the build folder, alongside the built game.
        //FileUtil.CopyFileOrDirectory("Assets/Templates/Readme.txt", path + "Readme.txt");

        // Run the game (Process class from System.Diagnostics).
        //Process proc = new Process();
        //proc.StartInfo.FileName = path + "/Screeps3D.exe";
        //proc.Start();
    }

    public static void Release(string path, string version, string output)
    {
        // tar it
        Process proc = new Process();
        proc.StartInfo.FileName = @"C:\Program Files\7-Zip\7zG.exe";
        proc.StartInfo.Arguments = $"a -ttar {output}/archive.tar {path}/*";
        //UnityEngine.Debug.Log(proc.StartInfo.Arguments);
        proc.Start();
        proc.WaitForExit(30000);

        // ball it
        Process ballProc = new Process();
        ballProc.StartInfo.FileName = @"C:\Program Files\7-Zip\7zG.exe";
        ballProc.StartInfo.Arguments = $"a {output}/Screeps3D_{version}_Linux_64.tgz {output}/archive.tar";
        //UnityEngine.Debug.Log(proc.StartInfo.Arguments);
        ballProc.Start();
        ballProc.WaitForExit(30000);

        FileUtil.DeleteFileOrDirectory($"{output}/archive.tar");
    }
}