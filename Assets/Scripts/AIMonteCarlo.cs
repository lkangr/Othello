using System;
using System.Collections.Generic;
using System.Linq;

public class MCTSNode
{
    public MCTSNode parent;
    public Position move;
    public int val;
    public int visit;
    public float uct;
    public List<MCTSNode> children = null;

    public GameState gameState;

    public MCTSNode(GameState gs, MCTSNode pr, Position mv)
    {
        gameState = gs;
        parent = pr;
        move = mv;

        val = 0;
        visit = 0;
        uct = 9999;
    }

    public void CalculationUCT()
    {
        if (parent == null || visit == 0) uct = 9999;
        else
            uct = val + MathF.Sqrt(2) * MathF.Sqrt(MathF.Log(parent.visit) / visit);
    }

    public void CreateChildren()
    {
        children = new List<MCTSNode>();
        foreach (var pos in gameState.LegalMoves.Keys)
        {
            GameState newGameState = gameState.Clone();
            newGameState.MakeMove(pos, out MoveInfo moveInfo);
            children.Add(new MCTSNode(newGameState, this, pos));
        }
    }
}

public class AIMonteCarlo
{
    public static int iteration = 1000;

    private static Random rnd = new Random();

    public static Position CalculationNextMove(GameState gameState)
    {
        MCTSNode root = new MCTSNode(gameState, null, null);
        root.CreateChildren();
        if (root.children.Count == 1) return root.children[0].move;

        for (int i = 0; i < iteration; i++)
        {
            SelectNode(root);
        }

        MCTSNode selectedNode = null;
        foreach (var child in root.children)
        {
            if (selectedNode == null) selectedNode = child;
            else if (selectedNode.uct < child.uct) selectedNode = child;
        }
        return selectedNode.move;
    }

    private static int SelectNode(MCTSNode node)
    {
        if (node.gameState.GameOver)
        {
            int result = 0;
            if (node.gameState.Winner == Player.None) result = 0;
            else if (node.gameState.Winner == node.parent.gameState.CurrentPlayer) result = 1;
            else result = -1;

            node.visit++;
            node.val += result;
            if (node.parent.parent == null) return 0;
            return node.parent.gameState.CurrentPlayer == node.parent.parent.gameState.CurrentPlayer ? result : -result;
        }

        if (node.parent != null && node.visit == 0)
        {
            int result = SimulateNode(node);
            node.visit++;
            node.val += result;
            if (node.parent.parent == null) return 0;
            return node.parent.gameState.CurrentPlayer == node.parent.parent.gameState.CurrentPlayer ? result : -result;
        }
        else
        {
            if (node.children == null) node.CreateChildren();
            MCTSNode selectedNode = null;
            foreach (var child in node.children)
            {
                if (selectedNode == null) selectedNode = child;
                else if (selectedNode.uct < child.uct) selectedNode = child;
            }

            int result = SelectNode(selectedNode);
            node.visit++;
            node.val += result;
            node.children.ForEach(ele => ele.CalculationUCT());
            if (node.parent.parent == null) return 0;
            return node.parent.gameState.CurrentPlayer == node.parent.parent.gameState.CurrentPlayer ? result : -result;
        }
    }

    private static int SimulateNode(MCTSNode node)
    {
        while (!node.gameState.GameOver)
        {
            var keys = node.gameState.LegalMoves.Keys.ToList();
            node.gameState.MakeMove(keys[rnd.Next(keys.Count)], out MoveInfo moveInfo);
        }

        if (node.gameState.Winner == Player.None) return 0;
        else if (node.gameState.Winner == node.parent.gameState.CurrentPlayer) return 1;
        else return -1;
    }
}
