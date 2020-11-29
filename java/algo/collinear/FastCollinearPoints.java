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

public class FastCollinearPoints {

    private final ArrayList<LineSegment> segments;

    public FastCollinearPoints(
            Point[] ps)     // finds all line segments containing 4 or more points
    {
        nullCheck(ps);
        Point[] points = ps.clone();
        Arrays.sort(points);
        dupCheck(points);

        segments = new ArrayList<LineSegment>();
        if (points.length < 4)
            return;

        ArrayList<Point> candidates = new ArrayList<Point>();
        ArrayList<Point> otherPoints = new ArrayList<Point>();

        for (int i = 0; i < points.length; i++) {
            Point origin = points[i];
            otherPoints.clear();
            candidates.clear();

            otherPoints.addAll(Arrays.asList(points));
            otherPoints.sort(origin.slopeOrder());

            candidates.add(otherPoints.get(1));

            for (int k = 2; k < otherPoints.size(); k++) {
                Point p = otherPoints.get(k);
                boolean sameSlope = origin.slopeTo(p) == origin.slopeTo(candidates.get(0));
                if (sameSlope)
                    candidates.add(p);
                if (!sameSlope || k == otherPoints.size() - 1) {
                    Point firstCand = candidates.get(0);
                    Point lastCand = candidates.get(candidates.size() - 1);
                    if (candidates.size() >= 3 && origin.compareTo(firstCand) < 0) {
                        segments.add(new LineSegment(origin, lastCand));
                    }

                    candidates.clear();
                    candidates.add(p);
                }
            }
        }
    }

    private void dupCheck(Point[] points) {
        for (int i = 0; i < points.length - 1; ++i) {
            if (points[i].compareTo(points[i + 1]) == 0) {
                throw new IllegalArgumentException("Contains a repeated point.");
            }
        }
    }

    private void nullCheck(Point[] ps) {
        if (ps == null)
            throw new IllegalArgumentException();
        for (int i = 0; i < ps.length; ++i)
            if (ps[i] == null) throw new IllegalArgumentException();
    }

    public int numberOfSegments()       // the number of line segments
    {
        return segments().length;
    }

    public LineSegment[] segments()     // the line segments
    {
        return segments.toArray(new LineSegment[0]);
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
        FastCollinearPoints collinear = new FastCollinearPoints(points);
        for (LineSegment segment : collinear.segments()) {
            StdOut.println(segment);
            segment.draw();
        }
        StdDraw.show();
    }
}
