/* *****************************************************************************
 *  Name:
 *  Date:
 *  Description:
 **************************************************************************** */

import edu.princeton.cs.algs4.FlowEdge;
import edu.princeton.cs.algs4.FlowNetwork;
import edu.princeton.cs.algs4.FordFulkerson;
import edu.princeton.cs.algs4.In;
import edu.princeton.cs.algs4.StdOut;

import java.util.Arrays;
import java.util.HashMap;
import java.util.HashSet;

public class BaseballElimination {
    private int n;
    private String[] teams;
    private HashMap<String, Integer> teamToIdx;
    private int[] wins;
    private int[] losses;
    private int[] remains;
    private int[][] games;

    private boolean[] eliminations;
    private HashMap<String, HashSet<String>> certificates;

    private int nGameVertices;

    // create a baseball division from given filename in format specified below
    public BaseballElimination(String filename) {
        if (filename == null) throw new IllegalArgumentException();
        parseInput(filename);

        nGameVertices = (n - 1) * (n - 2) / 2;

        eliminations = new boolean[n];
        certificates = new HashMap<>();
        for (int i = 0; i < n; i++) {
            checkElimination(i);
        }
    }

    private void checkElimination(int x) {
        int totVertices = nGameVertices + (n - 1) + 2;
        // StdOut.printf("totVertices = %d\n", totVertices);
        // StdOut.printf("x = %d, teams[x] = %s\n", x, teams[x]);

        FlowNetwork graph = new FlowNetwork(totVertices);
        int s = 0;
        int t = totVertices - 1;
        int totGames = 0;

        // 1 to c(n-1, 2) are games, c(n-1, 2) + 1 to t-1 are teams
        int v = 1;
        for (int i = 0; i < n - 1; i++) {
            for (int j = i + 1; j < n; j++) {
                if (i == x || j == x) continue;
                graph.addEdge(new FlowEdge(s, v, games[i][j]));
                graph.addEdge(new FlowEdge(v, teamIdx(i, x), Integer.MAX_VALUE));
                graph.addEdge(new FlowEdge(v, teamIdx(j, x), Integer.MAX_VALUE));

                // StdOut.printf("a v = %d, teamIdx(%d) = %d, teamIdx(%d) = %d, games[i][j]=%d\n", v,
                //               i, teamIdx(i, x), j, teamIdx(j, x), games[i][j]);

                totGames += games[i][j];
                v++;
            }
        }

        // StdOut.printf("v = %d\n", v);

        for (int i = 0; i < n; i++) {
            if (i == x) continue;
            int cap = wins[x] + remains[x] - wins[i];
            if (cap < 0) {
                eliminations[x] = true;
                if (!certificates.containsKey(teams[x]))
                    certificates.put(teams[x], new HashSet<>());
                certificates.get(teams[x]).add(teams[i]);
            }
            else graph.addEdge(new FlowEdge(teamIdx(i, x), t, cap));

            // StdOut.printf("b teamIdx(%d) = %d, t = %d, cap = %d\n", i, teamIdx(i, x), t, cap);
        }

        if (eliminations[x]) return;

        FordFulkerson ff = new FordFulkerson(graph, s, t);
        // StdOut.println(graph.toString());
        boolean sNotFull = ((int) (ff.value())) < totGames;
        eliminations[x] = sNotFull;

        if (eliminations[x]) {
            certificates.put(teams[x], new HashSet<String>());
            for (int i = 0; i < n; i++) {
                if (i == x) continue;
                if (ff.inCut(teamIdx(i, x))) certificates.get(teams[x]).add(teams[i]);
            }
        }
    }

    private int teamIdx(int i, int x) {
        int idx = nGameVertices + i + 1;
        if (i > x) idx--;
        return idx;
    }

    private void parseInput(String filename) {
        In in = null;
        try {
            in = new In(filename);
            n = Integer.parseInt(in.readLine());

            teams = new String[n];
            wins = new int[n];
            losses = new int[n];
            remains = new int[n];
            games = new int[n][n];
            teamToIdx = new HashMap<>();

            int i = 0;
            while (in.hasNextLine()) {
                String[] fields = in.readLine().trim().split("\\s+");
                // for (int j = 0; j < fields.length; j++) {
                //     StdOut.printf("j = %d, fields[j] = %s\n", j, fields[j]);
                // }

                teams[i] = fields[0];
                teamToIdx.put(fields[0], i);
                wins[i] = Integer.parseInt(fields[1]);
                losses[i] = Integer.parseInt(fields[2]);
                remains[i] = Integer.parseInt(fields[3]);
                for (int j = 0; j < n; j++) {
                    games[i][j] = Integer.parseInt(fields[4 + j]);
                }
                i++;
            }
        }
        finally {
            if (in != null) in.close();
        }
    }

    // number of teams
    public int numberOfTeams() {
        return n;
    }

    // all teams
    public Iterable<String> teams() {
        return Arrays.asList(teams);
    }

    // number of wins for given team
    public int wins(String team) {
        if (!teamToIdx.containsKey(team)) throw new IllegalArgumentException();
        return wins[teamToIdx.get(team)];
    }

    // number of losses for given team
    public int losses(String team) {
        if (!teamToIdx.containsKey(team)) throw new IllegalArgumentException();
        return losses[teamToIdx.get(team)];
    }

    // number of remaining games for given team
    public int remaining(String team) {
        if (!teamToIdx.containsKey(team)) throw new IllegalArgumentException();
        return remains[teamToIdx.get(team)];
    }

    // number of remaining games between team1 and team2
    public int against(String team1, String team2) {
        if (!teamToIdx.containsKey(team1) || !teamToIdx.containsKey(team2))
            throw new IllegalArgumentException();
        return games[teamToIdx.get(team1)][teamToIdx.get(team2)];
    }

    // is given team eliminated?
    public boolean isEliminated(String team) {
        if (!teamToIdx.containsKey(team)) throw new IllegalArgumentException();
        return eliminations[teamToIdx.get(team)];
    }

    // subset R of teams that eliminates given team; null if not eliminated
    public Iterable<String> certificateOfElimination(String team) {
        if (!teamToIdx.containsKey(team)) throw new IllegalArgumentException();

        if (!isEliminated(team)) return null;
        return certificates.get(team);
    }

    public static void main(String[] args) {
        BaseballElimination division = new BaseballElimination(args[0]);
        for (String team : division.teams()) {
            if (division.isEliminated(team)) {
                StdOut.print(team + " is eliminated by the subset R = { ");
                for (String t : division.certificateOfElimination(team)) {
                    StdOut.print(t + " ");
                }
                StdOut.println("}");
            }
            else {
                StdOut.println(team + " is not eliminated");
            }
        }
    }
}
