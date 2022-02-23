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

    
}
