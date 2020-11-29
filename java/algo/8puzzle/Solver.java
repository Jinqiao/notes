/* *****************************************************************************
 *  Name:
 *  Date:
 *  Description:
 **************************************************************************** */

import edu.princeton.cs.algs4.In;
import edu.princeton.cs.algs4.MinPQ;
import edu.princeton.cs.algs4.StdOut;

import java.util.LinkedList;

public class Solver {

    private boolean isSolvable;
    private SearchNode solutionNode;

    // find a solution to the initial board (using the A* algorithm)
    public Solver(Board initial) {
        if (initial == null) {
            throw new IllegalArgumentException();
        }

        solutionNode = null;
        MinPQ<SearchNode> minPQ = new MinPQ<>();
        minPQ.insert(new SearchNode(initial, 0, null));

        while (true) {
            SearchNode currNode = minPQ.delMin();
            Board currBoard = currNode.board;

            if (currBoard.isGoal()) {
                isSolvable = true;
                solutionNode = currNode;
                break;
            }

            if (currBoard.manhattan() == 2 && currBoard.twin().isGoal()) {
                isSolvable = false;
                break;
            }

            int moves = currNode.moves;
            Board prevBoard = moves > 0 ? currNode.prev.board : null;

            for (Board b : currBoard.neighbors()) {
                if (b.equals(prevBoard)) {
                    continue;
                }
                minPQ.insert(new SearchNode(b, moves + 1, currNode));
            }
        }

    }

    // is the initial board solvable? (see below)
    public boolean isSolvable() {
        return isSolvable;
    }

    // min number of moves to solve initial board; -1 if unsolvable
    public int moves() {
        return isSolvable() ? solutionNode.moves : -1;
    }

    // sequence of boards in a shortest solution; null if unsolvable
    public Iterable<Board> solution() {
        if (!isSolvable) return null;

        LinkedList<Board> solution = new LinkedList<Board>();
        SearchNode node = solutionNode;

        while (node != null) {
            solution.addFirst(node.board);
            node = node.prev;
        }

        return solution;
    }

    private class SearchNode implements Comparable<SearchNode> {

        private final SearchNode prev;
        private final Board board;
        private final int moves;
        private final int priority;

        SearchNode(Board board, int moves, SearchNode prev) {
            this.board = board;
            this.moves = moves;
            this.prev = prev;
            this.priority = board.manhattan() + moves;
        }

        @Override
        public int compareTo(SearchNode that) {
            return this.priority() - that.priority();
        }

        public int priority() {
            return this.priority;
        }
    }

    // test client (see below)
    public static void main(String[] args) {

        // create initial board from file
        In in = new In(args[0]);
        int n = in.readInt();
        int[][] tiles = new int[n][n];
        for (int i = 0; i < n; i++)
            for (int j = 0; j < n; j++)
                tiles[i][j] = in.readInt();
        Board initial = new Board(tiles);

        // solve the puzzle
        Solver solver = new Solver(initial);

        // print solution to standard output
        if (!solver.isSolvable())
            StdOut.println("No solution possible");
        else {
            StdOut.println("Minimum number of moves = " + solver.moves());
            for (Board board : solver.solution())
                StdOut.println(board);
        }
    }

}
