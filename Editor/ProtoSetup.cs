using UnityEditor;
using UnityEngine;
using System.IO;
using Newtonsoft.Json.Linq;

[InitializeOnLoad]
public static class ProtoSetup
{
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
        
        File.WriteAllText(manifestPath, manifest.ToString());
        AssetDatabase.Refresh();
        Debug.Log("AppLovin MAX Unity and OpenUPM registries added to manifest.");
    }
}
