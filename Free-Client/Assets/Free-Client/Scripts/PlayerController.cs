using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] Player player;
    [SerializeField] CharacterController characterController;

    private bool[] inputs;
    private int clientTick;

    private Vector3 currentPosition;
    private Vector3 lastPosition;
    private Vector3 nextPosition;
    private Quaternion currentRotation;
    private Quaternion nextRotation;

    private int animcounter;

    private void Start()
    {
        inputs = new bool[4];
    }

    private void Update()
    {
        if(player.IsLocalPlayer)
        {
            if (currentRotation != nextRotation)
            {
                var step = Time.fixedDeltaTime * 100f;
                player.transform.rotation = Quaternion.RotateTowards(player.transform.rotation, nextRotation, step);
                currentRotation = player.transform.rotation;
            }

            if (currentPosition != nextPosition)
            {
                var step = Time.fixedDeltaTime * 5.424f;
                player.transform.position = Vector3.MoveTowards(player.transform.position, nextPosition, step);
                currentPosition = player.transform.position;
            }
        }
        else
        {
            player.transform.rotation = nextRotation;
            /*
            if (currentRotation != nextRotation)
            {
                var step = Time.fixedDeltaTime * 100f;
                player.transform.rotation = Quaternion.RotateTowards(player.transform.rotation, nextRotation, step);
                currentRotation = player.transform.rotation;
            }
            */
            if (currentPosition != nextPosition)
            {
                var step = Time.fixedDeltaTime * 5.424f;
                player.transform.position = Vector3.MoveTowards(player.transform.position, nextPosition, step);
                currentPosition = player.transform.position;
            }
            
            //player.transform.position = nextPosition;
        }

        
        
        /*
        if (currentRotation != nextRotation)
        {
            var step = Time.fixedDeltaTime * 150f;
            player.transform.rotation = Quaternion.RotateTowards(player.transform.rotation, nextRotation, step);
        }
        */

        if (!player.IsLocalPlayer)
            return;

        GetUserInput();
        clientTick++;

        if (inputs[0] || inputs[1] || inputs[2] || inputs[3])
        {
            SendInput();
        }

        for (int i = 0; i < inputs.Length; i++)
            inputs[i] = false;
    }

    private void FixedUpdate()
    {
        CalculateAnimations();
        lastPosition = transform.position;

        if (!player.IsLocalPlayer)
            return;
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

    private void CalculateAnimations()
    {
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 movement = lastPosition - transform.position;

        if (Vector3.Dot(forward.normalized, movement.normalized) != 0)
        {
            if (Vector3.Dot(forward.normalized, movement.normalized) < 0)
                player.Animator.SetFloat("Vertical", 1f);
            else if (Vector3.Dot(forward.normalized, movement.normalized) > 0)
                player.Animator.SetFloat("Vertical", -1f);

            animcounter = 0;
        }
        else
        {
            animcounter++;
            if (animcounter > 3)
            {
                player.Animator.SetFloat("Vertical", 0f);
                animcounter = 0;
            }
        }  
    }

    private void SendInput()
    {
        NetworkManager.Instance.SendPlayerInput(clientTick, inputs, player.transform.rotation);
    }

    public void Move(Vector3 nextPosition)
    {
        this.nextPosition = nextPosition;
        //player.transform.position = nextPosition;
    }

    public void Rotate(Vector3 rotation)
    {
        nextRotation = Quaternion.Euler(rotation);
        //player.transform.rotation = Quaternion.Euler(rotation);
    }
}
