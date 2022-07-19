using UnityEngine;

// 발판을 생성하고 주기적으로 재배치하는 스크립트
public class PlatformSpawner : MonoBehaviour {
    public GameObject PlatformPrefab; // 생성할 발판의 원본 프리팹
    public int MaxPlatformCount = 10; // 생성할 발판의 개수

    public float MinSpawnCooltime = 1.25f; // 다음 배치까지의 시간 간격 최솟값
    public float MaxSpawnCooltime = 2.25f; // 다음 배치까지의 시간 간격 최댓값
    private float _nextSpawnCooltime; // 다음 배치까지의 시간 간격
    private float _cooltime; // 마지막 배치 시점

    public float MinY = -3.5f; // 배치할 위치의 최소 y값
    public float MaxY = 1.5f; // 배치할 위치의 최대 y값
    private float xPos = 20f; // 배치할 위치의 x 값

    private GameObject[] _platforms; // 미리 생성한 발판들s
    private int _nextSpawnPlatformIndex = 0; // 사용할 현재 순번의 발판

    private readonly Vector2 _poolPosition = new Vector2(0, -20); // 초반에 생성된 발판들을 화면 밖에 숨겨둘 위치


    void Start() 
    {
        // 변수들을 초기화하고 사용할 발판들을 미리 생성
        // MaxPlatformCount 만큼의 크기를 가지는 배열 생성
        _platforms = new GameObject[MaxPlatformCount];

        // 각 원소 초기화
        for(int i = 0; i < MaxPlatformCount; ++i)
        {
            _platforms[i] = Instantiate(PlatformPrefab, _poolPosition, Quaternion.identity); // 런타임 때 프리팹을 인스턴스화
            _platforms[i].SetActive(false);
        }
    }

    void Update() 
    {
        // 순서를 돌아가며 주기적으로 발판을 배치
        // 1. 배치 쿨타임이 다 찼는가?
        // 1-1. currentIndex에 델타타임을 더해준다.
        _cooltime += Time.deltaTime;
        if(_cooltime >= _nextSpawnCooltime)
        {
            _cooltime = 0f;
            _nextSpawnCooltime = Random.Range(MinSpawnCooltime, MaxSpawnCooltime);
        // 플랫폼을 소환한다
        Vector2 spawnPosition = new Vector2(xPos, Random.Range(MinY, MaxY));
        GameObject currentPlatform = _platforms[_nextSpawnPlatformIndex];
        currentPlatform.transform.position = spawnPosition;
        currentPlatform.SetActive(true);
        _nextSpawnPlatformIndex = (_nextSpawnPlatformIndex + 1) % MaxPlatformCount;
        }

        // 2. 랜덤하게 배치한다.
    }
}