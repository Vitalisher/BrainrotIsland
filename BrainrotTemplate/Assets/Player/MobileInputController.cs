using System;
using UnityEngine;
using UnityEngine.UI;
using YG;

public class MobileInputController : MonoBehaviour
{
    [Header("Joystick Reference")]
    public Joystick movementJoystick;
    public Button jumpButton;

    [Header("Camera Touch Settings")]
    public float touchSensitivity = 1f;
    public RectTransform joystickArea; // Область джойстика для исключения

    private int cameraFingerID = -1;
    private Vector2 lastTouchPosition;

    public float HorizontalInput { get; private set; }
    public float VerticalInput { get; private set; }
    public float MouseXInput { get; private set; }
    public float MouseYInput { get; private set; }

    void Update()
    {
        if (TimerBeforeAdsYG.pouse == false)
        {
            if (DeviceChecker.IsMobile)
            {
                HandleJoystickInput();
                HandleCameraTouch();
            }
            else
            {
                if (movementJoystick != null) movementJoystick.gameObject.SetActive(false);
                if (jumpButton != null) jumpButton.gameObject.SetActive(false);
                HorizontalInput = 0f;
                VerticalInput = 0f;
                MouseXInput = 0f;
                MouseYInput = 0f;
            }
        }
    }

    void HandleJoystickInput()
    {
        if (movementJoystick != null)
        {
            HorizontalInput = movementJoystick.Horizontal;
            VerticalInput = movementJoystick.Vertical;
        }
    }

    void HandleCameraTouch()
    {
        MouseXInput = 0f;
        MouseYInput = 0f;

        // Проверяем все активные касания
        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch touch = Input.touches[i];

            // Проверяем, не попадает ли касание в область джойстика
            if (IsTouchOverJoystick(touch.position))
            {
                // Если это наш палец для камеры - сбрасываем его
                if (touch.fingerId == cameraFingerID)
                {
                    cameraFingerID = -1;
                }
                continue;
            }

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    // Назначаем новый палец для камеры только если его еще нет
                    if (cameraFingerID == -1)
                    {
                        cameraFingerID = touch.fingerId;
                        lastTouchPosition = touch.position;
                    }
                    break;

                case TouchPhase.Moved:
                case TouchPhase.Stationary:
                    if (touch.fingerId == cameraFingerID)
                    {
                        Vector2 delta = touch.position - lastTouchPosition;
                        MouseXInput = delta.x * touchSensitivity * 0.1f;
                        MouseYInput = delta.y * touchSensitivity * 0.1f;
                        lastTouchPosition = touch.position;
                    }
                    break;

                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    if (touch.fingerId == cameraFingerID)
                    {
                        cameraFingerID = -1;
                    }
                    break;
            }
        }

        // Если палец для камеры потерян (вышел за пределы или убран), сбрасываем ID
        if (cameraFingerID != -1)
        {
            bool fingerStillActive = false;
            for (int i = 0; i < Input.touchCount; i++)
            {
                if (Input.touches[i].fingerId == cameraFingerID)
                {
                    fingerStillActive = true;
                    break;
                }
            }

            if (!fingerStillActive)
            {
                cameraFingerID = -1;
            }
        }
    }

    bool IsTouchOverJoystick(Vector2 touchPosition)
    {
        // Простая проверка по левой части экрана
        if (touchPosition.x < Screen.width * 0.35f)
            return true;

        // Дополнительная проверка через RectTransform если задан
        if (joystickArea != null)
        {
            return RectTransformUtility.RectangleContainsScreenPoint(joystickArea, touchPosition);
        }

        return false;
    }
}