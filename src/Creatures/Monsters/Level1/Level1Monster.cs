using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

abstract class Level1Monster : Creature {

    
    public Level1Monster() : base()
    {
        this.GoldPouch = Dice.D6();
        this.Inventory = new Container<Item>(2);
        this.XPGiven = 3;
    }
}