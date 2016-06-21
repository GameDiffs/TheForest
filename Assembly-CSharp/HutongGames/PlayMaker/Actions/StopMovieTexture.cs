using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Movie), HutongGames.PlayMaker.Tooltip("Stops playing the Movie Texture, and rewinds it to the beginning.")]
	public class StopMovieTexture : FsmStateAction
	{
		[ObjectType(typeof(MovieTexture)), RequiredField]
		public FsmObject movieTexture;

		public override void Reset()
		{
			this.movieTexture = null;
		}

		public override void OnEnter()
		{
			MovieTexture movieTexture = this.movieTexture.Value as MovieTexture;
			if (movieTexture != null)
			{
				movieTexture.Stop();
			}
			base.Finish();
		}
	}
}
