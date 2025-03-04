using UnityEngine;

namespace AutoRating.AdofaiRater
{
    internal class PPUtils
    {
        public static double XAccuracy()
        {
            scrController instance = scrController.instance;
            float? num;
            if (instance == null)
            {
                num = null;
            }
            else
            {
                scrMistakesManager mistakesManager = instance.mistakesManager;
                num = ((mistakesManager != null) ? new float?(mistakesManager.percentXAcc) : null) * (float)100;
            }
            float? num2 = num;
            float num3 = num2.GetValueOrDefault();
            if (double.IsNaN((double)num3))
            {
                num3 = 0f;
            }
            return (double)num3;
        }


        private static readonly Color[] Colors =
    {
        new Color(0f, 0.776f, 1f),       // #00C6FF
        new Color(0f, 1f, 0.984f),      // #00FFFB
        new Color(0.741f, 1f, 0.471f),  // #BDFF78
        new Color(1f, 0.973f, 0.094f),  // #FFF818
        new Color(1f, 0.753f, 0f),       // #FFC000
        new Color(1f, 0.282f, 0.282f),  // #FF4848
        new Color(1f, 0.282f, 0.757f),   // #FF48C1
        new Color(0.745f, 0.31f, 1f),   // #BE4FFF
        new Color(0.604f, 0.22f, 0.843f),// #9A38D7
        new Color(0.369f, 0.133f, 0.506f),// #5E2281
        new Color(0.231f, 0.082f, 0.318f),// #3B1551
        new Color(0f, 0f, 0f)            // #000000
    };

        // 获取渐变颜色
        public static Color GetColor(double rating)
        {
            // 确保 rating 在有效范围内
            if (rating <= 1) return Colors[0];
            if (rating >= 12) return Colors[Colors.Length - 1];

            // 计算插值
            int lowerIndex = (int)rating - 1; // 向下取整
            int upperIndex = lowerIndex + 1;  // 向上取整

            double t = rating - (int)rating; // 计算插值比例
            return Color.Lerp(Colors[lowerIndex], Colors[upperIndex], (float)t);
        }
    }
}
