using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
  
interface IMobile: ILocatable
{
    void GoTo(int x, int y);

    void MoveNorth(int distance);
    void MoveEast(int distance);
    void MoveSouth(int distance);
    void MoveWest(int distance);

    void MoveNorthEast(int distance);
    void MoveSouthEast(int distance);
    void MoveSouthWest(int distance);
    void MoveNorthWest(int distance);

    void Face(Direction dir);
    void MoveForward(int distance);
    void Turn(int degrees);

}
