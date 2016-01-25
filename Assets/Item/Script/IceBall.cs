using UnityEngine;
using System.Collections;

public class IceBall : MonoBehaviour {

	// Use this for initialization
    GameObject owner;
    float speed=3.0f;
    float damage;
    Vector3 nowVec;
   // Vector3 ownerLast;
    [SerializeField]
    private ParticleSystem explode;
    Vector3 center;
    Vector3 nextVec;
    float randomWait;
    float timeCount;
    float freezeTime=2.5f;

    void Start()
    {
        center = transform.position;
        //if (owner != null)
       
        //randomWait = ((float)Random.Range(100, 200)) / 100.0f;
        //nowVec.Normalize();
        //timeCount =0;
        //freezeTime = 2.5f;
        //speed = 2.0f;
    }

    // Update is called once per frame
    void FixedUpdate()
    {   
        timeCount += Time.fixedDeltaTime;
        if (timeCount >= randomWait)
        {
            nextVec = new Vector3(((float)Random.Range(-100,100))/100.0f,0,((float)Random.Range(-100,100))/100.0f);
            nextVec.Normalize();
            randomWait = ((float)Random.Range(100, 200)) / 100.0f;
            timeCount = 0;
            //nextVec = transform.position + 3.0f * temp * Time.fixedDeltaTime;
        }

        //nowVec = Vector3.Lerp(nowVec,nextVec,randomWait/0.5f);

        float x = Mathf.Lerp(nowVec.x, nextVec.x,Time.fixedDeltaTime*2.0f);
        float z = Mathf.Lerp(nowVec.z, nextVec.z,Time.fixedDeltaTime*2.0f);

        nowVec = new Vector3(x, 0, z);
        nowVec.Normalize();

        //transform.position += 3.0f * nowVec * Time.fixedDeltaTime;
       /* GetComponent<Rigidbody>().AddForce(nowVec * 10.0f);
        nowVec = nowVec *0.95f;
        if (GetComponent<Rigidbody>().velocity.magnitude > 8.0f)
        {
            Vector3 velocityVec = GetComponent<Rigidbody>().velocity.normalized;
            velocityVec = velocityVec * 2.0f;
            GetComponent<Rigidbody>().velocity = velocityVec;
        }
        */
        transform.position += speed * nowVec * Time.fixedDeltaTime;
        transform.Rotate(new Vector3(0,1,0), Time.fixedDeltaTime * 360.0f,Space.World);
       // Debug.Log("aaaa "+speed);
    }

    void OnCollisionEnter(Collision c)
    {
        Debug.Log("ccccc");
        if (c.transform.tag == "Player" || c.transform.tag == "Monster")
        {

            if (!c.collider.isTrigger)
            {
                if ((c.transform.tag == "Player" || c.transform.tag == "Monster") && c.gameObject!=owner)
                {
                    c.gameObject.GetComponent<Damagable>().Damage(nowVec, damage);
                    Animator a;
                    if (a = c.gameObject.GetComponent<Animator>())
                    {
                        
                        a.GetComponent<BoxController>().Freeze(freezeTime);
                        a.GetComponent<Damagable>().Damage(nowVec * 5.0f, 0);
                        Debug.Log("goFreeze");
                    }
                
                }

            }


        }
        Vector3 nor;
        ContactPoint[] cps = c.contacts;
        int t = 0;
       // do
       // {
       //    nor = cps[t++].normal;
       // } while (Mathf.Abs(nor.y)>=0.1f&&t<cps.Length);
       // if (Mathf.Abs(nor.y) >= 0.1)
        //    return;
        nor = cps[0].normal;
        Vector3 reflected = Vector3.Reflect(nowVec, nor);
      //  Debug.Log("A " + nowVec);
        nowVec = reflected;
        nextVec = nowVec;
      //  Debug.Log("B " + nowVec);
        //Debug.Log("C " + nor);
        timeCount = 0;
        randomWait = ((float)Random.Range(100, 400)) / 100.0f;

    }

    void OnTriggerEnter(Collider c)
    {
        // if(owner==null)return;

        if ( (c.gameObject.tag == "Player" || c.gameObject.tag == "Monster") && c.gameObject != owner)
        {
            Debug.Log("YOOOOOOOOOOOOO");
            Animator a;
            if (a=c.gameObject.GetComponent<Animator>())
            {
                //a.Stop();
                a.GetComponent<BoxController>().Freeze(freezeTime);
                a.GetComponent<Damagable>().Damage(nowVec * 10.0f,0);
                Debug.Log("goFreeze");
            }
        }
    }

    public void SetSpeed(float s)
    {
        speed = s;
        Debug.Log("SetSpeed");
    }

    public void SetDamage(float d)
    {
        damage = d;
    }
    

    public void SetOwner(GameObject g)
    {
        Debug.Log("OwO");
        owner = g;
        nowVec = owner.transform.forward;
        nowVec.Normalize();
        nextVec = new Vector3(((float)Random.Range(-100, 100)) / 100.0f, 0, ((float)Random.Range(-100, 100)) / 100.0f);
        nextVec.Normalize();
      //  ownerLast = owner.transform.position;
    }

    public void SetFreezeTime(float ft)
    {
        freezeTime = ft;
    }

    
}
