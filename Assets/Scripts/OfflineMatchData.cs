using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zoca
{
    public class OfflineMatchData : MonoBehaviour
    {
        enum Weapon { Gun, BigGun }

        enum Character { Saeko, Sen, Makoto, Hirota }

        public static OfflineMatchData Instance { get; private set; }

        public int LocalPlayerCharacterId
        {
            get { return (int)localPlayer; }
        }

        public int LocalPlayerWeaponId
        {
            get { return (int)localPlayerWeapon; }
        }

        public IList<int> AiCharacterIds
        {
            get { return ais.AsReadOnly(); }
        }
        public IList<int> AiWeaponIds
        {
            get { return aisWeapons.AsReadOnly(); }
        }

        [SerializeField]
        Character localPlayer;

        [SerializeField]
        Weapon localPlayerWeapon;

        [SerializeField]
        List<int> ais;
        List<int> aisWeapons;

        private void Awake()
        {
            if (!Instance)
            {
                Instance = this;

                ais = new List<int>();
                aisWeapons = new List<int>();

                int id = 0;
                for(int i=0; i<3; i++)
                {
                    if (localPlayer == (Character)i)
                        id++;

                    if (id > 3)
                        id = 0;

                    ais.Add(id);
                    aisWeapons.Add(Random.Range(0, 2));

                    id++;
                }
            }

            


        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }

}
