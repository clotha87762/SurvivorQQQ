using UnityEngine;
using System.Collections;

public class Ice :Item{
    
    [SerializeField] GameObject IceBallPrefab;
	public int UseCount;
    //bool inUse;
    public float timeCount;
    public bool inCasting;

	void Start () {
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
        Vector3 pos = new Vector3(owner.transform.position.x, owner.transform.position.y , owner.transform.position.z);
        GameObject ball = Instantiate(IceBallPrefab, pos + owner.transform.forward, Quaternion.Euler(281.0f,242.0f,115.0f)) as GameObject;
        ball.GetComponent<IceBall>().SetSpeed(5.0f);
        ball.GetComponent<IceBall>().SetFreezeTime(2.5f);
        ball.GetComponent<IceBall>().SetOwner(owner);
        Destroy(ball, 10.0f);
        UseCount--;
        inCasting = true;
        return (UseCount <= 0) ? false : true;
    }
}

