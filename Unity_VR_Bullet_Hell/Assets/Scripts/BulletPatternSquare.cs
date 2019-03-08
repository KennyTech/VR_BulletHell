using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPatternSquare : BulletPattern
{
    [SerializeField]
    int rows = 3;

    [SerializeField]
    int columns = 3;

    /// <summary>
    /// Length of the square
    /// </summary>
    [SerializeField]
    [Tooltip("Width of the square")]
    float squareX = 1;

    /// <summary>
    /// Width of the square
    /// </summary>
    [SerializeField]
    [Tooltip("Width of the square")]
    float squareY = 1;

    /// <summary>
    /// The angle bullets will deviate on the X as they get to the edge
    /// </summary>
    [SerializeField]
    [Tooltip("The angle bullets will deviate on the X as they get to the edge")]
    float angleX = 0;

    /// <summary>
    /// The angle bullets will deviate on the Y as they get to the edge
    /// </summary>
    [SerializeField]
    [Tooltip("The angle bullets will deviate on the Y as they get to the edge")]
    float angleY = 0;

    private void OnEnable()
    {
        //InvokeRepeating("FireBullet", 0, fireRate);
    }

    private void Update()
    {
        if (enableSpin)
            SpinEmitter();
    }

    public override void FireBullet()
    {
        float rowIncrement = squareY / (rows - 1);
        float rowPos = -(squareY / 2) - rowIncrement;
        float rotX = Mathf.Abs(angleX);
        float rotIncrementX = Mathf.Abs(angleX) * 2 / (columns - 1);
        float rotIncrementY = Mathf.Abs(angleY) * 2 / (rows - 1);
        for (int y = 0; y < rows; y++)
        {
            rowPos += rowIncrement;
            float columnPos = (squareX / 2) * -1;
            float columnIncrement = squareX / (columns - 1);
            float rotY = Mathf.Abs(angleY) * -1;
            for (int x = 0; x < columns; x++)
            {
                GameObject bullet = bp.GetBullet(bulletColour);
                if (bullet != null)
                {
                    bullet.transform.rotation = Quaternion.identity;

                    bullet.transform.position = spawnPoint.transform.position;
                    bullet.transform.Translate(new Vector3(columnPos, rowPos, 0), transform);
                    columnPos += columnIncrement;

                    bullet.transform.rotation = transform.rotation;
                    //bullet.transform.Rotate(0, rotY, 0);
                    bullet.transform.Rotate(rotX, rotY, 0);
                    rotY += rotIncrementY;

                    ApplyBulletProperties(bullet);

                    bullet.SetActive(true);
                }
            }
            rotX -= rotIncrementX;
        }
    }

    public override void FireBullet_Boss()
    {
        throw new System.NotImplementedException();
    }
}
