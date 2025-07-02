using SudokuSolver;
using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections.Generic;

internal class Program
{
    private static void Main()
    {
        // ---- 1) Paste the puzzle you want to solve here -----------------
        const string givens =
            "000004028406000005100030600000301000087000140000709000002010003900000507670400000";

        // ---- 2) Solve logically, recording every human‑style step --------
        var solver = SolverFactory.CreateFromGivens(givens);
        var steps = new List<LogicalStepDesc>();

        LogicResult result;
        do
        {
            result = solver.ConsolidateBoard(steps);
        } while (result == LogicResult.Changed);

        // If logic alone didn’t finish, fall back to brute‑force
        if (result != LogicResult.PuzzleComplete)
            solver.FindSolution();

        // ---- 3) Build output text ---------------------------------------
        var sb = new StringBuilder();
        sb.AppendLine($"Original grid: {givens}");
        sb.AppendLine();
        sb.AppendLine("Solved grid (single‑line):");
        sb.AppendLine(solver.OutputString);
        sb.AppendLine();
        sb.AppendLine("Solved grid (pretty):");
        for (int r = 0; r < solver.HEIGHT; r++)
        {
            for (int c = 0; c < solver.WIDTH; c++)
                sb.Append(solver.GetValue((r, c))).Append(' ');
            sb.AppendLine();
        }
        sb.AppendLine();
        sb.AppendLine($"Total logical steps: {steps.Count}\n");

        int idx = 1;
        foreach (var s in steps)
            sb.AppendLine($"{idx++}) {s.ToString().Trim()}");

        // ---- 4) Write ONE timestamped file ------------------------------
        string stamp = DateTime.Now.ToString("yyyyMMdd-HH-mm-ss");
        string outFile = $"{stamp}-Solved.txt";
        File.WriteAllText(outFile, sb.ToString());

        Console.WriteLine($"\nWrote {Path.GetFullPath(outFile)}");
    }
}
