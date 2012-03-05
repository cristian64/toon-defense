#region File Description
//-----------------------------------------------------------------------------
// ProjectileTrailParticleSystem.cs
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
    /// Custom particle system for leaving smoke trails behind the rocket projectiles.
    /// </summary>
    class WhiteTrailParticleSystem : ParticleSystem
    {
        public static WhiteTrailParticleSystem LastInstance = null;

        public WhiteTrailParticleSystem(Game game, ContentManager content, Camera camera)
            : base(game, content, camera)
        {
            LastInstance = this;
        }

        protected override void InitializeSettings(ParticleSettings settings)
        {
            settings.TextureName = "whitesmoke";

            settings.MaxParticles = 10000;

            settings.Duration = TimeSpan.FromSeconds(1);

            settings.DurationRandomness = 1.5f;

            settings.EmitterVelocitySensitivity = 0;

            settings.MinHorizontalVelocity = 0;
            settings.MaxHorizontalVelocity = 0;

            settings.MinVerticalVelocity = 0;
            settings.MaxVerticalVelocity = 0;

            settings.MinColor = new Color(255, 255, 255, 255);
            settings.MaxColor = new Color(255, 255, 255, 128);

            settings.MinRotateSpeed = -4;
            settings.MaxRotateSpeed = 4;

            settings.MinStartSize = 0.5f;
            settings.MaxStartSize = 0.6f;

            settings.MinEndSize = 0.3f;
            settings.MaxEndSize = 0.4f;

            settings.BlendState = BlendState.Additive;
        }
    }
}
