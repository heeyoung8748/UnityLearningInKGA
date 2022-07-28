using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyState
{
    None,       // 
    Idle,       // ���
    Walk,       // ���� patrol
    Run,        // ���� trace
    Attack,     // ����
    KnockBack   // �ǰ� damaged
}

public class EnemyAI : MonoBehaviour
{
    public EnemyState state;
    public EnemyState prevState = EnemyState.None;

    private Animator _animator;

    // �̵�����
    private Vector3 _targetPos;
    private float _moveSpeed = 1f;
    private float _rotationSpeed = 1f;

    // �� Ž�� ����
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
    // �� �����Ӹ��� �����ؾ� �ϴ� ���� (���°� �ٲ� �� ����)
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

        // ���������� �̵��ϴ� �ڵ�
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
        // ���������� �̵��ϴ� �ڵ�
        Vector3 dir = _targetPos - transform.position;
        //Debug.Log("Ÿ�ٰŸ� : " + dir.magnitude);
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
        // �ѹ��� �����ؾ� �ϴ� ���� (���°� �ٲ� �� ����)
        Debug.Log("��� ���� ����");
        _animator.SetBool("IsIdle", true);

        while (true)
        {
            yield return new WaitForSeconds(2f);
            // �ð����� �����ؾ� �ϴ� ���� (���°� �ٲ� �� ����)
            ChangeState(EnemyState.Walk);
            yield break;
        }
    }
    IEnumerator CoroutineWalk()
    {
        // �ѹ��� �����ؾ� �ϴ� ���� (���°� �ٲ� �� ����)
        Debug.Log("���� ���� ����");
        _animator.SetBool("IsWalk", true);

        // ������ ����
        _targetPos = transform.position + new Vector3(Random.Range(-7f, 7f), 0f, Random.Range(-7f, 7f));

        while (true)
        {
            yield return new WaitForSeconds(10f);
            // �ð����� �����ؾ� �ϴ� ���� (���°� �ٲ� �� ����)
            ChangeState(EnemyState.Idle);
        }
    }
    IEnumerator CoroutineRun()
    {
        // �ѹ��� �����ؾ� �ϴ� ���� (���°� �ٲ� �� ����)
        Debug.Log("���� ���� ����");
        _animator.SetBool("IsRun", true);
        _targetPos = target.transform.position;

        while (true)
        {
            yield return new WaitForSeconds(5f);
            // �ð����� �����ؾ� �ϴ� ���� (���°� �ٲ� �� ����)

        }
    }
    IEnumerator CoroutineAttack()
    {
        // �ѹ��� �����ؾ� �ϴ� ���� (���°� �ٲ� �� ����)
        Debug.Log("���� ���� ����");
        _animator.SetTrigger("IsAttack");

        yield return new WaitForSeconds(1f);
        ChangeState(EnemyState.Idle);
        yield break;
    }
    IEnumerator CoroutineKnockBack()
    {
        // �ѹ��� �����ؾ� �ϴ� ���� (���°� �ٲ� �� ����)

        while (true)
        {
            yield return new WaitForSeconds(2f);
            // �ð����� �����ؾ� �ϴ� ���� (���°� �ٲ� �� ����)

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

    // �̵�
    private Vector3 _targetPos;
    private float _moveSpeed = 2;
    private float _rotationSpeed = 3;

    // �� Ž��
    public GameObject Target;
    bool _isFindEnemy = false;
    Camera _eye;
    Plane[] _eyePlanes;

    private void Start()
    {
        animator = GetComponent<Animator>();
        _eye = transform.GetComponentInChildren<Camera>();
        // _eyePlanes = GeometryUtility.CalculateFrustumPlanes(_eye); //ī�޶� ��� plane���� ���� 
        PrevState = EnemyState.None;
        ChangeState(EnemyState.Idle);        
    }
    // ���°� ���ϴ� �������� ��� �����ϴ��Ŀ� ���� �з�
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
    // �� �����Ӹ��� �����ؾ� �ϴ� ����(������Ʈ)
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

        // ���������� �̵�
        Vector3 dir = _targetPos - transform.position; // ���⺤��
        if(dir.sqrMagnitude <= 0.1f)// ���Ͱ� ũ�� ��꺯��
        {
            ChangeState(EnemyState.Idle);
            return;
        }
        var targetRotation = Quaternion.LookRotation(dir, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);

        transform.position += transform.forward * _moveSpeed * Time.deltaTime;
        // ȸ�� �������δ� Slerp�� ���־���
    }
    void UpdateRun()
    {
        // ���������� �̵�
        Vector3 dir = _targetPos - transform.position; // ���⺤��
        if (dir.magnitude <= 2.0f)// ���Ͱ� ũ�� ��꺯��
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
        // �� ���� �����ؾ� �ϴ� ����(���� > while�� �ٱ�)
        Debug.Log("���ֽ��ϴ�");
        animator.SetBool("IsIdle", true);
        while(true)
        {

            yield return new WaitForSeconds(2f);
            // �ð����� �����ؾ� �ϴ� ����(���� > while�� �ȿ��� waitForSecond �ʸ���)
            ChangeState(EnemyState.Walk);
            yield break;
        }
    }
    IEnumerator CoroutineWalk()
    {
        // �� ���� �����ؾ� �ϴ� ����(���� > while�� �ٱ�)
        Debug.Log("�Ƚ��ϴ�");
        animator.SetBool("IsWalk", true);
        _targetPos = transform.position + new Vector3(Random.Range(-7f, 7f), 0f, Random.Range(-7f, 7f));
        while (true)
        {

            yield return new WaitForSeconds(2f);
            // �ð����� �����ؾ� �ϴ� ����(���� > while�� �ȿ��� waitForSecond �ʸ���)
            ChangeState(EnemyState.Idle);
            yield break;
        }
    }
    IEnumerator CoroutineRun()
    {
        // �� ���� �����ؾ� �ϴ� ����(���� > while�� �ٱ�)
        Debug.Log("�ʹٴ�");
        animator.SetBool("IsRun", true);
        _targetPos = Target.transform.position;
        while (true)
        {

            yield return new WaitForSeconds(5f);
            // �ð����� �����ؾ� �ϴ� ����(���� > while�� �ȿ��� waitForSecond �ʸ���)
            ChangeState(EnemyState.Attack);
            yield break;
        }
    }
    IEnumerator CoroutineAttack()
    {
        // �� ���� �����ؾ� �ϴ� ����(���� > while�� �ٱ�)
        Debug.Log("������");
        animator.SetTrigger("IsAttack");
        yield return new WaitForSeconds(1f);
        ChangeState(EnemyState.Idle);
        yield break;
    }
    IEnumerator CoroutineDamaged()
    {
        // �� ���� �����ؾ� �ϴ� ����(���� > while�� �ٱ�)
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
        // ī�޶� �ϳ� �� ���� ���̷��� �Ӹ��� ����, ��ó�� ����
        Bounds targetBounds = Target.GetComponentInChildren<SkinnedMeshRenderer>().bounds;
        _eyePlanes = GeometryUtility.CalculateFrustumPlanes(_eye); //ī�޶� ��� plane���� ���� 
        _isFindEnemy = GeometryUtility.TestPlanesAABB(_eyePlanes, targetBounds);
        return _isFindEnemy;
    }
}


 */