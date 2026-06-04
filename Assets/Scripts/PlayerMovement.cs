using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    //Movement
    public float movespeed;
    Rigidbody2D rb;
    [HideInInspector]
    public float lastHonrizontalVector;
    [HideInInspector]
    public float lastVerticalVector;
    [HideInInspector]
    public Vector2 moveDir;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        InputManagement();
    }

    void FixedUpdate()
    {
        Move();
    }

    void InputManagement()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        moveDir = new Vector2(moveX, moveY).normalized;

        if(moveDir.x != 0)
        {
            lastHonrizontalVector = moveDir.x;
        }
        
        if(moveDir.y != 0)
        {
            lastVerticalVector = moveDir.y;
        }
    }

    void Move()
    {
        rb.linearVelocity = new Vector2(moveDir.x * movespeed, moveDir.y * movespeed);
    }
}
