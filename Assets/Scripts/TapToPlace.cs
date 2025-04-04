/*
 * Corspe Game Studio
 * Jared Mackenzie 2018
 */
using UnityEngine;

public class TapToPlace : MonoBehaviour
{
    [Tooltip("The brick you want to spawn")]
    public GameObject brickObject;
    [Tooltip("The color of the brick that will be spawned")]
    public Color brickColor;

    void Update()
    {
        //check for click
        if (Input.GetMouseButtonDown(0))
        {
            //get the location of the click
            RaycastHit hitInfo;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if(Physics.Raycast(ray, out hitInfo))
            {
                //place a brick at the clicked location
                PlaceBrick(hitInfo.point);
            }
        }
    }

    void PlaceBrick(Vector3 point)
    {
        //varaible for the new brick
        GameObject brick;
        //adjust placement point for that specific bricks height
        point.y = point.y + brickObject.GetComponent<BrickStats>().brickHeight - 0.32f;
        //adjust placement point to the grid
        point = Grid.Instance.GetNearestPointOnGrid(point);
        //instantiate the brick at the proper place
        brick = Instantiate(brickObject, point, brickObject.transform.rotation);
        //scale the brick according to the grid
        brick.transform.localScale = brick.transform.localScale * Grid.Instance.scaleAmount;
        //set the material
        brick.GetComponent<MeshRenderer>().material.color = brickColor;
    }
}
