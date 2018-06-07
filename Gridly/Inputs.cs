using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Gridly
{
    public static class Inputs
    {
        public static Vector2 MousePos => curMousePos;
        public static int ScrollWheelValue => curMouseState.ScrollWheelValue;

        static Vector2 curMousePos, prevMousePos;
        static MouseState curMouseState, prevMouseState;
        static KeyboardState curKeyState, prevKeyState;
        static Matrix scale;

        static Inputs()
        {
            scale = Matrix.CreateScale(2);
        }

        public static void Update()
        {
            curMouseState = Mouse.GetState();
            var position = curMouseState.Position;
            curMousePos = Vector2.Transform(position.ToVector2(), Matrix.Invert(scale));
            curKeyState = Keyboard.GetState();
        }

        public static void LateUpdate()
        {
            prevMousePos = curMousePos;
            prevMouseState = curMouseState;
            prevKeyState = curKeyState;
        }

        public static bool IsKeyDown(Keys key)
            => curKeyState.IsKeyDown(key) && prevKeyState.IsKeyUp(key);

        public static bool IsKeyPressing(Keys key)
            => curKeyState.IsKeyDown(key);

        public static bool IsKeyUp(Keys key)
            => curKeyState.IsKeyUp(key) && prevKeyState.IsKeyDown(key);

        public static bool IsLeftMouseDown()
            => curMouseState.LeftButton == ButtonState.Pressed
            && prevMouseState.LeftButton == ButtonState.Released;

        public static bool IsLeftMousePressing()
            => curMouseState.LeftButton == ButtonState.Pressed;

        public static bool IsLeftMouseUp()
            => curMouseState.LeftButton == ButtonState.Released
            && prevMouseState.LeftButton == ButtonState.Pressed;

        public static bool IsRightMouseDown()
            => curMouseState.RightButton == ButtonState.Pressed
            && prevMouseState.RightButton == ButtonState.Released;

        public static bool IsRightMousePressing()
            => curMouseState.RightButton == ButtonState.Pressed;

        public static bool IsRightMouseUp()
            => curMouseState.RightButton == ButtonState.Released
            && prevMouseState.RightButton == ButtonState.Pressed;
    }
}
