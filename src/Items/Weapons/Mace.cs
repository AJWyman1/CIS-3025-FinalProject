using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class Mace : Weapon
{
    public Mace ()
    {
        this.DamageType = "bludgeoning";
        this.Name = "Mace";
        this.isRanged = false;
        this.WeaponType = new string[] {"mace", "simple weapon"};
        this.Sides = 6;
        this.NumDice = 1;
        this.Bonus = 0;
        this.RepresentWith = 'm';
        this.Color = ConsoleColor.DarkYellow;
    }

    public Mace (string name, int s, int d, int b) 
    {
        this.DamageType = "bludgeoning";
        this.isRanged = false;
        this.WeaponType = new string[] {"mace", "simple weapon"};
        this.Name = name;
        this.Sides = s;
        this.NumDice = d;
        this.Bonus = b;
        this.RepresentWith = 'm';
        this.Color = ConsoleColor.DarkYellow;
    }

    
    
}