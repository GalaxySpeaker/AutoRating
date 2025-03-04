using AutoRating.AdofaiRater.Objects;
using Newtonsoft.Json.Linq;
using Overlayer.Tags.Patches;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AdofaiRater
{
    public static class RaterUtils
    {
        private static readonly Dictionary<char, Tuple<double, bool>> PathMeta = new Dictionary<char, Tuple<double, bool>>
        {
            { 'R', new Tuple<double, bool>(0.0, false) },
            { 'p', new Tuple<double, bool>(15.0, false) },
            { 'J', new Tuple<double, bool>(30.0, false) },
            { 'E', new Tuple<double, bool>(45.0, false) },
            { 'T', new Tuple<double, bool>(60.0, false) },
            { 'o', new Tuple<double, bool>(75.0, false) },
            { 'U', new Tuple<double, bool>(90.0, false) },
            { 'q', new Tuple<double, bool>(105.0, false) },
            { 'G', new Tuple<double, bool>(120.0, false) },
            { 'Q', new Tuple<double, bool>(135.0, false) },
            { 'H', new Tuple<double, bool>(150.0, false) },
            { 'W', new Tuple<double, bool>(165.0, false) },
            { 'L', new Tuple<double, bool>(180.0, false) },
            { 'x', new Tuple<double, bool>(195.0, false) },
            { 'N', new Tuple<double, bool>(210.0, false) },
            { 'Z', new Tuple<double, bool>(225.0, false) },
            { 'F', new Tuple<double, bool>(240.0, false) },
            { 'V', new Tuple<double, bool>(255.0, false) },
            { 'D', new Tuple<double, bool>(270.0, false) },
            { 'Y', new Tuple<double, bool>(285.0, false) },
            { 'B', new Tuple<double, bool>(300.0, false) },
            { 'C', new Tuple<double, bool>(315.0, false) },
            { 'M', new Tuple<double, bool>(330.0, false) },
            { 'A', new Tuple<double, bool>(345.0, false) },
            { '5', new Tuple<double, bool>(108.0, true) },
            { '6', new Tuple<double, bool>(252.0, true) },
            { '7', new Tuple<double, bool>(900.0 / 7.0, true) },
            { '8', new Tuple<double, bool>(360 - 900.0 / 7.0, true) },
            { 't', new Tuple<double, bool>(60.0, true) },
            { 'h', new Tuple<double, bool>(120.0, true) },
            { 'j', new Tuple<double, bool>(240.0, true) },
            { 'y', new Tuple<double, bool>(300.0, true) },
            { '!', new Tuple<double, bool>(999.0, true) }
        };

        public static AdofaiLevel ConvertToLinear(string path)
        {     
            string jsonText = File.ReadAllText(path);
            JObject json = JObject.Parse(jsonText);

            JObject settingsJson = (JObject)json["settings"];
            double startBpm = double.Parse(settingsJson["bpm"].ToString());
            List<AdofaiTile> tiles = new List<AdofaiTile> { new AdofaiTile { LinearFloor = 0, OriginalFloor = 0, OriginalAngle = 0 } };

            if (json["angleData"] != null)
            {
                JArray angleDataJsonArray = (JArray)json["angleData"];

                for (int i = 0; i < angleDataJsonArray.Count; i++)
                {
                    double angle = angleDataJsonArray[i].Value<double>();
                    if (Math.Abs(angle - 999.0) < 0.001)
                    {
                        tiles.Add(new AdofaiTile { LinearFloor = i + 1, OriginalFloor = i + 1, PauseDuration = 0, OriginalAngle = 999 });
                    }
                    else
                    {
                        tiles.Add(new AdofaiTile { LinearFloor = i + 1, OriginalFloor = i + 1, PauseDuration = 0, OriginalAngle = WrapTo180(angle) });
                    }
                }
            }
            else
            {
                string pathData = json["pathData"].ToString();
                double staticAngle = 0;

                for (int i = 0; i < pathData.Length; i++)
                {
                    var pair = PathMeta[pathData[i]];
                    if (Math.Abs(pair.Item1 - 999.0) < 0.001)
                    {
                        tiles.Add(new AdofaiTile { LinearFloor = i + 1, OriginalFloor = i + 1, PauseDuration = 0, OriginalAngle = 999 });
                    }
                    else
                    {
                        if (pair.Item2)
                        {
                            staticAngle = GeneralizeAngle(staticAngle + 180 - pair.Item1);
                            tiles.Add(new AdofaiTile { LinearFloor = i + 1, OriginalFloor = i + 1, PauseDuration = 0, OriginalAngle = WrapTo180(staticAngle) });
                        }
                        else
                        {
                            staticAngle = pair.Item1;
                            tiles.Add(new AdofaiTile { LinearFloor = i + 1, OriginalFloor = i + 1, PauseDuration = 0, OriginalAngle = WrapTo180(staticAngle) });
                        }
                    }
                }
            }

            // Add Actions
            JArray actionsJsonArray = (JArray)json["actions"];
            double tempBpm = startBpm;
            double currentMarginScale = 100;

            for (int i = 0; i < actionsJsonArray.Count; i++)
            {
                JObject actionJson = (JObject)actionsJsonArray[i];
                string eventType = actionJson["eventType"].ToString();

                int floor = actionJson["floor"].Value<int>();
                if (tiles.Count <= floor)
                {
                    continue;
                }
                AdofaiTile t = tiles[floor];

                if (eventType == "Twirl")
                {
                    t.IsTwirl = true;
                }

                if (eventType == "SetSpeed")
                {
                    if (actionJson["speedType"] != null)
                    {
                        if (actionJson["speedType"].ToString() == "Bpm")
                        {
                            tempBpm = double.Parse(actionJson["beatsPerMinute"].ToString());
                        }
                        else
                        {
                            tempBpm *= double.Parse(actionJson["bpmMultiplier"].ToString());
                        }
                    }
                    else
                    {
                        tempBpm = double.Parse(actionJson["beatsPerMinute"].ToString());
                    }
                    t.PlanetBpm = tempBpm;
                }

                if (eventType == "ScaleMargin")
                {
                    if (actionJson["scale"] != null)
                    {
                        currentMarginScale = actionJson["scale"].Value<double>();
                    }
                }
                t.MarginScale = currentMarginScale;

                if (eventType == "Pause")
                {
                    t.PauseDuration = actionJson["duration"].Value<double>();
                }

                if (eventType == "MultiPlanet")
                {
                    string planetsStr = actionJson["planets"].ToString();
                    if (char.IsDigit(planetsStr[0]))
                    {
                        t.NewPlanetCount = int.Parse(planetsStr);
                    }
                    else
                    {
                        t.NewPlanetCount = planetsStr == "ThreePlanets" ? 3 : 2;
                    }
                }

                if (eventType == "Hold")
                {
                    t.PauseDuration = actionJson["duration"].Value<int>() * 2;
                }

                if (eventType == "FreeRoam")
                {
                    t.PauseDuration = (actionJson["duration"].Value<double>() - 1);
                }
            }

            // Convert Midspin To Normal
            for (int floor = 1; floor < tiles.Count; floor++)
            {
                AdofaiTile t = tiles[floor];
                t.LinearFloor = floor;

                if (!tiles[floor].IsMidspin()) continue;
                AdofaiTile midspinTile = tiles[floor];
                AdofaiTile tile = tiles[floor - 1];

                tile.IsTwirl = !tile.IsTwirl;
                if (midspinTile.IsTwirl) tile.IsTwirl = !tile.IsTwirl;
                if (midspinTile.PlanetBpm != 0) tile.PlanetBpm = midspinTile.PlanetBpm;

                double axis = WrapTo180((double)tile.OriginalAngle + 90);
                for (int tempFloor = floor + 1; tempFloor < tiles.Count; tempFloor++)
                {
                    AdofaiTile tempTile = tiles[tempFloor];
                    if (tempTile.IsMidspin())
                    {
                        continue;
                    }
                    double toAdd = (axis - (double)tempTile.OriginalAngle) * 2;
                    tempTile.OriginalAngle = WrapTo180((double)tempTile.OriginalAngle + toAdd);
                }

                tiles.RemoveAt(floor);
                floor--;
            }

            // Twirl To Angles
            for (int floor = 1; floor < tiles.Count; floor++)
            {
                AdofaiTile tile = tiles[floor];

                if (tile.IsTwirl)
                {
                    for (int tempFloor = floor + 1; tempFloor < tiles.Count; tempFloor++)
                    {
                        AdofaiTile tempTile = tiles[tempFloor];
                        if (tempTile.IsMidspin())
                        {
                            continue;
                        }
                        double toAdd = ((double)tile.OriginalAngle - (double)tempTile.OriginalAngle) * 2;
                        tempTile.OriginalAngle = WrapTo180((double)tempTile.OriginalAngle + toAdd);
                    }
                }
            }

            // Fix Triple Planet
            bool isTriplePlanet = false;
            int tripleCount = 0;
            for (int floor = 1; floor < tiles.Count; floor++)
            {
                AdofaiTile t = tiles[floor];
                if (!t.IsMidspin())
                {
                    t.OriginalAngle = WrapTo180((double)t.OriginalAngle + 60 * tripleCount);
                    isTriplePlanet = t.NewPlanetCount > 0 ? t.NewPlanetCount == 3 : isTriplePlanet;
                    tripleCount += isTriplePlanet ? 1 : 0;
                }
            }

            AdofaiLevel level = new AdofaiLevel();
            List<AdofaiTile> newTiles = new List<AdofaiTile> { new AdofaiTile { LinearFloor = 0, OriginalFloor = 0, OriginalAngle = 0 } };

            double currentPlanetBpm = startBpm;
            double currentFlatBpm = startBpm;
            for (int linearFloor = 1; linearFloor < tiles.Count - 1; linearFloor++)
            {
                AdofaiTile tile = tiles[linearFloor];
                AdofaiTile nextTile = tiles[linearFloor + 1];

                double realAngle = GetAngleDiff((double)tile.OriginalAngle, (double)nextTile.OriginalAngle) + 180;

                if (Math.Abs(realAngle) < 0.001)
                {
                    realAngle = 360;
                }

                if (tile.PlanetBpm != 0)
                {
                    currentPlanetBpm = (double)tile.PlanetBpm;
                }

                double pitch = scnGame.instance.levelData.pitch / 100d;

                double realBpm =
                    currentPlanetBpm * (180 / (realAngle + (double)tile.PauseDuration * 180)) * pitch;
                double hitWindowP =
                    (45 / currentPlanetBpm) * (1000 / 3);
                double hitWindowM =
                    (60 / currentPlanetBpm) * (1000 / 3);

                AdofaiTile newTile = new AdofaiTile
                {
                    LinearFloor = linearFloor,
                    OriginalFloor = tile.OriginalFloor,
                    OriginalAngle = tile.OriginalAngle,
                    PlanetBpm = currentPlanetBpm,
                    RealBpm = realBpm,
                    HitWindowP = hitWindowP,
                    HitWindowM = hitWindowM
                };

                if (Math.Abs(currentFlatBpm - realBpm) > 0.0001)
                {
                    currentFlatBpm = realBpm;
                    newTile.IsRealBpmChange = true;
                }

                newTiles.Add(newTile);
            }

            // Add Last Tile
            AdofaiTile lastTile = tiles[tiles.Count - 1];
            newTiles.Add(new AdofaiTile
            {
                LinearFloor = lastTile.LinearFloor,
                OriginalFloor = lastTile.OriginalFloor,
                OriginalAngle = lastTile.OriginalAngle,
                PlanetBpm = currentPlanetBpm,
                RealBpm = currentPlanetBpm
            });

            level.StartBpm = startBpm;
            level.Tiles = newTiles;

            JArray newAngleDataArray = new JArray();
            JArray newActionsArray = new JArray();
            for (int i = 1; i < newTiles.Count; i++)
            {
                AdofaiTile t = newTiles[i];
                newAngleDataArray.Add(0);

                if (t.IsRealBpmChange)
                {
                    JObject action = new JObject
                    {
                        ["floor"] = t.LinearFloor,
                        ["eventType"] = "SetSpeed",
                        ["speedType"] = "Bpm",
                        ["beatsPerMinute"] = (double)t.RealBpm,
                        ["bpmMultiplier"] = 1,
                        ["angleOffset"] = 0
                    };

                    newActionsArray.Add(action);
                }
            }
            json.Remove("pathData");
            json.Remove("angleData");
            json.Remove("actions");

            json["angleData"] = newAngleDataArray;
            json["actions"] = newActionsArray;
            level.LinearLevelJson = json;

            // Calculate Click Time
            double lastPerfectClickTime = 0;
            for (int i = 1; i < level.Tiles.Count; i++)
            {
                AdofaiTile t = level.Tiles[i];
                t.TileTime = lastPerfectClickTime;
                lastPerfectClickTime = lastPerfectClickTime + (60000 / t.RealBpm);
            }

            level.TotalLength = (long)(level.Tiles[level.Tiles.Count - 1].TileTime / 1000);

            return level;
        }

        public static double WrapTo180(double angle)
        {
            angle %= 360;

            if (angle > 180)
            {
                angle = -(360 - angle);
            }
            else if (angle < -180)
            {
                angle = angle + 360;
            }

            return angle;
        }

        public static double GeneralizeAngle(double angle)
        {
            return ((angle % 360) + 360) % 360;
        }

        public static double GetAngleDiff(double startAngle, double targetAngle)
        {
            double angleDiff = startAngle - targetAngle;

            if (angleDiff > 180)
            {
                angleDiff = -(360 - angleDiff);
            }
            else if (angleDiff < -180)
            {
                angleDiff = angleDiff + 360;
            }

            return WrapTo180(angleDiff);
        }
    }
}