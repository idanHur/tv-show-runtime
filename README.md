# tv-show-runtime
a program that calculates a tv show's total episode runtime written in C# and python on a Linux environment.
the c# program does a Get request from an API with the tv show name and then calculates and returns the tv show's total runtime.
the python program reads the tv show's name from a txt file and calls the C# script in parallel, after getting the result from the multiple calls it calculates which show had the longest runtime and which had the shortest. 
