using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Herbivore : MonoBehaviour
{
    public int fitness;
    public float maxVel;
    public float maxAccel;
    public float maxRot;
    public float[] wih;
    public float[][] who;
    public bool elite = false;
    private int nearestDeliciousVegetableIndex;
    private float speed = 0.0f;
    private float turnSpeed = 0.0f;
    private float turnAcc = 0.1f;
    private float dist;
    private float distToFood = Mathf.Infinity;
    private Vector3 pos;
    private Vector3 direction;
    private Vector3 deliciousVegetableDirecton;
    private GameObject mouth;
    private GameObject[] deliciousVegetables;
    private DeliciousVegetableScript[] deliciousVegetableScripts;
    // Start is called before the first frame update
    void Start()
    {
        GameObject deliciousVegetablesParent = GameObject.Find("DeliciousVegetables");
        deliciousVegetables = new GameObject[deliciousVegetablesParent.transform.childCount];
        deliciousVegetableScripts = new DeliciousVegetableScript[deliciousVegetables.Length];
        for (int i = 0; i < deliciousVegetables.Length; i++)
        {
            deliciousVegetables[i] = deliciousVegetablesParent.transform.GetChild(i).gameObject;
            deliciousVegetableScripts[i] = deliciousVegetables[i].GetComponent<DeliciousVegetableScript>();
        }
        mouth = transform.GetChild(0).gameObject;
        direction = mouth.transform.position - transform.position;
        direction = direction.normalized;
        pos = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        distToFood = Mathf.Infinity;
        for (int i = 0; i < deliciousVegetables.Length; i++)
        {
            if (deliciousVegetableScripts[i].isEaten == false)
            {
                dist = Vector3.Distance(transform.position, deliciousVegetables[i].transform.position);
                if (dist < distToFood)
                {
                    distToFood = dist;
                    nearestDeliciousVegetableIndex = i;
                }
            }
        }
        deliciousVegetableDirecton = transform.position - deliciousVegetables[nearestDeliciousVegetableIndex].transform.position;
        deliciousVegetableDirecton = deliciousVegetableDirecton.normalized;
        if (Vector3.Angle(deliciousVegetableDirecton, direction) > 0.0f)
        {
            if (Mathf.Abs(turnSpeed) < maxTurnSpeed)
            {
                if (Vector3.SignedAngle(deliciousVegetableDirecton, direction, new Vector3(0, 1, 0)) < 0)
                    turnSpeed += turnAcc;
            }
            transform.Rotate(0, Mathf.Rad2Deg * (Vector3.SignedAngle(deliciousVegetableDirecton, direction, new Vector3(0, 1, 0)) > 0 ? 1 : -1) * turnSpeed * Time.deltaTime, 0);
        }
        pos += direction * speed * Time.deltaTime;
        direction = mouth.transform.position - transform.position;
        direction = direction.normalized;
        transform.position = pos;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "deliciousVegetable")
        {
            DeliciousVegetableScript s = other.gameObject.GetComponent<DeliciousVegetableScript>();
            s.isEaten = true;
            fitness+=s.energy;
            s.energy = 0;
        }
    }
}
