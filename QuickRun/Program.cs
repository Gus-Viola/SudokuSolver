using SudokuSolver;
using System.IO;
using System;                         // ← already there, but just for clarity
using System.Text;

internal class Program
{
    private static void Main()
    {
        const string givens =
            "000260701680070090190004500820100040004602900050003028009300074040050036703018000";

        var solver = SolverFactory.CreateFromGivens(givens);
        bool solved = solver.FindSolution();
        Console.WriteLine($"Solved? {solved}");

        if (!solved)
        {
            Console.WriteLine("No solution found 🤔");
            return;
        }

        // ---------- NEW: timestamped file name ----------
        string stamp = DateTime.Now.ToString("yy-MM-dd-HH-mm-ss");   // YY-MO-DT-HR-MIN-SEC
        string filePath = $"Solved_{stamp}.txt";                     // e.g. Solved_25-07-01-20-15-42.txt
        // --------------------------------------------------

        string solvedLine = solver.OutputString;

        // Pretty multi-line view
        var sb = new StringBuilder();
        for (int r = 0; r < solver.HEIGHT; r++)
        {
            for (int c = 0; c < solver.WIDTH; c++)
                sb.Append(solver.GetValue((r, c))).Append(' ');
            sb.AppendLine();
        }

        File.WriteAllText(filePath,
            $"{stamp}{Environment.NewLine}{solvedLine}{Environment.NewLine}{Environment.NewLine}{sb}");

        Console.WriteLine($"Saved to {Path.GetFullPath(filePath)}");
        Console.WriteLine(sb);
    }
}




//using SudokuSolver;           // ← the namespace in your files
//using System.IO;

//internal class Program
//{
//    private static void Main()
//    {
//        // Andrew Stuart’s “very easy” example
//        const string givens =
//            "000260701" +
//            "680070090" +
//            "190004500" +
//            "820100040" +
//            "004602900" +
//            "050003028" +
//            "009300074" +
//            "040050036" +
//            "703018000";

//        // 1) Build a solver from the givens
//        var solver = SolverFactory.CreateFromGivens(givens);   // :contentReference[oaicite:6]{index=6}

//        // 2) One-shot brute-force + propagation
//        bool solved = solver.FindSolution();                   // :contentReference[oaicite:7]{index=7}
//        Console.WriteLine($"Solved? {solved}");

//        if (solved)
//        {
//            // 81-char solved line (handy for copy-paste or re-importing)
//            string solvedLine = solver.OutputString;          // e.g. 435269…

//            // Pretty, multi-line 9×9 view (same as console)
//            var sb = new System.Text.StringBuilder();
//            for (int r = 0; r < solver.HEIGHT; r++)
//            {
//                for (int c = 0; c < solver.WIDTH; c++)
//                    sb.Append(solver.GetValue((r, c))).Append(' ');
//                sb.AppendLine();
//            }

//            // Choose your flavour: write both to one file
//            string filePath = "SolvedPuzzle.txt";             // or @"C:\…\mygrid.txt"
//            File.WriteAllText(filePath,
//                solvedLine + Environment.NewLine + Environment.NewLine + sb.ToString());

//            Console.WriteLine($"Saved to {Path.GetFullPath(filePath)}");
//            Console.WriteLine(sb);  // still print to console
//        }
//        else
//        {
//            Console.WriteLine("No solution found 🤔");
//        }
//    }
//}
