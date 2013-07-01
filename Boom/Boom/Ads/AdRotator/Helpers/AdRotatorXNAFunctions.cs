using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace AdRotatorXNA
{
    static class AdRotatorXNAFunctions
    {
        private static bool Clicked = false;
        private static MouseState currState;
        private static MouseState prevState;



        public static bool TestAdClicked(Rectangle BannerRect)
        {
            prevState = currState;
            currState = Mouse.GetState();
            if (currState.LeftButton == ButtonState.Released && prevState.LeftButton == ButtonState.Pressed)
            {

                if (!Clicked)
                {

                    if (BannerRect.Contains(new Point(currState.X, currState.Y)))
                        {
                            Clicked = true;
                            return true;
                        }
                }
            }
            else
            {
                Clicked = false;
            }
            return false;
        }
    }
}
