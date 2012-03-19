#region File Description
//-----------------------------------------------------------------------------
// ExplosionParticleSystem.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace ToonDefense.ParticleSystem
{
    /// <summary>
    /// Custom particle system for creating the fiery part of the explosions.
    /// </summary>
    class ElectricityBallParticleSystem : ParticleSystem
    {
        public static ElectricityBallParticleSystem LastInstance = null;

        public ElectricityBallParticleSystem(Game game, ContentManager content, Camera camera)
            : base(game, content, camera)
        {
            LastInstance = this;
        }

        protected override void InitializeSettings(ParticleSettings settings)
        {
            settings.TextureName = "electricityball";

            settings.MaxParticles = 10000;

            settings.Duration = TimeSpan.FromSeconds(1);
            settings.DurationRandomness = 0.1f;

            settings.MinHorizontalVelocity = 0.00f;
            settings.MaxHorizontalVelocity = 0.00f;

            settings.MinVerticalVelocity = -0.1f;
            settings.MaxVerticalVelocity = 0.1f;

            settings.EndVelocity = 0;

            settings.MinColor = Color.White;
            settings.MaxColor = Color.White;

            settings.MinRotateSpeed = -10;
            settings.MaxRotateSpeed = 10;

            settings.MinStartSize = 1f;
            settings.MaxStartSize = 1f;

            settings.MinEndSize = 1.3f;
            settings.MaxEndSize = 1.7f;

            // Use additive blending.
            settings.BlendState = BlendState.Additive;
        }
    }
}
