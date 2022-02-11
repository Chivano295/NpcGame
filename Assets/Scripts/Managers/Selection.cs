using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using SimpleBehaviorTree.Examples;

public class Selection : MonoBehaviour
{
    [Header("Box Settings")]
    [SerializeField] private bool mouseDown = false;

    [SerializeField] private Vector3 holdPos = Vector3.zero;
    [SerializeField] private Vector3 movePos = Vector3.zero;

    [Header("Env")]
    [SerializeField] private GameObject agents;

    [Header("Epic")]
    public List<GameObject> SelectedAgents = new List<GameObject>();

    void Update()
    {
        BoxSelection();
        MoveAgents();
    }

    void BoxSelection()
    {
        bool mouseOne = Input.GetMouseButtonDown(0) ? true : false;
        bool mouseTwo = Input.GetMouseButton(0)     ? true : false;

        if (mouseOne)
            holdPos = Input.mousePosition;
        else if (mouseDown && !mouseTwo)
            ButtonReleased();

        if (mouseTwo)
            movePos = Input.mousePosition;

        mouseDown = mouseTwo;
    }

    void MoveAgents()
    {
        if (!Input.GetKeyDown(KeyCode.Space))
            return;

        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        Physics.Raycast(mouseRay, out hit, 99999);

        float sqrt = Mathf.Sqrt(SelectedAgents.Count);

        for (int i = 0; i < SelectedAgents.Count; i++)
        {
            GameObject agent = SelectedAgents[i];

            Vector3 position = new Vector3(sqrt % i / sqrt, 0, sqrt % i / sqrt);

            agent.GetComponent<AgentBrain>().ForceWalk(position);
        }
    }

    List<Transform> GetAgents() // Very ugly but whatever, it works ig :P
    {
        List<Transform> Agents = new List<Transform>();

        for (int a = 0; a < agents.transform.childCount; a++) 
            for (int b = 0; b < agents.transform.GetChild(a).childCount; b++)
                Agents.Add(agents.transform.GetChild(a).GetChild(b));

        return Agents;
    }

    bool IsInBox(Vector3 position)
    {
        Vector3 ok0 = holdPos - movePos;
        Vector3 ok1 = holdPos - position;

        if (Mathf.Abs(ok0.x) > Mathf.Abs(ok1.x) && Mathf.Abs(ok0.y) > Mathf.Abs(ok1.y) && ok1.z < 0)
            if (((ok0.x > 0 && ok1.x > 0) || (ok0.x < 0 && ok1.x < 0)) && ((ok0.y > 0 && ok1.y > 0) || ok0.y < 0 && ok1.y < 0))
                return true;

        return false;
    }

    void Highlight(bool enabled)
    {
        foreach (GameObject agent in SelectedAgents)
            agent.GetComponent<Outline>().enabled = enabled;
    }

    void ButtonReleased()
    {
        if ((holdPos - movePos).magnitude <= 15)
        {
            SingularSelect();
            return;
        }

        Highlight(false);

        SelectedAgents = new List<GameObject>();

        foreach (Transform agent in GetAgents())
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(agent.position);

            if (!agent.GetComponent<TeamBlue>())
                continue;
            else if (IsInBox(screenPos))
                SelectedAgents.Add(agent.gameObject);
        }

        Highlight(true);
    }

    void SingularSelect()
    {
        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        Highlight(false);

        if (Physics.Raycast(mouseRay, out hit, 99999))
            if (hit.transform.GetComponent<TeamBlue>())
                SelectedAgents = new List<GameObject>() { hit.transform.gameObject };
            else
                SelectedAgents.Clear();

        Highlight(true);
    }

    private void OnGUI()
    {
        if (!mouseDown)
            return;

        Rect rect = Support.GetScreenRect(holdPos, movePos);

        Support.DrawScreenRect(rect, new Color(0.8f, 0.8f, 0.8f, 0.15f));
        Support.DrawScreenRectBorder(rect, 1, Color.white);
    }
}
