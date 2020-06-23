using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_KoaSettingsOP : MonoBehaviour, IF_KoaForOperator, IF_Hover
{
    Vector3 sensibility;
    float timer;
    float timeBeforeSpawn;
    bool spawn = true;
    Vector3 initialScale;
    float initialRadius;
    [SerializeField]
    int factor;
    string koaID;
    int type;
    float maxKoaLife;
    float curKoaLife;

    [SerializeField]
    Material[] Tab_mat;

    [SerializeField]
    Mesh[] meshByType;

    [SerializeField]
    Color32[] Tab_color;
    [SerializeField]
    Color32[] Tab_colorSpawn;

    [SerializeField]
    Color32 colorCTA;


    public bool bSelected;

    //Animation
    bool deploy;
    bool flight;
    bool bullet;
    bool laser;
    float speedFactor;
    bool chargeLaser;


    bool spawnScale;

    public enum koaSelection
    {
        None,
        Selected,
        Hover
    }

    int curBoidSettingsIndex;

    [SerializeField]
    GameObject VFX_koadeath;
    [SerializeField]
    GameObject VFX_clicOnMe;
    [SerializeField]
    float timeBeforeCallToAction = 2;
    float timerCTA = 0;
    bool boolCTA = true;
    bool PSInstantiate = false;

    float totalscale;
    GameObject PS_CTA;

    SC_KoaID_operator_display textDisplay;
    public enum koaState
    {
        Spawn = 0,
        Roam = 1,
        AttackPlayer = 2,
        Death = 3,
        Reaction = 4
    }

    koaState currentState;


    void Start()
    {
        textDisplay = GetComponentInChildren<SC_KoaID_operator_display>();

        totalscale = initialScale.x;
    }


    public void SetSensibility(Vector3 sensibility)
    {
        this.sensibility = sensibility;
    }
    public void SetTimeBeforeSpawn(int spawnTimer)
    {
        this.timeBeforeSpawn = spawnTimer;
        initialScale = transform.localScale;
        initialRadius = GetComponent<SphereCollider>().radius;
        spawn = false;
        timer = 0;
    }


    public void SetKoaType(int type, bool spawnScale)
    {
        this.type = type;
        this.spawnScale = spawnScale;
        SetMesh();
        setMeshColor();
    }

    public void SetKoaID(string koaID)
    {
        this.koaID = koaID;
    }

    public void SetKoaLife(float curLife)
    {
        //this.curKoaLife = Mathf.Round(curLife);
        this.curKoaLife = curLife;

        if (curLife <= 0)
        {

            //https://www.youtube.com/watch?v=VUjn2Vs65Z8
            var vfx = Instantiate(VFX_koadeath);
            vfx.transform.position = transform.position;
            vfx.GetComponent<ParticleSystem>().startColor = Tab_color[type];
            vfx.GetComponent<ParticleSystemRenderer>().trailMaterial.color = Tab_color[type];
            vfx.GetComponent<ParticleSystem>().Play();
            textDisplay.SetTextActive(false);

        }
    }
    public void SetKoaState(int curState)
    {
        this.currentState = (koaState)curState;
    }

    public void SetKoamaxLife(float maxLife)
    {
        this.maxKoaLife = maxLife;
    }

    public void SetBoidSettings(int boidSettingsIndex)
    {
        curBoidSettingsIndex = boidSettingsIndex;
    }

    public string GetKoaID()
    {
        return koaID;
    }
    public float GetCurKoaLife()
    {
        return curKoaLife;
    }

    public float GetMaxKoaLife()
    {
        return maxKoaLife;
    }

    public Vector3 GetSensibility()
    {
        return sensibility;
    }


    public int GetBoidSettingsIndex()
    {
        return curBoidSettingsIndex;
    }

    public int GetKoaState()
    {
        return (int) currentState;
    }

    public void SetBoolAnimation(bool deploy, bool flight, bool bullet, bool laser, float speedFactor, bool chargeLaser)
    {
        this.deploy = deploy;
        this.flight = flight;
        this.bullet = bullet;
        this.laser = laser;
        this.speedFactor = speedFactor;
        this.chargeLaser = chargeLaser;
    }
    public bool GetDeploy()
    {
        return deploy;
    }
    public bool GetFlight()
    {
        return flight;
    }
    public bool GetBullet()
    {
        return bullet;
    }
    public bool GetLaser()
    {
        return laser;
    }
    public float GetSpeedFactor()
    {
        return speedFactor;
    } 
    public bool GetChargeLaser()
    {
        return chargeLaser;
    }

    public int GetKoaType()
    {
        return type;
    }

    void Update()
    {
        if(!spawn)
        {

            if(spawnScale)
            {
                float scale = ((initialScale.x * factor / timeBeforeSpawn) * Time.deltaTime);
                float radius = ((initialRadius / factor / timeBeforeSpawn) * Time.deltaTime);
                transform.localScale += new Vector3(scale, scale, scale);
                transform.GetComponent<SphereCollider>().radius -= radius;
                if(type == 4 && transform.localScale.x >= initialScale.x * factor)
                transform.localScale = new Vector3(initialScale.x * factor, initialScale.x * factor, initialScale.x * factor);

            }
            else
            {
                transform.localScale = new Vector3(initialScale.x * factor, initialScale.x * factor, initialScale.x * factor);
            }
  
            timer += Time.deltaTime;
            if (timer >= timeBeforeSpawn)
            {
                setMeshColor();
                spawn = true;
            }           
        }
        if (type == 0)
        {
            timerCTA += Time.deltaTime;
            if (timerCTA > timeBeforeCallToAction && boolCTA)
            {
                if (!PSInstantiate)
                {
                    PSInstantiate = true;
                    PS_CTA = Instantiate(VFX_clicOnMe,this.transform);
                    PS_CTA.GetComponent<ParticleSystem>().startColor = colorCTA;
                    PS_CTA.GetComponent<ParticleSystem>().Play();
                }
               

            }
        }
    }

    public void SetMaterial(koaSelection newSelection)
    {

        switch (newSelection)
        {
            case koaSelection.None:

                if (!bSelected)
                {
                    textDisplay.SetTextActive(false);
                    GetComponent<MeshRenderer>().material = Tab_mat[(int)newSelection];
                }

                break;
            case koaSelection.Selected:
                
                textDisplay.SetTextActive();

                GetComponent<MeshRenderer>().material = Tab_mat[(int)newSelection];
                boolCTA = false;
                if (PSInstantiate && PS_CTA != null)
                {
                    Destroy(PS_CTA);
                }

                break;
            case koaSelection.Hover:

                if (!bSelected)
                {
                    textDisplay.SetTextActive(false);
                    GetComponent<MeshRenderer>().material = Tab_mat[(int)newSelection];

                }

                break;
            default:
                break;
        }


        setMeshColor();
    }



    void setMeshColor()
    {
        Color32 newColor = Color.white;

        if (spawn)
            newColor = Tab_color[type];
        else
            newColor = Tab_colorSpawn[type];

        GetComponent<MeshRenderer>().material.color = newColor;
    }

    public void HoverAction()
    {
        SetMaterial(koaSelection.Hover);
        StartCoroutine(EndCoroutine());
    }

    public void OutAction()
    {
        SetMaterial(koaSelection.None);

    }
    IEnumerator EndCoroutine()
    {
        yield return new WaitForEndOfFrame();
        OutAction();
    }

    void SetMesh()
    {

        if(meshByType[type] != null) 
        GetComponent<MeshFilter>().mesh = meshByType[type];
    }
}
