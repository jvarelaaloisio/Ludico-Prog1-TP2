using System;
using UnityEngine;

namespace VarelaAloisio.Core
{
	/// <summary>
	/// Serializable wrapper for animation parameters to ease the use of hashes.
	/// </summary>
	[Serializable]
	public class AnimatorParameter
	{
		[field: SerializeField] public string Name { get; set; }

		private int? _hash;

		/// <summary /> Cached hash value for this animation
		public int Hash => _hash ??= Animator.StringToHash(Name);

		public AnimatorParameter(string name)
			=> this.Name = name;

		/// <summary /> Calls <see cref="Animator.Play(int)"/>
		/// <param name="animator">target animator</param>
		public void Play(in Animator animator)
			=> animator.Play(Hash);

		public void CrossFade(in Animator animator, float duration)
			=> animator.CrossFade(Hash, duration);

		/// <summary /> Calls <see cref="Animator.SetFloat(int,float)"/>
		/// <param name="animator">target animator</param>
		/// <param name="value">value to set to</param>
		public void SetFloat(in Animator animator, float value)
			=> animator.SetFloat(Hash, value);

		/// <summary /> Calls <see cref="Animator.SetBool(int,bool)"/>
		/// <param name="animator">target animator</param>
		/// <param name="value">value to set to</param>
		public void SetBool(in Animator animator, bool value)
			=> animator.SetBool(Hash, value);

		/// <summary /> Calls <see cref="Animator.SetInteger(int,int)"/>
		/// <param name="animator">target animator</param>
		/// <param name="value">value to set to</param>
		public void SetInteger(in Animator animator, int value)
			=> animator.SetInteger(Hash, value);

		/// <summary>
		/// Calls <see cref="Animator.SetTrigger(int)"/>
		/// </summary>
		/// <param name="animator">target animator</param>
		public void SetTrigger(in Animator animator)
			=> animator.SetTrigger(Hash);

		/// <summary>
		/// Calls <see cref="Animator.ResetTrigger(int)"/>
		/// </summary>
		/// <param name="animator">target animator</param>
		public void ResetTrigger(in Animator animator)
			=> animator.ResetTrigger(Hash);

		public static implicit operator int(AnimatorParameter param)
			=> param.Hash;

		public override string ToString()
			=> Name;
	}
}