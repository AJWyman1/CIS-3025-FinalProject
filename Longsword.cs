using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class Longsword : Weapon
{
    public Longsword ()
    {
        this.DamageType = "slashing";
        this.Name = "Longsword";
        this.isRanged = false;
        this.WeaponType = new string[] {"longsword", "martial weapon"};
        this.Sides = 8;
        this.NumDice = 1;
        this.Bonus = 0;
        this.RepresentWith = 'l';
    }

    public Longsword (string name, int s, int d, int b) 
    {
        this.DamageType = "slashing";
        this.isRanged = false;
        this.WeaponType = new string[] {"longsword", "martial weapon"};

        this.Name = name;
        this.Sides = s;
        this.NumDice = d;
        this.Bonus = b;
        this.RepresentWith = 'l';
    }
    
}