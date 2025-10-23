using UnityEditor;
using UnityEngine;
using System.IO;
using Newtonsoft.Json.Linq;

[InitializeOnLoad]
public static class ProtoSetup
{
    static ProtoSetup()
    {
        // Path to the manifest.json in the project
        string manifestPath = Path.Combine(Directory.GetCurrentDirectory(), "Packages/manifest.json");

        // Check if the manifest file exists
        if (File.Exists(manifestPath))
        {
            // Read the manifest file
            var manifestContent = File.ReadAllText(manifestPath);

            // Parse the manifest content as JSON
            var manifest = JObject.Parse(manifestContent);

            // Check if the scopedRegistries key exists, if not, create it
            var scopedRegistries = manifest["scopedRegistries"] as JArray ?? new JArray();

            // Check if the AppLovin registry is already added
            bool appLovinRegistryExists = false;
            foreach (var registry in scopedRegistries)
            {
                if (registry["name"].ToString() == "AppLovin MAX Unity")
                {
                    appLovinRegistryExists = true;
                    break;
                }
            }

            // If the AppLovin registry is not in the manifest, add it
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

            // Check if the OpenUPM registry is already added
            bool openUpmRegistryExists = false;
            foreach (var registry in scopedRegistries)
            {
                if (registry["name"].ToString() == "package.openupm.com")
                {
                    openUpmRegistryExists = true;
                    break;
                }
            }

            // If the OpenUPM registry is not in the manifest, add it
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

            // Assign the updated scopedRegistries array back to the manifest
            manifest["scopedRegistries"] = scopedRegistries;

            // Write the updated manifest content back to the file
            File.WriteAllText(manifestPath, manifest.ToString());
            AssetDatabase.Refresh();
            Debug.Log("AppLovin MAX Unity and OpenUPM registries added to manifest.");
        }
        else
        {
            Debug.LogError("manifest.json not found.");
        }
    }
}
