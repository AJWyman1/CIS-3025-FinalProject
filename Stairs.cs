using System;

class Stairs : ILocatable
{
    public int X {get; set;}
    public int Y {get; set;}
    public Direction Facing {get; set;}
    public char RepresentWith { get; set; }
    public ConsoleColor Color { get; set; }
    public bool Down {get; private set;}
    public Stairs(int X, int Y, bool Down)
    {
        this.X = X;
        this.Y = Y;
        if (Down)
        {
            this.RepresentWith = '<'; //down
        }
        else //Up
        {
            this.RepresentWith = '>'; //up
        }

        this.Color = ConsoleColor.Green;
        this.Down = Down;
    }
}