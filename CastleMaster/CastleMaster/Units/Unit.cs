using CastleMaster.Entities;
using CastleMaster.Graphics;
using CastleMaster.Physics;
using CastleMaster.Players;
using CastleMaster.World;
using Microsoft.Xna.Framework;
using System;

namespace CastleMaster.Units
{
    public class Unit : Entity
    {
        protected bool isSelectable;
        protected BoundingRectangle screenRect;
        protected BoundingRectangle screenRectOffset;
        protected Vector2 highlightOffset = Vector2.Zero;
        protected Point spriteSize;
        private float timer = -MathHelper.TwoPi, arrowSpeed = 0.1F;
        private Vector2 arrowOffs = Vector2.Zero;
        private Player owner;
        private int immunityTimer = 0;
        protected int maxHealth, immunityTime;
        protected Color colorizer = Color.White;
        protected bool colorizeOnHit, wasHurt = false;

        public Unit(Level level, Player owner)
            : base(level)
        {
            this.owner = owner;

            spriteSize = new Point(32, 32);
            screenRectOffset = new BoundingRectangle(0, 0, spriteSize.X, spriteSize.Y, null);
            isSelectable = true;
            IsSelected = false;
            HasHealth = false;
            maxHealth = 10;
            colorizeOnHit = true;
            immunityTime = 0;
        }

        public int MaxHealth { get { return maxHealth; } }

        public int Health { get; protected set; }

        public bool HasHealth { get; protected set; }

        public bool IsSelectable { get { return isSelectable; } }

        public Player Owner { get { return owner; } }

        public bool IsSelected { get; set; }

        public override void Init()
        {
            base.Init();
            Health = maxHealth;
        }

        public virtual void OnFocus() { }

        public virtual void OnFocusLost() { }

        public virtual bool CanBeHurtBy(Unit u)
        {
            return HasHealth && u.Owner != Owner;
        }

        public void Hit(Unit u, int damage, float dir, float pushPower)
        {
            if (u.CanBeHurtBy(this))
                u.Damage(this, damage, dir, pushPower);
        }

        public virtual void Damage(Unit attacker, int damage, float dir, float pushPower)
        {
            Health -= damage;
            if (colorizeOnHit)
                colorizer = Color.Red;
            if (immunityTime > 0)
            {
                wasHurt = true;
                HasHealth = false;
            }
            if (Health <= 0)
                Remove();
        }

        public virtual void OnFunctionClick(float x, float z, int sameOrderAmount, bool wasCalledBefore) { }

        public override void Update()
        {
            base.Update();

            if (IsSelected)
            {
                timer += arrowSpeed;
                if (timer >= MathHelper.TwoPi) timer = -MathHelper.TwoPi;
                arrowOffs.Y = (float)Math.Sin(timer) * 2.0F;
            }

            if (wasHurt)
            {
                immunityTimer++;
                if (immunityTimer >= immunityTime)
                {
                    wasHurt = false;
                    immunityTimer = 0;
                    colorizer = Color.White;
                    HasHealth = true;
                }
            }
        }

        public override void OnRemoved()
        {
            level.RemoveUnit(this);
        }

        public bool IntersectsWithScreenSpace(float x0, float y0, float x1, float y1)
        {
            if (!isSelectable) return false;
            screenRect = new BoundingRectangle(screenPos.X, screenPos.Y, screenPos.X, screenPos.Y, this).AddSelf(screenRectOffset).Scale(Viewport.ZOOM);
            return screenRect.Intersects(x0, y0, x1, y1);
        }

        public float DistanceFromScreenSpaceSqr(float x, float y)
        {
            float dx = screenPos.X * Viewport.ZOOM - x;
            float dy = screenPos.Y * Viewport.ZOOM - y;

            return dx * dx + dy * dy;
        }

        public virtual void RenderHighLight(RenderHelper renderer)
        {
            renderer.Render((screenPos + highlightOffset + arrowOffs) * Viewport.ZOOM, 1, 0, Resources.SPRITESHEET_ICONS, Viewport.ZOOM);
        }

        public override void Render(RenderHelper renderer)
        {
            if (IsSelected)
                RenderHighLight(renderer);
        }
    }
}
