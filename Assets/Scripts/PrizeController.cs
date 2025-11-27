using UnityEngine;
using UnityEngine.InputSystem;

public class PrizeController : MonoBehaviour
{
    // Rotación
    private Vector2 lastTouchPosition;
    private bool isDragging = false;
    private float rotationSpeed = 0.2f;

    // Zoom
    private float lastPinchDistance = 0f;
    private float pinchZoomSpeed = 0.01f;
    private float minScale = 0.2f;
    private float maxScale = 1f;

    // Pan (desplazamiento)
    private Vector2 lastPanPos;

    void Update()
    {
        HandleRotation();
        HandleZoom();
        //HandlePan();
    }

    private void HandleRotation()
    {
        // PC con ratón
        if (Mouse.current != null && Mouse.current.leftButton.isPressed)
        {
            Vector2 mouseDelta = Mouse.current.delta.ReadValue();
            transform.Rotate(Vector3.up, -mouseDelta.x * rotationSpeed, Space.World);
            transform.Rotate(Vector3.right, mouseDelta.y * rotationSpeed, Space.World);
        }

        // Móvil con un dedo
        if (Touchscreen.current != null && Touchscreen.current.touches.Count == 1)
        {
            var touch = Touchscreen.current.touches[0];
            if (touch.press.isPressed)
            {
                Vector2 touchPos = touch.position.ReadValue();
                if (!isDragging)
                {
                    lastTouchPosition = touchPos;
                    isDragging = true;
                }
                else
                {
                    Vector2 delta = touchPos - lastTouchPosition;
                    transform.Rotate(Vector3.up, -delta.x * rotationSpeed, Space.World);
                    transform.Rotate(Vector3.right, delta.y * rotationSpeed, Space.World);
                    lastTouchPosition = touchPos;
                }
            }
            else
            {
                isDragging = false;
            }
        }
        else
        {
            isDragging = false;
        }
    }

    private void HandleZoom()
    {
        float zoomDelta = 0f;

        // PC con rueda del ratón
        if (Mouse.current != null)
        {
            float scroll = Mouse.current.scroll.ReadValue().y;
            zoomDelta = scroll * 0.1f; // Sensibilidad
        }

        // Móvil con pinch
        if (Touchscreen.current != null && Touchscreen.current.touches.Count >= 2)
        {
            var touch0 = Touchscreen.current.touches[0].position.ReadValue();
            var touch1 = Touchscreen.current.touches[1].position.ReadValue();

            float currentDistance = Vector2.Distance(touch0, touch1);
            if (lastPinchDistance > 0)
                zoomDelta = (currentDistance - lastPinchDistance) * pinchZoomSpeed;

            lastPinchDistance = currentDistance;
        }
        else
        {
            lastPinchDistance = 0f;
        }

        // Aplicar zoom
        if (zoomDelta != 0f)
        {
            Vector3 scale = transform.localScale;
            scale += Vector3.one * zoomDelta;
            scale = Vector3.Max(scale, Vector3.one * minScale);
            scale = Vector3.Min(scale, Vector3.one * maxScale);
            transform.localScale = scale;
        }

    }

    private void HandlePan()
    {
        Vector2 panDelta = Vector2.zero;

        // PC con botón derecho
        if (Mouse.current != null && Mouse.current.rightButton.isPressed)
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            if (lastPanPos == Vector2.zero) lastPanPos = mousePos;
            panDelta = mousePos - lastPanPos;
            lastPanPos = mousePos;
        }
        else if (Touchscreen.current != null && Touchscreen.current.touches.Count == 2)
        {
            var touch0 = Touchscreen.current.touches[0].position.ReadValue();
            var touch1 = Touchscreen.current.touches[1].position.ReadValue();
            Vector2 midPoint = (touch0 + touch1) * 0.5f;

            if (lastPanPos == Vector2.zero) lastPanPos = midPoint;
            panDelta = midPoint - lastPanPos;
            lastPanPos = midPoint;
        }
        else
        {
            lastPanPos = Vector2.zero;
        }

        // Aplicar desplazamiento
        if (panDelta != Vector2.zero)
        {
            transform.Translate(new Vector3(panDelta.x, panDelta.y, 0) * 0.001f, Space.Self);
        }
    }
}
