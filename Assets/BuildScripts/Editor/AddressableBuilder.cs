using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

/// <summary>
/// Editor entry point for building Addressables content from CI.
/// Invoked by the unity-build-workflows toolkit via
/// <c>-executeMethod AddressableBuilder.Build</c> (platform=Addressables).
/// </summary>
public static class AddressableBuilder
{
    public static void Build()
    {
        var settings = AddressableAssetSettingsDefaultObject.Settings;
        if (settings == null)
        {
            throw new System.Exception(
                "AddressableAssetSettings not found. Open " +
                "Window > Asset Management > Addressables > Groups to create them.");
        }

        AddressableAssetSettings.BuildPlayerContent(out var result);

        if (!string.IsNullOrEmpty(result.Error))
        {
            throw new System.Exception("Addressables build failed: " + result.Error);
        }

        Debug.Log("[AddressableBuilder] Addressables build succeeded.");
    }
}
