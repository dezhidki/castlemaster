using CastleMaster.Players;
using CastleMaster.World;
using Microsoft.Xna.Framework.Audio;
using CastleMaster.Sound;

namespace CastleMaster.Units
{
    public class UnitBuilding : Unit
    {
        protected bool isDestroyed;
        protected AudioEmitter emitter;

        public UnitBuilding(Level level, Player owner)
            : base(level, owner)
        {
            emitter = new AudioEmitter();
            immunityTime = 20;
            isDestroyed = false;
            isSolid = true;
        }

        public bool IsDestroyed { get { return isDestroyed; } }

        public override void Remove()
        {
            if (isDestroyed) return;
            isSelectable = false;
            isDestroyed = true;
            Audio.PlaySound3D(emitter, screenPos, "explosion");
        }
    }
}
