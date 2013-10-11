using CastleMaster.Graphics;
using CastleMaster.Guis;
using CastleMaster.Players;
using CastleMaster.World;

namespace CastleMaster.Units
{
    public class UnitKing : UnitBuilding
    {
        private int timesHit = 0;
        
        public UnitKing(Level level, Player owner)
            : base(level, owner)
        {
            width = 5 * Viewport.TILESIZE;
            depth = 5 * Viewport.TILESIZE;
            HasHealth = true;
            maxHealth = 400;

            renderOffset.X = 64;
            renderOffset.Y = 80;

            spriteSize.X = 160;
            spriteSize.Y = 128;
            screenRectOffset.Update(0, 0, spriteSize.X, spriteSize.Y);

            highlightOffset.X = spriteSize.X / 2 - 4;

            rectOffset.Update(-2 * Viewport.TILESIZE, -2 * Viewport.TILESIZE, 2 * Viewport.TILESIZE, 2 * Viewport.TILESIZE);
        }

        public int TimesHit { get { return timesHit; } }

        public override void Damage(Unit attacker, int damage, float dir, float pushPower)
        {
            base.Damage(attacker, damage, dir, pushPower);
            timesHit++;
        }

        public override void Remove()
        {
            base.Remove();
            level.RemoveUnit(this);

            GuiWinLooseMessage winLooseMsg = new GuiWinLooseMessage(Game.GuiManager, Game.Instance, Game.GetEnemyTeam(Owner.Team));
            Game.GuiManager.AddGui(winLooseMsg, true, true);
            Game.Instance.IsGamePaused = true;
        }

        public override void Render(RenderHelper renderer)
        {
            renderer.Render(ScreenPos, 0, 0, Resources.SPRITE_KING, colorizer, Viewport.ZOOM);
            base.Render(renderer);
        }
    }
}
