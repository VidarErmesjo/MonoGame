using System.Collections.Generic;

namespace MonoGame.Aseprite
{
    public class Meta
    {
        public string app { get; set; }
        public string version { get; set; }
        public string image { get; set; }
        public string format { get; set; }
        public Rect size { get; set; }
        public float scale { get; set; }
        public List<FrameTagInfo> frameTags { get; set; }      
        public List<SliceInfo> slizes { get; set; }
    }
}