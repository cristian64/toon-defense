#region File Description
//-----------------------------------------------------------------------------
// FireParticleSystem.cs
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
    /// Custom particle system for creating a flame effect.
    /// </summary>
    class FireParticleSystem : ParticleSystem
    {
        public static FireParticleSystem LastInstance = null;

        public FireParticleSystem(Game game, ContentManager content, Camera camera)
            : base(game, content, camera)
        {
            LastInstance = this;
        }

        protected override void InitializeSettings(ParticleSettings settings)
        {
            settings.TextureName = "fire";

            settings.MaxParticles = 2400;

            settings.Duration = TimeSpan.FromSeconds(0.8f);

            settings.DurationRandomness = 1;

            settings.MinHorizontalVelocity = 0;
            settings.MaxHorizontalVelocity = 0.2f;

            settings.MinVerticalVelocity = -0.4f;
            settings.MaxVerticalVelocity = 0.4f;

            // Set gravity upside down, so the flames will 'fall' upward.
            settings.Gravity = new Vector3(0, 0.8f, 0);

            settings.MinColor = new Color(255, 255, 255, 70);
            settings.MaxColor = new Color(255, 255, 255, 150);

            settings.MinRotateSpeed = -0.1f;
            settings.MaxRotateSpeed = 0.1f;

            settings.MinStartSize = 0.1f;
            settings.MaxStartSize = 0.5f;

            settings.MinEndSize = 0.6f;
            settings.MaxEndSize = 1f;

            // Use additive blending.
            settings.BlendState = BlendState.AlphaBlend;
        }
    }
}
