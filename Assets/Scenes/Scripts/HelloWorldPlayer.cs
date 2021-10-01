using UnityEngine;
using MLAPI;
using MLAPI.NetworkVariable;

public class HelloWorldPlayer : NetworkBehaviour
{
    [SerializeField] public RCC_Camera camera;
    [SerializeField] public GameObject Canvas;
    [SerializeField] public bool isMine = false;

    public int newCarIndex = 0;

    public RCC_Camera cameraInst;
    public GameObject CanvasInst;
    private float updateDelay;

    private RCC_CarControllerV3 newVehicle;
    private TuningDetails tuningDetails;
    private NetworkVariableVector3 currentVelocity;
    private NetworkVariableFloat updateTime = new NetworkVariableFloat(0);

    private NetworkVariableBool lowBeamHeadLightsOn  = new NetworkVariableBool(false);
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

    private NetworkVariableBool applyEngineTorqueToExtraRearWheelColliders;
    private NetworkVariableBool canGoReverseNow;
    private NetworkVariableFloat engineTorque;
    private NetworkVariableFloat brakeTorque;
    private NetworkVariableFloat minEngineRPM;
    private NetworkVariableFloat maxEngineRPM;
    private NetworkVariableFloat engineInertia;
    private NetworkVariableBool useRevLimiter;
    private NetworkVariableBool useExhaustFlame;
    private NetworkVariableFloat highspeedsteerAngle;
    private NetworkVariableFloat highspeedsteerAngleAtspeed;
    private NetworkVariableFloat antiRollFrontHorizontal;
    private NetworkVariableFloat antiRollRearHorizontal;
    private NetworkVariableFloat antiRollVertical;
    private NetworkVariableFloat maxspeed;
    private NetworkVariableFloat engineHeat;
    private NetworkVariableFloat engineHeatMultiplier;
    private NetworkVariableInt totalGears;
    private NetworkVariableFloat gearShiftingDelay;
    private NetworkVariableFloat gearShiftingThreshold;
    private NetworkVariableFloat clutchInertia;
    private NetworkVariableBool NGear;
    private NetworkVariableFloat launched;
    private NetworkVariableBool ABS;
    private NetworkVariableBool TCS;
    private NetworkVariableBool ESP;
    private NetworkVariableBool tractionHelper;
    private NetworkVariableBool useNOS;
    private NetworkVariableBool useTurbo;

    private NetworkVariable<RCC_CarControllerV3.IndicatorsOn> indicatorsOn = new NetworkVariable<RCC_CarControllerV3.IndicatorsOn>();


    private NetworkVariableVector3 correctPlayerPos = new NetworkVariableVector3();
    private NetworkVariableQuaternion correctPlayerRot = new NetworkVariableQuaternion();

    public NetworkVariableInt currentFrontBumper;
    public NetworkVariableInt currentRearBumper;
    public NetworkVariableInt currentExhaust;
    public NetworkVariableInt currentExt;
    public NetworkVariableInt currentFender;
    public NetworkVariableInt currentRoof;
    public NetworkVariableInt currentSpoiler;

    private NetworkVariableFloat[] cambers;

    private RCC_WheelCollider[] wheelColliders;
    private Rigidbody rigidbody;
    private Vector3 correctPlayerPosLocal;



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
        updateTime.Settings.WritePermission = NetworkVariablePermission.Everyone;

        applyEngineTorqueToExtraRearWheelColliders.Settings.WritePermission = NetworkVariablePermission.Everyone;
        canGoReverseNow.Settings.WritePermission = NetworkVariablePermission.Everyone;
        engineTorque.Settings.WritePermission = NetworkVariablePermission.Everyone;
        brakeTorque.Settings.WritePermission = NetworkVariablePermission.Everyone;
        minEngineRPM.Settings.WritePermission = NetworkVariablePermission.Everyone;
        maxEngineRPM.Settings.WritePermission = NetworkVariablePermission.Everyone;
        engineInertia.Settings.WritePermission = NetworkVariablePermission.Everyone;
        useRevLimiter.Settings.WritePermission = NetworkVariablePermission.Everyone;
        useExhaustFlame.Settings.WritePermission = NetworkVariablePermission.Everyone;
        highspeedsteerAngle.Settings.WritePermission = NetworkVariablePermission.Everyone;
        highspeedsteerAngleAtspeed.Settings.WritePermission = NetworkVariablePermission.Everyone;
        antiRollFrontHorizontal.Settings.WritePermission = NetworkVariablePermission.Everyone;
        antiRollRearHorizontal.Settings.WritePermission = NetworkVariablePermission.Everyone;
        antiRollVertical.Settings.WritePermission = NetworkVariablePermission.Everyone;
        maxspeed.Settings.WritePermission = NetworkVariablePermission.Everyone;
        engineHeat.Settings.WritePermission = NetworkVariablePermission.Everyone;
        engineHeatMultiplier.Settings.WritePermission = NetworkVariablePermission.Everyone;
        totalGears.Settings.WritePermission = NetworkVariablePermission.Everyone;
        gearShiftingDelay.Settings.WritePermission = NetworkVariablePermission.Everyone;
        gearShiftingThreshold.Settings.WritePermission = NetworkVariablePermission.Everyone;
        clutchInertia.Settings.WritePermission = NetworkVariablePermission.Everyone;
        NGear.Settings.WritePermission = NetworkVariablePermission.Everyone;
        launched.Settings.WritePermission = NetworkVariablePermission.Everyone;
        ABS.Settings.WritePermission = NetworkVariablePermission.Everyone;
        TCS.Settings.WritePermission = NetworkVariablePermission.Everyone;
        ESP.Settings.WritePermission = NetworkVariablePermission.Everyone;
        tractionHelper.Settings.WritePermission = NetworkVariablePermission.Everyone;
        useNOS.Settings.WritePermission = NetworkVariablePermission.Everyone;
        useTurbo.Settings.WritePermission = NetworkVariablePermission.Everyone;

        indicatorsOn.Settings.WritePermission = NetworkVariablePermission.Everyone;

        currentFrontBumper.Settings.WritePermission = NetworkVariablePermission.Everyone;
        currentRearBumper.Settings.WritePermission = NetworkVariablePermission.Everyone;
        currentExhaust.Settings.WritePermission = NetworkVariablePermission.Everyone;
        currentExt.Settings.WritePermission = NetworkVariablePermission.Everyone;
        currentFender.Settings.WritePermission = NetworkVariablePermission.Everyone;
        currentSpoiler.Settings.WritePermission = NetworkVariablePermission.Everyone;
        currentRoof.Settings.WritePermission = NetworkVariablePermission.Everyone;


        correctPlayerPos.Settings.WritePermission = NetworkVariablePermission.Everyone;
        correctPlayerRot.Settings.WritePermission = NetworkVariablePermission.Everyone;


        if (NetworkManager.IsClient)
        {
            newVehicle = gameObject.GetComponent<RCC_CarControllerV3>();
            NetworkObject vehicleNO = newVehicle.GetComponent<NetworkObject>();
            wheelColliders = newVehicle.GetComponentsInChildren<RCC_WheelCollider>();
            cambers = new NetworkVariableFloat[wheelColliders.Length];
            tuningDetails = newVehicle.GetComponent<TuningDetails>();
            correctPlayerPosLocal = transform.position;

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
                    cameraInst = Instantiate(camera);
                    CanvasInst = Instantiate(Canvas);
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

            newVehicle.applyEngineTorqueToExtraRearWheelColliders = applyEngineTorqueToExtraRearWheelColliders.Value;
            newVehicle.canGoReverseNow = canGoReverseNow.Value;
            newVehicle.maxEngineTorque = engineTorque.Value;
            newVehicle.brakeTorque = brakeTorque.Value;
            newVehicle.minEngineRPM = minEngineRPM.Value;
            newVehicle.maxEngineRPM = maxEngineRPM.Value;
            newVehicle.engineInertia = engineInertia.Value;
            newVehicle.useRevLimiter = useRevLimiter.Value;
            newVehicle.useExhaustFlame = useExhaustFlame.Value;
            newVehicle.highspeedsteerAngle = highspeedsteerAngle.Value;
            newVehicle.highspeedsteerAngleAtspeed = highspeedsteerAngleAtspeed.Value;
            newVehicle.antiRollFrontHorizontal = antiRollFrontHorizontal.Value;
            newVehicle.antiRollRearHorizontal = antiRollRearHorizontal.Value;
            newVehicle.antiRollVertical = antiRollVertical.Value;
            newVehicle.maxspeed = maxspeed.Value;
            newVehicle.engineHeat = engineHeat.Value;
            newVehicle.engineHeatRate = engineHeatMultiplier.Value;
            newVehicle.totalGears = totalGears.Value;
            newVehicle.gearShiftingDelay = gearShiftingDelay.Value;
            newVehicle.gearShiftingThreshold = gearShiftingThreshold.Value;
            newVehicle.clutchInertia = clutchInertia.Value;
            newVehicle.NGear = NGear.Value;
            newVehicle.launched = launched.Value;
            newVehicle.ABS = ABS.Value;
            newVehicle.TCS = TCS.Value;
            newVehicle.ESP = ESP.Value;
            newVehicle.tractionHelper = tractionHelper.Value;
            newVehicle.useNOS = useNOS.Value;
            newVehicle.useTurbo = useTurbo.Value;

            //tuningDetails.setDetails(currentFrontBumper.Value, 0);
            //tuningDetails.setDetails(currentRearBumper.Value, 1);
            //tuningDetails.setDetails(currentExhaust.Value, 2);
            //tuningDetails.setDetails(currentExt.Value, 3);
            //tuningDetails.setDetails(currentFender.Value, 4);
            //tuningDetails.setDetails(currentRoof.Value, 5);
            //tuningDetails.setDetails(currentSpoiler.Value, 6);

            newVehicle.indicatorsOn = indicatorsOn.Value;

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

            applyEngineTorqueToExtraRearWheelColliders.Value = newVehicle.applyEngineTorqueToExtraRearWheelColliders;
            canGoReverseNow.Value = newVehicle.canGoReverseNow;
            engineTorque.Value = newVehicle.maxEngineTorque;
            brakeTorque.Value = newVehicle.brakeTorque;
            minEngineRPM.Value = newVehicle.minEngineRPM;
            maxEngineRPM.Value = newVehicle.maxEngineRPM;
            engineInertia.Value = newVehicle.engineInertia;
            useRevLimiter.Value = newVehicle.useRevLimiter;
            useRevLimiter.Value = newVehicle.useRevLimiter;
            useExhaustFlame.Value = newVehicle.useExhaustFlame;
            highspeedsteerAngle.Value = newVehicle.highspeedsteerAngle;
            highspeedsteerAngleAtspeed.Value = newVehicle.highspeedsteerAngleAtspeed;
            antiRollFrontHorizontal.Value = newVehicle.antiRollFrontHorizontal;
            antiRollRearHorizontal.Value = newVehicle.antiRollRearHorizontal;
            antiRollVertical.Value = newVehicle.antiRollVertical;
            maxspeed.Value = newVehicle.maxspeed;
            engineHeat.Value = newVehicle.engineHeat;
            engineHeatMultiplier.Value = newVehicle.engineHeatRate;
            totalGears.Value = newVehicle.totalGears;
            gearShiftingDelay.Value = newVehicle.gearShiftingDelay;
            gearShiftingThreshold.Value = newVehicle.gearShiftingThreshold;
            clutchInertia.Value = newVehicle.clutchInertia;
            NGear.Value = newVehicle.NGear;
            launched.Value = newVehicle.launched;
            ABS.Value = newVehicle.ABS;
            TCS.Value = newVehicle.TCS;
            ESP.Value = newVehicle.ESP;
            tractionHelper.Value = newVehicle.tractionHelper;
            useNOS.Value = newVehicle.useNOS;
            useTurbo.Value = newVehicle.useTurbo;

            indicatorsOn.Value = newVehicle.indicatorsOn;
        }
    }
}
