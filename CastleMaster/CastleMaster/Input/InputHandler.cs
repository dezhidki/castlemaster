using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Windows.Forms;
using Keys = System.Windows.Forms.Keys;

namespace CastleMaster.Input
{
    public class InputHandler
    {
        private class Key
        {
            private bool lastState = false, currentState = false, nextState = false;

            public bool IsDown { get { return currentState; } }

            public bool HasBeenPressed { get { return !lastState && currentState; } }

            public void Update()
            {
                lastState = currentState;
                currentState = nextState;
            }

            public void Toggle(bool state)
            {
                nextState = state;
            }

            public void Release()
            {
                currentState = false;
                nextState = false;
            }
        }

        private static Dictionary<Keys, Key> registeredKeys;
        private static Dictionary<MouseButtons, Key> registeredMouseButtons;
        private static int oldMouseScroll = 0, mouseScrollDelta;
        private static Point mousePos = Point.Zero;

        private Control windowControl;

        public InputHandler(GameWindow window)
        {
            registeredKeys = new Dictionary<Keys, Key>();
            registeredMouseButtons = new Dictionary<MouseButtons, Key>();

            windowControl = Control.FromHandle(window.Handle);
            windowControl.PreviewKeyDown += new PreviewKeyDownEventHandler(OnKeyDown);
            windowControl.KeyUp += new KeyEventHandler(OnKeyUp);
            windowControl.MouseDown += new MouseEventHandler(OnMouseDown);
            windowControl.MouseUp += new MouseEventHandler(OnMouseUp);
        }

        private void OnMouseUp(object sender, MouseEventArgs e)
        {
            Key k;
            if (registeredMouseButtons.TryGetValue(e.Button, out k)) k.Toggle(false);
        }

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            Key k;
            if (registeredMouseButtons.TryGetValue(e.Button, out k)) k.Toggle(true);
        }

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            Key k;
            if (registeredKeys.TryGetValue(e.KeyCode, out k)) k.Toggle(false);
        }

        private void OnKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            Key k;
            if (registeredKeys.TryGetValue(e.KeyCode, out k)) k.Toggle(true);
        }

        public void Update()
        {
            foreach (Key k in registeredKeys.Values)
                k.Update();

            MouseState state = Mouse.GetState();
            mousePos.X = state.X;
            mousePos.Y = state.Y;
            mouseScrollDelta = state.ScrollWheelValue - oldMouseScroll;
            oldMouseScroll = state.ScrollWheelValue;

            foreach (Key k in registeredMouseButtons.Values)
                k.Update();
        }

        public void RegisterKeyboardKey(Keys key)
        {
            registeredKeys.Add(key, new Key());
        }

        public void RegisterMouseKey(MouseButtons mb)
        {
            registeredMouseButtons.Add(mb, new Key());
        }

        public void ReleaseAll()
        {
            foreach (Key k in registeredKeys.Values)
                k.Release();

            foreach (Key k in registeredMouseButtons.Values)
                k.Release();
        }

        public static bool IsKeyDown(Keys key)
        {
            Key k;
            return registeredKeys.TryGetValue(key, out k) && k.IsDown;
        }

        public static bool HasKeyBeenPressed(Keys key)
        {
            Key k;
            return registeredKeys.TryGetValue(key, out k) && k.HasBeenPressed;
        }

        public static bool IsMouseButtonDown(MouseButtons mb)
        {
            Key k;
            return registeredMouseButtons.TryGetValue(mb, out k) && k.IsDown;
        }

        public static bool HasMouseButtonBeenPressed(MouseButtons mb)
        {
            Key k;
            return registeredMouseButtons.TryGetValue(mb, out k) && k.HasBeenPressed;
        }

        public static int MouseX { get { return mousePos.X; } }

        public static int MouseY { get { return mousePos.Y; } }

        public static int MouseScrollDelta { get { return mouseScrollDelta; } }

        public static Point MousePos { get { return mousePos; } }
    }
}
