/* *****************************************************************************
 *  Name:
 *  Date:
 *  Description:
 **************************************************************************** */

import edu.princeton.cs.algs4.Point2D;
import edu.princeton.cs.algs4.RectHV;
import edu.princeton.cs.algs4.StdDraw;

import java.awt.Color;
import java.util.ArrayList;

public class KdTree {

    private Node root;
    private int n;

    // construct an empty set of points
    public KdTree() {

    }

    // is the set empty?
    public boolean isEmpty() {
        return root == null;
    }

    // number of points in the set
    public int size() {
        return n;
    }

    // add the point to the set (if it is not already in the set)
    public void insert(Point2D p) {
        nullCheck(p);
        put(p);
    }

    // does the set contain point p?
    public boolean contains(Point2D p) {
        nullCheck(p);
        if (isEmpty()) return false;

        return get(p) != null;
    }

    // draw all of the points to standard draw in black
    // and the subdivisions in red (for vertical splits) and blue (for horizontal splits)
    public void draw() {
        draw(root);
    }

    private void draw(Node x) {
        if (x == null) return;

        Point2D p = x.point;
        StdDraw.setPenColor(Color.BLACK);
        StdDraw.setPenRadius(0.01);
        StdDraw.point(p.x(), p.y());

        StdDraw.setPenRadius(0.002);
        RectHV rect = x.rect;

        // StdOut.println(String.format("Draw node: %f, %f, isH: %b", p.x(), p.y(), x.isH));
        // StdOut.println(
        //         String.format("Rect: (%f, %f) -> (%f, %f)", rect.xmin(), rect.ymin(), rect.xmax(),
        //                       rect.ymax()));

        if (x.isH) {
            StdDraw.setPenColor(Color.BLUE);
            StdDraw.line(rect.xmin(), p.y(), rect.xmax(), p.y());
        }
        else {
            StdDraw.setPenColor(Color.RED);
            StdDraw.line(p.x(), rect.ymin(), p.x(), rect.ymax());
        }

        draw(x.leftBot);
        draw(x.rightTop);
    }

    // all points that are inside the rectangle (or on the boundary)
    public Iterable<Point2D> range(RectHV rect) {
        nullCheck(rect);
        ArrayList<Point2D> ps = new ArrayList<Point2D>();
        searchRange(rect, root, ps);
        return ps;
    }

    private void searchRange(RectHV rect, Node x, ArrayList<Point2D> ps) {
        if (x == null) return;
        if (rect.contains(x.point)) {
            ps.add(x.point);
            searchRange(rect, x.leftBot, ps);
            searchRange(rect, x.rightTop, ps);
            return;
        }

        if (x.leftBotContains(rect.xmin(), rect.ymin())) {
            searchRange(rect, x.leftBot, ps);
        }

        if (!x.leftBotContains(rect.xmax(), rect.ymax())) {
            searchRange(rect, x.rightTop, ps);
        }
    }

    // a nearest neighbor in the set to point p; null if the set is empty
    public Point2D nearest(Point2D p0) {
        nullCheck(p0);
        if (isEmpty()) return null;
        return getNearest(p0, root.point, root);
    }

    private Point2D getNearest(Point2D p0, Point2D np, Node x) {
        if (x == null) return np;

        double nd = np.distanceSquaredTo(p0);
        if (x.rect.distanceSquaredTo(p0) < nd) {
            double dist = x.point.distanceSquaredTo(p0);
            if (dist < nd) np = x.point;

            if (x.leftBotContains(p0.x(), p0.y())) {
                np = getNearest(p0, np, x.leftBot);
                np = getNearest(p0, np, x.rightTop);
            }
            else {
                np = getNearest(p0, np, x.rightTop);
                np = getNearest(p0, np, x.leftBot);
            }
        }

        return np;
    }

    private void nullCheck(Object obj) {
        if (obj == null) {
            throw new IllegalArgumentException();
        }
    }


    private class Node {
        private Point2D point;
        private RectHV rect;
        private Node leftBot, rightTop;
        private boolean isH;

        public Node(Point2D p, RectHV rect, boolean isH) {
            point = p;
            this.isH = isH;
            this.rect = rect;
        }

        public boolean leftBotContains(double x, double y) {
            if (isH) return y <= point.y();
            else return x <= point.x();
        }
    }

    private void put(Point2D p) {
        root = put(root, p, 0, 0, 1, 1, false);
    }


    private Node put(Node x, Point2D p, double x1, double y1, double x2, double y2, boolean isH) {
        if (x == null) {
            n++;
            return new Node(p, new RectHV(x1, y1, x2, y2), isH);
        }

        if (x.point.equals(p)) return x;

        if (x.isH) {
            if (p.y() <= x.point.y())
                x.leftBot = put(x.leftBot, p, x1, y1, x2, x.point.y(), false);
            else
                x.rightTop = put(x.rightTop, p, x1, x.point.y(), x2, y2, false);
        }
        else {
            if (p.x() <= x.point.x())
                x.leftBot = put(x.leftBot, p, x1, y1, x.point.x(), y2, true);
            else
                x.rightTop = put(x.rightTop, p, x.point.x(), y1, x2, y2, true);
        }

        return x;
    }

    private Node get(Point2D p) {
        Node x = root;
        while (x != null) {
            if (x.point.equals(p)) return x;

            if (x.isH) {
                if (p.y() <= x.point.y()) x = x.leftBot;
                else x = x.rightTop;
            }
            else {
                if (p.x() <= x.point.x()) x = x.leftBot;
                else x = x.rightTop;
            }
        }

        return null;
    }
}
