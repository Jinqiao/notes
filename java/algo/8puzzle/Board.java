/* *****************************************************************************
 *  Name:
 *  Date:
 *  Description:
 **************************************************************************** */

import java.util.ArrayList;

public class Board {

    private final int dim;
    private final int[][] tiles;

    // create a board from an n-by-n array of tiles,
    // where tiles[row][col] = tile at (row, col)
    public Board(int[][] ts) {
        dim = ts.length;
        tiles = copyOf(ts);
    }

    // string representation of this board
    public String toString() {
        StringBuilder sb = new StringBuilder();
        sb.append(dim).append("\n");

        for (int[] rows : tiles) {
            for (int i : rows) {
                sb.append(String.format("%2d ", i));
            }
            sb.append('\n');
        }

        return sb.toString();
    }

    // board dimension n
    public int dimension() {
        return dim;
    }

    // number of tiles out of place
    public int hamming() {
        int h = 0, p = 0;
        for (int[] nums : tiles) {
            for (int i : nums) {
                p++;
                if (i == 0) continue;
                if (i != p) h++;
            }
        }

        return h;
    }

    // sum of Manhattan distances between tiles and goal
    public int manhattan() {
        int m = 0;
        for (int r = 1; r <= dim; r++) {
            for (int c = 1; c <= dim; c++) {
                int i = tiles[r - 1][c - 1];
                if (i == 0) continue;

                int tr = i / dim;
                int tc = i % dim;

                tr = tc == 0 ? tr : tr + 1;
                tc = tc == 0 ? dim : tc;

                m += Math.abs(tr - r) + Math.abs(tc - c);
            }
        }

        return m;
    }


    // is this board the goal board?
    public boolean isGoal() {
        return hamming() == 0;
    }

    // does this board equal y?
    public boolean equals(Object y) {
        if (y == this)
            return true;
        if (y == null)
            return false;
        if (this.getClass() != y.getClass())
            return false;
        Board that = (Board) y;
        if (this.dim != that.dim)
            return false;
        for (int row = 0; row < dim; row++)
            for (int col = 0; col < dim; col++)
                if (this.tiles[row][col] != that.tiles[row][col])
                    return false;
        return true;
    }

    // all neighboring boards
    public Iterable<Board> neighbors() {
        ArrayList<Board> ns = new ArrayList<>();

        for (int r = 0; r < dim; r++) {
            for (int c = 0; c < dim; c++) {
                int i = tiles[r][c];
                if (i == 0) {                       // found 0
                    if (r != 0) ns.add(new Board(swap(copyOf(tiles), r, c, r - 1, c)));
                    if (r != dim - 1) ns.add(new Board(swap(copyOf(tiles), r, c, r + 1, c)));
                    if (c != 0) ns.add(new Board(swap(copyOf(tiles), r, c, r, c - 1)));
                    if (c != dim - 1) ns.add(new Board(swap(copyOf(tiles), r, c, r, c + 1)));
                    return ns;
                }
            }
        }

        throw new IllegalArgumentException();
    }

    // a board that is obtained by exchanging any pair of tiles
    public Board twin() {
        int[][] cloned = copyOf(tiles);
        if (cloned[0][0] == 0) {
            swap(cloned, 1, 0, 1, 1);
        }
        else {
            if (cloned[1][0] != 0)
                swap(cloned, 0, 0, 1, 0);
            else
                swap(cloned, 0, 0, 0, 1);
        }

        return new Board(cloned);
    }

    private int[][] copyOf(int[][] matrix) {
        int[][] clone = new int[matrix.length][];
        for (int row = 0; row < matrix.length; row++) {
            clone[row] = matrix[row].clone();
        }
        return clone;
    }

    private int[][] swap(int[][] v, int rowA, int colA, int rowB, int colB) {
        int swap = v[rowA][colA];
        v[rowA][colA] = v[rowB][colB];
        v[rowB][colB] = swap;
        return v;
    }
}
