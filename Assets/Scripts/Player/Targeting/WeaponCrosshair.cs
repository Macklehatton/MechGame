using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vectrosity;

public class WeaponCrosshair
{
    private Transform weaponTransform;
    private List<VectorLine> crossHairLines;
    private Vector2 screenCenter;
    private Camera camera;

    public WeaponCrosshair(Transform weaponTransform, float width)
    {
        this.weaponTransform = weaponTransform;
        crossHairLines = new List<VectorLine>();

        crossHairLines.Add(new VectorLine("Crosshair", new List<Vector2>(2), width));
        crossHairLines.Add(new VectorLine("Crosshair", new List<Vector2>(2), width));
        crossHairLines.Add(new VectorLine("Crosshair", new List<Vector2>(2), width));
        crossHairLines.Add(new VectorLine("Crosshair", new List<Vector2>(2), width));

        camera = Camera.main;
    }

    public void UpdateCrosshair(float length, float width, float separation, Vector3 raycastHit)
    {
        if (CheckAngle(weaponTransform) == false)
        {
            SetActive(false);
            return;
        }
        
        SetActive(true);

        Vector2 crossHairPosition = camera.WorldToScreenPoint(raycastHit);

        crossHairLines[0].points2[0] = crossHairPosition + new Vector2(-separation, 0.0f);
        crossHairLines[0].points2[1] = crossHairPosition + new Vector2(-separation - length, 0.0f);

        crossHairLines[1].points2[0] = crossHairPosition + new Vector2(0.0f, separation);
        crossHairLines[1].points2[1] = crossHairPosition + new Vector2(0.0f, separation + length);

        crossHairLines[2].points2[0] = crossHairPosition + new Vector2(separation, 0.0f);
        crossHairLines[2].points2[1] = crossHairPosition + new Vector2(separation + length, 0.0f);

        crossHairLines[3].points2[0] = crossHairPosition + new Vector2(0.0f, -separation);
        crossHairLines[3].points2[1] = crossHairPosition + new Vector2(0.0f, -separation - length);

        foreach (VectorLine vectorLine in crossHairLines)
        {
            vectorLine.Draw();
        }
    }

    public void DrawCentered(float length, float width, float separation)
    {
        if (CheckAngle(weaponTransform) == false)
        {
            SetActive(false);
            return;
        }

        SetActive(true);

        screenCenter = new Vector2(Screen.width / 2.0f, Screen.height / 2.0f);

        crossHairLines[0].points2[0] = screenCenter + new Vector2(-separation, 0.0f);
        crossHairLines[0].points2[1] = screenCenter + new Vector2(-separation - length, 0.0f);

        crossHairLines[1].points2[0] = screenCenter + new Vector2(0.0f, separation);
        crossHairLines[1].points2[1] = screenCenter + new Vector2(0.0f, separation + length);

        crossHairLines[2].points2[0] = screenCenter + new Vector2(separation, 0.0f);
        crossHairLines[2].points2[1] = screenCenter + new Vector2(separation + length, 0.0f);

        crossHairLines[3].points2[0] = screenCenter + new Vector2(0.0f, -separation);
        crossHairLines[3].points2[1] = screenCenter + new Vector2(0.0f, -separation - length);

        foreach (VectorLine vectorLine in crossHairLines)
        {
            vectorLine.Draw();
        }
    }

    public void DrawSetRange(float length, float width, float separation, float range)
    {
        if (CheckAngle(weaponTransform) == false)
        {
            SetActive(false);
            return;
        }

        SetActive(true);

        Vector2 crossHairPosition = camera.WorldToScreenPoint(weaponTransform.position + weaponTransform.forward * range);

        crossHairLines[0].points2[0] = crossHairPosition + new Vector2(-separation, 0.0f);
        crossHairLines[0].points2[1] = crossHairPosition + new Vector2(-separation - length, 0.0f);

        crossHairLines[1].points2[0] = crossHairPosition + new Vector2(0.0f, separation);
        crossHairLines[1].points2[1] = crossHairPosition + new Vector2(0.0f, separation + length);

        crossHairLines[2].points2[0] = crossHairPosition + new Vector2(separation, 0.0f);
        crossHairLines[2].points2[1] = crossHairPosition + new Vector2(separation + length, 0.0f);

        crossHairLines[3].points2[0] = crossHairPosition + new Vector2(0.0f, -separation);
        crossHairLines[3].points2[1] = crossHairPosition + new Vector2(0.0f, -separation - length);

        foreach (VectorLine vectorLine in crossHairLines)
        {
            vectorLine.Draw();
        }
    }

    public void SetActive(bool active)
    {
        foreach (VectorLine vectorLine in crossHairLines)
        {
            vectorLine.active = active;
        }
    }

    public bool CheckAngle(Transform weaponTransform)
    {
        if (Vector3.Angle(camera.transform.forward, weaponTransform.forward) > 90.0f)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}
