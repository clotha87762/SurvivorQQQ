﻿using UnityEngine;
using System.Collections;

public class FireBall : MonoBehaviour {

	// Use this for initialization

    [SerializeField] private ParticleSystem explode;
    float timeCount;
    Vector3 InitVec;
    float damage;

    void Start () {
        timeCount = 0;    
	}
	
	// Update is called once per frame
	void Update () {
        transform.position += 15*InitVec * Time.deltaTime;
       // timeCount += Time.deltaTime;
       // if (timeCount >= 5)
         //   Destroy(this);
	}

    void OnCollisionEnter(Collision c)
    {
		Debug.Log ("FIRE HIT: " + c.transform.tag);
		if (!c.collider.isTrigger)
		{
			if(c.transform.tag == "Player" || c.transform.tag == "Monster")
			{
				c.gameObject.GetComponent<Damagable>().Damage(InitVec,damage);
			}
			Destroy(Instantiate(explode.gameObject,transform.position,Quaternion.identity) as GameObject,explode.startLifetime);
			Destroy(gameObject);
        }
    }

    public void SetVec(Vector3 v)
    {
        InitVec = v;
    }

    public void SetDmage(float d)
    {
        damage = d;
    }
    
}
