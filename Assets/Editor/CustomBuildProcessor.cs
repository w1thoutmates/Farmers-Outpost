using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class CustomBuildProcessor : IPreprocessBuildWithReport
{
    public int callbackOrder { get; }
    public void OnPreprocessBuild(BuildReport report)
    {
        var db = Resources.Load<Database>("Database");
        db.SetItemIDs();
        Debug.Log("OnPreprocessBuild for target " + report.summary.platform + " at path " + report.summary.outputPath);
    }
}
