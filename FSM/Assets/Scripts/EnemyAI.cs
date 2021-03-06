using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyState
{
    None,       // 
    Idle,       // 企奄
    Walk,       // 授茸 patrol
    Run,        // 蓄旋 trace
    Attack,     // 因維
    KnockBack   // 杷維 damaged
}

public class EnemyAI : MonoBehaviour
{
    public EnemyState state;
    public EnemyState prevState = EnemyState.None;

    private Animator _animator;

    // 戚疑淫恵
    private Vector3 _targetPos;
    private float _moveSpeed = 1f;
    private float _rotationSpeed = 1f;

    // 旋 貼走 淫恵
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
    // 古 覗傾績原陥 呪楳背醤 馬澗 疑拙 (雌殿亜 郊介 凶 原陥)
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

        // 鯉旋走猿走 戚疑馬澗 坪球
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
        // 鯉旋走猿走 戚疑馬澗 坪球
        Vector3 dir = _targetPos - transform.position;
        //Debug.Log("展為暗軒 : " + dir.magnitude);
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
        // 廃腰幻 呪楳背醤 馬澗 疑拙 (雌殿亜 郊介 凶 原陥)
        Debug.Log("企奄 雌殿 獣拙");
        _animator.SetBool("IsIdle", true);

        while (true)
        {
            yield return new WaitForSeconds(2f);
            // 獣娃原陥 呪楳背醤 馬澗 疑拙 (雌殿亜 郊介 凶 原陥)
            ChangeState(EnemyState.Walk);
            yield break;
        }
    }
    IEnumerator CoroutineWalk()
    {
        // 廃腰幻 呪楳背醤 馬澗 疑拙 (雌殿亜 郊介 凶 原陥)
        Debug.Log("授茸 雌殿 獣拙");
        _animator.SetBool("IsWalk", true);

        // 鯉旋走 竺舛
        _targetPos = transform.position + new Vector3(Random.Range(-7f, 7f), 0f, Random.Range(-7f, 7f));

        while (true)
        {
            yield return new WaitForSeconds(10f);
            // 獣娃原陥 呪楳背醤 馬澗 疑拙 (雌殿亜 郊介 凶 原陥)
            ChangeState(EnemyState.Idle);
        }
    }
    IEnumerator CoroutineRun()
    {
        // 廃腰幻 呪楳背醤 馬澗 疑拙 (雌殿亜 郊介 凶 原陥)
        Debug.Log("蓄旋 雌殿 獣拙");
        _animator.SetBool("IsRun", true);
        _targetPos = target.transform.position;

        while (true)
        {
            yield return new WaitForSeconds(5f);
            // 獣娃原陥 呪楳背醤 馬澗 疑拙 (雌殿亜 郊介 凶 原陥)

        }
    }
    IEnumerator CoroutineAttack()
    {
        // 廃腰幻 呪楳背醤 馬澗 疑拙 (雌殿亜 郊介 凶 原陥)
        Debug.Log("因維 雌殿 獣拙");
        _animator.SetTrigger("IsAttack");

        yield return new WaitForSeconds(1f);
        ChangeState(EnemyState.Idle);
        yield break;
    }
    IEnumerator CoroutineKnockBack()
    {
        // 廃腰幻 呪楳背醤 馬澗 疑拙 (雌殿亜 郊介 凶 原陥)

        while (true)
        {
            yield return new WaitForSeconds(2f);
            // 獣娃原陥 呪楳背醤 馬澗 疑拙 (雌殿亜 郊介 凶 原陥)

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

    // 戚疑
    private Vector3 _targetPos;
    private float _moveSpeed = 2;
    private float _rotationSpeed = 3;

    // 旋 貼走
    public GameObject Target;
    bool _isFindEnemy = false;
    Camera _eye;
    Plane[] _eyePlanes;

    private void Start()
    {
        animator = GetComponent<Animator>();
        _eye = transform.GetComponentInChildren<Camera>();
        // _eyePlanes = GeometryUtility.CalculateFrustumPlanes(_eye); //朝五虞 乞丞 plane級戚 級嬢身 
        PrevState = EnemyState.None;
        ChangeState(EnemyState.Idle);        
    }
    // 雌殿亜 痕馬澗 獣繊拭辞 嬢胸惟 呪楳馬汗劃拭 魚虞 歳嫌
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
    // 古 覗傾績原陥 呪楳背醤 馬澗 疑拙(穣汽戚闘)
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

        // 鯉旋走猿走 戚疑
        Vector3 dir = _targetPos - transform.position; // 号狽困斗
        if(dir.sqrMagnitude <= 0.1f)// 困斗葵 滴奄 域至痕呪
        {
            ChangeState(EnemyState.Idle);
            return;
        }
        var targetRotation = Quaternion.LookRotation(dir, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);

        transform.position += transform.forward * _moveSpeed * Time.deltaTime;
        // 噺穿 淫恵生稽澗 Slerp研 切爽彰陥
    }
    void UpdateRun()
    {
        // 鯉旋走猿走 戚疑
        Vector3 dir = _targetPos - transform.position; // 号狽困斗
        if (dir.magnitude <= 2.0f)// 困斗葵 滴奄 域至痕呪
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
        // 廃 腰幻 呪楳背醤 馬澗 疑拙(せぎ > while庚 郊甥)
        Debug.Log("辞赤柔艦陥");
        animator.SetBool("IsIdle", true);
        while(true)
        {

            yield return new WaitForSeconds(2f);
            // 獣娃原陥 呪楳背醤 馬澗 疑拙(せぎ > while庚 照拭辞 waitForSecond 段原陥)
            ChangeState(EnemyState.Walk);
            yield break;
        }
    }
    IEnumerator CoroutineWalk()
    {
        // 廃 腰幻 呪楳背醤 馬澗 疑拙(せぎ > while庚 郊甥)
        Debug.Log("鞍柔艦陥");
        animator.SetBool("IsWalk", true);
        _targetPos = transform.position + new Vector3(Random.Range(-7f, 7f), 0f, Random.Range(-7f, 7f));
        while (true)
        {

            yield return new WaitForSeconds(2f);
            // 獣娃原陥 呪楳背醤 馬澗 疑拙(せぎ > while庚 照拭辞 waitForSecond 段原陥)
            ChangeState(EnemyState.Idle);
            yield break;
        }
    }
    IEnumerator CoroutineRun()
    {
        // 廃 腰幻 呪楳背醤 馬澗 疑拙(せぎ > while庚 郊甥)
        Debug.Log("人陥陥");
        animator.SetBool("IsRun", true);
        _targetPos = Target.transform.position;
        while (true)
        {

            yield return new WaitForSeconds(5f);
            // 獣娃原陥 呪楳背醤 馬澗 疑拙(せぎ > while庚 照拭辞 waitForSecond 段原陥)
            ChangeState(EnemyState.Attack);
            yield break;
        }
    }
    IEnumerator CoroutineAttack()
    {
        // 廃 腰幻 呪楳背醤 馬澗 疑拙(せぎ > while庚 郊甥)
        Debug.Log("凶形推");
        animator.SetTrigger("IsAttack");
        yield return new WaitForSeconds(1f);
        ChangeState(EnemyState.Idle);
        yield break;
    }
    IEnumerator CoroutineDamaged()
    {
        // 廃 腰幻 呪楳背醤 馬澗 疑拙(せぎ > while庚 郊甥)
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
        // 朝五虞研 馬蟹 希 幻級嬢辞 什通傾宕 袴軒拭 細績, 勧坦軍 疑拙
        Bounds targetBounds = Target.GetComponentInChildren<SkinnedMeshRenderer>().bounds;
        _eyePlanes = GeometryUtility.CalculateFrustumPlanes(_eye); //朝五虞 乞丞 plane級戚 級嬢身 
        _isFindEnemy = GeometryUtility.TestPlanesAABB(_eyePlanes, targetBounds);
        return _isFindEnemy;
    }
}


 */