using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    private Rigidbody2D rbPlayer;
    private Animator animPlayer;

    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    private float lives = 3;

    private bool isJumping;
    private bool isOnGround;
    private bool isPoweredUp = false;

    private void Awake()
    {
        rbPlayer = GetComponent<Rigidbody2D>();
        animPlayer = GetComponent<Animator>();
    }

    private void Start()
    {
        PhysicsMaterial2D noFriction = new PhysicsMaterial2D
        {
            friction = 0,
            bounciness = 0
        };
        GetComponent<Collider2D>().sharedMaterial = noFriction;
    }

    private void Update()
    {
        CheckGround();
        animPlayer.SetBool("Pulo", !isOnGround);

        if (Input.GetButtonDown("Jump") && isOnGround)
        {
            isJumping = true;
            Jump();
        }
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void CheckGround()
    {
        isOnGround = Physics2D.Linecast(transform.position, groundCheck.position, groundLayer);
        Debug.DrawLine(transform.position, groundCheck.position, Color.blue);
    }

    private void Move()
    {
        float xInput = Input.GetAxisRaw("Horizontal");
        float moveSpeed = isOnGround ? speed : speed * 0.8f;

        rbPlayer.velocity = new Vector2(xInput * moveSpeed, rbPlayer.velocity.y);
        animPlayer.SetFloat("Velocidade", Mathf.Abs(xInput));

        if (xInput > 0)
            transform.eulerAngles = new Vector2(0, 0);
        else if (xInput < 0)
            transform.eulerAngles = new Vector2(0, 180);
    }

    private void Jump()
    {
        if (rbPlayer.velocity.y > 0)
        {
            rbPlayer.velocity = new Vector2(rbPlayer.velocity.x, rbPlayer.velocity.y * 0.5f);
        }

        if (isJumping)
        {
            rbPlayer.velocity = new Vector2(rbPlayer.velocity.x, jumpForce);
            isJumping = false;
        }
    }
    private void PowerUp()
    {
        //animacao de power up
        isPoweredUp = true;
    }
    private void PowerDown()
    {
        //animacao de power down
        isPoweredUp = false;
    }
    public void Damage()
    {
        if (isPoweredUp == false)
        {
            //animacao de morte + tela de game over?
            rbPlayer.velocity = Vector2.zero;
            rbPlayer.isKinematic = true;
            StartCoroutine(ReloadSceneAfterDelay(2f));
            
        }
        if (isPoweredUp == true)
        {
            isPoweredUp = false;
            PowerDown();
        }
    }

    private IEnumerator ReloadSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("PowerUp"))
        {
            PowerUp();
            Destroy(collision.gameObject);
        }
    }
}
