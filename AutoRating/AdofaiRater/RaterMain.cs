using AutoRating.AdofaiRater.Objects;
using MathNet.Numerics.Interpolation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdofaiRater
{
    class RaterMain
    {
        public static double globalRating;
        public static double globalSpeedRating;
        public static double globalTechRating;
        public static double globalRhythmRating;
        public static double globalStaminaRating;
        public static double ProcessRatings(string path)
        {
            AdofaiLevel level = RaterUtils.ConvertToLinear(path);

            List<AdofaiTile> tiles = level.Tiles;
            var groups = GroupTiles(tiles);
            var groupsM = GroupTilesM(tiles);

            List<double> speedRatings = new List<double>();
            for (int i = 0; i < groups.Count - 1; i++)
            {
                speedRatings.Add(CalculateSingleSpeedRating(groups[i], groups[i + 1]));
            }

            List<double> techRatings = new List<double>();
            for (int i = 0; i < groupsM.Count - 1; i++)
            {
                techRatings.Add(GetTechSingleRating(groupsM[i], groupsM[i + 1]));
            }

            double alpha = 1 + Math.Pow(5 + Math.Pow(level.TotalLength - 60, 1.0 / 3), 0.75);
            double beta = Math.Min(1, 1 + 0.015 * (level.TotalLength - 60));

            double finalSpeed = CalculateWeightedAverage(speedRatings, alpha);
            double techRating = CalculateWeightedAverage(techRatings, alpha);
            double finalTech = 150 * Math.Pow((techRating - 1),0.75);

            var rhythmMultipliers = CalculateRhythmMultipliers(groups, speedRatings);
            var penalizedRhythm = DetectAndPenalizeRepetitions(rhythmMultipliers, 16);
            double finalRhythm = 30 * Math.Pow(finalSpeed, 0.25) * (CalculateWeightedAverage(penalizedRhythm, alpha) - 1);

            double staminaRating = Math.Pow(1.5, alpha) / 10 * Math.Pow((tiles.LongCount() / level.TotalLength),0.5);

            var finalRatings = new List<double> { finalRhythm, finalSpeed, staminaRating }
                .OrderByDescending(d => d).ToList();

            double totalRating = beta * (finalRatings[0] * 0.95 + finalRatings[1] * 1.0 +
                                       finalRatings[2] * 1.05) / 3.0;



            globalSpeedRating = finalSpeed;
            globalTechRating = finalTech;
            globalRhythmRating = finalRhythm;
            globalStaminaRating = staminaRating;

            RaterMain.globalRating = totalRating;

            return totalRating;
        }

        public static List<List<AdofaiTile>> GroupTiles(List<AdofaiTile> tiles)
        {
            List<List<AdofaiTile>> groups = new List<List<AdofaiTile>>();
            int i = 0;

            while (i < tiles.Count)
            {
                AdofaiTile current = tiles[i];
                double endTime = current.TileTime + Math.Max(current.HitWindowP, 35);
                List<AdofaiTile> group = new List<AdofaiTile> { current };

                for (int j = i + 1; j < tiles.Count; j++)
                {
                    if (tiles[j].TileTime <= endTime)
                        group.Add(tiles[j]);
                    else
                        break;
                }
                groups.Add(group);
                i += group.Count;
            }
            return groups;
        }

        public static double CalculateSingleSpeedRating(List<AdofaiTile> group, List<AdofaiTile> nextGroup)
        {
            if (group.Count == 0 || nextGroup.Count == 0) return 0;

            double groupStart = group[0].TileTime;
            double groupEnd = group.Last().TileTime;
            double nextStart = nextGroup[0].TileTime;

            double realGroupTime = groupEnd - groupStart;
            double interval = nextStart - groupStart;

            if (interval <= 0) interval = 1000;
            double penalty = 1.0;

            if (group.Count > 1)
            {
                double spacing = realGroupTime / interval;
                if (spacing >= 0.25 && spacing < 0.5)
                    penalty = Math.Pow(spacing + 0.5, 0.5);
                else if (spacing >= 0.125 && spacing < 0.25)
                    penalty = Math.Pow(spacing + 0.5, 0.25);
                else
                    penalty = Math.Pow(spacing + 0.5, 0.1);
            }

            return (500 / (interval / Math.Sqrt(group.Count))) * penalty;
        }

        public static double CalculateWeightedAverage(List<double> data, double alpha)
        {

            double threshold = data.Average();
            double total = 0;
            double weightSum = 0;

            foreach (double value in data)
            {
                double weight = Math.Pow(value / threshold, alpha);
                total += value * weight;
                weightSum += weight;
            }

            return weightSum == 0 ? 0 : total / weightSum;
        }

        public static List<double> CalculateRhythmMultipliers(List<List<AdofaiTile>> groups, List<double> ratingSingleSpeeds)
        {
            List<double> multipliers = new List<double>();
            for (int i = 1; i < groups.Count - 1; i++)
            {
                double ratingSingleSpeed = ratingSingleSpeeds[i];
                double bpmMultiplier = GetBpmMultiplier(ratingSingleSpeed);
                double intervalRatio = GetGroupInterval(
                    groups[i - 1],
                    groups[i],
                    groups[i + 1]
                );

                double totalRhythmMultiplier = 1.0;
                if (intervalRatio <= 0.92 || intervalRatio >= 1.08)
                {
                    double outputY = CalculateExpectedFunctionValue(intervalRatio);
                    totalRhythmMultiplier = bpmMultiplier * outputY;
                }

                multipliers.Add(totalRhythmMultiplier);
            }
            return multipliers;
        }

        public static double CalculateExpectedFunctionValue(double ratio)
        {
            ratio = Math.Min(1.0, ratio);
            double[] xPoints = { 0, 0.125, 0.2, 0.252, 0.28, 0.32, 0.42, 0.5,
                       0.58, 0.68, 0.72, 0.748, 0.8, 0.875, 1 };
            double[] yPoints = { 1, 1.05, 1.2, 1.05, 1.25, 1.15, 1.225, 1.0,
                       1.225, 1.15, 1.25, 1.1, 1.2, 1.05, 1 };

            // Use Akima spline with clamped inputs
            var spline = CubicSpline.InterpolateAkimaSorted(xPoints, yPoints);
            return spline.Interpolate(ratio);
        }

        public static List<double> DetectAndPenalizeRepetitions(List<double> sequence, int maxWindowSize)
        {
            List<double> penalized = new List<double>(sequence);
            int n = penalized.Count;

            for (int windowSize = 2; windowSize <= maxWindowSize && windowSize <= n / 2; windowSize++)
            {
                for (int i = 0; i <= n - 2 * windowSize; i++)
                {
                    int j = i + windowSize;
                    if (CompareGroups(penalized, i, j, windowSize))
                    {
                        for (int k = i; k < i + windowSize; k++)
                            penalized[k] = 1.0;
                        i += windowSize - 1;
                    }
                }
            }
            return penalized;
        }

        public static bool CompareGroups(List<double> sequence, int start1, int start2, int windowSize)
        {
            for (int k = 0; k < windowSize; k++)
            {
                double value1 = sequence[start1 + k];
                double value2 = sequence[start2 + k];

                if (Math.Round(Math.Abs(value1 - value2), 2) > 0.01)
                {
                    return false;
                }
            }
            return true;
        }

        public static double GetBpmMultiplier(double ratingSingleSpeed)
        {
            if (ratingSingleSpeed <= 2.5) return 1.0;
            if (ratingSingleSpeed <= 5) return 1.05;
            if (ratingSingleSpeed <= 8) return 1.1;
            if (ratingSingleSpeed <= 10) return 1.2;
            if (ratingSingleSpeed <= 12) return 1.25;
            if (ratingSingleSpeed <= 15) return 1.2;
            if (ratingSingleSpeed <= 20) return 1.15;
            if (ratingSingleSpeed <= 25) return 1.1;
            return 1.05;
        }

        public static List<List<AdofaiTile>> GroupTilesM(List<AdofaiTile> tiles)
        {
            List<List<AdofaiTile>> groups = new List<List<AdofaiTile>>();
            int i = 0;

            while (i < tiles.Count)
            {
                AdofaiTile current = tiles[i];
                if (current.TileTime == 0 && current.HitWindowM == 0)
                {
                    i++;
                    continue;
                }

                double endTime = current.TileTime + Math.Max(current.HitWindowM, 50);
                List<AdofaiTile> group = new List<AdofaiTile> { current };

                int j = i + 1;
                while (j < tiles.Count)
                {
                    AdofaiTile next = tiles[j];
                    if (next.TileTime <= endTime)
                    {
                        group.Add(next);
                        j++;
                    }
                    else
                    {
                        break;
                    }
                }
                groups.Add(group);
                i = j;
            }
            return groups;
        }

        public static double GetTechSingleRating(List<AdofaiTile> currentGroup, List<AdofaiTile> nextGroup)
        {
            int currentCount = currentGroup.Count;
            int nextCount = nextGroup.Count;
            int differenceNum = nextCount - currentCount;
            double differenceRatio = (double)nextCount / currentCount;

            if (differenceNum >= 6 && differenceRatio >= 3)
            {
                return 1.3 + 0.1 * (1.5 * Math.Pow(differenceRatio,0.5) - 1);
            }
            else if (differenceNum >= 3 && differenceRatio > 2)
            {
                return 1 + 0.1 * (differenceNum - 2);
            }
            return 1.0;
        }

        public static double GetGroupInterval(List<AdofaiTile> prevGroup, List<AdofaiTile> currGroup, List<AdofaiTile> nextGroup)
        {
            double prevStart = prevGroup[0].TileTime;
            double currStart = currGroup[0].TileTime;
            double nextStart = nextGroup[0].TileTime;

            double prevToCurr = currStart - prevStart;
            if (prevToCurr <= 0.001) return 1.0; // Prevent division by zero

            double currToNext = nextStart - currStart;
            return currToNext / prevToCurr;
        }

    }
}