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
    class ForceFieldParticleSystem : ParticleSystem
    {
        public static ForceFieldParticleSystem LastInstance = null;

        public ForceFieldParticleSystem(Game game, ContentManager content, Camera camera)
            : base(game, content, camera)
        {
            LastInstance = this;
        }

        protected override void InitializeSettings(ParticleSettings settings)
        {
            settings.TextureName = "forcefield";

            settings.MaxParticles = 10000;

            settings.Duration = TimeSpan.FromSeconds(2);
            settings.DurationRandomness = 0;

            settings.MinHorizontalVelocity = 0;
            settings.MaxHorizontalVelocity = 0;

            settings.MinVerticalVelocity = 0;
            settings.MaxVerticalVelocity = 0;

            settings.EndVelocity = 0;

            settings.MinColor = Color.White;
            settings.MaxColor = Color.White;

            settings.MinRotateSpeed = -0.5f;
            settings.MaxRotateSpeed = 0.5f;

            settings.MinStartSize = 0.5f;
            settings.MaxStartSize = 0.5f;

            settings.MinEndSize = 0.1f;
            settings.MaxEndSize = 0.1f;

            // Use additive blending.
            settings.BlendState = BlendState.Additive;
        }
    }
}
