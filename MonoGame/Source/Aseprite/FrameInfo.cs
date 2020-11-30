namespace MonoGame.Aseprite
{
    public class FrameInfo
    {
        public Rect frame { get; set; }
        public Rect spriteSourceSize { get; set; }
        public Rect sourceSize { get; set; }
        public bool rotated { get; set; }
        public bool trimmed { get; set; }
        public int duration { get; set; }   
    }
}