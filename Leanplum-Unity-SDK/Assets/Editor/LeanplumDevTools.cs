#if LP_UNENCRYPTED
using UnityEngine;
using System.Collections;
using UnityEditor;

namespace LeanplumSDK
{
    public static class LeanplumDevTools
    {
        [MenuItem("Tools/Leanplum/Clear All PlayerPrefs")]
        public static void ClearData()
        {
            PlayerPrefs.DeleteAll();
        }
    }
}
#endif
