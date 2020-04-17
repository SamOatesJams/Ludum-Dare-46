#addin nuget:?package=Cake.Unity3D&version=0.7.0

var _target = Argument("target", "Build");
var _platform = Argument("platform", "WIN64");
var _gameName = Argument("game_name", "Ludumdare46");

var _itchBulterLocation = Argument("itch_butler_location", "itch.io/butler.exe");
var _itchIoProject = Argument("itch_project", "samoatesgames/ludumdare-46");
var _itchBranch = Argument("itch_branch", "windows-dev");

var _testMode = Argument("test_mode", "playmode");

string GetGitCommitVersion()
{
  //git log -1 --format="%h" origin/master
  var processSettings = new ProcessSettings
  {
    Arguments = "log -1 --format=\"%h\" origin/master",
    RedirectStandardOutput = true
  };
  
  using (var process = StartAndReturnProcess("git", processSettings))
  {
    process.WaitForExit();
    
    var output = process.GetStandardOutput().ToArray();
    return output[0];
  }
}

bool GetPlatformSettings(string platformKey, out string outputPath, out string outputFolder, out Unity3DBuildPlatform platform)
{
  // Get the current directory.
  var currentPath = System.IO.Path.GetFullPath("./");
  
  // The location we want the build application to go
  outputFolder = System.IO.Path.Combine(currentPath, $"build-{platformKey.ToLower()}");
  
  switch(platformKey.ToUpper())
  {
    case "WIN64":
    {
      platform = Unity3DBuildPlatform.StandaloneWindows64;
      outputPath = System.IO.Path.Combine(outputFolder, $"{_gameName}.exe");
      break;
    }
    
    case "WEBGL":
    {
      platform = Unity3DBuildPlatform.WebGL;
      outputPath = System.IO.Path.Combine(outputFolder, $"{_gameName}");
      break;
    }
    
    default:
    {
      platform = Unity3DBuildPlatform.StandaloneWindows64;
      outputFolder = "";
      outputPath = "";
      return false;
    }
  }
  
  return true;
}

bool GetTestSettings(string testModeKey, out Unity3DTestMode testMode)
{
  switch (testModeKey.ToUpper())
  {
    case "EDITMODE":
    {
      testMode = Unity3DTestMode.EditMode;
      break;
    }
    
    case "PLAYMODE":
    {
      testMode = Unity3DTestMode.PlayMode;
      break;
    }
    
    default:
    {
      testMode = Unity3DTestMode.EditMode;
      return false;
    }
  }
  
  return true;
}

void CleanFolder(string folder)
{
  var dir = new DirectoryInfo(folder);
  foreach(var file in dir.GetFiles())
  {
    Information($" - Deleting '{file.FullName}'");
    file.Delete();
  }

  foreach (var directory in dir.GetDirectories())
  {
    CleanFolder(directory.FullName);
    directory.Delete();
  }
}

Task("Clean")
  .Does(() =>
{
  string outputPath, outputFolder;
  Unity3DBuildPlatform platform;
  if (!GetPlatformSettings(_platform, out outputPath, out outputFolder, out platform))
  {
    throw new Exception($"Failed to get platform settings for the platform '{_platform}'.");
  }
  
  if (!System.IO.Directory.Exists(outputFolder))
  {
    Information($"'{outputFolder}' doesn't exist, nothing to clean.");
    return;
  }
  
  Information($"Cleaning the output directory '{outputFolder}'.");
  CleanFolder(outputFolder);
  Information($"Cleaned the output directory '{outputFolder}'.");
});

Task("Build")
  .IsDependentOn("Clean")
  .Does(() =>
{
  // Presuming the build.cake file is within a folder inside the Unity3D project folder.
  var projectPath = System.IO.Path.GetFullPath("../");
  
  string outputPath, outputFolder;
  Unity3DBuildPlatform platform;
  if (!GetPlatformSettings(_platform, out outputPath, out outputFolder, out platform))
  {
    throw new Exception($"Failed to get platform settings for the platform '{_platform}'.");
  }
  
  // Get the version of Unity used by the Unity project
  string unityEditorVersion;
  if (!TryGetUnityVersionForProject(projectPath, out unityEditorVersion))
  {
    Error($"Failed to find Unity version for the project '{projectPath}'");
    return;
  }
  
  // Get the absolute path to the Unity3D editor.
  string unityEditorLocation;
  if (!TryGetUnityInstall(unityEditorVersion, out unityEditorLocation)) 
  {
    Error($"Failed to find '{unityEditorVersion}' install location, installed versions are:");
    foreach(var version in GetAllUnityInstalls().Keys)
    {
      Error($" - {version}");
    }
    return;
  }
  
  Information($"Using Unity editor located at '{unityEditorLocation}'");
  
  // Create our build options.
  var options = new Unity3DBuildOptions()
  {
    Platform = platform,
    OutputPath = outputPath,
    UnityEditorLocation = unityEditorLocation,
    ForceScriptInstall = true,
    BuildVersion = $"version-{GetGitCommitVersion()}"
  };
  
  // Perform the Unity3d build.
  BuildUnity3DProject(projectPath, options);
});

Task("Test")
  .Does(() =>
{
  // Presuming the build.cake file is within a folder inside the Unity3D project folder.
  var projectPath = System.IO.Path.GetFullPath("../");
  var testResultOutputPath = System.IO.Path.Combine(projectPath, "test_results.xml");
  
  Unity3DTestMode testMode;
  if (!GetTestSettings(_testMode, out testMode))
  {
    throw new Exception($"Failed to get test settings for the test mode '{_testMode}'.");
  }
  
  string unityEditorVersion;
  if (!TryGetUnityVersionForProject(projectPath, out unityEditorVersion))
  {
    Error($"Failed to find Unity version for the project '{projectPath}'");
    return;
  }
  
  string unityEditorLocation;
  if (!TryGetUnityInstall(unityEditorVersion, out unityEditorLocation))
  {
    Error($"Failed to find '{unityEditorVersion}' install location, installed versions are:");
    foreach(var version in GetAllUnityInstalls().Keys)
    {
      Error($" - {version}");
    }
    return;
  }
  
  Information($"Using Unity editor located at '{unityEditorLocation}'");
  
  var options = new Unity3DTestOptions()
  {
    TestMode = testMode,
    TestResultOutputPath = testResultOutputPath,
    UnityEditorLocation = unityEditorLocation
  };
  
  TestUnity3DProject(projectPath, options);
});

Task("ItchPublish")
  .IsDependentOn("Build")
  .Does(() =>
{
  string outputPath, outputFolder;
  Unity3DBuildPlatform platform;
  if (!GetPlatformSettings(_platform, out outputPath, out outputFolder, out platform))
  {
    throw new Exception($"Failed to get platform settings for the platform '{_platform}'.");
  }
  
  var version = GetGitCommitVersion();
  
  var processSettings = new ProcessSettings
  {
    Arguments = $"push --userversion={version} \"{outputFolder}\" \"{_itchIoProject}:{_itchBranch}\"",
    RedirectStandardOutput = true
  };
  
  using (var process = StartAndReturnProcess(_itchBulterLocation, processSettings))
  {
    process.WaitForExit();
    foreach (var line in process.GetStandardOutput())
    {
      Information(line);
    }
  }
});

RunTarget(_target);