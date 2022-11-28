using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class Greataxe : Weapon
{
    public Greataxe ()
    {
        this.DamageType = "slashing";
        this.Name = "Greataxe";
        this.isRanged = false;
        this.WeaponType = new string[] {"greataxe", "martial weapon"};
        this.Sides = 12;
        this.NumDice = 1;
        this.Bonus = 0;
    }
    public Greataxe (string name, int s, int d, int b) 
    {
        this.DamageType = "slashing";
        this.Name = name;
        this.isRanged = false;
        this.WeaponType = new string[] {"greataxe", "martial weapon"};
        this.Sides = s;
        this.NumDice = d;
        this.Bonus = b;
    }
}