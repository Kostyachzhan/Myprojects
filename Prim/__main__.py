import sys
import math
import time
import copy
sys.path.append(".")
from graph import Graph
import prim


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
print("Printing distance matrix")
Graph.print_matrix(dm, graph.nodes.__len__())

print("Enter start node(numeration starts from 0): ")
start_node = input()
start_node = int(start_node)

regular_find_time = time.time()
matrix = prim.prim_mst(graph, graph.nodes[start_node])
print("\nNon-parallel Prim time - " + str(time.time() - regular_find_time), end="\n\n")
prim.print_mst(matrix, graph.nodes[start_node])

parallel_find_time = time.time()
pmatrix = prim.parallel_prim_mst(graph, graph.nodes[start_node])
print("Parallel Prim time - " + str(time.time() - parallel_find_time))
prim.print_mst(pmatrix, graph.nodes[start_node])