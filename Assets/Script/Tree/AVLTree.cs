using System;
using System.Collections.Generic;
using UnityEngine;

public class AVLTree<TKey, TValue> : BinarySearchTree<TKey, TValue> where TKey : IComparable<TKey>
{
    public AVLTree() : base()
    {

    }

    protected override TreeNode<TKey, TValue> Add(TreeNode<TKey, TValue> node, TKey key, TValue value)
    {
        base.Add(node, key, value);

        return Balance(node);
    }
    protected override TreeNode<TKey, TValue> AddOrUpdate(TreeNode<TKey, TValue> node, TKey key, TValue value)
    {
        base.AddOrUpdate(node, key, value);

        return Balance(node);
    }
    public override TreeNode<TKey, TValue> Remove(TreeNode<TKey, TValue> node, TKey key)
    {
        node = base.Remove(node, key);
        if(node == null)
        {
            return node;
        }

        return Balance(node);
    }

    protected int BalanceFactor(TreeNode<TKey, TValue> node)
    {
        return node == null? 0: Height(node.Left) - Height(node.Right);
    }

    protected TreeNode<TKey, TValue> Balance(TreeNode<TKey, TValue> node)
    {
        int balanceFactor = BalanceFactor(node);
        if(balanceFactor > 1)
        {
            if(BalanceFactor(node.Left) < 0)
            {
                node.Left = RotateLeft(node.Left);
            }
            return RotateRight(node);
        }

        if(balanceFactor < -1)
        {
            if (BalanceFactor(node.Right) > 0)
            {
                node.Right = RotateLeft(node.Right);
            }
            return RotateLeft(node);
        }

        return node; 
    }

    protected TreeNode<TKey, TValue> RotateRight(TreeNode<TKey, TValue> node)
    {
        var leftChild = node.Left;
        var rightSubtreeOfLeftChild = node.Right;

        leftChild.Right = node;
        node.Left = rightSubtreeOfLeftChild;

        UpdateHeight(node);
        UpdateHeight(leftChild);
        return leftChild;
    }
    protected TreeNode<TKey, TValue> RotateLeft(TreeNode<TKey, TValue> node)
    {
        var RightChild = node.Right;
        var rightSubtreeOfRightChild = node.Left;

        RightChild.Right = node;
        node.Right = rightSubtreeOfRightChild;

        UpdateHeight(node);
        UpdateHeight(RightChild);
        return RightChild;
    }
}
