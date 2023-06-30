using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class Rapier : Weapon
{
    public Rapier ()
    {
        this.DamageType = "piercing";
        this.Name = "Rapier";
        this.isRanged = false;
        this.WeaponType = new string[] {"rapier", "martial weapon"};
        this.Sides = 8;
        this.NumDice = 1;
        this.Bonus = 0;
        this.RepresentWith = 'r';
        this.Color = ConsoleColor.DarkYellow;
    }

    public Rapier (string name, int s, int d, int b) 
    {
        this.DamageType = "piercing";
        this.isRanged = false;
        this.WeaponType = new string[] {"rapier", "martial weapon"};
        this.Name = name;
        this.Sides = s;
        this.NumDice = d;
        this.Bonus = b;
        this.RepresentWith = 'r';
        this.Color = ConsoleColor.DarkYellow;
    }

    
    
}