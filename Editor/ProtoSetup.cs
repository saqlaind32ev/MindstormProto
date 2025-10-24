using UnityEditor;
using UnityEngine;
using System.IO;
using Newtonsoft.Json.Linq;

[InitializeOnLoad]
public static class ProtoSetup
{
    private bool isChangeRequired = false;
    private static ProtoSetup()
    {
        string manifestPath = Path.Combine(Directory.GetCurrentDirectory(), "Packages/manifest.json");
        if (!File.Exists(manifestPath))
        {
            Debug.LogError("manifest.json not found.");
            return;
        }
       
        var manifestContent = File.ReadAllText(manifestPath);
        var manifest = JObject.Parse(manifestContent);
        
        var scopedRegistries = manifest["scopedRegistries"] as JArray ?? new JArray();
        
        bool appLovinRegistryExists = false;
        foreach (var registry in scopedRegistries)
        {
            if (registry["name"].ToString() == "AppLovin MAX Unity")
            {
                appLovinRegistryExists = true;
                isChangeRequired = true;
                break;
            }
        }
        
        if (!appLovinRegistryExists)
        {
            scopedRegistries.Add(new JObject
            {
                ["name"] = "AppLovin MAX Unity",
                ["url"] = "https://unity.packages.applovin.com",
                ["scopes"] = new JArray(
                    "com.applovin.mediation.ads",
                    "com.applovin.mediation.adapters",
                    "com.applovin.mediation.dsp"
                )
            });
        }
        
        bool openUpmRegistryExists = false;
        foreach (var registry in scopedRegistries)
        {
            if (registry["name"].ToString() == "package.openupm.com")
            {
                openUpmRegistryExists = true;
                isChangeRequired = true;
                break;
            }
        }
        
        if (!openUpmRegistryExists)
        {
            scopedRegistries.Add(new JObject
            {
                ["name"] = "package.openupm.com",
                ["url"] = "https://package.openupm.com",
                ["scopes"] = new JArray(
                    "com.google.external-dependency-manager"
                )
            });
        }
        
        manifest["scopedRegistries"] = scopedRegistries;
        isChangeRequired |= SetDep(deps, "com.unity.nuget.newtonsoft-json", "3.0.2");
        isChangeRequired |= SetDep(deps, "com.google.external-dependency-manager", "1.2.185"); // specific version after OpenUPM registry
        isChangeRequired |= SetDep(deps, "com.adjust.sdk", "https://github.com/adjust/unity_sdk.git#v5.4.3?path=Assets/Adjust");
        isChangeRequired |= SetDep(deps, "com.applovin.mediation.ads", "8.3.1");

        if(!isChangeRequired) == return;
        File.WriteAllText(manifestPath, manifest.ToString());
        AssetDatabase.Refresh();
        UnityEditor.PackageManager.Client.Resolve();
        Debug.Log("AppLovin MAX Unity and OpenUPM registries added to manifest.");
    }

    private static bool SetDep(JObject deps, string id, string value)
    {
        var cur = (string)deps[id];
        if (cur == value) return false;
        deps[id] = value;
        return true;
    }
}
