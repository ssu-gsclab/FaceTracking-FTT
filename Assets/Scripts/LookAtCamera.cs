using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    public Camera targetCamera; // ī�޶� ����

    void Update()
    {
        if (targetCamera != null)
        {
            // ī�޶� �ٶ󺸰� ȸ��
            transform.LookAt(targetCamera.transform);
        }
    }
}