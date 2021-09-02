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
        public static readonly float StartDelay = 4;
        public static readonly float GoalDelay = 7;
    }

    public class PhotonEvent
    {
        public const byte StateChanged = 199;
    }
}
