/* *****************************************************************************
 *  Name:
 *  Date:
 *  Description:
 **************************************************************************** */

import edu.princeton.cs.algs4.In;
import edu.princeton.cs.algs4.StdDraw;
import edu.princeton.cs.algs4.StdOut;

import java.util.ArrayList;
import java.util.Arrays;

public class BruteCollinearPoints {

    private final ArrayList<LineSegment> segmentList;
    private final Point[] collinearPoints;

    public BruteCollinearPoints(Point[] ps)    // finds all line segmentList containing 4 points
    {
        if (ps == null || ps.length == 0)
            throw new IllegalArgumentException();
        for (int i = 0; i < ps.length; i++)
            if (ps[i] == null) throw new IllegalArgumentException();

        Point[] points = ps.clone();
        Arrays.sort(points);
        for (int i = 0; i < points.length - 1; ++i) {
            if (points[i].compareTo(points[i + 1]) == 0) {
                throw new IllegalArgumentException("Contains a repeated point.");
            }
        }

        segmentList = new ArrayList<LineSegment>();
        collinearPoints = new Point[4];
        if (points.length < 4) {
            return;
        }

        // To find combinations: https://stackoverflow.com/a/29914908/4499942
        int k = 4;
        int[] s = { 0, 1, 2, 3 };
        addToSegmentsIfCollinear(points, s);
        while (true) {
            int i = k - 1;
            while (i >= 0 && s[i] == points.length - k + i)
                i--;
            if (i < 0)
                break;

            s[i]++;                         // increment this item
            for (++i; i < k; i++) {         // fill up remaining items
                s[i] = s[i - 1] + 1;
            }
            addToSegmentsIfCollinear(points, s);
        }
    }

    public int numberOfSegments()        // the number of line segmentList
    {
        return segments().length;
    }

    public LineSegment[] segments()      // the line segmentList
    {
        return segmentList.toArray(new LineSegment[0]);
    }

    private boolean isCollinear(Point p, Point q, Point r, Point s) {
        double pqSlope = p.slopeTo(q);


        return pqSlope == p.slopeTo(r) && pqSlope == p.slopeTo(s);
    }

    private void addToSegmentsIfCollinear(Point[] ps, int[] s) {
        if (isCollinear(ps[s[0]], ps[s[1]], ps[s[2]], ps[s[3]])) {

            for (int i = 0; i < 4; i++)
                collinearPoints[i] = ps[s[i]];

            Arrays.sort(collinearPoints);
            segmentList.add(new LineSegment(collinearPoints[0], collinearPoints[3]));
        }
    }

    public static void main(String[] args) {
        /* YOUR CODE HERE */

        // read the n points from a file
        In in = new In(args[0]);
        int n = in.readInt();
        Point[] points = new Point[n];
        for (int i = 0; i < n; i++) {
            int x = in.readInt();
            int y = in.readInt();
            points[i] = new Point(x, y);
        }

        // draw the points
        StdDraw.enableDoubleBuffering();
        StdDraw.setXscale(0, 32768);
        StdDraw.setYscale(0, 32768);
        for (Point p : points) {
            p.draw();
        }
        StdDraw.show();

        // print and draw the line segmentList
        BruteCollinearPoints collinear = new BruteCollinearPoints(points);
        for (LineSegment segment : collinear.segments()) {
            StdOut.println(segment);
            segment.draw();
        }
        StdDraw.show();
    }
}
