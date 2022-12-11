using System;
using System.Linq;
using System.Media;

class Program
{
    public static void Main(string[] args)
    {
        Console.Clear();

        Console.WriteLine("Would you like to play a game? [y/n]");
        char input = Console.ReadKey(true).KeyChar;
        if (input == 'y')
        {
            Console.Clear();
            PlayerCharacter Larry = new PlayerCharacter(ChooseRace(), SelectClass(), SetPlayerName(), ChooseStats());

            PotionOfHealing HealthPot = new PotionOfHealing();

            Larry.PickUpItem(HealthPot);

            PotionOfHealing HealthPot2 = new PotionOfHealing();
            Larry.PickUpItem(HealthPot2);
            
            Console.WriteLine("Before you is the entrance to a dungeon that is rumoured to contain a treasure chest full of wealth beyond your greatest imagination guarded by hordes of foes and others searching for wealth.\nAll those who have entered either come out scarred and empty handed or not at all. \nDo you dare to decend? [y/n]");
            if (Console.ReadKey(true).KeyChar == 'y')
            {
                Game(Larry);
            }
            else
            {
                Console.WriteLine($"Wise move {Larry.Name}, you may live to see another day.");
            }
        }else if (input == 'r')
        {
            Console.Clear();
            PlayerCharacter Larry = new PlayerCharacter(Race.HalfOrc, Class.Monk, "Larry");

            PotionOfHealing HealthPot = new PotionOfHealing();

            Larry.PickUpItem(HealthPot);

            PotionOfHealing HealthPot2 = new PotionOfHealing();
            Larry.PickUpItem(HealthPot2);

            Game(Larry);
        }
    }

    public static void Game(PlayerCharacter c)
    {
        Dungeon d1 = new Dungeon(c, Console.WindowHeight - 20, Console.WindowWidth - 45, 3);

        d1.PlaceHeroInRoom(c);
        Console.Clear();

        c.RollInitiative();
        Console.Clear();
        d1.PrintMap();

        ConsoleKey key;
        do
        {
            
            d1.PrintCharSheet(c);
            //d.PrintMap();
            key = d1.MonsterTurn();
            
            
            
        } while (key != ConsoleKey.Escape && c.HP > 0);
        if(c.HP <= 0)
        {
            d1.NewMessage("YOU DIED");    
        }
    
    }
    
    






    //// PC creation Methods




    

    public static Race ChooseRace()
	{
		int RaceInt;
		do {
			Console.Clear();
			OutputRaces();
			Console.WriteLine("Select a race: ");
			RaceInt = (int)Char.GetNumericValue(Console.ReadKey(true).KeyChar) - 1;
			
		} while (RaceInt < 0 || RaceInt > 8);

		Race PlayerRace = (Race)RaceInt;
		if (HasSubRace(PlayerRace))
		{
			PlayerRace = ChooseSubRace(PlayerRace);
		}

        return PlayerRace;
	}
    public static void OutputRaces()
    {
        Console.WriteLine(	"1 - Dragonborn\n" +
                            "2 - Dwarf\n" +
                            "3 - Elf\n" +
                            "4 - Gnome\n" +
                            "5 - Half-Elf\n" +
                            "6 - Half-Orc\n" +
                            "7 - Halfling\n" +
                            "8 - Human\n" +
                            "9 - Tiefling\n");

	}

	public static bool HasSubRace(Race r)
	{
		Race[] HasSubRaceArr = new Race[4] {(Race)1, (Race)2, (Race)3, (Race)6};
		return HasSubRaceArr.Contains(r);
	}

    public static Race ChooseSubRace(Race BaseRace)
	{
		int SubRaceInt;
		int[] options;
		
		do {
			options = new int[]{1, 2};
			Console.Clear();
			Console.WriteLine("Select a sub-race:");
			if (BaseRace == (Race)1) //Dwarf
			{
				Console.WriteLine(	"1 - Hill Dwarf\n" +
									"2 - Mountain Dwarf");
			}
			else if (BaseRace == (Race)2)
			{
				options = new int[]{1, 2, 3};
				Console.WriteLine(	"1 - High Elf\n" +
									"2 - Wood Elf\n" +
									"3 - Dark Elf");
			}
			else if (BaseRace == (Race)3)
			{
				Console.WriteLine(	"1 - Forest Gnome\n" +
									"2 - Rock Gnome");
			}
			else if (BaseRace == (Race)6)
			{
				Console.WriteLine(	"1 - Lightfoot Halfling\n" +
									"2 - Stout Halfling");
			}
			Console.WriteLine();
			SubRaceInt = (int)Char.GetNumericValue(Console.ReadKey(true).KeyChar);
		} while (!options.Contains(SubRaceInt)); //exits upon selection of acceptable inputs 
		SubRaceInt += ((int)BaseRace * 10);
		
		Race PlayerRace = (Race)SubRaceInt;

        return PlayerRace;
	}

    public static Class SelectClass()
	{
		char[] options = {'1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c'};
		char input;
		do{
		Console.Clear();
		Console.WriteLine(	"1  - Barbarian\n" +
							"2  - Bard\n" +
							"3  - Cleric\n" +
							"4  - Druid\n" +
							"5  - Fighter\n" +
							"6  - Monk\n" +
							"7  - Paladin\n" +
							"8  - Ranger\n" +
							"9  - Rogue\n" +
							"a - Sorcerer\n" +
							"b - Warlock\n" +
							"c - Wizard\n");
		Console.WriteLine("Select a Class: ");

		input = Console.ReadKey(true).KeyChar;

		}while(!(options.Contains(input)));

		Console.WriteLine();

		Class PlayerClass = (Class)Array.IndexOf(options, input); //Might be my favorite line
        return PlayerClass;
    }

    public static string SetPlayerName()
	{
        Console.Clear();
		Console.WriteLine("What is your name adventurer?");
		string Name = Console.ReadLine();
        return Name;
	}

    public static int[] ChooseStats()
	{
		Console.Clear();
		String[] AttributeNames = {"Strength", "Dexterity", "Constitution", "Intelligence", "Wisdom", "Charisma"};
		int[] attributes = new int[6];
        int[] Rolls = {Dice.DropAndSum(), Dice.DropAndSum(), Dice.DropAndSum(), Dice.DropAndSum(), Dice.DropAndSum(), Dice.DropAndSum()};
		int attsSelected = 0;
		int lastAtts = 1;
		do{
				Array.Sort(Rolls);
				Array.Reverse(Rolls);
				int roll = Rolls[0];
				Console.Write("Your remaining rolls are: \t");
				for (int i = 0; i < Rolls.Length; i++)
				{
					Console.Write($"{(Rolls[i] != 0 ? $"{Rolls[i]}\t" : "")}");
				}
				if (lastAtts == attsSelected)
				// {
				//  roll = Rolls[0];
				// }
				// else 
				{
					Console.WriteLine("Try again.");
				}
				Console.WriteLine($"\nYour next roll is a {roll}.\nSelect an attribute to apply:");
			
				for (int j = 0; j < attributes.Length; j ++){
					if (attributes[j] == 0)
					{
						Console.WriteLine($"[{j+1}]: {AttributeNames[j]}");
					}
					else 
					{
						Console.WriteLine($"[-]: {AttributeNames[j]} - {attributes[j]}");
					}
				}
				int KeyInt = (int)Char.GetNumericValue(Console.ReadKey(true).KeyChar) - 1;
				lastAtts = attsSelected;
				if (KeyInt >= 0 && KeyInt < 6)
				{
					if (attributes[KeyInt] == 0)
					{
						attributes[KeyInt] = roll;
						Rolls[0] = 0;
						attsSelected++;
					}
				}
				Console.Clear();
		} while (attsSelected < 6);

        return attributes;
	}

}