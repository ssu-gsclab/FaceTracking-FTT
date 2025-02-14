using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.ARCore;
using Unity.Collections;
using TMPro;
using System.Linq;

public class CameraAngle : MonoBehaviour
{
    public GameObject Camera;                      // 얼굴의 코 부분에 연결할 게임 오브젝트

    ARSessionOrigin sessionOrigin;
    ARFaceManager arFaceManager;

    NativeArray<ARCoreFaceRegionData> faceRegions;

    GameObject noseObject;

    bool isNoseVisible = false;                  // 코의 가시성 여부를 추적

    // 코 이동을 위한 변수들
    //Vector3 targetNosePosition;
    Quaternion targetNoseRotation;
    float moveSpeed = 5.0f;                     // 이동 속도 조절
    float distanceThreshold1 = 0.0f;            // 얼굴과 카메라 간의 초기 거리
    float facetoface1 = 0.0f;            // 얼굴과 이동한 얼굴 간의 초기 거리
    public GameObject zero;
    private List<float> FTP1 = new List<float>();
    private List<float> FTP2 = new List<float>();
    private float FTP1Avg;
    private float FTP2Avg;

    private void Start()
    {
        arFaceManager = GetComponent<ARFaceManager>();
        sessionOrigin = GetComponent<ARSessionOrigin>();
        for (float i = 0.0f; i < 10.0f; i++)
        {
            FTP1.Add(0.0f);
            FTP2.Add(0.0f);
        }
    }
    private void FixedUpdate()
    {
        ARCoreFaceSubsystem subsystem = (ARCoreFaceSubsystem)arFaceManager.subsystem;
        foreach (ARFace face in arFaceManager.trackables)
        {
            subsystem.GetRegionPoses(face.trackableId, Unity.Collections.Allocator.Persistent, ref faceRegions);

            foreach (ARCoreFaceRegionData faceRegion in faceRegions)
            {
                ARCoreFaceRegion regionType = faceRegion.region;

                if (regionType == ARCoreFaceRegion.NoseTip)
                {
                    if (!noseObject)
                    {
                        noseObject = Instantiate(Camera, sessionOrigin.trackablesParent);
                        isNoseVisible = true;
                    }
                    Vector3 localNosePosition = noseObject.transform.localPosition;

                    // 얼굴과 초기 위치 간의 거리 계산
                    float facetoface2 = face.transform.position.x - zero.transform.position.x;
                    // 얼굴과 카메라 간의 거리 계산
                    float distanceThreshold2 = Vector3.Distance(face.transform.position, Camera.transform.position);


                    FTP1.Add(distanceThreshold1 - distanceThreshold2);
                    FTP2.Add(facetoface1 - facetoface2);
                    FTP1Avg = FTP1.Average();
                    FTP2Avg = FTP2.Average();

                    // 사용자가 앞으로 이동하면
                    if (FTP1Avg > 0.005f)
                    {
                        Vector3 targetNosePosition = new Vector3(localNosePosition.x, localNosePosition.y, localNosePosition.z + 7.0f);
                        noseObject.transform.localPosition = Vector3.Lerp(localNosePosition, targetNosePosition, 0.5f * moveSpeed * Time.fixedDeltaTime);
                        localNosePosition = targetNosePosition;
                    }
                    // 사용자가 뒤로 이동하면
                    else if (FTP1Avg < -0.005f)
                    {
                        Vector3 targetNosePosition = new Vector3(localNosePosition.x, localNosePosition.y, localNosePosition.z - 7.0f);
                        noseObject.transform.localPosition = Vector3.Lerp(localNosePosition, targetNosePosition, 0.5f * moveSpeed * Time.fixedDeltaTime);
                        localNosePosition = targetNosePosition;
                    }
                    // 사용자가 좌측으로 이동하면
                    else if (FTP2Avg < -0.005f)
                    {
                        Vector3 targetNosePosition = new Vector3(localNosePosition.x - 7.0f, localNosePosition.y, localNosePosition.z);
                        noseObject.transform.localPosition = Vector3.Lerp(localNosePosition, targetNosePosition, 0.5f * moveSpeed * Time.fixedDeltaTime);
                        localNosePosition = targetNosePosition;
                    }
                    // 사용자가 우측으로 이동하면
                    else if (FTP2Avg > 0.005f)
                    {
                        Vector3 targetNosePosition = new Vector3(localNosePosition.x + 7.0f, localNosePosition.y, localNosePosition.z);
                        noseObject.transform.localPosition = Vector3.Lerp(localNosePosition, targetNosePosition, moveSpeed * 0.5f * Time.fixedDeltaTime);
                        localNosePosition = targetNosePosition;
                    }
                    distanceThreshold1 = distanceThreshold2;
                    facetoface1 = facetoface2;

                    // 사용자 얼굴 회전
                    Vector3 sourceEuler = faceRegion.pose.rotation.eulerAngles;
                    Vector3 targetEuler = new Vector3(-sourceEuler.x, sourceEuler.y, sourceEuler.z = 0);
                    targetNoseRotation = Quaternion.Euler(targetEuler);

                    noseObject.transform.localRotation = Quaternion.Lerp(noseObject.transform.localRotation, targetNoseRotation, Time.fixedDeltaTime * moveSpeed);

                    FTP1.RemoveAt(0);
                    FTP2.RemoveAt(0);
                }
            }
        }

        // ARFace가 사라질 때 코를 비활성화
        if (isNoseVisible && arFaceManager.trackables.count == 0)
        {
            Destroy(noseObject);
            isNoseVisible = false;
        }

    }
}