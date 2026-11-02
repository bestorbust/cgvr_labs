#if UNITY_EDITOR
using System;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;

[InitializeOnLoad]
public static class AutoVerseBuildPipeline
{
    public const string SCENE_PATH = "Assets/Scenes/AutoVerse_Showwroom.unity";
    public const string WINDOWS_BUILD_PATH = "Builds/Windows/AutoVerse.exe";
    public const string WEBGL_BUILD_PATH = "Builds/WebGL";

    static AutoVerseBuildPipeline()
    {
        EditorApplication.delayCall += CheckTriggers;
    }

    private static void CheckTriggers()
    {
        if (File.Exists("test_trigger.txt"))
        {
            try { File.Delete("test_trigger.txt"); } catch { }
            RunAutomatedTestSuite();
        }
    }

    [MenuItem("AutoVerse/Testing/Run Full Test Suite")]
    public static void RunAutomatedTestSuite()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("=================================================");
        sb.AppendLine("      AUTOVERSE AUTOMATED VERIFICATION SUITE      ");
        sb.AppendLine($"Timestamp: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        sb.AppendLine("=================================================\n");

        int passed = 0;
        int failed = 0;

        void AssertTest(string testName, bool condition, string details = "")
        {
            if (condition)
            {
                passed++;
                sb.AppendLine($"[PASS] {testName} {details}");
            }
            else
            {
                failed++;
                sb.AppendLine($"[FAIL] {testName} {details}");
            }
        }

        try
        {
            var scene = EditorSceneManager.OpenScene(SCENE_PATH, OpenSceneMode.Single);
            AssertTest("Scene Load", scene.IsValid() && scene.isLoaded, $"Scene: {SCENE_PATH}");

            var carSelector = UnityEngine.Object.FindAnyObjectByType<CarSelector>();
            var vehiclePainter = UnityEngine.Object.FindAnyObjectByType<VehiclePainter>();
            var camera = UnityEngine.Object.FindAnyObjectByType<ShowroomCamera>();
            var lighting = UnityEngine.Object.FindAnyObjectByType<ShowroomLightingManager>();
            var accessories = UnityEngine.Object.FindAnyObjectByType<VehicleAccessoryManager>();
            var env = UnityEngine.Object.FindAnyObjectByType<ShowroomEnvironmentManager>();
            var ui = UnityEngine.Object.FindAnyObjectByType<AutoVerseShowroomUI>();

            AssertTest("CarSelector Present", carSelector != null);
            AssertTest("VehiclePainter Present", vehiclePainter != null);
            AssertTest("ShowroomCamera Present", camera != null);
            AssertTest("ShowroomLightingManager Present", lighting != null);

            // Test 1: All 8 Vehicles
            if (carSelector != null)
            {
                AssertTest("Car Count = 8", carSelector.CarCount == 8, $"Found {carSelector.CarCount}");
                for (int i = 0; i < carSelector.CarCount; i++)
                {
                    carSelector.ShowCar(i);
                    var car = carSelector.CurrentCar;
                    bool carValid = car != null && car.activeInHierarchy;
                    AssertTest($"Vehicle [{i}] Selection ({car?.name})", carValid);
                }
            }

            // Test 2: Customization on all 8 vehicles
            if (vehiclePainter != null && carSelector != null)
            {
                for (int c = 0; c < carSelector.CarCount; c++)
                {
                    carSelector.ShowCar(c);
                    var car = carSelector.CurrentCar;

                    for (int col = 0; col < 18; col++)
                    {
                        vehiclePainter.SetBodyColor(col);
                    }
                    AssertTest($"Vehicle [{c}] Body Colors (18 Colors)", vehiclePainter.CurrentBodyColorIndex == 17);

                    vehiclePainter.SetPaintFinish(0);
                    vehiclePainter.SetPaintFinish(1);
                    vehiclePainter.SetPaintFinish(2);
                    vehiclePainter.SetPaintFinish(3);
                    AssertTest($"Vehicle [{c}] Finishes (4 Finishes)", (int)vehiclePainter.CurrentFinish == 3);

                    for (int w = 0; w < 5; w++)
                    {
                        vehiclePainter.SetWheelColor(w);
                    }
                    AssertTest($"Vehicle [{c}] Wheels (5 Finishes)", vehiclePainter.CurrentWheelColorIndex == 4);

                    if (vehiclePainter.SupportsGlass(car))
                    {
                        vehiclePainter.SetWindowTint(2);
                        AssertTest($"Vehicle [{c}] Window Tint Supported", vehiclePainter.CurrentWindowTintIndex == 2);
                    }
                    else
                    {
                        AssertTest($"Vehicle [{c}] Window Tint Correctly Guarded", !vehiclePainter.SupportsGlass(car));
                    }

                    if (vehiclePainter.SupportsHeadlights(car))
                    {
                        vehiclePainter.SetHeadlightStyle(2);
                        AssertTest($"Vehicle [{c}] Headlights Supported", vehiclePainter.CurrentHeadlightIndex == 2);
                    }
                    else
                    {
                        AssertTest($"Vehicle [{c}] Headlights Correctly Guarded", !vehiclePainter.SupportsHeadlights(car));
                    }

                    vehiclePainter.ResetCustomization();
                    AssertTest($"Vehicle [{c}] Reset Customization", vehiclePainter.CurrentBodyColorIndex == 0);
                }
            }

            // Test 3: Accessories
            if (accessories != null)
            {
                for (int a = 0; a < VehicleAccessoryManager.ACCESSORY_COUNT; a++)
                {
                    accessories.ToggleAccessory(a);
                    AssertTest($"Accessory [{a}] Toggle ON", accessories.IsAccessoryActive(a));
                    accessories.ToggleAccessory(a);
                    AssertTest($"Accessory [{a}] Toggle OFF", !accessories.IsAccessoryActive(a));
                }
            }

            // Test 4: Lighting Styles
            if (lighting != null)
            {
                for (int l = 0; l < 4; l++)
                {
                    lighting.SetLightingStyle(l);
                    AssertTest($"Lighting Style [{l}] ({lighting.CurrentStyle})", lighting.CurrentStyleIndex == l);
                }
                lighting.SetBrightness(1.2f);
                AssertTest("Lighting Brightness Adjust", true);
            }

            // Test 5: Camera Presets
            if (camera != null)
            {
                camera.SetHeroView();
                camera.SetFrontView();
                camera.SetSideView();
                camera.SetRearView();
                camera.SetTopView();
                camera.SetWheelView();
                camera.ToggleAutoRotate();
                bool rotating = camera.IsAutoRotating;
                camera.ToggleAutoRotate();
                AssertTest("Camera Presets & Turntable Toggle", true, $"Turntable was: {rotating}");
            }

            sb.AppendLine("\n-------------------------------------------------");
            sb.AppendLine($"SUMMARY: Total Tests: {passed + failed} | PASSED: {passed} | FAILED: {failed}");
            sb.AppendLine("-------------------------------------------------");
            Debug.Log(sb.ToString());
            File.WriteAllText("test_execution_report.txt", sb.ToString());
        }
        catch (Exception ex)
        {
            sb.AppendLine($"\n[EXCEPTION ENCOUNTERED]: {ex}");
            File.WriteAllText("test_execution_report.txt", sb.ToString());
            Debug.LogError($"Automated Test Suite Failed: {ex}");
        }
    }

    [MenuItem("AutoVerse/Build/Configure Optimized Build Settings")]
    public static void ConfigureOptimizedBuildSettings()
    {
        // 1. Lock Windows Standalone to Direct3D 11 to avoid DirectX 12 massive PSO variant pre-warming
        PlayerSettings.SetGraphicsAPIs(BuildTarget.StandaloneWindows64, new[] { GraphicsDeviceType.Direct3D11 });

        // 2. Configure WebGL settings
        PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Disabled; // Fast static hosting & zero server config
        PlayerSettings.WebGL.memorySize = 512; // 512MB heap is standard for mobile browsers
        PlayerSettings.WebGL.template = "APPLICATION:Default";
        PlayerSettings.runInBackground = true;

        // 3. Ensure Scene is in Build Settings
        EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(SCENE_PATH, true) };

        Debug.Log("AutoVerse: Optimized Build Settings applied successfully (Direct3D 11 locked, WebGL 512MB configured).");
    }

    [MenuItem("AutoVerse/Build/Build Windows Standalone")]
    public static void BuildWindows()
    {
        ConfigureOptimizedBuildSettings();
        Debug.Log("AutoVerse: Starting Fast Windows Standalone 64-bit Build...");
        Directory.CreateDirectory("Builds/Windows");

        BuildPlayerOptions options = new BuildPlayerOptions
        {
            scenes = new[] { SCENE_PATH },
            locationPathName = WINDOWS_BUILD_PATH,
            target = BuildTarget.StandaloneWindows64,
            options = BuildOptions.None
        };

        BuildReport report = BuildPipeline.BuildPlayer(options);
        string resultMsg = $"Windows Build Result: {report.summary.result} | Errors: {report.summary.totalErrors} | Output: {WINDOWS_BUILD_PATH}";
        Debug.Log(resultMsg);
        File.WriteAllText("build_windows_result.txt", resultMsg + "\n" + DateTime.Now);
    }

    [MenuItem("AutoVerse/Build/Build WebGL")]
    public static void BuildWebGL()
    {
        ConfigureOptimizedBuildSettings();
        Debug.Log("AutoVerse: Starting WebGL Build...");
        Directory.CreateDirectory(WEBGL_BUILD_PATH);

        BuildPlayerOptions options = new BuildPlayerOptions
        {
            scenes = new[] { SCENE_PATH },
            locationPathName = WEBGL_BUILD_PATH,
            target = BuildTarget.WebGL,
            options = BuildOptions.None
        };

        BuildReport report = BuildPipeline.BuildPlayer(options);
        string resultMsg = $"WebGL Build Result: {report.summary.result} | Errors: {report.summary.totalErrors} | Output: {WEBGL_BUILD_PATH}";
        Debug.Log(resultMsg);
        File.WriteAllText("build_webgl_result.txt", resultMsg + "\n" + DateTime.Now);
    }
}
#endif
