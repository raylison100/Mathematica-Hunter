using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControler : MonoBehaviour
{

    public bool face = true;
    public Transform heroiT;
    public float vel = 2.5f;
    public float force = 5f;
    public Rigidbody2D heroiRB;
    public bool liberaPulo = false;
    public Animator anim;
    public bool vivo = true;

    // Start is called before the first frame update
    void Start()
    {
        heroiT = GetComponent<Transform>();
        heroiRB = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>(); 
    }

    // Update is called once per frame
    void Update()
    {         
        
        if(Input.GetKey(KeyCode.LeftArrow) && !face)
        {
            Flip();
        }
        else if (Input.GetKey(KeyCode.RightArrow) && face)
        {
            Flip();
        }
        if (vivo)
        {
            Movimentar();
        }
        
    }

    void Movimentar()
    {
        if(Input.GetKey(KeyCode.RightArrow))
        {
            transform.Translate(new Vector2(vel * Time.deltaTime, 0));
            anim.SetBool("idle", false);
            anim.SetBool("correr", true);
        }

        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Translate(new Vector2(-vel * Time.deltaTime, 0));
            anim.SetBool("idle", false);
            anim.SetBool("correr", true);
        }
        else
        {
            anim.SetBool("idle", true);
            anim.SetBool("correr", false);
        }

        if (Input.GetKey(KeyCode.Space) && liberaPulo)
        {
            heroiRB.AddForce(new Vector2(0, force), ForceMode2D.Impulse);
            anim.SetBool("idle", false);
            anim.SetBool("pular", true);
        }
        else
        {
            anim.SetBool("idle", true);
            anim.SetBool("pular", false);
        }

    }

    void Flip()
    {
        face = !face;
        Vector3 scala = heroiT.localScale;
        scala.x *= -1;
        heroiT.localScale = scala;

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Chao"))
        {
            liberaPulo = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Chao"))
        {
            liberaPulo = false;
        }
    }
}
