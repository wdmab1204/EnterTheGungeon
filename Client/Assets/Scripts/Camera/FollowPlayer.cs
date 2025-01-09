using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Transform player;   // 플레이어의 Transform
    public Vector3 offset;     // 플레이어와 카메라 사이의 거리
    public float smoothSpeed = 0.125f; // 카메라 이동의 부드러움 정도

    void FixedUpdate()
    {
        // 카메라의 목표 위치
        Vector3 desiredPosition = player.position + offset;

        // 부드럽게 카메라가 목표 위치로 이동
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // 카메라 위치 업데이트
        transform.position = smoothedPosition;
    }
}
