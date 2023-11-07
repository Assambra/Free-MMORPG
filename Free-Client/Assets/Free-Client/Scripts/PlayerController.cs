using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Player Player;

    private bool[] inputs;
    private int clientTick;
    
    private Vector3 nextPosition;
    private Quaternion nextRotation;


    private void Start()
    {
        if(Player.IsLocalPlayer)
            inputs = new bool[4];
    }

    private void Update()
    {
        if (Player.Animator != null)
            CalculateAnimations();

        if (transform.position != nextPosition)
        {
            float step = Time.deltaTime * 5.424f;
            transform.position = Vector3.MoveTowards(transform.position, nextPosition, step);
        }

        if (transform.rotation != nextRotation)
        {
            float step = Time.deltaTime * 120f;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, nextRotation, step);
        }
    }

    private void FixedUpdate()
    {
        if (Player.IsLocalPlayer)
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
        Vector3 movement = transform.position - nextPosition;

        if (Vector3.Dot(forward.normalized, movement.normalized) != 0)
        {
            if (Vector3.Dot(forward.normalized, movement.normalized) < 0)
                Player.Animator.SetFloat("Vertical", 1f);
            else if (Vector3.Dot(forward.normalized, movement.normalized) > 0)
                Player.Animator.SetFloat("Vertical", -1f);
        }
        else
            Player.Animator.SetFloat("Vertical", 0f);
    }

    private void SendInput()
    {
        NetworkManagerGame.Instance.SendPlayerInput(clientTick, inputs, Player.transform.rotation);
    }

    public void Move(Vector3 nextPosition)
    {
        this.nextPosition = nextPosition;
    }

    public void Rotate(Vector3 nextRotation)
    {
        this.nextRotation = Quaternion.Euler(nextRotation);
    }
}
