class Link:
    ###Ініціалізація ребер. Ваги будуть видаватися випадково
    def __init__(self, starting_node, ending_node, weight):
        self.starting_node = starting_node
        self.ending_node = ending_node
        self.weight = weight