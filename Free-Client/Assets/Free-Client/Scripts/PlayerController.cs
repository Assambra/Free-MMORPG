using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Player Player;

    private bool[] inputs;
    private int clientTick;

    private Vector3 lastPosition;
    private int animcounter;

    private void Start()
    {
        inputs = new bool[4];
    }

    private void Update()
    {
        if (Player.Animator != null)
            CalculateAnimations();

        lastPosition = transform.position;

        if (!Player.IsLocalPlayer)
            return;
    }

    private void FixedUpdate()
    {
        clientTick++;

        GetUserInput();

        if (inputs[0] || inputs[1] || inputs[2] || inputs[3])
        {
            SendInput();

            for (int i = 0; i < inputs.Length; i++)
                inputs[i] = false;
        }
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
                Player.Animator.SetFloat("Vertical", 1f);
            else if (Vector3.Dot(forward.normalized, movement.normalized) > 0)
                Player.Animator.SetFloat("Vertical", -1f);

            animcounter = 0;
        }
        else
        {
            animcounter++;
            if (animcounter > 5)
            {
                Player.Animator.SetFloat("Vertical", 0f);
                animcounter = 0;
            }
        }  
    }

    private void SendInput()
    {
        NetworkManagerGame.Instance.SendPlayerInput(clientTick, inputs, Player.transform.rotation);
    }

    public void Move(Vector3 nextPosition)
    {
        Player.transform.position = nextPosition;
    }

    public void Rotate(Vector3 rotation)
    {
        Player.transform.rotation = Quaternion.Euler(rotation);
    }
}
