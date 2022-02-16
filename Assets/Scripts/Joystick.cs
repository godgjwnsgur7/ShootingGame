using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Joystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    Image imageBackgroud;
    Image imageController;

    Vector2 touchPosition;

    void Awake()
    {
        imageBackgroud = GetComponent<Image>();
        imageController = transform.GetChild(0).GetComponent<Image>();
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        touchPosition = Vector2.zero;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            imageBackgroud.rectTransform, eventData.position, eventData.pressEventCamera, out touchPosition))
        {
            touchPosition.x = (touchPosition.x / imageBackgroud.rectTransform.sizeDelta.x);
            touchPosition.y = (touchPosition.y / imageBackgroud.rectTransform.sizeDelta.y);

            touchPosition = new Vector2(touchPosition.x * 2 - 1, touchPosition.y * 2 - 1);

            touchPosition = (touchPosition.magnitude > 1) ? touchPosition.normalized : touchPosition;

            imageController.rectTransform.anchoredPosition = new Vector2(
                touchPosition.x * imageBackgroud.rectTransform.sizeDelta.x * 0.5f,
                touchPosition.y * imageBackgroud.rectTransform.sizeDelta.y * 0.5f);
        }
    }
    public void OnDrag(PointerEventData eventData)
    {
        if( RectTransformUtility.ScreenPointToLocalPointInRectangle(
            imageBackgroud.rectTransform, eventData.position, eventData.pressEventCamera, out touchPosition))
        {
            touchPosition.x = (touchPosition.x / imageBackgroud.rectTransform.sizeDelta.x);
            touchPosition.y = (touchPosition.y / imageBackgroud.rectTransform.sizeDelta.y);

            touchPosition = new Vector2(touchPosition.x * 2 - 1, touchPosition.y * 2 - 1);

            touchPosition = (touchPosition.magnitude > 1) ? touchPosition.normalized : touchPosition;

            imageController.rectTransform.anchoredPosition = new Vector2(
                touchPosition.x * imageBackgroud.rectTransform.sizeDelta.x * 0.5f,
                touchPosition.y * imageBackgroud.rectTransform.sizeDelta.y * 0.5f);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        imageController.rectTransform.anchoredPosition = Vector2.zero;
        touchPosition = Vector2.zero;
    }

    public float Horizontal()
    {
        return touchPosition.x;
    }

    public float Vertical()
    {
        return touchPosition.y;
    }
}
