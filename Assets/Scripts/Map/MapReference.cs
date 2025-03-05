using UnityEngine;

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

    public int[,] map = {{ -1, -1, 4,-1,-1},
                  { -1, 0, 0,-1,0},    
                  { -1, 0, -1,-1,0},    
                  { -1, 0, 0,0,0},    
                  { -1, -1, 5,-1,-1}};

}
