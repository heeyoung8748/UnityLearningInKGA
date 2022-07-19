using UnityEngine;

// 발판으로서 필요한 동작을 담은 스크립트
public class Platform : MonoBehaviour {
    private int _obstacleCount;
    private GameObject[] _obstacles; // 장애물 오브젝트들
    private bool _isStepped = false; // 플레이어 캐릭터가 밟았었는가

    void Awake()
    {
        _obstacleCount = transform.childCount;
        _obstacles = new GameObject[_obstacleCount];
        for(int i = 0; i < _obstacleCount; ++i)
        {
            _obstacles[i] = transform.GetChild(i).gameObject;
        }
    }
    // 컴포넌트가 활성화될때 마다 매번 실행되는 메서드
    private void OnEnable() 
    {
        // 발판을 리셋하는 처리
        // 오브젝트풀링 기법을 이용할 것. instanciate, destroy를 사용하는 것이 아니라 setActive를 가지고 객체의 생성과 삭제가 되는 것처럼 구현할 것
        _isStepped = false;
        
        // 장애물을 활성화하거나 비활성화하는 코드. 확률은 맘대로
        for(int i = 0; i < _obstacleCount; ++i)
        {
            if(0 == Random.Range(0, 4))
            {
                _obstacles[i].SetActive(true);
            }
            else
            {
                _obstacles[i].SetActive(false);
            }
        }
        
    }
    private static readonly float MIN_NORMAL_Y = Mathf.Sin(45f * Mathf.Deg2Rad); // 외적
    void OnCollisionEnter2D(Collision2D collision) 
    {
        if(collision.gameObject.tag == "Player")
        {
            if(_isStepped == false)
            {
                _isStepped = true;
                GameManager.Instance.AddcurrentScore();
            }
        }
    }
}