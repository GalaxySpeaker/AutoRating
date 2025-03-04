using AdofaiRater;
using AutoRating.AdofaiRater;
using HarmonyLib;
using Overlayer;
using UnityEngine;

namespace AutoRating
{


    public static class Patch
    {
        public static double CalculatedFinalLevelRating = 0;
        public static double PP = 0;

        private static bool IsOverlayerEnabled => AccessTools.TypeByName("TaggedText") != null;

        [HarmonyPatch(typeof(scnGame), "LoadLevel")]
        public static class scnGame_LoadLevel
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                CalculatedFinalLevelRating = RaterMain.ProcessRatings(scnGame.instance.levelPath);
                CalculatePP.Reset();
            }
        }

        [HarmonyPatch(typeof(scrController), "Hit")]
        public static class scrController_Hit
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                CalculatePP.count++;
                if (CalculatePP.count != 0)
                {
                    PP = CalculatePP.ProcessPP();
                }
            }
        }

        [HarmonyPatch(typeof(scnGame), "Play")]
        public static class scnGame_Play
        {
            [HarmonyPostfix]
            private static void Postfix()
            {
                CalculatePP.Reset();
            }
        }

        public static void Postfix(TaggedText __instance)
        {

            if (!IsOverlayerEnabled)
                return;

            string text = __instance.replacedText;


            // AutoRating 详情
            UnityEngine.Color color = PPUtils.GetColor(RaterMain.globalRating);
            string colorHex = ColorUtility.ToHtmlStringRGB(color);
            string colorHex2 = (RaterMain.globalRating >= 10) ? "FFFF00" : "FFFFFF";
            string background = $"<color=#{colorHex}>█</color>";

            string newText = text
                .Replace("{AutoRating}", $"<color=#{colorHex2}>{Patch.CalculatedFinalLevelRating:F2}</color>")
                .Replace("{AutoRatingBG}", $"<color=#{colorHex}>{background}</color>")
                .Replace("{SpeedRating}", RaterMain.globalSpeedRating.ToString("F2"))
                .Replace("{TechRating}", RaterMain.globalTechRating.ToString("F2"))
                .Replace("{RhythmRating}", RaterMain.globalRhythmRating.ToString("F2"))
                .Replace("{StaminaRating}", RaterMain.globalStaminaRating.ToString("F2"))
                .Replace("{PP}", CalculatePP.PP.ToString("F2"))
                .Replace("{maxPP}", CalculatePP.maxPP.ToString("F2"))
                .Replace("{FinalScore}", CalculatePP.finalScore.ToString("F0"))
                .Replace("{BaseMulti}", CalculatePP.baseMulti.ToString("F2")) 
                .Replace("{BaseScore}", CalculatePP.baseScore.ToString())
                .Replace("{MaxScore}", CalculatePP.maxScore.ToString())
                .Replace("{PMaxCombo}", CalculatePP.pMaxCombo.ToString("F0"))
                .Replace("{PCombo}", CalculatePP.pCombo.ToString("F0"))
                .Replace("{MMaxCombo}", CalculatePP.mMaxCombo.ToString("F0"))
                .Replace("{MCombo}", CalculatePP.mCombo.ToString("F0"));

        }
    }
}
