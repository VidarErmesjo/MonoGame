namespace MonoGame.Extended.Entities.Systems
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;
    using SpriteManager;

    public class PlayerSystem : UpdateSystem
    {

        public float charge { get; set; }
        public bool isCharging { get; private set; }

        public PlayerSystem()
        {
            charge = 0.0f;
            isCharging = false;
        }

        public override void Update(GameTime gameTime)
        {
            MouseState mouseState = Mouse.GetState();
            
            if(mouseState.LeftButton == ButtonState.Pressed)
            {
                if(!isCharging)
                {
                    toggleIsCharging();
                }
                else
                {
                    charge += 0.3333f;
                }
            }
            else
            {
                if(isCharging)
                {
                    toggleIsCharging();
                    charge = 0.0f;
                }
            }
        }

        public void toggleIsCharging()
        {
            isCharging = !isCharging;
        }
    }
}