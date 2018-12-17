using Microsoft.Xna.Framework.Audio;
using System;

namespace Phantoms.Sounds
{
    public static class SoundTrack
    {
        static SoundEffectInstance song;

        public static bool HasEnded { get { return song.State == SoundState.Stopped; } }
        public static TimeSpan Duration { get; private set; }

        public static void Load(SoundEffect song, bool play = false, bool playOnLoop = true)
        {
            SoundTrack.song = song.CreateInstance();
            Duration = song.Duration;

            if (play)
                Play(playOnLoop);
        }

        public static void Play(bool playOnLoop = true)
        {
            song.IsLooped = playOnLoop;
            song.Play();
        }
    }
}
