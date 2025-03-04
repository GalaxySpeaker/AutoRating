using System;

namespace AutoRating.AdofaiRater.Objects
{
    public class AdofaiTile
    {
        public int LinearFloor { get; set; }
        public int OriginalFloor { get; set; }
        public double OriginalAngle { get; set; }
        public double PauseDuration { get; set; }
        public double MarginScale { get; set; } = 100;
        public bool IsTwirl { get; set; }
        public bool IsRealBpmChange { get; set; }
        public int NewPlanetCount { get; set; }
        public double PlanetBpm { get; set; }
        public double RealBpm { get; set; }
        public double TileTime { get; set; }
        public double HitWindowP { get; set; }
        public double HitWindowM { get; set; }

        public bool IsMidspin() => Math.Abs(OriginalAngle - 999) < 0.001;
    }
}