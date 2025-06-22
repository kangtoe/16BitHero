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
    [SerializeField] private Vector2 boundsAdjust = Vector2.zero; // wallBounds 영역 조정값

    private void LateUpdate()
    {
        if (target == null)
        {
            Debug.LogWarning("No target has been specified... Komonnnn'''' ");
            return;
        }

        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        Vector3 targetPosition = target.position;
        targetPosition.z = -10;

        if (limitCameraMovement)
        {
            float cameraHeight = mainCamera.orthographicSize;
            float cameraWidth = cameraHeight * mainCamera.aspect;

            Bounds bounds = ArenaArea.Instance.wallBounds;
            // boundsAdjust만큼 영역 확장/축소
            bounds.Expand(new Vector3(boundsAdjust.x, boundsAdjust.y, 0f));

            float minX = bounds.min.x + cameraWidth;
            float maxX = bounds.max.x - cameraWidth;
            float minY = bounds.min.y + cameraHeight;
            float maxY = bounds.max.y - cameraHeight;

            targetPosition.x = Mathf.Clamp(targetPosition.x, minX, maxX);
            targetPosition.y = Mathf.Clamp(targetPosition.y, minY, maxY);
        }

        transform.position = targetPosition;
    }

    private void OnDrawGizmosSelected()
    {
        if (limitCameraMovement)
        {
            Gizmos.color = Color.yellow;
            if (ArenaArea.Instance)
            {
                Bounds bounds = ArenaArea.Instance.wallBounds;
                bounds.Expand(new Vector3(boundsAdjust.x, boundsAdjust.y, 0f));
                Gizmos.DrawWireCube(bounds.center, bounds.size);
            }
            else
            {
                // 에디터에서 wallBounds를 사용할 수 없을 때는 minMaxXY로 대체 (구버전 호환)
                //Vector3 center = Vector3.zero;
                //Vector3 size = new Vector3(minMaxXY.x * 2, minMaxXY.y * 2, 0.1f);
                //Gizmos.DrawWireCube(center, size);
            }
        }
    }
}
