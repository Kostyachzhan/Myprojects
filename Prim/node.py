import sys
sys.path.append(".")
from link import Link

class Node:
    ###Статическое поле. Заменяет счетчик идентификатора
    current_index = 0
    ###Инициализируем узел.
    def __init__(self):
        self.index = Node.current_index
        Node.current_index = Node.current_index + 1
        self.links = []
    ###Соединяем текущий узел с конечным и передаем вес
    def connect(self, ending_node, weight):
        link = Link(self, ending_node, weight)
        self.links.append(link)
        ending_node.links.append(link)
