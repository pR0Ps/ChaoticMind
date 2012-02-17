﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;

namespace ChaoticMind
{
    //This class is for all intelligent actors in the game, such as 
    // the main player character, and enemies.
    // (Enemies are coming at a later stage of development)
    class Character  : DrawableGameObject
    {

        CharacterType _characterType;

        public Character(CharacterType characterType, World world, Vector2 startingPosition)
            : base(characterType.ImagePrefix, characterType.NumFrames, characterType.AnimationDuration, world, startingPosition)
        {
            _characterType = characterType;
        }

        public override void Update(float deltaTime) {
            decideOnMovementTargets();
            performMovement(deltaTime);

            base.Update(deltaTime);
        }

        //Use input (in the case of a controllable character)
        // or an AI routine to decide what direction this character should try to face, and move
        protected virtual void decideOnMovementTargets()
        {
        }

        private void performMovement(float deltaTime)
        {
            Vector2 movement = LocationToMoveToward - _body.Position;

            _body.ApplyLinearImpulse(_characterType.MaxMovementForce * movement * deltaTime);

        }

        //The location in global coordinates that this character will attempt to
        // move torward (by strafing if necessary)
        protected Vector2 LocationToMoveToward
        {
            get;
            set;
        }

        //The location in global coordinates that this character will attempt to
        // turn torward
        protected Vector2 LocationToFace
        {
            get;
            set;
        }
    }
}