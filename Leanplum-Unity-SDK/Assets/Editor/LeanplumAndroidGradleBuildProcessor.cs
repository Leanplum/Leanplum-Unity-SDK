#if UNITY_EDITOR
using UnityEditor.Android;
using UnityEngine;
using System;
using System.IO;

namespace Leanplum.Private
{
    class LeanplumAndroidGradleBuildProcessor : IPostGenerateGradleAndroidProject
    {
        public int callbackOrder { get { return 0; } }
        public void OnPostGenerateGradleAndroidProject(string path)
        {
            var current = new Version(LeanplumSDK.Constants.SDK_VERSION);
            var min = new Version("2.0.0");

            if (current >= min)
            {
                string output = Path.Combine(Directory.GetCurrentDirectory(), path);
                output = output.Replace("unityLibrary", "");
                string gradleProperties = Path.Combine(output, "gradle.properties");
                string androidx = "android.useAndroidX=true";
                string jetifier = "android.enableJetifier=true";

                if (File.Exists(gradleProperties))
                {
                    StreamWriter writer = new StreamWriter(gradleProperties, true);
                    writer.WriteLine();
                    writer.WriteLine(androidx);
                    writer.WriteLine(jetifier);
                    writer.Close();
                }
                else
                {
                    Debug.Log("LeanplumAndroidGradleBuildProcessor: gradle.properties does not exist");
                }
            }
        }
    }
}
#endif