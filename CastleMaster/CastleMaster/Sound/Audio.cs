using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;
using CastleMaster.Graphics;
using Microsoft.Xna.Framework;

namespace CastleMaster.Sound
{
    public class Audio : IDisposable
    {
        private const float MIN_VOLUME = -30.0F;
        private const float MAX_VOLUME = 6.0F;
        private const float MAX_HEARING_DISTANCE = 1500.0F;

        private static Camera camera;
        private static SoundBank soundBank;
        private AudioEngine audioEngine;
        private WaveBank waveBank;

        public Audio(string settingsPath, string waveBankPath, string soundBankPath)
        {
            audioEngine = new AudioEngine(settingsPath);
            soundBank = new SoundBank(audioEngine, soundBankPath);
            audioEngine.Update();
            waveBank = new WaveBank(audioEngine, waveBankPath);
        }

        public AudioEngine Engine { get { return audioEngine; } }

        public Camera Camera
        {
            get { return camera; }
            set { camera = value; }
        }

        public static void PlaySound3D(AudioEmitter emitter, Vector2 emmiterNewPos, string name)
        {
            emitter.Position = new Vector3(emmiterNewPos.X, 0.0F, emmiterNewPos.Y);
            float dist = Vector3.Distance(camera.AudioListener.Position, emitter.Position);
            if (dist > MAX_HEARING_DISTANCE) 
                return;

            float volume = MathHelper.Lerp(MAX_VOLUME, MIN_VOLUME, MathHelper.Clamp(dist / MAX_HEARING_DISTANCE, 0.0F, 1.0F));
            Cue cue = soundBank.GetCue(name);
            cue.Apply3D(camera.AudioListener, emitter);
            cue.SetVariable("Volume", volume);
            cue.Play();
        }

        public static void PlaySound(string name)
        {
            soundBank.PlayCue(name);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            audioEngine.Dispose();
            soundBank.Dispose();
            waveBank.Dispose();
        }
    }
}
