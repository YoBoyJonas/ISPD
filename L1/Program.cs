using System.IO;

namespace L1;

public class Program
{
    public static void Main(string[] args)
    {
        // Read points from file -------------------------------------------
        var path = Path.Combine(Directory.GetCurrentDirectory(), "U2.txt");
        String[] lines = File.ReadAllLines(path);
        int numOfPoints = int.Parse(lines[0]);
        var points = new List<(int, int)>();
        for (int i = 1; i <= numOfPoints; i++)
        {
            var point = lines[i].Split(' ');
            points.Add((int.Parse(point[0]), int.Parse(point[1])));
        }

        // find points with the same x or y coordinate ----------------------
        var by_x = new Dictionary<int, List<(int, int)>>();
        var by_y = new Dictionary<int, List<(int, int)>>();
    }
}
