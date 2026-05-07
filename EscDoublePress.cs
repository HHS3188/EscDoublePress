using System;
using System.Reflection;
using HarmonyLib;
using UnityEngine;
using UnityModManagerNet;

namespace EscDoublePress
{
    public class Settings : UnityModManager.ModSettings
    {
        public int doublePressWindowMs = 500;
        public bool protectCountdown = true;
        public bool verboseLogging = false;
        public int language = 1;

        public override void Save(UnityModManager.ModEntry modEntry)
        {
            UnityModManager.ModSettings.Save(this, modEntry);
        }
    }

    public static class Main
    {
        private const int MinWindowMs = 100;
        private const int MaxWindowMs = 2000;

        public static Settings settings;
        public static UnityModManager.ModEntry modEntry;

        private static Harmony harmony;
        private static bool enabled;
        private static bool hasFirstPress;
        private static float firstPressTime = -999f;

        public static bool IsEnabled
        {
            get { return enabled; }
        }

        public static bool Load(UnityModManager.ModEntry entry)
        {
            modEntry = entry;
            settings = UnityModManager.ModSettings.Load<Settings>(entry);
            ClampSettings();

            entry.OnToggle = OnToggle;
            entry.OnGUI = OnGUI;
            entry.OnSaveGUI = OnSaveGUI;

            entry.Logger.Log("EscDoublePress loaded.");
            return true;
        }

        private static bool OnToggle(UnityModManager.ModEntry entry, bool value)
        {
            try
            {
                if (value)
                {
                    if (harmony == null)
                    {
                        harmony = new Harmony(entry.Info.Id);
                        harmony.PatchAll(Assembly.GetExecutingAssembly());
                    }

                    enabled = true;
                    ResetState();
                    entry.Logger.Log("EscDoublePress enabled.");
                }
                else
                {
                    enabled = false;
                    ResetState();

                    if (harmony != null)
                    {
                        harmony.UnpatchAll(entry.Info.Id);
                        harmony = null;
                    }

                    entry.Logger.Log("EscDoublePress disabled.");
                }

                return true;
            }
            catch (Exception ex)
            {
                enabled = false;
                entry.Logger.Error("EscDoublePress failed to toggle.");
                entry.Logger.LogException(ex);
                return false;
            }
        }

        private static void OnGUI(UnityModManager.ModEntry entry)
        {
            ClampSettings();

            GUILayout.BeginVertical();
            DrawLanguageSelector();
            GUILayout.Space(4);

            GUILayout.Label(Text("title"));
            GUILayout.Space(4);

            GUILayout.BeginHorizontal();
            GUILayout.Label(Text("window"), GUILayout.Width(210));
            string input = GUILayout.TextField(settings.doublePressWindowMs.ToString(), GUILayout.Width(80));
            int parsed;
            if (int.TryParse(input, out parsed))
            {
                settings.doublePressWindowMs = Clamp(parsed, MinWindowMs, MaxWindowMs);
            }
            GUILayout.Label(Text("range") + MinWindowMs + "-" + MaxWindowMs + " ms");
            GUILayout.EndHorizontal();

            settings.protectCountdown = GUILayout.Toggle(settings.protectCountdown, Text("protectCountdown"));
            settings.verboseLogging = GUILayout.Toggle(settings.verboseLogging, Text("verboseLogging"));

            GUILayout.Space(4);
            GUILayout.Label(Text("description"));
            GUILayout.EndVertical();
        }

        private static void OnSaveGUI(UnityModManager.ModEntry entry)
        {
            ClampSettings();
            settings.Save(entry);
        }

        public static bool ShouldBlockEscapePause(scrController controller)
        {
            if (!enabled || settings == null || controller == null)
            {
                ResetState();
                return false;
            }

            if (!IsProtectedGameplayState(controller))
            {
                ResetState();
                return false;
            }

            if (!IsKeyboardEscapePress())
            {
                return false;
            }

            return ShouldBlockCurrentEscape();
        }

        public static bool ShouldBlockEditorSwitchToEditMode(scnEditor editor, bool clsToEditor)
        {
            if (!enabled || settings == null || editor == null || clsToEditor)
            {
                ResetState();
                return false;
            }

            scrController controller = ADOBase.controller;
            if (controller == null || !ADOBase.isLevelEditor || controller.paused)
            {
                ResetState();
                return false;
            }

            if (!IsKeyboardEscapePress())
            {
                return false;
            }

            return ShouldBlockCurrentEscape();
        }

        public static void ResetState()
        {
            hasFirstPress = false;
            firstPressTime = -999f;
        }

        private static bool IsProtectedGameplayState(scrController controller)
        {
            if (controller.paused || !controller.gameworld)
            {
                return false;
            }

            States state = controller.currentState;
            if (state == States.PlayerControl)
            {
                return true;
            }

            if (settings.protectCountdown && (state == States.Countdown || state == States.Checkpoint))
            {
                return true;
            }

            return false;
        }

        private static bool ShouldBlockCurrentEscape()
        {
            float now = Time.unscaledTime;
            float windowSeconds = settings.doublePressWindowMs / 1000f;

            if (hasFirstPress && now - firstPressTime <= windowSeconds)
            {
                ResetState();
                Verbose("Second Esc accepted.");
                return false;
            }

            hasFirstPress = true;
            firstPressTime = now;
            Verbose("First Esc blocked.");
            return true;
        }

        private static bool IsKeyboardEscapePress()
        {
            try
            {
                if (RDInput.keyboardInput != null && RDInput.keyboardInput.isActive && RDInput.keyboardInput.Cancel())
                {
                    return true;
                }

                if (RDInput.asyncKeyboardMouseInput != null && RDInput.asyncKeyboardMouseInput.isActive && RDInput.asyncKeyboardMouseInput.Cancel())
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                Verbose("Keyboard cancel detection failed: " + ex.Message);
            }

            return Input.GetKeyDown(KeyCode.Escape);
        }

        private static void ClampSettings()
        {
            if (settings == null)
            {
                settings = new Settings();
            }

            settings.doublePressWindowMs = Clamp(settings.doublePressWindowMs, MinWindowMs, MaxWindowMs);
            settings.language = Clamp(settings.language, 0, 2);
        }

        private static int Clamp(int value, int min, int max)
        {
            if (value < min)
            {
                return min;
            }

            if (value > max)
            {
                return max;
            }

            return value;
        }

        private static void Verbose(string message)
        {
            if (settings != null && settings.verboseLogging && modEntry != null)
            {
                modEntry.Logger.Log(message);
            }
        }

        private static void DrawLanguageSelector()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(Text("language"), GUILayout.Width(80));
            if (GUILayout.Toggle(settings.language == 0, "中文", "Button", GUILayout.Width(96)))
            {
                settings.language = 0;
            }

            if (GUILayout.Toggle(settings.language == 1, "EN", "Button", GUILayout.Width(96)))
            {
                settings.language = 1;
            }

            if (GUILayout.Toggle(settings.language == 2, "KR", "Button", GUILayout.Width(96)))
            {
                settings.language = 2;
            }

            GUILayout.EndHorizontal();
        }

        private static string Text(string key)
        {
            int lang = settings != null ? settings.language : 0;
            if (lang == 1)
            {
                return TextEnglish(key);
            }

            if (lang == 2)
            {
                return TextKorean(key);
            }

            return TextChinese(key);
        }

        private static string TextChinese(string key)
        {
            switch (key)
            {
                case "language":
                    return "语言:";
                case "title":
                    return "Esc 双击暂停";
                case "window":
                    return "双击判定窗口 (毫秒):";
                case "range":
                    return "范围: ";
                case "protectCountdown":
                    return "倒计时/检查点状态下也需要双击 Esc";
                case "verboseLogging":
                    return "记录被拦截/放行的 Esc 日志";
                case "description":
                    return "官方关卡和编辑器播放测试中，只有在判定窗口内第二次按 Esc 才会暂停或返回编辑。UI 按钮和真实失败判定不会被修改。";
                default:
                    return key;
            }
        }

        private static string TextEnglish(string key)
        {
            switch (key)
            {
                case "language":
                    return "Language:";
                case "title":
                    return "Esc Double Press";
                case "window":
                    return "Double press window (ms):";
                case "range":
                    return "Range: ";
                case "protectCountdown":
                    return "Also require double Esc during countdown/checkpoint states";
                case "verboseLogging":
                    return "Verbose log blocked/allowed Esc presses";
                case "description":
                    return "In official gameplay and editor playtest, Esc only pauses or returns to edit mode after a second press within the window. UI buttons and real gameplay failures are not patched.";
                default:
                    return key;
            }
        }

        private static string TextKorean(string key)
        {
            switch (key)
            {
                case "language":
                    return "언어:";
                case "title":
                    return "Esc 두 번 누르기";
                case "window":
                    return "두 번 누르기 판정 시간 (ms):";
                case "range":
                    return "범위: ";
                case "protectCountdown":
                    return "카운트다운/체크포인트 상태에서도 Esc 두 번 누르기 필요";
                case "verboseLogging":
                    return "차단/허용된 Esc 입력 로그 기록";
                case "description":
                    return "공식 플레이와 에디터 테스트 플레이에서는 시간 내에 Esc를 두 번째로 눌러야 일시정지되거나 편집 모드로 돌아갑니다. UI 버튼과 실제 실패 판정은 수정하지 않습니다.";
                default:
                    return key;
            }
        }
    }

    [HarmonyPatch(typeof(scrController), "TogglePauseGame")]
    public static class TogglePauseGamePatch
    {
        public static bool Prefix(scrController __instance, ref bool __result)
        {
            if (Main.ShouldBlockEscapePause(__instance))
            {
                __result = __instance != null && __instance.paused;
                return false;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(scrController), "Start")]
    public static class ControllerStartPatch
    {
        public static void Postfix()
        {
            Main.ResetState();
        }
    }

    [HarmonyPatch(typeof(scnEditor), "SwitchToEditMode")]
    public static class EditorSwitchToEditModePatch
    {
        public static bool Prefix(scnEditor __instance, bool clsToEditor)
        {
            return !Main.ShouldBlockEditorSwitchToEditMode(__instance, clsToEditor);
        }
    }
}
