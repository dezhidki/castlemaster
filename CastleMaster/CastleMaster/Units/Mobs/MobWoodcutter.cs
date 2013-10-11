using CastleMaster.Entities.TileEntities;
using CastleMaster.Graphics;
using CastleMaster.Players;
using CastleMaster.Sound;
using CastleMaster.World;
using IsometricEngineTest.Ai.Orders;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace CastleMaster.Units.Mobs
{
    public class MobWoodcutter : Mob
    {
        private enum OrderType { CHOP, MOVE, AVOID, NONE }

        private TileEntityTree currentTarget;
        private bool isChopping = false;
        private TimerHelper choppingAnimation;
        private int currentSpriteX;
        private OrderType currentOrderType = OrderType.NONE;
        private int lastSameOrderAmount = 0;

        public MobWoodcutter(Level level, Player owner)
            : base(level, owner)
        {
            HasHealth = true;
            maxHealth = 20;

            spriteSize.X = 17;
            spriteSize.Y = 20;
            screenRectOffset.Update(8, 12, 8 + spriteSize.X, 12 + spriteSize.Y);
            renderOffset.Y = 20;

            highlightOffset.X = 11;

            rectOffset.Update(-4.0F, -4.0F, 5.0F, 5.0F);
            isSolid = true;

            moveSpeed = 2.0F;
            choppingAnimation = new TimerHelper(10, 5, false, 3);
            choppingAnimation.RoundEnded += delegate()
            {
                Audio.PlaySound3D(emitter, screenPos, "wood");
                owner.LumberAmount++;
                currentTarget.AvaliableLogs--;
                if (currentTarget.AvaliableLogs <= 0)
                {
                    currentTarget.Remove();
                    currentTarget = null;
                    OrderChop(level.GetNearestEntity<TileEntityTree>(this, 5), 1, false);
                }
            };
        }

        public override int TypeID
        {
            get { return 0; }
        }

        public override void OnRemoved()
        {
            base.OnRemoved();
            Audio.PlaySound3D(emitter, screenPos, "death2");
        }

        public void OrderChop(TileEntityTree te, int sameAmount, bool wasCalledBefore)
        {
            if (te == null)
                return;
            currentOrderType = OrderType.CHOP;
            currentTarget = te;
            SetOrder(new OrderMove(currentTarget.X, currentTarget.Z, 20.0F, sameAmount * 4.0F, true, !wasCalledBefore));
        }

        private void StartChopping()
        {
            choppingAnimation.Start();
            isChopping = true;
        }

        private void StopChopping()
        {
            isChopping = false;
            currentOrderType = OrderType.NONE;
            choppingAnimation.Stop();
            choppingAnimation.Reset();
        }

        public override void OnFunctionClick(float x, float z, int sameOrderAmount, bool wasCalledBefore)
        {
            //emmitter.Position = new Vector3(ScreenPos.X, 0.0F, ScreenPos.Y);
            //Sound2D.PlaySound3D(emmitter, "sword1");
            TileEntityTree te = level.GetTileEntity(this, (int)(x / Viewport.TILESIZE), (int)(z / Viewport.TILESIZE)) as TileEntityTree;
            if (te != null)
            {
                if (currentTarget != null) StopChopping();
                OrderChop(te, sameOrderAmount, wasCalledBefore);
            }
            else
            {
                if (isChopping)
                {
                    StopChopping();
                    currentTarget = null;
                }
                SetOrder(new OrderMove(x, z, 2.0F, sameOrderAmount * 4.0F, createNewPathFinder: !wasCalledBefore, stopOnMoveFail: true));
            }
            lastSameOrderAmount = sameOrderAmount;
        }

        protected override void OnOrderFinished()
        {
            base.OnOrderFinished();

            if (currentOrderType == OrderType.CHOP)
                StartChopping();
        }

        private void TryChopNearestTree(int radius)
        {
            TileEntityTree nearestTree = level.GetNearestEntity<TileEntityTree>(this, radius);
            if (nearestTree != null)
                OrderChop(nearestTree, 1, false);
            else return;
        }

        public override void Update()
        {
            base.Update();

            if (isChopping)
            {
                if (currentTarget == null)
                {
                    StopChopping();
                    TryChopNearestTree(5);
                }
                else
                {
                    TurnTowards(currentTarget.X, currentTarget.Z);
                    choppingAnimation.UpdateStep();
                }
            }
            else if (!isChopping && currentOrderType == OrderType.CHOP)
            {
                if (currentOrder != null && currentOrder is OrderMove && ((OrderMove)currentOrder).FailingToMove)
                {
                    StopChopping();
                    TryChopNearestTree(5);
                }
            }


            currentSpriteX = isChopping ? choppingAnimation.CurrentFrame : walkingAnimation.CurrentFrame;
        }

        public override void Render(RenderHelper renderer)
        {
            base.Render(renderer);
            renderer.Render(ScreenPos, currentSpriteX, dirID, Owner.Team.SpriteSheetWoodcutter, colorizer, Viewport.ZOOM);
        }
    }
}
