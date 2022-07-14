using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DotProductTest : MonoBehaviour
{
    public Transform Target;
    void Start()
    {
        
    }

    void Update()
    {
        //transform.forward // 이 오브젝트의 방향 벡터를 얻을 수 있음
        // 두 오브젝트 사이의 거리 구함
        Vector3 distanceVector = Target.position - transform.position;// a - b < a쪽 방향, b - a < b쪽방향
        
        Debug.Log(Vector3.Dot(transform.position, distanceVector.normalized)); // Dot의 인수로 자신의 위치와 정규화 된 벡터(방향 벡터) 받음
        
        // 내적
        Debug.DrawRay(transform.position, transform.forward * 5f, Color.blue); // 시작지점, 방향, 색, 
        Debug.DrawRay(transform.position, distanceVector, Color.red);
        
        // 외적
        Vector3 normalVector = Vector3.Cross(transform.forward, distanceVector.normalized); // 외적
        Debug.DrawRay(transform.position, normalVector * 5f, Color.green);

    }
}
