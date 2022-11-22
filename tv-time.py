import os
import fileinput
import multiprocessing as mp
import subprocess
import re

os.environ['GET_TVSHOW_TOTAL_LENGTH_BIN'] = './bin/Debug/net5.0/publish/Home-test'

def runtime_in_hours(runtime: int):
    """
        a function that given a runtime in int returns the string representation in hours and minutes 

        :param runtime: int; show runtime 

        :return: string; the string representation in hours and minutes
    """
    return f"{runtime//60}h {runtime%60}m"

def get_show_length(show_name: str):
    """
        a function that given a show name calls the C# script to get its runtime and returns the show that was given by it 

        :param show_name: string; a string that contains the show name

        :return: Show; returns an object of type Show 
    """
    if (show_name is None) or (show_name == ""):
        return None
    try:
        res = subprocess.check_output([os.environ['GET_TVSHOW_TOTAL_LENGTH_BIN'] ,show_name])
    except subprocess.CalledProcessError as exc:
        if exc.returncode == 10:
            print(f"Error: Could not get info for {show_name}")
        if exc.returncode == 1:
            print(f"Error: Got a bad status code in Get request of show {show_name}")
        return None

    show = Show(show_name.strip("\n"), res)
    return show

def get_shortest_and_longest_show(shows : list):
    """
        a function that given a list of shows finds and returns the longest and shortest shows that are in the list 

        :param shows: list; a list containing show

        :return: Show, Show; returns two Shows that are the longest and shortest
    """
    
    longest, shortrst = None, None
    for show in shows:
        if show is None:
            continue
        if (shortrst is None) and (longest is None):
            longest = show
            shortrst = show
        elif(shortrst > show):
            shortrst = show
        elif(longest < show):
            longest = show
    return longest, shortrst


class Show():
    def __init__(self, name, runtime) -> None:
        self.runtime = int(re.sub('\D', "", str(runtime)))
        self.name = name

    def __eq__(self, __o: object) -> bool:
        if not isinstance(__o, Show):
            return NotImplemented
        return self.runtime < __o.runtime

    def __lt__(self, other):
        if not isinstance(other, Show):
            return NotImplemented
        return self.runtime < other.runtime
    
    def __gt__(self, other):
        if not isinstance(other, Show):
            return NotImplemented
        return self.runtime > other.runtime
    
    def __str__(self) -> str:
        runtime_str = runtime_in_hours(self.runtime)
        return f"{self.name} ({runtime_str})"
    
    
if __name__ == '__main__':
    data = []
    results = []
    for line in fileinput.input():
        data.append(str(line))
    pool = mp.Pool(mp.cpu_count())

    for show_name in data:
        results.append(pool.apply_async(get_show_length, args= (show_name,)).get())
    pool.close()
    pool.join()
    longest, shortrst = get_shortest_and_longest_show(results)
    print(f"The shortest show: {shortrst} \nThe longest show: {longest}" )