﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ChaoticMind {
    class GameObjects : IGameFlowComponent {

        internal List<IGameObject> Particles { get; set; }
        internal List<IGameObject> Projectiles { get; set; }
        internal List<IGameObject> Collectables { get; set; }
        internal List<IGameObject> Enemies { get; set; }

        internal Player MainPlayer;

        internal AIDirector EnemyDirector { get; set; }

        internal MapManager Map { get; set; }

        internal World PhysicsWorld { get; set; }

        internal Camera MainCamera { get; set; }

        internal ObjectiveManager Objectives { get; set; }

        internal GameLevel Level { get; set; }

        ShiftInterface _shiftInterface;

        ChaoticMindPlayable _playable;

        internal void StartNewGame(ChaoticMindPlayable playable, GameLevel level) {
            _playable = playable;
            Level = level;

            //Create the physics simulator object, specifying that we want no gravity (since we're top-down)
            PhysicsWorld = new World(Vector2.Zero);

            Particles = new List<IGameObject>();
            Projectiles = new List<IGameObject>();
            Collectables = new List<IGameObject>();
            Enemies = new List<IGameObject>();

            MainPlayer = new Player(this, Vector2.Zero);

            Map = new MapManager(this);
            Map.StartNewGame(Level);

            EnemyDirector = new AIDirector(this);
            EnemyDirector.StartNewGame();

            MainCamera = new Camera(Vector2.Zero, 35.0f, Program.Graphics.GraphicsDevice);
            MainCamera.Target = MainPlayer.Body;

            Objectives = new ObjectiveManager(this, Level);

            _shiftInterface = new ShiftInterface(_playable, this);

            //Start the game off
            Update(0f);
        }

        public void Update(float deltaTime) {
            //Update the FarseerPhysics physics
            PhysicsWorld.Step(deltaTime);

            Projectiles.ForEach(p => p.Update(deltaTime));
            Particles.ForEach(p => p.Update(deltaTime));
            Collectables.ForEach(c => c.Update(deltaTime));
            Enemies.ForEach(e => e.Update(deltaTime));
            MainPlayer.Update(deltaTime);

            Map.Update(deltaTime);
            EnemyDirector.Update(deltaTime);

            MainCamera.Update(deltaTime);
            PainStaticMaker.Update(deltaTime);

            Cull(Projectiles);
            Cull(Particles);
            Cull(Collectables);
            Cull(Enemies);

            CheckGameState();
        }

        private void CheckGameState() {
            NextComponent = null;
            /*if (Objects.MainPlayer.ShouldBeKilled) {
                _deprecatedState.Mode = ObjectiveManager.GameMode.GAMEOVERLOSE;
            } else if (_deprecatedState.AllObjectivesCollected) {
                _deprecatedState.Mode = ObjectiveManager.GameMode.GAMEOVERWIN;
            } else */
            if (InputManager.IsKeyClicked(KeyInput.TOGGLE_PAUSE_MENU)) {
                NextComponent = new PauseMenu(_playable);
            } else if (InputManager.IsKeyClicked(KeyInput.TOGGLE_SHIFT_MENU)) {
                NextComponent = _shiftInterface;
            }
        }

        void Cull(List<IGameObject> objects) {
            for (int i = 0; i < objects.Count; i++) {
                IGameObject obj = objects[i];
                if (obj.ShouldBeKilled) {
                    obj.WasKilled();
                    objects.Remove(obj);
                    i--;
                }
            }
        }

        internal void DrawObjects() {
            Map.DrawTiles(MainCamera);
            Particles.ForEach(p => MainCamera.Draw(p));
            Projectiles.ForEach(p => MainCamera.Draw(p));
            Collectables.ForEach(c => MainCamera.Draw(c));
            Enemies.ForEach(e => MainCamera.Draw(e));
            MainCamera.Draw(MainPlayer);
        }

        internal void DrawGlows() {
            Map.DrawGlows(MainCamera);
            Particles.ForEach(p => MainCamera.DrawGlow(p));
            Projectiles.ForEach(p => MainCamera.DrawGlow(p));
            Collectables.ForEach(c => MainCamera.DrawGlow(c));
            Enemies.ForEach(e => MainCamera.DrawGlow(e));
            MainCamera.DrawGlow(MainPlayer);
        }

        public IGameFlowComponent NextComponent { get; set; }

        public void Draw(float deltaTime) {
            DrawObjects();
            Program.SpriteBatch.End();

            /**** Draw Glow Effects ****/
            //Using BlendState.Additive will make things drawn in this section only brighten, never darken.
            //This means colors will be intensified, and look like glow
            Program.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive);

            DrawGlows();
            PainStaticMaker.DrawStatic();

            Program.SpriteBatch.End();

            //Prep for others' drawing
            Program.SpriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
        }
    }


    internal interface IGameObject : IMiniMapable, IDrawable, IGlowDrawable {
        //Management
        bool ShouldBeKilled { get; }
        void WasKilled();
        void WasCleared();

        void Update(float deltaTime);
    }
}


