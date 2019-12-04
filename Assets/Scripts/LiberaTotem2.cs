using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LiberaTotem2 : MonoBehaviour
{
    public bool liberaPainel;
    public float time;
    public GameObject painel;
    public Text contador;
    public Text score;
    private float valor;

    private bool acertou;

    // Start is called before the first frame update
    void Start()
    {
        liberaPainel = false;
        time = 15;
    }

    // Update is called once per frame
    void Update()
    {

        painel.SetActive(liberaPainel);

        if (liberaPainel && time > 0)
        {
            time -= Time.deltaTime;
            contador.text = time.ToString();
        }
        else
        {
            liberaPainel = false;
        }

        if (acertou)
        {
            valor = float.Parse(score.text) + 10;
            score.text = valor.ToString();
            acertou = false;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("totem2"))
        {
            liberaPainel = true;
            Destroy(other.gameObject);
        }
    }

    public void errada()
    {
        liberaPainel = false;
        acertou = false;
    }

    public void certa()
    {
        liberaPainel = false;
        acertou = true;
    }
}
