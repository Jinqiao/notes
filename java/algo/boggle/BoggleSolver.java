/* *****************************************************************************
 *  Name:
 *  Date:
 *  Description:
 **************************************************************************** */

import edu.princeton.cs.algs4.In;
import edu.princeton.cs.algs4.StdOut;

import java.util.HashSet;

public class BoggleSolver {

    private final Trie dict;

    // Initializes the data structure using the given array of strings as the dictionary.
    // (You can assume each word in the dictionary contains only the uppercase letters A through Z.)
    public BoggleSolver(String[] dictionary) {
        if (dictionary == null) throw new IllegalArgumentException();

        dict = new Trie();
        for (String s : dictionary) {
            if (s.length() < 3) continue;
            try {
                dict.put(s);
            }
            catch (ArrayIndexOutOfBoundsException ex) {
                StdOut.println(s);
                throw ex;
            }
        }
    }

    // Returns the set of all valid words in the given Boggle board, as an Iterable.
    public Iterable<String> getAllValidWords(BoggleBoard board) {
        if (board == null) throw new IllegalArgumentException();

        HashSet<String> words = new HashSet<>();

        for (int i = 0; i < board.rows(); i++) {
            for (int j = 0; j < board.cols(); j++) {
                words.addAll(getWords(board, i, j));
            }
        }

        return words;
    }

    private HashSet<String> getWords(BoggleBoard board, int i0, int j0) {
        HashSet<String> words = new HashSet<>();
        boolean[][] marked = new boolean[board.rows()][board.cols()];
        marked[i0][j0] = true;
        char c = board.getLetter(i0, j0);
        if (c == 'Q') searchAround(board, words, marked, "QU", i0, j0);
        else searchAround(board, words, marked, Character.toString(c), i0, j0);
        return words;
    }

    private void searchAround(BoggleBoard board, HashSet<String> words, boolean[][] marked,
                              String prefix, int i0, int j0) {
        for (int i = i0 - 1; i <= i0 + 1; i++) {
            for (int j = j0 - 1; j <= j0 + 1; j++) {
                if (i == i0 && j == j0) continue;
                if (i < 0 || i >= board.rows() || j < 0 || j >= board.cols()) continue;
                if (marked[i][j]) continue;
                search(board, words, marked, prefix, i, j);
                marked[i][j] = false;
            }
        }
    }

    private void search(BoggleBoard board, HashSet<String> words, boolean[][] marked, String prefix,
                        int i, int j) {
        marked[i][j] = true;
        char c = board.getLetter(i, j);

        if (c == 'Q') checkCandidateAndSearch(board, words, marked, i, j, prefix + "QU");
        else checkCandidateAndSearch(board, words, marked, i, j, prefix + c);
    }

    private void checkCandidateAndSearch(BoggleBoard board, HashSet<String> words,
                                         boolean[][] marked, int i, int j,
                                         String candidate) {
        if (candidate.length() >= 3 && dict.contains(candidate)) words.add(candidate);

        if (dict.hasKeyWithPrefix(candidate)) {
            searchAround(board, words, marked, candidate, i, j);
        }
    }

    // private boolean[][] deepClone(boolean[][] arr) {
    //     boolean[][] newArr = new boolean[arr.length][];
    //     for (int i = 0; i < arr.length; i++)
    //         newArr[i] = arr[i].clone();
    //     return newArr;
    // }

    // Returns the score of the given word if it is in the dictionary, zero otherwise.
    // (You can assume the word contains only the uppercase letters A through Z.)
    public int scoreOf(String word) {
        if (dict.contains(word)) {
            switch (word.length()) {
                case 0:
                case 1:
                case 2:
                    return 0;
                case 3:
                case 4:
                    return 1;
                case 5:
                    return 2;
                case 6:
                    return 3;
                case 7:
                    return 5;
                default:
                    return 11;
            }
        }
        return 0;
    }

    public static void main(String[] args) {
        In in = new In(args[0]);
        String[] dictionary = in.readAllStrings();
        BoggleSolver solver = new BoggleSolver(dictionary);
        BoggleBoard board = new BoggleBoard(args[1]);
        int score = 0;
        for (String word : solver.getAllValidWords(board)) {
            StdOut.println(word);
            score += solver.scoreOf(word);
        }
        StdOut.println("Score = " + score);
    }
}
