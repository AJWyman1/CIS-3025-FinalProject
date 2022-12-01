using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

abstract class Level3Monster : Creature {

    public int XP { get; protected set; }
    public Level3Monster()
    {
        this.GoldPouch = Dice.Roll(3, 20);
        this.Inventory = new Container<Item>(5);
        this.XP = 300;
    }
}