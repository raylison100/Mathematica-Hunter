using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FimFase : MonoBehaviour
{

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("fim"))
        {
            SceneManager.LoadScene(0);
        }

        if (other.gameObject.CompareTag("mar"))
        {
            SceneManager.LoadScene(1);
        }
    }
}
