using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;

public class Generator
{
    public Board board { get; set; }

    //public Generator(string starting_file)
    //{
    //    var f = File.OpenText(starting_file);
    //    var numbers = new string(f.ReadToEnd().Where(x => "123456789".Contains(x)).ToArray());
    //    var numbersList = numbers.Select(x => Convert.ToInt32(x.ToString())).ToList();
    //    f.Close();

    //    board = new Board(numbersList);
    //}
    public Generator(string sudoku_sample)
    {
        var numbers = new string(sudoku_sample.Where(x => "123456789".Contains(x)).ToArray());
        var numbersList = numbers.Select(x => Convert.ToInt32(x.ToString())).ToList();        

        board = new Board(numbersList);
    }
    public void Randomize(int iterations)
    {
        if (board.get_used_cells().Count == 81)
        {
            var random = new Random();
            for (int i = 0; i < iterations; i++)
            {
                var caseNum = random.Next(0, 4);
                var block = random.Next(0, 3) * 3;
                var options = Enumerable.Range(0, 3).ToList();
                options.Shuffle();
                var piece1 = options[0];
                var piece2 = options[1];

                switch (caseNum)
                {
                    case 0:
                        board.swap_row(block + piece1, block + piece2);
                        break;
                    case 1:
                        board.swap_column(block + piece1, block + piece2);
                        break;
                    case 2:
                        board.swap_stack(piece1, piece2);
                        break;
                    case 3:
                        board.swap_band(piece1, piece2);
                        break;
                    default:
                        break;
                }
            }
        }
        else
        {
            throw new Exception("Rearranging partial board may compromise uniqueness.");
        }
    }

    public void ReduceViaLogical(int cutoff = 81)
    {
        var usedCells = board.get_used_cells();
        usedCells.Shuffle();
        foreach (var cell in usedCells)
        {
            var possibles = board.get_possibles(cell);
            if (possibles.Count == 1)
            {
                cell.Value = 0;
                cutoff -= 1;
            }

            if (cutoff == 0)
            {
                break;
            }
        }
    }

    public void ReduceViaRandom(int cutoff = 81)
    {
        var temp = board;
        var existing = temp.get_used_cells();
        var newSet = existing.Select(x => new { Cell = x, Density = temp.get_density(x) }).ToList();
        var elements = newSet.OrderByDescending(x => x.Density).Select(x => x.Cell).ToList();

        foreach (var cell in elements)
        {
            var original = cell.Value;
            var complement = Enumerable.Range(1, 9).Where(x => x != original).ToList();
            var ambiguous = false;

            foreach (var x in complement)
            {
                cell.Value = x;
                var s = new Solver(temp);
                if (s.solve() && s.is_valid())
                {
                    cell.Value = original;
                    ambiguous = true;
                    break;
                }
            }

            if (!ambiguous)
            {
                cell.Value = 0;
                cutoff -= 1;
            }

            if (cutoff == 0)
            {
                break;
            }
        }
    }

    public string GetCurrentState()
    {
        var template = "There are currently {0} starting cells.\n\rCurrent puzzle state:\n\r\n\r{1}\n\r";
        return string.Format(template, board.get_used_cells().Count, board.ToString());
    }


    public static (int[] unsolved, int[] solved) Sudoku_Generator(string difficultyStr)
    {
        Dictionary<string, Tuple<int, int>> difficulties =
            new Dictionary<string, Tuple<int, int>>() {
                { "Easy", Tuple.Create(35, 0) },
                { "Medium", Tuple.Create(81, 5) },
                { "Hard", Tuple.Create(81, 10) },
                { "Extreme", Tuple.Create(81, 15) }
            };
        Tuple<int, int> difficulty = difficulties[difficultyStr];

        // constructing generator object from puzzle file (space delimited columns, line delimited rows)
        Generator gen = new Generator(@"
            1 2 3 4 5 6 7 8 9
            4 5 6 7 8 9 1 2 3
            7 8 9 1 2 3 4 5 6
            2 1 4 3 6 5 8 9 7
            3 6 5 8 9 7 2 1 4
            8 9 7 2 1 4 3 6 5
            5 3 1 6 4 2 9 7 8
            6 4 2 9 7 8 5 3 1
            9 7 8 5 3 1 6 4 2");

        // applying 100 random transformations to puzzle
        gen.Randomize(100);
       
        // getting a copy before slots are removed
        Board initial = gen.board.copy();

        // applying logical reduction with corresponding difficulty cutoff
        gen.ReduceViaLogical(difficulty.Item1);

        // catching zero case
        if (difficulty.Item2 != 0)
        {
            // applying random reduction with corresponding difficulty cutoff
            gen.ReduceViaRandom(difficulty.Item2);
        }

        // getting copy after reductions are completed
        Board final = gen.board.copy();

        // printing out complete board (solution)
        Console.WriteLine("The initial board before removals was: \r\n\r\n{0}", initial);

        // printing out board after reduction
        Console.WriteLine("The generated board after removals was: \r\n\r\n{0}", final);

        return (final.cells.Select(x => x.Value).ToArray(), initial.cells.Select(x => x.Value).ToArray());
    }

    private struct SudokuBoardData
    {
        public int[] unsolved_data;
        public int[] solved_data;
        public SudokuBoardData(int[] unsolved, int[] solved) : this()
        {
            this.unsolved_data = unsolved;
            this.solved_data = solved;
        }
    };
    static void Main(string[] args) {
        //args.ForEachWithIndex((x, idx) => Console.WriteLine(idx.ToString() + ":" + x));
        
        var count = Int32.Parse(args[0]);
        var level = args[1];
        string file = args[2];

        StreamWriter writer = new StreamWriter(file, true);
        writer.WriteLine("[");
        while (count-- > 0)
        {
            Generator.Sudoku_Generator(level).Also(x =>
            {
                var board = new SudokuBoardData(x.unsolved, x.solved);
                var json = "{\"unsolved_data\":[";
                json += board.unsolved_data.Select(y => y.ToString()).Aggregate((a, b) => $"{a},{b}");
                json += "],";
                json += "\"solved_data\":[";
                json += board.solved_data.Select(y => y.ToString()).Aggregate((a, b) => $"{a},{b}");
                json += ("]}" + ((count == 0) ? "" : ","));
                writer.WriteLine(json);
                Console.WriteLine(json);
            });            
        }
        writer.Write("]");
        writer.Close();        
    }


}