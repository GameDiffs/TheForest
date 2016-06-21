using System;
using System.Collections.Generic;
using UnityEngine;

namespace DigitalOpus.MB.Core
{
	public class MB2_TexturePacker
	{
		private class PixRect
		{
			public int x;

			public int y;

			public int w;

			public int h;

			public PixRect()
			{
			}

			public PixRect(int xx, int yy, int ww, int hh)
			{
				this.x = xx;
				this.y = yy;
				this.w = ww;
				this.h = hh;
			}
		}

		private class Image
		{
			public int imgId;

			public int w;

			public int h;

			public int x;

			public int y;

			public Image(int id, int tw, int th, int padding, int minImageSizeX, int minImageSizeY)
			{
				this.imgId = id;
				this.w = Mathf.Max(tw + padding * 2, minImageSizeX);
				this.h = Mathf.Max(th + padding * 2, minImageSizeY);
			}

			public Image(MB2_TexturePacker.Image im)
			{
				this.imgId = im.imgId;
				this.w = im.w;
				this.h = im.h;
				this.x = im.x;
				this.y = im.y;
			}
		}

		private class ImgIDComparer : IComparer<MB2_TexturePacker.Image>
		{
			public int Compare(MB2_TexturePacker.Image x, MB2_TexturePacker.Image y)
			{
				if (x.imgId > y.imgId)
				{
					return 1;
				}
				if (x.imgId == y.imgId)
				{
					return 0;
				}
				return -1;
			}
		}

		private class ImageHeightComparer : IComparer<MB2_TexturePacker.Image>
		{
			public int Compare(MB2_TexturePacker.Image x, MB2_TexturePacker.Image y)
			{
				if (x.h > y.h)
				{
					return -1;
				}
				if (x.h == y.h)
				{
					return 0;
				}
				return 1;
			}
		}

		private class ImageWidthComparer : IComparer<MB2_TexturePacker.Image>
		{
			public int Compare(MB2_TexturePacker.Image x, MB2_TexturePacker.Image y)
			{
				if (x.w > y.w)
				{
					return -1;
				}
				if (x.w == y.w)
				{
					return 0;
				}
				return 1;
			}
		}

		private class ImageAreaComparer : IComparer<MB2_TexturePacker.Image>
		{
			public int Compare(MB2_TexturePacker.Image x, MB2_TexturePacker.Image y)
			{
				int num = x.w * x.h;
				int num2 = y.w * y.h;
				if (num > num2)
				{
					return -1;
				}
				if (num == num2)
				{
					return 0;
				}
				return 1;
			}
		}

		private class ProbeResult
		{
			public int w;

			public int h;

			public MB2_TexturePacker.Node root;

			public bool fitsInMaxSize;

			public float efficiency;

			public float squareness;

			public void Set(int ww, int hh, MB2_TexturePacker.Node r, bool fits, float e, float sq)
			{
				this.w = ww;
				this.h = hh;
				this.root = r;
				this.fitsInMaxSize = fits;
				this.efficiency = e;
				this.squareness = sq;
			}

			public float GetScore(bool doPowerOfTwoScore)
			{
				float num = (!this.fitsInMaxSize) ? 0f : 1f;
				if (doPowerOfTwoScore)
				{
					return num * 2f + this.efficiency;
				}
				return this.squareness + 2f * this.efficiency + num;
			}
		}

		private class Node
		{
			public MB2_TexturePacker.Node[] child = new MB2_TexturePacker.Node[2];

			public MB2_TexturePacker.PixRect r;

			public MB2_TexturePacker.Image img;

			private bool isLeaf()
			{
				return this.child[0] == null || this.child[1] == null;
			}

			public MB2_TexturePacker.Node Insert(MB2_TexturePacker.Image im, bool handed)
			{
				int num;
				int num2;
				if (handed)
				{
					num = 0;
					num2 = 1;
				}
				else
				{
					num = 1;
					num2 = 0;
				}
				if (!this.isLeaf())
				{
					MB2_TexturePacker.Node node = this.child[num].Insert(im, handed);
					if (node != null)
					{
						return node;
					}
					return this.child[num2].Insert(im, handed);
				}
				else
				{
					if (this.img != null)
					{
						return null;
					}
					if (this.r.w < im.w || this.r.h < im.h)
					{
						return null;
					}
					if (this.r.w == im.w && this.r.h == im.h)
					{
						this.img = im;
						return this;
					}
					this.child[num] = new MB2_TexturePacker.Node();
					this.child[num2] = new MB2_TexturePacker.Node();
					int num3 = this.r.w - im.w;
					int num4 = this.r.h - im.h;
					if (num3 > num4)
					{
						this.child[num].r = new MB2_TexturePacker.PixRect(this.r.x, this.r.y, im.w, this.r.h);
						this.child[num2].r = new MB2_TexturePacker.PixRect(this.r.x + im.w, this.r.y, this.r.w - im.w, this.r.h);
					}
					else
					{
						this.child[num].r = new MB2_TexturePacker.PixRect(this.r.x, this.r.y, this.r.w, im.h);
						this.child[num2].r = new MB2_TexturePacker.PixRect(this.r.x, this.r.y + im.h, this.r.w, this.r.h - im.h);
					}
					return this.child[num].Insert(im, handed);
				}
			}
		}

		public MB2_LogLevel LOG_LEVEL = MB2_LogLevel.info;

		private MB2_TexturePacker.ProbeResult bestRoot;

		public bool doPowerOfTwoTextures = true;

		private static void printTree(MB2_TexturePacker.Node r, string spc)
		{
			if (r.child[0] != null)
			{
				MB2_TexturePacker.printTree(r.child[0], spc + "  ");
			}
			if (r.child[1] != null)
			{
				MB2_TexturePacker.printTree(r.child[1], spc + "  ");
			}
		}

		private static void flattenTree(MB2_TexturePacker.Node r, List<MB2_TexturePacker.Image> putHere)
		{
			if (r.img != null)
			{
				r.img.x = r.r.x;
				r.img.y = r.r.y;
				putHere.Add(r.img);
			}
			if (r.child[0] != null)
			{
				MB2_TexturePacker.flattenTree(r.child[0], putHere);
			}
			if (r.child[1] != null)
			{
				MB2_TexturePacker.flattenTree(r.child[1], putHere);
			}
		}

		private static void drawGizmosNode(MB2_TexturePacker.Node r)
		{
			Vector3 size = new Vector3((float)r.r.w, (float)r.r.h, 0f);
			Vector3 center = new Vector3((float)r.r.x + size.x / 2f, (float)(-(float)r.r.y) - size.y / 2f, 0f);
			Gizmos.DrawWireCube(center, size);
			if (r.img != null)
			{
				Gizmos.color = Color.blue;
				size = new Vector3((float)r.img.w, (float)r.img.h, 0f);
				center = new Vector3((float)r.img.x + size.x / 2f, (float)(-(float)r.img.y) - size.y / 2f, 0f);
				Gizmos.DrawCube(center, size);
			}
			if (r.child[0] != null)
			{
				Gizmos.color = Color.red;
				MB2_TexturePacker.drawGizmosNode(r.child[0]);
			}
			if (r.child[1] != null)
			{
				Gizmos.color = Color.green;
				MB2_TexturePacker.drawGizmosNode(r.child[1]);
			}
		}

		private static Texture2D createFilledTex(Color c, int w, int h)
		{
			Texture2D texture2D = new Texture2D(w, h);
			for (int i = 0; i < w; i++)
			{
				for (int j = 0; j < h; j++)
				{
					texture2D.SetPixel(i, j, c);
				}
			}
			texture2D.Apply();
			return texture2D;
		}

		public void DrawGizmos()
		{
			if (this.bestRoot != null)
			{
				MB2_TexturePacker.drawGizmosNode(this.bestRoot.root);
			}
		}

		private bool Probe(MB2_TexturePacker.Image[] imgsToAdd, int idealAtlasW, int idealAtlasH, float imgArea, int maxAtlasDim, MB2_TexturePacker.ProbeResult pr)
		{
			MB2_TexturePacker.Node node = new MB2_TexturePacker.Node();
			node.r = new MB2_TexturePacker.PixRect(0, 0, idealAtlasW, idealAtlasH);
			for (int i = 0; i < imgsToAdd.Length; i++)
			{
				if (node.Insert(imgsToAdd[i], false) == null)
				{
					return false;
				}
				if (i == imgsToAdd.Length - 1)
				{
					int num = 0;
					int num2 = 0;
					this.GetExtent(node, ref num, ref num2);
					bool flag;
					float num8;
					float num9;
					if (this.doPowerOfTwoTextures)
					{
						int num3 = Mathf.Min(this.CeilToNearestPowerOfTwo(num), maxAtlasDim);
						int num4 = Mathf.Min(this.CeilToNearestPowerOfTwo(num2), maxAtlasDim);
						if (num4 < num3 / 2)
						{
							num4 = num3 / 2;
						}
						if (num3 < num4 / 2)
						{
							num3 = num4 / 2;
						}
						flag = (num <= maxAtlasDim && num2 <= maxAtlasDim);
						float num5 = Mathf.Max(1f, (float)num / (float)maxAtlasDim);
						float num6 = Mathf.Max(1f, (float)num2 / (float)maxAtlasDim);
						float num7 = (float)num3 * num5 * (float)num4 * num6;
						num8 = 1f - (num7 - imgArea) / num7;
						num9 = 1f;
					}
					else
					{
						num8 = 1f - ((float)(num * num2) - imgArea) / (float)(num * num2);
						if (num < num2)
						{
							num9 = (float)num / (float)num2;
						}
						else
						{
							num9 = (float)num2 / (float)num;
						}
						flag = (num <= maxAtlasDim && num2 <= maxAtlasDim);
					}
					pr.Set(num, num2, node, flag, num8, num9);
					if (this.LOG_LEVEL >= MB2_LogLevel.debug)
					{
						MB2_Log.LogDebug(string.Concat(new object[]
						{
							"Probe success efficiency w=",
							num,
							" h=",
							num2,
							" e=",
							num8,
							" sq=",
							num9,
							" fits=",
							flag
						}), new object[0]);
					}
					return true;
				}
			}
			Debug.LogError("Should never get here.");
			return false;
		}

		private void GetExtent(MB2_TexturePacker.Node r, ref int x, ref int y)
		{
			if (r.img != null)
			{
				if (r.r.x + r.img.w > x)
				{
					x = r.r.x + r.img.w;
				}
				if (r.r.y + r.img.h > y)
				{
					y = r.r.y + r.img.h;
				}
			}
			if (r.child[0] != null)
			{
				this.GetExtent(r.child[0], ref x, ref y);
			}
			if (r.child[1] != null)
			{
				this.GetExtent(r.child[1], ref x, ref y);
			}
		}

		private int StepWidthHeight(int oldVal, int step, int maxDim)
		{
			if (this.doPowerOfTwoTextures && oldVal < maxDim)
			{
				return oldVal * 2;
			}
			int num = oldVal + step;
			if (num > maxDim && oldVal < maxDim)
			{
				num = maxDim;
			}
			return num;
		}

		public int RoundToNearestPositivePowerOfTwo(int x)
		{
			int num = (int)Mathf.Pow(2f, (float)Mathf.RoundToInt(Mathf.Log((float)x) / Mathf.Log(2f)));
			if (num == 0 || num == 1)
			{
				num = 2;
			}
			return num;
		}

		public int CeilToNearestPowerOfTwo(int x)
		{
			int num = (int)Mathf.Pow(2f, Mathf.Ceil(Mathf.Log((float)x) / Mathf.Log(2f)));
			if (num == 0 || num == 1)
			{
				num = 2;
			}
			return num;
		}

		public Rect[] GetRects(List<Vector2> imgWidthHeights, int maxDimension, int padding, out int outW, out int outH)
		{
			return this._GetRects(imgWidthHeights, maxDimension, padding, 2 + padding * 2, 2 + padding * 2, 2 + padding * 2, 2 + padding * 2, out outW, out outH, 0);
		}

		private Rect[] _GetRects(List<Vector2> imgWidthHeights, int maxDimension, int padding, int minImageSizeX, int minImageSizeY, int masterImageSizeX, int masterImageSizeY, out int outW, out int outH, int recursionDepth)
		{
			if (this.LOG_LEVEL >= MB2_LogLevel.debug)
			{
				Debug.Log(string.Format("_GetRects numImages={0}, maxDimension={1}, padding={2}, minImageSizeX={3}, minImageSizeY={4}, masterImageSizeX={5}, masterImageSizeY={6}, recursionDepth={7}", new object[]
				{
					imgWidthHeights.Count,
					maxDimension,
					padding,
					minImageSizeX,
					minImageSizeY,
					masterImageSizeX,
					masterImageSizeY,
					recursionDepth
				}));
			}
			if (recursionDepth > 10)
			{
				Debug.LogError("Maximum recursion depth reached. Couldn't find packing for these textures.");
				outW = 0;
				outH = 0;
				return new Rect[0];
			}
			float num = 0f;
			int num2 = 0;
			int num3 = 0;
			MB2_TexturePacker.Image[] array = new MB2_TexturePacker.Image[imgWidthHeights.Count];
			for (int i = 0; i < array.Length; i++)
			{
				MB2_TexturePacker.Image image = array[i] = new MB2_TexturePacker.Image(i, (int)imgWidthHeights[i].x, (int)imgWidthHeights[i].y, padding, minImageSizeX, minImageSizeY);
				num += (float)(image.w * image.h);
				num2 = Mathf.Max(num2, image.w);
				num3 = Mathf.Max(num3, image.h);
			}
			if ((float)num3 / (float)num2 > 2f)
			{
				if (this.LOG_LEVEL >= MB2_LogLevel.debug)
				{
					MB2_Log.LogDebug("Using height Comparer", new object[0]);
				}
				Array.Sort<MB2_TexturePacker.Image>(array, new MB2_TexturePacker.ImageHeightComparer());
			}
			else if ((double)((float)num3 / (float)num2) < 0.5)
			{
				if (this.LOG_LEVEL >= MB2_LogLevel.debug)
				{
					MB2_Log.LogDebug("Using width Comparer", new object[0]);
				}
				Array.Sort<MB2_TexturePacker.Image>(array, new MB2_TexturePacker.ImageWidthComparer());
			}
			else
			{
				if (this.LOG_LEVEL >= MB2_LogLevel.debug)
				{
					MB2_Log.LogDebug("Using area Comparer", new object[0]);
				}
				Array.Sort<MB2_TexturePacker.Image>(array, new MB2_TexturePacker.ImageAreaComparer());
			}
			int num4 = (int)Mathf.Sqrt(num);
			int num6;
			int num5;
			if (this.doPowerOfTwoTextures)
			{
				num5 = (num6 = this.RoundToNearestPositivePowerOfTwo(num4));
				if (num2 > num6)
				{
					num6 = this.CeilToNearestPowerOfTwo(num6);
				}
				if (num3 > num5)
				{
					num5 = this.CeilToNearestPowerOfTwo(num5);
				}
			}
			else
			{
				num6 = num4;
				num5 = num4;
				if (num2 > num4)
				{
					num6 = num2;
					num5 = Mathf.Max(Mathf.CeilToInt(num / (float)num2), num3);
				}
				if (num3 > num4)
				{
					num6 = Mathf.Max(Mathf.CeilToInt(num / (float)num3), num2);
					num5 = num3;
				}
			}
			if (num6 == 0)
			{
				num6 = 1;
			}
			if (num5 == 0)
			{
				num5 = 1;
			}
			int num7 = (int)((float)num6 * 0.15f);
			int num8 = (int)((float)num5 * 0.15f);
			if (num7 == 0)
			{
				num7 = 1;
			}
			if (num8 == 0)
			{
				num8 = 1;
			}
			int num9 = 2;
			int num10 = num5;
			while (num9 >= 1 && num10 < num4 * 1000)
			{
				bool flag = false;
				num9 = 0;
				int num11 = num6;
				while (!flag && num11 < num4 * 1000)
				{
					MB2_TexturePacker.ProbeResult probeResult = new MB2_TexturePacker.ProbeResult();
					if (this.LOG_LEVEL >= MB2_LogLevel.trace)
					{
						Debug.Log(string.Concat(new object[]
						{
							"Probing h=",
							num10,
							" w=",
							num11
						}));
					}
					if (this.Probe(array, num11, num10, num, maxDimension, probeResult))
					{
						flag = true;
						if (this.bestRoot == null)
						{
							this.bestRoot = probeResult;
						}
						else if (probeResult.GetScore(this.doPowerOfTwoTextures) > this.bestRoot.GetScore(this.doPowerOfTwoTextures))
						{
							this.bestRoot = probeResult;
						}
					}
					else
					{
						num9++;
						num11 = this.StepWidthHeight(num11, num7, maxDimension);
						if (this.LOG_LEVEL >= MB2_LogLevel.debug)
						{
							MB2_Log.LogDebug(string.Concat(new object[]
							{
								"increasing Width h=",
								num10,
								" w=",
								num11
							}), new object[0]);
						}
					}
				}
				num10 = this.StepWidthHeight(num10, num8, maxDimension);
				if (this.LOG_LEVEL >= MB2_LogLevel.debug)
				{
					MB2_Log.LogDebug(string.Concat(new object[]
					{
						"increasing Height h=",
						num10,
						" w=",
						num11
					}), new object[0]);
				}
			}
			outW = 0;
			outH = 0;
			if (this.doPowerOfTwoTextures)
			{
				outW = Mathf.Min(this.CeilToNearestPowerOfTwo(this.bestRoot.w), maxDimension);
				outH = Mathf.Min(this.CeilToNearestPowerOfTwo(this.bestRoot.h), maxDimension);
				if (outH < outW / 2)
				{
					outH = outW / 2;
				}
				if (outW < outH / 2)
				{
					outW = outH / 2;
				}
			}
			else
			{
				outW = this.bestRoot.w;
				outH = this.bestRoot.h;
			}
			if (this.bestRoot == null)
			{
				return null;
			}
			if (this.LOG_LEVEL >= MB2_LogLevel.debug)
			{
				MB2_Log.LogDebug(string.Concat(new object[]
				{
					"Best fit found: atlasW=",
					outW,
					" atlasH",
					outH,
					" w=",
					this.bestRoot.w,
					" h=",
					this.bestRoot.h,
					" efficiency=",
					this.bestRoot.efficiency,
					" squareness=",
					this.bestRoot.squareness,
					" fits in max dimension=",
					this.bestRoot.fitsInMaxSize
				}), new object[0]);
			}
			List<MB2_TexturePacker.Image> list = new List<MB2_TexturePacker.Image>();
			MB2_TexturePacker.flattenTree(this.bestRoot.root, list);
			list.Sort(new MB2_TexturePacker.ImgIDComparer());
			if (list.Count != array.Length)
			{
				Debug.LogError("Result images not the same lentgh as source");
			}
			int minImageSizeX2 = minImageSizeX;
			int minImageSizeY2 = minImageSizeY;
			bool flag2 = false;
			float num12 = (float)padding / (float)outW;
			if (this.bestRoot.w > maxDimension)
			{
				num12 = (float)padding / (float)maxDimension;
				float num13 = (float)maxDimension / (float)this.bestRoot.w;
				if (this.LOG_LEVEL >= MB2_LogLevel.warn)
				{
					Debug.LogWarning("Packing exceeded atlas width shrinking to " + num13);
				}
				for (int j = 0; j < list.Count; j++)
				{
					MB2_TexturePacker.Image image2 = list[j];
					if ((float)image2.w * num13 < (float)masterImageSizeX)
					{
						if (this.LOG_LEVEL >= MB2_LogLevel.debug)
						{
							Debug.Log("Small images are being scaled to zero. Will need to redo packing with larger minTexSizeX.");
						}
						flag2 = true;
						minImageSizeX2 = Mathf.CeilToInt((float)minImageSizeX / num13);
					}
					int num14 = (int)((float)(image2.x + image2.w) * num13);
					image2.x = (int)(num13 * (float)image2.x);
					image2.w = num14 - image2.x;
				}
				outW = maxDimension;
			}
			float num15 = (float)padding / (float)outH;
			if (this.bestRoot.h > maxDimension)
			{
				num15 = (float)padding / (float)maxDimension;
				float num16 = (float)maxDimension / (float)this.bestRoot.h;
				if (this.LOG_LEVEL >= MB2_LogLevel.warn)
				{
					Debug.LogWarning("Packing exceeded atlas height shrinking to " + num16);
				}
				for (int k = 0; k < list.Count; k++)
				{
					MB2_TexturePacker.Image image3 = list[k];
					if ((float)image3.h * num16 < (float)masterImageSizeY)
					{
						if (this.LOG_LEVEL >= MB2_LogLevel.debug)
						{
							Debug.Log("Small images are being scaled to zero. Will need to redo packing with larger minTexSizeY.");
						}
						flag2 = true;
						minImageSizeY2 = Mathf.CeilToInt((float)minImageSizeY / num16);
					}
					int num17 = (int)((float)(image3.y + image3.h) * num16);
					image3.y = (int)(num16 * (float)image3.y);
					image3.h = num17 - image3.y;
				}
				outH = maxDimension;
			}
			Rect[] array2;
			if (!flag2)
			{
				array2 = new Rect[list.Count];
				for (int l = 0; l < list.Count; l++)
				{
					MB2_TexturePacker.Image image4 = list[l];
					Rect rect = array2[l] = new Rect((float)image4.x / (float)outW + num12, (float)image4.y / (float)outH + num15, (float)image4.w / (float)outW - num12 * 2f, (float)image4.h / (float)outH - num15 * 2f);
					if (this.LOG_LEVEL >= MB2_LogLevel.debug)
					{
						MB2_Log.LogDebug(string.Concat(new object[]
						{
							"Image: ",
							l,
							" imgID=",
							image4.imgId,
							" x=",
							rect.x * (float)outW,
							" y=",
							rect.y * (float)outH,
							" w=",
							rect.width * (float)outW,
							" h=",
							rect.height * (float)outH,
							" padding=",
							padding
						}), new object[0]);
					}
				}
			}
			else
			{
				if (this.LOG_LEVEL >= MB2_LogLevel.debug)
				{
					Debug.Log("==================== REDOING PACKING ================");
				}
				this.bestRoot = null;
				array2 = this._GetRects(imgWidthHeights, maxDimension, padding, minImageSizeX2, minImageSizeY2, masterImageSizeX, masterImageSizeY, out outW, out outH, recursionDepth + 1);
			}
			if (this.LOG_LEVEL >= MB2_LogLevel.debug)
			{
				MB2_Log.LogDebug("Done GetRects", new object[0]);
			}
			return array2;
		}

		public void RunTestHarness()
		{
			int num = 32;
			int min = 126;
			int max = 2046;
			List<Vector2> list = new List<Vector2>();
			for (int i = 0; i < num; i++)
			{
				list.Add(new Vector2((float)UnityEngine.Random.Range(min, max), (float)(UnityEngine.Random.Range(min, max) * 5)));
			}
			this.doPowerOfTwoTextures = true;
			this.LOG_LEVEL = MB2_LogLevel.trace;
			int padding = 1;
			int num2;
			int num3;
			this.GetRects(list, 4096, padding, out num2, out num3);
		}
	}
}
