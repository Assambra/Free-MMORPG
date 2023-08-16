using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MouseHandler : MonoBehaviour
{
    [Header("Public")]
    public bool IsOverUIElement = false;

    [Header("Raycast")]
    [SerializeField] private GraphicRaycaster graphicRaycaster = null;
    [SerializeField] private float waitTimeInSecondsForNextRaycast = 0.25f;
    
    [Header("EventSystem")]
    [SerializeField] private EventSystem eventSystem = null;

    [Header("Mouse Cursor")]
    [SerializeField] private Texture2D arrowLeftDefault = null;
    [SerializeField] private Vector2 hotspotArrowLeftDefault = Vector2.zero;
    [SerializeField] private Texture2D cursorDrag = null;
    [SerializeField] private Vector2 hotspotCursorDrag = Vector2.zero;
    [SerializeField] private Texture2D cursorResizeN_S = null;
    [SerializeField] private Vector2 hotspotCursorResizeN_S = Vector2.zero;
    [SerializeField] private Texture2D cursorResizeW_O = null;
    [SerializeField] private Vector2 hotspotCursorResizeW_O = Vector2.zero;
    [SerializeField] private Texture2D cursorResizeNO_SW = null;
    [SerializeField] private Vector2 hotspotCursorResizeNO_SW = Vector2.zero;
    [SerializeField] private Texture2D cursorResizeNW_SO = null;
    [SerializeField] private Vector2 hotspotCursorResizeNW_SO = Vector2.zero;

    [SerializeField] private CursorMode cursorMode = CursorMode.Auto;


    // Private Fields
    
    private PointerEventData pointerEventData;

    private List<GameObject> raycastedGameObjects = new List<GameObject>();

    private bool mouseMoveFromUIToWorld = false;
    private bool mouseMoveFromWorldToUI = false;

    private float timer = 0.0f;

    private void Awake()
    {
        if (graphicRaycaster == null)
            Debug.LogError("No GraphicRaycaster found");

        if (eventSystem == null)
            Debug.LogError("No EventSystem found");
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if(timer > waitTimeInSecondsForNextRaycast)
        {
            timer = timer - waitTimeInSecondsForNextRaycast;

            raycastedGameObjects.Clear();
            raycastedGameObjects = GetUIGameObjectsMousePointer();

            if (raycastedGameObjects.Count > 0)
            {
                if (Input.GetMouseButton(0) && !mouseMoveFromUIToWorld && !mouseMoveFromWorldToUI)
                    mouseMoveFromUIToWorld = true;
                

                if (mouseMoveFromWorldToUI)
                    IsOverUIElement = false;
                else if (mouseMoveFromUIToWorld)
                    IsOverUIElement = true;
                else
                    IsOverUIElement = true;


                SortOutAndSetCursor(raycastedGameObjects);
            }
            else
            {
                if (Input.GetMouseButton(0) && !mouseMoveFromWorldToUI && !mouseMoveFromUIToWorld)
                    mouseMoveFromWorldToUI = true;
                

                if (mouseMoveFromUIToWorld)
                    IsOverUIElement = true;
                else if (mouseMoveFromWorldToUI)
                    IsOverUIElement = false;
                else
                    IsOverUIElement = false;

                Cursor.SetCursor(arrowLeftDefault, hotspotArrowLeftDefault, cursorMode);
            }
            
            if (!Input.GetMouseButton(0))
            {
                mouseMoveFromUIToWorld = false;
                mouseMoveFromWorldToUI = false;
            }
        }

        // 3D World GameObjects Raycast not in use and is for later we can expand this function if needed 
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;

            // check if the hit a GameObject with layer 9 (9 is empty atm)
            if (Physics.Raycast(ray, out hitInfo, 1 >> 9))
            { }
        }
    }

    private void SortOutAndSetCursor(List<GameObject> listRaycastedGameObjects)
    {
        int i = listRaycastedGameObjects.Count;
        List<string> sortList = new List<string>();

        foreach (GameObject raycastedGameObject in listRaycastedGameObjects)
        {
            i--;
            
            // First we need to go through the List and then set the cursor, why? The "Zones" sit on top of the Window (changing position in hierarchy break the function of zones). The first
            // gameObject in this list is the zone, so we cant set the cursor and after this we observe that we dont need to change the cursor.

            sortList.Add(raycastedGameObject.name);

            if (i == 0 && sortList.Count > 0)
            {
                foreach (string s in sortList)
                {
                    if (ChangeMouseCursor(s))
                        return;
                }
            }
        }
    }

    private bool ChangeMouseCursor(string gameobjectname)
    {
        switch (gameobjectname)
        {
            case "ResizeLeftZone":
            case "ResizeRightZone":
                Cursor.SetCursor(cursorResizeW_O, hotspotCursorResizeW_O, cursorMode);
                return true;

            case "ResizeTopZone":
            case "ResizeBottomZone":
                Cursor.SetCursor(cursorResizeN_S, hotspotCursorResizeN_S, cursorMode);
                return true;

            case "ResizeLeftTopZone":
            case "ResizeRightBottomZone":
                Cursor.SetCursor(cursorResizeNW_SO, hotspotCursorResizeNW_SO, cursorMode);
                return true;

            case "ResizeLeftBottomZone":
            case "ResizeRightTopZone":
                Cursor.SetCursor(cursorResizeNO_SW, hotspotCursorResizeNO_SW, cursorMode);
                return true;
            
            /* This cases are from our UISystem and not in use, for now we comment it out
            // Reset to default e.g on Buttons
            case "ButtonCloseWindow":
            case "ButtonDecreaseWindow":
            case "ButtonMaximizeWindow":
            case "ButtonMinimizeWindow":
            case "ShortcutBarButtonDown":
            case "ShortcutBarButtonUp":
                Cursor.SetCursor(arrowLeftDefault, hotspotArrowLeftDefault, cursorMode);
                return true;
            */

            // Importend to check this as last one! There are more then one GameObject in top of the other to show the right curser on Window resizing and buttons
            case "DragZone":
                Cursor.SetCursor(cursorDrag, hotspotCursorDrag, cursorMode);
                return true;

            default:
                Cursor.SetCursor(arrowLeftDefault, hotspotArrowLeftDefault, cursorMode);
                return false;
        }
    }


    /// <summary>
    /// A list of GameObjects currently under the mouse pointer. 
    /// </summary>
    /// <returns>Returns a List<GameObject></returns>
    public List<GameObject> GetUIGameObjectsMousePointer()
    {
        pointerEventData = new PointerEventData(eventSystem);
        pointerEventData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        graphicRaycaster.Raycast(pointerEventData, results);

        List<GameObject> returnGameObjects = new List<GameObject>();

        foreach (RaycastResult result in results)
        {
            //only add and return GameObjects they are not in Layer 8, (Ignore Graphic Raycast)
            if(result.gameObject.layer != 8)
                returnGameObjects.Add(result.gameObject);
        }

        return returnGameObjects;
    }
}
