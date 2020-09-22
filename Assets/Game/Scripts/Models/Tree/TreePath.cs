using System.Collections.Generic;
using GT.Backgammon.AI;

public class TreePath<T> : IPath
{
    public List<TreeNode<T>> nodeList;

    public TreePath(TreeNode<T> topNode, TreeNode<T> bottonNode) : this()
    {
        if (topNode == bottonNode) return;

        nodeList.Add(bottonNode);
        TreeNode<T> tempNode = bottonNode.Parent;
        while (tempNode != topNode)
        {
            nodeList.Add(tempNode);
            tempNode = tempNode.Parent;
        }
    }

    public TreePath(TreeNode<T> node) : this()
    {
        nodeList.Add(node);
    }

    private TreePath()
    {
        nodeList = new List<TreeNode<T>>();
    }

    public T[] GetItemsFromPath()
    {
        T[] items = new T[nodeList.Count];
        for (int i = 0; i < nodeList.Count; i++)
        {
            items[i] = nodeList[i].Item;
        }
        return items;
    }

    public T[] GetReversedItemsFromPath()
    {
        T[] items = new T[nodeList.Count];
        for (int i = 0; i < nodeList.Count; i++)
        {
            items[i] = nodeList[nodeList.Count - 1 - i].Item;
        }
        return items;
    }

    public TreeNode<T> GetLastNode()
    {
        return nodeList[nodeList.Count - 1];
    }

    public override string ToString()
    {
        return nodeList.Display(" ==> ");
    }

    public static List<TreePath<T>> GetAllPaths(TreeNode<T> node)
    {
        List<TreePath<T>> paths = new List<TreePath<T>>();
        List<TreeNode<T>> leafs = node.GetAllLeafs();
        for (int i = 0; i < leafs.Count; i++)
        {
            TreePath<T> path = new TreePath<T>(node, leafs[i]);
            path.nodeList.Reverse();
            paths.Add(path);
        }
        return paths;
    }
}
