using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Reflection;

public class RemoveUnusedPostProcessingData : IPreprocessBuildWithReport
{
    public int callbackOrder => 0;

    public void OnPreprocessBuild(BuildReport report)
    {
        Debug.Log("=== URP 10 Optimizer: Removing FilmGrain & BlueNoise ===");

        // Get URP asset
        var urp = GraphicsSettings.renderPipelineAsset as UniversalRenderPipelineAsset;
        if (urp == null)
        {
            Debug.LogWarning("URP Asset not found.");
            return;
        }

        // Use reflection to access internal m_RendererDataList
        var field = typeof(UniversalRenderPipelineAsset)
            .GetField("m_RendererDataList", BindingFlags.NonPublic | BindingFlags.Instance);

        if (field == null)
        {
            Debug.LogError("Cannot find m_RendererDataList (URP changed?).");
            return;
        }

        var rendererDataList = field.GetValue(urp) as ScriptableRendererData[];

        if (rendererDataList == null || rendererDataList.Length == 0)
        {
            Debug.LogError("RendererDataList is empty.");
            return;
        }

        // Usually index 0 = main ForwardRendererData
        foreach (var rd in rendererDataList)
        {
            var forward = rd as ForwardRendererData;
            if (forward == null) continue;

            var post = forward.postProcessData;
            if (post == null)
            {
                Debug.LogWarning("postProcessData not found.");
                continue;
            }

            bool changed = false;

            // Clear FilmGrain
            if (post.textures.filmGrainTex != null && post.textures.filmGrainTex.Length > 0)
            {
                Debug.Log($"Removed FilmGrain: {post.textures.filmGrainTex.Length} textures");
                post.textures.filmGrainTex = new Texture2D[0];
                changed = true;
            }

            // Clear BlueNoise
            if (post.textures.blueNoise16LTex != null && post.textures.blueNoise16LTex.Length > 0)
            {
                Debug.Log($"Removed BlueNoise: {post.textures.blueNoise16LTex.Length} textures");
                post.textures.blueNoise16LTex = new Texture2D[0];
                changed = true;
            }

            // Remove SMAA (optional)
            if (post.textures.smaaAreaTex != null)
            {
                post.textures.smaaAreaTex = null;
                post.textures.smaaSearchTex = null;
                Debug.Log("Removed SMAA textures.");
                changed = true;
            }

            if (changed)
            {
                EditorUtility.SetDirty(post);
                EditorUtility.SetDirty(forward);
                AssetDatabase.SaveAssets();
            }
        }

        Debug.Log("=== URP 10 Optimizer: Done ===");
    }
}
