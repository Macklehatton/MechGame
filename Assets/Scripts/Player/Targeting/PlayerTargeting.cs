using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Vectrosity;

public class PlayerTargeting : MonoBehaviour
{
    public CombatTarget currentTarget;
    private Vector3 pointerHitPosition;
    public Vector3 targetTrackingPosition;

    [SerializeField]
    private LayerMask targetLayer;
    [SerializeField]
    private string targetTag;
    [SerializeField]
    private float pollingRate;
    [SerializeField]
    private float maxDistance;
    [SerializeField]
    private Transform cameraRaycastOrigin;

    public List<CombatTarget> validTargets;
    private Collider[] colliderResults;

    private float timeSinceRefresh;
    private bool targetSelected;

    private int selectedTargetIndex;

    private void Start()
    {
        cameraRaycastOrigin = GetComponent<PlayerSetup>().cameraObject.transform;
        validTargets = new List<CombatTarget>();
        colliderResults = new Collider[50];
        targetSelected = false;
        
    }

    private void Update()
    {
        timeSinceRefresh += Time.deltaTime;
        GetPointerPosition();

        //if (targetSelected == false)
        //{
        //    if (timeSinceRefresh > pollingRate)
        //    {
        //        RefreshTargets();
        //    }
        //}

        HandleSelectTarget();

        TrackTarget();
    }

    private void HandleSelectTarget()
    {
        if (Input.GetKeyDown(KeyCode.Y) == true)
        {
            RefreshTargets();
            selectedTargetIndex += 1;
            if (selectedTargetIndex >= validTargets.Count)
            {
                selectedTargetIndex = 0;
            }
            currentTarget = validTargets[selectedTargetIndex];
        }

        if (Input.GetKeyDown(KeyCode.T) == true)
        {
            RefreshTargets();
            selectedTargetIndex = 0;
            currentTarget = validTargets[selectedTargetIndex];
        }

        if (Input.GetKeyDown(KeyCode.R) == true)
        {
            GetTargets();
            SortTargetsByAngle();
            if (validTargets.Count > 0)
            {
                currentTarget = validTargets[0];
            }
        }
    }

    private void TrackTarget()
    {
        if (currentTarget != null)
        {
            targetTrackingPosition = currentTarget.transform.GetComponentInChildren<TargetMarker>().transform.position;
        }
        else
        {
            targetTrackingPosition = pointerHitPosition;
        }
    }

    private void GetPointerPosition()
    {
        RaycastHit hit;
        if (Physics.Raycast(cameraRaycastOrigin.position, cameraRaycastOrigin.forward, out hit, maxDistance))
        {
            pointerHitPosition = hit.point;
        }
        else
        {
            pointerHitPosition = cameraRaycastOrigin.position + cameraRaycastOrigin.forward * 10000.0f;
        }
    }

    public void RefreshTargets()
    {
        GetTargets();
        SortTargetsByDistance();       
        timeSinceRefresh = 0.0f;
    }

    private void GetTargets()
    {
        validTargets.Clear();
        int collidersFound = Physics.OverlapSphereNonAlloc(transform.position, maxDistance, colliderResults, targetLayer.value);

        Vector3 direction;

        if (colliderResults == null)
        {
            return;
        }

        for (int i = 0; i < collidersFound; i++)
        {
            direction = colliderResults[i].ClosestPoint(cameraRaycastOrigin.position) - cameraRaycastOrigin.position;

            RaycastHit hit;
            if (Physics.Raycast(cameraRaycastOrigin.position, direction, out hit, maxDistance))
            {
                if (hit.collider == colliderResults[i])
                {
                    validTargets.Add(new CombatTarget(hit.transform.gameObject));
                }
            }
        }
    }

    private void SortTargetsByAngle()
    {
        if (validTargets.Count > 0)
        {
            validTargets = validTargets.OrderBy(
                x => Vector3.Angle(Camera.main.transform.forward, x.transform.GetComponentInChildren<TargetMarker>().transform.position - Camera.main.transform.position))
                .ToList();
        }
    }

    private void SortTargetsByDistance()
    {
        if (validTargets.Count > 0)
        {
            validTargets = validTargets.OrderBy(x => Vector3.Distance(cameraRaycastOrigin.position, x.transform.position)).ToList();
        }
    }
}
