using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.Rendering;

public static class MacPortBuild
{
    public static void Build()
    {
        var target = NamedBuildTarget.Standalone;
        PlayerSettings.SetArchitecture(target, 1);
        PlayerSettings.SetScriptingBackend(target, ScriptingImplementation.Mono2x);
        PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.StandaloneOSX, false);
        PlayerSettings.SetGraphicsAPIs(BuildTarget.StandaloneOSX, new[] { GraphicsDeviceType.Metal });
        var scenes = EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => s.path).ToArray();
        if (scenes.Length == 0) throw new Exception("No recovered build scenes configured.");
        string output = Path.GetFullPath(Path.Combine(Application.dataPath, "../../..", "builds/YandereSimulator.app"));
        Directory.CreateDirectory(Path.GetDirectoryName(output));
        var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions {
            scenes = scenes, locationPathName = output,
            target = BuildTarget.StandaloneOSX, options = BuildOptions.Development
        });
        Debug.Log("MAC_PORT_BUILD_RESULT: " + report.summary.result);
        if (report.summary.result != BuildResult.Succeeded)
            throw new Exception("macOS build failed: " + report.summary.result);
    }
}
