using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Charybdis.Library.Core
{
    public class Pathing2<T>
    {
        public Array2<T> World;

        public bool AllowDiagonalMovement = false;

        public Path2 FindPath(Vec2 from, Vec2 to)
        {
            Vec2 difference = to - from;
            Path2 result = new Path2();
            bool xPositive = difference.Xi > 0;
            bool xStagnant = difference.Xi == 0;
            bool xNegative = difference.Xi < 0;
            bool yPositive = difference.Yi > 0;
            bool yStagnant = difference.Yi == 0;
            bool yNegative = difference.Yi < 0;
            Vec2 virtualPosition = from;
            while (virtualPosition != to) //Does the virtual position match the goal yet? No? Then run this..
            {
                Vec2 goalDirection = (to - virtualPosition).Normalize(); //Get a unit vector toward the goal.
                if (AllowDiagonalMovement) //If we can do diagonal movement..
                {
                    Vec2 potentialPosition = virtualPosition + goalDirection; //Make a diagonal move.
                    if (PassabilityCheck(World.Get(potentialPosition.Xi, potentialPosition.Yi))) //If it's a valid position..
                    {
                        result.Add(potentialPosition); //Add it to the path.
                        virtualPosition = potentialPosition; //Apply potential changes to the virtual position.
                    }
                }
                else //If we can't use diagonal movement..
                {
                    if (virtualPosition.Yi != to.Yi) //If the Y coordinate doesn't match the goal yet..
                    {
                        Vec2 potentialPosition = virtualPosition + new Vec2(0, goalDirection.Yi); //Move toward it on the Y axis.
                        if (PassabilityCheck(World.Get(potentialPosition.Xi, potentialPosition.Yi))) //If it's a valid position..
                        {
                            result.Add(potentialPosition); //Add it to the path.
                            virtualPosition = potentialPosition; //Apply potential changes to the virtual position.
                        }

                    }
                    if (virtualPosition.Xi != to.Xi) //If the X coordinate doesn't match the goal yet..
                    {
                        Vec2 potentialPosition = virtualPosition + new Vec2(goalDirection.Xi, 0); //Move toward it on the X axis.
                        if (PassabilityCheck(World.Get(potentialPosition.Xi, potentialPosition.Yi))) //If it's a valid position..
                        {
                            result.Add(virtualPosition); //Add it to the path.
                            virtualPosition = potentialPosition; //Apply potential changes to the virtual position.
                        }
                    }
                }
            }
            return result;
        }

        public Func<T, bool> PassabilityCheck;
    }

    public class Path2 : List<Vec2>
    {

    }
}
