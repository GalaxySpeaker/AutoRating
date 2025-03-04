using UnityEngine;
using UnityModManagerNet;

namespace AutoRating
{
    public class Settings : UnityModManager.ModSettings
    {
        // 是否显示文字
        public bool showText = true;

        // 文字位置
        public float textPositionX = 100f;
        public float textPositionY = 100f;

        // 文字大小
        public float textSize = 20f;

        // 文字颜色
        public float textColorR = 1f;
        public float textColorG = 1f;
        public float textColorB = 1f;
        public float textColorA = 1f;

        // 保存设置
        public override void Save(UnityModManager.ModEntry modEntry)
        {
            Save(this, modEntry);
        }

        // 将颜色转换为 Color 类型
        public Color GetTextColor()
        {
            return new Color(textColorR, textColorG, textColorB, textColorA);
        }

        // 设置颜色
        public void SetTextColor(Color color)
        {
            textColorR = color.r;
            textColorG = color.g;
            textColorB = color.b;
            textColorA = color.a;
        }
    }
}