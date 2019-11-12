using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControler : MonoBehaviour
{

    public float velocidade;

    public Transform    Player;
    public Animator     Animator;
    public Rigidbody2D  PlayerRigidbody;

    public int          forceJump;
    public bool         slide;

    //Chao
    public bool         grounded;
    public LayerMask    whatIsGround;
    public Transform    GroundCheck;

    //slide
    public  float       slideTemp;
    private float       timeTemp;

    //colisor
    public Transform   colisor;
    

    // Start is called before the first frame update
    void Start()
    {
        Animator = Player.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Movimentar();
    }

    void Movimentar()
    {

        Animator.SetFloat("Run", Mathf.Abs(Input.GetAxis("Horizontal")));

        if (Input.GetAxisRaw("Horizontal") > 0)
        {
            transform.Translate(Vector2.right * velocidade * Time.deltaTime);
            transform.eulerAngles = new Vector2(0, 0);
        }

        if (Input.GetAxisRaw("Horizontal") < 0)
        {
            transform.Translate(Vector2.right * velocidade * Time.deltaTime);
            transform.eulerAngles = new Vector2(0, 180);
        }

        if (Input.GetButtonDown("Jump") && grounded)
        {
            PlayerRigidbody.AddForce(new Vector2(velocidade, forceJump));
            if (slide)
            {
                colisor.position = new Vector3(colisor.position.x, colisor.position.y + 1.73f, colisor.position.z);
                slide = false;
            }
        }

        if (Input.GetButtonDown("Slide") && grounded)
        {
            if (!slide)
            {
                colisor.position = new Vector3(colisor.position.x, colisor.position.y - 1.73f, colisor.position.z);
            }
           
            slide = true;
            timeTemp = 0;
        }

        grounded = Physics2D.OverlapCircle(GroundCheck.position, 0.2f, whatIsGround);

        if (slide)
        {
            timeTemp += Time.deltaTime;
            if(timeTemp >= slideTemp)
            {
                colisor.position = new Vector3(colisor.position.x, colisor.position.y + 1.73f, colisor.position.z);
                slide = false;
            }
        }


        Animator.SetBool("Jump", !grounded);
        Animator.SetBool("Slide", slide);
    }
}
