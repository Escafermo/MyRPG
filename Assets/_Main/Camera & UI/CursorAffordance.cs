using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CameraRaycaster))]
public class CursorAffordance : MonoBehaviour {

    [SerializeField] Texture2D walkCursor = null;
    [SerializeField] Texture2D unknownCursor = null;
	[SerializeField] Texture2D targetCursor = null;
	[SerializeField] Texture2D buttonCursor = null;

    [SerializeField] Vector2 cursorHotspot = new Vector2(0, 0);

    [SerializeField] const int buttonCursorNumber = 5;
    [SerializeField] const int walkCursorNumber = 8;
    [SerializeField] const int targetCursorNumber = 9;

    CameraRaycaster cameraRaycaster;

	// Use this for initialization
	void Start () {
        cameraRaycaster = GetComponent<CameraRaycaster>();
        cameraRaycaster.notifyLayerChangeObservers += OnLayerChanged; // registering
	}

    void OnLayerChanged(int newLayer) {
        switch (newLayer)
        {
		case buttonCursorNumber: // TODO make cameraRaycaster member variables
			Cursor.SetCursor (buttonCursor, cursorHotspot, CursorMode.Auto);
			break;
		case walkCursorNumber:
            Cursor.SetCursor(walkCursor, cursorHotspot, CursorMode.Auto);
            break;
        case targetCursorNumber:
            Cursor.SetCursor(targetCursor, cursorHotspot, CursorMode.Auto);
            break;
        default:
			Cursor.SetCursor(unknownCursor, cursorHotspot, CursorMode.Auto);
            return;
        }
    }

    


    // TODO consider de-registering OnLayerChanged on leaving all game scenes
}
