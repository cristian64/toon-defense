using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using ToonDefense.Towers;
using ToonDefense.Spaceships;

namespace ToonDefense
{
    public class SelectingPanel : DrawableGameComponent
    {
        Object selected;
        Camera camera;
        World world;
        SpriteBatch spriteBatch;
        SpriteFont font;
        SpriteFont font2;
        SpriteFont font3;
        Texture2D greenButton;
        Texture2D redButton;
        Vector2 greenButtonPosition;
        Vector2 redButtonPosition;
        MouseState prevMouseState;

        public SelectingPanel(Game game, Camera camera, World world)
            : base(game)
        {
            this.camera = camera;
            this.world = world;
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            font = Game.Content.Load<SpriteFont>("fonts\\selection");
            font2 = Game.Content.Load<SpriteFont>("fonts\\selectionname");
            font3 = Game.Content.Load<SpriteFont>("fonts\\buttontext");
            greenButton = Game.Content.Load<Texture2D>("images\\greenbutton");
            redButton = Game.Content.Load<Texture2D>("images\\redbutton");
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            MouseState currentMouseState = Mouse.GetState();

            Spaceship spaceship = selected as Spaceship;
            if (spaceship != null && (spaceship.Health <= 0 || spaceship.Destinations.Count == 0))
                selected = null;

            Tower tower = selected as Tower;
            if (tower != null && tower.Sold)
                selected = null;

            int grabbingAmount = Math.Abs(currentMouseState.X - camera.GrabbingX) + Math.Abs(currentMouseState.Y - camera.GrabbingY);

            if (currentMouseState.LeftButton == ButtonState.Released && prevMouseState.LeftButton == ButtonState.Pressed && (!camera.Grabbing || grabbingAmount < 5) &&
                !IsOnSell(currentMouseState.X, currentMouseState.Y) && !IsOnUpgrade(currentMouseState.X, currentMouseState.Y) &&
                !SpeedPanel.LastInstance.IsOnButtons(currentMouseState.X, currentMouseState.Y))
            {
                if (selected != null)
                    selected.Selected = false;
                selected = null;

                float distance = float.MaxValue;
                Object candidate = null;
                Ray ray = camera.RayFromScreenToWorldRay(currentMouseState.X, currentMouseState.Y);
                foreach (DrawableGameComponent i in GameplayComponent.LastInstance.DrawableComponents)
                {
                    if (i as Spaceship != null || i as Tower != null)
                    {
                        Object selectable = i as Object;
                        float? intersection = selectable.Intersects(ray);
                        if (intersection != null && distance > intersection)
                            candidate = selectable;
                    }
                }

                selected = candidate;
                if (selected != null)
                    selected.Selected = true;
            }

            if (currentMouseState.LeftButton == ButtonState.Released && prevMouseState.LeftButton == ButtonState.Pressed && (!camera.Grabbing || grabbingAmount < 5) &&
                IsOnSell(currentMouseState.X, currentMouseState.Y))
            {
                (selected as Tower).Sold = true;
            }

            if (currentMouseState.LeftButton == ButtonState.Released && prevMouseState.LeftButton == ButtonState.Pressed && (!camera.Grabbing || grabbingAmount < 5) &&
                IsOnUpgrade(currentMouseState.X, currentMouseState.Y))
            {
                if ((selected as Tower).UpgradePrice <= Player.LastInstance.Money)
                    (selected as Tower).Upgrade();
            }

            prevMouseState = currentMouseState;
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            if (selected != null)
            {
                string[] text = selected.ToText();
                Vector2 position = new Vector2(GraphicsDevice.PresentationParameters.BackBufferWidth - Math.Max(font2.MeasureString(selected.Name).X, font.MeasureString(text[0]).X + font.MeasureString(text[1]).X), 0);
                position += new Vector2(-20, 10);
                Vector2 position2 = position + new Vector2(0, font2.MeasureString(selected.Name).Y);
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                spriteBatch.DrawString(font2, selected.Name, position + Vector2.UnitX, Color.Black);
                spriteBatch.DrawString(font2, selected.Name, position - Vector2.UnitX, Color.Black);
                spriteBatch.DrawString(font2, selected.Name, position + Vector2.UnitY, Color.Black);
                spriteBatch.DrawString(font2, selected.Name, position - Vector2.UnitY, Color.Black);
                spriteBatch.DrawString(font2, selected.Name, position, selected as Spaceship != null ? Color.Red : Color.Lime);
                spriteBatch.DrawString(font, text[0], position2 + Vector2.UnitX, Color.Black);
                spriteBatch.DrawString(font, text[0], position2 - Vector2.UnitX, Color.Black);
                spriteBatch.DrawString(font, text[0], position2 + Vector2.UnitY, Color.Black);
                spriteBatch.DrawString(font, text[0], position2 - Vector2.UnitY, Color.Black);
                spriteBatch.DrawString(font, text[0], position2, Color.White);
                Vector2 position3 = position2 + new Vector2(font.MeasureString(text[0]).X, 0);
                spriteBatch.DrawString(font, text[1], position3 + Vector2.UnitX, Color.Black);
                spriteBatch.DrawString(font, text[1], position3 - Vector2.UnitX, Color.Black);
                spriteBatch.DrawString(font, text[1], position3 + Vector2.UnitY, Color.Black);
                spriteBatch.DrawString(font, text[1], position3 - Vector2.UnitY, Color.Black);
                spriteBatch.DrawString(font, text[1], position3, Color.White);
                

                Tower tower = selected as Tower;
                if (tower != null)
                {
                    redButtonPosition = position2 + new Vector2(0, font.MeasureString(text[0]).Y + 5);
                    String redText = "Sell  for  " + ((tower.Price + (tower.Upgraded ? tower.UpgradePrice : 0)) / 2).ToString();
                    Vector2 redTextPosition = redButtonPosition + new Vector2((redButton.Width - font3.MeasureString(redText).X) / 2, (redButton.Height - font3.MeasureString(redText).Y) / 2);
                    if (GameplayComponent.LastInstance.SpeedLevel != SpeedLevel.PAUSED)
                    {
                        spriteBatch.Draw(redButton, redButtonPosition, IsOnSell(Mouse.GetState().X, Mouse.GetState().Y) ? Color.Crimson : Color.White);
                        spriteBatch.DrawString(font3, redText, redTextPosition, Color.White);
                    }
                    else
                    {
                        spriteBatch.Draw(redButton, redButtonPosition, Color.White * 0.5f);
                        spriteBatch.DrawString(font3, redText, redTextPosition, Color.White * 0.5f);
                    }

                    greenButtonPosition = redButtonPosition + new Vector2(0, redButton.Height + 2);
                    String greenText = "";
                    if (tower.Upgraded)
                        greenText = "Upgraded";
                    else
                        greenText = "Upgrade  for  " + (tower.UpgradePrice).ToString();
                    Vector2 greenTextPosition = greenButtonPosition + new Vector2((greenButton.Width - font3.MeasureString(greenText).X) / 2, (greenButton.Height - font3.MeasureString(greenText).Y) / 2);
                    if (!tower.Upgraded && GameplayComponent.LastInstance.SpeedLevel != SpeedLevel.PAUSED && tower.UpgradePrice <= Player.LastInstance.Money)
                    {
                        spriteBatch.Draw(greenButton, greenButtonPosition, IsOnUpgrade(Mouse.GetState().X, Mouse.GetState().Y) ? Color.LimeGreen : Color.White);
                        spriteBatch.DrawString(font3, greenText, greenTextPosition, Color.White);
                    }
                    else
                    {
                        spriteBatch.Draw(greenButton, greenButtonPosition, Color.White * 0.5f);
                        spriteBatch.DrawString(font3, greenText, greenTextPosition, Color.White * 0.5f);
                    }
                }

                spriteBatch.End();
            }

            base.Draw(gameTime);
        }

        public bool IsOnSell(int x, int y)
        {
            Tower tower = selected as Tower;
            return tower != null && BuildingPanel.LastInstance.Tower == null &&
                redButtonPosition.X <= x && x <= redButtonPosition.X + redButton.Width &&
                redButtonPosition.Y <= y && y <= redButtonPosition.Y + redButton.Height;
        }

        public bool IsOnUpgrade(int x, int y)
        {
            Tower tower = selected as Tower;
            return tower != null && BuildingPanel.LastInstance.Tower == null &&
                greenButtonPosition.X <= x && x <= greenButtonPosition.X + greenButton.Width &&
                greenButtonPosition.Y <= y && y <= greenButtonPosition.Y + greenButton.Height;
        }
    }
}