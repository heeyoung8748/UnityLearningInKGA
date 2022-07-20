using UnityEngine;

// 플레이어 캐릭터를 사용자 입력에 따라 움직이는 스크립트
public class PlayerMovement : MonoBehaviour 
{
    [SerializeField]
    private float MoveSpeed = 5f; // 앞뒤 움직임의 속도
    public float RotateSpeed = 180f; // 좌우 회전 속도


    private PlayerInput _input; // 플레이어 입력을 알려주는 컴포넌트
    private Rigidbody _rigidbody; // 플레이어 캐릭터의 리지드바디
    private Animator _animator; // 플레이어 캐릭터의 애니메이터

    private static class AnimID
    {
        public static readonly int MOVE = Animator.StringToHash("Move");
    }

    private void Awake() 
    {
        // 사용할 컴포넌트들의 참조를 가져오기
        _input = GetComponent<PlayerInput>();
        _rigidbody = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
    }

    // FixedUpdate는 물리 갱신 주기에 맞춰 실행됨
    private void FixedUpdate()
    {
        // 물리 갱신 주기마다 움직임, 회전, 애니메이션 처리 실행

        // rigidbody 사용시에는 여기서 업데이트 하는 게 맞다. 지금까지 편의상 update 씀
        // 50fps로 고정(설정에서 바꿔줄 수 있음) Time.fixedDeltaTime: fixedupdate시 deltatime대신 사용
        // 만일 컴퓨터 성능 문제로 50fps 미만의 fps가 나오면 일관된 결과가 나올 수 있도록 업데이트를 여러 번 함
        move();
        rotate();

        // 움직임 애니메이션 적용
        _animator.SetFloat(AnimID.MOVE, _input.MoveDirection);

    }

    // 입력값에 따라 캐릭터를 앞뒤로 움직임
    private void move() 
    {
        // 이동량
        float movementAmount = MoveSpeed * Time.fixedDeltaTime;
        Vector3 direction = _input.MoveDirection * transform.forward;
        Vector3 deltaPosition = direction * movementAmount;
       _rigidbody.MovePosition(_rigidbody.position + deltaPosition);
    }

    // 입력값에 따라 캐릭터를 좌우로 회전
    private void rotate() 
    {
        // 쿼터니언을 회전시킬 때는 곱셈을 이용함. ex: 현재로부터 60도 -> transform.rotation * Quaternion.Euler(어쩌고)
        float rotationAmount = _input.RotateDirection * RotateSpeed * Time.fixedDeltaTime;
        Quaternion deltaRotation = Quaternion.Euler(0f, rotationAmount ,0f);
        _rigidbody.MoveRotation(_rigidbody.rotation * deltaRotation);
    }
}