using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MapReference : MonoBehaviour
{

    private static MapReference _Instance;
	public static MapReference Instance
	{
		get
		{
			if (!_Instance)
			{
				_Instance = new GameObject().AddComponent<MapReference>();
				// name it for easy recognition
				_Instance.name = _Instance.GetType().ToString();
				// mark root as DontDestroyOnLoad();
				DontDestroyOnLoad(_Instance.gameObject);
			}
			return _Instance;
		}
	}


    public GameObject[,] tiles;
    public Vector2 playerPosition = Vector2.positiveInfinity;

   /*
   Map Syntax:
   -1 = wall
   0 = unexplored path
   1 = explored path
   2 = minigame
   3 = item
   4 = start
   5 = end
   
   This may be changed later
   */

    // public int[,] map = {{ -1, -1, 4},
    //               { -1, 0, 0},    
    //               { 0, 0, -1},    
    //               { -1, 0, 0},    
    //               { -1, -1, 5}};

    // public int[,] map = {{ -1, -1, 4,-1,-1},
    //               { -1, 0, 0,-1,0},    
    //               { -1, 0, -1,-1,0},    
    //               { -1, 0, 0,0,0},    
    //               { -1, -1, 5,-1,-1}};

     public int[,] map = {{ -1, -1, 4,-1,-1,-1,-1,-1,-1},
                  { -1, 0, 0,-1,0,-1,-1,-1,-1},    
                  { -1, 0, -1,-1,0,0,0,0,0},    
                  { 3, 0, -1,-1,0,-1,-1,-1,-1},    
                  { -1, 0, -1,-1,0,-1,-1,-1,-1},    
                  { -1, 0, -1,-1,0,-1,-1,-1,-1},    
                  { -1, 0, 0,0,0,2,-1,-1,-1},    
                  { -1, -1, 5,-1,-1,-1,-1,-1,-1}}; 

    // public int[,] map = {{ 0, 3, -1, 4, -1, 5, 0, 0, 0, -1},
    //               { 0, -1, 0, 0, -1, -1, -1, -1, 0, -1},    
    //               { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
    //               { -1, -1, -1, 0, -1, -1, -1, -1, 0, -1},
    //               { -1, 0, 0, 0, -1, -1, 0, 0, 0, -1},
    //               { -1, -1, -1, 0, -1, -1, 0, -1, 0, -1},
    //               { 0, 3, -1, 0, 0, 0, 0, -1, 0, -1},    
    //               { 0, -1, -1, 0, -1, 0, -1, -1, 0, -1},    
    //               { 0, 0, 0, 0, 0, 0, -1, -1, 0, 3}};



    void ChangeColor(Vector2 pos) 
    {
        switch(map[(int)pos.x,(int)pos.y]) 
            {
            case 0:
                tiles[(int)pos.x,(int)pos.y].GetComponent<Image>().color = Color.gray;
                break;
            case 1:
                tiles[(int)pos.x,(int)pos.y].GetComponent<Image>().color= Color.white;
                break;
            case 2:
                tiles[(int)pos.x,(int)pos.y].GetComponent<Image>().color = new Color(255,0,183);
                break;
            case 3:
                tiles[(int)pos.x,(int)pos.y].GetComponent<Image>().color = Color.yellow;
                break;
            case 4:
                tiles[(int)pos.x,(int)pos.y].GetComponent<Image>().color = Color.green;
                break;
            case 5:
                tiles[(int)pos.x,(int)pos.y].GetComponent<Image>().color = Color.red;
                break;
            default:
                break;
            }
    }




    void CheckVisited(Vector2 pos)
    {
        switch(map[(int)pos.x,(int)pos.y])
        {
            case 0:
                map[(int)pos.x,(int)pos.y] = 1;
                ChangeColor(pos);
                break;
            case 2:
                Debug.Log("Minigame");
                map[(int)pos.x,(int)pos.y] = 1;
                SceneManager.LoadScene("TEST_MapMinigame");
                break;
            case 3:
                Debug.Log("Item");
                break;
            default:
                break;
        }

        
        
    }

    public void CheckPosition(int x, int y)
    {
        if(InBounds(x+1,y) && tiles[x+1,y] != null) ChangeColor(new Vector2(x+1,y)); // above
        if(InBounds(x-1,y) && tiles[x-1,y] != null) ChangeColor(new Vector2(x-1,y)); // below
        if(InBounds(x,y+1) && tiles[x,y+1] != null) ChangeColor(new Vector2(x,y+1)); // left
        if(InBounds(x,y-1) && tiles[x,y-1] != null) ChangeColor(new Vector2(x,y-1)); // left

        CheckVisited(new Vector2(x,y));
    }

    bool InBounds(int i, int j)
    {
        if(i > -1 && i < map.GetLength(0) && j > -1 && j < map.GetLength(1)) return true;
        return false;
    }

    public void SetPlayerPosition(int x, int y)
    {
        playerPosition.x = x;
        playerPosition.y = y;
    }

}
