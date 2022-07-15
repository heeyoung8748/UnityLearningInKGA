using UnityEngine;


public class PlayerController : MonoBehaviour 
{
   public AudioClip deathClip;
   public float jumpForce = 700f;

   private int jumpCount = 0; 
   private bool isGrounded = false;
   private bool isDead = false; 

   private Rigidbody2D _rigidbody; 
   private Animator _animator;
   private AudioSource _audioSource;

   private void Start() 
   {
       _rigidbody = GetComponent<Rigidbody2D>();
       _animator = GetComponent<Animator>();
       _audioSource = GetComponent<AudioSource>();
   }

   private void Update() 
   {
       
   }

   private void Die() 
   {
       
   }

   private void OnTriggerEnter2D(Collider2D other) {
       // 트리거 콜라이더를 가진 장애물과의 충돌을 감지
   }

   private void OnCollisionEnter2D(Collision2D collision) {
       // 바닥에 닿았음을 감지하는 처리
   }

   private void OnCollisionExit2D(Collision2D collision) {
       // 바닥에서 벗어났음을 감지하는 처리
   }
}