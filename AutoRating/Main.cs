using AdofaiRater;
using HarmonyLib;
using System;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityModManagerNet;

namespace AutoRating
{
    public static class Main
    {
        public static UnityModManager.ModEntry.ModLogger logger;
        public static UnityModManager.ModEntry modEntry;

        public static Harmony harmony;

        // 在 Main 中管理 RatingDisplay 实例
        internal static RatingDisplay ratingDisplay;

        // 控制是否展开面板的标志
        private static bool showPanel = true;

        // 控制评分显示位置和字体大小的滑条变量
        private static float ratingPositionX = 0.1f;
        private static float ratingPositionY = 0.1f;
        private static int ratingFontSize = 30;

        public static void Setup(UnityModManager.ModEntry modEntry)
        {
            LoadAssembly("Mods/AutoRating/MathNet.Numerics.dll");

            Main.modEntry = modEntry;
            logger = modEntry.Logger;

            modEntry.OnToggle = OnToggle;
            modEntry.OnGUI = OnGUI;
        }

        private static bool OnToggle(UnityModManager.ModEntry modEntry, bool value)
        {
            if (value)
            {
                // 启动时实例化 RatingDisplay
                GameObject obj = new GameObject("RatingDisplay");
                ratingDisplay = obj.AddComponent<RatingDisplay>();
                GameObject.DontDestroyOnLoad(obj);
            }
            else
            {
                // 关闭时销毁 RatingDisplay 对象
                if (ratingDisplay != null)
                {
                    UnityEngine.Object.DestroyImmediate(ratingDisplay.gameObject);
                    ratingDisplay = null;
                }
            }

            // 初始化或销毁 Harmony patch
            if (value)
            {
                harmony = new Harmony(modEntry.Info.Id);
                harmony.PatchAll(Assembly.GetExecutingAssembly());
            }
            else
            {
                harmony.UnpatchAll(modEntry.Info.Id);
            }

            return true;
        }

        private static void OnGUI(UnityModManager.ModEntry modEntry)
        {
            // 设置GUI样式
            GUIStyle labelStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = ratingFontSize,  // 使用滑条调整的字体大小
                alignment = TextAnchor.UpperLeft,
                normal = { textColor = Color.white }
            };

            // 创建一个垂直布局容器
            GUILayout.BeginVertical();

            // 添加100px的空间
            GUILayout.Space(100f);

            // 显示评分
            string globalRating = RaterMain.globalRating.ToString("F2");

            // 控制评分位置的X和Y坐标
            ratingPositionX = GUILayout.HorizontalSlider(ratingPositionX, 0f, Screen.width);
            GUILayout.Label($"X Position: {ratingPositionX}", labelStyle);

            ratingPositionY = GUILayout.HorizontalSlider(ratingPositionY, 0f, Screen.height);
            GUILayout.Label($"Y Position: {ratingPositionY}", labelStyle);

            // 控制评分的字体大小
            ratingFontSize = (int)GUILayout.HorizontalSlider(ratingFontSize, 10f, 100f);
            GUILayout.Label($"Font Size: {ratingFontSize}", labelStyle);

            // 更新 RatingDisplay 中的值
            if (ratingDisplay != null)
            {
                ratingDisplay.positionX = ratingPositionX / Screen.width;  // 确保是0-1的范围
                ratingDisplay.positionY = ratingPositionY / Screen.height; // 确保是0-1的范围
                ratingDisplay.fontSize = ratingFontSize;
            }

            // 结束垂直布局容器
            GUILayout.EndVertical();
        }

        private static void LoadAssembly(string path)
        {
            using (FileStream fileStream = new FileStream(path, FileMode.Open))
            {
                byte[] array = new byte[fileStream.Length];
                fileStream.Read(array, 0, array.Length);
                AppDomain.CurrentDomain.Load(array);
            }
        }
    }
}
