using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zoca
{
    public class GeneralUtility
    {
        public static void ShowCursor(bool show)
        {
            if (show)
            {
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
        }

        /// <summary>
        /// Linear goes from 0 to 1:
        /// 0 -> -80db
        /// 1 -> 0db
        /// Using decibleMax = 20:
        /// 0 -> -60db
        /// 1 -> 20db
        /// </summary>
        /// <param name="volume"></param>
        /// <returns></returns>
        public static float LinearToDecibel(float linear, float decibelMax = 0)
        {
            if (linear == 0)
                linear += 0.0001f;
            return Mathf.Log10(linear) * 20f + decibelMax;
        }

        /// <summary>
        /// When you read from a slider you receive decibel going from 0 to -80 or from 20 to -60;
        /// decibelMax is used to clamp linear between 0 and 1.
        /// Receiving 20 as decibel the linear value is 10^((20-20)/20) = 1;
        /// receiving -60 ( the minimum value in this case ) the linear value is 10^((-60-20)/20) = 0.0001;
        /// </summary>
        /// <param name="decibel"></param>
        /// <returns></returns>
        public static float DecibelToLinear(float decibel, float decibelMax = 0)
        {
            return Mathf.Pow(10f, (decibel - decibelMax) / 20f);
        }

        public static Texture2D Texture2DFlipVertical(Texture2D original)
        {
            Texture2D flipped = new Texture2D(original.width, original.height);

            int xN = original.width;
            int yN = original.height;

            for (int i = 0; i < xN; i++)
            {
                for (int j = 0; j < yN; j++)
                {
                    flipped.SetPixel(i, yN - j - 1, original.GetPixel(i, j));
                }
            }

            flipped.Apply();

            return flipped;
        }
    }

}
