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
                touchPosition.x * imageBackgroud.rectTransform.sizeDelta.x * 0.4f,
                touchPosition.y * imageBackgroud.rectTransform.sizeDelta.y * 0.4f);
        }
    }
    public void OnDrag(PointerEventData eventData)
    {
        // #0. 터치되는 이미지의 중심축(Pivot)을 기준으로 현재 터치 좌표가 중심축으로부터 얼마나 떨어져 있는지를 touchPosition에 저장
        if ( RectTransformUtility.ScreenPointToLocalPointInRectangle(
            imageBackgroud.rectTransform, eventData.position, eventData.pressEventCamera, out touchPosition))
        {
            // #1. touchPosition 값의 정규화 (0 ~ 1)
            touchPosition.x = (touchPosition.x / imageBackgroud.rectTransform.sizeDelta.x);
            touchPosition.y = (touchPosition.y / imageBackgroud.rectTransform.sizeDelta.y);

            // #2. touchPosition 값의 정규화 (-n ~ n)
            touchPosition = new Vector2(touchPosition.x * 2 - 1, touchPosition.y * 2 - 1);
            
            // #3. touchPosition 값의 정규화 (-1 ~ 1)
            touchPosition = (touchPosition.magnitude > 1) ? touchPosition.normalized : touchPosition;

            // #4. 컨트롤러 이미지 이동
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
        if (touchPosition.x > 0.6f)
            touchPosition.x = 1;
        else if (touchPosition.x < -0.6f)
            touchPosition.x = -1;

        return touchPosition.x;
    }

    public float Vertical()
    {
        if (touchPosition.y > 0.6f)
            touchPosition.y = 1;
        else if (touchPosition.y < -0.6f)
            touchPosition.y = -1;

        return touchPosition.y;
    }
}
