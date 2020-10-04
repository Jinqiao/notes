import edu.princeton.cs.algs4.StdIn;
import edu.princeton.cs.algs4.StdOut;
import edu.princeton.cs.algs4.StdRandom;

import java.util.Iterator;
import java.util.NoSuchElementException;

public class RandomizedQueue<Item> implements Iterable<Item> {

    private Item[] items;
    private int size;

    // construct an empty randomized queue
    public RandomizedQueue() {
        // noinspection unchecked
        items = (Item[]) new Object[1];
    }

    // is the randomized queue empty?
    public boolean isEmpty() {
        return size == 0;
    }

    // return the number of items on the randomized queue
    public int size() {
        return size;
    }

    private void resize(int capacity) {

        // noinspection unchecked
        Item[] temp = (Item[]) new Object[capacity];
        for (int i = 0; i < size; i++)
            temp[i] = items[i];
        items = temp;
    }

    // add the item
    public void enqueue(Item item) {
        if (item == null) throw new IllegalArgumentException("item is null");

        if (size == items.length)
            resize(2 * size);

        items[size++] = item;
    }

    // remove and return a random item
    public Item dequeue() {
        if (isEmpty()) throw new NoSuchElementException("the queue is empty");

        int pos = StdRandom.uniform(size);
        Item item = items[pos];
        items[pos] = items[--size];
        items[size] = null;

        if (size > 0 && size <= items.length / 4)
            resize(items.length / 2);

        return item;
    }

    // return a random item (but do not remove it)
    public Item sample() {
        if (isEmpty()) throw new NoSuchElementException("the queue is empty");

        return items[StdRandom.uniform(size)];
    }

    // return an independent iterator over items in random order
    public Iterator<Item> iterator() {
        return new RandomIterator();
    }

    private class RandomIterator implements Iterator<Item> {
        private int i = size;
        private Item[] shuffledItems;

        public RandomIterator() {
            // noinspection unchecked
            shuffledItems = (Item[]) new Object[size];
            System.arraycopy(items, 0, shuffledItems, 0, size);
            StdRandom.shuffle(shuffledItems);
        }

        public boolean hasNext() {
            return i > 0;
        }

        public void remove() {
            throw new UnsupportedOperationException();
        }

        public Item next() {
            if (!hasNext())
                throw new NoSuchElementException();
            return shuffledItems[--i];
        }
    }

    // unit testing (required)
    public static void main(String[] args) {
        RandomizedQueue<String> rq = new RandomizedQueue<>();
        rq.enqueue(args[0]);
        rq.enqueue(args[1]);

        printDeque(rq);

        while (!StdIn.isEmpty()) {              // ctrl-Z to break loop
            String s = StdIn.readString();
            if (s.equals("-")) rq.dequeue();
            else if (s.equals("=")) StdOut.println(rq.sample());
            else {
                rq.enqueue(s);
            }

            printDeque(rq);
        }
    }

    private static void printDeque(RandomizedQueue<String> rq) {
        for (String s : rq) {
            StdOut.printf("%s ", s);
        }

        StdOut.printf("| size = %d, isEmpty() = %b", rq.size(), rq.isEmpty());
        StdOut.println();
    }
}
