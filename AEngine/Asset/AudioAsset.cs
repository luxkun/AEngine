using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aiv.Vorbis;

namespace AEngine
{
    public class AudioAsset : Asset
    {
        public AudioAsset(string fileName, bool stream = false) : base(fileName)
        {
            if (!stream)
                Clip = new AudioClip(FileName);
        }

        public AudioClip Clip { get; set; }

        public AudioAsset Clone()
        {
            var go = new AudioAsset(BaseFileName);
            return go;
        }
    }
}
