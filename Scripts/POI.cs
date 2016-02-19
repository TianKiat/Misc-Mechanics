//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// Name: POI (points of interest)
// Description:
// 
// Author:Ng Tian Kiat
// Date: //
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
using UnityEngine;
using UnityEngine.UI;
public class POI : MonoBehaviour
{
    [HeaderAttribute("UI Reference / Settings")]
    private Canvas canvas;
    public Image TargetSprite;
    public Image OnScreenSprite; // can use gameobject with spriterenderer, or reference a Sprite here, and create a new gameobject, add a sprite renderer to it.
    public Image OffScreenSprite;
    public bool displayOffscreen;
    Vector3 screenCenter = new Vector3(Screen.width, Screen.height, 0) * .5f;

    private Image targetSprite; //instance of targetSprite
    private Image onSprite;//instance of onscreen sprite
    private Image offSprite;//instance of offscreen sprite
    private Vector3 offScreen = new Vector3(-100, -100, 0);

    private Rect centerRect;
    // Use this for initialization
    void Start()
    {
        canvas = GameObject.Find("POICanvas").GetComponent<Canvas>();
        if (canvas == null)
            Debug.LogError("No POI canvas");
        targetSprite = Instantiate(TargetSprite, offScreen, Quaternion.Euler(new Vector3(0, 0, 0))) as Image;
        onSprite = Instantiate(OnScreenSprite, offScreen, Quaternion.Euler(new Vector3(0, 0, 0))) as Image;
        offSprite = Instantiate(OffScreenSprite, offScreen, Quaternion.Euler(new Vector3(0, 0, 0))) as Image;
        targetSprite.rectTransform.parent = canvas.transform;
        onSprite.rectTransform.parent = canvas.transform;
        offSprite.rectTransform.parent = canvas.transform;
        centerRect.width = Screen.width / 2;
        centerRect.height = Screen.height / 2;
        centerRect.position = new Vector2(screenCenter.x - centerRect.width / 2, screenCenter.y - centerRect.height / 2);
    }
    // Update is called once per frame
    void Update()
    {
        PlaceIndicators();
    }
    void PlaceIndicators()
    {
        //GameObject[] objects = GameObject.FindObjectsOfType(typeof(AI)) as GameObject[]; //find objects by type (might ahve to find by gameobejct, and then filter for AI)
        //go through all the objects we care about

        Vector3 screenpos = Camera.main.WorldToScreenPoint(transform.position);
        if (screenpos.z < -.8f)
        {
            //onSprite.rectTransform.position = offScreen;
            //offSprite.rectTransform.position =  offScreen;//get rid of the arrow indicator
            //return;
        }
        //if onscreen
        if (screenpos.z > 0 && screenpos.x < Screen.width && screenpos.x > 0 && screenpos.y < Screen.height && screenpos.y > 0)
        {
            offSprite.rectTransform.position = offScreen;//get rid of the arrow indicator
            //if in the center rect
            if (centerRect.Contains(screenpos))
            {
                targetSprite.rectTransform.position = screenpos;
                onSprite.rectTransform.position = offScreen;
            }
            else
            {
                onSprite.rectTransform.position = screenpos;
                targetSprite.rectTransform.position = offScreen;
                //Debug.Log("OnScreen: " + screenpos);
            }
        }
        else
        {
            if (displayOffscreen)
            {
                PlaceOffscreen(screenpos);
            }
            onSprite.rectTransform.position = offScreen; //get rid of the onscreen indicator
            targetSprite.rectTransform.position = offScreen;
        }
    }
    void PlaceOffscreen(Vector3 screenpos)
    {
        float x = screenpos.x;
        float y = screenpos.y;
        float offset = 20;
        float angle = 0;
        if (screenpos.z < 0)
        {
            screenpos = -screenpos;
        }
        if (screenpos.x > Screen.width) //right
        {
            angle = 90;
            x = Screen.width - offset;
        }
        if (screenpos.x < 0)
        {
            angle = -90;
            x = offset;
        }
        if (screenpos.y > Screen.height)
        {
            angle = 180;
            y = Screen.height - offset;
        }
        if (screenpos.y < 0)
        {
            y = offset;
        }
        Vector3 offscreenPos = new Vector3(x, y, 0);
        offSprite.rectTransform.position = offscreenPos;
        offSprite.rectTransform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }
    void OnDestroy()
    {
        Destroy(targetSprite);
        Destroy(onSprite);
        Destroy(offSprite);
    }
}
