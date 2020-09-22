using UnityEngine;
using System.Collections.Generic;
using System;

public class TreeNode<T>
{
    public TreeNode<T> Parent { get; private set; }

    public List<TreeNode<T>> children;

    public T Item;
	
    public TreeNode(T item)
    {
        children = new List<TreeNode<T>>();
        Item = item;
    }

    public void AddChild(TreeNode<T> node)
    {
        node.Parent = this;
        children.Add(node);
    }

    public void RemoveChild(TreeNode<T> node)
    {
        children.Remove(node);
        node.Parent = null;
    }

    public int MaxDepth(int currDepth = 0)
    {
        int max = currDepth;
        for (int i = 0; i < children.Count; i++)
        {
            max = Mathf.Max(max, children[i].MaxDepth(currDepth + 1));
        }
        return max;
    }

    public int NumberOfLeafs(int current = 0)
    {
        for (int i = 0; i < children.Count; i++)
        {
            current = Mathf.Max(current, children[i].NumberOfLeafs(current + 1));
        }
        return current;
    }

    public int DepthFromLeaf()
    {
        if (Parent == null)
            return 1;
        else return 1 + Parent.DepthFromLeaf();
    }

    public void RemoveShallowBranches(int minDepth)
    {
        for (int i = children.Count - 1; i >= 0; i--)
        {
            children[i].RemoveShallowBranches(minDepth - 1);
        }
        if (minDepth > 0 && children.Count == 0 && Parent != null)
        {
            Parent.RemoveChild(this);
        }
    }

    public List<TreeNode<T>> GetAllLeafs()
    {
        List<TreeNode<T>> list = new List<TreeNode<T>>();
        if (children.Count == 0)
        {
            list.Add(this);
        }
        else
        {
            for (int i = 0; i < children.Count; i++)
            {
                list.AddRange(children[i].GetAllLeafs());
            }
        }
        return list;
    }

    public override string ToString()
    {
        return ToString(0);
    }

    private string ToString(int depth)
    {
        string str = depth > 0 ? "[" + depth + "] ==> " + Item.ToString() + "\n" : string.Empty;
        for (int i = 0; i < children.Count; i++)
        {
            str += children[i].ToString(depth + 1);
        }
        return str;
    }
}
