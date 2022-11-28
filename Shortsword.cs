using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class Shortsword : Weapon
{
    public Shortsword ()
    {
        this.DamageType = "piercing";
        this.Name = "Shortsword";
        this.isRanged = false;
        this.WeaponType = new string[] {"shortsword", "martial weapon"};
        this.Sides = 6;
        this.NumDice = 1;
        this.Bonus = 0;
        this.RepresentWith = 's';
    }

    public Shortsword (string name, int s, int d, int b) 
    {
        this.DamageType = "piercing";
        this.isRanged = false;
        this.WeaponType = new string[] {"shortsword", "martial weapon"};
        this.Name = name;
        this.Sides = s;
        this.NumDice = d;
        this.Bonus = b;
        this.RepresentWith = 's';
    }

    
    
}