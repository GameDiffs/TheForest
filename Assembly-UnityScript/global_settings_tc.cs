using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class global_settings_tc : MonoBehaviour
{
	public int tc_script_id;

	public bool tc_installed;

	public Texture2D check_image;

	public string[] examples;

	public bool layer_count;

	public bool placed_count;

	public bool display_project;

	public bool tabs;

	public bool color_scheme;

	public color_settings_class color_layout;

	public bool color_layout_converted;

	public bool box_scheme;

	public bool display_color_curves;

	public bool display_mix_curves;

	public bool filter_select_text;

	public string install_path;

	public string install_path_full;

	public bool object_fast;

	public bool preview_texture;

	public int preview_texture_buffer;

	public bool preview_colors;

	public int preview_texture_resolution;

	public int preview_texture_resolution1;

	public int preview_quick_resolution_min;

	public float preview_splat_brightness;

	public bool preview_texture_dock;

	public int preview_target_frame;

	public List<Color> splat_colors;

	public int splat_custom_texture_resolution;

	public int splat_custom_texture_resolution1;

	public List<Color> tree_colors;

	public List<Color> grass_colors;

	public List<Color> object_colors;

	public bool toggle_text_no;

	public bool toggle_text_short;

	public bool toggle_text_long;

	public bool tooltip_text_no;

	public bool tooltip_text_short;

	public bool tooltip_text_long;

	public int tooltip_mode;

	public bool video_help;

	public bool run_in_background;

	public bool display_bar_auto_generate;

	public bool unload_textures;

	public bool clean_memory;

	public bool auto_speed;

	public int target_frame;

	public bool auto_save;

	public int auto_save_tc_instances;

	public int auto_save_scene_instances;

	public bool auto_save_tc;

	public List<string> auto_save_tc_list;

	public bool auto_save_scene;

	public List<string> auto_save_scene_list;

	public float auto_save_timer;

	public float auto_save_time_start;

	public bool auto_save_on_play;

	public string auto_save_path;

	public int terrain_tiles_max;

	public List<auto_search_class> auto_search_list;

	public map_class map;

	public Texture2D tex1;

	public Texture2D tex2;

	public Texture2D tex3;

	public Texture2D tex4;

	public select_window_class select_window;

	public List<int> preview_window;

	public float PI;

	public Texture2D map0;

	public Texture2D map1;

	public Texture2D map2;

	public Texture2D map3;

	public Texture2D map4;

	public Texture2D map5;

	public bool map_combine;

	public bool map_load;

	public bool map_load2;

	public bool map_load3;

	public bool map_load4;

	public int map_zoom1;

	public int map_zoom2;

	public int map_zoom3;

	public int map_zoom4;

	public latlong_class map_latlong;

	public latlong_class map_latlong_center;

	public int map_zoom;

	public int map_zoom_old;

	public global_settings_class settings;

	public double minLatitude;

	public double maxLatitude;

	public double minLongitude;

	public double maxLongitude;

	public global_settings_tc()
	{
		this.examples = new string[]
		{
			"Procedural Mountains",
			"Procedural Canyons",
			"Procedural Plateaus",
			"Procedural Islands",
			"Island Example"
		};
		this.layer_count = true;
		this.placed_count = true;
		this.display_project = true;
		this.tabs = true;
		this.color_scheme = true;
		this.color_layout = new color_settings_class();
		this.display_mix_curves = true;
		this.filter_select_text = true;
		this.object_fast = true;
		this.preview_texture = true;
		this.preview_texture_buffer = 100;
		this.preview_colors = true;
		this.preview_texture_resolution = 128;
		this.preview_texture_resolution1 = 128;
		this.preview_quick_resolution_min = 16;
		this.preview_splat_brightness = (float)1;
		this.preview_texture_dock = true;
		this.preview_target_frame = 30;
		this.splat_colors = new List<Color>();
		this.splat_custom_texture_resolution = 128;
		this.splat_custom_texture_resolution1 = 128;
		this.tree_colors = new List<Color>();
		this.grass_colors = new List<Color>();
		this.object_colors = new List<Color>();
		this.toggle_text_short = true;
		this.tooltip_text_long = true;
		this.tooltip_mode = 2;
		this.video_help = true;
		this.run_in_background = true;
		this.display_bar_auto_generate = true;
		this.auto_speed = true;
		this.target_frame = 40;
		this.auto_save = true;
		this.auto_save_tc_instances = 2;
		this.auto_save_scene_instances = 2;
		this.auto_save_tc = true;
		this.auto_save_tc_list = new List<string>();
		this.auto_save_scene = true;
		this.auto_save_scene_list = new List<string>();
		this.auto_save_timer = (float)10;
		this.auto_save_on_play = true;
		this.terrain_tiles_max = 15;
		this.auto_search_list = new List<auto_search_class>();
		this.map = new map_class();
		this.select_window = new select_window_class();
		this.preview_window = new List<int>();
		this.PI = 3.14159274f;
		this.map_latlong = new latlong_class();
		this.map_latlong_center = new latlong_class();
		this.map_zoom = 17;
		this.settings = new global_settings_class();
		this.minLatitude = (double)-85.05113f;
		this.maxLatitude = (double)85.05113f;
		this.minLongitude = (double)-180;
		this.maxLongitude = (double)180;
	}

	public override Vector2 drawText(string text, Vector2 pos, bool background, Color color, Color backgroundColor, float rotation, float fontSize, bool bold, int mode)
	{
		Matrix4x4 identity = Matrix4x4.identity;
		Matrix4x4 identity2 = Matrix4x4.identity;
		int fontSize2 = GUI.skin.label.fontSize;
		FontStyle fontStyle = GUI.skin.label.fontStyle;
		Color color2 = GUI.color;
		GUI.skin.label.fontSize = (int)fontSize;
		if (bold)
		{
			GUI.skin.label.fontStyle = FontStyle.Bold;
		}
		else
		{
			GUI.skin.label.fontStyle = FontStyle.Normal;
		}
		Vector2 result = GUI.skin.GetStyle("Label").CalcSize(new GUIContent(text));
		identity2.SetTRS(new Vector3(pos.x, pos.y, (float)0), Quaternion.Euler((float)0, (float)0, rotation), Vector3.one);
		if (mode == 1)
		{
			GUI.matrix = identity2;
		}
		else if (mode == 2)
		{
			identity.SetTRS(new Vector3(-result.x / (float)2, -result.y, (float)0), Quaternion.identity, Vector3.one);
			GUI.matrix = identity2 * identity;
		}
		else if (mode == 3)
		{
			identity.SetTRS(new Vector3((float)0, -result.y, (float)0), Quaternion.identity, Vector3.one);
			GUI.matrix = identity2 * identity;
		}
		else if (mode == 4)
		{
			identity.SetTRS(new Vector3(-result.x / (float)2, -result.y / (float)2, (float)0), Quaternion.identity, Vector3.one);
			GUI.matrix = identity2 * identity;
		}
		else if (mode == 5)
		{
			identity.SetTRS(new Vector3(-result.x, (float)0, (float)0), Quaternion.identity, Vector3.one);
			GUI.matrix = identity2 * identity;
		}
		else if (mode == 6)
		{
			identity.SetTRS(new Vector3(-result.x, -result.y, (float)0), Quaternion.identity, Vector3.one);
			GUI.matrix = identity2 * identity;
		}
		if (background)
		{
			GUI.color = backgroundColor;
		}
		GUI.color = color;
		GUI.Label(new Rect((float)0, (float)0, result.x, result.y), text);
		GUI.skin.label.fontSize = fontSize2;
		GUI.skin.label.fontStyle = fontStyle;
		GUI.color = color2;
		GUI.matrix = Matrix4x4.identity;
		return result;
	}

	public override bool drawText(Rect rect, edit_class edit, bool background, Color color, Color backgroundColor, float fontSize, bool bold, int mode)
	{
		Vector2 vector = default(Vector2);
		int fontSize2 = 0;
		FontStyle fontStyle = FontStyle.Normal;
		Color color2 = GUI.color;
		Vector2 size = default(Vector2);
		if (background)
		{
			GUI.color = backgroundColor;
		}
		GUI.color = color;
		if (!edit.edit)
		{
			fontSize2 = GUI.skin.label.fontSize;
			fontStyle = GUI.skin.label.fontStyle;
			GUI.skin.label.fontSize = (int)fontSize;
			if (bold)
			{
				GUI.skin.label.fontStyle = FontStyle.Bold;
			}
			else
			{
				GUI.skin.label.fontStyle = FontStyle.Normal;
			}
			size = GUI.skin.GetStyle("Label").CalcSize(new GUIContent(edit.default_text));
			vector = this.calc_rect_allign(rect, size, mode);
			GUI.Label(new Rect(vector.x, vector.y, size.x, size.y), edit.default_text);
			GUI.skin.label.fontSize = fontSize2;
			GUI.skin.label.fontStyle = fontStyle;
		}
		else
		{
			fontSize2 = GUI.skin.textField.fontSize;
			fontStyle = GUI.skin.textField.fontStyle;
			GUI.skin.textField.fontSize = (int)fontSize;
			if (bold)
			{
				GUI.skin.textField.fontStyle = FontStyle.Bold;
			}
			else
			{
				GUI.skin.textField.fontStyle = FontStyle.Normal;
			}
			size = GUI.skin.GetStyle("TextField").CalcSize(new GUIContent(edit.text));
			if (size.x < rect.width)
			{
				size.x = rect.width;
			}
			size.x += (float)10;
			vector = this.calc_rect_allign(rect, size, mode);
			edit.text = GUI.TextField(new Rect(vector.x, vector.y, size.x, size.y), edit.text);
			GUI.skin.textField.fontSize = fontSize2;
			GUI.skin.textField.fontStyle = fontStyle;
		}
		if (Event.current.button == 0 && Event.current.clickCount == 2 && Event.current.type == EventType.MouseDown && new Rect(vector.x, vector.y, size.x, size.y).Contains(Event.current.mousePosition))
		{
			edit.edit = !edit.edit;
		}
		bool arg_304_0;
		if (Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.Escape)
		{
			edit.edit = false;
			GUI.color = color2;
			arg_304_0 = true;
		}
		else
		{
			GUI.color = color2;
			arg_304_0 = false;
		}
		return arg_304_0;
	}

	public override void drawText(Rect rect, string text, bool background, Color color, Color backgroundColor, float fontSize, bool bold, int mode)
	{
		Vector2 vector = default(Vector2);
		int fontSize2 = GUI.skin.label.fontSize;
		FontStyle fontStyle = GUI.skin.label.fontStyle;
		Color color2 = GUI.color;
		Vector2 size = default(Vector2);
		if (background)
		{
			GUI.color = backgroundColor;
		}
		GUI.color = color;
		GUI.skin.label.fontSize = (int)fontSize;
		if (bold)
		{
			GUI.skin.label.fontStyle = FontStyle.Bold;
		}
		else
		{
			GUI.skin.label.fontStyle = FontStyle.Normal;
		}
		size = GUI.skin.GetStyle("Label").CalcSize(new GUIContent(text));
		vector = this.calc_rect_allign(rect, size, mode);
		GUI.Label(new Rect(vector.x, vector.y, size.x, size.y), text);
		GUI.skin.label.fontSize = fontSize2;
		GUI.skin.label.fontStyle = fontStyle;
		GUI.color = color2;
	}

	public override Vector2 calc_rect_allign(Rect rect, Vector2 size, int mode)
	{
		Vector2 result = default(Vector2);
		if (mode == 1)
		{
			result.x = rect.x;
			result.y = rect.y;
		}
		else if (mode == 2)
		{
			result.x = rect.x + rect.width / (float)2 - size.x / (float)2;
			result.y = rect.yMax;
		}
		else if (mode == 3)
		{
			result.x = rect.x;
			result.y = rect.yMax;
		}
		else if (mode == 4)
		{
			result.x = rect.x + rect.width / (float)2 - size.x / (float)2;
			result.y = rect.y + rect.height / (float)2 - size.y / (float)2;
		}
		else if (mode == 5)
		{
			result.x = rect.xMax - size.x;
			result.y = rect.y;
		}
		else if (mode == 6)
		{
			result.x = rect.x + rect.width / (float)2 - size.x / (float)2;
			result.y = rect.y - size.y;
		}
		return result;
	}

	public override bool drawGUIBox(Rect rect, edit_class edit, float fontSize, bool label2, float labelHeight, Color backgroundColor, Color highlightColor, Color highlightColor2, Color textColor, bool border, int width, Rect screen, bool select, Color select_color, bool active)
	{
		if (!select)
		{
			highlightColor += new Color(-0.3f, -0.3f, -0.3f);
			highlightColor2 += new Color(-0.3f, -0.3f, -0.3f);
		}
		GUI.color = highlightColor;
		bool result = this.drawText(rect, edit, false, textColor, new Color(0.1f, 0.1f, 0.1f, (float)1), fontSize, true, 6);
		if (label2)
		{
			GUI.color = highlightColor2;
			GUI.color = Color.white;
			if (!active)
			{
				Drawing_tc1.DrawLine(new Vector2(rect.x + (float)1, rect.y + labelHeight + (float)1), new Vector2(rect.xMax - (float)1, rect.yMax - labelHeight - (float)1), new Color((float)1, (float)0, (float)0, 0.7f), (float)3, false, screen);
				Drawing_tc1.DrawLine(new Vector2(rect.x + (float)1, rect.yMax - labelHeight - (float)1), new Vector2(rect.xMax - (float)1, rect.y + labelHeight + (float)1), new Color((float)1, (float)0, (float)0, 0.7f), (float)3, false, screen);
			}
		}
		else if (!active)
		{
			Drawing_tc1.DrawLine(new Vector2(rect.x + (float)1, rect.y + labelHeight + (float)1), new Vector2(rect.xMax - (float)1, rect.yMax - (float)1), new Color((float)1, (float)0, (float)0, 0.7f), (float)3, false, screen);
			Drawing_tc1.DrawLine(new Vector2(rect.x + (float)1, rect.yMax - (float)1), new Vector2(rect.xMax - (float)1, rect.y + labelHeight + (float)1), new Color((float)1, (float)0, (float)0, 0.7f), (float)3, false, screen);
		}
		if (border)
		{
			this.DrawRect(rect, highlightColor, (float)width, screen);
			Drawing_tc1.DrawLine(new Vector2(rect.x, rect.y + labelHeight), new Vector2(rect.xMax, rect.y + labelHeight), highlightColor, (float)width, false, screen);
			if (label2)
			{
				Drawing_tc1.DrawLine(new Vector2(rect.x, rect.yMax - labelHeight), new Vector2(rect.xMax, rect.yMax - labelHeight), highlightColor, (float)width, false, screen);
			}
		}
		GUI.color = Color.white;
		return result;
	}

	public override void drawJoinNode(Rect rect, int length, string text, float fontSize, bool label2, float labelHeight, Color backgroundColor, Color highlightColor, Color highlightColor2, Color textColor, bool border, int width, Rect screen, bool select, Color select_color, bool active)
	{
		if (!select)
		{
			highlightColor += new Color(-0.3f, -0.3f, -0.3f);
			highlightColor2 += new Color(-0.3f, -0.3f, -0.3f);
		}
		GUI.color = highlightColor;
		for (int i = 0; i < length; i++)
		{
		}
		for (int i = 0; i < length; i++)
		{
			if (i < length - 1)
			{
				Drawing_tc1.DrawLine(new Vector2(rect.x, rect.y + (float)(i + 1) * this.select_window.node_zoom), new Vector2(rect.xMax, rect.y + (float)(i + 1) * this.select_window.node_zoom), highlightColor, (float)width, false, screen);
			}
		}
		this.drawText(rect, text, false, textColor, new Color(0.1f, 0.1f, 0.1f, (float)1), fontSize, true, 6);
		if (border)
		{
			this.DrawRect(new Rect(rect.x, rect.y, rect.width, (float)length * this.select_window.node_zoom), highlightColor, (float)width, screen);
		}
		GUI.color = Color.white;
	}

	public override int get_label_width(string text, bool bold)
	{
		Vector2 vector = default(Vector2);
		if (bold)
		{
			FontStyle fontStyle = GUI.skin.label.fontStyle;
			GUI.skin.label.fontStyle = FontStyle.Bold;
			vector = GUI.skin.GetStyle("Label").CalcSize(new GUIContent(text));
			GUI.skin.label.fontStyle = fontStyle;
		}
		else
		{
			vector = GUI.skin.GetStyle("Label").CalcSize(new GUIContent(text));
		}
		return (int)vector.x;
	}

	public override void DrawRect(Rect rect, Color color, float width, Rect screen)
	{
		Drawing_tc1.DrawLine(new Vector2(rect.xMin, rect.yMin), new Vector2(rect.xMax, rect.yMin), color, width, false, screen);
		Drawing_tc1.DrawLine(new Vector2(rect.xMin, rect.yMin), new Vector2(rect.xMin, rect.yMax), color, width, false, screen);
		Drawing_tc1.DrawLine(new Vector2(rect.xMin, rect.yMax), new Vector2(rect.xMax, rect.yMax), color, width, false, screen);
		Drawing_tc1.DrawLine(new Vector2(rect.xMax, rect.yMin), new Vector2(rect.xMax, rect.yMax), color, width, false, screen);
	}

	public override void draw_arrow(Vector2 point1, int length, int length_arrow, float rotation, Color color, int width, Rect screen)
	{
		length_arrow = (int)(Mathf.Sqrt(2f) * (float)length_arrow);
		Vector2 vector = this.calc_rotation_pixel(point1.x, point1.y - (float)length, point1.x, point1.y, rotation);
		Vector2 pointB = this.calc_rotation_pixel(vector.x - (float)length_arrow, vector.y - (float)length_arrow, vector.x, vector.y, (float)-180 + rotation);
		Vector2 pointB2 = this.calc_rotation_pixel(vector.x + (float)length_arrow, vector.y - (float)length_arrow, vector.x, vector.y, (float)180 + rotation);
		Drawing_tc1.DrawLine(point1, vector, color, (float)width, false, screen);
		Drawing_tc1.DrawLine(vector, pointB, color, (float)width, false, screen);
		Drawing_tc1.DrawLine(vector, pointB2, color, (float)width, false, screen);
	}

	public override bool draw_latlong_raster(latlong_class latlong1, latlong_class latlong2, Vector2 offset, double zoom, double current_zoom, int resolution, Rect screen, Color color, int width)
	{
		bool result = true;
		Vector2 vector = this.latlong_to_pixel(latlong1, this.map_latlong_center, current_zoom, new Vector2(screen.width, screen.height));
		Vector2 a = this.latlong_to_pixel(latlong2, this.map_latlong_center, current_zoom, new Vector2(screen.width, screen.height));
		Vector2 vector2 = a - vector;
		vector += new Vector2(-offset.x, offset.y);
		a += new Vector2(-offset.x, offset.y);
		double num = (double)Mathf.Pow((float)2, (float)(zoom - current_zoom));
		float num2 = (float)((double)resolution / num);
		if (Mathf.Abs(Mathf.Round(vector2.x / num2) - vector2.x / num2) > 0.01f || Mathf.Abs(Mathf.Round(vector2.y / num2) - vector2.y / num2) > 0.01f)
		{
			result = false;
			color = Color.red;
		}
		for (float num3 = vector.x; num3 < vector.x + vector2.x; num3 += num2)
		{
			Drawing_tc1.DrawLine(new Vector2(num3, vector.y), new Vector2(num3, a.y), color, (float)width, false, screen);
		}
		for (float num4 = vector.y; num4 < vector.y + vector2.y; num4 += num2)
		{
			Drawing_tc1.DrawLine(new Vector2(vector.x, num4), new Vector2(a.x, num4), color, (float)width, false, screen);
		}
		return result;
	}

	public override void draw_grid(Rect rect, int tile_x, int tile_y, Color color, int width, Rect screen)
	{
		Vector2 vector = default(Vector2);
		vector.x = rect.width / (float)tile_x;
		vector.y = rect.height / (float)tile_y;
		for (float num = rect.x; num <= rect.xMax + vector.x / (float)2; num += vector.x)
		{
			Drawing_tc1.DrawLine(new Vector2(num, rect.y), new Vector2(num, rect.yMax), color, (float)width, false, screen);
		}
		for (float num2 = rect.y; num2 <= rect.yMax + vector.y / (float)2; num2 += vector.y)
		{
			Drawing_tc1.DrawLine(new Vector2(rect.x, num2), new Vector2(rect.xMax, num2), color, (float)width, false, screen);
		}
	}

	public override void draw_scale_grid(Rect rect, Vector2 offset, float zoom, float scale, Color color, int width, bool draw_center, Rect screen)
	{
		float num = 0f;
		Vector2 vector = new Vector2(screen.width, screen.height) / (float)2 + offset;
		Vector2 vector2 = default(Vector2);
		Vector2 vector3 = default(Vector2);
		float num2 = vector.x - rect.x;
		float num3 = vector.y - rect.y;
		int num4 = (int)(num2 / zoom);
		num4 = (int)(num2 - (float)num4 * zoom);
		num4 = (int)((float)num4 + rect.x);
		int num5 = this.calc_rest_value((vector.x - (float)num4) / zoom, (float)10);
		if (num5 < 0)
		{
			num5 = -9 - num5;
		}
		else
		{
			num5 = 9 - num5;
		}
		int num6 = (int)(-((vector.x - (float)num4) / zoom) + (float)(9 - num5));
		int num7 = (int)(num3 / zoom);
		num7 = (int)(num3 - (float)num7 * zoom);
		num7 = (int)((float)num7 + rect.y);
		for (float num8 = (float)num4; num8 <= rect.xMax; num8 += zoom)
		{
			Drawing_tc1.DrawLine(new Vector2(num8, rect.y), new Vector2(num8, rect.yMax), color, (float)width, false, screen);
			if (num5 > 9)
			{
				num5 = 0;
			}
			num5++;
		}
		for (float num9 = (float)num7; num9 <= rect.yMax; num9 += zoom)
		{
			Drawing_tc1.DrawLine(new Vector2(rect.x, num9), new Vector2(rect.xMax, num9), color, (float)width, false, screen);
		}
		if (draw_center)
		{
			Drawing_tc1.DrawLine(new Vector2(vector.x, rect.y), new Vector2(vector.x, rect.yMax), color, (float)(width + 2), false, screen);
			Drawing_tc1.DrawLine(new Vector2(rect.x, vector.y), new Vector2(rect.xMax, vector.y), color, (float)(width + 2), false, screen);
		}
	}

	public override Vector2 calc_rotation_pixel(float x, float y, float xx, float yy, float rotation)
	{
		Vector2 result = new Vector2(x - xx, y - yy);
		float magnitude = result.magnitude;
		if (magnitude != (float)0)
		{
			result.x /= magnitude;
			result.y /= magnitude;
		}
		float num = Mathf.Acos(result.x);
		if (result.y < (float)0)
		{
			num = this.PI * (float)2 - num;
		}
		num -= rotation * 0.0174532924f;
		result.x = Mathf.Cos(num) * magnitude + xx;
		result.y = Mathf.Sin(num) * magnitude + yy;
		return result;
	}

	public override double clip(double n, double minValue, double maxValue)
	{
		return this.calcMin(this.calcMax(n, minValue), maxValue);
	}

	public override latlong_class clip_latlong(latlong_class latlong)
	{
		if (latlong.latitude > this.maxLatitude)
		{
			latlong.latitude -= this.maxLatitude * (double)2;
		}
		else if (latlong.latitude < this.minLatitude)
		{
			latlong.latitude += this.maxLatitude * (double)2;
		}
		if (latlong.longitude > (double)180)
		{
			latlong.longitude -= (double)360;
		}
		else if (latlong.longitude < (double)-180)
		{
			latlong.longitude += (double)360;
		}
		return latlong;
	}

	public override map_pixel_class clip_pixel(map_pixel_class map_pixel, double zoom)
	{
		double num = (double)((float)256 * Mathf.Pow((float)2, (float)zoom));
		if (map_pixel.x > num - (double)1)
		{
			map_pixel.x -= num - (double)1;
		}
		else if (map_pixel.x < (double)0)
		{
			map_pixel.x = num - (double)1 - map_pixel.x;
		}
		if (map_pixel.y > num - (double)1)
		{
			map_pixel.y -= num - (double)1;
		}
		else if (map_pixel.y < (double)0)
		{
			map_pixel.y = num - (double)1 - map_pixel.y;
		}
		return map_pixel;
	}

	public override double calcMin(double a, double b)
	{
		return (a >= b) ? b : a;
	}

	public override double calcMax(double a, double b)
	{
		return (a <= b) ? b : a;
	}

	public override int mapSize(int zoom)
	{
		return (int)(Mathf.Pow((float)2, (float)zoom) * (float)256);
	}

	public override Vector2 latlong_to_pixel(latlong_class latlong, latlong_class latlong_center, double zoom, Vector2 screen_resolution)
	{
		latlong = this.clip_latlong(latlong);
		latlong_center = this.clip_latlong(latlong_center);
		double num = (double)3.14159274f;
		double num2 = (latlong.longitude + (double)180) / (double)360;
		double num3 = (double)Mathf.Sin((float)(latlong.latitude * num / (double)180));
		double num4 = (double)0.5f - (double)Mathf.Log((float)(((double)1 + num3) / ((double)1 - num3))) / ((double)4 * num);
		Vector2 a = new Vector2((float)num2, (float)num4);
		num2 = (latlong_center.longitude + (double)180) / (double)360;
		num3 = (double)Mathf.Sin((float)(latlong_center.latitude * num / (double)180));
		num4 = (double)0.5f - (double)Mathf.Log((float)(((double)1 + num3) / ((double)1 - num3))) / ((double)4 * num);
		Vector2 b = new Vector2((float)num2, (float)num4);
		Vector2 a2 = a - b;
		a2 *= (float)256 * Mathf.Pow((float)2, (float)zoom);
		return a2 + screen_resolution / (float)2;
	}

	public override map_pixel_class latlong_to_pixel2(latlong_class latlong, double zoom)
	{
		latlong = this.clip_latlong(latlong);
		double num = (double)3.14159274f;
		double num2 = (latlong.longitude + (double)180f) / (double)360f;
		double num3 = (double)Mathf.Sin((float)(latlong.latitude * num / (double)180f));
		double num4 = (double)0.5f - (double)Mathf.Log((float)(((double)1f + num3) / ((double)1f - num3))) / ((double)4f * num);
		num2 *= (double)(256f * Mathf.Pow(2f, (float)zoom));
		num4 *= (double)(256f * Mathf.Pow(2f, (float)zoom));
		return new map_pixel_class
		{
			x = num2,
			y = num4
		};
	}

	public override latlong_class pixel_to_latlong2(map_pixel_class map_pixel, double zoom)
	{
		map_pixel = this.clip_pixel(map_pixel, zoom);
		double num = (double)3.14159274f;
		double num2 = (double)(256f * Mathf.Pow(2f, (float)zoom));
		double num3 = map_pixel.x / num2 - (double)0.5f;
		double num4 = (double)0.5f - map_pixel.y / num2;
		return new latlong_class
		{
			latitude = (double)90f - (double)(360f * Mathf.Atan(Mathf.Exp((float)(-(float)num4 * (double)2f * num)))) / num,
			longitude = (double)360f * num3
		};
	}

	public override latlong_class pixel_to_latlong(Vector2 offset, latlong_class latlong_center, double zoom)
	{
		double num = (double)3.14159274f;
		double num2 = (double)((float)256 * Mathf.Pow((float)2, (float)zoom));
		map_pixel_class map_pixel_class = this.latlong_to_pixel2(latlong_center, zoom);
		map_pixel_class map_pixel_class2 = new map_pixel_class();
		map_pixel_class2.x = map_pixel_class.x + (double)offset.x;
		map_pixel_class2.y = map_pixel_class.y + (double)offset.y;
		double num3 = map_pixel_class2.x / num2 - (double)0.5f;
		double num4 = (double)0.5f - map_pixel_class2.y / num2;
		return this.clip_latlong(new latlong_class
		{
			latitude = (double)90 - (double)((float)360 * Mathf.Atan(Mathf.Exp((float)(-(float)num4 * (double)2 * num)))) / num,
			longitude = (double)360 * num3
		});
	}

	public override map_pixel_class calc_latlong_area_size(latlong_class latlong1, latlong_class latlong2, latlong_class latlong_center)
	{
		double num = (double)3.14159274f;
		map_pixel_class map_pixel_class = this.latlong_to_pixel2(latlong1, (double)19);
		map_pixel_class map_pixel_class2 = this.latlong_to_pixel2(latlong2, (double)19);
		double num2 = (double)(156543.047f * Mathf.Cos((float)(latlong_center.latitude * (num / (double)180))) / Mathf.Pow((float)2, (float)19));
		return new map_pixel_class
		{
			x = (map_pixel_class2.x - map_pixel_class.x) * num2,
			y = (map_pixel_class2.y - map_pixel_class.y) * num2
		};
	}

	public override double calc_latlong_area_resolution(latlong_class latlong, double zoom)
	{
		double num = (double)3.14159274f;
		map_pixel_class map_pixel_class = this.latlong_to_pixel2(latlong, zoom);
		return (double)(156543.047f * Mathf.Cos((float)(latlong.latitude * (num / (double)180))) / Mathf.Pow((float)2, (float)zoom));
	}

	public override latlong_area_class calc_latlong_area_rounded(latlong_class latlong1, latlong_class latlong2, double zoom, int resolution, bool square, int mode)
	{
		map_pixel_class map_pixel_class = this.latlong_to_pixel2(latlong1, zoom);
		map_pixel_class map_pixel_class2 = this.latlong_to_pixel2(latlong2, zoom);
		map_pixel_class map_pixel_class3 = new map_pixel_class();
		map_pixel_class3.x = (double)(Mathf.Round((float)((map_pixel_class2.x - map_pixel_class.x) / (double)resolution)) * (float)resolution);
		if (square)
		{
			map_pixel_class3.y = map_pixel_class3.x;
		}
		else
		{
			map_pixel_class3.y = (double)(Mathf.Round((float)((map_pixel_class2.y - map_pixel_class.y) / (double)resolution)) * (float)resolution);
		}
		if (mode == 1)
		{
			if (map_pixel_class.x > map_pixel_class2.x - (double)resolution)
			{
				map_pixel_class.x = map_pixel_class2.x - (double)resolution;
			}
			else
			{
				map_pixel_class.x = map_pixel_class2.x - map_pixel_class3.x;
			}
		}
		else if (mode == 2)
		{
			if (map_pixel_class2.x < map_pixel_class.x + (double)resolution)
			{
				map_pixel_class2.x = map_pixel_class.x + (double)resolution;
			}
			else
			{
				map_pixel_class2.x = map_pixel_class.x + map_pixel_class3.x;
			}
		}
		else if (mode == 3)
		{
			if (map_pixel_class.y > map_pixel_class2.y - (double)resolution)
			{
				map_pixel_class.y = map_pixel_class2.y - (double)resolution;
			}
			else
			{
				map_pixel_class.y = map_pixel_class2.y - map_pixel_class3.y;
			}
		}
		else if (mode == 4)
		{
			if (map_pixel_class2.y < map_pixel_class.y + (double)resolution)
			{
				map_pixel_class2.y = map_pixel_class.y + (double)resolution;
			}
			else
			{
				map_pixel_class2.y = map_pixel_class.y + map_pixel_class3.y;
			}
		}
		else if (mode == 5)
		{
			if (map_pixel_class.x > map_pixel_class2.x - (double)resolution)
			{
				map_pixel_class.x = map_pixel_class2.x - (double)resolution;
			}
			else
			{
				map_pixel_class.x = map_pixel_class2.x - map_pixel_class3.x;
			}
			if (map_pixel_class.y > map_pixel_class2.y - (double)resolution)
			{
				map_pixel_class.y = map_pixel_class2.y - (double)resolution;
			}
			else
			{
				map_pixel_class.y = map_pixel_class2.y - map_pixel_class3.y;
			}
		}
		else if (mode == 6)
		{
			if (map_pixel_class2.x < map_pixel_class.x + (double)resolution)
			{
				map_pixel_class2.x = map_pixel_class.x + (double)resolution;
			}
			else
			{
				map_pixel_class2.x = map_pixel_class.x + map_pixel_class3.x;
			}
			if (map_pixel_class.y > map_pixel_class2.y - (double)resolution)
			{
				map_pixel_class.y = map_pixel_class2.y - (double)resolution;
			}
			else
			{
				map_pixel_class.y = map_pixel_class2.y - map_pixel_class3.y;
			}
		}
		else if (mode == 7)
		{
			if (map_pixel_class.x > map_pixel_class2.x - (double)resolution)
			{
				map_pixel_class.x = map_pixel_class2.x - (double)resolution;
			}
			else
			{
				map_pixel_class.x = map_pixel_class2.x - map_pixel_class3.x;
			}
			if (map_pixel_class2.y < map_pixel_class.y + (double)resolution)
			{
				map_pixel_class2.y = map_pixel_class.y + (double)resolution;
			}
			else
			{
				map_pixel_class2.y = map_pixel_class.y + map_pixel_class3.y;
			}
		}
		else if (mode == 8)
		{
			if (map_pixel_class2.x - (double)resolution < map_pixel_class.x)
			{
				map_pixel_class2.x = map_pixel_class.x + (double)resolution;
			}
			else
			{
				map_pixel_class2.x = map_pixel_class.x + map_pixel_class3.x;
			}
			if (map_pixel_class2.y - (double)resolution < map_pixel_class.y)
			{
				map_pixel_class2.y = map_pixel_class.y + (double)resolution;
			}
			else
			{
				map_pixel_class2.y = map_pixel_class.y + map_pixel_class3.y;
			}
		}
		return new latlong_area_class
		{
			latlong1 = this.pixel_to_latlong2(map_pixel_class, zoom),
			latlong2 = this.pixel_to_latlong2(map_pixel_class2, zoom)
		};
	}

	public override tile_class calc_latlong_area_tiles(latlong_class latlong1, latlong_class latlong2, double zoom, int resolution)
	{
		tile_class tile_class = new tile_class();
		map_pixel_class map_pixel_class = this.latlong_to_pixel2(latlong1, zoom);
		map_pixel_class map_pixel_class2 = this.latlong_to_pixel2(latlong2, zoom);
		map_pixel_class map_pixel_class3 = new map_pixel_class();
		tile_class.x = (int)Mathf.Round((float)((map_pixel_class2.x - map_pixel_class.x) / (double)resolution));
		tile_class.y = (int)Mathf.Round((float)((map_pixel_class2.y - map_pixel_class.y) / (double)resolution));
		return tile_class;
	}

	public override latlong_class calc_latlong_center(latlong_class latlong1, latlong_class latlong2, double zoom, Vector2 screen_resolution)
	{
		map_pixel_class map_pixel_class = this.latlong_to_pixel2(latlong1, zoom);
		map_pixel_class map_pixel_class2 = this.latlong_to_pixel2(latlong2, zoom);
		return this.pixel_to_latlong2(new map_pixel_class
		{
			x = (map_pixel_class.x + map_pixel_class2.x) / (double)2,
			y = (map_pixel_class.y + map_pixel_class2.y) / (double)2
		}, zoom);
	}

	public override void calc_latlong_area_from_center(map_area_class area, latlong_class center, double zoom, Vector2 resolution)
	{
		map_pixel_class map_pixel_class = this.latlong_to_pixel2(area.center, zoom);
		map_pixel_class map_pixel_class2 = this.latlong_to_pixel2(center, zoom);
		map_pixel_class map_pixel_class3 = this.latlong_to_pixel2(area.upper_left, zoom);
		map_pixel_class map_pixel_class4 = this.latlong_to_pixel2(area.lower_right, zoom);
		map_pixel_class map_pixel_class5 = new map_pixel_class();
		map_pixel_class5.x = map_pixel_class2.x - map_pixel_class.x;
		map_pixel_class5.y = map_pixel_class2.y - map_pixel_class.y;
		map_pixel_class3.x += map_pixel_class5.x;
		map_pixel_class3.y += map_pixel_class5.y;
		map_pixel_class4.x = map_pixel_class3.x + (double)resolution.x;
		map_pixel_class4.y = map_pixel_class3.y + (double)resolution.y;
		area.upper_left = this.pixel_to_latlong2(map_pixel_class3, zoom);
		area.lower_right = this.pixel_to_latlong2(map_pixel_class4, zoom);
		area.center = center;
	}

	public override void calc_latlong1_area_from_center(map_area_class area, latlong_class center, double zoom)
	{
		map_pixel_class map_pixel_class = this.latlong_to_pixel2(area.upper_left, zoom);
		map_pixel_class map_pixel_class2 = this.latlong_to_pixel2(center, zoom);
		map_pixel_class map_pixel_class3 = this.latlong_to_pixel2(area.center, zoom);
		map_pixel_class map_pixel_class4 = this.latlong_to_pixel2(area.lower_right, zoom);
		map_pixel_class map_pixel_class5 = new map_pixel_class();
		map_pixel_class5.x = map_pixel_class2.x - map_pixel_class.x;
		map_pixel_class5.y = map_pixel_class2.y - map_pixel_class.y;
		map_pixel_class3.x += map_pixel_class5.x;
		map_pixel_class3.y += map_pixel_class5.y;
		map_pixel_class4.x += map_pixel_class5.x;
		map_pixel_class4.y += map_pixel_class5.y;
		area.upper_left = center;
		area.center = this.pixel_to_latlong2(map_pixel_class3, zoom);
		area.lower_right = this.pixel_to_latlong2(map_pixel_class4, zoom);
	}

	public override void calc_latlong2_area_from_center(map_area_class area, latlong_class center, double zoom)
	{
		map_pixel_class map_pixel_class = this.latlong_to_pixel2(area.lower_right, zoom);
		map_pixel_class map_pixel_class2 = this.latlong_to_pixel2(center, zoom);
		map_pixel_class map_pixel_class3 = this.latlong_to_pixel2(area.center, zoom);
		map_pixel_class map_pixel_class4 = this.latlong_to_pixel2(area.upper_left, zoom);
		map_pixel_class map_pixel_class5 = new map_pixel_class();
		map_pixel_class5.x = map_pixel_class2.x - map_pixel_class.x;
		map_pixel_class5.y = map_pixel_class2.y - map_pixel_class.y;
		map_pixel_class3.x += map_pixel_class5.x;
		map_pixel_class3.y += map_pixel_class5.y;
		map_pixel_class4.x += map_pixel_class5.x;
		map_pixel_class4.y += map_pixel_class5.y;
		area.lower_right = center;
		area.center = this.pixel_to_latlong2(map_pixel_class3, zoom);
		area.upper_left = this.pixel_to_latlong2(map_pixel_class4, zoom);
	}

	public override Vector2 calc_pixel_zoom(Vector2 pixel, double zoom, double current_zoom, Vector2 screen_resolution)
	{
		double num = (double)Mathf.Pow((float)2, (float)(zoom - current_zoom));
		Vector2 a = pixel - screen_resolution;
		a *= (float)num;
		return a + screen_resolution;
	}

	public override latlong_area_class calc_latlong_area_by_tile(latlong_class latlong, tile_class tile, double zoom, int resolution, Vector2 bresolution, Vector2 offset)
	{
		float num = Mathf.Pow((float)2, (float)((double)19 - zoom));
		zoom = (double)19;
		resolution = (int)((float)resolution * num);
		bresolution *= num;
		latlong_area_class latlong_area_class = new latlong_area_class();
		map_pixel_class map_pixel_class = this.latlong_to_pixel2(latlong, zoom);
		Vector2 vector = new Vector2((float)0, (float)0);
		map_pixel_class.x += (double)((float)(tile.x * resolution) + offset.x);
		map_pixel_class.y += (double)((float)(tile.y * resolution) + offset.y);
		if (tile.x > 0)
		{
			map_pixel_class.x += (double)num;
			vector.x = num;
		}
		if (tile.y > 0)
		{
			map_pixel_class.y += (double)num;
			vector.y = num;
		}
		latlong_class latlong_class = this.pixel_to_latlong2(map_pixel_class, zoom);
		latlong_area_class.latlong1 = latlong_class;
		map_pixel_class.x += (double)(bresolution.x - vector.x);
		map_pixel_class.y += (double)(bresolution.y - vector.y);
		latlong_class = this.pixel_to_latlong2(map_pixel_class, zoom);
		latlong_area_class.latlong2 = latlong_class;
		return latlong_area_class;
	}

	public override latlong_area_class calc_latlong_area_by_tile2(latlong_class latlong, tile_class tile, double zoom, int resolution, Vector2 bresolution)
	{
		latlong_area_class latlong_area_class = new latlong_area_class();
		map_pixel_class map_pixel_class = this.latlong_to_pixel2(latlong, zoom);
		Vector2 vector = new Vector2((float)0, (float)0);
		map_pixel_class.x += (double)(tile.x * resolution);
		map_pixel_class.y += (double)(tile.y * resolution);
		latlong_class latlong_class = this.pixel_to_latlong2(map_pixel_class, zoom);
		latlong_area_class.latlong1 = latlong_class;
		map_pixel_class.x += (double)bresolution.x;
		map_pixel_class.y += (double)bresolution.y;
		latlong_class = this.pixel_to_latlong2(map_pixel_class, zoom);
		latlong_area_class.latlong2 = latlong_class;
		return latlong_area_class;
	}

	public override latlong_class calc_latlong_center_by_tile(latlong_class latlong, tile_class tile, tile_class subtile, tile_class subtiles, double zoom, int resolution, Vector2 offset)
	{
		float num = Mathf.Pow((float)2, (float)((double)19 - zoom));
		zoom = (double)19;
		resolution = (int)((float)resolution * num);
		latlong_class latlong_class = new latlong_class();
		map_pixel_class map_pixel_class = this.latlong_to_pixel2(latlong, zoom);
		map_pixel_class.x += (double)(tile.x * subtiles.x * resolution + subtile.x * resolution);
		map_pixel_class.y += (double)(tile.y * subtiles.y * resolution + subtile.y * resolution);
		map_pixel_class.x += (double)((float)(resolution / 2) + offset.x);
		map_pixel_class.y += (double)((float)(resolution / 2) + offset.y);
		return this.pixel_to_latlong2(map_pixel_class, zoom);
	}

	public override int calc_rest_value(float value1, float divide)
	{
		int num = (int)(value1 / divide);
		return (int)(value1 - (float)num * divide);
	}

	public override map_pixel_class calc_latlong_to_mercator(latlong_class latlong)
	{
		map_pixel_class map_pixel_class = new map_pixel_class();
		map_pixel_class.x = latlong.latitude * (double)20037508f / (double)180;
		map_pixel_class.y = (double)(Mathf.Log(Mathf.Tan((float)(((double)90 + latlong.longitude) * (double)3.14159274f / (double)360))) / 0.0174532924f);
		map_pixel_class.y = map_pixel_class.y * (double)20037508f / (double)180;
		return map_pixel_class;
	}

	public override latlong_class calc_mercator_to_latlong(map_pixel_class pixel)
	{
		latlong_class latlong_class = new latlong_class();
		latlong_class.longitude = pixel.x / (double)20037508f * (double)180;
		latlong_class.latitude = pixel.y / (double)20037508f * (double)180;
		latlong_class.latitude = (double)(57.2957764f * ((float)2 * Mathf.Atan(Mathf.Exp((float)(latlong_class.latitude * (double)3.14159274f / (double)180))) - 1.57079637f));
		return latlong_class;
	}

	public override bool rect_contains(Rect rect1, Rect rect2)
	{
		return rect1.Contains(new Vector2(rect2.x, rect2.y)) || rect1.Contains(new Vector2(rect2.x, rect2.yMax)) || rect1.Contains(new Vector2(rect2.xMax, rect2.y)) || rect1.Contains(new Vector2(rect2.xMax, rect2.yMax));
	}

	public override tile_class calc_terrain_tile(int terrain_index, tile_class tiles)
	{
		tile_class tile_class = new tile_class();
		tile_class.y = terrain_index / tiles.x;
		tile_class.x = terrain_index - tile_class.y * tiles.x;
		return tile_class;
	}

	public override void Main()
	{
	}
}
