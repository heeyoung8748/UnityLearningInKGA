using System.Collections;
using UnityEngine;
using UnityEngine.AI; // AI, 내비게이션 시스템 관련 코드 가져오기

// 좀비 AI 구현
public class Zombie : LivingEntity
{
    public LayerMask TargetLayer; // 추적 대상 레이어
    public ParticleSystem HitEffect; // 피격 시 재생할 파티클 효과
    public AudioClip DeathSound; // 사망 시 재생할 소리
    public AudioClip HitSound; // 피격 시 재생할 소리
    public float Damage = 20f; // 공격력
    public float AttackCooltime = 0.5f; // 공격 간격

    private LivingEntity _target; // 추적 대상
    private NavMeshAgent _navMeshAgent; // 경로 계산 AI 에이전트
    private Animator _animator; // 애니메이터 컴포넌트
    private AudioSource _audioSource; // 오디오 소스 컴포넌트
    private Renderer _renderer; // 렌더러 컴포넌트

    private float _lastAttackTime; // 마지막 공격 시점
    private Collider[] _targetCandidates = new Collider[5]; // 타겟후보
    private int _targetCandidatesCount; // 반환값

    // 추적할 대상이 존재하는지 알려주는 프로퍼티
    private bool _hasTargetFound 
    {
        get
        {
            // 추적할 대상이 존재하고, 대상이 사망하지 않았다면 true
            if (_target != null && !_target.IsDead)
            {
                return true;
            }

            // 그렇지 않다면 false
            return false;
        }
    }

    private void Awake() 
    {
        // 초기화
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _audioSource = GetComponent<AudioSource>();
        _animator = GetComponent<Animator>();
        _renderer = GetComponentInChildren<Renderer>();
    }

    // 좀비 AI의 초기 스펙을 결정하는 셋업 메서드
    public void Setup(ZombieData zombieData) 
    {
        // 좀비프리팹을 만들어서 runtime때 함께 사용어쩌고
        InitialHealth = zombieData.health;
        Damage = zombieData.damage;
        _navMeshAgent.speed = zombieData.speed;
        _renderer.material.color = zombieData.skinColor;
    }

    private void Start() 
    {
        // 게임 오브젝트 활성화와 동시에 AI의 추적 루틴 시작
        StartCoroutine(UpdatePath());
    }

    private void Update() 
    {
        // 추적 대상의 존재 여부에 따라 다른 애니메이션 재생
        _animator.SetBool(ZombieAnimID.HasTarget, _hasTargetFound);
    }

    // 주기적으로 추적할 대상의 위치를 찾아 경로 갱신
    private IEnumerator UpdatePath() 
    {
        // 살아 있는 동안 무한 루프
        while (IsDead == false)
        {
            // 대상 찾
            if(_hasTargetFound)
            {
                _navMeshAgent.isStopped = false; // 목적지가 설정되어 있어도 이게 true면 멈춘다
                _navMeshAgent.SetDestination(_target.transform.position);
            }
            else //노찾
            {
                _navMeshAgent.isStopped = true;
                // 타겟 못찾? => 타겟 설정(몬스터의 일정 시야)
                // 레이캐스팅 때 봤던 Physics.Overlap~~메소드를 통해 런타임중에 동적으로 설정
                _targetCandidatesCount = Physics.OverlapSphereNonAlloc(transform.position, 7f, _targetCandidates, TargetLayer);
                // 순회하며 찾기
                for (int i = 0; i < _targetCandidatesCount; ++i)
                {
                    Collider targetCandidate = _targetCandidates[i];

                    LivingEntity livingEntity = targetCandidate.GetComponent<LivingEntity>();
                    Debug.Assert(livingEntity != null);
                    
                    if (livingEntity.IsDead == false)
                    {
                        _target = livingEntity;

                        break;
                    }
                }
            }
            
            yield return new WaitForSeconds(0.25f);
        }
    }

    // 데미지를 입었을 때 실행할 처리
    public override void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal) 
    {
        // LivingEntity의 OnDamage()를 실행하여 데미지 적용
        base.OnDamage(damage, hitPoint, hitNormal);

        if(IsDead == false)
        {
            _audioSource.PlayOneShot(HitSound);
            HitEffect.transform.position = hitPoint;
            HitEffect.transform.rotation = Quaternion.LookRotation(hitNormal);
            HitEffect.Play();
        }
    }

    // 사망 처리
    public override void Die() 
    {
        // LivingEntity의 Die()를 실행하여 기본 사망 처리 실행
        base.Die();
        // 사운드
        _audioSource.PlayOneShot(DeathSound);
        // 애니메이션 트리거 설정
        _animator.SetTrigger(ZombieAnimID.Die);
        // 네비메시 비활성화
        _navMeshAgent.isStopped = true;
        _navMeshAgent.enabled = false;
    }

    private void OnTriggerStay(Collider other) 
    {
        // 공격 가능여부 판단
        if(IsDead == false && Time.time >= _lastAttackTime + AttackCooltime)
        {
            // 트리거 충돌한 상대방 게임 오브젝트가 추적 대상이라면 공격 실행
            LivingEntity livingEntity = other.GetComponent<LivingEntity>();
            if(livingEntity == _target)
            {
                Vector3 hitPosition = other.ClosestPoint(transform.position);
                Vector3 hitNormal = transform.position - other.transform.position;
                livingEntity.OnDamage(Damage, hitPosition, hitNormal);

                _lastAttackTime = Time.time;
            }
        }
    }
}