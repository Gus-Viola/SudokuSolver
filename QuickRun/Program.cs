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
        // Andrew Stuart “Very Easy” example – replace with any 81‑char line
        const string givens =
            "000260701680070090190004500820100040004602900050003028009300074040050036703018000";

        // Build a solver preloaded with the givens
        var solver = SolverFactory.CreateFromGivens(givens);

        // Collect every logical step the solver applies
        var logicalSteps = new List<LogicalStepDesc>();

        // Run logical consolidation until no more changes
        LogicResult logicResult;
        do
        {
            logicResult = solver.ConsolidateBoard(logicalSteps);
        } while (logicResult == LogicResult.Changed);

        // If logic alone didn’t finish, fall back to brute‑force + propagation
        bool solved = logicResult == LogicResult.PuzzleComplete || solver.FindSolution();
        Console.WriteLine($"Solved? {solved}");

        if (!solved)
        {
            Console.WriteLine("No solution found 🤔");
            return;
        }

        // Timestamp for output files: YY-MO-DT-HR-MIN-SEC
        string stamp = DateTime.Now.ToString("yy-MM-dd-HH-mm-ss");
        string solvedPath = $"Solved_{stamp}.txt";
        string stepsPath  = $"Steps_{stamp}.txt";

        // ----------------------------
        // 1) Write solved grid file
        // ----------------------------
        string solvedLine = solver.OutputString;
        var gridBuilder = new StringBuilder();
        for (int r = 0; r < solver.HEIGHT; r++)
        {
            for (int c = 0; c < solver.WIDTH; c++)
                gridBuilder.Append(solver.GetValue((r, c))).Append(' ');
            gridBuilder.AppendLine();
        }

        File.WriteAllText(solvedPath,
            $"{stamp}{Environment.NewLine}{solvedLine}{Environment.NewLine}{Environment.NewLine}{gridBuilder}");

        // ----------------------------
        // 2) Write logical steps file
        // ----------------------------
        var stepLines = logicalSteps
            .Select((step, idx) => $"{idx + 1}) {step.ToString().Trim()}");
        File.WriteAllLines(stepsPath, stepLines);

        // Console summary
        Console.WriteLine($"Saved solved grid to {Path.GetFullPath(solvedPath)}");
        Console.WriteLine($"Logged {logicalSteps.Count} logical steps to {Path.GetFullPath(stepsPath)}\n");
        Console.WriteLine(gridBuilder);
    }
}
