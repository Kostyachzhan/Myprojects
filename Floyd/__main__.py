import sys
import math
import time
import copy
sys.path.append(".")
from graph import Graph
import floyd

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

print("Initializing sequence matrix")
sm = graph.generate_sequence_matrix()
print("Printing sequence matrix")
Graph.print_matrix(sm, graph.nodes.__len__())

print("Initializing distance matrix")
dm = graph.generate_distance_matrix()
print("Printing distance matrix")
Graph.print_matrix(dm, graph.nodes.__len__())

print("Enter start node(numeration starts from 0): ")
start_node = input()
start_node = int(start_node)

print("Enter end node(numeration starts from 0 and must be other than start node): ")
end_node = input()
end_node = int(end_node)

tmp_dm = copy.deepcopy(dm)
tmp_sm = copy.deepcopy(sm)

regular_find_time = time.time()
floyd.find(dm, sm, start_node, end_node, graph.nodes.__len__())
print("Non-parallel Floyd time - " + str(time.time() - regular_find_time))

print("\nEnter threads count(more than 2): ")
threads = input()

parallel_find_time = time.time()
floyd.parallel(tmp_dm, tmp_sm, start_node, end_node, int(threads), graph.nodes.__len__())
print("Parallel Floyd time - " + str(time.time() - parallel_find_time))