using System.Threading.Tasks;

namespace iNGen.Helpers
{
    public static class SoundManager
    {
        public static void PlayFile(string file)
        {
            if (file.EndsWith("wav", false, null))
                PlayWav(file);
            if (file.EndsWith("mp3", false, null))
                PlayMp3(file);
        }


        private static void PlayMp3(string mp3File)
        {
            Task.Run(() =>
            {
                var mediaPlayer = new MediaPlayer.MediaPlayer {FileName = mp3File};
                mediaPlayer.Play();
            });
        }

        private static void PlayWav(string wavFile)
        {
            Task.Run(() =>
            {
                var player = new System.Media.SoundPlayer {SoundLocation = wavFile};
                player.PlaySync();
            });
        }
    }
}