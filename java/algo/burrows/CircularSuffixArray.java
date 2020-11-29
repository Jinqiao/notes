/* *****************************************************************************
 *  Name:
 *  Date:
 *  Description:
 **************************************************************************** */

import edu.princeton.cs.algs4.StdOut;

import java.util.Arrays;

public class CircularSuffixArray {
    private final Integer[] indices;
    private final String s;

    // circular suffix array of s
    public CircularSuffixArray(String s) {
        if (s == null) throw new IllegalArgumentException();
        this.s = s;

        indices = new Integer[s.length()];
        for (int i = 0; i < s.length(); i++) indices[i] = i;

        int len = length();
        Arrays.sort(indices, (idx1, idx2) -> {
            for (int i = 0; i < len; i++) {
                char c1 = s.charAt((i + idx1) % len);
                char c2 = s.charAt((i + idx2) % len);
                if (c1 > c2) return 1;
                if (c1 < c2) return -1;
            }
            return 0;
        });
    }

    // length of s
    public int length() {
        return s.length();
    }

    // returns index of ith sorted suffix
    public int index(int i) {
        if (i < 0 || i >= length())
            throw new IllegalArgumentException();

        return indices[i];
    }

    // unit testing (required)
    public static void main(String[] args) {
        CircularSuffixArray csa = new CircularSuffixArray(args[0]);

        StdOut.println(args[0]);
        StdOut.println(args[0].length());

        StdOut.println(csa.length());
        StdOut.println(csa.index(0));
    }

}
