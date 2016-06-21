using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("ArrayMaker/ArrayList"), HutongGames.PlayMaker.Tooltip("Split a text asset or string into an arrayList")]
	public class SplitTextToArrayList : ArrayListActions
	{
		public enum ArrayMakerParseStringAs
		{
			String,
			Int,
			Float
		}

		public enum SplitSpecialChars
		{
			NewLine,
			Tab,
			Space
		}

		[ActionSection("Set up"), CheckForComponent(typeof(PlayMakerArrayListProxy)), RequiredField, HutongGames.PlayMaker.Tooltip("The gameObject with the PlayMaker ArrayList Proxy component")]
		public FsmOwnerDefault gameObject;

		[HutongGames.PlayMaker.Tooltip("Author defined Reference of the PlayMaker ArrayList Proxy component ( necessary if several component coexists on the same GameObject")]
		public FsmString reference;

		[HutongGames.PlayMaker.Tooltip("From where to start parsing, leave to 0 to start from the beginning")]
		public FsmInt startIndex;

		[HutongGames.PlayMaker.Tooltip("the range of parsing")]
		public FsmInt parseRange;

		[ActionSection("Source"), HutongGames.PlayMaker.Tooltip("Text asset source")]
		public TextAsset textAsset;

		[HutongGames.PlayMaker.Tooltip("Text Asset is ignored if this is set.")]
		public FsmString OrThisString;

		[ActionSection("Split"), HutongGames.PlayMaker.Tooltip("Split")]
		public SplitTextToArrayList.SplitSpecialChars split;

		[HutongGames.PlayMaker.Tooltip("Split is ignored if this value is not empty. Each chars taken in account for split")]
		public FsmString OrThisChar;

		[ActionSection("Value"), HutongGames.PlayMaker.Tooltip("Parse the line as a specific type")]
		public SplitTextToArrayList.ArrayMakerParseStringAs parseAsType;

		public override void Reset()
		{
			this.gameObject = null;
			this.reference = null;
			this.startIndex = null;
			this.parseRange = null;
			this.textAsset = null;
			this.split = SplitTextToArrayList.SplitSpecialChars.NewLine;
			this.parseAsType = SplitTextToArrayList.ArrayMakerParseStringAs.String;
		}

		public override void OnEnter()
		{
			if (base.SetUpArrayListProxyPointer(base.Fsm.GetOwnerDefaultTarget(this.gameObject), this.reference.Value))
			{
				this.splitText();
			}
			base.Finish();
		}

		public void splitText()
		{
			if (!base.isProxyValid())
			{
				return;
			}
			string text;
			if (this.OrThisString.Value.Length == 0)
			{
				if (this.textAsset == null)
				{
					return;
				}
				text = this.textAsset.text;
			}
			else
			{
				text = this.OrThisString.Value;
			}
			this.proxy.arrayList.Clear();
			string[] array;
			if (this.OrThisChar.Value.Length == 0)
			{
				char c = '\n';
				SplitTextToArrayList.SplitSpecialChars splitSpecialChars = this.split;
				if (splitSpecialChars != SplitTextToArrayList.SplitSpecialChars.Tab)
				{
					if (splitSpecialChars == SplitTextToArrayList.SplitSpecialChars.Space)
					{
						c = ' ';
					}
				}
				else
				{
					c = '\t';
				}
				array = text.Split(new char[]
				{
					c
				});
			}
			else
			{
				array = text.Split(this.OrThisChar.Value.ToCharArray());
			}
			int value = this.startIndex.Value;
			int num = array.Length;
			if (this.parseRange.Value > 0)
			{
				num = Mathf.Min(num - value, this.parseRange.Value);
			}
			string[] array2 = new string[num];
			int num2 = 0;
			for (int i = value; i < value + num; i++)
			{
				array2[num2] = array[i];
				num2++;
			}
			if (this.parseAsType == SplitTextToArrayList.ArrayMakerParseStringAs.String)
			{
				this.proxy.arrayList.InsertRange(0, array2);
			}
			else if (this.parseAsType == SplitTextToArrayList.ArrayMakerParseStringAs.Int)
			{
				int[] array3 = new int[array2.Length];
				int num3 = 0;
				string[] array4 = array2;
				for (int j = 0; j < array4.Length; j++)
				{
					string s = array4[j];
					int.TryParse(s, out array3[num3]);
					num3++;
				}
				this.proxy.arrayList.InsertRange(0, array3);
			}
			else if (this.parseAsType == SplitTextToArrayList.ArrayMakerParseStringAs.Float)
			{
				float[] array5 = new float[array2.Length];
				int num4 = 0;
				string[] array6 = array2;
				for (int k = 0; k < array6.Length; k++)
				{
					string s2 = array6[k];
					float.TryParse(s2, out array5[num4]);
					num4++;
				}
				this.proxy.arrayList.InsertRange(0, array5);
			}
		}
	}
}
