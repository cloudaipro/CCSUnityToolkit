# CCSUnityToolkit
CCS Unity Toolkit
compiler sudoku generator:
 
mcs Generator.cs Board.cs Cell.cs Solver.cs ../CommonExtensions.cs
# 
mono ./Generator.exe 100 Hard Hard.txt

 { "Easy", Tuple.Create(35, 0) }, <br />
 { "Medium", Tuple.Create(81, 5) },<br />
 { "Hard", Tuple.Create(81, 15) },<br />
 { "Extreme", Tuple.Create(81, 45) }<br />

# cutoff mode
  mono ./Generator.exe 100 72 evit.txt
