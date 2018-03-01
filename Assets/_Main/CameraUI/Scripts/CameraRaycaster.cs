using UnityEngine;
using RPG.Characters;

namespace RPG.CameraUI
{
    public class CameraRaycaster : MonoBehaviour
    {
        const int WALKABLE_LAYER_NUMBER = 8;

        [SerializeField] Vector2 cursorHotspot = new Vector2(0, 0);
        [SerializeField] Texture2D walkCursor = null;
        [SerializeField] Texture2D enemyCursor = null;

        float maxRaycastDepth = 1000f; // Hard coded value

        // Defining screen size to fix mouse hover bug
        Rect currentScreenRect; // TODO move inside update if want to support changing screen size

        // TODO remove below
        int topPriorityLayerLastFrame = -1; // So get ? from start with Default layer terrain

        // Setup delegates for broadcasting layer changes to other classes
        public delegate void OnMouseOverDestination(Vector3 destination);
        public event OnMouseOverDestination notifyNewDestinationObservers;

        public delegate void OnMouseOverEnemy(Enemy enemy);
        public event OnMouseOverEnemy notifyNewEnemyObservers;

        void Update()
        {
            currentScreenRect = new Rect(0, 0, Screen.width, Screen.height);
            // Check if pointer is over an interactable UI element
            //if (EventSystem.current.IsPointerOverGameObject())
            //{
            //    // TODO Implement UI interaction here
            //    return; // Stop looking for other objects
            //}
            //else
            //{
                PerformRayCast();
            //}
        }

        void PerformRayCast()
        {
            if (currentScreenRect.Contains(Input.mousePosition))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                //Specify layer priorities ->> ORDER OF CALL MATTERS
                if (RayCastForEnemy(ray)) { return; }
                if (RayCastForWalkable(ray)) { return; }
            }
        }

        private bool RayCastForEnemy(Ray ray)
        {
            RaycastHit hitInfo;
            Physics.Raycast(ray, out hitInfo, maxRaycastDepth);
            if (hitInfo.transform != null)
            {
                var gameObjectHit = hitInfo.collider.gameObject;
                var enemyHit = gameObjectHit.GetComponent<Enemy>();
                if (enemyHit)
                {
                    Cursor.SetCursor(enemyCursor, cursorHotspot, CursorMode.Auto);
                    notifyNewEnemyObservers(enemyHit);
                    return true;
                }
            }
            return false;
        }

        private bool RayCastForWalkable(Ray ray)
        {
            RaycastHit hitInfo;
            LayerMask walkableLayer = 1 << WALKABLE_LAYER_NUMBER;
            bool isWalkableHit = Physics.Raycast(ray, out hitInfo, maxRaycastDepth, walkableLayer);
            if (isWalkableHit)
            {
                Cursor.SetCursor(walkCursor, cursorHotspot, CursorMode.Auto);
                notifyNewDestinationObservers(hitInfo.point);
                return true;
            }
            return false;
        }
    }
}

#region OLD METHODS FOR RAYCASTING

//[SerializeField] int[] layerPriorities; // TODO remove

//public delegate void OnCursorLayerChange(int newLayer); // declare new delegate type
//public event OnCursorLayerChange notifyLayerChangeObservers; // instantiate an observer set

//public delegate void OnClickPriorityLayer(RaycastHit raycastHit, int layerHit); // declare new delegate type
//public event OnClickPriorityLayer notifyMouseClickObservers; // instantiate an observer set

//public delegate void OnClickSpendEnergy(RaycastHit raycastHit, int layerHit); // declare new delegate type
//public event OnClickSpendEnergy notifyEnergySpent; // instantiate an observer set

//private void FarTooComplex()
//{
//    // Raycast to max depth, every frame as things can move under mouse
//    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
//    RaycastHit[] raycastHits = Physics.RaycastAll(ray, maxRaycastDepth);

//    RaycastHit? priorityHit = FindTopPriorityHit(raycastHits);
//    if (!priorityHit.HasValue) // if hit no priority object
//    {
//        NotifyObserersIfLayerChanged(0); // broadcast default layer
//        return;
//    }

//    // Notify delegates of layer change
//    var layerHit = priorityHit.Value.collider.gameObject.layer;
//    NotifyObserersIfLayerChanged(layerHit);

//    // Notify delegates of highest priority game object under mouse when clicked
//    if (Input.GetMouseButton(0))
//    {
//        notifyMouseClickObservers(priorityHit.Value, layerHit);
//    }

//    // Notify delegates of second button mouse click for energy spenditure 
//    if (Input.GetMouseButtonDown(2))
//    {
//        notifyEnergySpent(priorityHit.Value, layerHit);
//    }
//}

//void NotifyObserersIfLayerChanged(int newLayer)
//{
//    if (newLayer != topPriorityLayerLastFrame)
//    {
//        topPriorityLayerLastFrame = newLayer;
//        notifyLayerChangeObservers(newLayer);
//    }
//}

//RaycastHit? FindTopPriorityHit(RaycastHit[] raycastHits)
//{
//    // Form list of layer numbers hit
//    List<int> layersOfHitColliders = new List<int>();
//    foreach (RaycastHit hit in raycastHits)
//    {
//        layersOfHitColliders.Add(hit.collider.gameObject.layer);
//    }

//    // Step through layers in order of priority looking for a gameobject with that layer
//    foreach (int layer in layerPriorities)
//    {
//        foreach (RaycastHit hit in raycastHits)
//        {
//            if (hit.collider.gameObject.layer == layer)
//            {
//                return hit; // stop looking
//            }
//        }
//    }
//    return null; // because cannot use GameObject? nullable
//}
#endregion