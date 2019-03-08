using UnityEngine;
using System.Collections;

public class SmartBullet : MonoBehaviour {

    bool isAlive, gravityEnabled;
    /// <summary>
    /// Flip direction of bullet travel
    /// </summary>
    [Tooltip("Flip direction of bullet travel")]
    [SerializeField]
    bool flipDirection = false;

    [SerializeField]
    float lifeTime;         // how long bullets live

    [SerializeField]
    bool isAnchored;        // world transform or local to emitter?

    [SerializeField]
    float forwardSpeed = 30;     // initial velocity

    [SerializeField]
    float rightSpeed = 0;

    [SerializeField]
    float upSpeed = 0;

    [SerializeField]
    float bulletAccel;        // acceleration
    [SerializeField]
    float accelDelay = 0;           // How long before acceleration begins
    [SerializeField]
    float accelDuration = 0;        // How long to accelerate

    [SerializeField]
    float rotationDelay = 0;        // How long before rotation begins
    [SerializeField]
    float rotationAmountX = 0;       // Angle degree to rotate/second
    [SerializeField]
    float rotationAmountY = 0;       // Angle degree to rotate/second
    [SerializeField]
    float rotationAmountZ = 0;       // Angle degree to rotate/second

    [SerializeField]
    float rotationDuration = 0;     // How long to rotate
    [SerializeField]
    int piercingAmount;
    [SerializeField]
    float erasingTime;

    [SerializeField]
    float scaleDelay = 0;
    [SerializeField]
    Vector3 scaleAmount = new Vector3(0, 0, 0);
    [SerializeField]
    float scaleDuration = 0;

    [SerializeField]
    bool moveSin;
    [SerializeField]
    float sinFrequency;         // How often it zigzags
    [SerializeField]
    float sinMagnitude;         // How far to zigzag

    [SerializeField]
    Transform emitter;

    Rigidbody rb;

    void Start()
    {
    }

    void OnEnable() {
        isAlive = true;
        gameObject.transform.localScale = Vector3.one;
        if (transform.parent == null) gameObject.transform.localPosition = Vector3.zero;
        if (rb == null) rb = GetComponent<Rigidbody>();
        rb.velocity = Vector3.zero;
        FireBullet();
        StartCoroutine("DeathTimer");
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy") && !gameObject.CompareTag("Bullet"))
        {
            if (piercingAmount >= 0)
                piercingAmount--;
            if (piercingAmount < 0)
                gameObject.SetActive(false);
        }

        if (other.CompareTag("Bullet") && erasingTime > 0)
            other.gameObject.SetActive(false);
    }

    public void SetErasing(float num)
    {
        erasingTime = num;
        if (erasingTime > 0)
            StartCoroutine("EraseBullets");
    }

    public void SetAnchored(bool a_isAnchored) {
        isAnchored = a_isAnchored;
    }

    public void SetForwardSpeed(float s) {
        forwardSpeed = s;
    }

    public void SetRightSpeed(float s)
    {
        rightSpeed = s;
    }

    public void SetUpSpeed(float s)
    {
        upSpeed = s;
    }

    public void SetBulletAccel(float a_bulletAccel) {
        bulletAccel = a_bulletAccel;
    }

    public void SetAccelDelay(float a_accelDelay) {
        accelDelay = a_accelDelay;
    }

    public void SetAccelDuration(float a_accelDuration) {
        accelDuration = a_accelDuration;
    }

    public void SetRotationDelay(float a_rotationDelay) {
        rotationDelay = a_rotationDelay;
    }

    public void SetRotationAmountX(float a_rotationAmount) {
        rotationAmountX = a_rotationAmount;
    }

    public void SetRotationAmountY(float a_rotationAmount)
    {
        rotationAmountY = a_rotationAmount;
    }

    public void SetRotationAmountZ(float a_rotationAmount)
    {
        rotationAmountZ = a_rotationAmount;
    }

    public void SetRotationDuration(float a_rotationDuration) {
        rotationDuration = a_rotationDuration;
    }

    public void SetLifeTime(float time)
    {
        lifeTime = time;
    }

    public void SetEmitter(Transform _emitter)
    {
        emitter = _emitter;
    }

    public void SetPiercingAmount(int num)
    {
        piercingAmount = num;
    }

    public void SetScaleDelay(float a_scaleDelay)
    {
        scaleDelay = a_scaleDelay;
    }

    public void SetScaleAmount(Vector3 a_scaleAmount)
    {
        scaleAmount = a_scaleAmount;
    }

    public void SetScaleDuration(float a_scaleDuration)
    {
        scaleDuration = a_scaleDuration;
    }

    public void SetMoveSin(bool a_moveSin)
    {
        moveSin = a_moveSin;
    }

    public void SetSinFrequency(float a_sinFrequency)
    {
        sinFrequency = a_sinFrequency;
    }

    public void SetSinMagnitude(float a_sinMagnitude)
    {
        sinMagnitude = a_sinMagnitude;
    }

    public void SetGravity(bool enabled)
    {
        if (enabled)
            GetComponent<Rigidbody2D>().gravityScale = 1;
        else
            GetComponent<Rigidbody2D>().gravityScale = 0;
    }

    public void FireBullet() {
        StartCoroutine("DelayAcceleration", accelDelay);
        StartCoroutine("DelayRotation", rotationDelay);
        StartCoroutine("DelayScale", scaleDelay);

        switch (isAnchored)
        {
            case false:
                // Not Anchored
                switch (moveSin)
                {
                    case true:
                        StartCoroutine("MoveSin");
                        break;
                    case false:
                        StartCoroutine("MoveNormal");
                        break;
                    default:
                        break;
                }
                break;
            case true:
                // Anchored
                transform.SetParent(emitter);
                switch (moveSin)
                {
                    case true:
                        StartCoroutine("MoveSin");
                        break;
                    case false:
                        StartCoroutine("MoveAnchored");
                        break;
                    default:
                        break;
                }
                break;
            default:
                break;
        }
    }

    IEnumerator EraseBullets()
    {
        do
        {
            erasingTime -= Time.deltaTime;
            if (erasingTime <= 0 && erasingTime != -1)
                gameObject.SetActive(false);
            yield return null;
        } while (erasingTime > 0);
    }

    IEnumerator DelayAcceleration(float a_accelDelay) {
        yield return new WaitForSeconds(a_accelDelay);
        StartCoroutine("Accelerate");
    }

    IEnumerator Accelerate() {
        do {
            forwardSpeed += bulletAccel * Time.deltaTime;
            rightSpeed += bulletAccel * Time.deltaTime;
            upSpeed += bulletAccel * Time.deltaTime;
            accelDuration -= Time.deltaTime;
            yield return null;
        } while (accelDuration > 0);
    }

    IEnumerator DelayRotation(float a_rotationDelay) {
        yield return new WaitForSeconds(a_rotationDelay);
        StartCoroutine("Rotate");
    }

    IEnumerator Rotate() {
        do {
            if (Time.timeScale != 0)
            {
                transform.Rotate(Vector3.right * rotationAmountX * Time.deltaTime);
                transform.Rotate(Vector3.up * rotationAmountY * Time.deltaTime);
                transform.Rotate(Vector3.forward * rotationAmountZ * Time.deltaTime);
                rotationDuration -= Time.deltaTime;
            }
            yield return null;
        } while (rotationDuration > 0);
    }
    IEnumerator DelayScale(float a_scaleDelay)
    {
        yield return new WaitForSeconds(a_scaleDelay);
        StartCoroutine("Scale");
    }

    IEnumerator Scale()
    {
        do
        {
            if (Time.timeScale != 0)
            {
                transform.localScale += scaleAmount * Time.deltaTime;
                scaleDuration -= Time.deltaTime;
            }
            yield return null;
        } while (scaleDuration > 0);
    }

    IEnumerator MoveAnchored() {
        this.transform.SetParent(emitter);
        do {
            rb.velocity = forwardSpeed * transform.forward * Time.deltaTime;
            //In case the bullet is actually not anchored but SmartBullet
            //didn't detect the change before the sorting began
            if (!isAnchored)
            {
                StartCoroutine("MoveNormal");
                StopCoroutine("MoveAnchored");
                break;
            }
            yield return null;
        } while (isAlive);
    }

    IEnumerator MoveNormal() {
        do {
            if (flipDirection)
            {
                rb.velocity = -forwardSpeed * transform.forward;
            }
            else
            {
                rb.velocity = forwardSpeed * transform.forward;
            }
            rb.velocity += rightSpeed * transform.right;
            rb.velocity += upSpeed * transform.up;
            rb.velocity *= Time.deltaTime;
            yield return null;
        } while (isAlive);
    }

    IEnumerator MoveSin()
    {
        //do
        //{
        //    if (Time.timeScale != 0)
        //    {
        //        transform.Translate(forwardSpeed * Time.deltaTime);
        //        if (transform.parent != null)
        //            transform.Translate(transform.parent.InverseTransformDirection(transform.right) * Mathf.Sin(Time.time * sinFrequency) * sinMagnitude);
        //        else
        //            transform.Translate(transform.up * Mathf.Sin(Time.time * sinFrequency) * sinMagnitude);
        //    }
            yield return null;
        //} while (isAlive);
    }

    IEnumerator DeathTimer() {
        yield return new WaitForSeconds(lifeTime);

        gameObject.SetActive(false);
    }

}