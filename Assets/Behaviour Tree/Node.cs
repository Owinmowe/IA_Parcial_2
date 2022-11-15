using System.Collections.Generic;

public enum NodeState
{
    RUNNING,
    SUCCESS,
    FAILURE
}

public abstract class Node
{
    protected NodeState state;
    public Node parent;
    protected List<Node> childrens = new List<Node>();
    private Dictionary<string, object> data = new Dictionary<string, object>();
    public Node()
    {
        parent = null;
    }

    public Node(List<Node> childrens)
    {
        foreach (Node n in childrens)
        {
            Attach(n);
        }
    }

    public void Attach(Node node)
    {
        node.parent = this;
        childrens.Add(node);
    }

    public virtual NodeState Evaluate() => NodeState.FAILURE;

    public void SetData(string key, object value)
    {
        if (data.ContainsKey(key))
            data[key] = value;
        else
            data.Add(key, value);
    }

    public object GetData(string key) 
    {
        object value = null;
        if (data.TryGetValue(key, out value))
        {
            return value;
        }

        Node parentNode = parent;
        while (parentNode != null)
        {
            value = parentNode.GetData(key);
            if (value != null)
            {
                return value;
            }
            parentNode = parent;
        }
        return null;
    }

    public bool RemoveData(string key)
    {
        if (data.ContainsKey(key))
        {
            data.Remove(key);
            return true;
        }

        Node parentNode = parent;
        while (parentNode != null)
        {
            bool cleaned = parentNode.RemoveData(key);
            if (cleaned)
                return true;
            parentNode = parent;
        }
        return false;
    }
}

