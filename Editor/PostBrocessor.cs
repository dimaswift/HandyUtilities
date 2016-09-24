using UnityEngine;
using System.Collections;
using UnityEditor;

namespace HandyUtilities
{
    public class PostBrocessor : AssetPostprocessor
    {
        void OnPreprocessTexture()
        {
            if(assetPath.Contains("HandyUtilities/Editor/Icons/") && assetPath.EndsWith(".png"))
            {
                var t = assetImporter as TextureImporter;
                t.textureType = TextureImporterType.GUI;
            }
        }

    }
}
