using Cinemachine;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float freeFallingSpeed = 3;
    public float parachuteSpeed = 6;

    [Header("Rotations")]
    public float rotateSpeed = 5;
    public float inclinateSpeed = 3;
    public float returnInPosSpeed = 1;
    public float openParachuteBodyRotationSpeed = 2;
    public float closeParachuteBodyRotationSpeed = 1;

    [Header("Fall Properties")]
    public float minDrag = 0;
    public float freeFallingDrag = 3;
    public float parachuteDrag = 6;
    public float heightToForceParachute = 20;

    Rigidbody rigidBody, nuttyRigidBody;
    Transform nuttyBody;
    Animator anim;
    bool canMove;
    bool parachuteMode;
    float startingX, startingY;
    Vector3 inputs;
    float mouseX;
    bool forceMode;

    float maxX = 45, minX = -45, maxY = 70, minY = 0, minStand = -90;
    float maxTurnOnParachute = 45;
    float rotationX = 0, rotationY = 0;
    Quaternion originalRotation;

    float spaceSpamControl, spaceSpamLimitTime = 2;

    private void Start()
    {
        anim = GetComponentInChildren<Animator>();
        rigidBody = GetComponent<Rigidbody>();
        nuttyBody = transform.GetChild(0);
        nuttyRigidBody = nuttyBody.GetComponent<Rigidbody>();
        CinemachineCore.CameraUpdatedEvent.AddListener(UpdateObjectPosition);
        originalRotation = nuttyBody.localRotation;
    }

    public void EnableCharacter()
    {
        canMove = true;
    }

    private void Update()
    {
        inputs = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        mouseX = Input.GetAxis("Mouse X");
    }

    void UpdateObjectPosition(CinemachineBrain brain)
    {
        if (!canMove)
            return;

        if (transform.position.y < heightToForceParachute)
        {
            forceMode = true;
            parachuteMode = true;
        }

        if (!forceMode && Input.GetKeyUp(KeyCode.Space) && Time.time > spaceSpamControl)
        {
            parachuteMode = !parachuteMode;
            spaceSpamControl = Time.time + spaceSpamLimitTime;
        }

        anim.SetBool("ParachuteOn", parachuteMode);

        if (!parachuteMode && spaceSpamControl > Time.time)
        {
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("CloseParachute") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.3f)
            {
                rotationY += closeParachuteBodyRotationSpeed;
                rotationY = ClampAngle(rotationY, minStand, minY);
                Quaternion yQuaternion = Quaternion.AngleAxis(rotationY, Vector3.right);
                nuttyBody.localRotation = originalRotation * yQuaternion;
            }
        }
        else
        {
            if (!parachuteMode)
            {
                if (inputs.x != 0)
                    rotationX += inputs.x * -inclinateSpeed;
                else
                {
                    if (rotationX > 0)
                        rotationX = Mathf.Clamp(rotationX - returnInPosSpeed, 0, maxX);
                    else if (rotationX < 0)
                        rotationX = Mathf.Clamp(rotationX + returnInPosSpeed, minX, 0);
                }
                rotationX = ClampAngle(rotationX, minX, maxX);

                if (inputs.z != 0)
                {
                    rotationY += inputs.z * inclinateSpeed;
                    rigidBody.drag = minDrag;
                }
                else
                {
                    rotationY -= returnInPosSpeed;
                    rigidBody.drag = freeFallingDrag;
                }
                rotationY = ClampAngle(rotationY, minY, maxY);

                Quaternion xQuaternion = Quaternion.AngleAxis(rotationX, Vector3.forward);
                Quaternion yQuaternion = Quaternion.AngleAxis(rotationY, Vector3.right);

                nuttyBody.localRotation = originalRotation * xQuaternion * yQuaternion;
                anim.SetFloat("Blend", inputs.z);
            }
            else
            {
                if (anim.GetCurrentAnimatorStateInfo(0).IsName("OpenParachute") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.7f)
                {
                    rotationY -= openParachuteBodyRotationSpeed;
                    rotationY = ClampAngle(rotationY, minStand, minY);
                    Quaternion yQuaternion = Quaternion.AngleAxis(rotationY, Vector3.right);
                    nuttyBody.localRotation = originalRotation * yQuaternion;
                }
                else if (anim.GetCurrentAnimatorStateInfo(0).IsName("ParachutePos"))
                {
                    if (inputs.x != 0)
                        rotationX += inputs.x * -inclinateSpeed;
                    else
                    {
                        if (rotationX > 0)
                            rotationX = Mathf.Clamp(rotationX - returnInPosSpeed, 0, maxX);
                        else if (rotationX < 0)
                            rotationX = Mathf.Clamp(rotationX + returnInPosSpeed, minX, 0);
                    }
                    rotationX = ClampAngle(rotationX, minX, maxX);

                    if (inputs.z != 0)
                        rotationY += inputs.z * inclinateSpeed;
                    else
                    {
                        if (rotationY > minStand)
                            rotationY = Mathf.Clamp(rotationY - returnInPosSpeed, minStand, minStand + 45);
                        else if (rotationY < minStand)
                            rotationY = Mathf.Clamp(rotationY + returnInPosSpeed, minStand - 45, minStand);
                    }
                    rotationY = ClampAngle(rotationY, minStand - 45, minStand + 45);

                    Quaternion xQuaternion = Quaternion.AngleAxis(rotationX, Vector3.forward);
                    Quaternion yQuaternion = Quaternion.AngleAxis(rotationY, Vector3.right);

                    nuttyBody.localRotation = originalRotation * xQuaternion * yQuaternion;
                }
            }
        }

    }

    private void FixedUpdate()
    {
        if (!canMove)
            return;

        if (mouseX != 0)
            rigidBody.MoveRotation(transform.rotation * Quaternion.AngleAxis(mouseX * rotateSpeed, Vector3.up));

        if (!parachuteMode)
            rigidBody.MovePosition(transform.position + transform.right * inputs.x * freeFallingSpeed * Time.fixedDeltaTime);
        else
        {
            var x = transform.right * inputs.x;
            var z = transform.forward * inputs.z;
            var dir = x + z;
            rigidBody.MovePosition(transform.position + dir * freeFallingSpeed * Time.fixedDeltaTime);
        }

        
    }

    public float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F)
         angle += 360F;
        if (angle > 360F)
         angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }
}






//if (inputs.x != 0)
//    rotationX += inputs.x * -inclinateSpeed;
//else
//{
//    if (rotationX > 0)
//        rotationX = Mathf.Clamp(rotationX - returnInPosSpeed, 0, maxX);
//    else if (rotationX < 0)
//        rotationX = Mathf.Clamp(rotationX + returnInPosSpeed, minX, 0);
//}
//rotationX = ClampAngle(rotationX, minX, maxX);

//if (inputs.z != 0)
//{
//    rotationY += inputs.z * inclinateSpeed;
//    rigidBody.drag = minDrag;
//}
//else
//{
//    rotationY -= returnInPosSpeed;
//    rigidBody.drag = freeFallingDrag;
//}
//rotationY = ClampAngle(rotationY, minY, maxY);

//Quaternion xQuaternion = Quaternion.AngleAxis(rotationX, Vector3.forward);
//Quaternion yQuaternion = Quaternion.AngleAxis(rotationY, Vector3.right);

////nuttyBody.localRotation = originalRotation * xQuaternion * yQuaternion;
//rigidBody.MoveRotation(originalRotation * xQuaternion * yQuaternion);

//anim.SetFloat("Blend", inputs.z);