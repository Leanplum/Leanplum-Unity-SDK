#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Android;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;

class LeanplumAndroidBuildProcessor : IPostGenerateGradleAndroidProject
{
    public int callbackOrder { get { return 0; } }
    public void OnPostGenerateGradleAndroidProject(string path)
    {
        string gradleProperties = Directory.GetCurrentDirectory() + "/" + path + "/gradle.properties";
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
#endif