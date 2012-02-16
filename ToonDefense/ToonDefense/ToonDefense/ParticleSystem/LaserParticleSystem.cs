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
    class LaserParticleSystem : ParticleSystem
    {
        public static LaserParticleSystem LastInstance = null;

        public LaserParticleSystem(Game game, ContentManager content, Camera camera)
            : base(game, content, camera)
        {
            LastInstance = this;
        }

        protected override void InitializeSettings(ParticleSettings settings)
        {
            settings.TextureName = "laser";

            settings.MaxParticles = 3000;

            settings.Duration = TimeSpan.FromSeconds(0.5f);
            settings.DurationRandomness = 0;

            settings.MinHorizontalVelocity = 0.01f;
            settings.MaxHorizontalVelocity = 0.05f;

            settings.MinVerticalVelocity = -0.1f;
            settings.MaxVerticalVelocity = 0.1f;

            settings.EndVelocity = 0;

            settings.MinColor = Color.White;
            settings.MaxColor = Color.White;

            settings.MinRotateSpeed = -0.5f;
            settings.MaxRotateSpeed = 0.5f;

            settings.MinStartSize = 0.4f;
            settings.MaxStartSize = 0.4f;

            settings.MinEndSize = 0.6f;
            settings.MaxEndSize = 0.6f;

            // Use additive blending.
            settings.BlendState = BlendState.Additive;
        }
    }
}
