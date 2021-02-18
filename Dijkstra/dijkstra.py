import random
import math
import sys
from threading import Thread
sys.path.append(".")
from graph import Graph
from node import Node

###розпаралелюємо цикл знаходження мінімальної ваги
def dijkstra_parallel_step(matrix, valid, weights, size):
	min_weight = 100001
	ID_min_weight = -1
	for j in range(size):
		if valid[j] and weights[j] < min_weight:
			min_weight = weights[j]
			ID_min_weight = j
	for z in range(size):
		if weights[ID_min_weight] + matrix[ID_min_weight][z] < weights[z]:
			weights[z] = weights[ID_min_weight] + matrix[ID_min_weight][z]
	valid[ID_min_weight] = False
###Запускаємо розпаралелений алгоритм дейкестри з потоками
def dijkstra_parallel_find(graph, starting_node):
    N = graph.nodes.__len__()
    matrix = graph.generate_distance_matrix()
    valid = []
    for x in range(N):
        valid.append(True)
    weights = []
    for x in range(N):
        weights.append(100000)
    weights[starting_node.index] = 0
    threads = []
    for i in range(N):
	    threads.append(Thread(target=dijkstra_parallel_step, args=(matrix, valid, weights, N)))
    for foo in threads:
        foo.start()
    for foo in threads:
        foo.join()
    return weights
		
def dijkstra_find(graph, starting_node):
    ###Кількість нод в графі
    N = graph.nodes.__len__()
    ###Матриця суміжності
    matrix = graph.generate_distance_matrix()
    ###Ті значення, які ще не перевіряли. Добавляємо в нього по кількості нод  true,він активний ще неперевірилию
    valid = []
    for x in range(N):
        valid.append(True)
    ###Масив вагю. Добавляємо в нього велике число(буде замість inf)
    weights = []
    for x in range(N):
        weights.append(100000)
    ###Вартість руху  зі стартової ноди дорівнює 0
    weights[starting_node.index] = 0
    for i in range(N):
        ###Встановюємо min
	    min_weight = 100001
	    ID_min_weight = -1
        ###Шукаємо найменше значення в діапазоні по кільості вузлів
	    for j in range(N):
            ###якщо ще лосі не перевірили ноду і ваги < за мінімальну вагу
		    if valid[j] and weights[j] < min_weight:
			    min_weight = weights[j]
			    ID_min_weight = j
        ###обновляємо матрицю вагів
	    for z in range(N):
            ###якщо в масиві вагів weights[ID_min_weight] + matrix[ID_min_weight][z] < weights[z] це означає знайшли мін вагу
		    if weights[ID_min_weight] + matrix[ID_min_weight][z] < weights[z]:
			    weights[z] = weights[ID_min_weight] + matrix[ID_min_weight][z]
        ###нода перевірена
	    valid[ID_min_weight] = False
    return weights

def print_result(matrix, starting_node):
    index = 0
    for m in matrix:
        if m == -1:
            print(str(starting_node.index) + " is starting node")
        else:
            print("Minimal distance from " + str(index) + " to " + str(starting_node.index) + " is " + str(m))
        index = index + 1
    print("\n")