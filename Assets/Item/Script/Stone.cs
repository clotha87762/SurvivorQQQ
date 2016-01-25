using UnityEngine;
using System.Collections;

public class Stone : Item
{

    // Use this for initialization
    [SerializeField]
    GameObject StoneBallPrefab;
    public int UseCount;
    //bool inUse;
    public float timeCount;
    public bool inCasting;

    void Start()
    {
        UseCount = 1;
        //inUse = false;
        timeCount = 0;
        inCasting = false;
    }

    void OnEnable()
    {
        UseCount = 1;
        timeCount = 0;
        inCasting = false;
    }


    // Update is called once per frame
    void Update()
    {
        if (!eaten) return;

        if (inCasting)
        {
            timeCount += Time.deltaTime;
            if (timeCount >= 0.2f)
            {
                inCasting = false;
                owner.GetComponent<BoxController>().acting = false;
                timeCount = 0;
            }
        }
        else if (UseCount <= 0)
        {
            Discard();
        }

    }

    public override bool Use()
    {
        if (UseCount <= 0)
            return false;
        owner.GetComponent<BoxController>().acting = true;
        Vector3 pos = new Vector3(owner.transform.position.x, owner.transform.position.y, owner.transform.position.z);
        GameObject ball = Instantiate(StoneBallPrefab, pos + owner.transform.forward * 1.0f, owner.transform.rotation) as GameObject;
        ball.GetComponent<StoneBall>().SetSpeed(1.0f);
        ball.GetComponent<StoneBall>().SetDamage(2.0f);
        ball.GetComponent<StoneBall>().SetOwner(owner);

        Destroy(ball, 15.0f);
        UseCount--;
        inCasting = true;
        return (UseCount <= 0) ? false : true;
    }
}

