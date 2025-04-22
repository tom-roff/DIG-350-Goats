using System.Collections.Generic;
using UnityEngine;

public class JewelrySpawner : MonoBehaviour
{
    public GameObject SquareJewelry;
    public GameObject Pearl;
    public GameObject Trash;

    void Start()
    {
        List<Vector3> vectors = new List<Vector3>();
        int i = 0;
        int pearl = 0;
        int square = 1;
        int trash = 0;
        while(i < 25)
        {
            Vector3 randomSpawner = new Vector3(Random.Range(-10,10), 1, Random.Range(-4, 4));
            if(!vectors.Contains(randomSpawner))
            {
                vectors.Add(randomSpawner);
                if(square == 1)
                {
                    Instantiate(SquareJewelry, randomSpawner, Quaternion.identity);
                    square = 0;
                    pearl = 1;
                }
                if(pearl == 1)
                {
                    Instantiate(Pearl, randomSpawner, Quaternion.identity);
                    pearl = 0;
                    trash = 1;
                } 
                if(trash == 1)
                {
                    Instantiate(Trash, randomSpawner, Quaternion.identity);
                    trash = 0;
                    square = 1;
                }
                i++;
            }
        }  
    }
}
