using System.Collections.Generic;
using UnityEngine;

public class JewelrySpawner : MonoBehaviour
{
    public GameObject SquareJewelry;

    void Start()
    {
        List<Vector3> vectors = new List<Vector3>();
        int i = 0;
        while(i <= 20)
        {
            Vector3 randomSpawner = new Vector3(Random.Range(-10,10), 1, Random.Range(-4, 4));
            if(!vectors.Contains(randomSpawner))
            {
                vectors.Add(randomSpawner);
                Instantiate(SquareJewelry, randomSpawner, Quaternion.identity);
                i++;
            }
        }  
    }
}
