using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyState
{
    None,       // 
    Idle,       // 대기
    Walk,       // 순찰 patrol
    Run,        // 추적 trace
    Attack,     // 공격
    KnockBack   // 피격 damaged
}

public class EnemyAI : MonoBehaviour
{
    public EnemyState state;
    public EnemyState prevState = EnemyState.None;

    private Animator _animator;

    // 이동관련
    private Vector3 _targetPos;
    private float _moveSpeed = 1f;
    private float _rotationSpeed = 1f;

    // 적 탐지 관련
    public GameObject target;
    private bool _isFindEnemy = false;
    private Camera _eye;
    private Plane[] _eyePlanes;

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        _eye = transform.GetComponentInChildren<Camera>();

        ChangeState(EnemyState.Idle);
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case EnemyState.Idle: UpdateIdle(); break;
            case EnemyState.Walk: UpdateWalk(); break;
            case EnemyState.Run: UpdateRun(); break;
            case EnemyState.Attack: UpdateAttack(); break;
            case EnemyState.KnockBack: UpdateKnockBack(); break;
        }
    }

    #region UpdateDetail
    // 매 프레임마다 수행해야 하는 동작 (상태가 바뀔 때 마다)
    void UpdateIdle()
    {

    }
    void UpdateWalk()
    {
        if (IsFindEnemy())
        {
            ChangeState(EnemyState.Run);
            return;
        }

        // 목적지까지 이동하는 코드
        Vector3 dir = _targetPos - transform.position;
        if (dir.sqrMagnitude <= 0.2f)
        {
            ChangeState(EnemyState.Idle);
            return;
        }

        var targetRotation = Quaternion.LookRotation(_targetPos - transform.position, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);

        transform.position += transform.forward * _moveSpeed * Time.deltaTime;
    }
    void UpdateRun()
    {
        // 목적지까지 이동하는 코드
        Vector3 dir = _targetPos - transform.position;
        //Debug.Log("타겟거리 : " + dir.magnitude);
        if (dir.magnitude <= 2.0f)
        {
            ChangeState(EnemyState.Attack);
            return;
        }

        var targetRotation = Quaternion.LookRotation(_targetPos - transform.position, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotationSpeed * 2f * Time.deltaTime);

        transform.position += transform.forward * _moveSpeed * 2f * Time.deltaTime;
    }
    void UpdateAttack()
    {

    }
    void UpdateKnockBack()
    {

    }
    #endregion


    #region CoroutineDetail
    IEnumerator CoroutineIdle()
    {
        // 한번만 수행해야 하는 동작 (상태가 바뀔 때 마다)
        Debug.Log("대기 상태 시작");
        _animator.SetBool("IsIdle", true);

        while (true)
        {
            yield return new WaitForSeconds(2f);
            // 시간마다 수행해야 하는 동작 (상태가 바뀔 때 마다)
            ChangeState(EnemyState.Walk);
            yield break;
        }
    }
    IEnumerator CoroutineWalk()
    {
        // 한번만 수행해야 하는 동작 (상태가 바뀔 때 마다)
        Debug.Log("순찰 상태 시작");
        _animator.SetBool("IsWalk", true);

        // 목적지 설정
        _targetPos = transform.position + new Vector3(Random.Range(-7f, 7f), 0f, Random.Range(-7f, 7f));

        while (true)
        {
            yield return new WaitForSeconds(10f);
            // 시간마다 수행해야 하는 동작 (상태가 바뀔 때 마다)
            ChangeState(EnemyState.Idle);
        }
    }
    IEnumerator CoroutineRun()
    {
        // 한번만 수행해야 하는 동작 (상태가 바뀔 때 마다)
        Debug.Log("추적 상태 시작");
        _animator.SetBool("IsRun", true);
        _targetPos = target.transform.position;

        while (true)
        {
            yield return new WaitForSeconds(5f);
            // 시간마다 수행해야 하는 동작 (상태가 바뀔 때 마다)

        }
    }
    IEnumerator CoroutineAttack()
    {
        // 한번만 수행해야 하는 동작 (상태가 바뀔 때 마다)
        Debug.Log("공격 상태 시작");
        _animator.SetTrigger("IsAttack");

        yield return new WaitForSeconds(1f);
        ChangeState(EnemyState.Idle);
        yield break;
    }
    IEnumerator CoroutineKnockBack()
    {
        // 한번만 수행해야 하는 동작 (상태가 바뀔 때 마다)

        while (true)
        {
            yield return new WaitForSeconds(2f);
            // 시간마다 수행해야 하는 동작 (상태가 바뀔 때 마다)

        }
    }
    #endregion

    void ChangeState(EnemyState nextState)
    {
        if (prevState == EnemyState.Run && prevState == nextState) return;

        StopAllCoroutines();

        prevState = state;
        state = nextState;
        _animator.SetBool("IsIdle", false);
        _animator.SetBool("IsWalk", false);
        _animator.SetBool("IsRun", false);
        _animator.SetBool("IsAttack", false);
        _animator.SetBool("IsKnockBack", false);

        switch (state)
        {
            case EnemyState.Idle: StartCoroutine(CoroutineIdle()); break;
            case EnemyState.Walk: StartCoroutine(CoroutineWalk()); break;
            case EnemyState.Run: StartCoroutine(CoroutineRun()); break;
            case EnemyState.Attack: StartCoroutine(CoroutineAttack()); break;
            case EnemyState.KnockBack: StartCoroutine(CoroutineKnockBack()); break;
        }
    }

    bool IsFindEnemy()
    {
        if (!target.activeSelf) return false;

        Bounds targetBounds = target.GetComponentInChildren<SkinnedMeshRenderer>().bounds;
        _eyePlanes = GeometryUtility.CalculateFrustumPlanes(_eye);
        _isFindEnemy = GeometryUtility.TestPlanesAABB(_eyePlanes, targetBounds);

        return _isFindEnemy;
    }
}


/*
 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyState
{
    Idle,
    Walk,
    Run,
    Attack,
    Damaged
}

public class EnemyAI : MonoBehaviour
{
    public EnemyState State;
    public EnemyState PrevState;
    Animator animator;

    // 이동
    private Vector3 _targetPos;
    private float _moveSpeed = 2;
    private float _rotationSpeed = 3;

    // 적 탐지
    public GameObject Target;
    bool _isFindEnemy = false;
    Camera _eye;
    Plane[] _eyePlanes;

    private void Start()
    {
        animator = GetComponent<Animator>();
        _eye = transform.GetComponentInChildren<Camera>();
        // _eyePlanes = GeometryUtility.CalculateFrustumPlanes(_eye); //카메라 모양 plane들이 들어옴 
        PrevState = EnemyState.None;
        ChangeState(EnemyState.Idle);        
    }
    // 상태가 변하는 시점에서 어떻게 수행하느냐에 따라 분류
    private void Update()
    {
        switch(State)
        {
            case EnemyState.Idle: UpdateIdle(); break;
            case EnemyState.Walk: UpdateWalk(); break;
            case EnemyState.Attack: UpdateAttack(); break;
            case EnemyState.Run: UpdateRun(); break;
            case EnemyState.Damaged: UpdateDamaged(); break;
        }
    }
    // 매 프레임마다 수행해야 하는 동작(업데이트)
    void UpdateIdle()
    {

    }
    void UpdateWalk()
    {
        if(IsFindEnemy())
        {
            ChangeState(EnemyState.Run);
            return;
        }

        // 목적지까지 이동
        Vector3 dir = _targetPos - transform.position; // 방향벡터
        if(dir.sqrMagnitude <= 0.1f)// 벡터값 크기 계산변수
        {
            ChangeState(EnemyState.Idle);
            return;
        }
        var targetRotation = Quaternion.LookRotation(dir, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);

        transform.position += transform.forward * _moveSpeed * Time.deltaTime;
        // 회전 관련으로는 Slerp를 자주쓴다
    }
    void UpdateRun()
    {
        // 목적지까지 이동
        Vector3 dir = _targetPos - transform.position; // 방향벡터
        if (dir.magnitude <= 2.0f)// 벡터값 크기 계산변수
        {
            ChangeState(EnemyState.Attack);
            return;
        }
        var targetRotation = Quaternion.LookRotation(dir, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
        transform.position += transform.forward * _moveSpeed * 2 * Time.deltaTime;
    }
    void UpdateAttack()
    {

    }
    void UpdateDamaged()
    {

    }

    IEnumerator CoroutineIdle()
    {
        // 한 번만 수행해야 하는 동작(ㅋㄾ > while문 바깥)
        Debug.Log("서있습니다");
        animator.SetBool("IsIdle", true);
        while(true)
        {

            yield return new WaitForSeconds(2f);
            // 시간마다 수행해야 하는 동작(ㅋㄾ > while문 안에서 waitForSecond 초마다)
            ChangeState(EnemyState.Walk);
            yield break;
        }
    }
    IEnumerator CoroutineWalk()
    {
        // 한 번만 수행해야 하는 동작(ㅋㄾ > while문 바깥)
        Debug.Log("걷습니다");
        animator.SetBool("IsWalk", true);
        _targetPos = transform.position + new Vector3(Random.Range(-7f, 7f), 0f, Random.Range(-7f, 7f));
        while (true)
        {

            yield return new WaitForSeconds(2f);
            // 시간마다 수행해야 하는 동작(ㅋㄾ > while문 안에서 waitForSecond 초마다)
            ChangeState(EnemyState.Idle);
            yield break;
        }
    }
    IEnumerator CoroutineRun()
    {
        // 한 번만 수행해야 하는 동작(ㅋㄾ > while문 바깥)
        Debug.Log("와다다");
        animator.SetBool("IsRun", true);
        _targetPos = Target.transform.position;
        while (true)
        {

            yield return new WaitForSeconds(5f);
            // 시간마다 수행해야 하는 동작(ㅋㄾ > while문 안에서 waitForSecond 초마다)
            ChangeState(EnemyState.Attack);
            yield break;
        }
    }
    IEnumerator CoroutineAttack()
    {
        // 한 번만 수행해야 하는 동작(ㅋㄾ > while문 바깥)
        Debug.Log("때려요");
        animator.SetTrigger("IsAttack");
        yield return new WaitForSeconds(1f);
        ChangeState(EnemyState.Idle);
        yield break;
    }
    IEnumerator CoroutineDamaged()
    {
        // 한 번만 수행해야 하는 동작(ㅋㄾ > while문 바깥)
        while (true)
        {

            yield return new WaitForSeconds(2f);
            ChangeState(EnemyState.Idle);
        }
    }

    void ChangeState(EnemyState nextState)
    {
        StopAllCoroutines();

        State = nextState;
         animator.SetBool("IsIdle", false);
        animator.SetBool("IsWalk", false);
        animator.SetBool("IsRun", false);
        animator.SetBool("IsKnockBack", false);
        animator.SetTrigger("O");

        //animator.SetBool((int)State, true);
        switch (State)
        {
            case EnemyState.Idle: StartCoroutine(CoroutineIdle()); break;
            case EnemyState.Walk: StartCoroutine(CoroutineWalk()); break;
            case EnemyState.Run: StartCoroutine(CoroutineRun()); break;
            case EnemyState.Attack: StartCoroutine(CoroutineAttack()); break;
            case EnemyState.Damaged: StartCoroutine(CoroutineDamaged()); break;
        }
    }

    bool IsFindEnemy()
    {
        if(!Target.activeSelf)
        {
            return false;
        }
        // 카메라를 하나 더 만들어서 스켈레톤 머리에 붙임, 눈처럼 동작
        Bounds targetBounds = Target.GetComponentInChildren<SkinnedMeshRenderer>().bounds;
        _eyePlanes = GeometryUtility.CalculateFrustumPlanes(_eye); //카메라 모양 plane들이 들어옴 
        _isFindEnemy = GeometryUtility.TestPlanesAABB(_eyePlanes, targetBounds);
        return _isFindEnemy;
    }
}


 */