using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;


    public class BuildingMod : MonoBehaviour
    {
        [Header("KeyBinds")]
        [SerializeField] KeyCode RailKey = KeyCode.R;
        [SerializeField] KeyCode BuildKey = KeyCode.Mouse0;
        [SerializeField] KeyCode wallKey = KeyCode.T;
        [SerializeField] KeyCode buildingModKey = KeyCode.B;

        [Header("References")]
        [SerializeField] MeshRenderer MeshRendererWeapon;
        public LineRenderer BuildingLineRenderer;
        [SerializeField] Transform MainCamTransform;
        [SerializeField] Transform WeaponTransform;
        UiBuildingMod Ui;

        [Header("Material")]
        [SerializeField] Material[] WeaponMaterial;

        [Header("Building")]
        [SerializeField] float LineRendererMaxPoint = 10f;
        [SerializeField] float LineRendererMinPoint = 2f;
        public bool bIsBuilding = false;
        private float CurrentLineRendererPoint = 5f;
        private LayerMask lineLayerMask;

        [Header("Rail")]
        [SerializeField] GameObject RailPrefab;
        [SerializeField] int MaxRailPoint;
        [SerializeField] int CurrentRailPoint = 0;
        List<Vector3> SplinePointList = new List<Vector3>();

        [Header("Wall")]
        [SerializeField] GameObject MarkerWall;
        [SerializeField] GameObject Wall;
        [SerializeField] Material wallMaterial;
        GameObject currentWall;
        GameObject startWall;
        GameObject endWall;
        int markerCounter = 0;
        bool currentlyBuildingWall = false;



        [Header("Building State")]
        public BuildingState state;
        public enum BuildingState
        {
            none,
            rail,
            wall
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            int lineLayer = BuildingLineRenderer.gameObject.layer;
            for (int i = 0; i < 32; i++)
            {
                if (!Physics.GetIgnoreLayerCollision(lineLayer, i))
                {
                    lineLayerMask |= 1 << i;
                }
            }

        Ui = GetComponent<UiBuildingMod>();
        }

        // Update is called once per frame
        void Update()
        {
            ActivateBuildMod();
            if (bIsBuilding == true)
            {
                SelectionMod();
                ModInputManager();
                BuildingModWeapon(bIsBuilding);
            }
            else if (bIsBuilding == false)
            {
                BuildingModWeapon(bIsBuilding);
            }

            if (currentlyBuildingWall == true)
            {
                AdjustWall();
            }

            if (bIsBuilding == true)
            {
                BuildingRange();
                ShowLineRenderer();
            }
        }

    void ActivateBuildMod()
    {
        if (Input.GetKeyDown(buildingModKey) && !bIsBuilding)
        {
            bIsBuilding = true;
            Ui.EnableUiBuild();
        }
        else if (Input.GetKeyDown(buildingModKey) && bIsBuilding == true)
        {
            state = BuildingState.none;
            bIsBuilding = false;
            Ui.EnableUiBuild();
        }
    }

        void ModInputManager()
        {
            if (state == BuildingState.rail)
            {
                CreatRailListPoint();
            }
            if (state == BuildingState.wall)
            {
                CreatWallListPoint();
            }
        }
        void SelectionMod()
        {

        /* if (Input.GetKeyDown(RailKey))
         {
             state = BuildingState.rail;
         }*/
        if (Input.GetKeyDown(wallKey))
            {
            if(state == BuildingState.none)
            {
                state = BuildingState.wall;
                Ui.EnableUiWal();
            }
            else
            {
                state = BuildingState.none;
                Ui.EnableUiWal();
            }
             
        }

        }
        void BuildingModWeapon(bool bBuildingMod)
        {
            if (bBuildingMod == true)
            {
                MeshRendererWeapon.material = WeaponMaterial[1];

            }
            else
            {
                MeshRendererWeapon.material = WeaponMaterial[0];
                BuildingLineRenderer.enabled = false;
            }
        }
        void BuildingRange()
        {
            CurrentLineRendererPoint += Input.GetAxis("Mouse ScrollWheel");
            CurrentLineRendererPoint = Mathf.Clamp(CurrentLineRendererPoint, LineRendererMinPoint, LineRendererMaxPoint);
            
        }
        void ShowLineRenderer()
        {
            BuildingLineRenderer.enabled = true;
            BuildingLineRenderer.SetPosition(0, WeaponTransform.transform.position);
            
            RaycastHit hit;
            if (Physics.Linecast(BuildingLineRenderer.GetPosition(0), BuildingLineRenderer.GetPosition(1), out hit, lineLayerMask))
            {
                BuildingLineRenderer.SetPosition(1, hit.point);
                return;
            }
            else
            {
                BuildingLineRenderer.SetPosition(1, MainCamTransform.forward * CurrentLineRendererPoint + transform.position);
            }
        }
        void CreatRailListPoint()
        {
            if (CurrentRailPoint < MaxRailPoint)
            {
                if (Input.GetKeyDown(BuildKey))
                {
                    SplinePointList.Add(BuildingLineRenderer.GetPosition(1));
                    CurrentRailPoint++;
                }
            }
            else
            {
                CreatRail();
            }

        }

        void CreatRail()
        {
            GameObject rail;
            rail = Instantiate(RailPrefab);
            SplineContainer SplineRail;
            SplineRail = rail.GetComponent<SplineContainer>();

            foreach (float3 point in SplinePointList)
            {
                SplineRail.Spline.Add(point);
            }
            //SplineExtrude extrude;
            //extrude = rail.AddComponent<SplineExtrude>();
            SplinePointList.Clear();
            CurrentRailPoint = 0;
        }

        void CreatWallListPoint()
        {
            if (markerCounter < 2)
            {
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {

                    if (markerCounter == 0)
                    {
                        ///instantiate firstMarker wall
                        Vector3 positionToBuild = BuildingLineRenderer.GetPosition(1);
                        startWall = Instantiate(MarkerWall, positionToBuild, MarkerWall.transform.rotation);

                        ///Instantiate wall
                        GameObject parentWall = Instantiate(Wall, startWall.transform.position, startWall.transform.rotation);
                        currentWall = parentWall.transform.GetChild(0).gameObject;
                        currentlyBuildingWall = true;

                        ///instantiate endMarkerWall
                        endWall = Instantiate(MarkerWall, positionToBuild, MarkerWall.transform.rotation);

                        ///Incremante markerCounter
                        markerCounter++;
                    }
                    else
                    {
                        markerCounter++;
                    }

                }
            }
            else
            {
                creatWall();
            }

        }

        void AdjustWall()
        {
            ///adjuste position of the end of the wall to the pointer of the player
            endWall.transform.position = BuildingLineRenderer.GetPosition(1);

            ///setUp lookAt to rotate the wall between two marker
            startWall.transform.LookAt(endWall.transform.position);
            endWall.transform.LookAt(startWall.transform.position);

            ///creatWall between two Marker
            Transform firstMarkerPos = startWall.transform;
            GameObject parentWall = currentWall.transform.parent.gameObject;
            float distance = Vector3.Distance(firstMarkerPos.position, BuildingLineRenderer.GetPosition(1));
            parentWall.transform.position = firstMarkerPos.position + distance / 2 * firstMarkerPos.forward;
            currentWall.transform.rotation = firstMarkerPos.rotation;
            currentWall.transform.localScale = new Vector3(currentWall.transform.localScale.x, currentWall.transform.localScale.y, distance);
        }

        void creatWall()
        {

            ///change Material of the wall
            currentWall.GetComponent<MeshRenderer>().material = wallMaterial;

            ///Enable collider
            currentWall.GetComponent<Collider>().enabled = true;

            ///Clear Marker
            Destroy(startWall);
            Destroy(endWall);

            ///Clear & reset
            currentWall = null;
            currentlyBuildingWall = false;
            startWall = null;
            endWall = null;
            markerCounter = 0;

        }

    }
