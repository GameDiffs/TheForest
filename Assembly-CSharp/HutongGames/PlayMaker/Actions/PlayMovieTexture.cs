using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Movie), HutongGames.PlayMaker.Tooltip("Plays a Movie Texture. Use the Movie Texture in a Material, or in the GUI.")]
	public class PlayMovieTexture : FsmStateAction
	{
		[ObjectType(typeof(MovieTexture)), RequiredField]
		public FsmObject movieTexture;

		public FsmBool loop;

		public override void Reset()
		{
			this.movieTexture = null;
			this.loop = false;
		}

		public override void OnEnter()
		{
			MovieTexture movieTexture = this.movieTexture.Value as MovieTexture;
			if (movieTexture != null)
			{
				movieTexture.loop = this.loop.Value;
				movieTexture.Play();
			}
			base.Finish();
		}
	}
}
