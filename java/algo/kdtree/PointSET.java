/* *****************************************************************************
 *  Name:
 *  Date:
 *  Description:
 **************************************************************************** */

import edu.princeton.cs.algs4.Point2D;
import edu.princeton.cs.algs4.RectHV;
import edu.princeton.cs.algs4.SET;
import edu.princeton.cs.algs4.StdDraw;

public class PointSET {

    private SET<Point2D> points;

    // construct an empty set of points
    public PointSET() {
        points = new SET<Point2D>();
    }

    // is the set empty?
    public boolean isEmpty() {
        return points.isEmpty();
    }

    // number of points in the set
    public int size() {
        return points.size();
    }

    // add the point to the set (if it is not already in the set)
    public void insert(Point2D p) {
        nullCheck(p);
        points.add(p);
    }

    // does the set contain point p?
    public boolean contains(Point2D p) {
        nullCheck(p);
        return points.contains(p);
    }

    // draw all points to standard draw
    public void draw() {
        for (Point2D p : points) {
            StdDraw.point(p.x(), p.y());
        }
    }

    // all points that are inside the rectangle (or on the boundary)
    public Iterable<Point2D> range(RectHV rect) {
        nullCheck(rect);
        SET<Point2D> ps = new SET<Point2D>();

        for (Point2D p : points) {
            if (rect.contains(p)) {
                ps.add(p);
            }
        }

        return ps;
    }

    // a nearest neighbor in the set to point p; null if the set is empty
    public Point2D nearest(Point2D p0) {
        nullCheck(p0);

        double nd = 0;
        Point2D np = null;

        for (Point2D p : points) {
            if (np == null) {
                np = p;
                nd = p.distanceSquaredTo(p0);
            }
            else {
                double dist = p.distanceSquaredTo(p0);

                if (dist < nd) {
                    nd = dist;
                    np = p;
                }
            }
        }

        return np;
    }

    private void nullCheck(Object obj) {
        if (obj == null) {
            throw new IllegalArgumentException();
        }
    }
}
