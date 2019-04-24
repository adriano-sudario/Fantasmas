using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Phantoms.Abstracts;
using Phantoms.Sounds;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Phantoms.Helpers
{
    public class SceneManager
    {
        private static int elapsedTime;
        private static bool isWaiting;
        private static int waitingTime;
        private static Action onStopWaiting;
        private static Dictionary<string, Cyclic> scenes = new Dictionary<string, Cyclic>();

        public static Cyclic CurrentScene { get; set; }

        public static T GetScene<T>(string sceneId) where T : Cyclic => (T)scenes[sceneId];

        public static T GetScene<T>() where T : Cyclic => (T)scenes.Where(s => s.Value.GetType() == typeof(T)).First().Value;

        public static T GetScene<T>(Cyclic scene) where T : Cyclic => (T)scenes.Where(s => s.Value == scene).First().Value;

        public static T GetCurrentScene<T>() where T : Cyclic => (T)CurrentScene;

        public static Dictionary<string, Cyclic> GetScenes() => scenes;

        public static void AddScene(string sceneId, Cyclic scene, bool setToCurrent = false)
        {
            scenes.Add(sceneId, scene);

            if (scenes.Count == 1 || setToCurrent)
                ChangeScene(sceneId);
        }

        public static void RemoveScene(string sceneId)
        {
            if (CurrentScene == scenes[sceneId])
                return;

            scenes.Remove(sceneId);
        }

        public static void ChangeScene(string sceneId, Action onSceneChanged = null)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            CurrentScene = scenes[sceneId];
            onSceneChanged?.Invoke();
        }

        public static void Wait(int milliseconds, Action onStopWaiting = null)
        {
            isWaiting = true;
            elapsedTime = 0;
            waitingTime = milliseconds;
            SceneManager.onStopWaiting = onStopWaiting;
        }

        private static void StopWaiting()
        {
            isWaiting = false;
            elapsedTime = 0;
            waitingTime = 0;
            onStopWaiting?.Invoke();
        }

        public static void Update(GameTime gameTime)
        {
            SoundTrack.Update(gameTime);

            if (isWaiting)
            {
                elapsedTime += (int)gameTime.ElapsedGameTime.TotalMilliseconds;

                if (elapsedTime < waitingTime)
                    return;
                else
                    StopWaiting();

                // It's possible to begin waiting again after invoke 'onStopWaiting'
                if (isWaiting)
                    return;
            }

            CurrentScene.Update(gameTime);
        }
        public static void Draw(SpriteBatch spriteBatch)
        {
            CurrentScene.Draw(spriteBatch);
        }
    }
}
