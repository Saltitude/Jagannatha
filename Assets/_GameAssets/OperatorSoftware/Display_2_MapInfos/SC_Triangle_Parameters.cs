using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SC_Triangle_Parameters : MonoBehaviour
{
    GameObject Mng_SyncVar = null;
    SC_SyncVar_calibr sc_syncvar;

    [SerializeField]
    Material triangleMat;


    Mesh triangleMesh;
    MeshRenderer msh_Rend;
    Vector3[] vertex;

    public float amplitudeValue;

    public float frequenceValue;

    public float phaseValue;

    [SerializeField]
    float precision = 0.2f;
    [SerializeField]
    float speed = 5;

    float delayLetter = 0.01f;

    int[] triangles;

    public Vector3[] vertexPos;

    public bool b_Init = false;

    [SerializeField]
    Text[] sensiTxt = new Text[3];

    [SerializeField]
    Font ArialFont;
    [SerializeField]
    Font sanskritFont;

    Coroutine[] coroutine = new Coroutine[3];
    // Start is called before the first frame update
    void Start()
    {
        Mng_SyncVar = GameObject.FindGameObjectWithTag("Mng_SyncVar");
        GetReferences();

        gameObject.AddComponent<MeshFilter>();
        msh_Rend =  gameObject.AddComponent<MeshRenderer>();

        msh_Rend.material = triangleMat;

        triangleMesh = new Mesh();
        GetComponent<MeshFilter>().mesh = triangleMesh;

        vertex = new[]
        {
            new Vector3(0,0,0),
            new Vector3(0,1,0),
            new Vector3(1,0,0),
        };
        vertexPos = new Vector3[3];
        triangleMesh.vertices = vertex;

        triangles = new[] { 0, 1, 2 };

        triangleMesh.triangles = triangles;
        for (int i = 0; i < vertexPos.Length; i++)
        {
            vertexPos[i] = vertex[i];
        }
       
    }

    void GetReferences()
    {
        if (Mng_SyncVar == null)
            Mng_SyncVar = GameObject.FindGameObjectWithTag("Mng_SyncVar");
        if (Mng_SyncVar != null && sc_syncvar == null)
            sc_syncvar = Mng_SyncVar.GetComponent<SC_SyncVar_calibr>();
    }

    // Update is called once per frame
    void Update()
    {
            if (sc_syncvar == null || Mng_SyncVar == null)
        {
            GetReferences();

        }
        if (sc_syncvar != null)
        {
            if(b_Init)
            {
                initTriangle();
            }
            else
            {
                animTriangle();
            }
        }
    }

    void initTriangle()
    {
        
        for (int i = 0; i < vertexPos.Length; i++)
        {
            vertex[i] = Vector3.zero;
            vertexPos[i] = Vector3.zero;
            sensiTxt[i].text = "0";
        }
        triangleMesh.vertices = vertex;
        
    }

    void animTriangle()
    {
            vertexPos[1] = new Vector3(-frequenceValue * 0.86f, -frequenceValue / 2, 0);
            vertexPos[0] = new Vector3(0, amplitudeValue, 0);
            vertexPos[2] = new Vector3(phaseValue * 0.86f, -phaseValue / 2, 0);


            for (int i = 0; i < vertexPos.Length; i++)
            {
                if (Vector3.Distance(vertex[0], vertexPos[0]) > precision)
                {
                    if (vertex[0].magnitude > vertexPos[0].magnitude)
                    {
                        vertex[0] -= new Vector3(0, (Time.deltaTime * speed), 0);
                    }
                    else
                    {
                        vertex[0] += new Vector3(0, (Time.deltaTime * speed), 0);
                    }
                }
                if (Vector3.Distance(vertex[1], vertexPos[1]) > precision)
                    {
                        if (vertex[1].magnitude > vertexPos[1].magnitude)
                        {
                            vertex[1] += new Vector3(Time.deltaTime * speed * 0.86f, (Time.deltaTime * speed) / 2, 0);
                        }
                        else
                        {
                            vertex[1] -= new Vector3(Time.deltaTime * speed * 0.86f, (Time.deltaTime * speed) / 2, 0);
                        }
                    }
                    
                if (Vector3.Distance(vertex[2], vertexPos[2]) > precision)
                {
                    if (vertex[2].magnitude > vertexPos[2].magnitude)
                    {
                        vertex[2] += new Vector3(-Time.deltaTime * speed * 0.86f, (Time.deltaTime * speed) / 2, 0);
                    }
                    else
                    {
                        vertex[2] += new Vector3(Time.deltaTime * speed * 0.86f, -(Time.deltaTime * speed) / 2, 0);
                    }
                }
                sensiTxt[i].text = Mathf.Round (vertex[i].magnitude).ToString();
                animText();
                }   

        triangleMesh.vertices = vertex;
        triangleMesh.triangles = triangles;
        
    }

    void animText()
    {
        if (sensiTxt[0].text != (SC_UI_Display_MapInfos_KoaState.Instance.koaSensibility.y + 1).ToString())
        {
            //sensiTxt[1].font = sanskritFont;
            coroutine[0] = StartCoroutine(multipleLetter(0));

        }
        else
        {
            StopCoroutine(coroutine[0]);
            sensiTxt[0].font = ArialFont;
        }

        if (sensiTxt[1].text != (SC_UI_Display_MapInfos_KoaState.Instance.koaSensibility.x + 1).ToString())
        {
            //sensiTxt[0].font = sanskritFont;
            coroutine[1] =  StartCoroutine(multipleLetter(1));
        }
        else
        {
            StopCoroutine(coroutine[1]);
            sensiTxt[1].font = ArialFont;
        }
        
        if (sensiTxt[2].text != (SC_UI_Display_MapInfos_KoaState.Instance.koaSensibility.z + 1).ToString())
        {
            //sensiTxt[2].font = sanskritFont;
            coroutine[2] = StartCoroutine(multipleLetter(2));

        }
        else
        {
            StopCoroutine(coroutine[2]);
            sensiTxt[2].font = ArialFont;
        }
    }

    IEnumerator multipleLetter(int index)
    {
        sensiTxt[index].font = sanskritFont;
        yield return new WaitForSeconds(delayLetter);
        sensiTxt[index].text = "j";
        yield return new WaitForSeconds(delayLetter);
        sensiTxt[index].text = "u";
        yield return new WaitForSeconds(delayLetter);
        sensiTxt[index].text = "g";
        yield return new WaitForSeconds(delayLetter);
        sensiTxt[index].text = "e";
        yield return new WaitForSeconds(delayLetter);
        sensiTxt[index].text = "r";

        if(index == 0)
        {
            sensiTxt[0].text = (SC_UI_Display_MapInfos_KoaState.Instance.koaSensibility.y + 1).ToString();
        }
        if (index == 1)
        {
            sensiTxt[1].text = (SC_UI_Display_MapInfos_KoaState.Instance.koaSensibility.x + 1).ToString();
        }
        if (index == 2)
        {
            sensiTxt[2].text = (SC_UI_Display_MapInfos_KoaState.Instance.koaSensibility.z + 1).ToString();
        }
        StopCoroutine("multipleLetter");
    }
}   
