/* *****************************************************************************
 *  Name:
 *  Date:
 *  Description:
 **************************************************************************** */

import edu.princeton.cs.algs4.In;
import edu.princeton.cs.algs4.StdOut;

public class Outcast {

    private WordNet wordnet;

    // constructor takes a WordNet object
    public Outcast(WordNet wordnet) {
        this.wordnet = wordnet;
    }

    // given an array of WordNet nouns, return an outcast
    public String outcast(String[] nouns) {
        if (nouns == null) throw new IllegalArgumentException();

        int max = Integer.MIN_VALUE;
        String oc = null;

        for (int i = 0; i < nouns.length; i++) {
            int dist = 0;
            for (int j = 0; j < nouns.length; j++) {
                if (i == j) continue;
                dist += wordnet.distance(nouns[i], nouns[j]);
            }
            if (dist > max) {
                max = dist;
                oc = nouns[i];
            }
        }

        return oc;
    }

    public static void main(String[] args) {
        WordNet wordnet = new WordNet(args[0], args[1]);

        // String x = wordnet.sap("coffee", "tea");
        // StdOut.printf("sap is %s\n", x);

        Outcast outcast = new Outcast(wordnet);
        for (int t = 2; t < args.length; t++) {
            In in = new In(args[t]);
            String[] nouns = in.readAllStrings();
            StdOut.println(args[t] + ": " + outcast.outcast(nouns));
        }
    }
}
