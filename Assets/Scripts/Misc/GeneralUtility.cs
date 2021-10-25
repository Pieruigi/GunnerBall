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
    }

}
