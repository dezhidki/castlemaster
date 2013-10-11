using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CastleMaster.World;
using CastleMaster.Input;
using System.Windows.Forms;
using CastleMaster.Graphics;

namespace CastleMaster.Entities
{
    public class EntityTestMob : Entity
    {
        private float walkSpeed = 1.0F;

        public EntityTestMob(Level level)
            : base(level)
        {
            isSolid = true;
            width = 6.0F;
            depth = 6.0F;

            renderOffset.Y = 20;

            rectOffset.Update(-4.0F, -4.0F, 5.0F, 5.0F);
        }

        public override void Update()
        {
            if (InputHandler.IsKeyDown(Keys.W))
                Move(0, -walkSpeed);
            if (InputHandler.IsKeyDown(Keys.S))
                Move(0, walkSpeed);
            if (InputHandler.IsKeyDown(Keys.A))
                Move(-walkSpeed, 0);
            if (InputHandler.IsKeyDown(Keys.D))
                Move(walkSpeed, 0);
        }

        public override void Render(RenderHelper renderer)
        {
            renderer.Render(ScreenPos, 0, 0, Resources.SPRITESHEET_WOODCUTTER, Viewport.ZOOM);
        }

    }
}
