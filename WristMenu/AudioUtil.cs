using System.Collections.Generic;
using NAudio.Wave;
using UnityEngine;
using System.IO;
using System.Reflection;

namespace Monke_Mod_Panel
{
    public class AudioUtil
    {
        private static Dictionary<string, AudioClip> clipCache = new Dictionary<string, AudioClip>();

        public static AudioClip GetClip(string resourcePath)
        {
            if (clipCache.TryGetValue(resourcePath, out AudioClip cached))
                return cached;

            Assembly asm = Assembly.GetExecutingAssembly();
            using Stream stream = asm.GetManifestResourceStream(resourcePath);

            if (stream == null)
            {
                MelonLoader.MelonLogger.Error($"Audio resource not found: {resourcePath}");
                return null;
            }

            AudioClip clip = LoadWav(stream, Path.GetFileNameWithoutExtension(resourcePath));

            if (clip != null)
                clipCache[resourcePath] = clip;

            return clip;
        }

        public static void PlayClip(string resourcePath, Vector3 position)
        {
            AudioClip clip = GetClip(resourcePath);
            if (clip == null) return;

            AudioSource.PlayClipAtPoint(clip, position, 0.5f);
        }

        private static AudioClip LoadWav(Stream stream, string clipName)
        {
            using WaveFileReader reader = new WaveFileReader(stream);

            int channels = reader.WaveFormat.Channels;
            int sampleRate = reader.WaveFormat.SampleRate;
            int totalSamples = (int)reader.SampleCount;

            float[] samples = new float[totalSamples * channels];
            
            int offset = 0;
            float[] buffer = new float[reader.WaveFormat.Channels];
            int read;

            ISampleProvider floatProvider = reader.ToSampleProvider();
            while ((read = floatProvider.Read(buffer, 0, buffer.Length)) > 0)
            {
                for (int i = 0; i < read; i++)
                {
                    samples[offset++] = buffer[i];
                }
            }

            AudioClip clip = AudioClip.Create(clipName, totalSamples, channels, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }
    }
}