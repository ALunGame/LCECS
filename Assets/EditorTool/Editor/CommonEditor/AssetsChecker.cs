using UnityEditor;
using UnityEngine;

public class AssetsChecker : AssetPostprocessor
{
    public void OnPreprocessTexture()
    {
        if (!assetPath.Contains("Res/UI"))
        {
            return;
        }

        TextureImporterType   type    = TextureImporterType.Sprite;
        TextureImporterFormat pc      = TextureImporterFormat.DXT5;
        TextureImporterFormat android = TextureImporterFormat.ETC2_RGBA8;
        TextureImporterFormat ios     = TextureImporterFormat.PVRTC_RGBA4;
        TextureImporter       ti      = AssetImporter.GetAtPath(assetPath) as TextureImporter;

        //if (ti.textureType != TextureImporterType.Default)
        //{
        //    return;
        //}


        if (assetPath.Contains("BigImage"))
        {
            if (ti.DoesSourceTextureHaveAlpha())
            {
                if (!assetPath.Contains("_a.png"))
                {
                    Debug.LogError(string.Format("{0}带alpha通道，但是并没有加上_a标签，请检查是否需要alpha通道", assetPath));
                }
                pc      = TextureImporterFormat.DXT5;
                android = TextureImporterFormat.ETC2_RGBA8;
                ios     = TextureImporterFormat.PVRTC_RGBA4;
            }
            else
            {
                pc      = TextureImporterFormat.DXT5;
                android = TextureImporterFormat.ETC2_RGB4;
                ios     = TextureImporterFormat.PVRTC_RGB4;
            }
        }

        if (assetPath.Contains("Dynamic") || assetPath.Contains("FrameAnim") || assetPath.Contains("Login"))
        {
            pc      = TextureImporterFormat.DXT5;
            android = TextureImporterFormat.ETC_RGB4;
            ios     = TextureImporterFormat.ASTC_RGBA_4x4;
        }

        string tag = GetTagName(assetPath);
        SetTextureImportFormat(ti, assetPath, type, 1024, false, pc, android, ios, tag);
    }

    private static string GetTagName(string path)
    {
        if (string.IsNullOrEmpty(path))
            return "";
        path = path.Replace("\\", "/");
        if (path.IndexOf("Res/UI/BigImage/") >= 0)
            return "";
        int nIndex = path.IndexOf("Res/UI/");
        if (nIndex < 0)
            return "";
        string strDir = path.Substring(nIndex + 7, path.Length - nIndex - 7);
        nIndex = strDir.LastIndexOf("/");
        if (nIndex < 0)
            return "";
        strDir = strDir.Substring(0, nIndex);
        string[] array = strDir.Split("/".ToCharArray());

        if (array.Length > 0)
        {
            string strTagName = array[0].ToLower();
            if (array.Length > 1)
                strTagName += "_" + array[1].ToLower();
            return strTagName;
        }
        return "";
    }

    private static void SetTextureImportFormat(TextureImporter ti, string assetPath, TextureImporterType  type, int maxSize, bool isReadable, TextureImporterFormat pcFormat, TextureImporterFormat androidFormat, TextureImporterFormat iosFormat, string tag)
    {
        if (ti == null)
        {
            return;
        }
        bool changed = false;

        if (ti.mipmapEnabled)
        {
            ti.mipmapEnabled = false;
            changed          = true;
        }

        if (ti.wrapMode      != TextureWrapMode.Clamp)
        {
            ti.wrapMode = TextureWrapMode.Clamp;
            changed     = true;
        }

        if (ti.textureType != type)
        {
            ti.textureType = type;
            changed        = true;
        }

        if (ti.filterMode != FilterMode.Trilinear)
        {
            ti.filterMode = FilterMode.Trilinear;
            changed       = true;
        }

        if (ti.isReadable != isReadable)
        {
            ti.isReadable = isReadable;
            changed       = true;
        }

        if (ti.spritePackingTag != tag)
        {
            ti.spritePackingTag = tag;
            changed             = true;
        }

        //--设置平台格式--
        changed = SetImporterSettings(ti, true, "Standalone", TextureImporterCompression.Uncompressed, maxSize, pcFormat, true) || changed;
        changed = SetImporterSettings(ti, true, "Android", TextureImporterCompression.Compressed, maxSize, androidFormat, true) || changed;
        changed = SetImporterSettings(ti, true, "iPhone", TextureImporterCompression.Compressed, maxSize, iosFormat, true) || changed;

        if (changed)
        {
            ti.SaveAndReimport();
            AssetDatabase.ImportAsset(assetPath);
        }
    }

    private static bool SetImporterSettings(TextureImporter ti, bool ov, string name, TextureImporterCompression compression, int max, TextureImporterFormat importerFormat, bool alpha)
    {
        bool                            changed          = false;
        TextureImporterPlatformSettings importerSettings = ti.GetPlatformTextureSettings(name);
        if (importerSettings.overridden           != ov)
        {
            importerSettings.overridden = ov;
            changed                     = true;
        }
        if (importerSettings.name                 != name)
        {
            importerSettings.name = name;
            changed               = true;
        }
        if (importerSettings.textureCompression   != compression)
        {
            importerSettings.textureCompression = compression;
            changed                             = true;
        }
        if (importerSettings.maxTextureSize       != max)
        {
            importerSettings.maxTextureSize = max;
            changed                         = true;
        }
        if (importerSettings.format               != importerFormat)
        {
            importerSettings.format = importerFormat;
            changed                 = true;
        }
        if (importerSettings.allowsAlphaSplitting != alpha)
        {
            importerSettings.allowsAlphaSplitting = alpha;
            changed                               = true;
        }
        if (changed)
        {
            ti.SetPlatformTextureSettings(importerSettings);
        }
        return changed;
    }
}