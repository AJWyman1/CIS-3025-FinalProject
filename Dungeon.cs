using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class Dungeon {

    public char[,] Map;
    public char[,] EmptyMap;
    public Dictionary<(int, int), Creature> MonsterDict;
    public PlayerCharacter Hero;
    public Dictionary<(int, int, ILocatable), int> LootDict;

    public Dungeon()
    {
        this.Map = this.MakeRoom();
        this.EmptyMap = (char[,])this.Map.Clone();
        this.MonsterDict = new Dictionary<(int, int), Creature>();
        this.LootDict = new Dictionary<(int, int, ILocatable), int>();
    }

    public char[,] MakeRoom()
    {
        int H = Dice.D20() + 5;
        int W = Dice.D20() + 5;
        char[,] Room = new char[H,W];
        for (int i = 0; i < Room.GetLength(0); i++)
        {
            for (int j = 0; j < Room.GetLength(1); j++)
            {
                char fill = '.';
                if(i == 0 || i == H-1 || j == 0 || j == W-1)
                {
                    fill = '#';
                }
                Room[i,j] = fill;
            }
        }
        return Room;
    }
    public void PlaceHeroInRoom(PlayerCharacter p)
    {
        this.Hero = p;
    }

    public void PlaceCreatureInRoom()
    {
        int X;
        int Y;
        do{
        X = Dice.Roll(this.Map.GetLength(0) - 2);
        Y =  Dice.Roll(this.Map.GetLength(1) - 2);
        }while(this.MonsterDict.ContainsKey((X, Y))); //Rolls dice for location

        int MonsterRoll = Dice.D20();
        if (MonsterRoll < 5)
        {
            this.MonsterDict.Add((X, Y), new GiantRat());
        }
        else if (MonsterRoll < 10)
        {
            this.MonsterDict.Add((X, Y), new Bandit());
        }
        else if (MonsterRoll < 15)
        {
            this.MonsterDict.Add((X, Y), new Hobgoblin());
        }
        else if (MonsterRoll < 19)
        {
            this.MonsterDict.Add((X, Y), new Orc());
        }
        else if (MonsterRoll == 20)
        {
            this.MonsterDict.Add((X, Y), new Mimic());
        }


        this.MonsterDict[(X, Y)].GoTo(X, Y);
        this.MonsterDict[(X, Y)].RollInitiative();
        this.Map[X,Y] = (char)this.MonsterDict[(X, Y)];
    }

    public bool MovementAllowed(int X, int Y, Creature c)
    {
        if (this.Map[X,Y] == '#')
            return false;
        if (this.MonsterDict.ContainsKey((X, Y)))
        {
            if (this.MonsterDict[(X,Y)].IsAlive)
            {
                PrintFightLog(c, this.MonsterDict[(X,Y)]);
                return false;
            }
        }
        return true;
    }

    public void PrintFightLog(Creature a, Creature b)
    {
        Creature[] fighters = {a, b};
        Array.Sort(fighters);
        //Console.SetCursorPosition(0, 60);
        
        Console.WriteLine($"{fighters[0].Attack(fighters[1]).PadRight(Console.WindowWidth)}");
        if(fighters[1].HP > 0) //Benefit of higher initiative
        {
        Console.WriteLine($"{fighters[1].Attack(fighters[0]).PadRight(Console.WindowWidth)}");
        }
        this.PostFightUpdate(a);
        this.PostFightUpdate(b);
    }

    public void PostFightUpdate(Creature c)
    {
        if (!c.IsAlive)
        {
            this.EmptyMap[c.X,c.Y] = 'x';
            this.MonsterDict.Remove((c.X,c.Y));
        }
    }

    public void MovementUpdate(Creature c, int LastX, int LastY)
    {
        this.MonsterDict.Remove((LastX, LastY));
        this.MonsterDict.Add((c.X, c.Y), c);
    }

    public Creature GetCreature(int x, int y)
    {
        return this.MonsterDict[(x, y)];
    }


    public void PrintMap()
    {
        Console.SetCursorPosition(0, 0);
        for (int i = 0; i < this.Map.GetLength(0); i++)
        {
            for (int j = 0; j < this.Map.GetLength(1); j++)
            {
                if(this.MonsterDict.ContainsKey((i, j)))
                {
                    Console.Write((char)this.MonsterDict[(i, j)]);
                }
                else if (i == this.Hero.X && j == this.Hero.Y)
                {
                    Console.Write((char)this.Hero);
                }
                else
                {
                    Console.Write(this.EmptyMap[i, j]);
                }
            }
            Console.Write("\n");
        }
    }
}
