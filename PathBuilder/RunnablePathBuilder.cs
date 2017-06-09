using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using HandyUtilities;

public class RunnablePathBuilder : PathBuilder<RunnablePathChunk>
{

    public float playerSpeed = 10;
    public Transform player;
    public List<RunnablePathChunk> areas { get { return m_chunkPool; } }

    int m_currenPlayerWaypoint;

    public bool movePlayer { get; set; }

    public override void Init()
    {
        base.Init();
        Build();
        var area = areas[0];
        player.position = area.playerWayPoints[0].position;
        movePlayer = true;
    }

    protected virtual void MovePlayer()
    {
        if (!movePlayer) return;
        var area = areas[currentChunkIndex];
        var playerPos = player.position;
        var targetWayPoint = area.playerWayPoints[m_currenPlayerWaypoint];
        var distanceToTarget = Vector3.Distance(playerPos, targetWayPoint.position);
        if (distanceToTarget < .1f)
        {
            if (m_currenPlayerWaypoint < area.playerWayPoints.Length - 1)
            {
                m_currenPlayerWaypoint++;
            }
            else MoveToNextArea();

        }
        player.position = Vector3.MoveTowards(playerPos, targetWayPoint.position, Time.deltaTime * playerSpeed);
        player.LookAt(targetWayPoint, Vector3.up);
    }

    public override void Build()
    {
        base.Build();
        m_currenPlayerWaypoint = 0;
    }

    void MoveToNextArea()
    {
        m_currenPlayerWaypoint = 0;
        OnChunkPassed();
    }

}