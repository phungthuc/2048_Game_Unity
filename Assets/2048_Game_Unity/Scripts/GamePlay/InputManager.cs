using System;
using UnityEngine;
using UnityEngine.Events;

public class InputManager : MonoBehaviour
{
    private Vector2 fingerDownPosition;
    private Vector2 fingerUpPosition;

    // Khoảng cách tối thiểu trước khi xử lý di chuyển
    public float minDistanceForSwipe = 40f;
    public UnityEvent eventMoveRight;
    public UnityEvent eventMoveLeft;
    public UnityEvent eventMoveUp;
    public UnityEvent eventMoveDown;

    private void OnGUI()
    {
        //Keyboard
        Event e = Event.current;
        switch (e.keyCode)
        {
            case KeyCode.UpArrow:
                eventMoveUp.Invoke();
                break;
            case KeyCode.LeftArrow:
                eventMoveLeft.Invoke();
                break;
            case KeyCode.DownArrow:
                eventMoveDown.Invoke();
                break;
            case KeyCode.RightArrow:
                eventMoveRight.Invoke();
                break;
            default:
                break;
        }

        //Touch
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                fingerDownPosition = touch.position;
                fingerUpPosition = touch.position;
            }

            if (touch.phase == TouchPhase.Ended)
            {
                fingerUpPosition = touch.position;
                CheckSwipe();
            }
        }
    }

    private void CheckSwipe()
    {
        float deltaX = fingerUpPosition.x - fingerDownPosition.x;
        float deltaY = fingerUpPosition.y - fingerDownPosition.y;

        if (Mathf.Abs(deltaX) > minDistanceForSwipe || Mathf.Abs(deltaY) > minDistanceForSwipe)
        {
            if (Mathf.Abs(deltaX) > Mathf.Abs(deltaY))
            {
                if (deltaX > 0)
                {
                    eventMoveRight.Invoke();
                }
                else
                {
                    eventMoveLeft.Invoke();
                }
            }
            else
            {
                if (deltaY > 0)
                {
                    eventMoveUp.Invoke();
                }
                else
                {
                    eventMoveDown.Invoke();
                }
            }
        }
    }
}