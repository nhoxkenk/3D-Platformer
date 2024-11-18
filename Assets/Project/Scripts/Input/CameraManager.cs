using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Platformer
{
    public class CameraManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] InputReader input;
        [SerializeField] CinemachineFreeLook freeLookVCam;

        [Header("Settings")]
        [SerializeField, Range(0.5f, 3f)] float speedMultiplier;

        bool isRMBPressed;
        bool isDeviceMouse;
        bool cameraMovementLock;

        private void OnEnable()
        {
            input.Look += OnLook;
            input.EnableMouseControlCamera += OnEnableMouseControlCamera;
            input.DisableMouseControlCamera += OnDisableMouseControlCamera;
        }

        private void OnDisable()
        {
            input.Look -= OnLook;
            input.EnableMouseControlCamera -= OnEnableMouseControlCamera;
            input.DisableMouseControlCamera -= OnDisableMouseControlCamera;
        }

        void OnLook(Vector2 cameraMovement, bool isDeviceMouse)
        {
            if(cameraMovementLock) return;

            if(isDeviceMouse && isRMBPressed) return;

            //If the device is mouse then use fixedDeltaTime, otherwise use deltaTime
            float deviceMultiplier = isDeviceMouse ? Time.fixedDeltaTime : Time.deltaTime;

            //Set the camera axis values
            freeLookVCam.m_XAxis.m_InputAxisValue = cameraMovement.x * speedMultiplier * deviceMultiplier;
            freeLookVCam.m_YAxis.m_InputAxisValue = cameraMovement.y * speedMultiplier * deviceMultiplier;
        }

        void OnEnableMouseControlCamera()
        {
            isRMBPressed = true;

            //Lock the cursor to the center of the screen and Hide it
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            StartCoroutine(DisableMouseForFrame());
        }
        IEnumerator DisableMouseForFrame()
        {
            cameraMovementLock = true;
            yield return new WaitForEndOfFrame();
            cameraMovementLock = false;
        }
        void OnDisableMouseControlCamera()
        {
            isRMBPressed = false;

            //Unlock the cursor and make it visible
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            //Reset the camera axis to prevent jumping when re-enabling mouse control
            freeLookVCam.m_XAxis.m_InputAxisValue = 0f;
            freeLookVCam.m_YAxis.m_InputAxisValue = 0f;
        }
    }
}
