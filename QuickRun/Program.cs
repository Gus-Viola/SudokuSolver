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
            "358967421741352689629184375173546892492873516586219743264795138915438267837621954";

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

        // after you’ve built the List<LogicalStepDesc> called 'steps'
        double raw = 0;
        foreach (var s in steps)
        {
            string d = s.ToString();
            if (d.StartsWith("Naked Single")) raw += 0.1;   // one elim.
            else if (d.StartsWith("Hidden Single")) raw += 2.0;   // one elim.
                                                                  // add more techniques later – see table below
        }
        // add one point per solved cell (you always reach 81)
        raw += 81;

        // Stuart’s public paper shows he divides by 17.5 (≈1000 / 57)
        // to squeeze the number into a 0-80-ish range, then rounds.
        int stuartLike = (int)Math.Round(raw / 17.5);
        int Dev1776grading = (int)Math.Round(raw);

        



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

        sb.AppendLine($"Stuart-style rating: {stuartLike}");
        sb.AppendLine($"Dev1776 rating: {Dev1776grading}");

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
