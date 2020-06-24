using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SC_BulletFlock : NetworkBehaviour
{

    public bool b_IsFire = false;
    public bool b_ReactionFire = false;
    public FlockSettings flockSettings;
    Rigidbody rb = null;
    [SerializeField]
    float f_Scale_OP = 2f;
    Vector3 ScaleOP;

    public float damageFactor = 1;

    void Start()
    {
        GetRigidBody();
        ScaleOP = new Vector3(f_Scale_OP, f_Scale_OP, f_Scale_OP);
    }

    void Update()
    {
        if(isServer && b_IsFire)
            RpcDisplayFBulletOP(this.gameObject, this.transform.position, this.transform.rotation, ScaleOP);
    }

    private void OnTriggerEnter(Collider other)
    {
        //JE TOUCHE LE PLAYER 
        if(other.gameObject.layer == 20)
        {
            Sc_ScreenShake.Instance.ShakeIt(0.015f,0.1f);
            SC_CockpitShake.Instance.ShakeIt(0.0075f, 0.1f);
            //SC_HitDisplay.Instance.Hit(transform.position);
            //on fait subir des dmg au joueur
            if(flockSettings != null)
            {
                if(b_ReactionFire)
                {
                    SC_MainBreakDownManager.Instance.CauseDamageOnSystem(flockSettings.attackFocus, flockSettings.damageOnSystemHitReaction);
                }
                else
                {
                    int damage = Mathf.RoundToInt(flockSettings.damageOnSystem * damageFactor);
                    SC_MainBreakDownManager.Instance.CauseDamageOnSystem(flockSettings.attackFocus, damage);
                }
            }

            ResetPos();
        }
        if(other.gameObject.layer == 16)
        {
            ResetPos();
        }
     
    }

    void ResetPos()
    {

        if (rb == null)
            GetRigidBody();

        if (rb != null)
        {
            GetComponent<Rigidbody>().isKinematic = true;
            transform.position = new Vector3(1000, 1000, 1000);
            b_IsFire = false;
            if (isServer)
                RpcDisplayFBulletOP(this.gameObject, this.transform.position, this.transform.rotation, this.transform.localScale);
        }    

    }

    void GetRigidBody()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
            Debug.LogWarning("SC_BulletMiniGun - ResetPos - Can't Find RigidBody");
    }

    [ClientRpc]
    public void RpcDisplayFBulletOP(GameObject target, Vector3 position, Quaternion rotation, Vector3 scale)
    {
        if (!isServer)
        {
            target.transform.position = position;
            target.transform.rotation = rotation;
            target.transform.localScale = scale;
        }
    }

}
