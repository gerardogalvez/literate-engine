using MoodyBlues.BeatEmUp;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace MoodyBlues
{
    public static class SaveManager
    {

        public static string BeatEmUpFolderName = "BeatEmUpResults";

        public static void SaveBeatEmUpResults(int day, LevelResults levelResults)
        {
            BinaryFormatter formatter = new BinaryFormatter();

            string folderPath = Path.Combine(Application.persistentDataPath, BeatEmUpFolderName);

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            string filePath = Path.Combine(folderPath, string.Concat("Day", day.ToString(), ".dat"));
            using (FileStream fileStream = File.Open(filePath, FileMode.OpenOrCreate, FileAccess.Write))
            {
                formatter.Serialize(fileStream, levelResults);
            }
        }

        public static LevelResults LoadBeatEmUpResults(int day)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            string folderPath = Path.Combine(Application.persistentDataPath, BeatEmUpFolderName);

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            string filePath = Path.Combine(folderPath, string.Concat("Day", day.ToString(), ".dat"));
            Debug.Assert(File.Exists(filePath), $"{filePath} does not exist.");

            using (FileStream fileStream = File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                LevelResults loadedResults = (LevelResults)formatter.Deserialize(fileStream);
                return loadedResults;
            }
        }
    }
}
