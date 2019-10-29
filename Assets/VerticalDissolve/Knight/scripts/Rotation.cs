using UnityEngine;
using UnityEngine.Serialization;

namespace VerticalDissolve.Knight.scripts
{
	public class Rotation : MonoBehaviour {

		[FormerlySerializedAs("Speed")] public float speed;
		[FormerlySerializedAs("Direction")] public Vector3 direction = Vector3.zero;

		private void Update () 
		{
			transform.Rotate(speed * Time.deltaTime * direction, Space.World);
		
		}
	}
}
