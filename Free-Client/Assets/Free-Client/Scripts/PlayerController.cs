using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] Player player;

    private bool[] inputs;
    private int clientTick;

    private Vector3 currentPosition;
    private Vector3 nextPosition;


    private void Start()
    {
        inputs = new bool[4];
    }

    private void Update()
    {
        if(currentPosition != nextPosition)
        {
            var step = Time.deltaTime * 2;
            player.transform.position = Vector3.MoveTowards(player.transform.position, nextPosition, step);
        }
        
        if (!player.IsLocalPlayer)
            return;

        GetUserInput();
    }

    private void FixedUpdate()
    {
        if (!player.IsLocalPlayer)
            return;

        clientTick++;

        if (inputs[0] || inputs[1] || inputs[2] || inputs[3])
        {
            SendInput();
        }

        for (int i = 0; i < inputs.Length; i++)
            inputs[i] = false;
    }

    private void GetUserInput()
    {
        if (Input.GetKey(KeyCode.W))
            inputs[0] = true;

        if (Input.GetKey(KeyCode.S))
            inputs[1] = true;

        if (Input.GetKey(KeyCode.A))
            inputs[2] = true;

        if (Input.GetKey(KeyCode.D))
            inputs[3] = true;
    }

    private void SendInput()
    {
        NetworkManager.Instance.SendPlayerInput(clientTick, inputs, player.transform.rotation);
    }

    public void Move(Vector3 nextPosition)
    {
        this.nextPosition = nextPosition;
    }
}
