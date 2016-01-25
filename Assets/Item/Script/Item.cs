using UnityEngine;
using System.Collections;

public abstract class Item : MonoBehaviour {

	// Use this for initialization
    public bool eaten;
    public GameObject owner;

    void Start()
    {
        eaten = false;
    }

    void OnEnable()
    {
        eaten = false;
        gameObject.GetComponent<Renderer>().enabled = true;
    }

    public Item Discard()
    {
        if (owner != null)
        {
            owner = null;
        }
        eaten = false;
        gameObject.SetActive(false);
        return this;
    }

    public Item Reuse()
    {
        gameObject.SetActive(true);
        return this;
    }

    void OnDisable()
    {
        gameObject.GetComponent<Renderer>().enabled = true;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider c)
    {
        if (eaten)
        {
            return;
        }
        if (c.gameObject.tag == "Player")
        {
            c.gameObject.GetComponent<Damagable>().SetItem(this);
            gameObject.GetComponent<Renderer>().enabled = false;
            eaten = true;
            owner = c.gameObject;
        }
    }

    public bool isEaten()
    {
        return eaten;
    }

    public GameObject getOwner()
    {
        return owner.gameObject;
    }

    public abstract bool Use();


}
