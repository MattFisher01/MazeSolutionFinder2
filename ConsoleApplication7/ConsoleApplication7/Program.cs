using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication7
{

    /*Program Overview:
    After use has entered a file path (or a default one is used), the LoadMaze method is used to read the text file line by line, parsing the start and end coordinates,
    as well as the maze itself.
    Starting at the start coordinates, the program loops each cell, checking to see if there are possible moves N, E, S and W that have not already been visited.
    If there is more than one possible move, the current cell is noted in the NodeList.  If a deadend is reached, program sets position back to last known node and continues from there.
    */

    class Program
    {
        static void Main(string[] args)
        {
            string FA;
            Console.WriteLine("Please enter the maze filepath, including the file extension of file");
            string UserResponse = Console.ReadLine();
            if (UserResponse == "")
            {
                FA = @"C:\Users\Matt\Documents\SmallMaze.txt";
            }
            else
            {
                FA = UserResponse;
            }
            TextFileToArray TFTA = new TextFileToArray(FA);
            TFTA.LoadMaze();
            TFTA.FindSolution();
            Console.WriteLine("Solution Found!");
            Console.ReadLine();
        }
    }
}


public class TextFileToArray
{
    string _FileAddress;
    string _Line;
    int[,] Maze;
    int[,] CellsVisited;
    int[,] DirectionCanMove = new int[4, 3];
    int i = 0;
    int k;
    int StartCoordX;
    int StartCoordY;
    int EndCoordX;
    int EndCoordY;
    int ArrayX;
    int ArrayY;
    int ArrayCounterX;
    int[] Position = new int[2];
    Stack<Node> NodeList = new Stack<Node>();
    int NumberOfPossibleRoutes;
    bool EndReachedFlag = false;

    public TextFileToArray(string FileAddress)
    {
        this._FileAddress = FileAddress;
    }

    public int[] StartCoordinates
    {
        get
        {
            int[] Coords = new int[2];
            Coords[0] = this.StartCoordX;
            Coords[1] = this.StartCoordY;
            return Coords;
        }
    }

    public void LoadMaze()
    {
        StreamReader SR = new StreamReader(_FileAddress);

        while ((_Line = SR.ReadLine()) != null)
        {
            ++i;
            ArrayCounterX = 0;

            if (i == 1)
            {
                //Read in width and height of array
                ArrayX = Convert.ToInt32(_Line[0].ToString());
                ArrayY = Convert.ToInt32(_Line[2].ToString());
                //declare array dimensions
                Maze = new int[(ArrayX), (ArrayY)];
                CellsVisited = new int[(ArrayX), (ArrayY)];
            }
            else if (i == 2)
            {
                //Start Coordinates
                StartCoordX = Convert.ToInt32(_Line[0].ToString());
                StartCoordY = Convert.ToInt32(_Line[2].ToString());
            }
            else if (i == 3)
            {
                //End Coordinates
                EndCoordX = Convert.ToInt32(_Line[0].ToString());
                EndCoordY = Convert.ToInt32(_Line[2].ToString());
            }
            else if (i > 3)
            {
                //Read in Maze to array
                foreach (char c in _Line)
                {
                    if (c != 32)        //32 is unicode decimal for space
                    {
                        //Add to array
                        Maze[ArrayCounterX, (i - 4)] = Convert.ToInt32(c.ToString());
                        ++ArrayCounterX;
                    }
                }
            }
        }
    }

    public void FindSolution()
    {
        //Set position to start position
        int[] SC = this.StartCoordinates;
        Position[0] = SC[0];
        Position[1] = SC[1];
        CellsVisited[Position[0],Position[1]]=1;
        
        //Loop until we find the solution
        while (!this.HasReachedEnd())
        {
            this.CheckCanMove(Position[0], Position[1]);
            this.Move();
        }
    }

    public void CheckCanMove(int x, int y)
    {
        k = 0;
        NumberOfPossibleRoutes = 0;
        Array.Clear(DirectionCanMove, 0, 12);

        this.MoveNorth(x, y);
        this.MoveEast(x, y);
        this.MoveSouth(x, y);
        this.MoveWest(x, y);
        NumberOfPossibleRoutes = k;
    }

    public void MoveNorth(int X, int Y)
    {
        if (Y > 0)
        {
            if(Maze[X, (Y-1)]==0)
            {
                if (CellsVisited[X, (Y - 1)] == 0)
                {
                    DirectionCanMove[k, 0] = X;
                    DirectionCanMove[k, 1] = Y-1;
                    DirectionCanMove[k, 2] = 1;
                    ++k;
                }
            }
        }
    }

    public void MoveEast(int X, int Y)
    {
        if ( X < Maze.GetLength(0))                     //Check for boundary
        {
            if (Maze[X +1, Y] == 0)                     //Check cell is space and not a wall
            {
                if (CellsVisited[X+1, Y] == 0)          //Check cell not already visited
                {
                    DirectionCanMove[k, 0] = X+1;
                    DirectionCanMove[k, 1] = Y;
                    DirectionCanMove[k, 2] = 2;
                    ++k;
                }
            }
        }
    }

    public void MoveSouth(int X, int Y)
    {
        if (Y <Maze.GetLength(1))
        {
            if (Maze[X, (Y + 1)] == 0)
            {
                if (CellsVisited[X, (Y + 1)] == 0)
                {
                    DirectionCanMove[k, 0] = X;
                    DirectionCanMove[k, 1] = Y+1;
                    DirectionCanMove[k, 2] = 3;
                    ++k;
                }
            }
        }
    }

    public void MoveWest(int X, int Y)
    {
        if (X > 0)
        {
            if (Maze[X-1, Y] == 0)
            {
                if (CellsVisited[X-1, Y] == 0)
                {
                    DirectionCanMove[k, 0] = X;
                    DirectionCanMove[k, 1] = Y;
                    DirectionCanMove[k, 2] = 4;
                    ++k;
                }
            }
        }
    }

    public void Move()
    {
        if (NumberOfPossibleRoutes > 0)
        {
            //Add current position into Node list if it has more than one possible route - used for backtracking
            if (NumberOfPossibleRoutes > 1)      
            {
                NodeList.Push(new Node(Position[0], Position[1]));
            }

            //Use first available route to move to
            Position[0] = DirectionCanMove[0, 0];
            Position[1] = DirectionCanMove[0, 1];
            //Mark this new position as 'visited'
            CellsVisited[Position[0], Position[1]] = 1;
        }
        else if (NumberOfPossibleRoutes == 0)
        {
            //Have reached a dead end.  return to last node
            Node PreviousNode = NodeList.Pop();
            Position[0] = PreviousNode.GetX;
            Position[1] = PreviousNode.GetY;
            

        }
    }

    private bool HasReachedEnd()
    {
        if (Position[0] == EndCoordX && Position[1] == EndCoordY)
        {
            return true;
        }
        return false;
    }
}

public class Node
{
    int _X;
    int _Y;

    public Node(int X, int Y)
    {
        this._X = X;
        this._Y = Y;
    }

    public int GetX
    {
        get {return _X;}
    }

    public int GetY
    {
        get {return _Y;}
    }

}



