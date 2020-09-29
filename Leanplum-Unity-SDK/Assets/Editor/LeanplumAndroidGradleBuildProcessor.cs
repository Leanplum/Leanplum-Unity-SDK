#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Android;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;

namespace Leanplum.Private
{
    class LeanplumAndroidGradleBuildProcessor : IPostGenerateGradleAndroidProject
    {
        public int callbackOrder { get { return 0; } }
        public void OnPostGenerateGradleAndroidProject(string path)
        {
            var current = new Version(LeanplumSDK.SharedConstants.SDK_VERSION);
            var min = new Version("2.0.0");

            if (current >= min)
            {
                string output = Path.Combine(Directory.GetCurrentDirectory(), path);
                string gradleProperties = Path.Combine(output, "gradle.properties");
                string androidx = "android.useAndroidX=true";
                string jetifier = "android.enableJetifier=true";

                if (File.Exists(gradleProperties))
                {
                    StreamWriter writer = new StreamWriter(gradleProperties, true);
                    writer.WriteLine("\n");
                    writer.WriteLine(androidx);
                    writer.WriteLine(jetifier);
                    writer.Close();
                }
            }
        }
    }
}
#endif