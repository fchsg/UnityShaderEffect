using UnityEngine;

namespace _2D.Sprite.SpriteOutline
{
    [AddComponentMenu("Effects/Sprite Outline")]
    [RequireComponent(typeof(SpriteRenderer)), DisallowMultipleComponent, ExecuteInEditMode]
    public class SpriteOutline : MonoBehaviour
    {
        public SpriteRenderer Renderer { get; private set; }

        private Color GlowColor
        {
            get => glowColor;
            set { if (glowColor != value) { glowColor = value; SetMaterialProperties(); } }
        }

        private float GlowBrightness
        {
            get => glowBrightness;
            set { if (glowBrightness != value) { glowBrightness = value; SetMaterialProperties(); } }
        }

        private int OutlineWidth
        {
            get => outlineWidth;
            set { if (outlineWidth != value) { outlineWidth = value; SetMaterialProperties(); } }
        }

        private float AlphaThreshold
        {
            get => alphaThreshold;
            set { if (alphaThreshold != value) { alphaThreshold = value; SetMaterialProperties(); } }
        }
        public bool DrawOutside
        {
            get => drawOutside;
            set { if (drawOutside != value) { drawOutside = value; SetMaterialProperties(); } }
        }
        public bool EnableInstancing
        {
            get => enableInstancing;
            set { if (enableInstancing != value) { enableInstancing = value; SetMaterialProperties(); } }
        }

        [Tooltip("Base color of the glow.")]
        [SerializeField] private Color glowColor = Color.white;
        [Tooltip("The brightness (power) of the glow."), Range(1, 10)]
        [SerializeField] private float glowBrightness = 2f;
        [Tooltip("Width of the outline, in texels."), Range(0, 10)]
        [SerializeField] private int outlineWidth = 1;
        [Tooltip("Threshold to determine sprite borders."), Range(0f, 1f)]
        [SerializeField] private float alphaThreshold = .01f;
        [Tooltip("Whether the outline should only be drawn outside of the sprite borders. Make sure sprite texture has sufficient transparent space for the required outline width.")]
        [SerializeField] private bool drawOutside = false;
        [Tooltip("Whether to enable GPU instancing.")]
        [SerializeField] private bool enableInstancing = false;

        private static readonly int IsOutlineEnabledId = Shader.PropertyToID("_IsOutlineEnabled");
        private static readonly int OutlineColorId = Shader.PropertyToID("_OutlineColor");
        private static readonly int OutlineSizeId = Shader.PropertyToID("_OutlineSize");
        private static readonly int AlphaThresholdId = Shader.PropertyToID("_AlphaThreshold");

        private MaterialPropertyBlock _materialProperties;

        private void Awake ()
        {
            Renderer = GetComponent<SpriteRenderer>();
        }

        private void OnEnable ()
        {
            SetMaterialProperties();
        }

        private void OnDisable ()
        {
            SetMaterialProperties();
        }

        private void OnValidate ()
        {
            if (!isActiveAndEnabled) return;
            SetMaterialProperties();
        }

        private void OnDidApplyAnimationProperties ()
        {
            SetMaterialProperties();
        }

        private void SetMaterialProperties ()
        {
            if (!Renderer) return;

            Renderer.sharedMaterial = SpriteOutlineMat.GetSharedFor(this);

            if (_materialProperties == null) // Initializing it at `Awake` or `OnEnable` causes nullref in editor in some cases.
                _materialProperties = new MaterialPropertyBlock();

            _materialProperties.SetFloat(IsOutlineEnabledId, isActiveAndEnabled ? 1 : 0);
            _materialProperties.SetColor(OutlineColorId, GlowColor * GlowBrightness);
            _materialProperties.SetFloat(OutlineSizeId, OutlineWidth);
            _materialProperties.SetFloat(AlphaThresholdId, AlphaThreshold);

            Renderer.SetPropertyBlock(_materialProperties);
        }
    }
}
