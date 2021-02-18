import random
import math
import sys
sys.path.append(".")
from link import Link
from node import Node

class Graph:
    ###Зєднуємо графи
    def __init__(self, node_count, link_count = None):
        self.nodes = []

        is_ready = False
        while not is_ready:
            cycle_count = 0
            self.nodes.clear()
            Node.current_index = 0
            for n in range(node_count):
                self.nodes.append(Node())
            
            while not self.is_linked():
                if link_count == None:
                    starting_node = random.choice(self.nodes)
                    ending_node = random.choice(self.nodes)
                    is_in_inf = False
                    while starting_node == ending_node or (not Graph.is_unique(starting_node, ending_node)):
                        starting_node = random.choice(self.nodes)
                        ending_node = random.choice(self.nodes)
                        if cycle_count > node_count*2:
                            is_in_inf = True
                            break
                        cycle_count = cycle_count + 1
                    cycle_count = 0
                    if not is_in_inf:
                        weight = random.randint(1, 101)
                        starting_node.connect(ending_node, weight)
                    else:
                        break
                else:
                    if link_count <= node_count+1:
                        raise Exception("link_count must be greater than node_count for connection proccess")
                    for l in range(link_count):
                        starting_node = random.choice(self.nodes)
                        ending_node = random.choice(self.nodes)
                        is_in_inf = False
                        while starting_node == ending_node or (not Graph.is_unique(starting_node, ending_node)):
                            starting_node = random.choice(self.nodes)
                            ending_node = random.choice(self.nodes)
                            if cycle_count > link_count:
                                is_in_inf = True
                                break
                            cycle_count = cycle_count + 1
                        cycle_count = 0
                        if not is_in_inf:
                            weight = random.randint(1, 101)
                            starting_node.connect(ending_node, weight)
                        else:
                            break
                    break
            if self.is_linked():
                is_ready = True
    ###Перевіряємо унікальне чи з'єднання.
    def is_unique(node_one, node_two):
        is_uniq = True
        for no in node_one.links:
            if (no.starting_node == node_one and no.ending_node == node_two) or (no.starting_node == node_two and no.ending_node == node_one):
                is_uniq = False
                break
        return is_uniq
    ###Перевіряємо, чи немає кинутих, не поєднаних вузлів.
    def is_linked(self):
        is_linkd = True
        for n in self.nodes:
            if not n.links:
                is_linkd = False
                break
            else:
                is_node_started = False
                is_node_ended = False
                for l in n.links:
                    if l.ending_node == n:
                        is_node_ended = True
                    if l.starting_node == n:
                        is_node_started = True
                if not is_node_started or not is_node_ended:
                    is_linkd = False
        return is_linkd
    def view(self):
        for n in self.nodes:
            print("Node: " + str(n.index) + ", contains " + str(n.links.__len__()) + " links")
            for l in n.links:
                print("\t\tLink starting node " + str(l.starting_node.index) + ", ending node " + str(l.ending_node.index) + " and weight " + str(l.weight))
 
    def generate_distance_matrix(self):
        dm = []
        for i in range(self.nodes.__len__()):
            row = []
            for j in range(self.nodes.__len__()):
                if i == j:
                    row.append(-1)
                else:
                    n = self.get_distance(self.nodes[i], self.nodes[j])
                    if(n == math.inf):
                        n = self.get_distance(self.nodes[j], self.nodes[i])
                    row.append(n)
            dm.append(row)
        return dm

    ##Змінили, так щоб враховувалися обидва напрямки, а не тільки один
    def get_distance(self, starting_node, ending_node):
        for link in starting_node.links:
            if link.ending_node == ending_node:
                return link.weight
        return math.inf

    def print_matrix(m, length):
        i = 0
        print(" ", end="  ")
        for j in range(length):
            print(str(j), end=" ")
        print("\n")
        for column in m:
            print(i, end="  ")
            for row in column:
                if row == -1:
                    print("N", end = " ")
                elif row == math.inf:
                    print("I", end = " ")
                else:
                    print(str(row), end = " ")
            print()
            i = i + 1              