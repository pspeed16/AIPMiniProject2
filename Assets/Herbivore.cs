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
        pos = transform.position;
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
        
        float[] thoughts = Think();
        transform.Rotate(new Vector3(0, 1, 0), (thoughts[1] * maxRot * Time.deltaTime));
        direction = mouth.transform.position - transform.position;
        direction = direction.normalized;
        speed += thoughts[0] * maxAccel * Time.deltaTime;
        if (speed < 0) speed = 0;
        if (speed < maxVel) speed = maxVel;
        pos = transform.position;
        pos += direction * speed * Time.deltaTime;
        pos.y = 0;
        transform.position = pos;
    }
    private float[] Think()
    {
        float[] result = new float[2];
        float[] h1 = new float[5];
        for (int i = 0; i < h1.Length; i++) h1[i] = (float)System.Math.Tanh(wih[i] * -Vector3.SignedAngle(direction,deliciousVegetableDirecton,Vector3.up));
        for (int i = 0; i < h1.Length; i++) {
            result[0] += (float)System.Math.Tanh(who[0][i] * h1[i]);
            result[1] += (float)System.Math.Tanh(who[1][i] * h1[i]);
        }
        return result;
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
