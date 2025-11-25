using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JetSystems
{

    public static class PlayerPrefsManager
    {
        static string COINSKEY = "COINS";
        static string SOUNDKEY = "SOUNDS";



        public static int GetCoins()
        { return PlayerPrefs.GetInt(COINSKEY); }

        public static void SaveCoins(int coinsAmount)
        { PlayerPrefs.SetInt(COINSKEY, coinsAmount); }

        public static int GetSoundState()
        { return PlayerPrefs.GetInt(SOUNDKEY); }

        public static void SetSoundState(int state)
        { PlayerPrefs.SetInt(SOUNDKEY, state); }


    }
}
