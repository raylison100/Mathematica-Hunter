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
    public float nochaoRaio = 0.1f;
    public LayerMask oqueEChao;
    [Range(1, 20)]
    private float jumpForce = 30f;

    public Animator animH;

    [SerializeField]
    private bool clickPulo = false;

    [SerializeField]
    private bool clickSlide = false;
    private float timeSlide = 0f;





    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animH = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(nochao);

        if (nochao && this.clickPulo)
        {
            this.clickPulo = false;
            rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
            animH.SetBool("chao", nochao);

        }

        animH.SetFloat("x", Mathf.Abs(move));

        if (nochao && this.clickSlide)
        {
            animH.SetBool("slide", true);
            this.timeSlide = 4f;
        }

        this.timeSlide -= Time.deltaTime;

        if(this.timeSlide <= 0)
        {
            this.clickSlide = false;
            animH.SetBool("slide", false);
        }

    }


    private void FixedUpdate()
    {
        nochao = Physics2D.OverlapCircle(nochaoCheck.position, nochaoRaio, oqueEChao);

        if (rb.velocity.y > 0 && !this.clickPulo)
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

    public void Pular()
    {
        this.clickPulo = true;
    }

    public void Deslisar()
    {
        this.clickSlide = true;
    }

    public void Esquerda()
    {
        this.move = -1;
    }

    public void Parado()
    {
        this.move = 0;
    }
}
