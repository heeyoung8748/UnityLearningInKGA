using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// 게임 오버 상태를 표현하고, 게임 점수와 UI를 관리하는 게임 매니저
// 씬에는 단 하나의 게임 매니저만 존재할 수 있다.
public class GameManager : SingletonBehavior<GameManager> 
{
    public int ScoreIncreaseAmount = 1;
    public GameObject GameOverUI;
    public UnityEvent OnGameEnd = new UnityEvent();
    public event UnityAction OnGameEnd2;
    // Unity에서는 UnityAction 타입을 주면 되고 이름 주면 끝

    public UnityEvent<int> OnScoreChanged = new UnityEvent<int>();
    public event UnityAction<int> OnScoreChanged2;
    // 객체를 만드는 부분이 없다. c#의 이벤트는 객체를 만들지 않아도 된다

    public int CurrentScore
    {
        get
        {
            return _currentScore;
        }
        set
        {
            _currentScore = value;
            OnScoreChanged.Invoke(_currentScore);
            
            OnScoreChanged2.Invoke(_currentScore); // 구독자 여부 묻기
        }
    }

    private int _currentScore = 0; 
    private bool _isEnd = false;

    void Update() 
    {
        // 게임 오버 상태에서 게임을 재시작할 수 있게 하는 처리
        if(_isEnd && Input.GetKeyDown(KeyCode.R))
        {
            reset();
            SceneManager.LoadScene(0);
        }
    }

    // 점수를 증가시키는 메서드
    public void AddcurrentScore() 
    {
        CurrentScore += ScoreIncreaseAmount;
        
    }

    // 플레이어 캐릭터가 사망시 게임 오버를 실행하는 메서드
    public void End() 
    {
        _isEnd = true;
        OnGameEnd.Invoke();
        OnGameEnd2?.Invoke();
    }

    private void reset()
    {
        _currentScore = 0;
        _isEnd = false;
    }
}