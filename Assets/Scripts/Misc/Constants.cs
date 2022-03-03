using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zoca
{
    public enum Team { Blue, Red }

    public enum Skill { Speed, Stamina, Resistance, FirePower, FireRate, FireRange }

    public class ResourceFolder
    {
        // Base folders
        public static readonly string Collections = "Collections";
        public static readonly string GameAssets = "GameAssets";

        // Sepcific folders
        public static readonly string Balls = GameAssets + "/Balls";
        public static readonly string Sprites = "Sprites";
        public static readonly string TeamFormations = "AI/Formations";


        
    }

    public class Tag
    {
        public static readonly string Ball = "Ball";
        public static readonly string Player = "Player";
        public static readonly string PlayerCamera = "PlayerCamera";
    }

    public class Layer
    {
        public static readonly string Ground = "Floor";
        public static readonly string Ball = "Ball";
        public static readonly string Player = "Player";
        public static readonly string Wall = "Walls";
        public static readonly string Top = "Top";
    }

    public class Constants
    {
        public static readonly float StartDelay = 9;
        public static readonly float GoalDelay = 7;
        public static readonly float StartDelayOnGoal = 4;

    }

    public class PhotonEvent
    {
        public const byte StateChanged = 199;
        public const byte Synchronize = 198;
        public const byte SpawnBarrier = 197;
        public const byte SpawnElectricGrenade = 196;
        public const byte SpawnMagnet = 195;
    }

    public class CharacterSkillsInfo
    {
        public const float SpeedMin = 7;
        public const float SpeedMax = 10;
        //public const float SpeedStep = 0.15f;

        public const float StaminaMin = 140;
        public const float StaminaMax = 300;
        //public const float StaminaStep = 8f;

        public const float FreezeTimeMin = 2;
        public const float FreezeTimeMax = 5;
        //public const float FreezeTimeStep = -0.15f;
    }

    public class FireWeaponStatsInfo
    {
        public const float FirePowerMin = 26;
        public const float FirePowerMax = 36;
        //public const float PowerStep = 0.5f;

        public const float FireRateMin = 0.6f;
        public const float FireRateMax = 1;
        //public const float RateStep = 0.02f;

        public const float FireRangeMin = 8;
        public const float FireRangeMax = 12;
        //public const float FreezeTimeStep = -0.15f;
    }
}
