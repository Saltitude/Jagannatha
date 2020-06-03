using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SC_textTyping : MonoBehaviour
{
    [SerializeField]
    string [] Phrase;
    [SerializeField]
    bool Defilement = true;
    [SerializeField]
    float speed = 0.2f;
    TextMeshPro Txt;
    string TxtDepart;

    // Start is called before the first frame update
    void Start()
    {
        Txt = transform.GetChild(0).GetComponent<TextMeshPro>();
        TxtDepart = Phrase [0].Replace("|", System.Environment.NewLine);
        if (Defilement)
            StartCoroutine(AfficheTxt());
        else
            Txt.text = TxtDepart;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator AfficheTxt()
    {
        string temp = TxtDepart;
        int nbChar = TxtDepart.Length;

        for(int i = 1; i<= nbChar;i++)
        {
            yield return new WaitForSeconds(speed);
            Txt.text = temp.Substring(0, i);
        }
    }
}
