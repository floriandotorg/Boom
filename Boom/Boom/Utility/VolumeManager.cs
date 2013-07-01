using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;

namespace Boom
{
    class VolumeManager
    {
        public static bool IsMute()
        {
            return SoundEffect.MasterVolume == 0f;
        }

        public static void Mute()
        {
            GameSettings.Speaker = false;
            SoundEffect.MasterVolume = 0f;
            MediaPlayer.Volume = 0f;
        }

        public static void SetDefaultVolume()
        {
            GameSettings.Speaker = true;
            SoundEffect.MasterVolume = .4f;
            MediaPlayer.Volume = .7f;
        }
    }
}
