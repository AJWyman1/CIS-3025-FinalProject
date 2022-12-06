using System;
using System.Collections.Generic;
using System.Linq;

class Room {

    public int X;
    public int Y;
    public int Width;
    public int Length;
    public int NumDoors;
    private int FarX;
    private int FarY;

    public Dictionary<(int, int), char> DoorLocations;

    public Room(int X, int Y, int W, int L, int D)
    {
        this.X = X;
        this.Y = Y;
        this.Width = W;
        this.Length = L;

        this.FarX = this.X + this.Width;
        this.FarY = this.Y + this.Length;

        this.NumDoors = D;
        this.DoorLocations = new Dictionary<(int, int), char>();

        this.SetDoorLocations();
    }

    public bool IsInRoom(int Xloc, int Yloc)
    {
        if (this.FarX > Xloc && this.X < Xloc && this.FarY > Yloc && this.Y < Yloc)
        {
            return true;
        }
        
        return false;
    }

    public void SetDoorLocations()
    {
        int DoorsSet = 0;
        do
        {
            int Side = Dice.D4();
            int DoorX;
            int DoorY;
            char Repr;

            if(Side == 1) //Top
                {
                    DoorX = this.X;
                    DoorY = this.Y + Dice.Roll(this.Length - 2);
                    Repr ='━';
                }
            else if (Side == 2) //Right
                {
                    DoorX = this.X + Dice.Roll(this.Width - 2);
                    DoorY = this.Y + this.Length - 1;
                    Repr = '┃';
                }
            else if(Side == 3) //Bottom
                {
                    DoorX = this.X + this.Width - 1;
                    DoorY = this.Y + Dice.Roll(this.Length - 2);
                    Repr = '━';
                }
            else //Left
                {
                    DoorX = this.X + Dice.Roll(this.Width - 2);
                    DoorY = this.Y;
                    Repr = '┃';
                }

            if (!this.DoorLocations.ContainsKey((DoorX,DoorY))) 
            {
                this.DoorLocations.Add((DoorX, DoorY), Repr);
                DoorsSet ++;
            }

        }while (DoorsSet < this.NumDoors);
    }

    public char[,] Representation()
    {
        char[,] output = new char[this.Width, this.Length];

        for(int i = 0; i < this.Width; i++)
        {
            for (int j = 0; j < this.Length; j++)
            {
                char draw = '.'; //default

                if (i == 0 || i == this.Width - 1)
                {
                    draw = '─'; //top and bottom default
                }
                else if (j == 0 || j == this.Length - 1)
                {
                    draw = '│'; //left and right default
                }
                
                if (i == 0) //top corners
                {
                    if (j == 0)
                    {
                        draw = '┌';
                    }
                    else if (j == this.Length - 1)
                    {
                        draw = '┐';
                    }
                }
                
                if (i == this.Width - 1) //bottom corners
                {
                    if (j == 0)
                    {
                        draw = '└';
                    }
                    else if (j == this.Length - 1)
                    {
                        draw = '┘';
                    }
                }

                if (this.DoorLocations.ContainsKey((i + this.X, j + this.Y))) //Doors
                {
                    draw = this.DoorLocations[(i + this.X, j + this.Y)];
                }

                output[i,j] = draw;
            }
        }

        return output;
    }
}