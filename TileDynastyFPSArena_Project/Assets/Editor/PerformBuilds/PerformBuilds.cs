using UnityEditor;

public class PerformBuilds
{

	[MenuItem("Build/Build OSX 64")]
	public static void OSXBuild () {

		string[] scenes = {
			"Assets/Scenes/0_Preloader.unity",
			"Assets/Scenes/1_BeginScene.unity",
			"Assets/Scenes/2_SECTOR2.unity",
			"Assets/Scenes/3_HomeScene.unity",
		};

		BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
		buildPlayerOptions.scenes = scenes;
		buildPlayerOptions.locationPathName = "/Users/petertan/Documents/Unity_Projects/TileDynasty/Builds/TileDynasty_OSX.app";
        buildPlayerOptions.target = BuildTarget.StandaloneOSX;
        buildPlayerOptions.options = BuildOptions.None;
		BuildPipeline.BuildPlayer (buildPlayerOptions);
	}

	[MenuItem("Scripts Only Build/Build OSX 64")]
	public static void OSXBuild_ScriptsOnly () {

		string[] scenes = {
			"Assets/Scenes/0_Preloader.unity",
			"Assets/Scenes/1_BeginScene.unity",
			"Assets/Scenes/2_SECTOR2.unity",
			"Assets/Scenes/3_HomeScene.unity",
		};

		BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
		buildPlayerOptions.scenes = scenes;
		buildPlayerOptions.locationPathName = "/Users/petertan/Documents/Unity_Projects/TileDynasty/Builds/TileDynasty_OSX.app";
        buildPlayerOptions.target = BuildTarget.StandaloneOSX;
        buildPlayerOptions.options = BuildOptions.BuildScriptsOnly;
		BuildPipeline.BuildPlayer (buildPlayerOptions);
	}

	[MenuItem("Build/Build OSX 64")]
	public static void WindowsBuild () {

		string[] scenes = {
			"Assets/Scenes/0_Preloader.unity",
			"Assets/Scenes/1_BeginScene.unity",
			"Assets/Scenes/2_SECTOR2.unity",
			"Assets/Scenes/3_HomeScene.unity",
		};

		BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
		buildPlayerOptions.scenes = scenes;
		buildPlayerOptions.locationPathName = "/Users/petertan/Documents/Unity_Projects/TileDynasty/Builds/TileDynasty_Windows.exe";
		buildPlayerOptions.target = BuildTarget.StandaloneWindows64;
		buildPlayerOptions.options = BuildOptions.None;
		BuildPipeline.BuildPlayer (buildPlayerOptions);
	}

	[MenuItem("Scripts Only Build/Build OSX 64")]
	public static void WindowsBuild_ScriptsOnly () {

		string[] scenes = {
			"Assets/Scenes/0_Preloader.unity",
			"Assets/Scenes/1_BeginScene.unity",
			"Assets/Scenes/2_SECTOR2.unity",
			"Assets/Scenes/3_HomeScene.unity",
		};

		BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
		buildPlayerOptions.scenes = scenes;
		buildPlayerOptions.locationPathName = "/Users/petertan/Documents/Unity_Projects/TileDynasty/Builds/TileDynasty_Windows.exe";
		buildPlayerOptions.target = BuildTarget.StandaloneWindows64;
		buildPlayerOptions.options = BuildOptions.BuildScriptsOnly;
		BuildPipeline.BuildPlayer (buildPlayerOptions);
	}

	[MenuItem("Build/OSX 64 + Windows 64 ")]
	public static void WindowsOSXBuild () {

		string[] scenes = {
			"Assets/Scenes/0_Preloader.unity",
			"Assets/Scenes/1_BeginScene.unity",
			"Assets/Scenes/2_SECTOR2.unity",
			"Assets/Scenes/3_HomeScene.unity",
		};

		BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
		buildPlayerOptions.scenes = scenes;
		buildPlayerOptions.locationPathName = "/Users/petertan/Documents/Unity_Projects/TileDynasty/Builds/TileDynasty_OSX.app";
        buildPlayerOptions.target = BuildTarget.StandaloneOSX;
        buildPlayerOptions.options = BuildOptions.None;
		BuildPipeline.BuildPlayer (buildPlayerOptions);
		BuildPlayerOptions buildPlayerOptionsWindows = new BuildPlayerOptions();
		buildPlayerOptionsWindows.scenes = scenes;
		buildPlayerOptionsWindows.locationPathName = "/Users/petertan/Documents/Unity_Projects/TileDynasty/Builds/TileDynasty_Windows.exe";
		buildPlayerOptionsWindows.target = BuildTarget.StandaloneWindows64;
		buildPlayerOptionsWindows.options = BuildOptions.None;
		BuildPipeline.BuildPlayer (buildPlayerOptionsWindows);

	}

    [MenuItem("Build/PC - OSX 64 + Windows 64 ")]
    public static void PC_WindowsOSXBuild()
    {

        string[] scenes = {
            "Assets/Scenes/0_Preloader.unity",
            "Assets/Scenes/1_BeginScene.unity",
            "Assets/Scenes/2_SECTOR2.unity",
            "Assets/Scenes/3_HomeScene.unity",
        };

        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = scenes;
        buildPlayerOptions.locationPathName = @"C:\Users\Peter Tan\Documents\Purpl3grapestudios\Unity\TileDynasty\Builds\TileDynasty_OSX.app";
        buildPlayerOptions.target = BuildTarget.StandaloneOSX;
        buildPlayerOptions.options = BuildOptions.None;
        BuildPipeline.BuildPlayer(buildPlayerOptions);

        BuildPlayerOptions buildPlayerOptionsWindows = new BuildPlayerOptions();
        buildPlayerOptionsWindows.scenes = scenes;
        buildPlayerOptionsWindows.locationPathName = @"C:\Users\Peter Tan\Documents\Purpl3grapestudios\Unity\TileDynasty\Builds\TileDynasty_Windows.exe";
        buildPlayerOptionsWindows.target = BuildTarget.StandaloneWindows64;
        buildPlayerOptionsWindows.options = BuildOptions.None;
        BuildPipeline.BuildPlayer(buildPlayerOptionsWindows);

    }


}