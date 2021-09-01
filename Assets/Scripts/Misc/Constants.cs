using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zoca
{
    public enum Team { Blue, Red }


    public class ResourceFolder
    {
        public static readonly string Character = "Characters";
        public static readonly string Ball = "Balls";
    }

    public class Tag
    {
        public static readonly string Ball = "Ball";
    }

    public class Constants
    {
        public static readonly float StartDelay = 7;
        public static readonly float PauseDelay = 3;
    }

    public class PhotonEvent
    {
        public const byte StartMatch = 200;
        public const byte TeamScored = 199;
    }
}
