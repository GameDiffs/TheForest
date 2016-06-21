using System;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GameObject), Tooltip("Selects a Random Game Object from an array of Game Objects.")]
	public class SelectRandomGameObject : FsmStateAction
	{
		[CompoundArray("Game Objects", "Game Object", "Weight")]
		public FsmGameObject[] gameObjects;

		[HasFloatSlider(0f, 1f)]
		public FsmFloat[] weights;

		[RequiredField, UIHint(UIHint.Variable)]
		public FsmGameObject storeGameObject;

		public override void Reset()
		{
			this.gameObjects = new FsmGameObject[3];
			this.weights = new FsmFloat[]
			{
				1f,
				1f,
				1f
			};
			this.storeGameObject = null;
		}

		public override void OnEnter()
		{
			this.DoSelectRandomGameObject();
			base.Finish();
		}

		private void DoSelectRandomGameObject()
		{
			if (this.gameObjects == null)
			{
				return;
			}
			if (this.gameObjects.Length == 0)
			{
				return;
			}
			if (this.storeGameObject == null)
			{
				return;
			}
			int randomWeightedIndex = ActionHelpers.GetRandomWeightedIndex(this.weights);
			if (randomWeightedIndex != -1)
			{
				this.storeGameObject.Value = this.gameObjects[randomWeightedIndex].Value;
			}
		}
	}
}
