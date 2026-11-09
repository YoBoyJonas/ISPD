namespace Solutions;

using NLog;


/// <summary>
/// Solution for the problem U2 "Spalvoti taškai" (multithreaded version).
///
/// Kiekvienos spalvos (raudona/mėlyna/žalia) taškams didžiausias lygiašonis
/// trikampis ieškomas atskiroje gijoje. Baigusi darbą gija užregistruoja savo
/// rezultatą rezultatų lentelėje (mSlots), iš kurios Run metodas
/// po visų gijų pabaigos surenka galutinį Output.
/// </summary>
public class Solution
{
    /// <summary>
    /// Vienas spalvotas taškas plokštumoje.
    /// </summary>
    public class ColorPoint
    {
        /// <summary>Taško spalvos kodas: 'r' - raudona, 'm' - mėlyna, 'z' - žalia.</summary>
        public char Color { get; set; }

        /// <summary>X koordinatė.</summary>
        public int X { get; set; }

        /// <summary>Y koordinatė.</summary>
        public int Y { get; set; }

        public ColorPoint(char color, int x, int y)
        {
            Color = color;
            X = x;
            Y = y;
        }
    }

    /// <summary>
    /// A single input.
    /// </summary>
    public class Input
    {
        /// <summary>Visi įvesties taškai (nepriklausomai nuo spalvos).</summary>
        public List<ColorPoint> Points { get; set; } = new();
    }

    /// <summary>
    /// Vienos spalvos rezultatas.
    /// </summary>
    public class ColorResult
    {
        /// <summary>Ar tos spalvos taškams pavyko rasti lygiašonį trikampį.</summary>
        public bool Found { get; set; }

        /// <summary>Rasto (didžiausio) lygiašonio trikampio perimetras. Reikšminga tik kai Found == true.</summary>
        public double Perimeter { get; set; }
    }

    /// <summary>
    /// A single output.
    /// </summary>
    public class Output
    {
        /// <summary>Rezultatas raudonos spalvos taškams.</summary>
        public ColorResult Raudona { get; set; } = new();

        /// <summary>Rezultatas mėlynos spalvos taškams.</summary>
        public ColorResult Melyna { get; set; } = new();

        /// <summary>Rezultatas žalios spalvos taškams.</summary>
        public ColorResult Zalia { get; set; } = new();
    }

    /// <summary>
    /// Test data.
    /// </summary>
    public class TestData
    {
        /// <summary>
        /// Array of test inputs.
        /// </summary>
        public static Input[] Inputs { get; } = {
            // 1) Pavyzdys iš užduoties sąlygos (n = 12).
            new Input {
                Points = new List<ColorPoint> {
                    new('r', -3, -2), new('z', -6, 2), new('r', -5, 10), new('z', 0, 10),
                    new('m', 0, 12), new('m', 1, 8), new('r', 2, 3), new('z', 4, 6),
                    new('z', 4, 11), new('r', 2, -3), new('r', 6, 0), new('z', 6, 2),
                }
            },

            // 2) Tik raudonas lygiašonis trikampis.
            new Input {
                Points = new List<ColorPoint> {
                    new('r', 0, 0), new('r', 4, 0), new('r', 2, 4),
                }
            },

            // 3) Tik žalias lygiašonis trikampis (neracionalus perimetras).
            new Input {
                Points = new List<ColorPoint> {
                    new('z', 0, 0), new('z', 3, 0), new('z', 0, 3),
                }
            },

            // 4) Visų trijų spalvų trikampiai: kiekvienos gijos rezultatas
            //    yra reikšmingas ir turi pasiekti galutinį Output.
            new Input {
                Points = new List<ColorPoint> {
                    new('r', 0, 0), new('r', 4, 0), new('r', 2, 4),
                    new('m', 10, 0), new('m', 14, 0), new('m', 12, 4),
                    new('z', 0, 10), new('z', 3, 10), new('z', 0, 13),
                }
            },
        };

        /// <summary>
        /// Array of test outputs in corresponding order to test inputs.
        /// </summary>
        public static Output[] Outputs { get; } = {
            new Output {
                Raudona = new ColorResult { Found = true, Perimeter = 16.0 },
                Melyna = new ColorResult { Found = false },
                Zalia = new ColorResult { Found = true, Perimeter = 32.0 },
            },
            new Output {
                Raudona = new ColorResult { Found = true, Perimeter = 4.0 + 2.0 * Math.Sqrt(20.0) },
                Melyna = new ColorResult { Found = false },
                Zalia = new ColorResult { Found = false },
            },
            new Output {
                Raudona = new ColorResult { Found = false },
                Melyna = new ColorResult { Found = false },
                Zalia = new ColorResult { Found = true, Perimeter = 6.0 + 3.0 * Math.Sqrt(2.0) },
            },
            new Output {
                Raudona = new ColorResult { Found = true, Perimeter = 4.0 + 2.0 * Math.Sqrt(20.0) },
                Melyna = new ColorResult { Found = true, Perimeter = 4.0 + 2.0 * Math.Sqrt(20.0) },
                Zalia = new ColorResult { Found = true, Perimeter = 6.0 + 3.0 * Math.Sqrt(2.0) },
            },
        };
    }

    /// <summary>
    /// Logger for this class.
    /// </summary>
    Logger mLog = LogManager.GetCurrentClassLogger();

    /// <summary>Spalvų kodai, kurių kiekvienai paleidžiama po giją.</summary>
    static readonly char[] Colors = { 'r', 'm', 'z' };

    /// <summary>Gijų rezultatai.</summary>
    (char Color, ColorResult Result)?[] mSlots = new (char, ColorResult)?[Colors.Length];
    int mSlotCount;


    /// <summary>
    /// Runs the task solution.
    /// </summary>
    /// <param name="input">Input</param>
    /// <returns>Output</returns>
    public Output Run(Input input)
    {
        mLog.Info("Sprendžiama U2 - Spalvoti taškai, taškų skaičius: {0}", input.Points.Count);

        mSlots = new (char, ColorResult)?[Colors.Length];
        mSlotCount = 0;

        //po giją kiekvienai spalvai
        var threads = Colors
            .Select(color => new Thread(() => Worker(input.Points, color)))
            .ToList();

        threads.ForEach(t => t.Start());
        threads.ForEach(t => t.Join());

        //surenkame rezultatus iš lentelės
        var output = new Output();
        foreach (var slot in mSlots)
        {
            if (slot is null)
                continue;

            switch (slot.Value.Color)
            {
                case 'r': output.Raudona = slot.Value.Result; break;
                case 'm': output.Melyna = slot.Value.Result; break;
                case 'z': output.Zalia = slot.Value.Result; break;
            }
        }

        return output;
    }

    /// <summary>
    /// Gijos darbas: suranda vienos spalvos rezultatą ir jį užregistruoja.
    /// </summary>
    void Worker(List<ColorPoint> allPoints, char color)
    {
        var result = FindLargestIsoscelesTriangle(allPoints, color);
        Register(color, result);
    }
    readonly object mLock = new();
    /// <summary>
    /// Įrašo gijos rezultatą į kitą laisvą rezultatų lentelės langelį.
    /// </summary>
    void Register(char color, ColorResult result)
    {
        lock (mLock)
        {
            int slot = mSlotCount;
            Jitter(2, 2);
            mSlots[slot] = (color, result);
            mSlotCount = slot + 1;
        }
    }

    /// <summary>
    /// Iš visų pateiktų taškų atrenka nurodytos spalvos taškus ir tarp jų
    /// suranda didžiausią (pagal plotą) lygiašonį trikampį.
    /// </summary>
    ColorResult FindLargestIsoscelesTriangle(List<ColorPoint> allPoints, char color)
    {
        var pts = allPoints.Where(p => p.Color == color).ToList();

        var best = new ColorResult { Found = false };
        double bestArea = -1.0;

        for (int i = 0; i < pts.Count; i++)
            for (int j = i + 1; j < pts.Count; j++)
                for (int k = j + 1; k < pts.Count; k++)
                {
                    var a = pts[i];
                    var b = pts[j];
                    var c = pts[k];

                    double doubledArea = DoubledTriangleArea(a, b, c);
                    if (doubledArea == 0)
                        continue; // taškai kolinearūs - trikampio nėra

                    double dAB2 = SquaredDistance(a, b);
                    double dBC2 = SquaredDistance(b, c);
                    double dAC2 = SquaredDistance(a, c);

                    bool isIsosceles = dAB2 == dBC2 || dAB2 == dAC2 || dBC2 == dAC2;
                    if (!isIsosceles)
                        continue;

                    double area = doubledArea / 2.0;
                    if (area > bestArea)
                    {
                        bestArea = area;
                        best = new ColorResult
                        {
                            Found = true,
                            Perimeter = Math.Sqrt(dAB2) + Math.Sqrt(dBC2) + Math.Sqrt(dAC2),
                        };
                    }
                }

        return best;
    }

    /// <summary>Taškų atstumo kvadratas.</summary>
    static double SquaredDistance(ColorPoint a, ColorPoint b)
    {
        double dx = a.X - b.X;
        double dy = a.Y - b.Y;
        return dx * dx + dy * dy;
    }

    /// <summary>Dvigubas trikampio ABC plotas (shoelace formulė).</summary>
    static double DoubledTriangleArea(ColorPoint a, ColorPoint b, ColorPoint c)
    {
        return Math.Abs((b.X - a.X) * (double)(c.Y - a.Y) - (c.X - a.X) * (double)(b.Y - a.Y));
    }

    /// <summary>
    /// This is a helper method to add artificial jitter to thread timing.
    /// </summary>
    /// <param name="baseTimeMs">Constant time delay in ms.</param>
    /// <param name="randomTimeMs">Maximum range for additional random time delay in ms.</param>
    private void Jitter(int baseTimeMs, int randomTimeMs)
    {
        Thread.Sleep(baseTimeMs + Random.Shared.Next(randomTimeMs));
    }
}