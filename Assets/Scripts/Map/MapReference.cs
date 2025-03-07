using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class MapReference : MonoBehaviour
{

    public GameObject[,] tiles;
    
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

/*     public int[,] map = {{ -1, -1, 4,-1,-1,-1,-1,-1,-1},
                  { -1, 0, 0,-1,0,-1,-1,-1,-1},    
                  { -1, 0, -1,-1,0,0,0,0,0},    
                  { 3, 0, -1,-1,0,-1,-1,-1,-1},    
                  { -1, 0, -1,-1,0,-1,-1,-1,-1},    
                  { -1, 0, -1,-1,0,-1,-1,-1,-1},    
                  { -1, 0, 0,0,0,2,-1,-1,-1},    
                  { -1, -1, 5,-1,-1,-1,-1,-1,-1}}; */

    public int[,] map = {{ 0, 3, -1, 4, -1, 5, 0, 0, 0, -1},
                  { 0, -1, 0, 0, -1, -1, -1, -1, 0, -1},    
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                  { -1, -1, -1, 0, -1, -1, -1, -1, 0, -1},
                  { -1, 0, 0, 0, -1, -1, 0, 0, 0, -1},
                  { -1, -1, -1, 0, -1, -1, 0, -1, 0, -1},
                  { 0, 3, -1, 0, 0, 0, 0, -1, 0, -1},    
                  { 0, -1, -1, 0, -1, 0, -1, -1, 0, -1},    
                  { 0, 0, 0, 0, 0, 0, -1, -1, 0, 3}};



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
        if(map[(int)pos.x,(int)pos.y] == 0)
        {
            map[(int)pos.x,(int)pos.y] = 1;
            ChangeColor(pos);
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

}
