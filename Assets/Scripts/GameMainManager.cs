using System.Collections.Generic;
using Lean;
using RVO;
using UnityEngine;
using UnityEngine.Assertions;
using Vector2 = RVO.Vector2;

public class GameMainManager : SingletonBehaviour<GameMainManager>
{
    public GameObject agentPrefab;

    [HideInInspector] public Vector2 mousePosition;

    private Plane m_hPlane = new Plane(Vector3.forward, Vector3.zero);
    private Dictionary<int, GameAgent> m_agentMap = new Dictionary<int, GameAgent>();

    // Use this for initialization
    void Start()
    {
        //设置时步 越大越快
        Simulator.Instance.setTimeStep(0.25f);
        //设置Agent的默认属性
        float neighborDist = 15.0f;//检测与邻居避障的最大距离（要关心的范围），越大纳入思考的数据量越大，越小越不安全
        int maxNeighbors = 10;//检测与邻居避障的最大个数（要关心的其他单位），越大纳入思考的数据量越大，越小越不安全
        float timeHorizon = 5.0f; //预测提前规避时间, 提前得越多，速度变化越频繁
        float timeHorizonObst = 5.0f; //预测提前规避时间(针对固定障碍), 提前得越多，速度变化越频繁
        float radius = .25f;//阻挡半径
        float maxSpeed = 2.0f;//所能达到得最大速度
        Vector2 velocity = new Vector2(0.0f, 0.0f);//初始的 2元线性速度，影响出生单位时排挤其他的速度
        Simulator.Instance.setAgentDefaults(neighborDist, maxNeighbors, timeHorizon, timeHorizonObst,
            radius, maxSpeed, velocity);

        // add in awake
        Simulator.Instance.processObstacles();
    }

    private void UpdateMousePosition()
    {
        Vector3 position = Vector3.zero;
        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        float rayDistance;
        if (m_hPlane.Raycast(mouseRay, out rayDistance))
            position = mouseRay.GetPoint(rayDistance);

        mousePosition.x_ = position.x;
        mousePosition.y_ = position.y;
    }

    void DeleteAgent()
    {
        int agentNo = Simulator.Instance.queryNearAgent(mousePosition, 1.5f);
        if (agentNo == -1 || !m_agentMap.ContainsKey(agentNo))
            return;

        Simulator.Instance.delAgent(agentNo);
        LeanPool.Despawn(m_agentMap[agentNo].gameObject);
        m_agentMap.Remove(agentNo);
    }

    void CreatAgent()
    {
        int sid = Simulator.Instance.addAgent(mousePosition);
        if (sid >= 0)
        {
            GameObject go = LeanPool.Spawn(agentPrefab, new Vector3(mousePosition.x(), mousePosition.y(), 0), Quaternion.identity);
            GameAgent ga = go.GetComponent<GameAgent>();
            Assert.IsNotNull(ga);
            ga.sid = sid;
            m_agentMap.Add(sid, ga);
        }
    }

    // Update is called once per frame
    private void Update()
    {
        UpdateMousePosition();
        if (Input.GetMouseButtonUp(0))
        {
            if (Input.GetKey(KeyCode.Delete))
            {
                DeleteAgent();
            }
            else
            {
                CreatAgent();
            }
        }

        Simulator.Instance.doStep();
    }
}