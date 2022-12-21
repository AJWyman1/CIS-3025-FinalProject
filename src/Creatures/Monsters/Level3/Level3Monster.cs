using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

abstract class Level3Monster : Creature {

    public Level3Monster()
    {
        this.GoldPouch = Dice.Roll(3, 20);
        this.Inventory = new Container<Item>(5);
        this.XPGiven = 50;
    }
}