import sys
import math
import time
import copy
sys.path.append(".")
from graph import Graph
import dijkstra


print("Enter nodes count: ")
node_count = input()
node_count = int(node_count)

print("Enter links count(or 0 if default init is used): ")
links_count = input()
if links_count == "0": 
    links_count = None
else: 
    links_count = int(links_count)

init_time = time.time()
print("Initializing Graph")
graph = Graph(node_count, links_count)
print("Initialization ended in - " + str((time.time() - init_time)))
print("\nPrinting Graph info")
graph.view()

print("Initializing distance matrix")
dm = graph.generate_distance_matrix()
#print("Printing distance matrix")
#Graph.print_matrix(dm, graph.nodes.__len__())

print("Enter start node(numeration starts from 0): ")
start_node = input()
start_node = int(start_node)

regular_find_time = time.time()
matrix = dijkstra.dijkstra_find(graph, graph.nodes[start_node])
dijkstra.print_result(matrix, graph.nodes[start_node])
print("Non-parallel Dijkstra time - " + str(time.time() - regular_find_time))

parallel_find_time = time.time()
pmatrix = dijkstra.dijkstra_parallel_find(graph, graph.nodes[start_node])
dijkstra.print_result(pmatrix, graph.nodes[start_node])
print("Parallel Dijkstra time - " + str(time.time() - parallel_find_time))