using UnityEngine;
using UnityEngine.EventSystems;

public class MyButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("Button State")]
    public bool isPressed;

    [Header("Press Smoothing")]
    public float dampenPress;
    public float sensitivity = 2f;

    void Update()
    {
        if (isPressed)
            dampenPress += sensitivity * Time.deltaTime;
        else
            dampenPress -= sensitivity * Time.deltaTime;

        dampenPress = Mathf.Clamp01(dampenPress);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isPressed = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPressed = false;
    }
}
