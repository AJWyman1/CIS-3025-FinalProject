using System;

class Program
{
    public static void Main(string[] args)
    {
        Console.Clear();
        //ConstructionandBattleTest();

        IMobileTest();
        IUsableTest();

        Console.WriteLine("Would you like to play a game? [y/n]");
        if (Console.ReadKey(true).KeyChar == 'y')
        {
            Console.Clear();
            PlayerCharacter Larry = new PlayerCharacter();
            Larry.EquipWeapon(new Greataxe());

            PotionOfHealing HealthPot = new PotionOfHealing();

            Larry.PickUpItem(new Schimitar());
            Larry.PickUpItem(HealthPot);
            
            GameTest(Larry);
        }
    }

    public static void ConstructionandBattleTest()
    {
        PlayerCharacter Larry = new PlayerCharacter();

        PotionOfHealing HealthPot = new PotionOfHealing();
        PotionOfHealing HealthPot2 = new PotionOfHealing("Big Health pot", 20, 4, 30);
        PotionOfHealing HealthPot3 = new PotionOfHealing("Small Health pot", 3, 1, 0);

        Longsword sword = new Longsword();

        Console.WriteLine();

        Larry.PickUpItem(sword);

        Larry.PickUpItem(HealthPot2);
        Larry.PickUpItem(HealthPot);
        Larry.PickUpItem(HealthPot3);

        Console.WriteLine(Larry);
        Console.WriteLine();

        Larry.EquipWeapon(sword);

        Console.WriteLine(Larry);
        Console.WriteLine();

        Console.WriteLine(Larry.Proficiencies);


        GiantRat ratty = new GiantRat();

        Console.WriteLine(ratty);
        Console.WriteLine();


        SparringMatch(ratty, Larry);

        Console.WriteLine(Larry.Rest());
        Console.WriteLine(Larry.Rest());
        Console.WriteLine(Larry.Rest());
        Console.WriteLine();

        Bandit band = new Bandit();

        SparringMatch(Larry, band);

        Hobgoblin gobby = new Hobgoblin();

        Console.WriteLine(gobby);
        Console.WriteLine();

        SparringMatch(Larry, gobby);
    }

    private static void SparringMatch(Creature attacker, Creature defender)
    {
        //System.Console.Clear();
        System.Console.WriteLine("========================");
        System.Console.WriteLine("Pretend Fight between " + attacker.Name + " and " +
                                  defender.Name + "\n\n\n");
        while ((attacker.HP > 0) && (defender.HP > 0))
        {
            System.Console.WriteLine(attacker.Attack(defender));
            System.Console.WriteLine(defender.Attack(attacker));
        }

        if (attacker.HP <= 0)
        {
            System.Console.WriteLine("Attacker " + attacker.Name + " died!");
        }
        if (defender.HP <= 0)
        {
            System.Console.WriteLine("Defender " + defender.Name + " died!");
        }

    }
    
    // public static Creature[] PopulateRoom(int numMonsters, int level, ref char[,] room)
    // {
    //     Creature[] Monsters = new Creature[numMonsters];

    // }
    public static void GameTest(PlayerCharacter c)
    {
        Dungeon d = new Dungeon();
        Console.Clear();
        //char[,] map = d.MakeRoom();
        //char[,] orig = (char[,])map.Clone();

        c.GoTo(1, 1);
        d.PlaceHeroInRoom(c);
        //d.Map[c.X, c.Y] = (char)c;

        d.PlaceCreatureInRoom();

        c.RollInitiative();
        d.PrintMap();

        ConsoleKeyInfo keyInfo;
        do
        {
            
            d.PrintMap();
            PrintCharSheet(c);
            keyInfo = Console.ReadKey(true);
            int lastX = c.X;
            int lastY = c.Y;
            if (keyInfo.KeyChar == 'w')
            {
                if (d.MovementAllowed(c.X - 1, c.Y, c))
                    { c.MoveNorth(); }
            }
            else
            if (keyInfo.KeyChar == 'a')
            {
                if (d.MovementAllowed(c.X, c.Y - 1, c))
                    { c.MoveWest(); }
            }
            else
            if (keyInfo.KeyChar == 's')
            {
                if (d.MovementAllowed(c.X + 1, c.Y, c))
                    { c.MoveSouth(); }
            }
            else
            if (keyInfo.KeyChar == 'd')
            {
                if (d.MovementAllowed(c.X, c.Y + 1, c))
                    { c.MoveEast(); }
            }
            else
            if (keyInfo.KeyChar == 'r')
            {
                Console.WriteLine(c.Rest());
            }
            else
            {
                int input = (int)Char.GetNumericValue(keyInfo.KeyChar) - 1;
                if ( input >= 0 && c.Inventory[input] != null)
                {
                    Console.Clear();
                    d.PrintMap();
                    PrintCharSheet(c);
                    Console.WriteLine(c.Inventory[input].Use(c));
                }
            }
            d.Map[lastX, lastY] = d.EmptyMap[lastX, lastY];
            d.Map[c.X, c.Y] = (char)c;
            
            if (lastX != c.X || lastY != c.Y)
            {
                d.MovementUpdate(c, lastX, lastY);
                Console.Clear();
            }
            
        } while (keyInfo.Key != ConsoleKey.Escape && c.HP > 0);
        if(c.HP <= 0){Console.WriteLine($"YOU DIED{new String(' ', Console.BufferWidth / 2)}");}
    
    }
    
    public static void PrintCharSheet(Creature c)
    {
        int width = Console.WindowWidth;
        int height = Console.WindowHeight;
        string[] charsheet = c.ToString().Split('\n');
        for (int i = 0; i < charsheet.Length; i++)
        {
            Console.SetCursorPosition(width - 40, i);
            Console.WriteLine(charsheet[i].PadRight(39));
        }
        Console.SetCursorPosition(width - 40, charsheet.Length);
        Console.WriteLine("".PadRight(40,'*'));
        Console.SetCursorPosition(width - 30, charsheet.Length+1);
        Console.WriteLine("Movement Controls");
        Console.SetCursorPosition(width - 24, charsheet.Length+2);
        Console.WriteLine("  ↑");
        Console.SetCursorPosition(width - 24, charsheet.Length+3);
        Console.WriteLine("  W");
        Console.SetCursorPosition(width - 24, charsheet.Length+4);
        Console.WriteLine("←ASD→");
        Console.SetCursorPosition(width - 24, charsheet.Length+5);
        Console.WriteLine("  ↓");
        Console.SetCursorPosition(width - 30, charsheet.Length+6);
        Console.WriteLine("[r]est");
    }

    public static void IMobileTest()
    {
        Hobgoblin TestDummy = new Hobgoblin();

        Console.WriteLine("TESTING IMobile and ILocatable interfaces");
        Console.WriteLine($"The Test Dummy's location is {TestDummy.X}, {TestDummy.Y}");

        Console.WriteLine("testing GoTo(10, 10):");
        TestDummy.GoTo(10, 10);
        Console.WriteLine($"The Test Dummy's location is {TestDummy.X}, {TestDummy.Y}");
        Console.WriteLine();

        Console.WriteLine("testing MoveNorth()");
        TestDummy.MoveNorth();
        Console.WriteLine($"The Test Dummy's location is {TestDummy.X}, {TestDummy.Y}");
        Console.WriteLine();

        Console.WriteLine("testing MoveEast()");
        TestDummy.MoveEast();
        Console.WriteLine($"The Test Dummy's location is {TestDummy.X}, {TestDummy.Y}");
        Console.WriteLine();

        Console.WriteLine("testing MoveSouth()");
        TestDummy.MoveSouth();
        Console.WriteLine($"The Test Dummy's location is {TestDummy.X}, {TestDummy.Y}");
        Console.WriteLine();

        Console.WriteLine("testing MoveWest()");
        TestDummy.MoveWest();
        Console.WriteLine($"The Test Dummy's location is {TestDummy.X}, {TestDummy.Y}");
        Console.WriteLine();

        Console.WriteLine("testing MoveNorthEast()");
        TestDummy.MoveNorthEast();
        Console.WriteLine($"The Test Dummy's location is {TestDummy.X}, {TestDummy.Y}");
        Console.WriteLine();

        Console.WriteLine("testing MoveSouthEast()");
        TestDummy.MoveSouthEast();
        Console.WriteLine($"The Test Dummy's location is {TestDummy.X}, {TestDummy.Y}");
        Console.WriteLine();

        Console.WriteLine("testing MoveSouthWest()");
        TestDummy.MoveSouthWest();
        Console.WriteLine($"The Test Dummy's location is {TestDummy.X}, {TestDummy.Y}");
        Console.WriteLine();

        Console.WriteLine("testing MoveNorthWest()");
        TestDummy.MoveNorthWest();
        Console.WriteLine($"The Test Dummy's location is {TestDummy.X}, {TestDummy.Y}");
        Console.WriteLine();

        Console.WriteLine("testing Face(North)");
        TestDummy.Face(Direction.NORTH);
        Console.WriteLine($"The Test Dummy is facing {TestDummy.Facing}");
        Console.WriteLine();

        Console.WriteLine("testing MoveForward()");
        TestDummy.MoveForward();
        Console.WriteLine($"The Test Dummy's location is {TestDummy.X}, {TestDummy.Y}");
        Console.WriteLine();

        Console.WriteLine($"The Test Dummy is facing {TestDummy.Facing}");
        Console.WriteLine("testing Turn(180)");
        TestDummy.Turn(180);
        Console.WriteLine($"The Test Dummy is facing {TestDummy.Facing}");
        Console.WriteLine();
    }

    public static void IUsableTest()
    {
        PotionOfHealing TestPotion = new PotionOfHealing();
        Quarterstaff TestStaff = new Quarterstaff();

        Hobgoblin TestDummy = new Hobgoblin();
        TestDummy.PickUpItem(TestStaff);

        Console.WriteLine($"Testing IUsable items {TestPotion.Name} and {TestStaff.Name}");

        Console.WriteLine($"The Test Potion has {TestPotion.UsesLeft} uses left.");
        Console.WriteLine($"The Test Potion has a {TestPotion.UseChance} use chance.");
        Console.WriteLine();

        Console.WriteLine($"The potion's Success message is \"{TestPotion.SuccessMessage()}\"");
        Console.WriteLine($"The potion's Failure message is \"{TestPotion.FailureMessage()}\"");
        Console.WriteLine($"The potion's Use message is \"{TestPotion.Use(TestDummy)}\"");
        Console.WriteLine();

        Console.WriteLine($"The Quarterstaff has {TestStaff.UsesLeft} uses left.");
        Console.WriteLine($"The Quarterstaff has a {TestStaff.UseChance} use chance.");
        Console.WriteLine();

        Console.WriteLine($"The Quarterstaff's Success message is \"{TestStaff.SuccessMessage()}\"");
        Console.WriteLine($"The Quarterstaff's Failure message is \"{TestStaff.FailureMessage()}\"");
        Console.WriteLine($"The Quarterstaff's Use message is \"{TestStaff.Use(TestDummy)}\"");
        Console.WriteLine();
    }
}