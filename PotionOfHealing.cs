using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class PotionOfHealing : Potion
{
    private int Sides;
    private int NumDice;
    private int Bonus;

    public PotionOfHealing()
    {
        this.UseChance = 1.0f;
        this.UsesLeft = 1;
        this.Name = "Potion of Healing";
        this.Sides = 4;
        this.NumDice = 2;
        this.Bonus = 2;
        this.RepresentWith = 'p';
        this.Color = ConsoleColor.Magenta;
    }

    public PotionOfHealing(string name, int s, int d, int b) //Health pot customization
    {
        this.UseChance = 1.0f;
        this.UsesLeft = 1;
        this.Name = name;
        this.Sides = s;
        this.NumDice = d;
        this.Bonus = b;
    }

    public override string SuccessMessage()
    {
        int HealthGained = this.Heal();
        return $"Success, you healed {HealthGained} Hit Points";
    }
    public override string FailureMessage()
    {
        return "Failure to heal";
    }

    public int Heal()
    {
        return Dice.Roll(this.NumDice, this.Sides, this.Bonus);
    }
    public override string Use(Creature c, int Slot)
    {
        if (this.UsesLeft > 0)
        {
            this.UsesLeft -= 1;
            //this.Name = "Empty " + this.Name;

            
            int healed = this.Heal();
            c.Heal(healed);
            
            return $"{c.Name} drank the {this.Name} and regained {healed} HP!";
        }
        return "This Potion is empty";
    }
}