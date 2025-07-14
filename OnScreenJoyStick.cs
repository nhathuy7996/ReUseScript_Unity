using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.OnScreen;
using DG.Tweening;
using UnityEngine.InputSystem.EnhancedTouch;

namespace Huynn.Lib
{
    public class OnScreenJoyStick : OnScreenControl, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        public RectTransform handle, backGround;
        public float movementRange = 50f;

        private Vector2? startPos = null;
        private Vector2 defaultPos;
        private bool isDragging;

        protected override string controlPathInternal { get => m_ControlPath; set => m_ControlPath = value; }
        [SerializeField] private string m_ControlPath = "<Gamepad>/leftStick";

        Tweener tweenerHandle, tweenerBG;

        int _activeTouchId = -1;

        void Start()
        {
            if (handle == null) handle = GetComponent<RectTransform>();
            defaultPos = handle.anchoredPosition;
        }

        void Update()
        {
            if (UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches.Count == 0 && isDragging)
                this.OnPointerUp(null);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            this.tweenerHandle?.Kill();
            this.tweenerBG?.Kill();
            isDragging = true;
            if (startPos == null)
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                handle.parent as RectTransform, eventData.position, eventData.pressEventCamera, out var pos);
                this.startPos = pos;
                this.tweenerBG = DOTweenModuleUI.DOAnchorPos(backGround, pos, 0.2f).SetEase(Ease.OutBack);
                this._activeTouchId = eventData.pointerId;
            }
            OnDrag(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!isDragging) return;
            if (eventData.pointerId != _activeTouchId) return;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                handle.parent as RectTransform, eventData.position, eventData.pressEventCamera, out var pos);
            Vector2 delta = pos - (Vector2)startPos;
            delta = Vector2.ClampMagnitude(delta, movementRange);

            this.tweenerHandle = DOTweenModuleUI.DOAnchorPos(handle, (Vector2)startPos + delta, 0.2f).SetEase(Ease.OutBack);
            SendValueToControl(delta / movementRange);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            isDragging = false;

            this.tweenerHandle?.Kill();
            this.tweenerBG?.Kill();

            this.tweenerHandle = DOTweenModuleUI.DOAnchorPos(handle, defaultPos, 0.5f).SetEase(Ease.OutBack);
            this.tweenerBG = DOTweenModuleUI.DOAnchorPos(backGround, defaultPos, 0.5f).SetEase(Ease.OutBack);
            this.startPos = null;
            this._activeTouchId = -1;
            SendValueToControl(Vector2.zero);
        }
    }
}