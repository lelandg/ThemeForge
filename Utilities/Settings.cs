namespace ThemeForge.Utilities;

public class Settings
{
    public string? LastImagePath { get; set; }
    public string? LastModelPath { get; set; }
    public string? DepthMethod { get; set; } = "Depth Anything V2 Large (ViT-L)";
    public double DepthScale { get; set; } = 1.0;
    public bool MirrorBack { get; set; }
    public bool FlatBack { get; set; } = true;
    public bool Watertight { get; set; }
    public string? SketchfabToken { get; set; }
    public string? DeviceName { get; set; } = "CUDA (nVidia)"; 
    public int DecimationValue { get; set; }
    public int Resolution { get; set; } = 20;
    public double WindowHeight { get; set; }
    public double WindowWidth { get; set; }
    public double WindowX { get; set; }
    public double WindowY { get; set; }
    public bool Contiguous { get; set; } = true;
    public int MaskRange { get; set; } = 20;
    public bool AutoMaskBackground { get; set; }
    public string? ProjectName { get; set; }
    public double HorizontalSplitterPosition { get; set; } = 0.15;
    public double VerticalSplitterPosition { get; set; } = 0.5;
    public string? WorkingFolder { get; set; }

    /// <summary>
    /// Name of the currently selected UI theme
    /// </summary>
    public string? ThemeName { get; set; }
}