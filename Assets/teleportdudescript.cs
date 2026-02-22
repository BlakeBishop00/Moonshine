using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class teleportdudescript : MonoBehaviour
{
    public GameButton gb;
    bool canDo;

    public GameObject obbbyafasdasdf1;
    public GameObject obbbyafasdasdf2;
    public GameObject obbbyafasdasdf3;
    public GameObject obbbyafasdasdf4;
    public GameObject obbbyafasdasdf5;
    public GameObject obbbyafasdasdf6;
    public GameObject obbbyafasdasdf7;
    public GameObject obbbyafasdasdf8;
    public GameObject obbbyafasdasdf9;
    public GameObject obbbyafasdasdf10;
    public GameObject obbbyafasdasdf11;
    // great coding
    public GameObject arrow;
    public GameObject teleportLoc;
    public float timeEach;
    
    void Update()
    {
        if (gb.triggered && !canDo)
        {
            Ssstart();
            canDo = true;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Ssstart()
    {
        StartCoroutine("StartTheStuff");
    }

    IEnumerator StartTheStuff()
    {
        obbbyafasdasdf1.SetActive(true);

        yield return timeEach;

        obbbyafasdasdf1.SetActive(false);
        obbbyafasdasdf2.SetActive(true);

        yield return timeEach;

        obbbyafasdasdf2.SetActive(false);
        obbbyafasdasdf3.SetActive(true);

        yield return timeEach;

        obbbyafasdasdf3.SetActive(false);
        obbbyafasdasdf4.SetActive(true);

        yield return timeEach;

        obbbyafasdasdf4.SetActive(false);
        obbbyafasdasdf5.SetActive(true);

        yield return timeEach;

        obbbyafasdasdf5.SetActive(false);
        obbbyafasdasdf6.SetActive(true);

        yield return timeEach;

        obbbyafasdasdf6.SetActive(false);
        obbbyafasdasdf7.SetActive(true);

        yield return timeEach;

        obbbyafasdasdf7.SetActive(false);
        obbbyafasdasdf8.SetActive(true);

        arrow.SetActive(true);

        yield return timeEach;

        obbbyafasdasdf8.SetActive(false);
        obbbyafasdasdf9.SetActive(true);

        yield return null;

        transform.position = teleportLoc.transform.position;
       

        yield return timeEach;

        obbbyafasdasdf9.SetActive(false);
        obbbyafasdasdf10.SetActive(true);

        yield return timeEach;

        obbbyafasdasdf10.SetActive(false);
        obbbyafasdasdf11.SetActive(true);

        yield return timeEach;

        obbbyafasdasdf11.SetActive(false);
        

        yield return timeEach;

        this.enabled = false;
    }
}
