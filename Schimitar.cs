using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class Schimitar : Weapon
{
    public Schimitar ()
    {
        this.DamageType = "slashing";
        this.Name = "Schimitar";
        this.isRanged = false;
        this.WeaponType = new string[] {"schimitar", "martial weapon"};
        this.Sides = 6;
        this.NumDice = 1;
        this.Bonus = 0;
        this.RepresentWith = 'c';
    }

    public Schimitar (string name, int s, int d, int b) 
    {
        this.DamageType = "slashing";
        this.isRanged = false;
        this.WeaponType = new string[] {"schimitar", "martial weapon"};
        this.Name = name;
        this.Sides = s;
        this.NumDice = d;
        this.Bonus = b;
        this.RepresentWith = 'c';
    }
    
}