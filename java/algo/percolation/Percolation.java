import edu.princeton.cs.algs4.WeightedQuickUnionUF;

public class Percolation {
    private final WeightedQuickUnionUF grid;
    private final WeightedQuickUnionUF full;
    private final int top, bottom;
    private final int n;
    private int opened;
    private boolean[][] openNodes;

    // creates n-by-n grid, with all sites initially blocked
    public Percolation(int n) {
        if (n <= 0) throw new IllegalArgumentException("n must be greater than zero");

        grid = new WeightedQuickUnionUF(n * n + 2);
        full = new WeightedQuickUnionUF(n * n + 1);
        this.n = n;
        top = n * n;
        bottom = top + 1;
        openNodes = new boolean[n][n];
        opened = 0;
    }

    private void checkBounds(int row, int col) {
        if (row < 1 || row > n || col < 1 || col > n) {
            throw new IllegalArgumentException("Out of bounds");
        }
    }

    private int rowColToIdx(int row, int col) {
        return (row - 1) * n + (col - 1);
    }

    // opens the site (row, col) if it is not open already
    public void open(int row, int col) {
        checkBounds(row, col);
        if (isOpen(row, col)) return;

        openNodes[row - 1][col - 1] = true;
        opened++;

        int idx = rowColToIdx(row, col);

        if (row == 1) {
            grid.union(idx, top);
            full.union(idx, top);
        }
        if (row == n) grid.union(idx, bottom);

        if (row > 1 && isOpen(row - 1, col)) {
            grid.union(idx, rowColToIdx(row - 1, col));
            full.union(idx, rowColToIdx(row - 1, col));
        }

        if (col < n && isOpen(row, col + 1)) {
            grid.union(idx, rowColToIdx(row, col + 1));
            full.union(idx, rowColToIdx(row, col + 1));
        }

        if (col > 1 && isOpen(row, col - 1)) {
            grid.union(idx, rowColToIdx(row, col - 1));
            full.union(idx, rowColToIdx(row, col - 1));
        }

        if (row < n && isOpen(row + 1, col)) {
            grid.union(idx, rowColToIdx(row + 1, col));
            full.union(idx, rowColToIdx(row + 1, col));
        }
    }

    // is the site (row, col) open?
    public boolean isOpen(int row, int col) {
        checkBounds(row, col);
        return openNodes[row - 1][col - 1];
    }

    // is the site (row, col) full?
    public boolean isFull(int row, int col) {
        checkBounds(row, col);
        return full.find(top) == full.find(rowColToIdx(row, col));
    }

    // returns the number of open sites
    public int numberOfOpenSites() {
        return opened;
    }

    // does the system percolate?
    public boolean percolates() {
        return grid.find(top) == grid.find(bottom);
    }

}
