using UnityEngine;

[ModInfo("FunnyMod", "NullCore-Systems", "1.0.0")] 
public class FunnyMod : IMod
{
    private Rigidbody2D player;
    private SpriteRenderer renderer;
    public static bool wantToUnload = false;

    private Vector3 velocity;
    private Vector3 targetVelocity;
    private float changeInterval = 2f;
    private float timeSinceLastChange = 0f;
    private float maxSpeed = 2.5f;
    private float minSpeed = 0.5f;
    private float smoothTime = 1.5f;

    private Vector3 velocitySmoothDamp;
    private float hue = 0f;
    private bool rainbowMode = true;

    private Camera cam;
    private float screenMargin = 0.15f; // Margin to avoid going too close to the screen edge

    public void OnLoad()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        if (playerObj != null)
        {
            player = playerObj.GetComponent<Rigidbody2D>();
            renderer = playerObj.GetComponent<SpriteRenderer>();
            cam = Object.FindObjectOfType<Camera>();

            targetVelocity = GetRandomVelocity();
            velocity = targetVelocity;
        }
    }

    public void OnUpdate()
    {
        // Attempt recovery if anything is null
        if (player == null || renderer == null || cam == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.GetComponent<Rigidbody2D>();
                renderer = playerObj.GetComponent<SpriteRenderer>();
            }

            cam = Camera.main ?? Object.FindObjectOfType<Camera>();

            // Still missing essentials? Wait for next frame
            if (player == null || renderer == null || cam == null)
                return;

            Debug.Log("[FunnyMod] Recovered references after null!");
            return;
        }

        timeSinceLastChange += Time.deltaTime;

        if (timeSinceLastChange > changeInterval)
        {
            targetVelocity = GetRandomVelocity();
            timeSinceLastChange = 0f;
        }

        // Avoid screen edges
        Vector3 viewportPos = cam.WorldToViewportPoint(player.transform.position);
        Vector3 adjustment = Vector3.zero;

        if (viewportPos.x < screenMargin) adjustment.x = 1f;
        else if (viewportPos.x > 1f - screenMargin) adjustment.x = -1f;

        if (viewportPos.y < screenMargin) adjustment.y = 1f;
        else if (viewportPos.y > 1f - screenMargin) adjustment.y = -1f;

        if (adjustment != Vector3.zero)
        {
            adjustment.Normalize();
            targetVelocity = Vector3.Lerp(targetVelocity, adjustment * maxSpeed, 0.5f);
        }

        velocity = Vector3.SmoothDamp(velocity, targetVelocity, ref velocitySmoothDamp, smoothTime);

        // Move
        Vector3 newPosition = player.transform.position + velocity * Time.deltaTime;
        player.transform.position = newPosition;

        // Rainbow mode
        if (rainbowMode)
        {
            hue += Time.deltaTime * 0.5f;
            if (hue > 1f) hue -= 1f;
            renderer.color = Color.HSVToRGB(hue, 1f, 1f);
        }
    }

    private Vector3 GetRandomVelocity()
    {
        float speed = Random.Range(minSpeed, maxSpeed);
        Vector2 dir = Random.insideUnitCircle.normalized;
        return new Vector3(dir.x, dir.y, 0) * speed;
    }

    public void OnUnload()
    {
        Debug.Log("FunnyMod unloaded.");
    }

    public bool ShouldUnload()
    {
        return wantToUnload;
    }
}
