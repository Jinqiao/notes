/* *****************************************************************************
 *  Name:
 *  Date:
 *  Description:
 **************************************************************************** */

import edu.princeton.cs.algs4.BinaryStdIn;
import edu.princeton.cs.algs4.BinaryStdOut;

public class BurrowsWheeler {
    private static final int R = 256;

    // apply Burrows-Wheeler transform,
    // reading from standard input and writing to standard output
    public static void transform() {
        String s = BinaryStdIn.readString();
        CircularSuffixArray csa = new CircularSuffixArray(s);

        int first = 0;
        for (int i = 0; i < s.length(); i++) {
            if (csa.index(i) == 0) {
                first = i;
                break;
            }
        }
        BinaryStdOut.write(first);

        for (int i = 0; i < s.length(); i++) {
            int x = csa.index(i) - 1;
            BinaryStdOut.write(s.charAt(x < 0 ? s.length() - 1 : x));
        }
        BinaryStdOut.flush();
    }

    // apply Burrows-Wheeler inverse transform,
    // reading from standard input and writing to standard output
    public static void inverseTransform() {
        int first = BinaryStdIn.readInt();
        String t = BinaryStdIn.readString();

        // build next
        // char[] a = t.toCharArray();
        // Arrays.sort(a);
        // int[] next = new int[a.length];
        // int j = 0;
        // for (int i = 0; i < a.length; i++) {
        //     while (t.charAt(j) != a[i]) j++;
        //     next[i] = j;
        //     j++;
        //     if (i + 1 < a.length && a[i + 1] != a[i]) j = 0;
        // }

        int len = t.length();
        int[] next = new int[len];
        int[] count = new int[R + 1];
        char[] firstCol = new char[len];
        for (int i = 0; i < len; i++)
            count[t.charAt(i) + 1]++;
        for (int i = 0; i < R; i++)
            count[i + 1] += count[i];
        for (int i = 0; i < len; i++) {
            int p = count[t.charAt(i)]++;
            firstCol[p] = t.charAt(i);
            next[p] = i;
        }

        for (int i = 0; i < firstCol.length; i++) {
            BinaryStdOut.write(firstCol[first]);
            first = next[first];
        }
        BinaryStdOut.flush();
    }

    // if args[0] is "-", apply Burrows-Wheeler transform
    // if args[0] is "+", apply Burrows-Wheeler inverse transform
    public static void main(String[] args) {
        if (args[0].equals("-")) transform();
        else if (args[0].equals("+")) inverseTransform();
    }
}
