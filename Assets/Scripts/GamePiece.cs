using UnityEngine;
using System.Collections;

public class GamePiece : MonoBehaviour {
	Vector3 boundXZ;

	public bool Stable = true;
    public Rigidbody myRigidbody;
	public bool whiteUp = true;

	// Use this for initialization
	void Start () {
        myRigidbody = GetComponent<Rigidbody>();
        boundXZ = new Vector3(myRigidbody.position.x, 0, myRigidbody.position.z);
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonUp(1))
		{
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast (ray, out hit, 100)) {
				myRigidbody.AddExplosionForce(2000f, hit.point + Vector3.down/2, 8f);
			}
			
		}



		if(myRigidbody.velocity.magnitude < .0001 && myRigidbody.angularVelocity.magnitude < .00001)
		{
			//If we're not right-side up, flip
			if(whiteUp && myRigidbody.rotation.eulerAngles.z > 30 && myRigidbody.rotation.eulerAngles.z < 330)
			{
				StartCoroutine (flipTileCoroutine ());
				return;
			}
			if(!whiteUp && myRigidbody.rotation.eulerAngles.z < 150)
			{
				StartCoroutine (flipTileCoroutine ());
				return;
			}
			
			if (!EqualWithTolerance(myRigidbody.position.x, boundXZ.x))
			{
				float xVal = 100 * (boundXZ.x - myRigidbody.position.x);
				myRigidbody.velocity += Vector3.up;
				myRigidbody.AddForce(new Vector3(xVal, 400, 0));
				Stable = false;
			}
			else if (!EqualWithTolerance(myRigidbody.position.z, boundXZ.z))
			{ 
				float zVal = 100 * (boundXZ.z - myRigidbody.position.z);
				myRigidbody.velocity += Vector3.up;
				myRigidbody.AddForce(new Vector3(0, 400, zVal));
				Stable = false;
			}
			else
			{
				Stable = true;
			}
		}
		else
		{
			Stable = false;
		}
	}

	void FixedUpdate()
	{
//		if(myRigidbody.velocity.magnitude < .0001 && myRigidbody.angularVelocity.magnitude < .00001)
//		{
//			//If we're not right-side up, flip
//			if(whiteUp && myRigidbody.rotation.eulerAngles.z > 30 && myRigidbody.rotation.eulerAngles.z < 330)
//			{
//				StartCoroutine (flipTileCoroutine ());
//				return;
//			}
//			if(!whiteUp && myRigidbody.rotation.eulerAngles.z < 150)
//			{
//				StartCoroutine (flipTileCoroutine ());
//				return;
//			}
//
//			if (!EqualWithTolerance(myRigidbody.position.x, boundXZ.x))
//			{
//				float xVal = 100 * (boundXZ.x - myRigidbody.position.x);
//				myRigidbody.velocity += Vector3.up;
//				myRigidbody.AddForce(new Vector3(xVal, 400, 0));
//				Stable = false;
//			}
//			else if (!EqualWithTolerance(myRigidbody.position.z, boundXZ.z))
//			{ 
//				float zVal = 100 * (boundXZ.z - myRigidbody.position.z);
//				myRigidbody.velocity += Vector3.up;
//				myRigidbody.AddForce(new Vector3(0, 400, zVal));
//				Stable = false;
//			}
//			else
//			{
//				Stable = true;
//			}
//		}
//		else
//		{
//			Stable = false;
//		}
	}

	bool EqualWithTolerance(float one, float two)
	{
		float tolerance = 0.05f;
		return (two < one + tolerance && two > one - tolerance);
	}

	void OnMouseUp()
	{
		//StartCoroutine (flipTileCoroutine ());
		//flipTile ();
	}

	public void flipTile()
	{
		myRigidbody.velocity += Vector3.up;
		StartCoroutine (flipTileCoroutine ());
		whiteUp = !whiteUp;
	}

	private IEnumerator flipTileCoroutine()
	{
		myRigidbody.useGravity = false;
		while (myRigidbody.position.y < 2)
		{
			myRigidbody.AddForce (Vector3.up * 50 * (2 - myRigidbody.position.y));
			//myRigidbody.AddForce (Vector3.up * 560);
			yield return new WaitForSeconds(.05f);
		}
		yield return new WaitForSeconds(.15f);

		//myRigidbody.AddForce (Vector3.up * 100);
		myRigidbody.AddTorque (0, 0, 8);
		myRigidbody.useGravity = true;
        myRigidbody.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
           
    }
}
