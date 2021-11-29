using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Zoca.Collections;
//using Zom.Pie.Collections;

namespace Zoca.Editor
{

    public class AssetBuilder : MonoBehaviour
    {

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }


        [MenuItem("Assets/Create/Collections/Character")]
        public static void CreateItem()
        {
            Character asset = ScriptableObject.CreateInstance<Character>();
            
            string name = "/empty.asset";
            
            string folder = System.IO.Path.Combine("Assets/Resources", ResourceFolder.Collections);
            folder = System.IO.Path.Combine("Assets/Resources", ResourceFolder.Characters);

            if (!System.IO.Directory.Exists(folder))
                System.IO.Directory.CreateDirectory(folder);

            AssetDatabase.CreateAsset(asset, folder + name);
            
            AssetDatabase.SaveAssets();

            EditorUtility.FocusProjectWindow();

            Selection.activeObject = asset;
        }

    
    }

}
