using System.Collections;
using UnityEngine;

// 총을 구현
public class Gun : MonoBehaviour 
{
    // 총의 상태를 표현하는 데 사용할 타입을 선언
    public enum State // 총의 3가지 상태를 나타내는 열거형
    {
        Ready,
        Empty,
        Reloading
    }

    public State CurrentState { get; private set; } // 현재 총의 상태

    public Transform FireTransform;

    public ParticleSystem MuzzleFlashEffect;
    public ParticleSystem ShellEjectEffect; // 탄피 배출 효과

    private LineRenderer _bulletLineRenderer; // 탄알 궤적을 그리기 위한 렌더러

    public GunData Data;
    private AudioSource _audioSource; 


    private int _remainAmmoCount; // 남은 전체 탄알
    private int _ammoInMagazine;  // 현재 남아있는거
    private float _fireDistance = 50f;
    private float _lastFireTime; // 총을 마지막으로 발사한 시점

    private void Awake() 
    {
        // 사용할 컴포넌트의 참조 가져오기
        _bulletLineRenderer = GetComponent<LineRenderer>();
        _bulletLineRenderer.positionCount = 2; // 사용할 점 개수 보고
        _bulletLineRenderer.enabled = false; // 켜져있으면 안 되니 비활성화

        _audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable() 
    {
        // 총 상태 초기화
        // 1. 남은 탄피 초기화
        _remainAmmoCount = Data.InitialAmmoCount;
        _ammoInMagazine = Data.MagazineCapacity; // 현재 남은 총알
        CurrentState = State.Ready; // 총 상태 지정
        _lastFireTime = 0f;
    }

    // 발사 시도
    public void Fire() 
    {
        // 발사가 가능할 때
        // 1. State가 Ready일 때
        // 2. 쿨타임이 다 찼을 때
        if(CurrentState != State.Ready || Time.time < _lastFireTime + Data.FireCoolTime)
        {
            return;
        }
        _lastFireTime = Time.time;
        Shot();
    }

    // 실제 발사 처리
    private void Shot() 
    {
        Vector3 hitPosition;
        
        RaycastHit hit;
        if(Physics.Raycast(FireTransform.position, transform.forward, out hit, _fireDistance))
        {
            IDamageable target = hit.collider.GetComponent<IDamageable>();
            
        if(target != null)
        {
            target.OnDamage(Data.Damage, hit.point, hit.normal);
            // target?.OnDamage(Data.Damage, hit.point, hit.normal);
        }

            hitPosition = hit.point;
        }
        else
        {
            hitPosition = FireTransform.position + transform.forward * _fireDistance;
        }

        StartCoroutine(ShotEffect(hitPosition));
        --_ammoInMagazine;
        if(_ammoInMagazine <= 0)
        {
            CurrentState = State.Empty;
        }
    }

    // 발사 이펙트와 소리를 재생하고 탄알 궤적을 그림
    private IEnumerator ShotEffect(Vector3 hitPosition) 
    {
        // 발사 이펙트
        MuzzleFlashEffect.Play();
        ShellEjectEffect.Play();
        // 점 세팅
        _bulletLineRenderer.SetPosition(0, FireTransform.position);
        _bulletLineRenderer.SetPosition(1, hitPosition);
        // 라인 렌더러를 활성화하여 탄알 궤적을 그림
        _bulletLineRenderer.enabled = true;

    _audioSource.PlayOneShot(Data.ShotClip);
        // 0.03초 동안 잠시 처리를 대기
        yield return new WaitForSeconds(0.03f);

        // 라인 렌더러를 비활성화하여 탄알 궤적을 지움
        _bulletLineRenderer.enabled = false;
    }

    public bool TryReload()
    {
        if(CurrentState == State.Reloading || _remainAmmoCount <= 0 || _ammoInMagazine == Data.MagazineCapacity)
        {
            return false;
        }

        StartCoroutine(ReloadRoutine());
        return true;
    }

    // 실제 재장전 처리를 진행
    private IEnumerator ReloadRoutine()
    {
        // 현재 상태를 재장전 중 상태로 전환
        CurrentState = State.Reloading;

        // 재장전 소리 재생
        _audioSource.PlayOneShot(Data.ReloadClip);

        // 재장전 소요 시간 만큼 처리 쉬기
        yield return new WaitForSeconds(Data.ReloadTime);

        // 총알을 잘 채워줌
        int ammoToFill = Mathf.Min(Data.MagazineCapacity - _ammoInMagazine, _remainAmmoCount);
        _ammoInMagazine += ammoToFill;
        _remainAmmoCount -= ammoToFill;

        // 총의 현재 상태를 발사 준비된 상태로 변경
        CurrentState = State.Ready;
    }
}