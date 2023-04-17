namespace KataNeo.Aseprite
{
    public class AnimMetadata
    {
        public FrameMetadata[] frames { get; set; }
        public AseMeta Meta { get; set; }
    }

    public class FrameMetadata
    {
        public string filename { get; set; }
        public Rect frame { get; set; }
        public bool rotated { get; set; }
        public bool trimmed { get; set; }
        public Rect spriteSourceSize { get; set; }
        public Vec2 sourceSize { get; set; }
        public float duration { get; set; }
    }

    public class Rect
    {
        public float x { get; set; }
        public float y { get; set; }
        public float w { get; set; }
        public float h { get; set; }
    }

    public class Vec2
    {
        public float w { get; set; }
        public float h { get; set; }
    }

    public class AseMeta
    {
        public string app { get; set; }
        public string version { get; set; }
        public string image { get; set; }
        public string format { get; set; }
        public Vec2 size { get; set; }
        public string scale { get; set; }
        public FrameTags[] frameTags { get; set; }
        public AseLayer[] layers { get; set; }
        public AseSlice[] slices { get; set; }
    }

    public class FrameTags
    {
        public string name { get; set; }
        public int from { get; set; }
        public int to { get; set; }
        public string direction { get; set; }
        public string color { get; set; }
    }

    public class AseLayer
    {
        public string name { get; set; }
        public int opacity { get; set; }
        public string blendMode { get; set; }
    }

    public class AseSlice
    {
        public string name { get; set; }
        public string color { get; set; }
        public AseKey[] keys { get; set; }
        public string data { get; set; }
    }

    public class AseKey
    {
        public int Frame { get; set; }
        public Rect Bounds { get; set; }
    }
}