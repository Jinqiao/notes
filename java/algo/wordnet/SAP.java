/* *****************************************************************************
 *  Name:
 *  Date:
 *  Description:
 **************************************************************************** */

import edu.princeton.cs.algs4.Digraph;
import edu.princeton.cs.algs4.In;
import edu.princeton.cs.algs4.Queue;
import edu.princeton.cs.algs4.StdIn;
import edu.princeton.cs.algs4.StdOut;

import java.util.HashMap;

public class SAP {
    private Digraph G;

    // constructor takes a digraph (not necessarily a DAG)
    public SAP(Digraph g) {
        G = new Digraph(g.V());
        for (int v = 0; v < g.V(); v++) {
            for (int w : g.adj(v))
                G.addEdge(v, w);
        }
    }

    // length of shortest ancestral path between v and w; -1 if no such path
    public int length(int v, int w) {
        int[] saResults = saLength(v, w);
        return saResults == null ? -1 : saResults[0];
    }

    // a common ancestor of v and w that participates in a shortest ancestral path; -1 if no such path
    public int ancestor(int v, int w) {
        int[] saResults = saLength(v, w);
        return saResults == null ? -1 : saResults[1];
    }

    // length of shortest ancestral path between any vertex in v and any vertex in w; -1 if no such path
    public int length(Iterable<Integer> vs, Iterable<Integer> ws) {
        if (vs == null || ws == null) throw new IllegalArgumentException();

        int minDist = Integer.MAX_VALUE;
        for (Integer v : vs) {
            for (Integer w : ws) {
                if (v == null || w == null) throw new IllegalArgumentException();
                int x = length(v, w);
                if (x != -1 && x < minDist) {
                    minDist = x;
                }
            }
        }

        return minDist == Integer.MAX_VALUE ? -1 : minDist;
    }

    // a common ancestor that participates in shortest ancestral path; -1 if no such path
    public int ancestor(Iterable<Integer> vs, Iterable<Integer> ws) {
        if (vs == null || ws == null) throw new IllegalArgumentException();

        int minDist = Integer.MAX_VALUE;

        int ca = 0;
        for (Integer v : vs) {
            for (Integer w : ws) {
                if (v == null || w == null) throw new IllegalArgumentException();
                int[] arr = saLength(v, w);
                if (arr != null && arr[0] < minDist) {
                    minDist = arr[0];
                    ca = arr[1];
                }
            }
        }

        return minDist == Integer.MAX_VALUE ? -1 : ca;
    }

    private HashMap<Integer, Integer> distFrom(int v) {
        Queue<Integer> q = new Queue<>();
        HashMap<Integer, Integer> distMap = new HashMap<>();
        q.enqueue(v);
        distMap.put(v, 0);
        while (!q.isEmpty()) {
            v = q.dequeue();
            for (int w : G.adj(v)) {
                if (distMap.containsKey(w)) continue;

                distMap.put(w, distMap.get(v) + 1);
                q.enqueue(w);
            }
        }
        return distMap;
    }

    private int[] saLength(int v, int w) {
        HashMap<Integer, Integer> vDist = distFrom(v);
        HashMap<Integer, Integer> wDist = distFrom(w);

        int minDist = Integer.MAX_VALUE;

        // swap
        if (vDist.size() > wDist.size()) {
            HashMap<Integer, Integer> tmp = vDist;
            vDist = wDist;
            wDist = tmp;
        }

        int ca = 0;
        for (int vt : vDist.keySet()) {
            if (wDist.containsKey(vt)) {
                int dist = vDist.get(vt) + wDist.get(vt);
                if (dist < minDist) {
                    minDist = dist;
                    ca = vt;
                }
            }
        }
        return minDist == Integer.MAX_VALUE ? null : new int[] { minDist, ca };
    }

    // do unit testing of this class
    public static void main(String[] args) {
        In in = new In(args[0]);
        Digraph G = new Digraph(in);
        SAP sap = new SAP(G);
        while (!StdIn.isEmpty()) {
            int v = StdIn.readInt();
            int w = StdIn.readInt();
            int length = sap.length(v, w);
            int ancestor = sap.ancestor(v, w);
            StdOut.printf("length = %d, ancestor = %d\n", length, ancestor);
        }
    }
}
