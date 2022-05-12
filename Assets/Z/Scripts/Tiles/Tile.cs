using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace Zio
{
    public enum CubeAnimationState
    {
        IsDefault,
        IsDamage
    }

    public enum TileType
    {
        IsDefault,
        IsDamage,
        IsPoint,
        IsSafe,
        IsPointVanishing,
        IsPointCollected
    }

    [Serializable]
    public class Tile : MonoBehaviour
    {
        [SerializeField] private Color[] _colors =
            {new(0f, 0f, 0f, 0.2f), Color.red, Color.green, Color.blue, new(1f, 0f, 0f, 0.5f)};

        public CubeAnimationState cubeData;
        private int _baseMap;
        private int _colorId;
        private float _colorIntensity = 2.5f;

        private Color _currentColor;

        private int _emissionColorId;
        private int _emissionColorIdHDRP;

        private bool _manualOverride;

        private WaitForSeconds _pointTime = new(1.5f);
        private WaitForSeconds _pointVanishingTime = new(1.5f);

        private MaterialPropertyBlock _propBlock;
        private Renderer _renderer;
        private TileType _tileType;


        private void Start()
        {
            _baseMap = Shader.PropertyToID("_BaseColor");
            _emissionColorId = Shader.PropertyToID("_EmissionColor");
            _emissionColorIdHDRP = Shader.PropertyToID("_EmissiveColor");
            _colorId = Shader.PropertyToID("_BaseColor");
            _propBlock = new MaterialPropertyBlock();
            _renderer = GetComponent<Renderer>();
        }

        public void Reset()
        {
            StopAllCoroutines();
            cubeData = CubeAnimationState.IsDefault;
            _tileType = TileType.IsDefault;
            _manualOverride = false;
            EventManager.TriggerEvent(On.DeSpawnedPoint, gameObject);
            UpdateTile();
        }

        private void OnEnable()
        {
            EventManager.StartListening(On.UpdateTileAnimation, OnUpdateTileAnimation);
        }


        private void OnDisable()
        {
            EventManager.StopListening(On.UpdateTileAnimation, OnUpdateTileAnimation);
        }


//#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            UpdateTile(true);
        }

        private void OnValidate()
        {
            _emissionColorId = Shader.PropertyToID("_EmissionColor");
            _emissionColorIdHDRP = Shader.PropertyToID("_EmissiveColor");
            _colorId = Shader.PropertyToID("_Color");
            _propBlock = new MaterialPropertyBlock();
            _renderer = GetComponent<Renderer>();
            UpdateTile(true);
        }

        private void OnUpdateTileAnimation()
        {
            if (_manualOverride) return;
            StopAllCoroutines();
            UpdateTile(true);
        }

        public void ChangeType(TileType type)
        {
            StopAllCoroutines();
            _tileType = type;
            _currentColor = _colors[(int) type];
            _manualOverride = true;
            UpdateTile();
        }

        public bool IsDamage()
        {
            return _tileType is TileType.IsDamage;
        }
        public bool IsPoint()
        {
            return _tileType is TileType.IsPoint or TileType.IsPointVanishing;
        }


        private void SetColor(float intensity = 0f)
        {
            if (intensity == 0f) intensity = _colorIntensity;
            var color = _currentColor * intensity;
            _renderer.GetPropertyBlock(_propBlock);
            _propBlock.SetColor(_emissionColorId, color);
            _propBlock.SetColor(_emissionColorIdHDRP, color);
            _propBlock.SetColor(_colorId, color);
            _propBlock.SetColor(_baseMap, color);
            _propBlock.SetColor(Shader.PropertyToID("_BaseColor"), color);
            _renderer.SetPropertyBlock(_propBlock);
        }

        private void UpdateTile(bool isAnimation = false)
        {
            switch (isAnimation)
            {
                case true:
                    _tileType = (TileType) cubeData;
                    _currentColor = _colors[(int) cubeData];
                    SetColor();
                    break;
                case false:
                    switch (_tileType)
                    {
                        case TileType.IsPoint:
                            StartCoroutine(SpawnPointAnimation());
                            break;
                        case TileType.IsPointVanishing:
                            StartCoroutine(VanishPointAnimation());
                            break;
                        case TileType.IsPointCollected:
                            DOVirtual.Float(2f, _colorIntensity, 0.3f, SetColor).SetEase(Ease.InOutFlash).onComplete+=Reset;
                            break;
                        case TileType.IsSafe:
                            SetColor();
                            break;
                        case TileType.IsDamage:
                        case TileType.IsDefault:
                        default:
                            SetColor();
                            break;
                    }

                    break;
            }
        }

        private IEnumerator VanishPointAnimation()
        {
            DOVirtual.Float(_colorIntensity, _colorIntensity/2f, 0.6f, SetColor).SetLoops(3).SetEase(Ease.InOutFlash);
            yield return _pointVanishingTime;
            EventManager.TriggerEvent(On.PointTimedOut,null);
            Reset();
        }

        private IEnumerator SpawnPointAnimation()
        {
            DOVirtual.Float(_colorIntensity/2f ,_colorIntensity, 0.4f, SetColor).SetEase(Ease.OutBounce);
            yield return _pointTime;
            ChangeType(TileType.IsPointVanishing);
        }
    }
}