using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BSTNode
{
    public PathNode pathNode;
    public BSTNode left;
    public BSTNode right;

    public BSTNode(PathNode pathNode)
    {
        this.pathNode = pathNode;
        this.left = null;
        this.right = null;
    }
}

public class BinarySearchTree
{
    private BSTNode root;

    public BinarySearchTree()
    {
        root = null;
    }

    public void Insert(PathNode pathNode)
    {
        /*
        * Inserts a PathNode into the BST sorted by F cost
        * Parameters:
        *      pathNode: PathNode to insert
        */
        root = InsertBST(root, pathNode);
    }

    public void Delete(PathNode pathNode)
    {
        /*
        * Deletes a PathNode from the BST
        * Parameters:
        *      pathNode: PathNode to delete
        */
        root = DeleteBST(root, pathNode);
    }

    public PathNode GetLowestFCostNode()
    {
        /*
        * Gets the node with the lowest F cost from the BST in O(log n) time
        * Returns: PathNode with lowest F cost
        */
        if (root == null) return null;
        
        BSTNode current = root;
        while (current.left != null)
        {
            current = current.left;
        }
        return current.pathNode;
    }

    public bool IsEmpty()
    {
        /*
        * Checks if the BST is empty
        * Returns: true if empty, false otherwise
        */
        return root == null;
    }

    private BSTNode InsertBST(BSTNode node, PathNode pathNode)
    {
        if (node == null)
        {
            return new BSTNode(pathNode);
        }

        if (pathNode.fCost < node.pathNode.fCost)
        {
            node.left = InsertBST(node.left, pathNode);
        }
        else
        {
            node.right = InsertBST(node.right, pathNode);
        }

        return node;
    }

    private BSTNode DeleteBST(BSTNode node, PathNode pathNode)
    {
        if (node == null) return null;

        if (pathNode.fCost < node.pathNode.fCost)
        {
            node.left = DeleteBST(node.left, pathNode);
        }
        else if (pathNode.fCost > node.pathNode.fCost)
        {
            node.right = DeleteBST(node.right, pathNode);
        }
        else if (node.pathNode == pathNode)
        {
            if (node.left == null) return node.right;
            if (node.right == null) return node.left;

            BSTNode minRight = node.right;
            while (minRight.left != null)
            {
                minRight = minRight.left;
            }
            node.pathNode = minRight.pathNode;
            node.right = DeleteBST(node.right, minRight.pathNode);
        }

        return node;
    }
}