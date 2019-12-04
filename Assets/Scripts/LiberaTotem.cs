using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LiberaTotem : MonoBehaviour
{
   
    public bool liberaPainel;
    public float time;
    public GameObject painel;
    public Text contador;


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

        if (liberaPainel && time > 0 )
        {
            time -= Time.deltaTime;
            contador.text = time.ToString();
        }
        else
        {
            liberaPainel = false;
        }
        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("totem"))
        {
            liberaPainel = true;
            Destroy(other.gameObject);
        }
    }

}
