
namespace IsometricEngineTest.Ai
{
    public class Node
    {
        private int x, z;
        private Node parent;
        private int travelCost, estimatedCost, totalCost;
        private bool isOpen, isClosed;

        public Node(int x, int z, Node parent, int travelCost, int estimatedCost, bool isOpen, bool isClosed)
        {
            this.x = x;
            this.z = z;
            this.parent = parent;
            this.travelCost = travelCost;
            this.isOpen = isOpen;
            this.isClosed = isClosed;
            this.estimatedCost = estimatedCost;

            totalCost = travelCost + estimatedCost;
        }

        public int X { get { return x; } }

        public int Z { get { return z; } }

        public Node Parent { get { return parent; } }

        public int TotalCost { get { return totalCost; } }

        public int TravelCost { get { return travelCost; } }

        public bool IsOpen
        {
            set { isOpen = value; }
            get { return isOpen; }
        }

        public bool IsClosed
        {
            set { isClosed = value; }
            get { return isClosed; }
        }
    }
}
