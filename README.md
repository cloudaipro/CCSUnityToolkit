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
# ex  mono ./Generator.exe 100 72 evit.txt
mono ./Generator.exe 1000 15 intermediator.txt <br />
mono ./Generator.exe 1000 45 medium.txt <br />
mono ./Generator.exe 1000 72 hard.txt <br />
mono ./Generator.exe 500 81 extreme.txt <br />

