/*
 * Corspe Game Studio
 * Jared Mackenzie 2018
 */
using UnityEngine;

public class Grid : Singleton<Grid>
{

    [Tooltip("Multiplier for the size of the grid.")]
    public float scaleAmount = 1f;

    [Tooltip("Set values for the grid steps in each direction")] // 1x1x1 grid makes each grid step 1 unit apart.  Adjust individually for different effects
    public float sizeX = 1f;
    public float sizeY = 1f;
    public float sizeZ = 1f;

    [Space]
    [Tooltip("Visualize the grid?")]
    public bool isGridRendered = false;

    [Tooltip("Point Primitive Type?")]
    public PrimitiveType pointType = PrimitiveType.Cube;

    [Tooltip("Point color?")]
    public Color pointColor = Color.red;

    [Tooltip("How far do you want to visualize the grid. Careful not to create to many grid points as it can create a slow down")]
    [Range(0, 20)] public float gridSizeX = 10;
    [Range(0, 20)] public float gridSizeY = 10;
    [Range(0, 20)] public float gridSizeZ = 10;

    [Tooltip("Size of the grid points")]
    [Range(0, 1)] public float pointSize = 0.25f;

    public Vector3 GetNearestPointOnGrid(Vector3 position)
    {
        //round each axis to whole number and adjust for size
        int xCount = Mathf.RoundToInt(position.x / sizeX);
        int yCount = Mathf.RoundToInt(position.y / sizeY);
        int zCount = Mathf.RoundToInt(position.z / sizeZ);
        //convert to vector3 for placement
        Vector3 result = new Vector3(
            (float)xCount * sizeX,
            (float)yCount * sizeY,
            (float)zCount * sizeZ);

        //the new position
        return result;
    }

    private void Start()
    {
        // set the scale amount
        sizeX = sizeX * scaleAmount;
        sizeY = sizeY * scaleAmount;
        sizeZ = sizeZ * scaleAmount;
        pointSize = pointSize * scaleAmount;
        
        //dispaly the grid visually?
        if (isGridRendered)
        {
            for (float x = -gridSizeX; x < gridSizeX; x += 1)
            {
                for (float y = -gridSizeY; y < gridSizeY; y += 1)
                {
                    for (float z = -gridSizeZ; z < gridSizeZ; z += 1)
                    {
                        //instantiate the chosen points
                        var point = GameObject.CreatePrimitive(pointType);
                        //set the color
                        point.GetComponent<MeshRenderer>().material.color = pointColor;
                        //parent the points to keep the hierarchy clean
                        point.transform.parent = transform;
                        //place grid points at each grid step based on the grid size

                        point.transform.position = GetNearestPointOnGrid(new Vector3((sizeX * x), (sizeY * y), (sizeZ * z)));
                        //scale points accordingly
                        point.transform.localScale = new Vector3(pointSize, pointSize, pointSize);
                    }
                }
            }
        }
    }
}