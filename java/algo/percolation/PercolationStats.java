import edu.princeton.cs.algs4.StdRandom;
import edu.princeton.cs.algs4.StdStats;

public class PercolationStats {

    private final double x1 = 1.96;
    private final int n;
    private final int t;
    private final double[] thresholds;

    // perform independent trials on an n-by-n grid
    public PercolationStats(int n, int trials) {
        if (n <= 0 || trials <= 0) throw new IllegalArgumentException("n and trials must > 0");

        this.n = n;
        t = trials;
        thresholds = new double[trials];

        for (int i = 0; i < t; i++) {
            Percolation p = new Percolation(n);
            int openCnt = 0;
            while (!p.percolates()) {
                openRandomNode(p);
                openCnt++;
            }

            thresholds[i] = (double) openCnt / (n * n);
        }
    }

    private void openRandomNode(Percolation p) {
        boolean openNode = true;
        int row = 0;
        int col = 0;

        while (openNode) {
            row = StdRandom.uniform(1, n + 1);
            col = StdRandom.uniform(1, n + 1);

            openNode = p.isOpen(row, col);
        }

        p.open(row, col);
    }

    // sample mean of percolation threshold
    public double mean() {
        return StdStats.mean(thresholds);
    }

    // sample standard deviation of percolation threshold
    public double stddev() {
        return StdStats.stddev(thresholds);
    }

    // low endpoint of 95% confidence interval
    public double confidenceLo() {
        return mean() - ((x1 * stddev()) / Math.sqrt(t));
    }

    // high endpoint of 95% confidence interval
    public double confidenceHi() {
        return mean() + ((x1 * stddev()) / Math.sqrt(t));
    }

    // test client (see below)
    public static void main(String[] args) {
        int n = Integer.parseInt(args[0]);
        int t = Integer.parseInt(args[1]);

        PercolationStats s = new PercolationStats(n, t);

        System.out.println("mean() = " + s.mean());
        System.out.println("stddev() = " + s.stddev());
        System.out.format("95%% confidence interval = [%f, %f]\n", s.confidenceLo(),
                          s.confidenceHi());
    }
}
