using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

static class Dice
{
    /* fields */

    private static Random RNG = new Random();

    /* methods */

    /* Simulates a random single die roll */
    public static int Roll(int sides)
    {
      return RNG.Next(sides) + 1;
    }

    /* Simulates random series of dice rolls 
        
        Roll(3,4):
          3D4
          is 3 three four-sided dice summed.      
        
        You can add an OPTIONAL bonus, example: 
        Roll(3,4,5):
          3D4 + 5
          is 3 three four-sided dice summed with 
          an additional 5 added in.           
        
    */
    public static int Roll(int numDice, int sides, int bonus = 0)
    {
        int total = 0;
        for (int die = 0; die < numDice; die++)
        {
            total += Roll(sides);
        }
        return total + bonus;
    }

    public static int DropAndSum(bool lowest = true)
    {
        int total = 0;
        int[] rolls = { Dice.D6(), Dice.D6(), Dice.D6(), Dice.D6() };
        Array.Sort(rolls);

        if (lowest)
        {
            rolls[0] = 0;
        }
        else
        {
            rolls[rolls.Length - 1] = 0;
        }
        
        foreach (int roll in rolls)
        {
            total += roll;
        }
        return total;
    }

    public static int D4()
    {
      return RNG.Next(4) + 1;
    }

    public static int D6()
    {
      return RNG.Next(6) + 1;
    }

    public static int D8()
    {
      return RNG.Next(8) + 1;
    }

    public static int D10()
    {
      return RNG.Next(10) + 1;
    }

    public static int D20()
    {
        return RNG.Next(20) + 1;
    }
}
