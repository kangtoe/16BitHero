using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header(" Elements ")]
    [SerializeField] private Transform target;
    [SerializeField] private Camera mainCamera;

    [Header(" Settings ")]
    [SerializeField] private bool limitCameraMovement = true;
    [SerializeField] private Vector2 minMaxXY;
    

    private void LateUpdate()
    {
        if(target == null)
        {
            Debug.LogWarning("No target has been specified... Komonnnn'''' ");
            return;
        }

        if(mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        Vector3 targetPosition = target.position;
        targetPosition.z = -10;

        if(limitCameraMovement)
        {
            float cameraHeight = mainCamera.orthographicSize;
            float cameraWidth = cameraHeight * mainCamera.aspect;

            // 카메라 가장자리가 minMaxXY 범위를 벗어나지 않도록 조정
            targetPosition.x = Mathf.Clamp(targetPosition.x, -minMaxXY.x + cameraWidth, minMaxXY.x - cameraWidth);
            targetPosition.y = Mathf.Clamp(targetPosition.y, -minMaxXY.y + cameraHeight, minMaxXY.y - cameraHeight);
        }

        transform.position = targetPosition;
    }

    private void OnDrawGizmos()
    {
        #if UNITY_EDITOR
        if(limitCameraMovement)
        {
            Gizmos.color = Color.yellow;
            Vector3 center = Vector3.zero;
            Vector3 size = new Vector3(minMaxXY.x * 2, minMaxXY.y * 2, 0.1f);
            Gizmos.DrawWireCube(center, size);
        }
        #endif
    }
}
