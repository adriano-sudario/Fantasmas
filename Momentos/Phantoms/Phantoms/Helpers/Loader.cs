using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using System.IO;
using System.Linq;

namespace Phantoms.Helpers
{
    static class Loader
    {
        private static ContentManager content;

        static string _contentFullPath;
        public static string ContentFullPath
        {
            get
            {
                if (_contentFullPath == null || _contentFullPath == "")
                {
                    var directory = new DirectoryInfo(Directory.GetCurrentDirectory());
                    while (directory != null && !directory.GetFiles("app.manifest").Any())
                    {
                        directory = directory.Parent;
                    }
                    _contentFullPath = directory.GetDirectories("Content")[0].FullName;
                }

                return _contentFullPath;
            }
        }

        public static void Initialize(ContentManager content)
        {
            Loader.content = content;
        }

        public static Texture2D LoadTexture(string textureName)
        {
            return content.Load<Texture2D>("Graphics\\" + textureName);
        }

        public static SoundEffect LoadSound(string soundName)
        {
            return content.Load<SoundEffect>("Sounds\\" + soundName);
        }

        public static T LoadDeserializedJsonFile<T>(string fileName)
        {
            string jsonString = LoadJsonFile(fileName);
            return JsonConvert.DeserializeObject<T>(jsonString);
        }

        private static string LoadJsonFile(string fileName)
        {
            return File.ReadAllText(Path.Combine(ContentFullPath, "Data\\" + fileName + ".json"));
        }

        private static object DeserializeJsonFile(string jsonString)
        {
            return JsonConvert.DeserializeObject<object>(jsonString);
        }

        public static void SaveJsonFile<T>(string fileName, T data)
        {
            SaveJsonFile(fileName, JsonConvert.SerializeObject(data));
        }

        private static void SaveJsonFile(string fileName, string jsonText)
        {
            File.WriteAllText(Path.Combine(ContentFullPath, "Data\\" + fileName + ".json"), jsonText);
        }
    }
}
