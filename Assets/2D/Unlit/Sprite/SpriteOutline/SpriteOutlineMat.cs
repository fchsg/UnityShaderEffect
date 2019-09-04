using System.Collections.Generic;
using UnityEngine;

namespace _2D.Unlit.Sprite.SpriteOutline
{
    public class SpriteOutlineMat : Material
    {
        private Texture SpriteTexture => mainTexture;
        private bool DrawOutside => IsKeywordEnabled(OutsideMaterialKeyword);
        private bool InstancingEnabled => enableInstancing;

        private const string OutlineShaderName = "SG/Unlit/Sprite/Outline";
        private const string OutsideMaterialKeyword = "SPRITE_OUTLINE_OUTSIDE";
        private static readonly Shader OutlineShader = Shader.Find(OutlineShaderName);

        private static readonly List<SpriteOutlineMat> sharedMaterials = new List<SpriteOutlineMat>();

        private SpriteOutlineMat (Texture spriteTexture, bool drawOutside = false, bool instancingEnabled = false)
            : base(OutlineShader)
        {
            if (!OutlineShader) Debug.LogError($"{OutlineShaderName} shader not found. Make sure the shader is included to the build.");

            mainTexture = spriteTexture;
            if (drawOutside) EnableKeyword(OutsideMaterialKeyword);
            if (instancingEnabled) enableInstancing = true;
        }

        public static Material GetSharedFor (_2D.Sprite.SpriteOutline.SpriteOutline spriteGlow)
        {
            foreach (var t in sharedMaterials)
            {
                if (t.SpriteTexture == spriteGlow.Renderer.sprite.texture &&
                    t.DrawOutside == spriteGlow.DrawOutside &&
                    t.InstancingEnabled == spriteGlow.EnableInstancing)
                    return t;
            }

            var material = new SpriteOutlineMat(spriteGlow.Renderer.sprite.texture, spriteGlow.DrawOutside, spriteGlow.EnableInstancing);
            material.hideFlags = HideFlags.DontSaveInBuild | HideFlags.DontSaveInEditor | HideFlags.NotEditable;
            sharedMaterials.Add(material);

            return material;
        }
    }
}
