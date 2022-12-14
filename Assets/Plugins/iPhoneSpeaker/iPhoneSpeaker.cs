using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
namespace Photon.Voice.Unity
{
	public class iPhoneSpeaker : MonoBehaviour
	{

#if UNITY_IPHONE
		[DllImport("__Internal")]
		private static extern void _forceToSpeaker();
#endif

#if UNITY_IPHONE
		[DllImport("__Internal")]
		private static extern void _checkiOSPrepare();
#endif

		public static void CheckiOSPrepare()
		{
#if UNITY_IPHONE
			if (Application.platform == RuntimePlatform.IPhonePlayer)
			{
				_checkiOSPrepare();
			}
#endif
		}

		public static void ForceToSpeaker()
		{
#if UNITY_IPHONE
			if (Application.platform == RuntimePlatform.IPhonePlayer)
			{
				_forceToSpeaker();
			}
#endif
		}
	}
}
