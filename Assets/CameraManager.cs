using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    // SizeX to move Camera --> 20

    float camZ = -5;

    public enum CameraMode
    {
        FLAT_X,
        FLAT_Y,
        STATIC
    }
    public CameraMode camMode;

    public Transform targetTransform;
    Vector2 camTarget;
    float forwardXOffset = 0; //5;

    public float leftX, rightX;
    public float topY, bottomY;

    public Vector2 targetStaticPosition;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        camTarget = targetTransform.position + new Vector3(GetComponent<GameManager>().myPlayer.GetComponent<Player>().lastXdir * forwardXOffset, 0, 0);

        switch (camMode)
        {
            case CameraMode.FLAT_X:

                this.transform.position = new Vector3(camTarget.x, targetStaticPosition.y, camZ);

                if (this.transform.position.x < leftX)
                {
                    this.transform.position = new Vector3(leftX, this.transform.position.y, camZ);
                }
                else if (this.transform.position.x > rightX)
                {
                    this.transform.position = new Vector3(rightX, this.transform.position.y, camZ);
                }

                break;

            case CameraMode.FLAT_Y:

                this.transform.position = new Vector3(targetStaticPosition.x, camTarget.y, camZ);

                if (this.transform.position.y < bottomY)
                {
                    this.transform.position = new Vector3(this.transform.position.x, bottomY, camZ);
                }
                else if (this.transform.position.y > topY)
                {
                    this.transform.position = new Vector3(this.transform.position.x, topY, camZ);
                }
                break;

            case CameraMode.STATIC:
                break;
        }
    }
}
