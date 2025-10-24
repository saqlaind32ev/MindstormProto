using UnityEditor;
using UnityEngine;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

[InitializeOnLoad]
public static class ProtoSetup
{
    private static readonly string MANIFEST_PATH =  Path.Combine(Directory.GetCurrentDirectory(), "Packages/manifest.json");

    private static readonly List<RegistryEntry> Registries = new List<RegistryEntry>
    {
        new RegistryEntry(
            "AppLovin MAX Unity",
            "https://unity.packages.applovin.com",
            "com.applovin.mediation.ads",
            "com.applovin.mediation.adapters",
            "com.applovin.mediation.dsp"
        ),
        new RegistryEntry(
            "package.openupm.com",
            "https://package.openupm.com",
            "com.google.external-dependency-manager"
        ),
        new RegistryEntry(
            "Mindstorm Studios",
            "https://packages.mindstormstudios.com",
            "com.mindstorm"
        )
    };
    private static readonly Dictionary<string, string> Dependencies = new Dictionary<string, string>
    {
        { "com.google.external-dependency-manager", "1.2.185" },
        { "com.applovin.mediation.ads", "8.3.1" },
        { "com.applovin.mediation.adapters.bidmachine.android", "3030002.0.0" },
        { "com.applovin.mediation.adapters.bidmachine.ios", "304000000.0.0" },
        { "com.applovin.mediation.adapters.bigoads.android", "5050000.0.0" },
        { "com.applovin.mediation.adapters.bigoads.ios", "4090000.0.0" },
        { "com.applovin.mediation.adapters.bytedance.android", "701000700.0.0" },
        { "com.applovin.mediation.adapters.bytedance.ios", "702000400.0.0" },
        { "com.applovin.mediation.adapters.chartboost.android", "9090200.0.0" },
        { "com.applovin.mediation.adapters.chartboost.ios", "9090200.0.0" },
        { "com.applovin.mediation.adapters.facebook.android", "6200000.0.0" },
        { "com.applovin.mediation.adapters.facebook.ios", "6200100.0.0" },
        { "com.applovin.mediation.adapters.fyber.android", "8030800.0.0" },
        { "com.applovin.mediation.adapters.fyber.ios", "8030800.0.0" },
        { "com.applovin.mediation.adapters.google.android", "24050000.0.0" },
        { "com.applovin.mediation.adapters.google.ios", "12080000.0.0" },
        { "com.applovin.mediation.adapters.googleadmanager.android", "24050000.0.0" },
        { "com.applovin.mediation.adapters.googleadmanager.ios", "12080000.0.0" },
        { "com.applovin.mediation.adapters.inmobi.android", "10080700.0.0" },
        { "com.applovin.mediation.adapters.inmobi.ios", "10080600.0.0" },
        { "com.applovin.mediation.adapters.ironsource.android", "810000000.0.0" },
        { "com.applovin.mediation.adapters.ironsource.ios", "810000000.0.0" },
        { "com.applovin.mediation.adapters.mintegral.android", "16099100.0.0" },
        { "com.applovin.mediation.adapters.mintegral.ios", "707090000.0.0" },
        { "com.applovin.mediation.adapters.mobilefuse.android", "1090201.0.0" },
        { "com.applovin.mediation.adapters.mobilefuse.ios", "1090201.0.0" },
        { "com.applovin.mediation.adapters.moloco.android", "3120100.0.0" },
        { "com.applovin.mediation.adapters.moloco.ios", "3120100.0.0" },
        { "com.applovin.mediation.adapters.ogurypresage.android", "6010001.0.0" },
        { "com.applovin.mediation.adapters.ogurypresage.ios", "5000200.0.0" },
        { "com.applovin.mediation.adapters.pubmatic.android", "4080000.0.0" },
        { "com.applovin.mediation.adapters.pubmatic.ios", "4080000.0.0" },
        { "com.applovin.mediation.adapters.smaato.android", "22070201.0.0" },
        { "com.applovin.mediation.adapters.smaato.ios", "22090301.0.0" },
        { "com.applovin.mediation.adapters.unityads.android", "4160000.0.0" },
        { "com.applovin.mediation.adapters.unityads.ios", "4160000.0.0" },
        { "com.applovin.mediation.adapters.verve.android", "3030000.0.0" },
        { "com.applovin.mediation.adapters.verve.ios", "3020000.0.0" },
        { "com.applovin.mediation.adapters.vungle.android", "7050100.0.0" },
        { "com.applovin.mediation.adapters.vungle.ios", "7050300.0.0" },
        { "com.mindstorm.analytics", "git@github.com:saqlaind32ev/MindstormProto.git"}
    };
     
    static ProtoSetup()
    {
        if (!File.Exists(MANIFEST_PATH))
        { 
            Debug.LogError("manifest.json not found.");
            return; 
        }

        var manifestContent = File.ReadAllText(MANIFEST_PATH);
        var manifest = JObject.Parse(manifestContent);

        EnsureScopedRegistries(manifest, Registries);
        EnsureDependencies(manifest, Dependencies);

        File.WriteAllText(MANIFEST_PATH, manifest.ToString());
        AssetDatabase.Refresh();
    }
    
    private static void EnsureScopedRegistries(JObject manifest, List<RegistryEntry> registries)
    {
        var scopedRegistries = manifest["scopedRegistries"] as JArray ?? new JArray();
        bool changed = false;

        foreach (var entry in registries)
        {
            bool exists = false;
            foreach (var r in scopedRegistries)
            {
                if (r["name"]?.ToString() == entry.Name) { exists = true; break; }
            }
            if (!exists)
            {
                scopedRegistries.Add(new JObject
                {
                    ["name"] = entry.Name,
                    ["url"] = entry.Url,
                    ["scopes"] = new JArray(entry.Scopes)
                });
                changed = true;
            }
        }

        if (changed) manifest["scopedRegistries"] = scopedRegistries;
    }

    private static void EnsureDependencies(JObject manifest, Dictionary<string, string> depsToEnsure)
    {
        var dependencies = manifest["dependencies"] as JObject ?? new JObject();
        bool changed = false;

        foreach (var kv in depsToEnsure)
        {
            if (dependencies[kv.Key] == null)
            {
                dependencies[kv.Key] = kv.Value;
                changed = true;
            }
        }

        if (changed)
            manifest["dependencies"] = dependencies;
    }
    
    private sealed class RegistryEntry
    {
        public string Name;
        public string Url;
        public string[] Scopes;
        public RegistryEntry(string name, string url, params string[] scopes)
        {
            Name = name; Url = url; Scopes = scopes;
        }
    }
}
