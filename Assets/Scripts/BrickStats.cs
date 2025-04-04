/*
 * Corspe Game Studio
 * Jared Mackenzie 2018
 */
using UnityEngine;

public class BrickStats : MonoBehaviour
{
    //Use this to set the hieght of individual bricks.  This helps when placing bricks.
    [Tooltip("Plates = .0032 | Regular = .0096 | 2xRegular = .0192 | 3xRegular = .0288 | 4xRegular = .0384")]
    public float brickHeight = 0.0032f;
}