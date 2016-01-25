using UnityEngine;
using System.Collections;

public class Fire : Item
{

    // Use this for initialization
    [SerializeField]
    GameObject FireBallPrefab;
    public int UseCount;
    //bool inUse;
    public float timeCount;
    public bool inCasting;

    void Start()
    {
        UseCount = 5;
        //inUse = false;
        timeCount = 0;
        inCasting = false;
    }

    void OnEnable()
    {
        UseCount = 5;
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
        GameObject ball = Instantiate(FireBallPrefab, pos + owner.transform.forward, owner.transform.rotation) as GameObject;
        ball.GetComponent<FireBall>().SetVec(owner.transform.forward);
        ball.GetComponent<FireBall>().SetDmage(2);
        Destroy(ball, 5.0f);
        UseCount--;
        inCasting = true;
        return (UseCount <= 0) ? false : true;
    }
}
