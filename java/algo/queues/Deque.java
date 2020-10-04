import edu.princeton.cs.algs4.StdIn;
import edu.princeton.cs.algs4.StdOut;

import java.util.Iterator;
import java.util.NoSuchElementException;

public class Deque<Item> implements Iterable<Item> {

    private Node first, last;
    private int n;

    private class Node {
        private Item item;
        private Node next, prev;

        public Node(Item i, Node n, Node p) {
            item = i;
            next = n;
            prev = p;
        }
    }

    // construct an empty deque
    public Deque() {

    }

    // is the deque empty?
    public boolean isEmpty() {
        return n == 0;
    }

    // return the number of items on the deque
    public int size() {
        return n;
    }

    // add the item to the front
    public void addFirst(Item item) {
        if (item == null) throw new IllegalArgumentException("item is null");

        if (isEmpty()) {
            first = new Node(item, null, null);
            last = first;
        }
        else {
            Node oldFirst = first;
            first = new Node(item, oldFirst, null);
            oldFirst.prev = first;
        }
        n++;
    }

    // add the item to the back
    public void addLast(Item item) {
        if (item == null) throw new IllegalArgumentException("item is null");

        if (isEmpty()) {
            last = new Node(item, null, null);
            first = last;
        }
        else {
            Node oldLast = last;
            last = new Node(item, null, oldLast);
            oldLast.next = last;
        }
        n++;
    }

    // remove and return the item from the front
    public Item removeFirst() {
        if (isEmpty()) throw new NoSuchElementException("the queue is empty");

        Item oldFirstItem = first.item;
        first = first.next;
        if (first != null)
            first.prev = null;
        n--;
        if (n == 0) {
            first = null;
            last = null;
        }
        return oldFirstItem;
    }

    // remove and return the item from the back
    public Item removeLast() {
        if (isEmpty()) throw new NoSuchElementException("the queue is empty");

        Item oldLastItem = last.item;
        last = last.prev;
        if (last != null)
            last.next = null;
        n--;
        if (n == 0) {
            first = null;
            last = null;
        }
        return oldLastItem;
    }

    // return an iterator over items in order from front to back
    public Iterator<Item> iterator() {
        return new DequeIterator();
    }

    private class DequeIterator implements Iterator<Item> {

        private Node head = first;

        public boolean hasNext() {
            return head != null;
        }

        public Item next() {
            if (!hasNext()) throw new NoSuchElementException("no more");

            Item item = head.item;
            head = head.next;

            return item;
        }

        public void remove() {
            throw new UnsupportedOperationException();
        }
    }


    // unit testing (required)
    public static void main(String[] args) {
        Deque<String> d = new Deque<String>();
        d.addFirst(args[0]);
        d.addLast(args[1]);
        d.addFirst(args[2]);

        printDeque(d);

        while (!StdIn.isEmpty()) {              // ctrl-Z to break loop
            String s = StdIn.readString();
            if (s.equals("-")) d.removeFirst();
            else if (s.equals("=")) d.removeLast();
            else if (s.length() > 1) d.addLast(s);
            else {
                d.addFirst(s);
            }

            printDeque(d);
        }
    }

    private static void printDeque(Deque<String> dq) {
        for (String s : dq) {
            StdOut.printf("%s ", s);
        }

        StdOut.printf("| size = %d, isEmpty() = %b", dq.size(), dq.isEmpty());
        StdOut.println();
    }
}
