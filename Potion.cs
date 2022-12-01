using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

abstract class Potion : Item
{
    
    public Potion ()
    {

    }

    public override string Use(Creature c, int Slot)
    {
        return this.SuccessMessage();
    }
}