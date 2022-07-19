using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreText : MonoBehaviour
{
    private TextMeshProUGUI _ui;
    void Awake()
    {
        _ui = GetComponent<TextMeshProUGUI>();
    }
    void OnEnable()
    {
        // GameManager.Instance.OnScoreChanged.AddListener(UpdateText);
        // GameManager.Instance.OnScoreChanged.Invoke(10); // 유니티이벤트는 객체라서 멋대로 호출이 가능함
        // 그러나 C# event는 불가능함
        // GameManager.Instance.OnScoreChanged2.Invoke(10); <= 컴파일 에러!

        // 이벤트가 일어났을 때 Update Text를 실행하도록 만든 것

        // unity event는 유니티 내에서 자체적으로 구현한 것
        // event unity action은 C#에서 구현한 것
        GameManager.Instance.OnScoreChanged2 += UpdateText; // 구독
    }
    public void UpdateText(int score)
    {
        // _ui의 텍스트를 수정
        _ui.text = $"Score: {score}";
    }

    void OnDisable()
    {
        // GameManager.Instance.OnScoreChanged.RemoveListener(UpdateText);
        // 비활성화가 되면 이 이벤트에 대해 통지를 받을 필요가 없으므로 Ondisable때 구독 해지
        GameManager.Instance.OnScoreChanged2 -= UpdateText; // 구취
    }
}
