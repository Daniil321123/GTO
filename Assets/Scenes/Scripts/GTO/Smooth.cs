using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using UnityEngine;

public class Smooth : NetworkBehaviour
{
    [SerializeField] NetworkObject networkObject;
    private float smoothingDelay = 5f;
    private float updateDelay;

    private NetworkVariableVector3 correctPlayerPos = new NetworkVariableVector3();
    private NetworkVariableQuaternion correctPlayerRot = new NetworkVariableQuaternion();

    private void FixedUpdate()
    {
        if (!networkObject) return;
       
 
        if (!networkObject.IsLocalPlayer)
        {
            if (Vector3.Distance(transform.position, correctPlayerPos.Value) < 30f) //расстояние между игроками
            {
                transform.position = Vector3.Lerp(transform.position, correctPlayerPos.Value, Time.deltaTime * this.smoothingDelay);
                transform.rotation = Quaternion.Lerp(transform.rotation, correctPlayerRot.Value, Time.deltaTime * this.smoothingDelay);
            }
            else
            {
                transform.position = correctPlayerPos.Value;
                transform.rotation = correctPlayerRot.Value;
            }

        }
        else
        {
            updateDelay += Time.deltaTime;
            if (updateDelay > 0.11f)//раз в 110 милисекунд и было в update() deltaTime зависит от кадра
            {
                updateDelay = 0f;
                setPositionServerRpc(transform.position, transform.rotation);
            }
        }
    }

    [ServerRpc]
    private void setPositionServerRpc(Vector3 pos, Quaternion rot)
    {
        //проверки на читы 
        correctPlayerPos.Value = pos;
        correctPlayerRot.Value = rot;
    }
}
