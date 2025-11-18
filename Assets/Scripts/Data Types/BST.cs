using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BSTNode
{
    private PathNode pathNode;
    private BSTNode left;
    private BSTNode right;

    public BSTNode(PathNode pathNode)
    {
        this.pathNode = pathNode;
        this.left = null;
        this.right = null;
    }

    public PathNode getPathNode()
    {
        return pathNode;
    }

    public BSTNode getLeft()
    {
        return left;
    }

    public BSTNode getRight()
    {
        return right;
    }

    public void setLeft(BSTNode left)
    {
        this.left = left;
    }

    public void setRight(BSTNode right)
    {
        this.right = right;
    }

    public void setPathNode(PathNode pathNode)
    {
        this.pathNode = pathNode;
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
        while (current.getLeft() != null)
        {
            current = current.getLeft();
        }
        return current.getPathNode();
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

        if (pathNode.fCost < node.getPathNode().fCost)
        {
            node.setLeft(InsertBST(node.getLeft(), pathNode));
        }
        else
        {
            node.setRight(InsertBST(node.getRight(), pathNode));
        }

        return node;
    }

    private BSTNode DeleteBST(BSTNode node, PathNode pathNode)
    {
        if (node == null) return null;

        if (pathNode.fCost < node.getPathNode().fCost)
        {
            node.setLeft(DeleteBST(node.getLeft(), pathNode));
        }
        else if (pathNode.fCost > node.getPathNode().fCost)
        {
            node.setRight(DeleteBST(node.getRight(), pathNode));
        }
        else if (node.getPathNode() == pathNode)
        {
            if (node.getLeft() == null) return node.getRight();
            if (node.getRight() == null) return node.getLeft();

            BSTNode minRight = node.getRight();
            while (minRight.getLeft() != null)
            {
                minRight = minRight.getLeft();
            }
            node.setPathNode(minRight.getPathNode());
            node.setRight(DeleteBST(node.getRight(), minRight.getPathNode()));
        }

        return node;
    }
}