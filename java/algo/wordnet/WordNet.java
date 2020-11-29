/* *****************************************************************************
 *  Name:
 *  Date:
 *  Description:
 **************************************************************************** */

import edu.princeton.cs.algs4.Bag;
import edu.princeton.cs.algs4.Digraph;
import edu.princeton.cs.algs4.In;
import edu.princeton.cs.algs4.StdOut;

import java.util.HashMap;

public class WordNet {

    private HashMap<String, Bag<Integer>> wordToIds;
    private HashMap<Integer, String> idToString;
    private Digraph net;
    private SAP sap;

    // constructor takes the name of the two input files
    public WordNet(String synsets, String hypernyms) {
        if (synsets == null || hypernyms == null) throw new IllegalArgumentException();

        wordToIds = new HashMap<String, Bag<Integer>>();
        idToString = new HashMap<Integer, String>();

        In in1 = null, in2 = null;
        try {
            in1 = new In(synsets);
            int n = 0;
            while (in1.hasNextLine()) {
                String[] fields = in1.readLine().split(",");
                int id = Integer.parseInt(fields[0]);
                n++;
                for (String s : fields[1].split(" ")) {
                    if (!wordToIds.containsKey(s))
                        wordToIds.put(s, new Bag<Integer>());
                    wordToIds.get(s).add(id);
                }
                idToString.put(id, fields[1]);
            }

            in2 = new In(hypernyms);
            net = new Digraph(n);
            while (in2.hasNextLine()) {
                String[] fields = in2.readLine().split(",");
                int v = Integer.parseInt(fields[0]);
                for (int i = 1; i < fields.length; i++)
                    net.addEdge(v, Integer.parseInt(fields[i]));
            }

            if (hasCycle() || multiRoot()) throw new IllegalArgumentException();

            sap = new SAP(net);
        }
        finally {
            if (in1 != null) in1.close();
            if (in2 != null) in2.close();
        }
    }

    // returns all WordNet nouns
    public Iterable<String> nouns() {
        return wordToIds.keySet();
    }

    // is the word a WordNet noun?
    public boolean isNoun(String word) {
        if (word == null)
            throw new IllegalArgumentException();
        return wordToIds.containsKey(word);
    }

    // distance between nounA and nounB (defined below)
    public int distance(String nounA, String nounB) {
        if (!wordToIds.containsKey(nounA) || !wordToIds.containsKey(nounB))
            throw new IllegalArgumentException();

        return sap.length(wordToIds.get(nounA), wordToIds.get(nounB));
    }

    // a synset (second field of synsets.txt) that is the common ancestor of nounA and nounB
    // in a shortest ancestral path (defined below)
    public String sap(String nounA, String nounB) {
        if (!wordToIds.containsKey(nounA) || !wordToIds.containsKey(nounB))
            throw new IllegalArgumentException();

        int sa = sap.ancestor(wordToIds.get(nounA), wordToIds.get(nounB));
        return idToString.get(sa);
    }

    private boolean hasCycleDfs(boolean[] marked, boolean[] onStack, int v) {
        onStack[v] = true;
        marked[v] = true;

        for (int w : net.adj(v)) {
            if (!marked[w]) {
                if (hasCycleDfs(marked, onStack, w)) return true;
            }
            else if (onStack[w]) return true;
        }
        onStack[v] = false;
        return false;
    }

    private boolean hasCycle() {
        int n = net.V();
        boolean[] marked = new boolean[n];
        boolean[] onStack = new boolean[n];
        for (int v = 0; v < n; v++) {
            if (marked[v]) continue;
            if (hasCycleDfs(marked, onStack, v)) {
                StdOut.printf("Has Cycle\n");
                return true;
            }
        }

        return false;
    }

    private boolean multiRoot() {
        int n = net.V();
        int nRoot = 0;
        for (int v = 0; v < n; v++) {
            if (!net.adj(v).iterator().hasNext())
                nRoot++;
            if (nRoot > 1) {
                StdOut.printf("Multi root\n");
                return true;
            }
        }

        return false;
    }
}
