#region File Description
//-----------------------------------------------------------------------------
// ExplosionSmokeParticleSystem.cs
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
    /// Custom particle system for creating the smokey part of the explosions.
    /// </summary>
    class ExplosionSmokeParticleSystem : ParticleSystem
    {
        public static ExplosionSmokeParticleSystem LastInstance = null;

        public ExplosionSmokeParticleSystem(Game game, ContentManager content, Camera camera)
            : base(game, content, camera)
        {
            LastInstance = this;
        }

        protected override void InitializeSettings(ParticleSettings settings)
        {
            settings.TextureName = "smoke";

            settings.MaxParticles = 200;

            settings.Duration = TimeSpan.FromSeconds(1);

            settings.MinHorizontalVelocity = 0;
            settings.MaxHorizontalVelocity = 0.7f;

            settings.MinVerticalVelocity = -0.5f;
            settings.MaxVerticalVelocity = 1;

            settings.Gravity = new Vector3(0, -0.5f, 0);

            settings.EndVelocity = 0;

            settings.MinColor = Color.LightGray;
            settings.MaxColor = Color.White;

            settings.MinRotateSpeed = -1;
            settings.MaxRotateSpeed = 1;

            settings.MinStartSize = 0.5f;
            settings.MaxStartSize = 1;

            settings.MinEndSize = 1.5f;
            settings.MaxEndSize = 2;
        }
    }
}
