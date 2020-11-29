/* *****************************************************************************
 *  Name:
 *  Date:
 *  Description:
 **************************************************************************** */

import edu.princeton.cs.algs4.Picture;

import java.awt.Color;

public class SeamCarver {

    private Picture pic;
    private double[][] eng;
    // private HashMap<Integer, Integer> redMap;
    // private HashMap<Integer, Integer> greenMap;
    // private HashMap<Integer, Integer> blueMap;

    // create a seam carver object based on the given picture
    public SeamCarver(Picture picture) {
        if (picture == null) throw new IllegalArgumentException();
        pic = copy(picture);
        eng = new double[pic.height()][picture.width()];
        calcEng();
    }

    private void calcEng() {
        int maxRowIdx = pic.height() - 1, maxColIdx = pic.width() - 1;

        // redMap = new HashMap<>();
        // greenMap = new HashMap<>();
        // blueMap = new HashMap<>();

        for (int r = 0; r <= maxRowIdx; r++) {
            for (int c = 0; c <= maxColIdx; c++) {
                if (r == 0 || c == 0 || r == maxRowIdx || c == maxColIdx) {
                    eng[r][c] = 1000;
                    continue;
                }

                Color top = pic.get(c, r - 1);
                Color bot = pic.get(c, r + 1);
                Color lf = pic.get(c - 1, r);
                Color rt = pic.get(c + 1, r);

                int rx = rt.getRed() - lf.getRed();
                int gx = rt.getGreen() - lf.getGreen();
                int bx = rt.getBlue() - lf.getBlue();

                int ry = bot.getRed() - top.getRed();
                int gy = bot.getGreen() - top.getGreen();
                int by = bot.getBlue() - top.getBlue();

                // optimization, cache get color call, but not allowed to use HashMap
                // int rx = getColor(c + 1, r, RGB.Red) - getColor(c - 1, r, RGB.Red);
                // int gx = getColor(c + 1, r, RGB.Green) - getColor(c - 1, r, RGB.Green);
                // int bx = getColor(c + 1, r, RGB.Blue) - getColor(c - 1, r, RGB.Blue);
                //
                // int ry = getColor(c, r + 1, RGB.Red) - getColor(c, r - 1, RGB.Red);
                // int gy = getColor(c, r + 1, RGB.Green) - getColor(c, r - 1, RGB.Green);
                // int by = getColor(c, r + 1, RGB.Blue) - getColor(c, r - 1, RGB.Blue);

                int deltaXSq = rx * rx + gx * gx + bx * bx;
                int deltaYSq = ry * ry + gy * gy + by * by;

                eng[r][c] = Math.sqrt(deltaXSq + deltaYSq);
            }
        }
    }

    // private enum RGB {
    //     Red, Green, Blue
    // }
    //
    // private int getColor(int c, int r, RGB rgb) {
    //     int key = key(r, c);
    //     switch (rgb) {
    //         case Red:
    //             if (!redMap.containsKey(key)) redMap.put(key, pic.get(c, r).getRed());
    //             return redMap.get(key);
    //         case Green:
    //             if (!greenMap.containsKey(key)) greenMap.put(key, pic.get(c, r).getGreen());
    //             return greenMap.get(key);
    //         case Blue:
    //             if (!blueMap.containsKey(key)) blueMap.put(key, pic.get(c, r).getBlue());
    //             return blueMap.get(key);
    //         default:
    //             throw new IllegalArgumentException();
    //     }
    // }


    // current picture
    public Picture picture() {
        return copy(pic);
    }

    // width of current picture
    public int width() {
        return pic.width();
    }

    // height of current picture
    public int height() {
        return pic.height();
    }

    // energy of pixel at column x and row y
    public double energy(int x, int y) {
        if (x < 0 || y < 0 || x >= pic.width() || y >= pic.height())
            throw new IllegalArgumentException();

        return eng[y][x];
    }

    // sequence of indices for horizontal seam
    public int[] findHorizontalSeam() {
        int maxRowIdx = pic.height() - 1, maxColIdx = pic.width() - 1;
        int[] edgeTo = new int[pic.height() * pic.width()];
        double[] distTo = initDistTo(false);

        for (int c = 0; c < maxColIdx; c++) {
            for (int r = 0; r <= maxRowIdx; r++) {
                int v = key(r, c);
                relax(v, key(r, c + 1), eng[r][c + 1], distTo, edgeTo);
                if (r - 1 >= 0) relax(v, key(r - 1, c + 1), eng[r - 1][c + 1], distTo, edgeTo);
                if (r + 1 <= maxRowIdx)
                    relax(v, key(r + 1, c + 1), eng[r + 1][c + 1], distTo, edgeTo);
            }
        }

        int[] rows = new int[pic.width()];
        double minDist = Double.POSITIVE_INFINITY;
        for (int r = 0; r < pic.height(); r++) {
            double dist = distTo[key(r, maxColIdx)];
            if (dist < minDist) {
                rows[maxColIdx] = r;
                minDist = dist;
            }
        }

        for (int c = maxColIdx - 1; c >= 0; c--) {
            int v = edgeTo[key(rows[c + 1], c + 1)];
            rows[c] = row(v);
        }
        return rows;
    }

    // sequence of indices for vertical seam
    public int[] findVerticalSeam() {
        int maxRowIdx = pic.height() - 1, maxColIdx = pic.width() - 1;
        int[] edgeTo = new int[pic.height() * pic.width()];
        double[] distTo = initDistTo(true);

        for (int r = 0; r < maxRowIdx; r++) {
            for (int c = 0; c <= maxColIdx; c++) {
                int v = key(r, c);
                relax(v, key(r + 1, c), eng[r + 1][c], distTo, edgeTo);
                if (c - 1 >= 0) relax(v, key(r + 1, c - 1), eng[r + 1][c - 1], distTo, edgeTo);
                if (c + 1 <= maxColIdx)
                    relax(v, key(r + 1, c + 1), eng[r + 1][c + 1], distTo, edgeTo);
            }
        }

        int[] cols = new int[pic.height()];
        double minDist = Double.POSITIVE_INFINITY;
        for (int c = 0; c < pic.width(); c++) {
            double dist = distTo[key(maxRowIdx, c)];
            if (dist < minDist) {
                cols[maxRowIdx] = c;
                minDist = dist;
            }
        }
        for (int r = maxRowIdx - 1; r >= 0; r--) {
            int v = edgeTo[key(r + 1, cols[r + 1])];
            cols[r] = col(v);
        }
        return cols;
    }

    // remove horizontal seam from current picture
    public void removeHorizontalSeam(int[] seam) {
        if (seam == null || seam.length != pic.width() || pic.height() <= 1)
            throw new IllegalArgumentException();
        for (int c = 0; c < pic.width(); c++) {
            if (seam[c] < 0 || seam[c] >= pic.height()) throw new IllegalArgumentException();
            if (c + 1 < pic.width() && Math.abs(seam[c] - seam[c + 1]) > 1) {
                throw new IllegalArgumentException();
            }
        }

        Picture newPic = new Picture(pic.width(), pic.height() - 1);
        int maxRowIdx = pic.height() - 1, maxColIdx = pic.width() - 1;

        for (int r = 0; r <= maxRowIdx; r++) {
            for (int c = 0; c <= maxColIdx; c++) {
                if (seam[c] == r) continue;
                int r2 = r < seam[c] ? r : r - 1;
                newPic.setRGB(c, r2, pic.getRGB(c, r));
            }
        }

        pic = newPic;
        calcEng();
    }

    // remove vertical seam from current picture
    public void removeVerticalSeam(int[] seam) {
        if (seam == null || seam.length != pic.height() || pic.width() <= 1)
            throw new IllegalArgumentException();
        for (int r = 0; r < pic.height(); r++) {
            if (seam[r] < 0 || seam[r] >= pic.width()) throw new IllegalArgumentException();
            if (r + 1 < pic.height() && Math.abs(seam[r] - seam[r + 1]) > 1) {
                throw new IllegalArgumentException();
            }
        }

        Picture newPic = new Picture(pic.width() - 1, pic.height());
        int maxRowIdx = pic.height() - 1, maxColIdx = pic.width() - 1;

        for (int r = 0; r <= maxRowIdx; r++) {
            for (int c = 0; c <= maxColIdx; c++) {
                if (seam[r] == c) continue;
                int c2 = c < seam[r] ? c : c - 1;
                newPic.setRGB(c2, r, pic.getRGB(c, r));
            }
        }

        pic = newPic;
        calcEng();
    }

    private void relax(int v, int w, double wt, double[] distTo, int[] edgeTo) {
        if (distTo[w] <= distTo[v] + wt) return;

        distTo[w] = distTo[v] + wt;
        edgeTo[w] = v;
    }

    private int row(int v) {
        return v / pic.width();
    }

    private int col(int v) {
        return v % pic.width();
    }

    private int key(int r, int c) {
        return r * pic.width() + c;
    }

    private double[] initDistTo(boolean skipFirstRow) {
        double[] distTo = new double[pic.height() * pic.width()];
        int maxRowIdx = pic.height() - 1, maxColIdx = pic.width() - 1;

        for (int r = skipFirstRow ? 1 : 0; r <= maxRowIdx; r++) {
            for (int c = skipFirstRow ? 0 : 1; c <= maxColIdx; c++) {
                distTo[key(r, c)] = Double.POSITIVE_INFINITY;
            }
        }

        return distTo;
    }

    private Picture copy(Picture picture) {
        Picture p = new Picture(picture.width(), picture.height());

        for (int r = 0; r < p.height(); r++) {
            for (int c = 0; c < p.width(); c++) {
                p.setRGB(c, r, picture.getRGB(c, r));
            }
        }

        return p;
    }
}
