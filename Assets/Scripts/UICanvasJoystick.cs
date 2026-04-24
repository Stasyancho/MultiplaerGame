using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UICanvasJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [SerializeField] private RectTransform backgroundRect;
    [SerializeField] private RectTransform handleRect;
    [SerializeField] private float handleRange = 80f;
    

    public Action<Vector2> OnJoystickPressed;
    public Action<Vector2> OnJoystickReleased;
    public Action OnJoystickCancelled;
    public Action<Vector2> OnJoystickPositionChanged;

    private void Awake()
    {
        if (backgroundRect == null)
            backgroundRect = GetComponent<RectTransform>();
        if (handleRect == null)
            handleRect = GetComponentInChildren<Image>()?.rectTransform;

        handleRect.anchoredPosition = Vector2.zero;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        SetHandlePosition(eventData, out var direction);
        OnJoystickPressed?.Invoke(direction);
    }

    public void OnDrag(PointerEventData eventData)
    {
        SetHandlePosition(eventData, out var direction);
        OnJoystickPositionChanged?.Invoke(direction);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        SetHandlePosition(eventData, out var direction);
        if (direction == Vector2.zero)
        {
            OnJoystickCancelled?.Invoke();
        }
        else
        {
            OnJoystickReleased?.Invoke(direction);
            handleRect.anchoredPosition = Vector2.zero;
        }
    }

    private void SetHandlePosition(PointerEventData eventData, out Vector2 direction)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(backgroundRect, eventData.position, eventData.pressEventCamera, out var localPoint);
        
        float distance = Vector2.Distance(Vector2.zero, localPoint);
        if (distance > handleRange)
            localPoint = localPoint.normalized * handleRange;

        if (localPoint.magnitude / handleRange < 0.25f)
            localPoint = Vector2.zero;
        
        handleRect.anchoredPosition = localPoint;

        direction = localPoint / handleRange;
    }
}