using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    //Current generation
    public int gen = 0;
    //Evolution settings.
    public int pop = 50;
    public int food = 100;
    public int gens = 100;
    public float elitism = 0.2f;
    public float mutate = 0.1f;
    //Simulation settings.
    public int genTime = 60;
    float t = 0;
    public float maxRot = 720.0f;
    public float maxVel = 2.0f;
    public float maxAccel = 1.0f;
    private Vector2 min = new Vector2(-50, -50);
    private Vector2 max = new Vector2(50, 50);
    //Nodes
    public int inodes = 1;
    public int hnodes = 5;
    public int onodes = 2;
    //Delicious Vegetables
    public GameObject veggieParent;
    public GameObject deliciousVegetable;
    private GameObject[] deliciousVegetables;
    //Herbivores
    public GameObject herbivore;
    private GameObject[] herbivores;
    private Herbivore[] hScripts;
    // Start is called before the first frame update
    void Start()
    {
        deliciousVegetables = new GameObject[food];
        for (int i = 0; i < deliciousVegetables.Length; i++)
        {
            deliciousVegetables[i] = Instantiate(deliciousVegetable,
                new Vector3(Random.Range(max.x, min.x), 0, Random.Range(max.y, min.y)),
                Quaternion.identity);
            deliciousVegetables[i].transform.parent = veggieParent.transform;
        }
        herbivores = new GameObject[pop];
        hScripts = new Herbivore[pop];
        for (int i = 0; i < herbivores.Length; i++)
        {
            herbivores[i] = Instantiate(herbivore,
                new Vector3(Random.Range(max.x, min.x), 0, Random.Range(max.y, min.y)),
                Quaternion.Euler(0, Random.Range(0, 360.0f), 0));
            hScripts[i] = herbivores[i].GetComponent<Herbivore>();
            hScripts[i].wih = new float[5] { Random.Range(-1, 1),
                Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f),
                Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f) };
            hScripts[i].who = new float[2][];
            hScripts[i].who[0] = new float[5] { Random.Range(-1.0f, 1.0f),
                Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f),
                Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f) };
            hScripts[i].who[1] = new float[5] { Random.Range(-1.0f, 1.0f),
                Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f),
                Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f) };
            hScripts[i].maxAccel = maxAccel;
            hScripts[i].maxRot = maxRot;
            hScripts[i].maxVel = maxVel;
        }
    }

    // Update is called once per frame
    void Update()
    {
        t += Time.deltaTime;
        if (t >= genTime && gen!=gens) Evolve();
    }

    void Evolve()
    {
        int[] fitness = new int[hScripts.Length];
        Herbivore[] elites = new Herbivore[Mathf.FloorToInt(pop * elitism)];
        int highest=0;
        float avg = 0;
        for (int i = 0; i < fitness.Length; i++)
        {
            fitness[i] = hScripts[i].fitness;
            avg += fitness[i];
        }
        avg /= fitness.Length;
        for (int i = 0; i < Mathf.FloorToInt(pop * elitism); i++)
        {
            int maxFit = 0;
            int maxIndex = 0;
            for (int j = 0; j < fitness.Length; j++)
            {
                if (fitness[j] > maxFit)
                {
                    maxFit = fitness[j];
                    maxIndex = j;
                }
            }
            if (i == 0) highest = maxFit;
            fitness[maxIndex] = 0;
            hScripts[maxIndex].elite = true;
            elites[i] = hScripts[maxIndex];
            Debug.Log("Best: " + highest + "Average: " + avg);
        }
        for (int i = 0; i < pop; i++)
        {
            if (!hScripts[i].elite)
            {
                //Parents. Random.Range has exclusive max with integers.
                int[] pIndex = new int[2] { Random.Range(0, elites.Length), Random.Range(0, elites.Length) };
                while (pIndex[0] == pIndex[1]) pIndex[1] = Random.Range(0, elites.Length);
                float[] newWih = genWih(hScripts[pIndex[0]].wih, hScripts[pIndex[1]].wih);
                float[][] newWho = genWho(hScripts[pIndex[0]].who, hScripts[pIndex[1]].who);
                hScripts[i].wih = newWih;
                hScripts[i].who = newWho;
                if (Random.Range(0.0f, 1.0f) > mutate)
                {
                    //Implement mutation later
                }
                Vector3 newPos = new Vector3(Random.Range(max.x, min.x), 0, Random.Range(max.y, min.y));
                herbivores[i].transform.position = newPos;
                Quaternion newRot = Quaternion.Euler(0, Random.Range(0, 360), 0);
                herbivores[i].transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
            }
            else
            {
                hScripts[i].elite = false;
                hScripts[i].fitness = 0;
            }
        }
        gen++;
        t = 0;
    }
    float[] genWih(float[] p1, float[] p2)
    {
        float[] result = new float[5];
        float crossWeight = Random.Range(0.0f, 1.0f);
        for (int i = 0; i < result.Length; i++) result[i] = (crossWeight*p1[i])+((1-crossWeight)*p2[i]);
        return result;
    }
    float[][] genWho(float[][] p1, float[][] p2)
    {
        float[][] result = new float[2][] { new float[5], new float[5] };
        float crossWeight = Random.Range(0.0f, 1.0f);
        for (int i = 0; i < result.Length; i++)
        {
            result[0][i] = (crossWeight * p1[0][i]) + ((1 - crossWeight) * p2[0][i]);
            result[1][i] = (crossWeight * p1[1][i]) + ((1 - crossWeight) * p2[1][i]);
        }
        return result;
    }
}
