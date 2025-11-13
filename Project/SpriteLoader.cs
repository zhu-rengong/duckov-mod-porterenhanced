using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public readonly struct SpriteKey : System.IEquatable<SpriteKey>
{
    public string FilePath { get; }
    public Rect SourceRect { get; }
    public Vector2 Pivot { get; }

    public SpriteKey(string filePath, Rect sourceRect, Vector2 pivot)
    {
        FilePath = filePath;
        SourceRect = sourceRect;
        Pivot = pivot;
    }

    public bool Equals(SpriteKey other) =>
        FilePath == other.FilePath &&
        SourceRect.Equals(other.SourceRect) &&
        Pivot.Equals(other.Pivot);

    public override bool Equals(object obj) => obj is SpriteKey other && Equals(other);

    public override int GetHashCode() => System.HashCode.Combine(FilePath, SourceRect, Pivot);

    public override string ToString() =>
        $"{FilePath} | {nameof(SourceRect)}: {SourceRect} | {nameof(Pivot)}: {Pivot}";

    public static bool operator ==(SpriteKey left, SpriteKey right) => left.Equals(right);
    public static bool operator !=(SpriteKey left, SpriteKey right) => !left.Equals(right);
}

public static class SpriteLoader
{
    private static Dictionary<string, Texture2D> textureCache = new Dictionary<string, Texture2D>();
    private static Dictionary<SpriteKey, Sprite> spriteCache = new Dictionary<SpriteKey, Sprite>();

    public static Sprite? CreateSprite(string filePath, Rect sourceRect, Vector2? pivot = null)
    {
        pivot ??= new Vector2(0.5f, 0.5f);
        SpriteKey spriteKey = new SpriteKey(filePath, sourceRect, pivot.Value);

        if (spriteCache.TryGetValue(spriteKey, out Sprite cachedSprite))
        {
            Debug.Log($"[{nameof(SpriteLoader)}] Sprite retrieved from cache: {spriteKey}");
            return cachedSprite;
        }

        Texture2D? texture = LoadTexture(filePath);

        if (texture == null)
        {
            return null;
        }
        
        Rect unityRect = new Rect(sourceRect.x, texture.height - sourceRect.y - sourceRect.height, sourceRect.width, sourceRect.height);
        
        Sprite sprite = Sprite.Create(texture, unityRect, pivot.Value);
        spriteCache[spriteKey] = sprite;
        Debug.Log($"[{nameof(SpriteLoader)}] Sprite created and cached: {spriteKey}");

        return sprite;
    }

    public static Texture2D? LoadTexture(string filePath)
    {
        if (textureCache.TryGetValue(filePath, out Texture2D cachedTexture))
        {
            Debug.Log($"[{nameof(SpriteLoader)}] Texture retrieved from cache: {filePath}");
            return cachedTexture;
        }

        try
        {
            if (!File.Exists(filePath))
            {
                Debug.LogError($"[{nameof(SpriteLoader)}] File does not exist: {filePath}");
                return null;
            }

            Texture2D texture = new(width: 2, height: 2);
            if (texture.LoadImage(File.ReadAllBytes(filePath)))
            {
                Debug.Log($"[{nameof(SpriteLoader)}] Texture loaded and cached: {filePath}");
                textureCache[filePath] = texture;
                return texture;
            }
            else
            {
                Debug.LogError($"[{nameof(SpriteLoader)}] Failed to load image from: {filePath}");
                return null;
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[{nameof(SpriteLoader)}] Error loading texture from {filePath}: {ex.Message}");
            textureCache.Remove(filePath);
            return null;
        }
    }

    public static void ClearCache()
    {
        Debug.Log($"[{nameof(SpriteLoader)}] Clearing cache.");

        foreach (var texture in textureCache.Values)
        {
            if (texture != null)
            {
                Object.Destroy(texture);
            }
        }

        foreach (var sprite in spriteCache.Values)
        {
            if (sprite != null)
            {
                Object.Destroy(sprite);
            }
        }

        textureCache.Clear();
        spriteCache.Clear();

        Debug.Log($"[{nameof(SpriteLoader)}] Cache cleared.");
    }

    public static void ClearCache(string filePath)
    {
        Debug.Log($"[{nameof(SpriteLoader)}] Clearing cache for file: {filePath}");

        if (textureCache.ContainsKey(filePath))
        {
            if (textureCache[filePath] != null)
            {
                Object.Destroy(textureCache[filePath]);
            }
            textureCache.Remove(filePath);
        }

        var keysToRemove = new List<SpriteKey>();
        foreach (var key in spriteCache.Keys)
        {
            if (key.FilePath == filePath)
            {
                if (spriteCache[key] != null)
                {
                    Object.Destroy(spriteCache[key]);
                }
                keysToRemove.Add(key);
            }
        }

        foreach (var key in keysToRemove)
        {
            spriteCache.Remove(key);
        }

        Debug.Log($"[{nameof(SpriteLoader)}] Cache cleared for file: {filePath}");
    }
}