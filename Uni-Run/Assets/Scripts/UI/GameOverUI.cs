using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverUI : MonoBehaviour
{
    private GameObject[] _childs;
    private int _childCount;
                                            // 식 본문
    public void Activate() 
    {
        for(int i = 0; i < _childCount; ++i)
        {
            _childs[i].SetActive(true);
        }
    }

    void Awake() // 게임오버 ui 하위 오브젝트들
    {
        _childCount = transform.childCount;
        _childs = new GameObject[_childCount];
        for(int i = 0; i < _childCount; ++i)
        {
            _childs[i] = transform.GetChild(i).gameObject;
        }

    }
    void OnEnable()
    {
        GameManager.Instance.OnGameEnd.AddListener(Activate);
    }

    void OnDisable()
    {
        GameManager.Instance.OnGameEnd.RemoveListener(Activate);
    }
}
