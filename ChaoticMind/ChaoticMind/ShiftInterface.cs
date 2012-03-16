﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ChaoticMind {
    class ShiftInterface {

        //The space around the shift interface on each of the tightest sides
        const int interfacePixelPadding = 50;

        MapManager _mapManager;
        MapTile[,] _tiles;
        SpriteBatch _spriteBatch;
        
        float _tileDimension;
        float _scalingFactor;
        List<ShiftButton> _buttons = new List<ShiftButton>();
        private ShiftButton _pressedButton;

        internal void Initialize(MapManager mapManager, SpriteBatch spriteBatch) {
            _mapManager = mapManager;
            _spriteBatch = spriteBatch;

            _tiles = _mapManager.Tiles;

            //Calculate the size of our interface
            int interfaceSideLength = Math.Min(Screen.Width, Screen.Height) - 2 * interfacePixelPadding;

            //Add 2 for the buttons on each end
            int gridSideDimension = _tiles.GetLength(0) + 2;

            _tileDimension = interfaceSideLength / (float)gridSideDimension;

            StaticSprite tileSprite = _tiles[0, 0].ShiftTexture;

            _scalingFactor = _tileDimension / (float)tileSprite.CurrentTextureBounds.Width;



            //Create buttons
            float longLength = (gridSideDimension / (float)2) * _tileDimension;
            float shortLength = (_tiles.GetLength(0) / (float)2) * _tileDimension;
            float startX = Screen.Center.X - shortLength;
            float startY = Screen.Center.Y - shortLength;
            for (int i = 0; i < _tiles.GetLength(0); i++) {
                //Top buttons
                _buttons.Add(new ShiftButton(this, new Vector2(startX + _tileDimension * i, Screen.Center.Y - longLength), _tileDimension, (float)Math.PI, i, ShiftDirection.DOWN));
                //Bottom buttons
                _buttons.Add(new ShiftButton(this, new Vector2(startX + _tileDimension * i, Screen.Center.Y + longLength - _tileDimension), _tileDimension, 0.0f, i, ShiftDirection.UP));
                //Left buttons
                _buttons.Add(new ShiftButton(this, new Vector2(Screen.Center.X - longLength, startY + _tileDimension * i), _tileDimension, (float)Math.PI / 2, i, ShiftDirection.RIGHT));
                //Right buttons
                _buttons.Add(new ShiftButton(this, new Vector2(Screen.Center.X + longLength - _tileDimension, startY + _tileDimension * i), _tileDimension, -(float)Math.PI / 2, i, ShiftDirection.LEFT));
            }
        }

        internal void Update() {
            foreach (ShiftButton button in _buttons) {
                button.Update();
            }

            if (InputManager.IsKeyClicked(Keys.Enter)) {
                if (_pressedButton != null) {
                    _mapManager.shiftTiles(_pressedButton.Index, _pressedButton.Direction, DoorDirections.RandomDoors());
                    Program.SharedGame.closeShiftInterface();
                }
            }
        }

        internal void DrawInterface() {
            drawTiles();
            drawShiftButtons();
        }

        private void drawTiles() {
            float halfLength = (_tiles.GetLength(0) / (float)2) * _tileDimension;
            float startingX = Screen.Center.X - halfLength;
            Vector2 drawingLocation = new Vector2(startingX, Screen.Center.Y - halfLength);

            for (int y = 0; y < _tiles.GetLength(0); y++) {
                for (int x = 0; x < _tiles.GetLength(1); x++) {
                    MapTile tile = _tiles[x, y];
                    _spriteBatch.Draw(tile.ShiftTexture.Texture, drawingLocation, tile.ShiftTexture.CurrentTextureBounds, Color.White, tile.Rotation, tile.ShiftTexture.CurrentTextureOrigin, _scalingFactor, SpriteEffects.None, 1.0f);
                    drawingLocation += new Vector2(_tileDimension, 0);
                }
                drawingLocation += new Vector2(0, _tileDimension);
                drawingLocation.X = startingX;
            }
        }

        private void drawShiftButtons() {
            foreach (ShiftButton button in _buttons) {
                _spriteBatch.Draw(button.Sprite.Texture, button.Center, button.Sprite.CurrentTextureBounds, Color.White, button.Rotation, button.Sprite.CurrentTextureOrigin, button.ScalingFactor, SpriteEffects.None, 1.0f);
            }
        }

        internal void ButtonWasPressed(ShiftButton pressedButton) {
            foreach (ShiftButton button in _buttons) {
                if (button != pressedButton) {
                    button.reset();
                }
            }
            _pressedButton = pressedButton;
        }

        internal void ButtonWasToggledOff() {
            _pressedButton = null;
        }
    }
}
