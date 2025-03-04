using AdofaiRater;
using MathNet.Numerics.Distributions;
using Overlayer.Tags;
using Overlayer.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoRating.AdofaiRater
{
    internal class CalculatePP
    {
        public static double PP;
        public static double maxPP;
        public static double finalScore;
        public static double baseScore;
        public static double baseMulti = 1;
        public static double maxScore;
        public static int count;
        public static int countTile;

        public static int pMaxCombo;
        public static int pCombo;
        public static int mMaxCombo;
        public static int mCombo;


        public static double ProcessPP()
        {
            int TotalTile = ADOBase.lm.listFloors.Count;

            scrController instance = scrController.instance;
            scrFloor currFloor = instance.currFloor;
            HitMargin hitMargin = scrMistakesManager.hitMargins[countTile];

            countTile++;
            count++;

            maxScore = Math.Max(300 * (TotalTile - 1), 0);

            if (hitMargin == HitMargin.Perfect) { baseScore += 300; pCombo++; mCombo++; }
            if (hitMargin == HitMargin.EarlyPerfect || hitMargin == HitMargin.LatePerfect) { baseScore += 100; mCombo++; pCombo = 0; }
            if (hitMargin == HitMargin.VeryEarly || hitMargin == HitMargin.VeryLate) { baseScore += 50; mCombo++; pCombo = 0; }
            if (hitMargin == HitMargin.OverPress) { baseScore += 50; mCombo++; pCombo = 0; }
            if (hitMargin == HitMargin.TooEarly || hitMargin == HitMargin.TooLate) { baseMulti *= 0.99; pCombo = 0; }
            if (hitMargin == HitMargin.FailMiss || hitMargin == HitMargin.FailOverload) { baseMulti *= 0.975; mCombo = 0; pCombo = 0; }

            pMaxCombo = Math.Max(pCombo, pMaxCombo);
            mMaxCombo = Math.Max(mCombo, mMaxCombo);

            double XAccMulti;
            if (PPUtils.XAccuracy() >= 99)
            {
                XAccMulti = 1.5 + 1 * (PPUtils.XAccuracy() - 99);
            }
            else if (PPUtils.XAccuracy() >= 95)
            {
                XAccMulti = 1 + 0.125 * (PPUtils.XAccuracy() - 95);
            }
            else if (PPUtils.XAccuracy() >= 90)
            {
                XAccMulti = 0.8 + 0.04 * (PPUtils.XAccuracy() - 90);
            }
            else
            {
                XAccMulti = 0.35 + 0.005 * PPUtils.XAccuracy();
            }

            finalScore =
                (Math.Pow(baseScore, 1.5) / Math.Pow(maxScore, 1.5) * 1000000) *
                (Math.Pow((double)pMaxCombo / (TotalTile - 1), 0.1)) *
                (Math.Pow((double)mMaxCombo / (TotalTile - 1), 1/3)) *
                baseMulti;

            PP = finalScore / 500000 * Math.Pow(RaterMain.globalRating, 3) * XAccMulti;
            maxPP = 2 * Math.Pow(RaterMain.globalRating, 3) * 2.5;

            return PP;
        }

        public static void Reset()
        {
            count = 0;
            PP = 0;
            maxPP = 2 * Math.Pow(RaterMain.globalRating, 3) * 2.5;
            finalScore = 0;
            baseScore = 0;
            pMaxCombo = pCombo = mMaxCombo = mCombo = 0;
            countTile = 0;
            baseMulti = 1;
        }
    }
}
