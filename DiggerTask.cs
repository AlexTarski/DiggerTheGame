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
                        if (y - 1 >= 0)
                        {
                            return new CreatureCommand() { DeltaY = -1};
                        }
                        break;
                    case Key.Down:
                        if (y + 1 < Game.MapHeight)
                        {
                            return new CreatureCommand() { DeltaY = 1 };
                        }
                        break;
                    case Key.Left:
                        if (x - 1 >= 0)
                        {
                            return new CreatureCommand() { DeltaX = -1 };
                        }
                        break;
                    case Key.Right:
                        if (x + 1 < Game.MapWidth)
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
}