using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class BoxController : MonoBehaviour {

	[SerializeField] private float speed = 1.0f;
	[SerializeField] private float rotateSpeed = 20.0f;
	[SerializeField] private float jumpPower = 10.0f;
	[SerializeField] private float groundCheckDistance = 0.1f;
	[Range(0.0f, 1.0f)][SerializeField] private float runExtraSpeedScale = 0.3f;      // speed * (mDamagable.Speed + runExtraSpeedScale)
	[Range(0.0f, 1.0f)][SerializeField] private float notGroundedScale = 0.5f;
	[Range(0.0f, 1.0f)][SerializeField] private float attackStateScale = 0.3f;
	[Range(0.0f, 1.0f)][SerializeField] private float damageStateScale = 0.3f;
	[SerializeField] private ParticleSystem deathEffect;
	[SerializeField] private ParticleSystem spawnEffect;
	[SerializeField] private ParticleSystem damageEffect;
	[SerializeField] private ParticleSystem jumpAttackEffect;

	private int DeathTimes = 0;

	private Rigidbody mRigidbody;
	private Collider mCollider;
	private Animator mAnimator;
	private Transform mGroundCheck;
	private Collider mAttackRange;
	private TriggerChecker mGroundTrigger;
	private Collider mHeadCheck;
	private List<Renderer> mBodyRenderers = new List<Renderer>();

	private float deathTime = 2.0f;

	private Vector3 jumpVelocity;
	private bool mSprinting = false;
	private float mSprintTimer = 0.0f;
	private bool mGrounded = true;
	private bool mDamaged = false;
	private bool mRoll = false;
	private bool mAttack = false;
	private int mAttackCount = 1;
	private bool mSprintAttack = false;
	private bool mJumpAttack = false;
	private bool attackHit = false;

	private bool controlByPlayer;

	private Damagable mDamagable;

	private Vector3 mRollVector;
	private float rollSpeed = 0.0f;
	private float rollInitialSpeed = 15.0f;
	bool rollable = true;

	private float sprintAttackSpeed = 0.0f;
	private float sprintAttackInitialSpeed = 0.0f;

	private int jumpAttackPhase = 0;
	private float jumpAttackInitialSpeed = 20.0f;

	public bool acting = false;
	private bool movable = true;
	public float fallingSpeed = 0.0f;

	//private float ATK = 3.0f;
	//private float HP = 10.0f;
	//private float AGI = 5.0f;

	void Awake()
	{
		if (transform.gameObject.GetComponent<UserInputController>()) controlByPlayer = true;
		else controlByPlayer = false;
	}

	// Use this for initialization
	void Start () {
		mRigidbody = transform.GetComponent<Rigidbody>();
		mCollider = transform.GetComponent<Collider>();
		mAnimator = transform.GetComponent<Animator>();
		mDamagable = transform.GetComponent<Damagable>();

		GameObject mainBody = transform.Find("Body").gameObject;
		mBodyRenderers = new List<Renderer>(mainBody.transform.GetComponentsInChildren<Renderer>());

		mGroundCheck = transform.FindChild("groundCheck").transform;
		mAttackRange = transform.FindChild("attackRange").GetComponent<Collider>();
		mGroundTrigger = transform.FindChild("groundCheckCollider").GetComponent<TriggerChecker>();
		mHeadCheck = transform.FindChild ("headCheckCollider").GetComponent<Collider> ();
	}

	void FixedUpdate()
	{
		GroundStateCheck();
		UpdateFallingSpeed();
	}

	public void Move(Vector3 mVector, bool mRunning)
	{
		Vector3 mMove = mVector;
		//check if movable
		if (!movable) mMove = Vector3.zero;


		if (mMove.magnitude > 0.0f) mSprinting = mRunning;
		else mSprinting = false;
		if (mSprinting) mSprintTimer += Time.fixedDeltaTime;
		else mSprintTimer = 0.0f;

		UpdateAnimator(mMove, mRunning);

		//normalize vector
		if (mMove.magnitude > 1.0f) mMove.Normalize();

		//rolling speed management
		if (rollSpeed > 0.01f || rollSpeed < -0.01f) rollSpeed -= Time.deltaTime * rollInitialSpeed / 1.0f;
		else
		{
			rollSpeed = 0.0f;
			mRollVector = Vector3.zero;
		}

		//sprint speed management
		if (sprintAttackSpeed > 0.01f || sprintAttackSpeed < -0.01f) sprintAttackSpeed -= Time.deltaTime * sprintAttackInitialSpeed / 1.0f;
		else 
		{
			sprintAttackSpeed = 0.0f;
		}

		//rotate transform with mMove if not attacking
		if (mMove.magnitude > 0.0f && !mAttack)
		{
			Vector3 lookRotationVector = transform.position + mMove - transform.position;
			if(lookRotationVector.magnitude > 0.1f)
			{
				Quaternion targetRotation = Quaternion.LookRotation(lookRotationVector);
				transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * rotateSpeed);
			}
		}

		//rotate character immediately if rolling
		if (mRollVector.magnitude > 0.1f)
		{
			transform.LookAt(transform.position + mRollVector);
		}

		Vector3 originalVelocity = mRigidbody.velocity;
		//OnAir move slower
		if (!mGrounded) mMove = mMove * notGroundedScale;
		//OnAttack move slower
		if (mAttack) mMove = mMove * attackStateScale;
		if (mDamaged) mMove = mMove * damageStateScale;
		//mMove = mMove * mDamagable.Speed;
		float extraSpeed = (mRunning) ? mDamagable.Speed + runExtraSpeedScale : mDamagable.Speed; 
		mRigidbody.velocity = mMove * speed * extraSpeed + new Vector3(0, originalVelocity.y, 0);
		if (!mGrounded) mRigidbody.velocity += jumpVelocity * (1.0f-notGroundedScale);

		//mRigidbody.velocity += transform.forward * rollSpeed;

		//add rolling direction and speed
		mRigidbody.velocity += (mRollVector.magnitude > 0.1f) ? mRollVector * rollSpeed : transform.forward * rollSpeed;

		//add sprintAttack direction and speed
		mRigidbody.velocity += transform.forward * sprintAttackSpeed;

		//jump attack
		if (mJumpAttack) {
			//set to initial speed
			if(jumpAttackPhase==0)
				mRigidbody.velocity = new Vector3 (0, 0, 0);
			else if(jumpAttackPhase==1)
				mRigidbody.velocity = new Vector3 (0, -jumpAttackInitialSpeed, 0);
		}
	}

	public void Jump()
	{
		//jump if grounded
		if (mGrounded && !acting /*&& Mathf.Abs(mRigidbody.velocity.y) < 0.1f */)
		{
			mRigidbody.velocity = new Vector3(mRigidbody.velocity.x, jumpPower, mRigidbody.velocity.z);
			jumpVelocity = new Vector3( mRigidbody.velocity.x, 0, mRigidbody.velocity.z);
		}
	}

	public void Roll(Vector3 dir)
	{
		if (rollable && !acting)
		{
			//mAnimator.Play("Roll");
			mAnimator.Play("Roll", -1, 0.0f);
			rollSpeed = rollInitialSpeed;
			mRollVector = dir;
			startRoll();
		}
	}

	public void Damage(Vector3 dir)
	{
		mDamaged = true;
		mRigidbody.velocity = new Vector3(dir.x * 4.0f, 3.0f, dir.z * 4.0f);
		jumpVelocity = new Vector3( mRigidbody.velocity.x, 0, mRigidbody.velocity.z);
		Destroy(Instantiate(damageEffect.gameObject, transform.position, transform.rotation) as GameObject, 0.8f);
	}


	//check if character grounded, update mGrounded
	void GroundStateCheck()
	{
		/*
        RaycastHit hit;
        if (Physics.Raycast(mGroundCheck.position + Vector3.up * 0.1f, Vector3.down, out hit, groundCheckDistance)) 
            mGrounded = true;
        else 
            mGrounded = false;
        */
		//leaving ground
		if (mGrounded == true && mGroundTrigger.isGrounded() == false)
		{
			if (mDamaged == false) jumpVelocity = new Vector3(mRigidbody.velocity.x, 0, mRigidbody.velocity.z);
		}
		//entering ground
		else if (mGrounded == false && mGroundTrigger.isGrounded() == true)
		{
			if (mDamaged == true) mDamaged = false;
			//falling damage maangement
			if ( !mJumpAttack && !mRoll && fallingSpeed > 9.0f) 
			{
				mDamagable.Damage(Vector3.zero, fallingSpeed * 2.0f);
				fallingSpeed = 0.0f;
			}
		}
		float VY = -mRigidbody.velocity.y;

		if ( (!mGrounded && mGroundTrigger.isGrounded ()) || (VY < 0.1f && VY > 0.0f)) 
		{
			if (mJumpAttack)
				JumpAttackCompleteEvent ();
		}

		mGrounded = mGroundTrigger.isGrounded();
	}

	void UpdateFallingSpeed()
	{
		fallingSpeed = (-mRigidbody.velocity.y > fallingSpeed) ? -mRigidbody.velocity.y : fallingSpeed;
	}


	/// 
	/// 
	/// Animation Control
	/// 
	/// 

	public void UpdateAnimator(Vector3 move, bool mRunning)
	{
		//in attack
		if (mAttack)
			mAnimator.Play ("boxAttack00");
		else if (mSprintAttack)
			mAnimator.Play ("SprintAttack");
		else if (mJumpAttack) 
		{
			if (jumpAttackPhase == 0)
				mAnimator.SetBool ("JumpAttack", true);
			else if (jumpAttackPhase == 2) 
			{
				mAnimator.SetBool ("JumpAttackLoop", true);
			}

		}
		//in moving
		if (move.magnitude > 0.0f && mGrounded)
		{
			mAnimator.SetBool("Walking", true);
			if (mRunning) mAnimator.speed = mDamagable.Speed + runExtraSpeedScale;
			else mAnimator.speed = mDamagable.Speed;
		}
		else
		{
			mAnimator.SetBool("Walking", false);
			mAnimator.speed = 1.0f;
		}
	}
	/*
	if (rollable && !acting)
	{
		//mAnimator.Play("Roll");
		mAnimator.Play("Roll", -1, 0.0f);
		rollSpeed = rollInitialSpeed;
		mRollVector = dir;
		startRoll();
	}*/


	public void Attack()
	{
		if (acting) return;
		//jump attack
		//sprint attack
		if (mSprintTimer > 0.4f) 
		{
			mSprintAttack = true;
			acting = true;
			disableableMovable ();
			sprintAttackInitialSpeed = mRigidbody.velocity.magnitude;
			sprintAttackSpeed = sprintAttackInitialSpeed;
		}
		//jump attack
		else if((!mGrounded && Mathf.Abs(mRigidbody.velocity.y) > 0.1f ) && !mDamaged )
		{
			mJumpAttack = true;
			acting = true;
			disableableMovable ();
			jumpAttackPhase = 0;
		}
		//normal attack
		else 
		{
			mAttack = true;
			acting = true;
			disableableMovable();
		}
	}

	public void AttackCompleteEvent()
	{
		mAttack = false;
		acting = false;
		enableMovable();
	}

	public void AttackHitEvent()
	{
		attackHit = true;
		GetComponent<Damagable>().HitEvent(0.3f, 1.0f);
	}

	public void AttackLeaveEvent()
	{
		attackHit = false;
	}

	public void SprintAttackCompleteEvent()
	{
		mSprintAttack = false;
		acting = false;
		enableMovable();
	}

	public void SprintAttackHitEvent()
	{
		attackHit = true;
		GetComponent<Damagable>().HitEvent(0.5f, 5.0f);
	}

	public void SprintAttackLeaveEvent()
	{

		attackHit = false;
	}

	public void JumpAttackCompleteEvent()
	{
		mJumpAttack = false;
		acting = false;
		enableMovable();
		fallingSpeed = 0.0f;
		jumpAttackPhase = 0;
		mAnimator.SetBool ("JumpAttackLoop", false);
		mAnimator.SetBool ("JumpAttack", false);
		JumpAttackHitGroundEffect ();
	}

	public void JumpAttackHitEvent()
	{
		attackHit = true;
		GetComponent<Damagable>().HitEvent(1.0f, 5.0f, -transform.forward * 0.5f, true);
	}

	public void JumpAttackLeaveEvent()
	{
		attackHit = false;
	}

	public void JumpAttackFallingStart()
	{
		jumpAttackPhase = 1;
	}

	public void JumpAttackFallingLoopStart()
	{
		jumpAttackPhase = 2;
	}

	void JumpAttackHitGroundEffect()
	{
		Destroy(Instantiate(jumpAttackEffect.gameObject, transform.position - transform.up * 0.4f, Quaternion.Euler(270.0f, 180.0f, 0)) as GameObject, jumpAttackEffect.startLifetime);
	}

	public void startRoll()
	{
		Debug.Log("STARTROLL");
		disableableMovable();
		rollable = false;
		mRoll = true;
		acting = true;
	}

	public void endRoll()
	{
		Debug.Log("ENDROLL");
		rollable = true;
		acting = false;
		mRoll = false;
	}

	public void enableMovable()
	{
		movable = true;
	}

	public void disableableMovable()
	{
		movable = false;
	}

	public void Death()
	{
		Debug.Log(" die." + DeathTimes);
		DeathTimes++;
		jumpVelocity = Vector3.zero;

		//mBody.SetActive(false);
		mRigidbody.constraints = RigidbodyConstraints.FreezePosition;
		mRigidbody.velocity = Vector3.zero;
		mRigidbody.angularVelocity = Vector3.zero;
		mCollider.enabled = false;
		mHeadCheck.enabled = false;
		SetRenderer(false);
		Destroy(Instantiate(deathEffect.gameObject, transform.position, transform.rotation) as GameObject, deathEffect.startLifetime);
		if (!controlByPlayer) Destroy(this);
		else
		{
			transform.gameObject.GetComponent<UserInputController>().SetEnable(false);
		}
	}

	public void SetRenderer(bool set)
	{
		foreach (Renderer element in mBodyRenderers)
		{
			element.enabled = set;
		}
	}

	public void lerpShow()
	{
		foreach (Renderer element in mBodyRenderers)
		{
			element.material.DOFade(0.0f, 1.0f).From();
		}
	}

	public void UseItem()
	{
		// acting = true;
		if (acting) return;

		mDamagable.UseItem();
	}
}
