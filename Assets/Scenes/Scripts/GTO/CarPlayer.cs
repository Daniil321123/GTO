using UnityEngine;
using MLAPI;
using MLAPI.NetworkVariable;

public class CarPlayer : NetworkBehaviour
{
    [SerializeField] public RCC_Camera camera;
    [SerializeField] public GameObject Canvas;
    [SerializeField] public bool isMine = false;
    RCC_CarControllerV3 newVehicle;
    private NetworkVariableVector3 currentVelocity;
    float updateTime = 0;

    private NetworkVariableBool lowBeamHeadLightsOn = new NetworkVariableBool(false);
    private NetworkVariableFloat steerInput = new NetworkVariableFloat(0.0f);
    private NetworkVariableFloat gasInput = new NetworkVariableFloat(0.0f);
    private NetworkVariableFloat brakeInput = new NetworkVariableFloat(0.0f);
    private NetworkVariableFloat handbrakeInput = new NetworkVariableFloat(0.0f);
    private NetworkVariableFloat boostInput = new NetworkVariableFloat(0.0f);
    private NetworkVariableFloat clutchInput = new NetworkVariableFloat(0.0f);
    private NetworkVariableInt gear = new NetworkVariableInt(0);
    private NetworkVariableInt direction = new NetworkVariableInt(1);
    private NetworkVariableBool changingGear = new NetworkVariableBool(false);
    private NetworkVariableBool semiAutomaticGear = new NetworkVariableBool(false);
    private NetworkVariableFloat fuelInput = new NetworkVariableFloat(1f);
    private NetworkVariableBool engineRunning = new NetworkVariableBool(false);
    private NetworkVariableBool applyCounterSteering = new NetworkVariableBool(true);
    private NetworkVariableBool steeringHelper = new NetworkVariableBool();
    private NetworkVariableVector3 correctPlayerPos = new NetworkVariableVector3();
    private NetworkVariableQuaternion correctPlayerRot = new NetworkVariableQuaternion();

    private NetworkVariableFloat[] cambers;

    private RCC_WheelCollider[] wheelColliders;
    private Rigidbody rigidbody;

    private NetworkVariable<RCC_CarControllerV3.IndicatorsOn> indicatorsOn = new NetworkVariable<RCC_CarControllerV3.IndicatorsOn>();


    public override void NetworkStart()
    {
        lowBeamHeadLightsOn.Settings.WritePermission = NetworkVariablePermission.Everyone;
        steerInput.Settings.WritePermission = NetworkVariablePermission.Everyone;
        gasInput.Settings.WritePermission = NetworkVariablePermission.Everyone;
        brakeInput.Settings.WritePermission = NetworkVariablePermission.Everyone;
        handbrakeInput.Settings.WritePermission = NetworkVariablePermission.Everyone;
        boostInput.Settings.WritePermission = NetworkVariablePermission.Everyone;
        clutchInput.Settings.WritePermission = NetworkVariablePermission.Everyone;
        gear.Settings.WritePermission = NetworkVariablePermission.Everyone;
        direction.Settings.WritePermission = NetworkVariablePermission.Everyone;
        changingGear.Settings.WritePermission = NetworkVariablePermission.Everyone;
        semiAutomaticGear.Settings.WritePermission = NetworkVariablePermission.Everyone;
        fuelInput.Settings.WritePermission = NetworkVariablePermission.Everyone;
        engineRunning.Settings.WritePermission = NetworkVariablePermission.Everyone;
        applyCounterSteering.Settings.WritePermission = NetworkVariablePermission.Everyone;
        steeringHelper.Settings.WritePermission = NetworkVariablePermission.Everyone;
        currentVelocity.Settings.WritePermission = NetworkVariablePermission.Everyone;
        indicatorsOn.Settings.WritePermission = NetworkVariablePermission.Everyone;

        correctPlayerPos.Settings.WritePermission = NetworkVariablePermission.Everyone;
        correctPlayerRot.Settings.WritePermission = NetworkVariablePermission.Everyone;


        if (NetworkManager.IsClient)
        {
            newVehicle = gameObject.GetComponent<RCC_CarControllerV3>();
            NetworkObject vehicleNO = newVehicle.GetComponent<NetworkObject>();
            wheelColliders = newVehicle.GetComponentsInChildren<RCC_WheelCollider>();
            cambers = new NetworkVariableFloat[wheelColliders.Length];

            if (vehicleNO.IsLocalPlayer)
            {
                newVehicle.externalController = false;
                newVehicle.canControl = true;
            }
            else
            {
                newVehicle.externalController = true;
                newVehicle.canControl = false;
            }
            if (vehicleNO != null && vehicleNO.IsLocalPlayer)
            {
                isMine = true;
                if (newVehicle != null)
                {
                    var cameraInst = Instantiate(camera);
                    Instantiate(Canvas);
                    cameraInst.playerCar = newVehicle;
                    RCC.RegisterPlayerVehicle(newVehicle);
                    RCC.SetControl(newVehicle, true);
                    rigidbody = GetComponent<Rigidbody>();
                }
            }
        }
    }
    private void FixedUpdate()
    {
        if (!newVehicle)
        {
            return;
        }


        if (!isMine)
        {
            newVehicle.steerInput = steerInput.Value;
            newVehicle.lowBeamHeadLightsOn = lowBeamHeadLightsOn.Value;
            newVehicle.throttleInput = gasInput.Value;
            newVehicle.brakeInput = brakeInput.Value;
            newVehicle.handbrakeInput = handbrakeInput.Value;
            newVehicle.boostInput = boostInput.Value;
            newVehicle.clutchInput = clutchInput.Value;
            newVehicle.currentGear = gear.Value;
            newVehicle.direction = direction.Value;
            newVehicle.changingGear = changingGear.Value;
            newVehicle.semiAutomaticGear = semiAutomaticGear.Value;
            newVehicle.fuelInput = fuelInput.Value;
            newVehicle.engineRunning = engineRunning.Value;
            newVehicle.useCounterSteering = applyCounterSteering.Value;
            newVehicle.steeringHelper = steeringHelper.Value;

            Vector3 projectedPosition = this.correctPlayerPos.Value + currentVelocity.Value * (Time.time - updateTime);

            if (Vector3.Distance(transform.position, correctPlayerPos.Value) < 15f)
            {

                transform.position = Vector3.Lerp(transform.position, projectedPosition, Time.deltaTime * 5f);
                transform.rotation = Quaternion.Lerp(transform.rotation, this.correctPlayerRot.Value, Time.deltaTime * 5f);

            }
            else
            {

                transform.position = correctPlayerPos.Value;
                transform.rotation = correctPlayerRot.Value;

            }

            //transform.position = correctPlayerPos.Value;
            //transform.rotation = correctPlayerRot.Value; 
        }
        else
        {
            steerInput.Value = newVehicle.steerInput;
            lowBeamHeadLightsOn.Value = newVehicle.lowBeamHeadLightsOn;
            gasInput.Value = newVehicle.throttleInput;
            brakeInput.Value = newVehicle.brakeInput;
            handbrakeInput.Value = newVehicle.handbrakeInput;
            boostInput.Value = newVehicle.boostInput;
            clutchInput.Value = newVehicle.clutchInput;
            gear.Value = newVehicle.currentGear;
            direction.Value = newVehicle.direction;
            changingGear.Value = newVehicle.changingGear;
            semiAutomaticGear.Value = newVehicle.semiAutomaticGear;
            fuelInput.Value = newVehicle.fuelInput;
            engineRunning.Value = newVehicle.engineRunning;
            applyCounterSteering.Value = newVehicle.useCounterSteering;
            steeringHelper.Value = newVehicle.steeringHelper;

            Vector3 projectedPosition = transform.position + currentVelocity.Value * (Time.time - updateTime);

            if (Vector3.Distance(correctPlayerPos.Value, transform.position) < 15f)
            {

                correctPlayerPos.Value = Vector3.Lerp(correctPlayerPos.Value, projectedPosition, Time.deltaTime * 5f);
                correctPlayerRot.Value = Quaternion.Lerp(correctPlayerRot.Value, transform.rotation, Time.deltaTime * 5f);

            }
            else
            {
                correctPlayerPos.Value = transform.position;
                correctPlayerRot.Value = transform.rotation;
            }

        }
    }
}
