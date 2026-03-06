using AxGrid.Base;
using AxGrid.Model;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Slot
{
    public class SlotView : MonoBehaviourExtBind
    {
        [Header("References")]
        [SerializeField] private SlotController controller;
        [SerializeField] private RectTransform viewport;
        [SerializeField] private RectTransform content;
        [SerializeField] private List<Sprite> slotSprites = new List<Sprite>();

        [Header("Wind (speed lines)")]
        [SerializeField] private Image leftWindImage;
        [SerializeField] private Image rightWindImage;
        [SerializeField] private float windAlphaMin = 0.12f;
        [SerializeField] private float windAlphaMax = 0.45f;
        [SerializeField] private float windAlphaFadeSpeed = 1.2f;
        [SerializeField] private float windScrollSpeedBase = 2f;

        [Header("VFX")]
        [SerializeField] private List<ParticleSystem> vfx;

        private readonly List<Transform> _pendingSpriteChange = new List<Transform>();
        private readonly List<float> _prevCellY = new List<float>();
        private float _windScrollOffset;
        private float _currentWindAlpha;
        private float _prevTotalScrollY;

        [OnEnable]
        private void OnEnableThis() { }

        [OnStart]
        private void StartThis()
        {
            content.anchorMin = new Vector2(0.5f, 1f);
            content.anchorMax = new Vector2(0.5f, 1f);
            content.pivot = new Vector2(0.5f, 1f);
            content.anchoredPosition = Vector2.zero;
            _prevTotalScrollY = controller.TotalScrollY;
            _currentWindAlpha = 0f;
            SetWindAlpha(0f);
            _windScrollOffset = 0f;
            EnsureWindMaterialInstances();
            AssignRandomSprites();
            ApplyCellsPosition();
            _prevCellY.Clear();
            for (int i = 0; i < content.childCount; i++)
                _prevCellY.Add((content.GetChild(i) as RectTransform).anchoredPosition.y);
        }

        [Bind("SlotStopped")]
        private void OnSlotStopped()
        {
            for (int i = 0; i < vfx.Count; i++)
                vfx[i].Play();
        }

        [OnUpdate]
        private void UpdateThis()
        {
            float totalScrollY = controller.TotalScrollY;
            float cellHeight = controller.CellHeight;
            float speed = controller.Speed;
            bool rolling = controller.Rolling;
            bool stopping = controller.Stopping;
            bool active = rolling || stopping;
            float maxSpeed = controller.MaxSpeed;

            float targetWindAlpha = active ? Mathf.Lerp(windAlphaMin, windAlphaMax, speed / maxSpeed) : 0f;
            _currentWindAlpha = Mathf.MoveTowards(_currentWindAlpha, targetWindAlpha, windAlphaFadeSpeed * Time.deltaTime);
            SetWindAlpha(_currentWindAlpha);
            float scrollSpeed = active ? windScrollSpeedBase * (0.5f + 0.5f * speed / maxSpeed) : 0f;
            _windScrollOffset += scrollSpeed * Time.deltaTime;
            if (_windScrollOffset > 1f) _windScrollOffset -= 1f;
            if (_windScrollOffset < 0f) _windScrollOffset += 1f;
            SetWindScrollOffset(_windScrollOffset);

            int n = content.childCount;
            int prevTop = n > 0 ? ((int)Mathf.Floor(-_prevTotalScrollY / cellHeight) % n + n) % n : 0;
            int newTop = n > 0 ? ((int)Mathf.Floor(-totalScrollY / cellHeight) % n + n) % n : 0;
            if (newTop != prevTop)
            {
                Transform cellThatLeft = content.GetChild(prevTop);
                if (!_pendingSpriteChange.Contains(cellThatLeft))
                    _pendingSpriteChange.Add(cellThatLeft);
            }
            _prevTotalScrollY = totalScrollY;

            ApplyCellsPosition();

            float visibleBottom = -3f * cellHeight;
            for (int i = _pendingSpriteChange.Count - 1; i >= 0; i--)
            {
                Transform cell = _pendingSpriteChange[i];
                float y = (cell as RectTransform).anchoredPosition.y;
                if (y > 0f || y < visibleBottom - cellHeight)
                {
                    var img = cell.GetComponent<Image>();
                    if (slotSprites.Count > 0)
                        img.sprite = slotSprites[Random.Range(0, slotSprites.Count)];
                    _pendingSpriteChange.RemoveAt(i);
                }
            }
        }

        private void ApplyCellsPosition()
        {
            float totalScrollY = controller.TotalScrollY;
            float cellHeight = controller.CellHeight;
            int n = content.childCount;
            float stripPeriod = n * cellHeight;
            while (_prevCellY.Count < n) _prevCellY.Add(0f);
            for (int i = 0; i < n; i++)
            {
                float raw = -totalScrollY - i * cellHeight;
                float phase = ((raw % stripPeriod) + stripPeriod) % stripPeriod;
                float newY = phase < cellHeight ? phase : phase - stripPeriod;
                (content.GetChild(i) as RectTransform).anchoredPosition = new Vector2(0f, newY);
                _prevCellY[i] = newY;
            }
        }

        private void AssignRandomSprites()
        {
            for (int i = 0; i < content.childCount; i++)
            {
                var child = (RectTransform)content.GetChild(i);
                var img = child.GetComponent<Image>();
                if (slotSprites.Count > 0)
                    img.sprite = slotSprites[Random.Range(0, slotSprites.Count)];
            }
        }

        public Sprite GetCenterCellSprite()
        {
            if (content.childCount == 0) return null;
            float totalScrollY = controller.TotalScrollY;
            float cellHeight = controller.CellHeight;
            int n = content.childCount;
            int topIndex = ((int)Mathf.Floor(-totalScrollY / cellHeight) % n + n) % n;
            int centerIndex = (topIndex + 1) % n;
            return content.GetChild(centerIndex).GetComponent<Image>().sprite;
        }

        private void EnsureWindMaterialInstances()
        {
            if (!leftWindImage.material.name.Contains("(Instance)"))
                leftWindImage.material = new Material(leftWindImage.material);
            if (!rightWindImage.material.name.Contains("(Instance)"))
                rightWindImage.material = new Material(rightWindImage.material);
        }

        private void SetWindAlpha(float alpha)
        {
            var c = leftWindImage.color; c.a = alpha; leftWindImage.color = c;
            c = rightWindImage.color; c.a = alpha; rightWindImage.color = c;
        }

        private void SetWindScrollOffset(float offsetY)
        {
            Vector4 offset = new Vector4(0f, offsetY, 0f, 0f);
            leftWindImage.material.SetVector("_ScrollOffset", offset);
            rightWindImage.material.SetVector("_ScrollOffset", offset);
        }
    }
}
