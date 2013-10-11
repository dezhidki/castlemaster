using CastleMaster.Graphics;
using CastleMaster.Physics;
using CastleMaster.World;
using CastleMaster.World.Tiles;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace CastleMaster.Entities
{
    public class Entity : BoundingRectangleOwner
    {
        protected float x, z, width = 16.0F, depth = 16.0F;
        protected Level level;
        private BoundingRectangle boundingRect;
        protected BoundingRectangle rectOffset;
        protected Vector2 renderOffset, screenPos;
        protected bool isSolid = true;
        protected float moveSpeed = 0.0F;

        public Entity(Level level)
        {
            this.level = level;
            renderOffset = Vector2.Zero;
            screenPos = Vector2.Zero;
            Removed = false;
            rectOffset = new BoundingRectangle(-width / 2, -depth / 2, width / 2, depth / 2, null);
            IsRendering = false;
        }

        public bool IsRendering { get; set; }

        public bool Removed { get; protected set; }

        public float X
        {
            get { return x; }
            set { x = value; }
        }

        public float Z
        {
            get { return z; }
            set { z = value; }
        }

        public float Width { get { return width; } }

        public float Depth { get { return depth; } }

        public BoundingRectangle BoundingRectangle { get { return boundingRect; } }

        public Vector2 RenderOffset { get { return renderOffset; } }

        protected Vector2 ScreenPos { get { return screenPos * Viewport.ZOOM; } }

        public virtual void Remove()
        {
            Removed = true;
        }

        public virtual void Init()
        {
            screenPos.X = (x - z) * Viewport.X_SCALE - renderOffset.X;
            screenPos.Y = (x + z) * Viewport.Y_SCALE - renderOffset.Y;

            boundingRect = new BoundingRectangle(x, z, x, z, this).AddSelf(rectOffset);
        }

        public void AddBoundingRect(ref List<BoundingRectangle> list, Entity ent)
        {
            if (ent != this)
                list.Add(BoundingRectangle);
        }

        public virtual bool IsSolidTo(Entity ent)
        {
            return isSolid;
        }

        public bool Blocks(Entity ent)
        {
            return isSolid && ent.isSolid && IsSolidTo(ent) && ent.IsSolidTo(this);
        }

        public virtual void OnTriedExitLevel() { }

        public bool Move(float xd, float zd)
        {
            if (xd == 0.0F && zd == 0.0F) return false;

            bool inLevel = true;

            if (BoundingRectangle.XRight + xd > level.Width * Viewport.TILESIZE) inLevel = false;
            else if (BoundingRectangle.XLeft + xd < 0.0F) inLevel = false;
            else if (BoundingRectangle.ZFar + zd < 0.0F) inLevel = false;
            else if (BoundingRectangle.ZNear + zd > level.Height * Viewport.TILESIZE) inLevel = false;

            if (!inLevel)
            {
                OnTriedExitLevel();
                return false;
            }

            int moveSteps = (int)(Math.Sqrt(xd * xd + zd * zd) + 1);

            bool hasMoved = false;
            for (int i = 0; i < moveSteps; i++)
            {
                hasMoved |= MovePart(xd / moveSteps, 0);
                hasMoved |= MovePart(0, zd / moveSteps);
            }

            if (hasMoved)
            {
                screenPos.X = (x - z) * Viewport.X_SCALE - renderOffset.X;
                screenPos.Y = (x + z) * Viewport.Y_SCALE - renderOffset.Y;
            }

            return hasMoved;
        }

        private bool MovePart(float xd, float zd)
        {
            if (xd != 0 && zd != 0) return false;

            List<BoundingRectangle> collidables = level.GetCollidables(this, BoundingRectangle + new Vector2(xd, zd));

            foreach (BoundingRectangle collidable in collidables)
            {
                collidable.Owner.OnTouchedBy(this);
                OnTouched(collidable.Owner);
            }

            collidables.RemoveAll(br => SkipCollisionCheck(br.Owner));

            if (collidables.Count > 0)
            {
                foreach (BoundingRectangle collidable in collidables)
                {
                    OnCollidedWith(collidable.Owner);
                    collidable.Owner.OnCollidedBy(this);
                }

                return false;
            }

            x += xd;
            z += zd;

            BoundingRectangle.Update(x, z, x, z).AddSelf(rectOffset);

            return true;
        }

        private bool SkipCollisionCheck(BoundingRectangleOwner collidableOwner)
        {
            //if (typeof(Entity).IsAssignableFrom(collidableOwner.GetType()))
            //    return !((Entity)collidableOwner).Blocks(this);
            //if (typeof(Tile).IsAssignableFrom(collidableOwner.GetType()))
            //    return false;
            Entity ent = collidableOwner as Entity;
            if (ent != null)
                return !ent.Blocks(this);
            if (collidableOwner as Tile != null)
                return false;
            return true;
        }

        public float DistanceTo(float x, float z)
        {
            float xd = X - x;
            float zd = Z - z;

            return (float)(Math.Sqrt(xd * xd + zd * zd));
        }

        public float DistanceToSqr(float x, float z)
        {
            float xd = X - x;
            float zd = Z - z;

            return xd * xd + zd * zd;
        }

        public virtual void Update() { }

        public virtual void Render(RenderHelper renderer) { }

        public virtual void OnTouched(BoundingRectangleOwner touching) { }

        public virtual void OnTouchedBy(BoundingRectangleOwner toucher) { }

        public virtual void OnCollidedWith(BoundingRectangleOwner colliding) { }

        public virtual void OnCollidedBy(BoundingRectangleOwner collidable) { }

        public virtual void OnRemoved() { }

    }
}
