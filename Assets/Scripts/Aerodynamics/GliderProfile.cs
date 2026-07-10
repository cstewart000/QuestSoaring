using UnityEngine;

namespace QuestSoaring.Aerodynamics
{
    /// <summary>Tunable glider definitions — beginner vs performance.</summary>
    public struct GliderProfile
    {
        public string Name;
        public GliderAeroModel Aero;
        public float PitchAuthority;
        public float RollAuthority;

        public static GliderProfile Beginner => new()
        {
            Name = "School Two-Seater",
            Aero = GliderAeroModel.Beginner,
            PitchAuthority = 8000f,
            RollAuthority = 12000f
        };

        public static GliderProfile Performance => new()
        {
            Name = "Club Racing 15m",
            Aero = new GliderAeroModel
            {
                WingArea = 10.5f, Mass = 280f, ClMax = 1.55f,
                MinSinkSpeed = 16f, BestGlideRatio = 38f
            },
            PitchAuthority = 6500f,
            RollAuthority = 15000f
        };
    }
}
