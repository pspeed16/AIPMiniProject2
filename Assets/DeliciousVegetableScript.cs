using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliciousVegetableScript : MonoBehaviour
{
    public int energy = 1;
    public bool isEaten = false;
    private void Update()
    {
        if (isEaten) gameObject.SetActive(false);
        if (!gameObject.activeSelf) respawn();
    }
    void respawn()
    {
        Vector3 pos = new Vector3(Random.Range(-25.0f, 25.0f), 0, Random.Range(-25.0f, 25.0f));
        transform.position = pos;
        energy = 1;
        isEaten = false;
        gameObject.SetActive(true);
    }
}
