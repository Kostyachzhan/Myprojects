import sys
sys.path.append(".")
from link import Link

class Node:
    ###Статичне поле. Заміняємо лічильник індентифікатора
    current_index = 0
    ###Ініціалізуємо вузол
    def __init__(self):
        self.index = Node.current_index
        Node.current_index = Node.current_index + 1
        self.links = []
    ###Зєднуємо поточний вузол з кінцевим і передаємо вагу
    def connect(self, ending_node, weight):
        link = Link(self, ending_node, weight)
        self.links.append(link)
        ending_node.links.append(link)
