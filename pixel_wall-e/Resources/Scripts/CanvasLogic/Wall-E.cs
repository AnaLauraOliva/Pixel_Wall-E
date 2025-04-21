public class WallE
{
    public int X { get; private set; }=-1;
    public int Y { get; private set; }=-1;
    public void Spawn(int newX, int newY, Pixel[,] canvas)
    {

        canvas[newY, newX].hasWallE = true;
        X = newX;
        Y = newY;
        
    }
    public void MoveWallE(int newX, int newY, Pixel[,] canvas)
    {
        canvas[Y, X].hasWallE = false;
        canvas[newY, newX].hasWallE = true;
        X = newX;
        Y = newY;

    }
}