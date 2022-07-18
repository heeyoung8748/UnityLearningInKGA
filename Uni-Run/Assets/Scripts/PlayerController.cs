using UnityEngine;


public class PlayerController : MonoBehaviour 
{
   public AudioClip deathClip;
   public float jumpForce = 700f;
   public int maxJumpCount = 2;

   private int _jumpCount = 0; 
   private bool _isOnGround = false;
   private bool _isDead = false; 

   private Rigidbody2D _rigidbody; 
   private Animator _animator;
   private AudioSource _audioSource;
   private Vector2 _zero;

private static class AnimationID // static클래스는 인스턴스화할 수 없다, 즉 정적 멤버만 넣을 수 있다
{
    // 여기서 private라고 쓰면 AnimationID라는 클래스 내에서만 사용이 가능하게 된다. 그러므로 public으로 바꿔준다.
   public static readonly int IS_ON_GROUND = Animator.StringToHash("IsOnGround");
   public static readonly int DIE = Animator.StringToHash("die");
}
   
   private static readonly float MIN_NORMAL_Y = Mathf.Sin(45f * Mathf.Deg2Rad); // 외적


   private void Awake() 
   {
       _rigidbody = GetComponent<Rigidbody2D>();
       _animator = GetComponent<Animator>();
       _audioSource = GetComponent<AudioSource>();

       _zero = Vector2.zero;
   }

   private void Update() 
   {
       // 사용자 입력을 감지하고 점프하는 처리
       if(_isDead)
       {
        return;
       }

       if(Input.GetMouseButtonDown(0))
       {
            //최대 점프에 도달했으면 아무 것도 안 함
            if(_jumpCount >= maxJumpCount)
            {
                return;
            }

            // 점프 횟수 증가
            _jumpCount++;
            _rigidbody.velocity = _zero;
            _rigidbody.AddForce(new Vector2(0f, jumpForce));
            _audioSource.Play(); // 이미 갖고 있는 클립으로 재생
       }
        if (Input.GetMouseButtonUp(0))
        {
            if(_rigidbody.velocity.y > 0)
            {
                _rigidbody.velocity *= 0.5f;
            }
        }

        _animator.SetBool(AnimationID.IS_ON_GROUND, _isOnGround);
   }

   private void Die() 
   {
       // 사망처리
       _isDead = true;
       // 애니메이션 업데이트
       _animator.SetTrigger(AnimationID.DIE);
       // 플레이어 캐릭터 멈추기
       _rigidbody.velocity = _zero;
       // 죽을 때 소리 재생
       // 책 내에서는 클립을 아예 바꿔서 재생하는데, AudioSource.PlayOneShot(오디오 클립, 볼륨)을 사용하면 클립을 교체하지 않고 외부에서 클립을 받아와 한 번 사용할 수 있다
       _audioSource.PlayOneShot(deathClip);

       GameManager.Instance.End();
   }

   private void OnTriggerEnter2D(Collider2D other) 
   {
       // 트리거 콜라이더를 가진 장애물과의 충돌을 감지
       if (other.tag == "Dead")
       {
            if(_isDead == false)
            {
                Die();
            }
       }
   }

   private void OnCollisionEnter2D(Collision2D collision) 
   {
       // 바닥에 닿았음을 감지하는 처리
       ContactPoint2D point = collision.GetContact(0);
       if (point.normal.y >= MIN_NORMAL_Y)
       {
            // 플랫폼 위로 안착
            // 1. 점프 가능
            _isOnGround = true;
            _jumpCount = 0;

            GameManager.Instance.AddScore();
       }
   }

   private void OnCollisionExit2D(Collision2D collision) 
   {
       // 바닥에서 벗어났음을 감지하는 처리
       _isOnGround = false;
   }
}