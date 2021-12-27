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


        [MenuItem("Assets/Create/Collection/Character")]
        public static void CreateCharacter()
        {
            Character asset = ScriptableObject.CreateInstance<Character>();
            
            string name = "/character.asset";
            
            string folder = System.IO.Path.Combine("Assets/Resources", Character.CollectionFolder);
            //folder = System.IO.Path.Combine(folder, ResourceFolder.Collections);

            if (!System.IO.Directory.Exists(folder))
                System.IO.Directory.CreateDirectory(folder);

            AssetDatabase.CreateAsset(asset, folder + name);
            
            AssetDatabase.SaveAssets();

            EditorUtility.FocusProjectWindow();

            Selection.activeObject = asset;
        }

        [MenuItem("Assets/Create/Collection/Weapon")]
        public static void CreateWeapon()
        {
            Weapon asset = ScriptableObject.CreateInstance<Weapon>();

            string name = "/weapon.asset";

            string folder = System.IO.Path.Combine("Assets/Resources", Weapon.CollectionFolder);
            
            if (!System.IO.Directory.Exists(folder))
                System.IO.Directory.CreateDirectory(folder);

            AssetDatabase.CreateAsset(asset, folder + name);

            AssetDatabase.SaveAssets();

            EditorUtility.FocusProjectWindow();

            Selection.activeObject = asset;
        }

    }

}
