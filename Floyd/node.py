import sys
sys.path.append(".")
from link import Link

class Node:
    ###Статичне поле. Заміняє лічильник індентифікатора
    current_index = 0
    ###Ініціалізуєм вузол
    def __init__(self):
        self.index = Node.current_index
        Node.current_index = Node.current_index + 1
        self.links = []
    ###Зєднюємо вузол поточний вузол з кінцевим і передаємо вагу
    def connect(self, ending_node, weight):
        link = Link(self, ending_node, weight)
        self.links.append(link)
        ending_node.links.append(link)
    ###Відєднуємо вузол від кінцевого
    def disconnect(self, ending_node):
        link = 0
        for l in self.links:
            if l.ending_node == ending_node:
                link = l
        self.links.remove(link)
        ending_node.links.remove(link)
