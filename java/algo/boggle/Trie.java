/* *****************************************************************************
 *  Name:
 *  Date:
 *  Description:
 **************************************************************************** */

class Trie {
    private static final int R = 26;
    private Trie.Node root;

    public class Node {
        private Trie.Node[] next = new Trie.Node[R];
        private boolean isWord;

        Node getNext(char ch) {
            return next[ch - 'A'];
        }
    }

    public Node getRoot() {
        return root;
    }

    public void put(String key) {
        if (key == null) throw new IllegalArgumentException("argument to put() is null");
        else root = put(root, key, 0);
    }

    public boolean contains(String s) {
        Node node = root;
        for (int i = 0; i < s.length(); i++) {
            char c = s.charAt(i);
            node = node.getNext(c);
            if (node == null) return false;
        }
        return node.isWord;
    }

    public boolean hasKeyWithPrefix(String s) {
        Node node = root;
        if (node == null) return false;
        for (int i = 0; i < s.length(); i++) {
            char c = s.charAt(i);
            node = node.getNext(c);
            if (node == null) return false;
        }
        for (int i = 0; i < R; i++) {
            if (node.next[i] != null) return true;
        }
        return false;
    }

    private Node put(Node x, String key, int d) {
        if (x == null) x = new Trie.Node();
        if (d == key.length()) {
            x.isWord = true;
            return x;
        }
        char c = key.charAt(d);
        x.next[c - 'A'] = put(x.getNext(c), key, d + 1);
        return x;
    }
}
