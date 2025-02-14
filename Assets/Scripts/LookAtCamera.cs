using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    public Camera targetCamera; // 카메라 변수

    void Update()
    {
        if (targetCamera != null)
        {
            // 카메라를 바라보게 회전
            transform.LookAt(targetCamera.transform);
        }
    }
}