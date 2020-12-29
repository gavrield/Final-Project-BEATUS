using Audio;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioBeatDetector
{
    class Program
    {
        static void Main(string[] args)
        {
            Program snippets = new Program();
            WavFile audio = new WavFile();
            //string nameOfSong = "TheFatRat - Unity";
            //string nameOfSong = "Mikey's AA";
            string nameOfSong = "Guns-N-Roses-Sweet-Child-O-Mine";
            string path = Directory.GetCurrentDirectory();
            string parent = snippets.GetParent(path);
            string assetsFolder = path;

            while (parent != "Beat-Us")
            {
            parent = snippets.GetParent(parent);
                assetsFolder = parent + "/Assets";
                if (Directory.Exists(assetsFolder))
                {
                    break;
                }
            }
            string pathToSong = assetsFolder + "/Resources/Music/GameSongs/WAV/" + nameOfSong + ".wav";
            audio = audio.Create(pathToSong);
            var wavelwtBPMDetector = new WaveletBPMDetector(audio, 131072);
            double bpm = wavelwtBPMDetector.bpm();
            List<double> list = wavelwtBPMDetector.InstantBpm;

  
            string pathToSave = assetsFolder + "/Resources/Music/BpmVectors/" + nameOfSong + "_bpm.txt";
            using (TextWriter tw = new StreamWriter(pathToSave))
            {
                foreach (double s in list)
                    tw.WriteLine(s);
            }
        }
        string GetParent(string path)
        {
            try
            {
                DirectoryInfo directoryInfo =
                Directory.GetParent(path);
                return directoryInfo.FullName;
            }
            catch (ArgumentNullException)
            {
                Console.WriteLine("Path is a null reference.");
            }
            catch (ArgumentException)
            {
                Console.WriteLine("Path is an empty string, " +
                    "contains only white spaces, or " +
                    "contains invalid characters.");
            }
            return null;
        }
    }

}
