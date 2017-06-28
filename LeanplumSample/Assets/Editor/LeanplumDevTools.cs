#if LEANER_PLUM
using UnityEngine;
using System.Collections;
using UnityEditor;

namespace LeanplumSDK
{
    public static class LeanplumDevTools
    {
        [MenuItem("Tools/Leanplum/Clear Data")]
        public static void ClearData()
        {
            PlayerPrefs.DeleteAll();
        }
    }
}
#endif
