using Specular.Runtime.Scripts.Portal;
using UnityEngine;

namespace Specular.Samples.Demo {
  public class FpsController : PortalTraveller {
    public float walkSpeed = 3;
    public float runSpeed = 6;
    public float smoothMoveTime = 0.1f;
    public float jumpForce = 8;
    public float gravity = 18;

    public bool lockCursor;
    public float mouseSensitivity = 10;
    public Vector2 pitchMinMax = new Vector2(-40, 85);
    public float rotationSmoothTime = 0.1f;

    CharacterController _controller;
    Camera _cam;
    public float yaw;
    public float pitch;
    float _smooth_yaw;
    float _smooth_pitch;

    float _yaw_smooth_v;
    float _pitch_smooth_v;
    float _vertical_velocity;
    Vector3 _velocity;
    Vector3 _smooth_v;
    Vector3 _rotation_smooth_velocity;
    Vector3 _current_rotation;

    bool _jumping;
    float _last_grounded_time;
    bool _disabled;

    void Start() {
      this._cam = Camera.main;
      if (this.lockCursor) {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
      }

      this._controller = this.GetComponent<CharacterController>();

      this.yaw = this.transform.eulerAngles.y;
      this.pitch = this._cam.transform.localEulerAngles.x;
      this._smooth_yaw = this.yaw;
      this._smooth_pitch = this.pitch;
    }

    void Update() {
      if (Input.GetKeyDown(key : KeyCode.P)) {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Debug.Break();
      }

      if (Input.GetKeyDown(key : KeyCode.O)) {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        this._disabled = !this._disabled;
      }

      if (this._disabled) {
        return;
      }

      var input = new Vector2(x : Input.GetAxisRaw("Horizontal"), y : Input.GetAxisRaw("Vertical"));

      var input_dir = new Vector3(x : input.x, 0, z : input.y).normalized;
      var world_input_dir = this.transform.TransformDirection(direction : input_dir);

      var current_speed = (Input.GetKey(key : KeyCode.LeftShift)) ? this.runSpeed : this.walkSpeed;
      var target_velocity = world_input_dir * current_speed;
      this._velocity = Vector3.SmoothDamp(current : this._velocity,
                                          target : target_velocity,
                                          currentVelocity : ref this._smooth_v,
                                          smoothTime : this.smoothMoveTime);

      this._vertical_velocity -= this.gravity * Time.deltaTime;
      this._velocity = new Vector3(x : this._velocity.x, y : this._vertical_velocity, z : this._velocity.z);

      var flags = this._controller.Move(motion : this._velocity * Time.deltaTime);
      if (flags == CollisionFlags.Below) {
        this._jumping = false;
        this._last_grounded_time = Time.time;
        this._vertical_velocity = 0;
      }

      if (Input.GetKeyDown(key : KeyCode.Space)) {
        var time_since_last_touched_ground = Time.time - this._last_grounded_time;
        if (this._controller.isGrounded || (!this._jumping && time_since_last_touched_ground < 0.15f)) {
          this._jumping = true;
          this._vertical_velocity = this.jumpForce;
        }
      }

      var m_x = Input.GetAxisRaw("Mouse X");
      var m_y = Input.GetAxisRaw("Mouse Y");

      // Verrrrrry gross hack to stop camera swinging down at start
      var m_mag = Mathf.Sqrt(f : m_x * m_x + m_y * m_y);
      if (m_mag > 5) {
        m_x = 0;
        m_y = 0;
      }

      this.yaw += m_x * this.mouseSensitivity;
      this.pitch -= m_y * this.mouseSensitivity;
      this.pitch = Mathf.Clamp(value : this.pitch, min : this.pitchMinMax.x, max : this.pitchMinMax.y);
      this._smooth_pitch = Mathf.SmoothDampAngle(current : this._smooth_pitch,
                                                 target : this.pitch,
                                                 currentVelocity : ref this._pitch_smooth_v,
                                                 smoothTime : this.rotationSmoothTime);
      this._smooth_yaw = Mathf.SmoothDampAngle(current : this._smooth_yaw,
                                               target : this.yaw,
                                               currentVelocity : ref this._yaw_smooth_v,
                                               smoothTime : this.rotationSmoothTime);

      this.transform.eulerAngles = Vector3.up * this._smooth_yaw;
      this._cam.transform.localEulerAngles = Vector3.right * this._smooth_pitch;
    }

    public override void Teleport(Transform from_portal, Transform to_portal, Vector3 pos, Quaternion rot) {
      this.transform.position = pos;
      var euler_rot = rot.eulerAngles;
      var delta = Mathf.DeltaAngle(current : this._smooth_yaw, target : euler_rot.y);
      this.yaw += delta;
      this._smooth_yaw += delta;
      this.transform.eulerAngles = Vector3.up * this._smooth_yaw;
      this._velocity =
          to_portal.TransformVector(vector : from_portal.InverseTransformVector(vector : this._velocity));
      Physics.SyncTransforms();
    }
  }
}
