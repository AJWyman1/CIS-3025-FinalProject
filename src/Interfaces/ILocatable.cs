using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

interface ILocatable
{
    int X { get; set; }
    int Y { get; set; }
    ConsoleColor Color {get; protected set;}
    Direction Facing { get; set; }
    char RepresentWith { get; protected set; } //Char representation for print out in Console

}
