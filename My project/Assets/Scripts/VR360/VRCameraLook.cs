using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace VR360
{
    /// <summary>
    /// Zorgt voor 360° rondkijken met muis en toetsenbord (WASD / Pijltjestoetsen)
    /// volledig compatibel met Unity's New Input System.
    /// </summary>
    public class VRCameraLook : MonoBehaviour
    {
        [Header("Muis Rondkijken")]
        [Tooltip("Gevoeligheid van de muis")]
        [SerializeField] private float mouseSensitivity = 0.15f;
        [Tooltip("Alleen draaien als de muisknop ingedrukt is")]
        [SerializeField] private bool requireMouseButton = true;
        [SerializeField] private bool invertY = false;

        [Header("Toetsenbord Rondkijken (WASD / Pijltjes)")]
        [Tooltip("Draaisnelheid via het toetsenbord in graden per seconde")]
        [SerializeField] private float keyboardTurnSpeed = 75f;

        [Header("XR Gedrag")]
        [Tooltip("Schakel muis/toetsenbord uit als een actieve VR headset tracking data levert")]
        [SerializeField] private bool disableWhenXRActive = false;

        private float yaw = 0f;
        private float pitch = 0f;

        private void Start()
        {
            Vector3 currentAngles = transform.localEulerAngles;
            yaw = currentAngles.y;
            pitch = currentAngles.x > 180 ? currentAngles.x - 360 : currentAngles.x;
        }

        private void Update()
        {
#if ENABLE_INPUT_SYSTEM
            UpdateWithNewInputSystem();
#else
            UpdateWithLegacyInput();
#endif
        }

#if ENABLE_INPUT_SYSTEM
        private void UpdateWithNewInputSystem()
        {
            float rotX = 0f;
            float rotY = 0f;

            // 1. Muis Input
            var mouse = Mouse.current;
            if (mouse != null)
            {
                bool isDragging = !requireMouseButton || mouse.rightButton.isPressed || mouse.leftButton.isPressed;
                if (isDragging)
                {
                    Vector2 delta = mouse.delta.ReadValue();
                    rotX += delta.x * mouseSensitivity;
                    rotY += (invertY ? delta.y : -delta.y) * mouseSensitivity;
                }
            }

            // 2. Toetsenbord Input (WASD & Pijltjestoetsen)
            var kb = Keyboard.current;
            if (kb != null)
            {
                float keyHorizontal = 0f;
                float keyVertical = 0f;

                if (kb.leftArrowKey.isPressed || kb.aKey.isPressed) keyHorizontal -= 1f;
                if (kb.rightArrowKey.isPressed || kb.dKey.isPressed) keyHorizontal += 1f;
                if (kb.upArrowKey.isPressed || kb.wKey.isPressed) keyVertical += 1f;
                if (kb.downArrowKey.isPressed || kb.sKey.isPressed) keyVertical -= 1f;

                rotX += keyHorizontal * keyboardTurnSpeed * Time.deltaTime;
                rotY += (invertY ? -keyVertical : keyVertical) * keyboardTurnSpeed * Time.deltaTime;
            }

            ApplyRotation(rotX, rotY);
        }
#endif

        private void UpdateWithLegacyInput()
        {
            float rotX = 0f;
            float rotY = 0f;

            bool isDragging = !requireMouseButton || Input.GetMouseButton(0) || Input.GetMouseButton(1);
            if (isDragging)
            {
                rotX += Input.GetAxis("Mouse X") * mouseSensitivity * 10f;
                rotY += (invertY ? Input.GetAxis("Mouse Y") : -Input.GetAxis("Mouse Y")) * mouseSensitivity * 10f;
            }

            float keyHorizontal = Input.GetAxisRaw("Horizontal");
            float keyVertical = Input.GetAxisRaw("Vertical");

            rotX += keyHorizontal * keyboardTurnSpeed * Time.deltaTime;
            rotY += (invertY ? -keyVertical : keyVertical) * keyboardTurnSpeed * Time.deltaTime;

            ApplyRotation(rotX, rotY);
        }

        private void ApplyRotation(float deltaYaw, float deltaPitch)
        {
            if (Mathf.Approximately(deltaYaw, 0f) && Mathf.Approximately(deltaPitch, 0f))
                return;

            yaw += deltaYaw;
            pitch += deltaPitch;
            pitch = Mathf.Clamp(pitch, -89f, 89f);

            transform.localRotation = Quaternion.Euler(pitch, yaw, 0f);
        }
    }
}
