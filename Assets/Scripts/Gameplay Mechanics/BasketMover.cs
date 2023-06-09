using UnityEngine;

public class BasketMover : MonoBehaviour
{
    public Transform objectToMove;
    public Transform referenceObject;
    public float movementSpeed = 5f;

    private float leftBoundary;
    private float rightBoundary;
    private bool movingRight = true;
    private bool startMoving = false;
    private bool checkScore = true;

    void Start()
    {
        Vector3 referenceLocalScale = referenceObject.localScale;
        leftBoundary = referenceObject.localPosition.x - referenceLocalScale.x / 2f;
        rightBoundary = referenceObject.localPosition.x + referenceLocalScale.x / 2f;
    }

    void Update()
    {
        if(checkScore)
        {
            if(ScoreCalculator.instance.scoreValue >= 20)
            {
                startMoving = true;
                checkScore = false;
            }
        }

        if (startMoving)
        {
            if (movingRight)
            {
                objectToMove.Translate(referenceObject.right * movementSpeed * Time.deltaTime);
            }
            else
            {
                objectToMove.Translate(-referenceObject.right * movementSpeed * Time.deltaTime);
            }

            if (objectToMove.localPosition.x >= rightBoundary)
            {
                movingRight = false;
            }
            else if (objectToMove.localPosition.x <= leftBoundary)
            {
                movingRight = true;
            }
        }
    }
}
