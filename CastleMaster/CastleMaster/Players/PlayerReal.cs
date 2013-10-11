using CastleMaster.Graphics;
using CastleMaster.Input;
using CastleMaster.MathHelpers;
using CastleMaster.Physics;
using CastleMaster.Units;
using CastleMaster.Units.Mobs;
using CastleMaster.World;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using CastleMaster.Sound;

namespace CastleMaster.Players
{
    public class PlayerReal : Player
    {
        private const int SPEED_SLOW = 10;
        private const int SPREED_FAST = 20;

        private int dXOffs = 0, dYOffs = 0;
        private bool moveWorldWithMouse = false;
        private Point oldMousePos;
        private Unit selectedUnit;
        private int cameraSpeed = 10;
        private List<Mob> selectedMobUnits;

        public PlayerReal(Team team, Level level, Camera camera)
            : base(team, level, camera)
        {
            oldMousePos = Point.Zero;
            selectedMobUnits = new List<Mob>();
        }

        private bool trackBox = false;
        private Point oldPoint, newPoint;
        public override void Update()
        {
            int mouseScroll = InputHandler.MouseScrollDelta;
            if (mouseScroll != 0)
            {
                if (InputHandler.MouseScrollDelta > 1)
                    camera.Zoom(Viewport.ZOOM_STEP);
                else if (InputHandler.MouseScrollDelta < 0)
                    camera.Zoom(-Viewport.ZOOM_STEP);
            }

            cameraSpeed = InputHandler.IsKeyDown(Keys.ShiftKey) ? SPREED_FAST : SPEED_SLOW;

            if (InputHandler.IsKeyDown(Keys.W))
                dYOffs -= cameraSpeed;
            if (InputHandler.IsKeyDown(Keys.S))
                dYOffs += cameraSpeed;
            if (InputHandler.IsKeyDown(Keys.A))
                dXOffs -= cameraSpeed;
            if (InputHandler.IsKeyDown(Keys.D))
                dXOffs += cameraSpeed;

            if (InputHandler.HasMouseButtonBeenPressed(MouseButtons.Middle) || moveWorldWithMouse)
            {
                if (!moveWorldWithMouse)
                {
                    moveWorldWithMouse = true;
                    oldMousePos = InputHandler.MousePos;
                }

                if (InputHandler.IsMouseButtonDown(MouseButtons.Middle))
                {
                    dXOffs -= (InputHandler.MousePos.X - oldMousePos.X);
                    dYOffs -= (InputHandler.MousePos.Y - oldMousePos.Y);

                    oldMousePos = InputHandler.MousePos;
                }
                else moveWorldWithMouse = false;
            }

            if (dXOffs != 0 || dYOffs != 0 || mouseScroll != 0)
            {
                camera.XLeft += dXOffs;
                camera.YTop += dYOffs;
                dXOffs = dYOffs = 0;
                camera.UpdateAudioListener();
            }

            if (InputHandler.HasMouseButtonBeenPressed(MouseButtons.Right))
            {
                if (InputHandler.IsKeyDown(Keys.ControlKey))
                {
                    if (!trackBox)
                    {
                        trackBox = true;
                        oldPoint = InputHandler.MousePos;
                    }
                }
                else
                {
                    Unit u = SelectUnit(Team);

                    selectedMobUnits.ForEach(delegate(Mob m)
                    {
                        m.OnFocusLost();
                        m.IsSelected = false;
                    });

                    selectedMobUnits.Clear();

                    if (selectedUnit != null)
                    {
                        selectedUnit.OnFocusLost();
                        selectedUnit.IsSelected = false;
                    }
                    selectedUnit = null;
                    if (u != null)
                    {
                        if (!(u is Mob))
                            selectedUnit = u;
                        else
                            selectedMobUnits.Add((Mob)u);

                        u.IsSelected = true;
                        u.OnFocus();
                        Audio.PlaySound("select");
                    }
                }
            }

            if (trackBox && InputHandler.IsMouseButtonDown(MouseButtons.Right) && InputHandler.IsKeyDown(Keys.ControlKey))
            {
                newPoint = InputHandler.MousePos;
            }
            else if (trackBox && (!InputHandler.IsMouseButtonDown(MouseButtons.Right) || !InputHandler.IsKeyDown(Keys.ControlKey)))
            {
                trackBox = false;
                bool removePreviousSelections = true;
                List<Mob> selectedMobs;

                if (newPoint == oldPoint)
                {
                    removePreviousSelections = false;
                    selectedMobs = new List<Mob>();
                    Mob m = SelectUnit(Team) as Mob;
                    if (m != null)
                        selectedMobs.Add(m);
                }
                else
                    selectedMobs = SelectMobs(new BoundingRectangle(oldPoint, newPoint, null));

                if (selectedMobs.Count > 0)
                {
                    if (selectedUnit != null)
                    {
                        selectedUnit.OnFocusLost();
                        selectedUnit.IsSelected = false;
                        selectedUnit = null;
                    }

                    if (removePreviousSelections)
                    {
                        selectedMobUnits.ForEach(delegate(Mob m)
                        {
                            m.OnFocusLost();
                            m.IsSelected = false;
                        });
                        selectedMobUnits.Clear();
                    }

                    selectedMobs.ForEach(delegate(Mob m)
                    {
                        selectedMobUnits.Add(m);
                        m.IsSelected = true;
                        m.OnFocus();
                    });
                    Audio.PlaySound("select");
                }
            }

            if (InputHandler.HasMouseButtonBeenPressed(MouseButtons.Left))
            {
                if (selectedUnit != null)
                    selectedUnit.OnFunctionClick(camera.MouseWorldPos.X, camera.MouseWorldPos.Y, 1, false);
                else if (selectedMobUnits.Count > 0)
                    selectedMobUnits[0].OnGroupOrder(selectedMobUnits, camera.MouseWorldPos.X, camera.MouseWorldPos.Y);
            }
        }

        public override Unit SelectUnit(Team team)
        {
            float mouseX = InputHandler.MouseX + camera.XLeft;
            float mouseY = InputHandler.MouseY + camera.YTop;

            float r = 1.0F;
            List<Unit> units = level.GetUnitsWithinScreenSpace(mouseX - r, mouseY - r, mouseX + r, mouseY + r, team);

            Unit current = null;
            float dist = 0.0F;
            foreach (Unit u in units)
            {
                float udist = u.DistanceFromScreenSpaceSqr(mouseX, mouseY);
                if (current == null || udist < dist)
                {
                    current = u;
                    dist = udist;
                }
            }

            return current;
        }

        private List<Mob> SelectMobs(BoundingRectangle rect)
        {
            rect.Translate(new Vector2(camera.XLeft, camera.YTop));

            List<Mob> mobs = level.GetUnitsWithinScreenSpace<Mob>(rect, Team);

            return mobs;
        }

        public override void Render(RenderHelper renderer)
        {
            if (trackBox)
            {
                renderer.SpriteBatch.Draw(Resources.SPRITE_HIGHTLIGHT, FastFunctions.RectangleFromPoints(oldPoint, newPoint), Color.White);
            }
        }
    }
}
