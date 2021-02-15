using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Enum for movement's direction
public enum Dirs
{
    UP,
    DOWN,
    LEFT,
    RIGHT 
}

/*
 * Class TaquinGame use to initiate and play the game
 * TODO : - Refactor
 *        - Extends some functions to N*N game
 */
public class TaquinGame : MonoBehaviour
{
    // Public list of Cube use to play
    public List<GameObject> Palets = new List<GameObject>(9);

    // Size of the game (N * N)
    public int size = 3;

    // Variables use to handle the selection and movement of the palets
    private bool buttonDown;
    private Vector3 mouseStart;
    private Vector3 mouseFinish;
    private Vector3 goTo;
    private Vector3 direction;

    // Palets clicked
    private GameObject targetPalet;

    // Variables use to create chrono and stop game on winning/losing cases
    public float targetTime = 180.0f;
    public Text ChronoText;
    public Text WinningSon;
    private bool timeRunning = true;
    private Color chronoColor = new Color(1,1,1,1);

    // Static possible positions of the palet (3*3 grid)
    private readonly List<Vector3> listOfPostion = new List<Vector3>()
    {
        new Vector3(0,0,0),new Vector3(1,0,0),new Vector3(2,0,0),
        new Vector3(0,1,0),new Vector3(1,1,0),new Vector3(2,1,0),
        new Vector3(0,2,0),new Vector3(1,2,0),new Vector3(2,2,0)
    };

    // Grid of the game. True if there is no Palets on one of the positions above (init to false)
    private List<bool> isEmpty = new List<bool>()
        {false,false,false,
        false,false,false,
        false,false,false};



    // Start is called before the first frame update
    void Start()
    {
        generateTaquin();
    }


    // Update is called once per frame
    void Update()
    {   
        if (timeRunning)
        {
            // Detect if we move a Palet
            this.DetectMove();
            this.DetectPalet();

            // Chrono and point
            targetTime -= Time.deltaTime;
            this.Chrono(targetTime);
            
            // Game stops if chrono = 0
            if (targetTime <= 0.0f)
            {
                timeRunning = false;
                timerEnded();
            }
        }

        // Win if the condition (palet to the right spot)
        if (isWin() & timeRunning)
        {
            timeRunning = false;
            this.win();
        }
    }

    // Display Chrono value. Change color while decrementing
    void Chrono(float tTime)
    {  
        if(tTime < 120 & tTime > 60)
        {
            chronoColor = Color.yellow;
        }
        if(tTime < 60)
        {
            chronoColor = Color.red;
        }
        ChronoText.color = chronoColor;
        ChronoText.text = ((int)tTime).ToString();
    }

    // Put in targetPalet the palet the user click on
    void DetectPalet()
    {
        // Ray cast on click
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Input.GetMouseButtonDown(0))
        {
            // Get the cube collide by ray
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.tag == "Palet")
                {
                    // Colider become our target
                    targetPalet = hit.transform.gameObject;

                }
            }
        
        }
    }

    /*
     * Fonction to detect the direction of the click drag drop
     */
    void DetectMove()
    {
        
        // On left click
        if (Input.GetMouseButtonDown(0))
        {
            // Save the position of left-click
            buttonDown = true;
            Ray rayStart = Camera.main.ScreenPointToRay(Input.mousePosition);
            mouseStart = rayStart.origin;
        }

        // On left release
        if (Input.GetMouseButtonUp(0))
        {
            // Check if we were dragging
            if (buttonDown)
            {
                Ray rayEnd = Camera.main.ScreenPointToRay(Input.mousePosition);
                mouseFinish = rayEnd.origin;

                // Give the direction of the drag n drop in Vector3 ([-1 -> 1], [-1 -> 1],0)
                goTo = mouseFinish - mouseStart;
                float distance = goTo.magnitude;
                direction = goTo / distance;

                // Check if possible movement (we exclude diagonale with margin of error)
                if (CheckDiagonale(direction))
                {
                    this.MovePalet(direction);
                }
                
            }
        }
    }

    /*
     * CheckDiagonale take a Vector3 of a direction  ([-1 -> 1], [-1 -> 1],0)
     * Personals notes :  TODO : Refactor
     */
    private bool  CheckDiagonale(Vector3 dirToCheck)
    {
        // If diagonale then impossible movement
        // When the direction is around -0.5 or 0.5 is like 45°. So we exclude this movement to avoid diagonale
        if((dirToCheck.x > 0.5f || dirToCheck.x < -0.5f) && (dirToCheck.y < -0.5f || dirToCheck.y > 0.5f))
        {
            Debug.Log("Mouvement Impossible");
            return false;
        }
        // Else possible movement
        return true;
    }

    /*
     * MovePalet move cubes on possible movement (not diagonale) take Vector3 already check
     * We don't do movement in diagonale, now we need to check linear movement
     * Personals notes :  TODO : Refactor
     */
    private void MovePalet(Vector3 dirToMove)
    {
        // Left-right movement
       if(Mathf.Abs(dirToMove.x) > Mathf.Abs(dirToMove.y))
        {
            // Left
            if(dirToMove.x < 0)
            {
                CheckMovePalet(Dirs.LEFT, targetPalet);
            }
            // Right
            else
            {
                CheckMovePalet(Dirs.RIGHT, targetPalet);
            }
        }
        
       // Up-Down movement
        if (Mathf.Abs(dirToMove.x) < Mathf.Abs(dirToMove.y))
        {
            // Down
            if (dirToMove.y < 0)
            {
                CheckMovePalet(Dirs.DOWN, targetPalet);
            }
            // Up
            else
            {
                CheckMovePalet(Dirs.UP, targetPalet);
            }
        }
    }

    /*
     * Check if the selected Palet can move
     * Personals notes :  TODO : Refactor this function is doing two things at the same time (check and move) the boolean is never use
     */
    public bool CheckMovePalet(Dirs dirToGo, GameObject PaletToMove)
    {
        // Check the side of the game 

        // A bottom palet can't move down
        if((PaletToMove.transform.position.y == 0) && (dirToGo == Dirs.DOWN)) 
        {
            return false;
        }
        // A top palet can't move up
        if ((PaletToMove.transform.position.y == 2) && (dirToGo == Dirs.UP)) 
        {
            return false;
        }
        // A left palet can't go more left
        if ((PaletToMove.transform.position.x == 0) && (dirToGo == Dirs.LEFT)) 
        {
            return false;
        }
        // Right palet can't fo more right
        if ((PaletToMove.transform.position.x == 2) && (dirToGo == Dirs.RIGHT)) 
        {
            return false;
        }

        // Check if the future position is empty
        // Personals notes : Shouldn't be here this part need to be refactor and move
        int indexOfActualPos = listOfPostion.IndexOf(PaletToMove.transform.position);
        if(this.isEmpty[indexOfActualPos + dirsToInt(dirToGo)])
        {
            // here the destination is correct and empty so we move
            targetPalet.transform.position = listOfPostion[indexOfActualPos + dirsToInt(dirToGo)];
            // Setting last positiion as empty and the next as not empty
            this.isEmpty[indexOfActualPos] = true;
            this.isEmpty[indexOfActualPos + dirsToInt(dirToGo)] = false;
        }
        

        return false;
    }

    /*
     * Function to cast enum Dirs in Int to navigate index of pos
     * Use to navigate in 1D list as 2D array
     */
    private int dirsToInt(Dirs d)
    {
        int res = 0;
        if (d == Dirs.UP) res = 3;
        if (d == Dirs.DOWN) res = -3;
        if (d == Dirs.LEFT) res = -1;
        if (d == Dirs.RIGHT) res = +1;

        return res;
    }

    /*
     * When chrono is 0 display gameover
     */
    private void timerEnded()
    {
        WinningSon.color = Color.red;
        WinningSon.text = "Game Over";
    }

    /* 
     * When all the palets are at the good place the player win
     * Display all my gg and congrats
     */
    private void win()
    {
        WinningSon.color = Color.green;
        WinningSon.text = "Win";
        if (isMax((int)this.targetTime))
        {
            MaxSaver.GetInstance().SaveGame((int)targetTime);
        }
    }

    /*
     * Look for new personna lbest
     */
    private bool isMax(int score)
    {
        return score > MaxSaver.GetInstance().MaxToSave;
    }

    /*
     * Check if each Palet is at his original position 
     * TO OPTIMIZE (ugly code)
     */
    private bool isWin()
    {
        if (Palets[0].transform.position != new Vector3(0, 0, 0)) return false;
        if (Palets[1].transform.position != new Vector3(1, 0, 0)) return false;
        if (Palets[2].transform.position != new Vector3(2, 0, 0)) return false;
        if (Palets[3].transform.position != new Vector3(0, 1, 0)) return false;
        if (Palets[4].transform.position != new Vector3(2, 1, 0)) return false;
        if (Palets[5].transform.position != new Vector3(0, 2, 0)) return false;
        if (Palets[6].transform.position != new Vector3(1, 2, 0)) return false;
        if (Palets[7].transform.position != new Vector3(2, 2, 0)) return false;

        return true;
    }

    /*
     * Generate a solvable taquin
     * Create list of index :
     *      -> Shuffle the index until it's solvable ( odd grid so we need even number of invert)
     * Dispose palet according to our grid (listOfPosition)
     * Delete the palet 0
     * play game
     */
    public void generateTaquin()
    {
        // Initialize list 
        List<int> taquinList = new List<int>();
        for (int i = 0; i < this.size * this.size; i++) taquinList.Add(i);

        // Shuffle it
        do
        {
            ShuffleTaquin(taquinList);
        } while (isSolvable(taquinList));

        // Display palets on the listOfPosition 
        for(int t = 0; t < this.size * this.size; t++)
        {
            Palets[taquinList[t]].transform.position = this.listOfPostion[t];
        }

        // Remove then delete palet number 0
        for(int d = 0; d < this.size *this.size; d++)
        {
            if (taquinList[d] == 0)
            {
                GameObject toDelete = Palets[taquinList[d]];
                this.isEmpty[d] = true;
                Palets.Remove(toDelete);
                Destroy(toDelete.gameObject);
            }
        }
    }

    /*
     * count the number of inversion. Our Taquin is 3*3 so it needs to be pair
     */
    public int CountInvert(List<int> taquinArray)
    {
        int count = 0;

        // If i is before j but i is greater, it's an inversion
        for(int i = 0; i < this.size * this.size -1; i++)
        {
            for(int j = i + 1; j < this.size * this.size; j++)
            {
                if(taquinArray[i] > taquinArray[j])
                {
                    count++;
                }
            }
        }

        return count;
    } 

    /*
     * It's solvable if the inversion is even
     */
    public bool isSolvable(List<int> taquinArray)
    {
        return (this.CountInvert(taquinArray) % 2 == 0);
    }

    /*
     * Do 1 shuffle on taquin list
     */
    public List<int> ShuffleTaquin(List<int> taquin)
    {
        for(int j = 0; j < 100; j++)
        {
            // Randomly permuting index some timl
            for (int i = 0; i < taquin.Count; i++)
            {
                int temp = taquin[i];
                int randomIndex = UnityEngine.Random.Range(0, taquin.Count);
                taquin[i] = taquin[randomIndex];
                taquin[randomIndex] = temp;
            }
        }
        
        return taquin;
    }
}
