
namespace CastleMaster.Players
{
    public struct Team
    {
        public string Name;
        public int ID;
        public int SpriteSheetWoodcutter, SpriteSheetWarrior, SpriteSheetRanger;

        public Team(string name, int id, int spriteSheetWoodcutter, int spriteSheetWarrior, int spriteSheetRanger)
        {
            Name = name;
            ID = id;
            SpriteSheetWoodcutter = spriteSheetWoodcutter;
            SpriteSheetWarrior = spriteSheetWarrior;
            SpriteSheetRanger = spriteSheetRanger;
        }

        public static bool operator !=(Team t1, Team t2)
        {
            return t1.ID != t2.ID;
        }

        public static bool operator ==(Team t1, Team t2)
        {
            return t1.ID == t2.ID;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Team))
                return base.Equals(obj);
            return ID == ((Team)obj).ID;
        }

        public override int GetHashCode()
        {
            int id = 13;
            id += Name.GetHashCode() * 13;
            id += ID * 13;
            return id;
        }
    }
}
