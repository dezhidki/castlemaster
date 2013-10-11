using CastleMaster.Units.Mobs;
using CastleMaster.World;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace IsometricEngineTest.Ai
{
    public class AStar
    {
        public const int MOVE_STRAIGHT = 10;
        public const int MOVE_VERTICAL = 14;
        public const int MOVE_COSTS_SAVED = MOVE_VERTICAL - 2 * MOVE_STRAIGHT;
        private readonly double TIRE_FIX;

        private Level level;
        private Mob mob;
        private List<Node> open, path;
        private bool[] tiles;
        private Node[] nodeMap;
        private bool isPathFinding = false;
        private bool canPathFind = true;
        private int calls;
        private int xStart, zStart, xEnd, zEnd;

#if DEBUG
        private Stopwatch timer;
#endif

        public AStar(Level level, Mob mob)
        {
            this.level = level;
            this.mob = mob;
            open = new List<Node>();
            path = new List<Node>();

            TIRE_FIX = (1.0 + (MOVE_STRAIGHT / 1000.0));

#if DEBUG
            timer = new Stopwatch();
#endif
        }

        public List<Node> Path { get { return path; } }

        public bool IsPathFinding { get { return isPathFinding; } }

        public bool CanFindPath { get { return canPathFind; } }

        public int EstimateCost(int x, int z)
        {
            int dx = Math.Abs(x - xEnd);
            int dz = Math.Abs(z - zEnd);

            int h = MOVE_STRAIGHT * (dx + dz) + MOVE_COSTS_SAVED * Math.Min(dx, dz);
            h = (int)(h * TIRE_FIX);
            return h;
        }

        public void InitializePathFinder(int xStart, int zStart, int xEnd, int zEnd, bool ignoreSolidnessForEnd)
        {
            tiles = level.BuildSolidnessTable(mob, ignoreSolidnessForEnd);
            nodeMap = new Node[level.Width * level.Height];
            open.Clear();
            path.Clear();
            calls = 0;

            if (ignoreSolidnessForEnd)
                tiles[xEnd + zEnd * level.Width] = false;

#if DEBUG
            timer.Reset();
            Console.WriteLine("[START] From: [ " + xStart + ", " + zStart + " ] to : [ " + xEnd + ", " + zEnd + "].");
#endif

            this.xStart = xStart;
            this.xEnd = xEnd;
            this.zStart = zStart;
            this.zEnd = zEnd;

            Node startNode = new Node(xStart, zStart, null, 0, EstimateCost(xStart, zStart), true, false);
            open.Add(startNode);
            nodeMap[xStart + zStart * level.Width] = startNode;

            isPathFinding = true;
            canPathFind = true;

            if ((xStart == xEnd && zStart == zEnd))
                isPathFinding = false;

            if (xEnd < 0 || zEnd < 0 || xEnd >= level.Width || zEnd >= level.Height)
            {
#if DEBUG
                Console.WriteLine("[FAIL] End is out of level boundaries.");
#endif
                canPathFind = false;
            }
            else if (tiles[xEnd + zEnd * level.Width])
            {
#if DEBUG
                Console.WriteLine("[FAIL] End is solid.");
#endif
                canPathFind = false;
            }
        }

        public void ApplySolidnessToPos(List<int> posList, bool ignoreEndSolidness)
        {
            foreach (int i in posList)
                tiles[i] = false;

            if(ignoreEndSolidness)
                tiles[xEnd + zEnd * level.Width] = true;
        }

        public void Reset()
        {
            Path.Clear();
            isPathFinding = false;
            canPathFind = true;
        }

        public void FindPath(int maxVisits)
        {
#if DEBUG
            TimeSpan lastTime = timer.Elapsed;
            timer.Start();
#endif
            calls++;

            Node current = null;
            int visits = 0;
            do
            {
                visits++;
                if (visits == maxVisits)
                {
#if DEBUG
                    timer.Stop();
                    Console.WriteLine("[PAUSE] Calls so far: " + calls + ". Time spent: " + ((timer.Elapsed - lastTime).TotalMilliseconds) + " ms.");
#endif
                    return;
                }

                current = GetBestOpenNode();
                if (current == null)
                {
#if DEBUG
                    timer.Stop();
                    Console.WriteLine("[FAIL] No more open nodes! Calls: " + calls + ". Total time: " + timer.Elapsed.TotalMilliseconds + " ms.");
#endif
                    canPathFind = false;
                    return;
                }
                current.IsClosed = true;
                current.IsOpen = false;
                open.Remove(current);

                for (int x = current.X - 1; x <= current.X + 1; x++)
                {
                    for (int z = current.Z - 1; z <= current.Z + 1; z++)
                    {
                        if (x == current.X && z == current.Z) continue;
                        if (x < 0 || z < 0 || x >= level.Width || z >= level.Height) continue;
                        if (tiles[x + z * level.Width]) continue;
                        if (tiles[x + current.Z * level.Width] && tiles[current.X + z * level.Width]) continue;

                        int moveCost = (x == current.X || z == current.Z) ? MOVE_STRAIGHT : MOVE_VERTICAL;
                        int travelCost = current.TravelCost + moveCost;

                        Node neighbour = nodeMap[x + z * level.Width];

                        if (neighbour != null)
                        {
                            if (neighbour.IsOpen && travelCost < neighbour.TravelCost)
                            {
                                neighbour.IsOpen = false;
                                open.Remove(neighbour);
                            }
                            if (neighbour.IsClosed && travelCost < neighbour.TravelCost)
                                neighbour.IsClosed = false;
                        }
                        if (neighbour == null || (!neighbour.IsClosed && !neighbour.IsOpen))
                        {
                            neighbour = new Node(x, z, current, travelCost, EstimateCost(x, z), true, false);
                            open.Add(neighbour);
                            nodeMap[x + z * level.Width] = neighbour;
                        }
                    }
                }
            } while (current.X != xEnd || current.Z != zEnd);

            isPathFinding = false;

#if DEBUG
            timer.Stop();
            Console.WriteLine("[FINISH] Calls: " + calls + ". Total time: " + timer.Elapsed.TotalMilliseconds + " ms.");
#endif

            path.Add(new Node(xEnd, zEnd, current, 0, 0, false, false));
            Node reverse = current.Parent;
            while (reverse.X != xStart || reverse.Z != zStart)
            {
                path.Add(reverse);
                reverse = reverse.Parent;
            }

            path.Reverse();
        }

        private Node GetBestOpenNode()
        {
            Node result = null;

            if (open.Count == 1)
            {
                result = open[0];
                return result;
            }

            foreach (Node node in open)
            {
                if (result == null || node.TotalCost < result.TotalCost)
                    result = node;
            }

            return result;
        }


    }
}
