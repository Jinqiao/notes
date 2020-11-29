/* *****************************************************************************
 *  Name:
 *  Date:
 *  Description:
 **************************************************************************** */

import edu.princeton.cs.algs4.BinaryStdIn;
import edu.princeton.cs.algs4.BinaryStdOut;

public class MoveToFront {
    private static final int R = 256;
    private static final int W = 8;

    // apply move-to-front encoding, reading from standard input and writing to standard output
    public static void encode() {
        int[] arr = getIntArr();
        while (!BinaryStdIn.isEmpty()) {
            int c = BinaryStdIn.readByte() & 0xFF;
            for (int i = 0; i < R; i++) {
                if (arr[i] == c) {
                    BinaryStdOut.write(i, W);
                    moveToFront(arr, i);
                    break;
                }
            }
        }
        BinaryStdOut.flush();
    }

    // apply move-to-front decoding, reading from standard input and writing to standard output
    public static void decode() {
        int[] arr = getIntArr();
        while (!BinaryStdIn.isEmpty()) {
            int i = BinaryStdIn.readByte() & 0xFF;
            BinaryStdOut.write(arr[i], W);
            moveToFront(arr, i);
        }
        BinaryStdOut.flush();
    }

    private static int[] getIntArr() {
        int[] arr = new int[R];
        for (int i = 0; i < R; i++) {
            arr[i] = i;
        }
        return arr;
    }

    private static void moveToFront(int[] arr, int i) {
        int c = arr[i];
        while (i > 0) {
            arr[i] = arr[i - 1];
            i--;
        }
        arr[0] = c;
    }

    // if args[0] is "-", apply move-to-front encoding
    // if args[0] is "+", apply move-to-front decoding
    public static void main(String[] args) {
        if (args[0].equals("-")) encode();
        else if (args[0].equals("+")) decode();
    }
}
