import sys
import math
import time
from threading import Thread
sys.path.append(".")
from stack import Stack
from parallel_graph import ParallelGraph


def place(stack, element):
    q = Stack()
    q.Element = element
    q.Next = stack
    return q

def pop(stack):
    element = stack.Element
    stack = stack.Next
    return [element, stack]

def pathfinder(sm, one, two):
    stack = None
    st = None
    ukzv = None

    flag = False

    elem_one = 0
    elem_two = 0
    k = 0

    stack = place(stack, two)
    stack = place(stack, one)

    while not flag:
        tmp = pop(stack)
        elem_one = tmp[0]
        stack = tmp[1]

        tmp = pop(stack)
        elem_two = tmp[0]
        stack = tmp[1]
        if sm[elem_one][elem_two]==elem_two:
            if elem_two==two:
                flag = True
                st = place(st, elem_one)
                st = place(st, elem_two)
            else:
                st = place(st, elem_one)
                stack = place(stack, elem_two)
        else:
            stack = place(stack, elem_two)
            k = sm[elem_one][elem_two]
            stack = place(stack, k)
            stack = place(stack, elem_one)
    path = []
    ukzv = st
    while(ukzv != None):
        path.append(ukzv.Element)
        ukzv = ukzv.Next
    path.reverse()
    return path
#алгоритм флойда
def find(dm, sm, starting_node, ending_node, length):
    shortest_distance_time = time.time()
    for k in range(length):
        for i in range(length):
            for j in range(length):
                if (i!=j and i!=k and j!=k and dm[i][k]>0 and dm[k][j]>0 and dm[i][k] < math.inf and dm[k][j] < math.inf):
                    if (dm[i][k]+dm[k][j]<dm[i][j] or dm[i][j]==0):
                        dm[i][j]=dm[i][k]+dm[k][j]
                        sm[i][j]=k
    print("\nSearch for shortest path distance between nodes ended in - " + str(time.time() - shortest_distance_time))
    shortest_path_time = time.time()
    i=starting_node
    if (sm[i][ending_node]!=ending_node and sm[i][ending_node]!=0): 
        while (sm[i][ending_node]!=ending_node):
            j=sm[i][ending_node]
            while (sm[i][j]!=j):
                j=sm[i][j]
            i=j
    path = pathfinder(sm, starting_node, ending_node)
    print("Search for shortest path ended in - " + str(time.time() - shortest_path_time))
    print("Shortest path: ", end=" ")
    for p in path:
        print(p, end=" ")
    print("\nShortest path distance between those nodes: " + str(dm[starting_node][ending_node]))
#розпаралелений цикл алгоритма флойда
def parallel_find(graph, start, end, k):
    for i in range(int(start), int(end)):
        for j in range(graph.length):
            if (i!=j and i!=k and j!=k and graph.dm[i][k]>0 and graph.dm[k][j]>0 and graph.dm[i][k] < math.inf and graph.dm[k][j] < math.inf):
                if (graph.dm[i][k]+graph.dm[k][j]<graph.dm[i][j] or graph.dm[i][j]==0):
                    graph.dm[i][j]=graph.dm[i][k]+graph.dm[k][j]
                    graph.sm[i][j]=k
#розпаралелений алгоритм флойда
def parallel(dm, sm, starting_node, ending_node, numProc, length):
    shortest_distance_time = time.time()
    for k in range(length):
        graph = ParallelGraph(dm, sm, length)
        begin = []
        end = []

        begin.append(0)
        end.append(length / numProc)

        for i in range(1, numProc - 1):
                begin.append(begin[i - 1] + end[0])
                end.append(end[i - 1] + end[0])
        begin.append(begin[numProc - 2] + end[0])
        end.append(length)
        threads = []

        for i in range(numProc):
            threads.append(Thread(target=parallel_find, args=(graph, begin[i], end[i], k)))

        for foo in threads:
            foo.start()

        for foo in threads:
            foo.join()
    print("\nSearch for shortest path distance between nodes ended in - " + str(time.time() - shortest_distance_time))
    shortest_path_time = time.time()
    i=starting_node
    if (graph.sm[i][ending_node]!=ending_node and graph.sm[i][ending_node]!=0): 
        while (graph.sm[i][ending_node]!=ending_node):
            j=graph.sm[i][ending_node]
            while (graph.sm[i][j]!=j):
                j=graph.sm[i][j]
            i=j
    path = pathfinder(graph.sm, starting_node, ending_node)
    print("Search for shortest path ended in - " + str(time.time() - shortest_path_time))
    print("Shortest path: ", end=" ")
    for p in path:
        print(p, end=" ")
    print("\nShortest path distance between those nodes: " + str(dm[starting_node][ending_node]))