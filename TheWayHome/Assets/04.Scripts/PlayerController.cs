using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Components")]
    Rigidbody2D rigid;
    CapsuleCollider2D capsule;
    SpriteRenderer sprite;
    Animator animator;

    [Space(10f)]
    [Header("Status")]
    public float maxSpeed;
    public float jumpPower;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        capsule = GetComponent<CapsuleCollider2D>();
        sprite = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Jump
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            animator.SetBool("isJump", true);
        }
            
        // Sprite Flip X
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            sprite.flipX = true;
        else if (Input.GetKeyDown(KeyCode.RightArrow))
            sprite.flipX = false;

        // Idle -> Run
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow))
            animator.SetBool("isRun", true);
        // Run -> Idle
        if (Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.RightArrow))
            animator.SetBool("isRun", false);
    }

    void FixedUpdate()
    {
        // Move By Key Control
        float h = Input.GetAxisRaw("Horizontal");

        rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);

        if (rigid.velocity.x > maxSpeed) //Right Max Speed
            rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y);
        else if (rigid.velocity.x < maxSpeed * (-1)) //Left Max Speed
            rigid.velocity = new Vector2(maxSpeed * (-1), rigid.velocity.y);

        
        // Ãß¶ôÁßÀÏ ¶§´Â
        if (rigid.velocity.y < 0)
        {
            // Fall
            animator.SetBool("isJump", false);
            animator.SetBool("isFall", true);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // ¶¥¿¡ ´êÀ¸¸é Âø·ú ¾Ö´Ï¸ÞÀÌ¼Ç 1¹ø ½ÇÇà
        if(collision.gameObject.tag == "Ground")
        {
            animator.SetBool("isFall", false);
            animator.SetTrigger("isLand");
        }
    }
}
