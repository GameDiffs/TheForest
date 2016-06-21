using System;
using UnityEngine;

public class PlatformerCharacter2D : MonoBehaviour
{
	private bool facingRight = true;

	[SerializeField]
	private float maxSpeed = 10f;

	[SerializeField]
	private float jumpForce = 400f;

	[Range(0f, 1f), SerializeField]
	private float crouchSpeed = 0.36f;

	[SerializeField]
	private bool airControl;

	[SerializeField]
	private LayerMask whatIsGround;

	private Transform groundCheck;

	private float groundedRadius = 0.2f;

	private bool grounded;

	private Transform ceilingCheck;

	private float ceilingRadius = 0.01f;

	private Animator anim;

	private void Awake()
	{
		this.groundCheck = base.transform.Find("GroundCheck");
		this.ceilingCheck = base.transform.Find("CeilingCheck");
		this.anim = base.GetComponent<Animator>();
	}

	private void FixedUpdate()
	{
		this.grounded = Physics2D.OverlapCircle(this.groundCheck.position, this.groundedRadius, this.whatIsGround);
		this.anim.SetBool("Ground", this.grounded);
		this.anim.SetFloatReflected("vSpeed", base.GetComponent<Rigidbody2D>().velocity.y);
	}

	public void Move(float move, bool crouch, bool jump)
	{
		if (!crouch && this.anim.GetBool("Crouch") && Physics2D.OverlapCircle(this.ceilingCheck.position, this.ceilingRadius, this.whatIsGround))
		{
			crouch = true;
		}
		this.anim.SetBool("Crouch", crouch);
		if (this.grounded || this.airControl)
		{
			move = ((!crouch) ? move : (move * this.crouchSpeed));
			this.anim.SetFloatReflected("Speed", Mathf.Abs(move));
			base.GetComponent<Rigidbody2D>().velocity = new Vector2(move * this.maxSpeed, base.GetComponent<Rigidbody2D>().velocity.y);
			if (move > 0f && !this.facingRight)
			{
				this.Flip();
			}
			else if (move < 0f && this.facingRight)
			{
				this.Flip();
			}
		}
		if (this.grounded && jump)
		{
			this.anim.SetBool("Ground", false);
			base.GetComponent<Rigidbody2D>().AddForce(new Vector2(0f, this.jumpForce));
		}
	}

	private void Flip()
	{
		this.facingRight = !this.facingRight;
		Vector3 localScale = base.transform.localScale;
		localScale.x *= -1f;
		base.transform.localScale = localScale;
	}
}
