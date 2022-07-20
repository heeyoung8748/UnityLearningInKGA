using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 우리가 만드는 것: component singleton
// 보통 타입에 대해 제약을 걸 수 있는 문법을 제공함
// template<typename T> int float 이런 거 말고 component가 들어가야 함
public class SingletonBehavior<T> : MonoBehaviour where T : MonoBehaviour
{ 
    private static T _Instance;

    public static T Instance 
    {
        get 
        {
            if(_Instance == null)
            {
                // 인스턴스가 초기화되기 전이라면 해당 오브젝트를 찾아 할당한다
                _Instance = FindObjectOfType<T>();
                DontDestroyOnLoad(_Instance.gameObject);
            }

            return _Instance; 
        } 
    }

    protected void Awake()
    {
        if(_Instance != null)
        {
            if(_Instance != this)
            {   
                Destroy(gameObject);
            }

            return;
        }
        _Instance = GetComponent<T>();
        // Scene을 전환해도 파괴되어선 안됨
        // 이를 위해 사용하는 함수: DontDestroyOnLoad()
        DontDestroyOnLoad(gameObject);

    }
}
