using AxGrid;
using AxGrid.Base;
using AxGrid.Model;
using UnityEngine;

namespace Slot
{
    [DefaultExecutionOrder(-100)]
    public class SlotController : MonoBehaviourExtBind
    {
        [Header("Scroll")]
        [SerializeField] private float cellHeight = 200f;
        [SerializeField] private float maxSpeed = 1200f;
        [SerializeField] private float acceleration = 400f;
        [SerializeField] private float deceleration = 800f;
        [SerializeField] private float snapDuration = 0.3f;

        private float _totalScrollY;
        private float _speed;
        private bool _rolling;
        private bool _stopping;
        private float _snapTimer = -2f;
        private float _snapFromY;

        public float TotalScrollY => _totalScrollY;
        public float CellHeight => cellHeight;
        public float MaxSpeed => maxSpeed;
        public float Speed => _speed;
        public bool Rolling => _rolling;
        public bool Stopping => _stopping;

        [OnEnable]
        private void OnEnableThis() { }

        [OnStart]
        private void StartThis()
        {
            _totalScrollY = 0f;
            _speed = 0f;
            _rolling = false;
            _stopping = false;
            _snapTimer = -2f;
        }

        [Bind("SlotStartScroll")]
        private void OnStartScroll()
        {
            _rolling = true;
            _stopping = false;
            _speed = 0f;
        }

        [Bind("SlotRequestStop")]
        private void OnRequestStop()
        {
            if (!_rolling) return;
            _stopping = true;
        }

        [OnUpdate]
        private void UpdateThis()
        {
            if (_stopping)
            {
                _speed -= deceleration * Time.deltaTime;
                if (_speed <= 0f)
                {
                    _speed = 0f;
                    _rolling = false;
                    _stopping = false;
                    StartSnapToCenter();
                    return;
                }
                _totalScrollY += _speed * Time.deltaTime;
                return;
            }
            if (_rolling)
            {
                _speed += acceleration * Time.deltaTime;
                if (_speed > maxSpeed) _speed = maxSpeed;
                _totalScrollY += _speed * Time.deltaTime;
                return;
            }
            if (_snapTimer >= 0f)
            {
                _snapTimer -= Time.deltaTime;
                float t = 1f - Mathf.Clamp01(_snapTimer / snapDuration);
                t = 1f - (1f - t) * (1f - t);
                float snapTarget = Mathf.Round(_snapFromY / cellHeight) * cellHeight;
                _totalScrollY = Mathf.Lerp(_snapFromY, snapTarget, t);
                if (_snapTimer <= 0f)
                {
                    _totalScrollY = Mathf.Round(_totalScrollY / cellHeight) * cellHeight;
                    _snapTimer = -2f;
                    Settings.Fsm.Invoke("SlotStopped");
                    Settings.Model.EventManager.Invoke("SlotStopped");
                }
                return;
            }
            if (_snapTimer > -2f && _snapTimer <= 0f)
            {
                _snapTimer = -2f;
                Settings.Fsm.Invoke("SlotStopped");
                Settings.Model.EventManager.Invoke("SlotStopped");
                return;
            }

            _totalScrollY += _speed * Time.deltaTime;
        }

        private void StartSnapToCenter()
        {
            _snapFromY = _totalScrollY;
            _snapTimer = snapDuration;
        }
    }
}
