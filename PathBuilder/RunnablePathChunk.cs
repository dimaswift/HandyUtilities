using UnityEngine;
using System.Collections;
using HandyUtilities;
using HandyUtilities.PoolSystem;

public class RunnablePathChunk : PooledObject<RunnablePathChunk>, IPathChunk
{
    [SerializeField]
    AreaPool m_pool;

    [SerializeField]
    Transform m_connector;

    public Transform[] playerWayPoints { get; private set; }

    public Transform connector { get { return m_connector; } }

    public override RunnablePathChunk Object { get { return this; } }

    public override PoolContainer<RunnablePathChunk> pool
    {
        get
        {
            return m_pool;
        }
        set
        {
            m_pool = value as AreaPool;
        }
    }

    public override void Init()
    {
        base.Init();
        var wp = transform.FindChild("playerWayPoints");
        if(wp == null)
        {
            playerWayPoints = new Transform[0];
            Debug.LogError(string.Format("{0}", "Add 'playerWayPoints' gameObject to the root and place waypoints there!"));
        }
        else playerWayPoints = wp.GetChildren();
    }

    public void OnConnect(IPathChunk connected)
    {

    }
}