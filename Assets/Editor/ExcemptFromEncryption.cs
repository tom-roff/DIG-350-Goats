using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.iOS.Xcode;
using UnityEngine;

/// <summary>
/// Marks ITSAppUsesNonExemptEncryption in Unity generated Info.plist file to false. Doing this no longer requires manually marking the app as not using non-exempt encryption in iTunes Connect for Testflight beta testing. https://gist.github.com/TiborUdvari/4679d636b17ddff0d83065eefa399c04
/// Also possible to manually add the field https://stackoverflow.com/a/35895077/441878
/// </summary>
public class ExcemptFromEncryption : IPostprocessBuildWithReport // Will execute after XCode project is built
{
    public int callbackOrder
    {
        get { return 0; }
    }

    public void OnPostprocessBuild(BuildReport report)
    {
        if (report.summary.platform == BuildTarget.iOS) // Check if the build is for iOS
        {
            string plistPath = report.summary.outputPath + "/Info.plist";

            PlistDocument plist = new PlistDocument(); // Read Info.plist file into memory
            plist.ReadFromString(File.ReadAllText(plistPath));

            PlistElementDict rootDict = plist.root;
            rootDict.SetBoolean("ITSAppUsesNonExemptEncryption", false);

            File.WriteAllText(plistPath, plist.WriteToString()); // Override Info.plist
        }
    }
}
