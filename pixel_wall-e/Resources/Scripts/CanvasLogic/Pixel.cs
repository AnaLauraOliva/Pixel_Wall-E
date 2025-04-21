public class Pixel
{
    public Pixel(int x0, int y0, int x1, int y1, string Color)
    {
        X0 = x0;
        Y0 = y0;
        X1 = x1;
        Y1 = y1;
        this.Color = Color;
        hasWallE = false;
    }
    public int X0 { get; }
    public int Y0 { get; }
    public int X1 { get; }
    public int Y1 { get; }
    public string Color { get; set;}
    public bool hasWallE { get; set; }
}