using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class preimage_edit_class
{
	public List<image_edit_class> edit_color;

	public int y1;

	public int x1;

	public int x;

	public int y;

	public float frames;

	public float auto_speed_time;

	public float target_frame;

	public float time_start;

	public float time;

	public bool generate;

	public bool loop;

	public bool generate_call;

	public bool active;

	public bool loop_active;

	public bool import_settings;

	public bool regen;

	public bool regenRaw;

	public bool border;

	public float progress;

	public Vector2 resolution;

	public Vector2 resolutionRaw;

	public byte[] byte1;

	public bool raw;

	public int xx;

	public Vector2 position;

	public Vector2 position2;

	public Vector2 direction;

	public int dir;

	public Vector2 pos_old;

	public bool first;

	public int count;

	public buffer_class inputBuffer;

	public buffer_class outputBuffer;

	public int radius;

	public int radiusSelect;

	public int mode;

	public tile_class tile;

	public int repeat;

	public int repeatAmount;

	public bool content;

	public preimage_edit_class()
	{
		this.edit_color = new List<image_edit_class>();
		this.target_frame = (float)30;
		this.active = true;
		this.loop_active = true;
		this.byte1 = new byte[3];
		this.xx = 3;
		this.position = new Vector2((float)(this.x - 1), (float)(this.y - 1));
		this.direction = new Vector2((float)1, (float)0);
		this.dir = 1;
		this.inputBuffer = new buffer_class();
		this.outputBuffer = new buffer_class();
		this.radius = 300;
		this.radiusSelect = 300;
		this.mode = 1;
		this.tile = new tile_class();
		this.repeatAmount = 3;
		this.content = true;
	}

	public override float calc_color_pos(Color color, Color color_start, Color color_end)
	{
		Color color2 = color_start;
		Color color3 = default(Color);
		if (color_start.r > color_end.r)
		{
			color_start.r = color_end.r;
			color_end.r = color2.r;
		}
		if (color_start.g > color_end.g)
		{
			color_start.g = color_end.g;
			color_end.g = color2.g;
		}
		if (color_start.b > color_end.b)
		{
			color_start.b = color_end.b;
			color_end.b = color2.b;
		}
		color3 = color_end - color_start;
		color -= color_start;
		float arg_1C4_0;
		if (color.r < (float)0 || color.g < (float)0 || color.b < (float)0)
		{
			arg_1C4_0 = (float)-1;
		}
		else if (color.r > color3.r || color.g > color3.g || color.b > color3.b)
		{
			arg_1C4_0 = (float)-1;
		}
		else
		{
			float num = color3.r + color3.g + color3.b;
			float num2 = color.r + color.g + color.b;
			arg_1C4_0 = ((num == (float)0) ? ((float)1) : (num2 / num));
		}
		return arg_1C4_0;
	}

	public override Color calc_color_from_pos(float pos, Color color_start, Color color_end)
	{
		Color color = color_start;
		Color color2 = default(Color);
		if (color_start.r > color_end.r)
		{
			color_start.r = color_end.r;
			color_end.r = color.r;
		}
		if (color_start.g > color_end.g)
		{
			color_start.g = color_end.g;
			color_end.g = color.g;
		}
		if (color_start.b > color_end.b)
		{
			color_start.b = color_end.b;
			color_end.b = color.b;
		}
		color2 = color_end - color_start;
		return color_start + new Color(color2.r * pos, color2.g * pos, color2.b * pos);
	}

	public override void swap_color(int color_index1, int color_index2)
	{
		image_edit_class value = this.edit_color[color_index1];
		this.edit_color[color_index1] = this.edit_color[color_index2];
		this.edit_color[color_index2] = value;
	}

	public override void copy_color(int color_index1, int color_index2)
	{
		this.edit_color[color_index1].color1_start = this.edit_color[color_index2].color1_start;
		this.edit_color[color_index1].color1_end = this.edit_color[color_index2].color1_end;
		this.edit_color[color_index1].curve1 = this.edit_color[color_index2].curve1;
		this.edit_color[color_index1].color2_start = this.edit_color[color_index2].color2_start;
		this.edit_color[color_index1].color2_end = this.edit_color[color_index2].color2_end;
		this.edit_color[color_index1].curve2 = this.edit_color[color_index2].curve2;
		this.edit_color[color_index1].strength = this.edit_color[color_index2].strength;
		this.edit_color[color_index1].output = this.edit_color[color_index2].output;
		this.edit_color[color_index1].active = this.edit_color[color_index2].active;
		this.edit_color[color_index1].solid_color = this.edit_color[color_index2].solid_color;
	}

	public override void convert_texture_raw(bool multithread)
	{
		Color color = default(Color);
		Color color2 = default(Color);
		Color color3 = default(Color);
		float num = 0f;
		float pos = 0f;
		float num2 = 0f;
		float num3 = 0f;
		this.auto_speed_time = Time.realtimeSinceStartup;
		this.pos_old.y = (float)-100;
		this.y = this.y1;
		while ((float)this.y < this.inputBuffer.innerRect.height + this.inputBuffer.offset.y)
		{
			this.xx = 3;
			this.position = new Vector2((float)-1, (float)(this.y - 1));
			this.direction = new Vector2((float)1, (float)0);
			this.dir = 1;
			this.count = 0;
			this.x = (int)this.inputBuffer.offset.x;
			while ((float)this.x < this.inputBuffer.innerRect.width + this.inputBuffer.offset.x)
			{
				color = this.GetPixelRaw(this.inputBuffer, (long)this.x, (long)this.y);
				color3 = color;
				for (int i = 0; i < this.edit_color.Count; i++)
				{
					if (this.edit_color[i].active || this.edit_color[i].solid_color)
					{
						num = this.calc_color_pos(color, this.edit_color[i].color1_start, this.edit_color[i].color1_end);
						if (num != (float)-1)
						{
							num = this.edit_color[i].curve1.Evaluate(num);
							pos = this.edit_color[i].curve2.Evaluate(num);
							color2 = this.calc_color_from_pos(pos, this.edit_color[i].color2_start, this.edit_color[i].color2_end);
							num2 = this.edit_color[i].strength;
							if (!this.edit_color[i].solid_color)
							{
								if (this.edit_color[i].output == image_output_enum.content)
								{
									if ((float)this.x == this.pos_old.x + (float)1 && (float)this.y == this.pos_old.y && this.xx > 3)
									{
										if (this.dir == 1)
										{
											if (this.count == 0)
											{
												this.position.x = this.position.x + (float)1;
												this.xx -= 2;
											}
											else
											{
												this.count--;
											}
										}
										else if (this.dir == 2)
										{
											this.xx -= 2;
										}
										else if (this.dir == 3 || this.dir == 4)
										{
											this.position.x = (float)(this.x + (this.xx - 1) / 2);
											this.position.y = (float)(this.y - (this.xx - 1) / 2);
											this.dir = 2;
											this.count = 0;
											this.direction = new Vector2((float)-1, (float)0);
										}
										color2 = this.content_fill_raw(this.x, this.y, this.edit_color[i].color1_start, this.edit_color[i].color1_end, this.edit_color[i].color2_start, false);
									}
									else
									{
										color2 = this.content_fill_raw(this.x, this.y, this.edit_color[i].color1_start, this.edit_color[i].color1_end, this.edit_color[i].color2_start, true);
									}
									this.pos_old = new Vector2((float)this.x, (float)this.y);
									color3 = color2;
								}
								image_output_enum output = this.edit_color[i].output;
								if (output == image_output_enum.add)
								{
									color3.r += color2.r * num2;
									color3.g += color2.g * num2;
									color3.b += color2.b * num2;
								}
								else if (output == image_output_enum.subtract)
								{
									color3.r -= color2.r * num2;
									color3.g -= color2.g * num2;
									color3.b -= color2.b * num2;
								}
								else if (output == image_output_enum.change)
								{
									color3.r = color.r * ((float)1 - num2) + color2.r * num2;
									color3.g = color.g * ((float)1 - num2) + color2.g * num2;
									color3.b = color.b * ((float)1 - num2) + color2.b * num2;
								}
								else if (output == image_output_enum.multiply)
								{
									color3.r *= color2.r * num2;
									color3.g *= color2.g * num2;
									color3.b *= color2.b * num2;
								}
								else if (output == image_output_enum.divide)
								{
									if (color2.r * num2 != (float)0)
									{
										color3.r = color.r / (color2.r * num2);
									}
									if (color2.g * num2 != (float)0)
									{
										color3.g = color.g / (color2.g * num2);
									}
									if (color2.b * num2 != (float)0)
									{
										color3.b = color.b / (color2.b * num2);
									}
								}
								else if (output == image_output_enum.difference)
								{
									color3.r = Mathf.Abs(color2.r * num2 - color.r);
									color3.g = Mathf.Abs(color2.g * num2 - color.g);
									color3.b = Mathf.Abs(color2.b * num2 - color.b);
								}
								else if (output == image_output_enum.average)
								{
									color3.r = (color.r + color2.r * num2) / (float)2;
									color3.g = (color.g + color2.g * num2) / (float)2;
									color3.b = (color.b + color2.b * num2) / (float)2;
								}
								else if (output == image_output_enum.max)
								{
									if (color2.r * num2 > color.r)
									{
										color3.r = color2.r * num2;
									}
									if (color2.g * num2 > color.g)
									{
										color3.g = color2.g * num2;
									}
									if (color2.b * num2 > color.b)
									{
										color3.b = color2.b * num2;
									}
								}
								else if (output == image_output_enum.min)
								{
									if (color2.r * num2 < color.r)
									{
										color3.r = color2.r * num2;
									}
									if (color2.g * num2 < color.g)
									{
										color3.g = color2.g * num2;
									}
									if (color2.b * num2 < color.b)
									{
										color3.b = color2.b * num2;
									}
								}
							}
							else
							{
								color3.r += (float)1 - num;
								color3.g += num;
								color3.b += (float)1;
							}
						}
					}
				}
				if (color3[0] > (float)1)
				{
					color3[0] = (float)1;
				}
				else if (color3[0] < (float)0)
				{
					color3[0] = (float)0;
				}
				if (color3[1] > (float)1)
				{
					color3[1] = (float)1;
				}
				else if (color3[1] < (float)0)
				{
					color3[1] = (float)0;
				}
				if (color3[2] > (float)1)
				{
					color3[2] = (float)1;
				}
				else if (color3[2] < (float)0)
				{
					color3[2] = (float)0;
				}
				this.SetPixelRaw(this.outputBuffer, (long)this.x, (long)this.y, color3);
				this.x++;
			}
			if (Time.realtimeSinceStartup - this.auto_speed_time > 1f / this.target_frame && multithread)
			{
				this.y1 = this.y + 1;
				if (this.mode == 2)
				{
					this.time = Time.realtimeSinceStartup - this.time_start;
				}
				return;
			}
			this.y++;
		}
		this.generate = false;
	}

	public override Color content_fill_raw(int _x, int _y, Color exclude_start, Color exclude_end, Color exclude2, bool reset)
	{
		Vector2 vector = default(Vector2);
		Vector2 vector2 = default(Vector2);
		Color color = default(Color);
		Color color2 = default(Color);
		Color color3 = default(Color);
		float num = (float)0;
		float num2 = (float)360;
		float num3 = (float)20;
		Vector2 a = default(Vector2);
		Vector2 vector3 = default(Vector2);
		float num4 = 0f;
		float num5 = 0f;
		bool flag = false;
		float num6 = 0f;
		float num7 = 0f;
		bool flag2 = false;
		if (reset)
		{
			this.xx = 3;
			this.position = new Vector2((float)(_x - 1), (float)(_y - 1));
			this.direction = new Vector2((float)1, (float)0);
			this.dir = 1;
			this.count = 0;
		}
		do
		{
			color = this.GetPixelRaw(this.inputBuffer, (long)this.position.x, (long)this.position.y);
			if (!this.color_in_range(exclude_start, exclude_end, color))
			{
				break;
			}
			this.count++;
			if (this.count >= this.xx && this.dir == 1)
			{
				this.direction = new Vector2((float)0, (float)1);
				this.count = 0;
				this.dir = 2;
			}
			else if (this.count >= this.xx - 1 && this.dir == 2)
			{
				this.direction = new Vector2((float)-1, (float)0);
				this.count = 0;
				this.dir = 3;
			}
			else if (this.count >= this.xx - 1 && this.dir == 3)
			{
				this.direction = new Vector2((float)0, (float)-1);
				this.count = 0;
				this.dir = 4;
			}
			else if (this.count >= this.xx - 2 && this.dir == 4)
			{
				this.direction = new Vector2((float)1, (float)0);
				this.count = 0;
				this.position += new Vector2((float)-1, (float)-2);
				this.dir = 1;
				this.xx += 2;
				continue;
			}
			this.position += this.direction;
		}
		while (!flag);
		a.x = this.position.x - (float)_x;
		a.y = this.position.y - (float)_y;
		num4 = a.magnitude;
		if (this.repeat < 1 && num4 > (float)4)
		{
			int num8 = (int)(this.position.y - (float)1);
			while ((float)num8 <= this.position.y + (float)1)
			{
				int num9 = (int)(this.position.x - (float)1);
				while ((float)num9 <= this.position.x + (float)1)
				{
					color2 = this.GetPixelRaw(this.inputBuffer, (long)num9, (long)num8);
					if (color2[0] <= exclude2[0] && color2[1] <= exclude2[1] && color2[2] <= exclude2[2])
					{
						this.SetPixelRaw(this.outputBuffer, (long)num9, (long)num8, new Color((float)0, (float)0, (float)0));
					}
					num9++;
				}
				num8++;
			}
		}
		if (this.repeat < this.repeatAmount - 1)
		{
			vector3 = a / num4 * (float)this.radius;
			this.position2.x = (float)_x + vector3.x;
			this.position2.y = (float)_y + vector3.y;
			color = this.GetPixelRaw(this.inputBuffer, (long)this.position2.x, (long)this.position2.y);
			if (this.color_in_range(exclude_start, exclude_end, color))
			{
				this.regen = true;
				flag2 = true;
			}
		}
		if (!flag2)
		{
			color2 = this.GetPixelRaw(this.outputBuffer, (long)(_x - 1), (long)_y);
			if (!this.color_in_range(exclude_start, exclude_end, color2))
			{
				num5 = (float)this.color_difference(color2, color);
				if (this.GetPixelRaw(this.inputBuffer, (long)(_x - 1), (long)_y) == color2)
				{
					num5 *= (float)10;
				}
				if (num5 > num3)
				{
					color3 += color2 * (num5 / num2);
					num += num5 / num2;
				}
			}
			color2 = this.GetPixelRaw(this.outputBuffer, (long)_x, (long)(_y - 1));
			if (!this.color_in_range(exclude_start, exclude_end, color2))
			{
				num5 = (float)this.color_difference(color2, color);
				if (this.GetPixelRaw(this.inputBuffer, (long)_x, (long)(_y - 1)) == color2)
				{
					num5 *= (float)10;
				}
				if (num5 > num3)
				{
					color3 += color2 * (num5 / num2);
					num += num5 / num2;
				}
			}
			color2 = this.GetPixelRaw(this.outputBuffer, (long)(_x + 1), (long)_y);
			if (!this.color_in_range(exclude_start, exclude_end, color2))
			{
				num5 = (float)this.color_difference(color2, color);
				if (this.GetPixelRaw(this.inputBuffer, (long)(_x + 1), (long)_y) == color2)
				{
					num5 *= (float)10;
				}
				if (num5 > num3)
				{
					color3 += color2 * (num5 / num2);
					num += num5 / num2;
				}
			}
			color2 = this.GetPixelRaw(this.outputBuffer, (long)_x, (long)(_y + 1));
			if (!this.color_in_range(exclude_start, exclude_end, color2))
			{
				num5 = (float)this.color_difference(color2, color);
				if (this.GetPixelRaw(this.inputBuffer, (long)_x, (long)(_y + 1)) == color2)
				{
					num5 *= (float)10;
				}
				if (num5 > num3)
				{
					color3 += color2 * (num5 / num2);
					num += num5 / num2;
				}
			}
			color += color3;
			color /= (float)1 + num;
		}
		this.SetPixelRaw(this.outputBuffer, (long)_x, (long)_y, color);
		return color;
	}

	public override Color GetPixelRaw(buffer_class buffer, long x, long y)
	{
		Color arg_154_0;
		if (this.mode == 1)
		{
			if (x < 0L)
			{
				x = 0L - x;
			}
			else if ((float)x > buffer.outerRect.width - (float)1)
			{
				x = (long)((float)x - ((float)x - (buffer.outerRect.width - (float)1)));
			}
			if (y < 0L)
			{
				y = 0L - y;
			}
			else if ((float)y > buffer.outerRect.height - (float)1)
			{
				y = (long)((float)y - ((float)y - (buffer.outerRect.height - (float)1)));
			}
		}
		else if (x < 0L || (float)x > buffer.outerRect.width - (float)1 || y < 0L || (float)y > buffer.outerRect.height - (float)1)
		{
			arg_154_0 = this.GetPixelRaw2(buffer, x, y);
			return arg_154_0;
		}
		ulong num = (ulong)(buffer.outerRect.width * (float)3 * (float)y + (float)(x * 3L));
		arg_154_0 = new Color((float)buffer.bytes[(int)num] * 1f / (float)255, (float)buffer.bytes[(int)((long)num + (long)((ulong)1))] * 1f / (float)255, (float)buffer.bytes[(int)((long)num + (long)((ulong)2))] * 1f / (float)255);
		return arg_154_0;
	}

	public override void SetPixelRaw(buffer_class buffer, long x, long y, Color color)
	{
		if (x < 0L)
		{
			x = 0L - x;
		}
		else if ((float)x > buffer.outerRect.width - (float)1)
		{
			x = (long)((float)x - ((float)x - (buffer.outerRect.width - (float)1)));
		}
		if (y < 0L)
		{
			y = 0L - y;
		}
		else if ((float)y > buffer.outerRect.height - (float)1)
		{
			y = (long)((float)y - ((float)y - (buffer.outerRect.height - (float)1)));
		}
		ulong num = (ulong)(buffer.outerRect.width * (float)3 * (float)y + (float)(x * 3L));
		buffer.bytes[(int)num] = (byte)(color[0] * (float)255);
		buffer.bytes[(int)((long)num + (long)((ulong)1))] = (byte)(color[1] * (float)255);
		buffer.bytes[(int)((long)num + (long)((ulong)2))] = (byte)(color[2] * (float)255);
	}

	public override Color GetPixelRaw2(buffer_class buffer, long x, long y)
	{
		x = (long)((float)x + buffer.outerRect.x);
		y = (long)((float)y + buffer.outerRect.y);
		if (x < 0L)
		{
			x = -x;
		}
		else if ((float)x > buffer.resolution.x - (float)1)
		{
			x = (long)((float)x - ((float)x - buffer.resolution.x - (float)1));
		}
		if (y < 0L)
		{
			y = -y;
		}
		else if ((float)y > buffer.resolution.y - (float)1)
		{
			y = (long)((float)y - ((float)y - buffer.resolution.y - (float)1));
		}
		ulong num = (ulong)((long)buffer.row * y + x * 3L);
		buffer.file.Seek((long)num, SeekOrigin.Begin);
		byte[] array = new byte[3];
		buffer.file.Read(array, 0, 3);
		return new Color((float)array[0] * 1f / (float)255, (float)array[1] * 1f / (float)255, (float)array[2] * 1f / (float)255);
	}

	public override bool color_in_range(Color color_start, Color color_end, Color color)
	{
		return color[0] >= color_start[0] && color[0] <= color_end[0] && color[1] >= color_start[1] && color[1] <= color_end[1] && color[2] >= color_start[2] && color[2] <= color_end[2];
	}

	public override int color_difference(Color color1, Color color2)
	{
		return (int)((Mathf.Abs(color1[0] - color2[0]) + Mathf.Abs(color1[1] - color2[1]) + Mathf.Abs(color1[2] - color2[1])) * (float)255);
	}
}
