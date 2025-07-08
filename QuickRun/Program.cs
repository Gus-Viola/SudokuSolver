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
            "720096003000205000080004020000000060106503807040000000030800090000702000200430018";
        //Easiest 300967001040302080020000070070000090000873000500010003004705100905000207800621004 5 points
        //Gentle 000004028406000005100030600000301000087000140000709000002010003900000507670400000 18 points
        //Moderate 720096003000205000080004020000000060106503807040000000030800090000702000200430018 
        //Solved 358967421741352689629184375173546892492873516586219743264795138915438267837621954 0 points

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
        //double raw = 0;
        //foreach (var s in steps)
        //{
        //    string d = s.ToString();
        //    if (d.StartsWith("Naked Single")) raw += 0.25;// one elim.
        //    else if (d.StartsWith("Hidden Single")) raw += 16.0;// one elim.
        //    // add more techniques later
        //}
        //// add one point per solved cell (you always reach 81)
        ///

        // Define point values for each strategy
        var techniquePoints = new Dictionary<string, double>
        {
            ["Naked Single"] = 0.25,
            ["Hidden Single"] = 16.0,
            ["Naked Pair"] = 2.0,
            ["Hidden Pair"] = 4.0,
            ["Naked Triple"] = 6.0,
            ["Hidden Triple"] = 8.0,
            ["Naked Quad"] = 10.0,
            ["Hidden Quad"] = 12.0,
            ["Pointing"] = 35.0,
            ["Tuple"] = 12.0,
            ["X-Wing"] = 175.0,
            ["Y-Wing"] = 75.0,
            ["X-Cycle"] = 24.0,
            ["Unique Rectangle"] = 35.0,
            // Add more techniques here as needed
        };

        // Score the steps
        double raw = 0;
        foreach (var s in steps)
        {
            string desc = s.ToString();
            bool matched = false;

            foreach (var kvp in techniquePoints)
            {
                if (desc.StartsWith(kvp.Key) || desc.Contains(kvp.Key))
                {
                    raw += kvp.Value;
                    matched = true;
                    Console.WriteLine($"{desc} => +{kvp.Value}");
                    break;
                }
            }

            //if (!matched)
            //    raw += 10.0; // Fallback score for unknown strategies
            if (!matched)
            {
                Console.WriteLine($"⚠️ Unmatched strategy: {desc}");
                raw += 50.0; // Temporarily boost unknowns
            }
        }

        //raw += 81;                     // always added 81
        // Stuart adds +81 once per solve **if the starting grid had blanks**
        bool puzzleHadBlanks = givens.Contains('0');
        if (puzzleHadBlanks) raw += 81;


        // Stuart’s public paper shows he divides by 17.5 (≈1000 / 57)
        // to squeeze the number into a 0-80-ish range, then rounds.
        int stuartLike = (int)Math.Round(raw / 17.5);
        int Dev1776grading = (int)Math.Round(raw);

        //prints on screen
        Console.WriteLine($"Stuart-style rating: {stuartLike}");
        Console.WriteLine($"Dev1776 rating: {Dev1776grading}");





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
