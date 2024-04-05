using System;
using Avalonia.Input;
using Digger.Architecture;

namespace Digger
{
    public class Terrain : ICreature
    {
        public CreatureCommand Act(int x, int y)
        {
            return new CreatureCommand() { DeltaX = 0, DeltaY = 0 };
        }

        public bool DeadInConflict(ICreature conflictedObject)
        {
            return true;
        }

        public int GetDrawingPriority()
        {
            return 5;
        }

        public string GetImageFileName()
        {
            return "Terrain.png";
        }
    }

    public class Player : ICreature
    {
        public CreatureCommand Act(int x, int y)
        {
            switch (Game.KeyPressed)
                {
                    case Key.Up:
                        if (y - 1 >= 0 && Game.Map[x, y - 1] is not Sack)
                        {
                            return new CreatureCommand() { DeltaY = -1};
                        }
                        break;
                    case Key.Down:
                        if (y + 1 < Game.MapHeight && Game.Map[x, y + 1] is not Sack)
                        {
                            return new CreatureCommand() { DeltaY = 1 };
                        }
                        break;
                    case Key.Left:
                        if (x - 1 >= 0 && Game.Map[x -1, y] is not Sack)
                        {
                            return new CreatureCommand() { DeltaX = -1 };
                        }
                        break;
                    case Key.Right:
                        if (x + 1 < Game.MapWidth && Game.Map[x + 1, y] is not Sack)
                        {
                            return new CreatureCommand() { DeltaX = 1 };
                        }
                        break;
                    default:
                    return new CreatureCommand() { DeltaX = 0, DeltaY = 0 };
                }
            
            return new CreatureCommand() { DeltaX = 0, DeltaY = 0 };
        }

        public bool DeadInConflict(ICreature conflictedObject)
        {
            if (conflictedObject is Gold)
            {
                Game.Scores += 10;
                return false;
            }
            if (conflictedObject is Sack)
            {
                return true;
            }
            return false;
        }

        public int GetDrawingPriority()
        {
            return 4;
        }

        public string GetImageFileName()
        {
            return "Digger.png";
        }
    }

    public class Sack : ICreature
    {
        public int FallingDistance = 0;

        public CreatureCommand Act(int x, int y)
        {
            if (y + 1 < Game.MapHeight && Game.Map[x, y+1] == null)
            {
                FallingDistance += 1;
                return new CreatureCommand() { DeltaY = 1 };
            }
            else if (y + 1 < Game.MapHeight && Game.Map[x, y + 1] is Player && FallingDistance > 0)
            {
                return new CreatureCommand() { DeltaY = 1 };
            }
            else if ((y == Game.MapHeight - 1 && FallingDistance > 1) || (y + 1 < Game.MapHeight && (Game.Map[x, y + 1] is not Player && Game.Map[x, y + 1] is not null) && FallingDistance > 1))
            {
                return new CreatureCommand() { TransformTo = new Gold() };
            }
            else
            {
                return new CreatureCommand() { DeltaX = 0, DeltaY = 0 };
            }
        }

        public bool DeadInConflict(ICreature conflictedObject)
        {
            return false;
        }

        public int GetDrawingPriority()
        {
            return 1;
        }

        public string GetImageFileName()
        {
            return "Sack.png";
        }
    }

    public class Gold : ICreature
    {
        public CreatureCommand Act(int x, int y)
        {
            return new CreatureCommand() { DeltaX = 0, DeltaY = 0 };
        }

        public bool DeadInConflict(ICreature conflictedObject)
        {
            if (conflictedObject is Player)
            {
                return true;
            }
            return false;
        }

        public int GetDrawingPriority()
        {
            return 5;
        }

        public string GetImageFileName()
        {
            return "Gold.png";
        }
    }
}