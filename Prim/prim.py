import random
import math
import sys
from multiprocessing import Process, Lock, Value, Array
sys.path.append(".")
from graph import Graph
from node import Node

###цикл знаходження мінімального
def p_prim_step_two(selected, matrix, minimum, i, j, x, y):
    if ((not selected[j]) and matrix[i][j]):  
        if minimum.value > matrix[i][j]:
            minimum.value = matrix[i][j]
            x.value = i
            y.value = j

###розпаралелений цикл знаходження мінімального з потоками
def p_prim_step_one(selected, results, matrix, N, i, x, y, minimum):
    if selected[i]:
        procs = []
        for j in range(N):
            p = Process(target=p_prim_step_two, args=(selected, matrix, minimum, i, j, x, y))
            procs.append(p)
        for proc in procs:
            proc.start()
        for proc in procs:
            proc.join()

###повністью розпаралелений алгоритм пріма 
def parallel_prim_mst(graph, starting_node):
    N = graph.nodes.__len__()
    matrix = graph.generate_distance_matrix()
    Graph.print_matrix(matrix, N)
    results = []
    selected = []
    for x in range(N):
        selected.append(False)
    no_edge = 0
    selected[starting_node.index] = True

    while (no_edge < N - 1):
        minimum = Value("i", 100001)
        x = Value("i", 0)
        y = Value("i", 0)
        procs = []
        for i in range(N):
            p = Process(target=p_prim_step_one, args=(selected, results, matrix, N, i, x, y, minimum))
            procs.append(p)
        for proc in procs:
            proc.start()
        for proc in procs:
            proc.join()
        results.append([x.value, y.value, matrix[x.value][y.value]])
        selected[y.value] = True
        no_edge += 1
    return results

def prim_mst(graph, starting_node):
    ###Розмірність матриці
    N = graph.nodes.__len__()
    ###генеруємо матрицю суміжності
    matrix = graph.generate_distance_matrix()
    results = []
    ###масив вибраних вузлів
    selected = []
    for x in range(N):
        selected.append(0)
    ###кількість звязків
    no_edge = 0
    ###для початкової ноди true, з цієї ноди ми починаємо вона відома
    selected[starting_node.index] = True
    while (no_edge < N - 1):
        ###встановлюємо мінімум звязків, для того щоб можна було зрівнявати їх
        minimum = 100001
        ###індекси
        x = 0
        y = 0
        for i in range(N):
            if selected[i]:
                for j in range(N):
                    ###якщо нода ще не вибрана і представляє якесь значення
                    if ((not selected[j]) and matrix[i][j]):
                        ###зрівнюємо, шукаємо мінімальний звязок
                        if minimum > matrix[i][j]:
                            minimum = matrix[i][j]
                            x = i
                            y = j
        ###добавляємо в results
        results.append([x, y, matrix[x][y]])
        ###говоримо, що знайшли звязок з вершиною
        selected[y] = True
        ###збільшуємо звязок на 1
        no_edge += 1
    return results

def print_mst(matrix, starting_node):
    print("MST Starting Node: " + str(starting_node.index))
    print("Edge : Weight\n")
    for m in matrix:
        print(str(m[0]) + "-" + str(m[1]) + ":" + str(m[2]))