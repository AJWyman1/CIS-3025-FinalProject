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
    public bool Discovered;
    public PlayerCharacter Hero;
    public bool isPway;

    public Dictionary<(int, int), char> DoorLocations;

    public Room(int X, int Y, int W, int L, int D, PlayerCharacter c)
    {
        this.X = X;
        this.Y = Y;
        this.Width = W;
        this.Length = L;

        this.Hero = c;
        this.Discovered = false;

        this.FarX = this.X + this.Width;
        this.FarY = this.Y + this.Length;

        this.NumDoors = D;
        this.DoorLocations = new Dictionary<(int, int), char>();

        if (this.Width == 1 && this.Length == 1)
        {
            this.isPway = true;
        }
        //this.SetDoorLocations();
    }


    public bool IsInRoom(int Xloc, int Yloc)
    {
        if (this.FarX > Xloc && this.X <= Xloc && this.FarY > Yloc && this.Y <= Yloc)
        {
            return true;
        }

        return false;
    }
    public bool PlayerInRoom()
    {
        if (IsInRoom(this.Hero.X, this.Hero.Y))
        {
            this.Discovered = true;
            return true;
        }
        if (this.Width == 1 && this.Length == 1)
        {
        if ((X >= Hero.X - 2 && X <= Hero.X + 2) && (Y >= Hero.Y - 2 && Y <= Hero.Y + 2))
        {
            this.Discovered = true;
            return true;
        }
        }
        return false;
    }

    public void AddDoor(int DoorX, int DoorY)
    {
        
        char Repr;
        if (DoorX == this.X || DoorX == this.X + this.Width - 1)
        {
            Repr = '━';
        }
        else 
        {
            Repr = '┃';
        }
        if (this.DoorLocations.ContainsKey((DoorX, DoorY)))
        {
            return;
        }
        this.DoorLocations.Add((DoorX, DoorY), Repr);
        // Console.WriteLine($"({this.X}, {this.Y}) ({this.FarX},{this.FarY})  W:{this.Width} L:{this.Length} {DoorX},{DoorY} {Repr}");
        // char input = Console.ReadKey(true).KeyChar;
    }

    public char[,] Representation(bool Override = false)
    {
        char[,] output = new char[this.Width, this.Length];

        if (this.isPway)
        {
            if (this.Discovered || PlayerInRoom() || Override)
                output[0,0] = '#';
            else
                output[0,0] = ' ';
            return output;
        }

        for(int i = 0; i < this.Width; i++)
        {
            for (int j = 0; j < this.Length; j++)
            {   
                char draw = ' '; //default
                if (this.PlayerInRoom() || Override)
                {
                    draw = '.'; //default
                }
                if (this.Discovered || Override)
                {
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