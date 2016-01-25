using UnityEngine;
using System.Collections;

public class Damagable : MonoBehaviour {

	public float STR, VIT, AGI;

	// Use this for initialization
	public float MaxHP;
	public float BasicATK;
	public float BasicSpeed;
	public float Speed;
    public float HP;
	public float ATK;
	[SerializeField] private Item mItem;

    public GameObject checkSphere;
    float rotateSpeed = 5.0f;
    bool onDamage;
    Vector3 damageDir,endDir,startDir;
    Quaternion damageQ;
    float count;
	private BoxController mBox;
    [SerializeField] private ParticleSystem deathEffect;

    void Start () {
        onDamage = false;
        count = 0;
		mBox = gameObject.GetComponent<BoxController> ();
		init ();
    }

	void init()
	{
		MaxHP = VIT * 10.0f + 50.0f;
		BasicATK = STR * 2.0f + 10.0f;
		BasicSpeed = 1.0f + AGI / (AGI + 10.0f);
		HP = MaxHP;
		ATK = BasicATK;
		Speed = BasicSpeed;
	}

	// Update is called once per frame
	void Update () {
       

	}

    void FixedUpdate()
    {
      /*   if (onDamage)
        {   
            Quaternion q;
            if(count<=0.4)
              q = Quaternion.LookRotation(endDir);
            else
              q = Quaternion.LookRotation(startDir);

            Debug.Log("qqq");
            count+=Time.fixedDeltaTime;
            transform.rotation = Quaternion.Slerp(transform.rotation, q ,Time.fixedDeltaTime*rotateSpeed);
            
            if(count>=0.8f){
                count =0;
                onDamage = false;
                damageDir = Vector3.zero;
                transform.rotation = Quaternion.Euler(startDir);
            }
        
        }
        */
    }
    
	public void SetAbility(float str, float vit, float agi)
	{
		STR = str;
		VIT = vit;
		AGI = agi;
		init ();
	}

	public void HitEvent(float radius = 0.3f, float damageActionScale = 1.0f, Vector3 sphereOffset = default(Vector3), bool relativeHit = false)
    {   
        //GameObject checkSphere = GameObject.Find("Body/rightHand");
		Collider[] hitObj = Physics.OverlapSphere(checkSphere.transform.position + sphereOffset, radius);
        Debug.Log("qqqq1");
        foreach (Collider c in hitObj)
        {
            Debug.Log("qqqq2");
            Debug.Log(c.gameObject.tag);
            Damagable cDamamgable = c.gameObject.GetComponent<Damagable>();
            if (c.gameObject != gameObject && cDamamgable!=null)
            {
                Debug.Log("qqqq3");
                if(relativeHit)
                    cDamamgable.Damage((c.transform.position - gameObject.transform.position).normalized * damageActionScale, ATK);
				else
                    cDamamgable.Damage(gameObject.transform.forward * damageActionScale, ATK);
			}
        }
    }

    public void Damage(Vector3 v,float atk)
	{
		mBox.Damage(v);
        HP -= atk;
        if (HP <= 0)
			mBox.Death();
       // onDamage = true;
       // damageDir = Vector3.Cross(v,new Vector3(0,1.0f,0));
      // endDir = new Vector3(0,-1,0) ;//damageDir*40 ;
      //  startDir = transform.forward;
       // gameObject.GetComponent<Animator>().Play("Damaged");
    }


    public void SetItem(Item item)
    {
        mItem = item;
        Debug.Log("ITEM GET");
    }

    public void UseItem()
    {
        if(mItem!=null)
        mItem.Use();
    }
}
