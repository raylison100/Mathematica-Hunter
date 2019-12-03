using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControler : MonoBehaviour
{
    [SerializeField]
    private float pulo = 6.0f;
    private Rigidbody2D rb;
    public bool face = true;
    [Range(1, 20)]
    public float maxSpeed = 5f;
    public float move;

    public bool nochao;
    public Transform nochaoCheck;
    private Collider2D playerCollider;
    private float nochaoRaio = 1f;
    public LayerMask oqueEChao;
    [Range(1, 20)]
    private float jumpForce = 30f;

    public Animator animH;

    private float timeSlide = 0f;
    private bool deslizando = false;





    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animH = GetComponent<Animator>();
        playerCollider = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        //anadar e correr
        animH.SetFloat("x", Mathf.Abs(move));      
        // pular
        animH.SetBool("chao", nochao); 
        //deslizar
        animH.SetBool("slider", deslizando);

        this.timeSlide -= Time.deltaTime;

        if(this.timeSlide <= 0)
        {
            deslizando = false;
            playerCollider.offset = new Vector2(playerCollider.offset.x,-0.055f);
        }

    }


    private void FixedUpdate()
    {
        nochao = Physics2D.OverlapCircle(nochaoCheck.position, nochaoRaio, oqueEChao);

        if (rb.velocity.y > 0)
        {
            rb.gravityScale = pulo;
        }else
        {
            rb.gravityScale = 1;
        }

        //
        if (nochao)
        {
            rb.velocity = new Vector2(move * maxSpeed, rb.velocity.y);
        }

        if(move > 0 && !face){
            Flip();
        }else if (move < 0 && face)
        {
            Flip();
        }

    }

    void Flip()
    {
        face = !face;
        Vector3 tempScale = transform.localScale;
        tempScale.x *= -1;
        transform.localScale = tempScale;
    }
    
    public void Direita()
    {
        this.move = 1;
    }

    public void Esquerda()
    {
        this.move = -1;
    }

    public void Parado()
    {
        this.move = 0;
    }

    public void Pular()
    {
        if (nochao)
        {
            rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);            
        }
    }

    public void Deslisar()
    {
        if (nochao && !deslizando)
        {

            //-0.05518246
            playerCollider.offset = new Vector2(playerCollider.offset.x,0.66f);
            deslizando = true;
            this.timeSlide = 1f;
        }
    }
}
