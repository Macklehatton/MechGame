using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vectrosity;

public class TargetBrackets
{
    public VectorLine topLeft;
    public VectorLine topRight;
    public VectorLine bottomLeft;
    public VectorLine bottomRight;

    public Camera camera;

    public TargetBrackets(float lineWidth)
    {
        topLeft = new VectorLine(
            "Top Left Bracket",
            new List<Vector2>(3),
            lineWidth,
            LineType.Continuous);

        topRight = new VectorLine(
            "Top Right Bracket",
            new List<Vector2>(3),
            lineWidth,
            LineType.Continuous);

        bottomLeft = new VectorLine(
            "Bottom Left Bracket",
            new List<Vector2>(3),
            lineWidth,
            LineType.Continuous);

        bottomRight = new VectorLine(
            "Bottom Right Bracket",
            new List<Vector2>(3),
            lineWidth,
            LineType.Continuous);

        this.camera = Camera.main;
    }

    public void SetBounds(CombatTarget target, float bracketLengthFactor)
    {
        if (target == null || target.gameObject == null)
        {
            return;
        }

        Collider targetCollider = target.gameObject.GetComponent<Collider>();

        if (targetCollider == null)
        {
            return;
        }

        if (CheckAngle(target.transform) == false)
        {
            SetActive(false);
            return;
        }

        SetActive(true);

        Rect targetRect = targetCollider.GetScreenRect();        

        Vector2 screenCenter = new Vector2(Screen.width / 2.0f, Screen.height / 2.0f);

        float bracketSize = Mathf.Min(targetRect.height * bracketLengthFactor, targetRect.width * bracketLengthFactor);

        topLeft.points2[0] = new Vector2(targetRect.min.x, targetRect.max.y - bracketSize);
        topLeft.points2[1] = new Vector2(targetRect.min.x, targetRect.max.y);
        topLeft.points2[2] = new Vector2(targetRect.min.x + bracketSize, targetRect.max.y);

        topRight.points2[0] = new Vector2(targetRect.max.x, targetRect.max.y - bracketSize);
        topRight.points2[1] = new Vector2(targetRect.max.x, targetRect.max.y);
        topRight.points2[2] = new Vector2(targetRect.max.x - bracketSize, targetRect.max.y);

        bottomLeft.points2[0] = new Vector2(targetRect.min.x, targetRect.min.y + bracketSize);
        bottomLeft.points2[1] = new Vector2(targetRect.min.x, targetRect.min.y);
        bottomLeft.points2[2] = new Vector2(targetRect.min.x + bracketSize, targetRect.min.y);

        bottomRight.points2[0] = new Vector2(targetRect.max.x, targetRect.min.y + bracketSize);
        bottomRight.points2[1] = new Vector2(targetRect.max.x, targetRect.min.y);
        bottomRight.points2[2] = new Vector2(targetRect.max.x - bracketSize, targetRect.min.y);

    }

    public bool CheckAngle(Transform target)
    {
        if (Vector3.Angle(camera.transform.forward, target.position - camera.transform.position) > 90.0f)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public void DrawLines()
    {
        topLeft.Draw();
        topRight.Draw();
        bottomLeft.Draw();
        bottomRight.Draw();
    }

    public void SetActive(bool active)
    {
        topLeft.active = active;
        topRight.active = active;
        bottomLeft.active = active;
        bottomRight.active = active;
    }   
}
