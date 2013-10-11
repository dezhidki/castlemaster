using CastleMaster.Graphics;
using CastleMaster.Physics;
using CastleMaster.Players;
using CastleMaster.Units;
using CastleMaster.World;
using Microsoft.Xna.Framework;
using System;
using Microsoft.Xna.Framework.Audio;
using CastleMaster.Sound;

namespace CastleMaster.Entities
{
    public class EntityArrow : Entity
    {
        private float moveDir, pushPower;
        private int dirID, damage;
        private Team enemyTeam, attackerTeam;
        private Unit target, archer;
        private AudioEmitter emmitter;

        public EntityArrow(Level level, int damage, float pushPower, Unit target, Unit archer, Team attackerTeam)
            : base(level)
        {
            emmitter = new AudioEmitter();
            width = 3.0F;
            depth = 1.0F;
            this.damage = damage;
            this.pushPower = pushPower;
            this.target = target;
            this.archer = archer;
            this.attackerTeam = attackerTeam;
            enemyTeam = Game.GetEnemyTeam(attackerTeam);
            moveSpeed = 2.0F;

            renderOffset.Y = 16;
            rectOffset.Update(-1.0F, -0.5F, 1.0F, 0.5F);
            isSolid = true;
        }

        public override bool IsSolidTo(Entity ent)
        {
            Unit u = ent as Unit;
            if (u != null)
                return u.Owner.Team.ID == enemyTeam.ID;
            return base.IsSolidTo(ent);
        }

        public override void Init()
        {
            base.Init();

            moveDir = (float)(Math.Atan2(target.Z - Z, target.X - X));
            dirID = (int)(Math.Floor(moveDir * 8 / MathHelper.TwoPi + 0.5F)) & 7;
        }

        public override void OnCollidedWith(BoundingRectangleOwner colliding)
        {
            Unit u = colliding as Unit;
            if (u != null && u.CanBeHurtBy(archer))
            {
                u.Damage(archer, damage, moveDir, pushPower);
                Audio.PlaySound3D(emmitter, screenPos, "hurt");
            }
            Remove();
        }

        public override void OnTriedExitLevel()
        {
            Remove();
        }

        public override void Update()
        {
            base.Update();

            Move((float)Math.Cos(moveDir) * moveSpeed, (float)Math.Sin(moveDir) * moveSpeed);
        }

        public override void Render(RenderHelper renderer)
        {
            renderer.Render(ScreenPos, 6, dirID, attackerTeam.SpriteSheetRanger, Viewport.ZOOM);
        }
    }
}
