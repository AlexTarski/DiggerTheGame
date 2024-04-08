using System;
using Avalonia.Input;
using Digger.Architecture;

namespace Digger
{
    public static class SeekAndDestroy
    {
        public static void DiggerIsAlive(ICreature[,] map, out int aX, out int aY, out bool diggerAlive)
        {
            int h = map.GetLength(0);
            int w = map.GetLength(1);
            for (int n = 0; n < h; n++)
            {
                for (int i = 0; i < w; i++)
                {
                    if (map[n,i] is Player)
                    {
                        diggerAlive = true;
                        aX = n;
                        aY = i;
                        return;
                    }
                }
            }

            diggerAlive = false;
            aX = 0; aY = 0;
        }

        public static double DistanceToDigger(int aX, int aY, int bX, int bY)
        {
            return Math.Sqrt(((aX - bX) * (aX - bX)) + ((aY - bY) * (aY - bY)));
        }
    }
    
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
            if (conflictedObject is Sack || conflictedObject is Monster)
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
        public int fallingDistance = 0;

        public CreatureCommand Act(int x, int y)
        {
            if (y + 1 < Game.MapHeight && (Game.Map[x, y + 1] == null || ((Game.Map[x, y + 1] is Player || Game.Map[x, y + 1] is Monster) && fallingDistance > 0)))
            {
                fallingDistance += 1;
                return new CreatureCommand() { DeltaY = 1 };
            }
            else if ((y == Game.MapHeight - 1 && fallingDistance > 1) || (y + 1 < Game.MapHeight && (Game.Map[x, y + 1] is not Player && Game.Map[x, y + 1] is not null && Game.Map[x, y + 1] is not Monster) && fallingDistance > 1))
            {
                fallingDistance = 0;
                return new CreatureCommand() { TransformTo = new Gold() };
            }
            else
            {
                fallingDistance = 0;
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
            if (conflictedObject is Player || conflictedObject is Monster)
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

    public class Monster : ICreature
    {
        public CreatureCommand Act(int x, int y)
        {
                SeekAndDestroy.DiggerIsAlive(Game.Map, out int aX, out int aY, out bool diggerAlive);
                
                if (diggerAlive)
                {
                    if ((y - 1 >= 0 && Game.Map[x, y - 1] is Player) || (y - 1 >= 0 && Game.Map[x, y - 1] is not Sack
                        && Game.Map[x, y - 1] is not Terrain
                        && Game.Map[x, y - 1] is not Monster
                        && SeekAndDestroy.DistanceToDigger(aX, aY, x, y - 1) < SeekAndDestroy.DistanceToDigger(aX, aY, x, y)))
                    {
                        return new CreatureCommand() { DeltaY = -1 };
                    }
                    if ((y + 1 < Game.MapHeight && Game.Map[x, y + 1] is Player) || (y + 1 < Game.MapHeight && Game.Map[x, y + 1] is not Sack
                        && Game.Map[x, y + 1] is not Terrain
                        && Game.Map[x, y + 1] is not Monster
                        && SeekAndDestroy.DistanceToDigger(aX, aY, x, y + 1) < SeekAndDestroy.DistanceToDigger(aX, aY, x, y)))
                    {
                        return new CreatureCommand() { DeltaY = 1 };
                    }
                    if ((x - 1 >= 0 && Game.Map[x - 1, y] is Player) || (x - 1 >= 0 && Game.Map[x - 1, y] is not Sack
                        && Game.Map[x - 1, y] is not Terrain
                        && Game.Map[x - 1, y] is not Monster
                        && SeekAndDestroy.DistanceToDigger(aX, aY, x - 1, y) < SeekAndDestroy.DistanceToDigger(aX, aY, x, y)))
                    {
                        return new CreatureCommand() { DeltaX = -1 };
                    }
                    if ((x + 1 < Game.MapWidth && Game.Map[x + 1, y] is Player) || (x + 1 < Game.MapWidth && Game.Map[x + 1, y] is not Sack
                        && Game.Map[x + 1, y] is not Terrain
                        && Game.Map[x + 1, y] is not Monster
                        && SeekAndDestroy.DistanceToDigger(aX, aY, x + 1, y) < SeekAndDestroy.DistanceToDigger(aX, aY, x, y)))
                    {
                        return new CreatureCommand() { DeltaX = 1 };
                    }

                    return new CreatureCommand() { DeltaX = 0, DeltaY = 0 };
                }
                else
                {
                    return new CreatureCommand() { DeltaX = 0, DeltaY = 0 };
                }
        }

        public bool DeadInConflict(ICreature conflictedObject)
        {
            if (conflictedObject is Sack or Monster)
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
            return "Monster.png";
        }
    }
}